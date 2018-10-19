using System.Data.Common;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace DotnetTestAbstractions.DependencyInjection.Data
{
    public interface IConnectionConfigurator
    {
        DbContextOptionsBuilder<TContext> ConfigureConnection<TContext>(DbConnection connection) 
            where TContext : DbContext;
    }

    public class SqlServerConnectionConfigurator : IConnectionConfigurator
    {
        public DbContextOptionsBuilder<TContext> ConfigureConnection<TContext>(DbConnection connection) 
            where TContext : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();

            var assembly = typeof(TContext).Assembly
                    .GetReferencedAssemblies()
                    .Single(a => a.Name.Contains("Microsoft.EntityFrameworkCore.SqlServer"));
                
            var loadedAssembly = Assembly.Load(assembly.Name);
            var type = loadedAssembly.DefinedTypes.Single(t => t.Name == "SqlServerDbContextOptionsExtensions");

            var useSqlServerMethods = type
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(mi => mi.Name == "UseSqlServer");

            MethodInfo method = null;
            foreach (var methodInfo in useSqlServerMethods) {
                if(methodInfo.GetGenericArguments().Count() == 1) {
                    method = methodInfo;
                }
            }

            var useSqlServer = method.MakeGenericMethod(typeof(TContext));
            optionsBuilder = (DbContextOptionsBuilder<TContext>) useSqlServer.Invoke(null, new object[] { optionsBuilder, connection, null });

            return optionsBuilder;
        }
    }
}