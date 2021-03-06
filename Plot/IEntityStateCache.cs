﻿using System;
using System.Collections.Generic;

namespace Plot
{
    public interface IEntityStateCache : IDisposable
    {
        EntityState Create(object proxy);

        EntityState Get(object proxy);

        bool Contains(object proxy);

        void Remove(object proxy);

        IEnumerable<EntityState> State { get; }
    }
}
