using System;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetTestAbstractions.DependencyInjection
{
    public class ServiceScope : IServiceScope
    {
        public ServiceScope(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
            ServiceProvider = new ScopedServiceProvider(lifetimeScope);
        }

        private readonly ILifetimeScope _lifetimeScope;

        public IServiceProvider ServiceProvider { get; }

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