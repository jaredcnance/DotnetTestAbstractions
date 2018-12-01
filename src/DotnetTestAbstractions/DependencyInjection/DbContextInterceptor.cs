using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading;
using Autofac;
using DotnetTestAbstractions.DependencyInjection.Data;
using DotnetTestAbstractions.Fixtures;
using DotnetTestAbstractions.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DotnetTestAbstractions.DependencyInjection
{
    public class DbContextInterceptor<TContext> : IServiceInterceptor 
        where TContext : Microsoft.EntityFrameworkCore.DbContext
    {
        private static bool _databaseIsCreated = false;
        private static object _dbCreatedLock = new object();
        private static AsyncLocal<IDbContextTransaction> _currentTransaction = new AsyncLocal<IDbContextTransaction>();
        private static AsyncLocal<DbConnection> _currentConnection = new AsyncLocal<DbConnection>();
        private readonly DatabaseProvider _provider;

        public DbContextInterceptor(DatabaseProvider provider = DatabaseProvider.SqlServer)
        {
            _provider = provider;
        }

        public object OnResolving(Object service, IComponentContext context)
        {
            if(service is TContext dbContext)
                return SetupDbContext(dbContext, context);

            return service;
        }

        protected virtual TContext SetupDbContext(TContext dbContext, IComponentContext context)
        {
            try {
                dbContext = EnsureDatabaseConnection(dbContext, context);
                EnsureDatabaseIsCreated(dbContext);
                EnsureTransaction(dbContext);
                return dbContext;
            }
            catch (Exception e) {
                Logger.Error($"Something wen wrong While constructing the DbContext. {e.Message}");
                throw;
            }
        }

        protected virtual TContext EnsureDatabaseConnection(TContext dbContext, IComponentContext context) 
        {
            // use the pre-existing connection, or create a new DbContext instance and set the connection
            if(_currentConnection.Value == null)
                _currentConnection.Value = dbContext.Database.GetDbConnection();
            else
                dbContext = ConstructDbContextInstance(context);

            return dbContext;
        }

        // TODO: most of this can be cached and should be moved to a factory
        private TContext ConstructDbContextInstance(IComponentContext context)
        {
            // we need to find a constructor for the DbContext
            var dbContextType = typeof(TContext);
            var orderedConstructors = dbContextType.GetConstructors().OrderBy(c => c.GetParameters().Length);

            List<object> targetCtorArgs = null;
            foreach(var ctor in orderedConstructors)
            {
                if(ShouldUseConstructor(ctor, context, out var args))
                    targetCtorArgs = args;
            }

            if(targetCtorArgs == null)
                throw new DotnetTestAbstractionsFixtureException($"Could not locate a valid constructor for {dbContextType}");
            
            var dbContextOptions = GetConfiguredDbContextOptions();
            targetCtorArgs.Insert(0, dbContextOptions);
            return (TContext)Activator.CreateInstance(dbContextType, targetCtorArgs.ToArray());
        }

        // gets DbOptions that are configured to use the existing transaction
        private DbContextOptions GetConfiguredDbContextOptions() 
            => ConnectionConfiguratorFactory.Create(_provider)
                .ConfigureConnection<TContext>(_currentConnection.Value)
                .Options;

        private bool ShouldUseConstructor(ConstructorInfo ctor, IComponentContext context, out List<object> args)
        {
            args = null;

            // the Dbcontext MUST have a constructor that accepts DbContextOptions
            if(ctor.GetParameters().Any(p => p.ParameterType == typeof(DbContextOptions<TContext>)) == false)
                return false;

            foreach(var param in ctor.GetParameters()) 
            {
                args = new List<object>();

                var paramInstance = context.ResolveOptional(param.ParameterType);
                if(paramInstance == null)
                    return false;

                args.Add(paramInstance);
            }

            return true;
        }

        protected void EnsureDatabaseIsCreated(DbContext dbContext)
        {
            if (_databaseIsCreated == false)
            {
                lock (_dbCreatedLock)
                {
                    DropDatabase(dbContext);
                    CreateDatabase(dbContext);
                    _databaseIsCreated = true;
                }
            }
        }

        protected virtual void EnsureTransaction(TContext dbContext) 
        {
            if(_currentTransaction.Value == null)
            {
                _currentTransaction.Value = dbContext.Database.BeginTransaction();
            }
            else 
            {
                dbContext.Database.UseTransaction(_currentTransaction.Value.GetDbTransaction());
            }
        }

         /// <summary>
        /// Drops the database prior to starting the test.
        /// </summary>
        protected virtual void DropDatabase(DbContext dbContext)
        {
            if (ShouldDropDatabase())
                dbContext.Database.EnsureDeleted();
        }

        /// <summary>
        /// Create the database. 
        /// By default, this will use <c>EnsureCreated</c>.
        /// If you need to use migrations, you should override this method.
        /// </summary>
        protected virtual void CreateDatabase(DbContext dbContext) 
            => dbContext.Database.EnsureCreated();

        /// <summary>
        /// Whether or not to drop the database before tests run.
        /// By default, this only happens if the environment variable
        /// DROP_DATABASE_ONSTART == true
        /// </summary>
        protected virtual bool ShouldDropDatabase()
            => Environment.GetEnvironmentVariable("DROP_DATABASE_ONSTART") == "true";
    }
}