using System.Data.Common;
using System.Threading;
using DotnetTestAbstractions.Utilities;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DotnetTestAbstractions.Database
{
    static class ScopedConnection
    {
        public static AsyncLocal<IDbContextTransaction> CurrentTransaction = new AsyncLocal<IDbContextTransaction>();
        public static AsyncLocal<DbConnection> CurrentConnection = new AsyncLocal<DbConnection>();

        public static void BeginTransaction(DatabaseFacade database)
        {
            Logger.Debug("BeginTransaction>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
            ScopedConnection.CurrentTransaction.Value = database.BeginTransaction();
            Logger.Debug("Started " + ScopedConnection.CurrentTransaction.Value.TransactionId);
        }

        public static void Rollback()
        {
            CurrentTransaction.Value.Rollback();
            ScopedConnection.CurrentConnection.Value.Close();

            Logger.Debug("Rolled back transaction " + ScopedConnection.CurrentTransaction.Value?.TransactionId);
            ScopedConnection.CurrentTransaction.Value = null;
            ScopedConnection.CurrentConnection.Value = null;
            Logger.Debug("<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<EndTransaction");
        }
    }
}