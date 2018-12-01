using System;
using System.Net.Http;
using DotnetTestAbstractions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetTestAbstractions.Fixtures
{
    public class TestServerFixture<TStartup, TContext>
        : BaseFixture
        where TStartup : class
        where TContext : DbContext
    {
        private IServiceProvider _services;
        private TestServer _currentServer;

        public TestServerFixture()
        {
            SetupScopedServer();
            DbContext = GetService<TContext>();
        }

        protected HttpClient Client { get; set; }
        protected TContext DbContext { get; private set; }
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
        protected void SetupScopedServer(bool forceRefresh = false)
        {
            _currentServer = TestServerCache.GetOrCreateServer<TStartup>(forceRefresh);
            Client = _currentServer.CreateClient();
            _services = _currentServer.Host.Services.CreateScope().ServiceProvider;
        }

        /// <summary>
        /// List of types that when requested should be resolved from the ambient scope
        /// instead of the child scope. These should be interfaces of instances that need to be shared
        /// between the test and the web application.
        /// </summary>
        protected virtual List<ServiceDescriptor> GetAmbientTypes()
            => new List<ServiceDescriptor> { new ServiceDescriptor(typeof(TContext), typeof(TContext), ServiceLifetime.Scoped) };

        public void Dispose()
        {
            DbContext.Database.CurrentTransaction.Rollback();
            Client.Dispose();
        }
    }
}
