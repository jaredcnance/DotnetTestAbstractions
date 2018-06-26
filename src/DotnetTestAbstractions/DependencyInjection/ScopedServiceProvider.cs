using System;
using Autofac;

namespace DotnetTestAbstractions.DependencyInjection
{
    public class ScopedServiceProvider : IServiceProvider
    {
        private readonly ILifetimeScope _lifetimeScope;

        public ScopedServiceProvider(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public object GetService(Type serviceType) => _lifetimeScope.Resolve(serviceType);
    }
}
