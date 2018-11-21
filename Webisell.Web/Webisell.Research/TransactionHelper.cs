using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Text;
using Webisell.Research.EF;

namespace Webisell.Research
{
    class TransactionHelper : IDisposable
    {
        private string _connectionString;
        private SqlTransaction _transaction;
        private SqlConnection _connection;

        public TransactionHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        internal delegate void TransactionalOperation<T1>(T1 val1);
        internal delegate void TransactionalOperation<T1, T2>(T1 val1, T2 val2);

        void OpenConnection()
        {
            if (_connection == null)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
                _transaction = _connection.BeginTransaction();
            }
        }

        public void RunInTransaction(TransactionalOperation<WebisellSqlServerDbContext> operation)
        {
            OpenConnection();
            // Run an EF Core command in the transaction
            var options = new DbContextOptionsBuilder<WebisellSqlServerDbContext>()
                .UseSqlServer(_connection)
                .Options;

            using (var context = new WebisellSqlServerDbContext(options))
            {
                context.Database.UseTransaction(_transaction);
                operation(context);
                context.SaveChanges();
            }
        }

        public void RunInTransaction(string sql)
        {
            OpenConnection();
            using (var command = _connection.CreateCommand())
            {
                command.CommandText = sql;
                command.Transaction = _transaction;
                command.ExecuteNonQuery();
            }
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Dispose()
        {            
            _transaction.Dispose();
            _connection.Dispose();
        }
    }
}
