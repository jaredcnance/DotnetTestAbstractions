using System;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace DotnetTestAbstractions.DependencyInjection
{
    public class DbContextInterceptor<TContext> : IServiceInterceptor 
        where TContext : DbContext
    {
        private static bool _databaseIsCreated = false;
        private static object _dbCreatedLock = new object();
        private static AsyncLocal<IDbContextTransaction> _currentTransaction = new AsyncLocal<IDbContextTransaction>();
        private static AsyncLocal<DbConnection> _currentConnection = new AsyncLocal<DbConnection>();

        public object OnResolving(Object service)
        {
            if(service is TContext dbContext)
                return SetupDbContext(dbContext);

            return service;
        }

        protected virtual TContext SetupDbContext(TContext dbContext)
        {
            dbContext = EnsureDatabaseConnection(dbContext);
            EnsureDatabaseIsCreated(dbContext);
            EnsureTransaction(dbContext);
            return dbContext;
        }

        protected virtual TContext EnsureDatabaseConnection(TContext dbContext) 
        {
            if(_currentConnection.Value == null)
            {
                _currentConnection.Value = dbContext.Database.GetDbConnection();
            }
            else
            {
                var options = new DbContextOptionsBuilder<TContext>()
                    .UseSqlServer((SqlConnection)_currentConnection.Value)
                    .Options;

                dbContext = (TContext)Activator.CreateInstance(typeof(TContext), options);
            }

            return dbContext;
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