using System;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetTestAbstractions.DependencyInjection
{
    public class ServiceScopeFactory
    {
        public readonly AutofacServiceProvider ServiceProvider;

        public ServiceScopeFactory(IServiceCollection services, IServiceInterceptor interceptor)
        {
            var containerBuilder = new ContainerBuilder();
            
            containerBuilder.Populate(services);
            containerBuilder.RegisterModule(new InterceptorModule(interceptor));

            var container = containerBuilder.Build();
            ServiceProvider = new AutofacServiceProvider(container);
        }
    }
}