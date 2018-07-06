using System;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace DotnetTestAbstractions.DependencyInjection
{
    public class ServiceScope : IServiceScope, IServiceProvider
    {
        private readonly ILifetimeScope _ambientScope;
        private readonly Dictionary<Type, Type> _ambientTypes;
        private readonly ILifetimeScope _childScope;
        private bool _isChildScope => _childScope != null;

        public ServiceScope(
            ILifetimeScope ambientScope,
            Dictionary<Type, Type> ambientTypes,
            ILifetimeScope childScope = null)
        {
            _ambientScope = ambientScope;
            _ambientTypes = ambientTypes ?? new Dictionary<Type, Type>();
            _childScope = childScope;
            ServiceProvider = this;
        }

        public IServiceProvider ServiceProvider { get; }

        public object GetService(Type serviceType)
        {
            if (_isChildScope && _ambientTypes.ContainsKey(serviceType) == false)
            {
                Console.WriteLine(">>>> Child Resolve " + serviceType);
                return _childScope.Resolve(serviceType);
            }

            Console.WriteLine(">>>> Ambient Resolve " + serviceType);
            return _ambientScope.Resolve(serviceType);
        }


        public ServiceScope CreateChildScope()
            => new ServiceScope(_ambientScope, _ambientTypes, _ambientScope.BeginLifetimeScope());

        /// <summary>
        /// This method is a no-op to prevent the test app from disposing the
        /// scope prematurely.
        /// If the scope needs to be disposed, call <see cref="ActuallyDispose" />
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Dispose the container.
        /// </summary>
        public void ActuallyDispose()
        {
            if (_isChildScope)
            {
                Console.WriteLine(">>>> Child Dispose");
                _childScope.Dispose();
            }

            else
            {
                Console.WriteLine(">>>> Ambient Dispose");
                _ambientScope.Dispose();
            }
        }
    }
}
