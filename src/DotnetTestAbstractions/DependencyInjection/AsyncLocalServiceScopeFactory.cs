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

            return CreateAmbientScope();
        }

        /// <summary>
        /// Creates a new ambient scope to replace the existing scope.
        /// The existing scope will be disposed.
        /// </summary>
        public static ServiceScope CreateAmbientScope()
        {
            var existingScopes = _asyncLocalScopes.Value;
            existingScopes?.ForEach(s => s.Dispose());

            var lifetimeScope = _container.BeginLifetimeScope();
            var scope = new ServiceScope(lifetimeScope);
            _asyncLocalScopes.Value = new List<ServiceScope>() { scope };

            return scope;
        }

        /// <summary>
        /// Creates a child scope to be used with each request.
        /// </summary>
        public static ServiceScope CreateChildScope()
        {
            if (_asyncLocalScopes.Value == null || _asyncLocalScopes.Value.Count == 0)
                throw new InvalidOperationException("Cannot create child scope without an ambient scope. Make sure you have called CreateAmbientScope() first.");

            if (_asyncLocalScopes.Value.Count > 1)
                throw new InvalidOperationException("Cannot create child scope, one already exists. Call dispose on the child scope before creating a new one.");

            var parentScope = _asyncLocalScopes.Value.Single();
            var childScope = parentScope.CreateChildScope();
            _asyncLocalScopes.Value.Add(childScope);

            return childScope;
        }
    }
}