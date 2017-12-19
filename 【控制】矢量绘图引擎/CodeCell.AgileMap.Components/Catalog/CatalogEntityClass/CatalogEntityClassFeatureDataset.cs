using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class CatalogEntityClassFeatureDataset:CatalogEntityClassBase
    {
        public CatalogEntityClassFeatureDataset(string connString)
            : base(connString)
        { 
        }

        protected override string GetTableName()
        {
            return BudGISMetadata.cstFeatureDatasetTableName;
        }

        protected override ICatalogEntity NewCatalogEntity()
        {
            return new SpatialFeatureDataset();
        }

        public override void Delete(ICatalogEntity entity)
        {
            using (IDbCommand cmd = _dbConnection.CreateCommand())
            {
                string sql = null;
                TransactionManager.BeginTransaction(_dbConnection, cmd);
                try
                {
                    int ret = 0;
                    //delete tables
                    sql = "select datatable,annotable from " + BudGISMetadata.cstFeatureClassTableName + " where datasetid='" + entity.Id + "'";
                    cmd.CommandText = sql;
                    List<string> dropTables = new List<string>();
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string tname = dr.GetString(0);
                            string anntname = dr.GetString(1);
                            if(tname != null)
                                dropTables.Add(tname);
                            if(anntname != null)
                                dropTables.Add(anntname);
                        }
                    }
                    if (dropTables.Count > 0)
                    {
                        //check table is exist
                        for (int i = dropTables.Count - 1; i >= 0; i--)
                        {
                            sql = _adapter.GetTableIsExistSql(dropTables[i], _adapter.GetDataSource(_connString));
                            cmd.CommandText = sql;
                            object v = cmd.ExecuteScalar();
                            ret = int.Parse(v.ToString());
                            if (ret <1)// remove not existed table
                            {
                                dropTables.RemoveAt(i);
                            }
                        }
                        //drop tables
                        foreach (string table in dropTables)
                        {
                            sql = "drop table " + table;
                            cmd.CommandText = sql;
                            ret = cmd.ExecuteNonQuery();
                        }
                    }
                    //delete featureclass
                    sql = "delete from " + BudGISMetadata.cstFeatureClassTableName + " where datasetid='" + entity.Id + "'";
                    cmd.CommandText = sql;
                    ret = cmd.ExecuteNonQuery();
                    //delete dataset
                    sql = "delete from " + GetTableName() + " where id='" + entity.Id + "'";
                    cmd.CommandText = sql;
                    ret = cmd.ExecuteNonQuery();
                    TransactionManager.Commit();
                }
                catch
                {
                    TransactionManager.Rollback();
                    throw;
                }
            }
        }
    }
}
