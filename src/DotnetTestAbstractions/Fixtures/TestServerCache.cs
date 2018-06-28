using System;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace DotnetTestAbstractions.Fixtures
{
    public static class TestServerCache
    {
        private static ConcurrentDictionary<Type, TestServer> _servers
            = new ConcurrentDictionary<Type, TestServer>();

        public static TestServer GetOrCreateServer<TStartup>(bool forceRefresh = false)
        {
            var startupType = typeof(TStartup);

            if (_servers.TryGetValue(startupType, out var server) && forceRefresh == false)
                return server;

            var builder = new WebHostBuilder().UseStartup(startupType);
            var newServer = new TestServer(builder);
            _servers[startupType] = newServer;

            return newServer;
        }
    }
}