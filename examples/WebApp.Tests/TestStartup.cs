using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using AppContext = WebApp.Data.AppContext;

using Autofac;
using Autofac.Extensions.DependencyInjection;

using DotnetTestAbstractions.DependencyInjection;

namespace WebApp.Tests
{
    public class TestStartup : Startup
    {
        public static IContainer Container => Startup.Container;

        public override IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IServiceScopeFactory, AsyncLocalServiceScopeFactory<TestStartup>>();

            return base.ConfigureServices(services);
        }
    }
}