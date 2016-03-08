﻿using System;
using Castle.DynamicProxy;

namespace Plot.Proxies
{
    public class EntityStateTrackerInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase) && EntityStateTracker.Contains(invocation.InvocationTarget))
            {
                EntityStateTracker.Get(invocation.InvocationTarget).Dirty();
            }

            if (invocation.InvocationTarget == null)
            {
                return;
            }

            invocation.Proceed();
        }
    }
}
