using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;

namespace CodeCell.AgileMap.Core
{
    public class DbConnectionInstance:IDisposable
    {
        public IDbConnection DbConnection = null;
        public string ConnectionString = null;
        public bool IsBusy = false;
        //private bool _dataReaderIsFree = true;
        //internal static int cstWaitingInterval = 100;
        internal static int idx = 0;
        public int Index = idx++;

        public DbConnectionInstance(IDbConnection dbConn,string connectionString)
        {
            dbConn.Open();
            DbConnection = dbConn;
            ConnectionString = connectionString;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (DbConnection != null)
            {
                DbConnection.Dispose();
                DbConnection = null;
            }
        }

        public void Free()
        {
            IsBusy = false;
        }

        #endregion
    }

    public class DbConnectionPool : IDisposable
    {
        private readonly int cstMaxConnectionCount = 5;
        private readonly int cstWaitingInterval = 100;
        private Dictionary<string, List<DbConnectionInstance>> _dbConnInstances = new Dictionary<string, List<DbConnectionInstance>>();

        public DbConnectionPool(int maxCount)
        {
            cstMaxConnectionCount = maxCount;
        }

        public DbConnectionInstance GetDbConnection(string connectionString, bool waitingUntilUseable)
        {
            List<DbConnectionInstance> instances = null;
            lock (_dbConnInstances)
            {
                if (!_dbConnInstances.ContainsKey(connectionString))
                {
                    DbConnectionInstance instance = CreateNewInstance(connectionString);
                    instance.IsBusy = true;
                    instances = new List<DbConnectionInstance>();
                    instances.Add(instance);
                    _dbConnInstances.Add(connectionString, instances);
                    return instance;
                }
            }
            instances = _dbConnInstances[connectionString];
            lock (instances)
            {
                foreach (DbConnectionInstance instance in instances)
                {
                    if (!instance.IsBusy)
                    {
                        instance.IsBusy = true;
                        return instance;
                    }
                }
                //
                if (instances.Count < cstMaxConnectionCount)
                {
                    DbConnectionInstance instance = CreateNewInstance(connectionString);
                    instance.IsBusy = true;
                    instances.Add(instance);
                    return instance;
                }
            }
            //
            while (true && waitingUntilUseable)
            {
            waitingLine:
                int n = instances.Count;
                for (int i = 0; i < n; i++)
                {
                    if (!instances[i].IsBusy)
                    {
                        instances[i].IsBusy = true;
                        return instances[i];
                    }
                }
                Thread.Sleep(cstWaitingInterval);
                goto waitingLine;
            }
            return null;
        }

        private DbConnectionInstance CreateNewInstance(string connectionString)
        {
            return new DbConnectionInstance(DbConnectionFactory.CreateDbConnection(connectionString), connectionString);
        }

        public void FreeDbConnectionInstance(DbConnectionInstance instance)
        {
            instance.IsBusy = false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            foreach (string key in _dbConnInstances.Keys)
            {
                List<DbConnectionInstance> instances = _dbConnInstances[key];
                foreach (DbConnectionInstance ist in instances)
                    ist.Dispose();
                instances.Clear();
            }
            _dbConnInstances.Clear();
        }

        #endregion
    }
}
