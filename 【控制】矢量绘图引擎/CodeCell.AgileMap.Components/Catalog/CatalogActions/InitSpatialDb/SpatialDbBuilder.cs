using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Components
{
    internal class SpatialDbBuilder
    {
        private DiffDbAdapter _adapter = null;

        public SpatialDbBuilder(IDbConnection connection)
        {
            _adapter = DiffDbAdapterFactory.GetDiffDbAdapter(connection);
        }

        public virtual void Clear(IDbConnection connection, BudGISMetadata metadata, bool isClearDataTables,IProgressTracker tracker)
        {
            IDbTransaction tran = connection.BeginTransaction();
            try
            {
                using (IDbCommand cmd = connection.CreateCommand())
                {
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.Text;
                    if (tracker != null)
                        tracker.StartTracking("开始清空空间数据库...",4);
                    //Clear dataTables
                    if (isClearDataTables)
                    {
                        if(tracker != null)
                            tracker.Tracking("正在清空数据表...", 0);
                        ClearDataTables(connection, cmd);
                    }
                    //drop BGIS_FeatureClass
                    if (tracker != null)
                        tracker.Tracking("正在删除系统表\"BGIS_FeatureClass\"...", 1);
                    cmd.CommandText = "drop table BGIS_FeatureClass";
                    cmd.ExecuteNonQuery();
                    //drop BGIS_FeatureDataset
                    if (tracker != null)
                        tracker.Tracking("正在删除系统表\"BGIS_FeatureDataset\"...", 2);
                    cmd.CommandText = "drop table BGIS_FeatureDataset";
                    cmd.ExecuteNonQuery();
                    //drop BGIS_Map
                    if (tracker != null)
                        tracker.Tracking("正在删除系统表\"BGIS_Map\"...", 3);
                    cmd.CommandText = "drop table BGIS_Map";
                    cmd.ExecuteNonQuery();
                }
                //
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
        }

        private void ClearDataTables(IDbConnection connection, IDbCommand cmd)
        {
            cmd.CommandText = "select DATATABLE from BGIS_FeatureClass";
            List<string> tables = new List<string>();
            using (IDataReader dr = cmd.ExecuteReader())
            {
                while (dr.Read())
                {
                    string tablename = dr.GetString(0);
                    tables.Add(tablename);
                }
            }
            if(tables.Count ==0)
                return ;
            foreach (string t in tables)
            {
                try
                {
                    cmd.CommandText = "drop table " + t;
                    cmd.ExecuteReader();
                }
                catch (Exception ex)
                {
                    Log.WriterException(ex);
                }
            }
        }

        public virtual void Build(IDbConnection connection, BudGISMetadata metadata, IProgressTracker tracker,params object[] args)
        {
            IDbTransaction tran = connection.BeginTransaction();
            try
            {
                using (IDbCommand cmd = connection.CreateCommand())
                {
                    cmd.Transaction = tran;
                    cmd.CommandType = CommandType.Text;
                    //
                    int steps = 0;
                    if (tracker != null)
                        tracker.StartTracking("开始创建元数据表...", 3);
                    //Create Tables
                    CreateTables(connection, metadata,cmd,tracker,ref steps);
                }
                //
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
        }

        private void CreateTables(IDbConnection connection, BudGISMetadata metadata,IDbCommand cmd,IProgressTracker tracker,ref int steps)
        {
            if (metadata.Tables == null || metadata.Tables.Length == 0)
                throw new ArgumentNullException("空间数据库元数据配置对象错误,元数据表为空。");
            foreach (SystemTable table in metadata.Tables)
            {
                if (tracker != null)
                    tracker.Tracking("正在创建\"" + table.Name+"...", steps);
                CreateTables(connection, table, cmd);
                steps++;
            }
        }

        protected virtual void CreateTables(IDbConnection connection, SystemTable table, IDbCommand cmd)
        {
            if (table.Name == null)
                throw new ArgumentNullException("元数据表名称为空。");
            if (table.Fields == null)
                throw new ArgumentNullException("元数据表\""+table.Name+"\"没有定义字段。");
            List<string> fieldTypes = new List<string>();
            foreach (SystemField fld in table.Fields)
            {
                fieldTypes.Add(fld.Name + " " +_adapter.GetFieldTypeString(fld.FieldType));
            }
            _adapter.CreateTable(connection,false,"ID",null, table.Name, fieldTypes.ToArray(), cmd);
        }
    }
}
