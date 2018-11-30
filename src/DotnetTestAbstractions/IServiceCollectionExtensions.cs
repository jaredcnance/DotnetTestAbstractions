using System;
using DotnetTestAbstractions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DotnetTestAbstractions
{
    public static class IServiceCollectionExtensions
    {
        [Obsolete("Use ServiceProviderFactory directly")]
        public static IServiceProvider UseAmbientDbConnection<TContext>(this IServiceCollection services)
            where TContext : DbContext => ServiceProviderFactory.CreateServiceProvider<TContext>(services);
    }
}