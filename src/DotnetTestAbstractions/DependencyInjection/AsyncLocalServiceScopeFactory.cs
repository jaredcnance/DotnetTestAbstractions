using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetTestAbstractions.DependencyInjection
{
    public class AsyncLocalServiceScopeFactory<TStartup> : IServiceScopeFactory
    {
        private static AsyncLocal<List<ServiceScope>> _asyncLocalScopes = new AsyncLocal<List<ServiceScope>>();
        private static readonly IContainer _container
            = StartupConventionLoader.GetStaticField<IContainer>(typeof(TStartup), StartupConventions.CONTAINER_FIELD_NAME);

        /// <summary>
        /// This method will be called by the test server.
        /// Any new scopes the server tries to create, will receive the existing
        /// ambient scope. If a new ambient scope is required, you should call
        /// CreateAmbientScope instead.
        /// </summary>
        public IServiceScope CreateScope()
        {
            if (_asyncLocalScopes.Value != null)
            {
                return _asyncLocalScopes.Value.Last();
            }

            return CreateAmbientScope(new Dictionary<Type, Type>());
        }

        /// <summary>
        /// Creates a new ambient scope to replace the existing scope.
        /// The existing scope will be disposed.
        /// </summary>
        public static ServiceScope CreateAmbientScope(Dictionary<Type, Type> ambientScopedRegistrations)
        {
            var existingScopes = _asyncLocalScopes.Value;
            existingScopes?.ForEach(s => s.Dispose());

            var lifetimeScope = _container.BeginLifetimeScope(b => RegisterAmbientScopedServices(b, ambientScopedRegistrations));
            var scope = new ServiceScope(lifetimeScope, ambientScopedRegistrations);
            _asyncLocalScopes.Value = new List<ServiceScope>() { scope };

            return scope;
        }

        private static void RegisterAmbientScopedServices(ContainerBuilder b, Dictionary<Type, Type> ambientScopedRegistrations)
        {
            foreach (var registration in ambientScopedRegistrations)
                b.RegisterType(registration.Value)
                    .As(registration.Key)
                    .InstancePerLifetimeScope();
        }

        /// <summary>
        /// Creates a child scope to be used with each request.
        /// </summary>
        public static ServiceScope CreateChildScope()
        {
            if (_asyncLocalScopes.Value == null || _asyncLocalScopes.Value.Count == 0)
                throw new InvalidOperationException("Cannot create child scope without an ambient scope. Make sure you have called CreateAmbientScope() first.");

            if (_asyncLocalScopes.Value.Count > 1)
            {
                Console.WriteLine("Disposing Existing Child Scope.");
                var currentScope = _asyncLocalScopes.Value[1];
                _asyncLocalScopes.Value.Remove(currentScope);
                currentScope.ActuallyDispose();
            }

            var parentScope = _asyncLocalScopes.Value.Single();
            var childScope = parentScope.CreateChildScope();
            _asyncLocalScopes.Value.Add(childScope);

            return childScope;
        }
    }
}
