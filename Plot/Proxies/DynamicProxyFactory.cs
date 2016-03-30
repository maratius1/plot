﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Plot.Logging;
using Plot.Metadata;

namespace Plot.Proxies
{
    public class DynamicProxyFactory : IProxyFactory
    {
        private readonly IMetadataFactory _metadataFactory;

        private readonly ILogger _logger;

        private readonly ProxyGenerator _generator;

        public DynamicProxyFactory(IMetadataFactory metadataFactory, ILogger logger)
        {
            _metadataFactory = metadataFactory;
            _logger = logger;
            _generator = new ProxyGenerator();
        }

        public T Create<T>(T item, IGraphSession session, EntityStatus status = EntityStatus.Clean) where T : class
        {
            using (Timer.Start("Proxy Creation", _logger))
            {
                var generator = new Generator(session, _metadataFactory, _generator, status);
                return generator.Create(item);
            }
        }

        private class Generator
        {
            private readonly ProxyGenerator _generator;

            private readonly IGraphSession _session;

            private readonly IMetadataFactory _metadataFactory;

            private readonly ProxyGenerationOptions _options;

            private readonly IEntityStateCache _state;

            private readonly EntityStatus _status;

            private readonly Stack<TraceKey> _callStack;

            public Generator(IGraphSession session, IMetadataFactory metadataFactory, ProxyGenerator generator, EntityStatus status = EntityStatus.Clean)
            {
                _generator = generator;
                _session = session;
                _metadataFactory = metadataFactory;
                _options = new ProxyGenerationOptions(new ProxyGenerationHook());
                _state = session.State;
                _status = status;
                _callStack = new Stack<TraceKey>();
            }

            public T Create<T>(T item)
            {
                var proxy = (T)Create(typeof(T), item);
                if (proxy is IRequireSession)
                {
                    ((IRequireSession) proxy).Set(_session);
                }
                return proxy;
            }

            private object Create(Type type, object item)
            {
                ProxyUtils.SetEntityId(item);
                var id = ProxyUtils.GetEntityId(item);
                var key = new TraceKey(id, type);
                if (IsBusyCreatingProxy(key))
                {
                    return _session.Uow.Get(id, type);
                }
                _callStack.Push(key);
                var proxy = ProxyUtils.IsProxy(item) ? item : NewProxy(type, item);
                var state = GetState(proxy);
                state.Lock();
                _session.Register(proxy, state);
                Populate(proxy);
                state.Set(_status);
                state.Unlock();
                _callStack.Pop();
                return proxy;
            }

            private object NewProxy(Type type, object source)
            {
                var interceptors = new IInterceptor[]
                {
                    new EntityStateInterceptor(_state),
                    new RelationshipInterceptor(_metadataFactory, _state)
                };
                var proxy = _generator.CreateClassProxy(type, _options, interceptors);
                foreach (var property in ProxyUtils.GetTargetEntityType(source).GetProperties())
                {
                    if (property.SetMethod == null)
                    {
                        continue;
                    }
                    var value = property.GetMethod.Invoke(source, null);
                    if (value == null)
                    {
                        continue;
                    }
                    if (ProxyUtils.IsProxy(value))
                    {
                        continue;
                    }
                    property.SetMethod.Invoke(proxy, new[] { value });
                }
                return proxy;
            }

            private void Populate(object item)
            {
                var type = ProxyUtils.GetTargetType(item);
                var metadata = _metadataFactory.Create(type);
                var properties = type.GetProperties();
                foreach (var property in properties)
                {
                    var propertyMetadata = metadata[property.Name];
                    if (propertyMetadata.IsPrimitive)
                    {
                        continue;
                    }
                    if (propertyMetadata.IsIgnored)
                    {
                        continue;
                    }
                    var value = property.GetValue(item);
                    if (value == null)
                    {
                        continue;
                    }
                    var proxy = Factory(propertyMetadata)(metadata, property, value, item);
                    property.SetValue(item, proxy);
                }
            }

            private Func<NodeMetadata, PropertyInfo, object, object, object> Factory(PropertyMetadata property)
            {
                if (property.IsList)
                {
                    return List;
                }
                return Entity;
            }

            private object List(NodeMetadata metadata, PropertyInfo property, object item, object parent)
            {
                var type = property.PropertyType;
                var genericType = typeof (TrackableCollection<>).MakeGenericType(type.GenericTypeArguments[0]);
                var items = ProxyListItems(((IEnumerable<object>)item).ToList());
                var args = new[] { parent, metadata[property.Name].Relationship, items, _state};
                var proxy = Activator.CreateInstance(genericType, args, null);
                return proxy;
            }

            private object Entity(NodeMetadata metadata, PropertyInfo property, object item, object parent)
            {
                var type = ProxyUtils.GetTargetType(item);
                var entity = _session.Uow.Get(ProxyUtils.GetEntityId(item), type) ?? item;
                return Create(type, entity);
            }

            private List<object> ProxyListItems(IList<object> source)
            {
                var destination = new List<object>();
                for (int i = 0; i < source.Count; i++)
                {
                    var type = ProxyUtils.GetTargetType(source[i]);
                    var existing = _session.Uow.Get(ProxyUtils.GetEntityId(source[i]), type);
                    var child = existing ?? Create(type, source[i]);
                    destination.Add(child);
                }
                return destination;
            }
            
            private EntityState GetState(object proxy)
            {
                return _state.Contains(proxy) ? _state.Get(proxy) : _state.Create(proxy);
            }

            private bool IsBusyCreatingProxy(TraceKey key)
            {
                return _callStack.Contains(key);
            }

            private class TraceKey
            {
                public TraceKey(string id, Type type)
                {
                    Id = id;
                    Type = type;
                }

                public string Id { get;}

                public Type Type { get; }

                public override int GetHashCode()
                {
                    return Id.GetHashCode() ^ Type.GetHashCode();
                }

                public override bool Equals(object obj)
                {
                    var other = obj as TraceKey;
                    if (other == null)
                    {
                        return false;
                    }
                    return GetHashCode() == other.GetHashCode();
                }
            }
        }
    }
}
