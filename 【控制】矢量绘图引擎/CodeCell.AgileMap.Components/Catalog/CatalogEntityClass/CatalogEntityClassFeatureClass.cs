using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class CatalogEntityClassFeatureClass:CatalogEntityClassBase
    {
        public CatalogEntityClassFeatureClass(string connString)
            : base(connString)
        { 
        }

        protected override string GetTableName()
        {
            return BudGISMetadata.cstFeatureClassTableName;
        }

        protected override ICatalogEntity NewCatalogEntity()
        {
            return new SpatialFeatureClass();
        }

        public override void Delete(ICatalogEntity entity)
        {
            using (IDbCommand cmd = _dbConnection.CreateCommand())
            {
                TransactionManager.BeginTransaction(_dbConnection, cmd);
                string sql = null;
                try
                {
                    int ret = 0;
                    //delete table
                    SpatialFeatureClass fetc = entity as SpatialFeatureClass;
                    if (fetc.DataTable != null)
                    {
                        //delete fetclass table
                        sql = _adapter.GetTableIsExistSql(fetc.DataTable, _adapter.GetDataSource(_connString));
                        cmd.CommandText = sql;
                        object v = cmd.ExecuteScalar();
                        ret = int.Parse(v.ToString());
                        if (ret > 0)
                        {
                            sql = "drop table " + fetc.DataTable;
                            cmd.CommandText = sql;
                            ret = cmd.ExecuteNonQuery();
                            //delete spatialindex metadata
                            sql = _adapter.DeleteSpatiaoIndexMetadata(fetc.DataTable);
                            if (sql != null)
                            {
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }
                        }
                        //delete ann table
                        sql = _adapter.GetTableIsExistSql(fetc.AnnoTable, _adapter.GetDataSource(_connString));
                        cmd.CommandText = sql;
                        v = cmd.ExecuteScalar();
                        ret = int.Parse(v.ToString());
                        if (ret > 0)
                        {
                            sql = "drop table " + fetc.AnnoTable;
                            cmd.CommandText = sql;
                            ret = cmd.ExecuteNonQuery();
                            //delete spatialindex metadata
                            sql = _adapter.DeleteSpatiaoIndexMetadata(fetc.AnnoTable);
                            if (sql != null)
                            {
                                cmd.CommandText = sql;
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                    //delete fetclass
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
