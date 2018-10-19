using System;
using Microsoft.Extensions.DependencyInjection;
using Autofac;
using DotnetTestAbstractions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using AppContext = WebApp.Data.AppContext;
using DotnetTestAbstractions;

namespace WebApp.Tests
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration config) : base(config) { }

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
            return services.UseAmbientDbConnection<AppContext>();
        }
    }
}