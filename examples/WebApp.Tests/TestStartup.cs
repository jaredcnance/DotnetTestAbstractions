using System;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using DotnetTestAbstractions.DependencyInjection;

namespace WebApp.Tests
{
    public class TestStartup : Startup
    {
        public static new IContainer Container => Startup.Container;

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            // this needs to be added to override the default scope creation mechanism
            services.AddSingleton<IServiceScopeFactory, AsyncLocalServiceScopeFactory<TestStartup>>();

            return base.ConfigureServices(services);
        }
    }
}