using System;

namespace DotnetTestAbstractions.DependencyInjection.Data
{
    public static class ConnectionConfiguratorFactory
    {
        public static IConnectionConfigurator Create(DatabaseProvider provider)
        {
            switch(provider)
            {
                case DatabaseProvider.SqlServer:
                    return new SqlServerConnectionConfigurator();
                default:
                    throw new ArgumentException();
            }
        }
    }
}