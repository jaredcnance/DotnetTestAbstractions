using System;
using System.Collections.Concurrent;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace DotnetTestAbstractions.Fixtures
{
    public static class TestServerCache
    {
        private static ConcurrentDictionary<Type, TestServer> _servers
            = new ConcurrentDictionary<Type, TestServer>();

        public static TestServer GetOrCreateServer<TStartup>(bool forceRefresh = false, Action<WebHostBuilder> configureBuilder = null)
        {
            var startupType = typeof(TStartup);

            if (_servers.TryGetValue(startupType, out var server) && forceRefresh == false)
                return server;

            var builder = new WebHostBuilder();

            if (configureBuilder != null)
                configureBuilder(builder);
            else
                GetDefaultWebHostBuilder(builder, startupType);

            var newServer = new TestServer(builder);
            _servers[startupType] = newServer;

            return newServer;
        }

        private static IWebHostBuilder GetDefaultWebHostBuilder(WebHostBuilder builder, Type startupType)
            => builder.UseConfiguration(
                    new ConfigurationBuilder()
                        .AddJsonFile("appsettings.json")
                        .AddEnvironmentVariables()
                        .Build()
                )
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup(startupType);
    }
}