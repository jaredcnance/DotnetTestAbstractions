using System;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetTestAbstractions.DependencyInjection
{
    public static class ServiceProviderFactory
    {
        public static IServiceProvider CreateServiceProvider<TContext>(IServiceCollection services)
            where TContext : DbContext
        {
            var serviceScopeFactory = new ServiceScopeFactory(services, new DbContextInterceptor<TContext>());
            return serviceScopeFactory.ServiceProvider;
        }

        public static IServiceProvider CreateServiceProvider<TContext>(ContainerBuilder containerBuilder)
            where TContext : DbContext
        {
            var serviceScopeFactory = new ServiceScopeFactory(containerBuilder, new DbContextInterceptor<TContext>());
            return serviceScopeFactory.ServiceProvider;
        }
    }
}