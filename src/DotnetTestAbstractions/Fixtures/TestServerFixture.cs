using System;
using System.Net.Http;
using DotnetTestAbstractions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;

namespace DotnetTestAbstractions.Fixtures
{
    public class TestServerFixture<TStartup, TContext>
        : BaseDbContextFixture<TContext>
        where TStartup : class
        where TContext : DbContext
    {
        private ServiceScope _testScope;
        private IServiceProvider _services;
        private TestServer _currentServer;

        public TestServerFixture()
        {
            SetupScopedServer();
            SetupDatabase();
        }

        protected HttpClient Client { get; set; }
        protected TContext DbContext { get; private set; }
        protected IDbContextTransaction Transaction { get; private set; }
        protected T GetService<T>() => (T)_services.GetService(typeof(T));

        /// <summary>
        /// Ensures that a TestServer instance is setup for this test.
        /// </summary>
        /// <param name="forceRefresh">
        /// Recreate the server and services even if it exists in cache already.
        /// The cached instance will be replaced with the new one.
        /// </param>
        /// <param name="configureBuilder">
        /// Use this to override the default WebHost configuration.
        /// </param>
        protected void SetupScopedServer(bool forceRefresh = false, Action<WebHostBuilder> configureBuilder = null)
        {
            _currentServer = TestServerCache.GetOrCreateServer<TStartup>(forceRefresh, configureBuilder);
            Client = _currentServer.CreateClient();
            _testScope = AsyncLocalServiceScopeFactory<TStartup>.CreateAmbientScope(GetAmbientTypes());
            _services = _testScope.ServiceProvider;
        }

        protected ServiceScope CreateRequestScope()
            => AsyncLocalServiceScopeFactory<TStartup>.CreateChildScope();


        /// <summary>
        /// List of types that when requested should be resolved from the ambient scope
        /// instead of the child scope. These should be interfaces of instances that need to be shared
        /// between the test and the web application.
        /// </summary>
        protected virtual Dictionary<Type, Type> GetAmbientTypes()
            => new Dictionary<Type, Type> { { typeof(TContext), typeof(TContext) } };

        private void SetupDatabase()
        {
            DbContext = GetService<TContext>();
            EnsureDatabaseIsCreated(DbContext);
            Transaction = DbContext.Database.BeginTransaction();
        }

        public override void Dispose()
        {
            Transaction.Rollback();
            _testScope.ActuallyDispose();
            Client.Dispose();
            base.Dispose();
        }
    }
}
