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

namespace WebApp
{
    public class Startup
    {
        public static IContainer Container { get; private set; }

        public virtual IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(new DbContextOptionsBuilder<AppContext>()
                    .UseSqlServer("Server=localhost; Database=WebApp; User Id=sa; Password=P@ssword1;MultipleActiveResultSets=True")
                    .Options);

            services.AddScoped<AppContext>();

            return CreateContainer(services);
        }

        protected IServiceProvider CreateContainer(IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);
            Container = containerBuilder.Build();
            return new AutofacServiceProvider(Container);
        }

        public void Configure(
            IApplicationBuilder app,
            AppContext context)
        {
            context.Database.EnsureCreated();
            app.UseMvc();
        }
    }
}
