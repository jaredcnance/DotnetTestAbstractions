using System;
using Microsoft.EntityFrameworkCore;

namespace DotnetTestAbstractions.Fixtures
{
    /// <summary>
    /// The base fixture that should be used for any test using a <see cref="DbContext" />
    /// </summary>
    public class BaseDbContextFixture<TContext>
        : BaseFixture, IDisposable
        where TContext : DbContext
    {
        private static bool _databaseIsCreated = false;
        private static object _dbCreatedLock = new object();

        protected void EnsureDatabaseIsCreated(TContext dbContext)
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

        /// <summary>
        /// Create the database. 
        /// By default, this will use <c>EnsureCreated</c>.
        /// If you need to use migrations, you should override this method.
        /// </summary>
        protected virtual void CreateDatabase(TContext dbContext) 
            => dbContext.Database.EnsureCreated();

        /// <summary>
        /// Drops the database prior to starting the test.
        /// </summary>
        protected virtual void DropDatabase(TContext dbContext)
        {
            if (ShouldDropDatabase())
                dbContext.Database.EnsureDeleted();
        }

        /// <summary>
        /// Whether or not to drop the database before tests run.
        /// By default, this only happens if the environment variable
        /// DROP_DATABASE_ONSTART == true
        /// </summary>
        protected virtual bool ShouldDropDatabase()
            => Environment.GetEnvironmentVariable("DROP_DATABASE_ONSTART") == "true";

        public virtual void Dispose() { }
    }
}