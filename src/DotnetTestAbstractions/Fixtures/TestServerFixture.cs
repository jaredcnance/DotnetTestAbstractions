using System;
using System.Net.Http;
using DotnetTestAbstractions.DependencyInjection;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

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
        protected void SetupScopedServer(bool forceRefresh = false)
        {
            _currentServer = TestServerCache.GetOrCreateServer<TStartup>(forceRefresh);
            Client = _currentServer.CreateClient();
            _testScope = AsyncLocalServiceScopeFactory<TStartup>.CreateAmbientScope();
            _services = _testScope.ServiceProvider;
        }

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