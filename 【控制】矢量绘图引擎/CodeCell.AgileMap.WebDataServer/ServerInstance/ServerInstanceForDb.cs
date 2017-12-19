using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.WebDataServer
{
    internal class ServerInstanceForDb : ServerInstanceBase
    {
        private Dictionary<string, string> _datatables = new Dictionary<string, string>();

        public ServerInstanceForDb(InstanceIdentify instanceIdentify, ICatalogProvider catalogProvider,string args)
            : base(instanceIdentify, catalogProvider,args)
        {
        }

        private object lockObj = new object();
        public override IFeaturesReaderService GetFeaturesReaderService(string fetclassId)
        {
            lock (lockObj)
            {
                string tablename = null;
                if (_datatables.ContainsKey(fetclassId))
                    tablename = _datatables[fetclassId];
                else
                {
                    tablename = GetTablenameById(fetclassId);
                    _datatables.Add(fetclassId, tablename);
                }
                if (tablename == null)
                    throw new Exception("从实例\"" + _id.Name + "\"获取要素类\"" + fetclassId + "\"读取服务失败。");
                return new FeaturesReaderServiceDb(_args, tablename);
            }
        }

        private string GetTablenameById(string fetclassId)
        {
            waitLine:
            using (IDbConnection dbConn = DbConnectionFactory.CreateDbConnection(_args))
            {
                if (dbConn == null)
                    goto waitLine;
                dbConn.Open();
                using (IDbCommand cmd = dbConn.CreateCommand())
                {
                    cmd.CommandText = "select datatable from " + BudGISMetadata.cstFeatureClassTableName + " where id='" + fetclassId + "'";
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            string dt = dr.IsDBNull(0) ? null : dr.GetString(0);
                            return dt;
                        }
                    }
                }
            }
            return null;
        }
    }
}
