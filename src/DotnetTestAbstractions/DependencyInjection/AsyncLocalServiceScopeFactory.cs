using System.Threading;
using Autofac;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetTestAbstractions.DependencyInjection
{
    public class AsyncLocalServiceScopeFactory<TStartup> : IServiceScopeFactory
    {
        private static AsyncLocal<ServiceScope> _asyncLocalScope = new AsyncLocal<ServiceScope>();
        private static readonly IContainer _container
            = StartupConventionLoader.GetStaticField<TStartup, IContainer>(StartupConventions.CONTAINER_FIELD_NAME);

        /// <summary>
        /// This method will be called by the test server.
        /// Any new scopes the server tries to create, will receive the existing
        /// ambient scope. If a new ambient scope is required, you should call
        /// CreateAmbientScope instead.
        /// </summary>
        public IServiceScope CreateScope()
        {
            if (_asyncLocalScope.Value != null)
            {
                return _asyncLocalScope.Value;
            }

            return CreateAmbientScope();
        }

        /// <summary>
        /// Creates a new ambient scope to replace the existing scope.
        /// The existing scope will be disposed.
        /// </summary>
        public static ServiceScope CreateAmbientScope()
        {
            var lifetimeScope = _container.BeginLifetimeScope();
            var scope = new ServiceScope(lifetimeScope);

            _asyncLocalScope.Value?.Dispose();
            _asyncLocalScope.Value = scope;

            return scope;
        }
    }
}