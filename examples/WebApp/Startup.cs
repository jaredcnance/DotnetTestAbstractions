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
using Microsoft.Extensions.Configuration;

namespace WebApp
{
    public class Startup
    {
        private readonly IConfiguration _config;

        public Startup(IConfiguration config)
        {
            _config = config;
        }

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(new DbContextOptionsBuilder<AppContext>()
                    .UseSqlServer(_config["Data:DefaultConnection"])
                    .Options);

            services.AddScoped<AppContext>();
            return services.BuildServiceProvider();
        }

        public void Configure(
            IApplicationBuilder app,
            AppContext context)
        {
            app.UseMvc();
        }
    }
}
