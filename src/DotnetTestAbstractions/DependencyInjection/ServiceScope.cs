using System;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetTestAbstractions.DependencyInjection
{
    public class ServiceScope : IServiceScope, IServiceProvider
    {
        private readonly ILifetimeScope _lifetimeScope;

        public ServiceScope(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
            ServiceProvider = this;
        }

        public IServiceProvider ServiceProvider { get; }
        public object GetService(Type serviceType) => _lifetimeScope.Resolve(serviceType);
        public ServiceScope CreateChildScope() => new ServiceScope(_lifetimeScope.BeginLifetimeScope());

        /// <summary>
        /// This method is a no-op to prevent the test app from disposing the 
        /// scope prematurely. 
        /// If the scope needs to be disposed, call <see cref="ActuallyDispose" />
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Dispose the container.
        /// </summary>
        public void ActuallyDispose() => _lifetimeScope.Dispose();
    }
}