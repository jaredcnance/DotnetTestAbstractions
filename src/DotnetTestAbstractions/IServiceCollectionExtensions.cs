using System;
using DotnetTestAbstractions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetTestAbstractions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceProvider UseAmbientDbConnection<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            var serviceScopeFactory = new ServiceScopeFactory(services, new DbContextInterceptor<TContext>());
            return serviceScopeFactory.ServiceProvider;
        }
    }
}