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

        public virtual void Dispose() { }
    }
}