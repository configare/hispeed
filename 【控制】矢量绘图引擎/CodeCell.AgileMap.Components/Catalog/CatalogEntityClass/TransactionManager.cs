using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CodeCell.AgileMap.Components
{
    internal static class TransactionManager
    {
        private static IDbTransaction _transaction = null;
        private static int _count = 0;

        public static void BeginTransaction(IDbConnection conn,IDbCommand command)
        {
            if (_transaction == null)
            {
                _transaction = conn.BeginTransaction();
                command.Transaction = _transaction;
            }
            _count++;
        }

        public static void Commit()
        {
            _count--;
            if (_count == 0)
            {
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public static void Rollback()
        {
            _count--;
            if (_count == 0)
            {
                _transaction.Rollback();
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }
}
