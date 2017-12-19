using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Components
{
    internal class DataImporter:IDataImporter,IDisposable
    {
        private DiffDbAdapter _adapter = null;

        public DataImporter()
        { 
        }

        #region IDataImporter Members

        public void Import(Feature[] features,MetaFeatureClassDef fetclassdef, IDbConnection dbConnection, IProgressTracker tracker, params object[] args)
        {
            if (features == null)
                throw new ArgumentNullException("features为空。");
            if (dbConnection == null)
                throw new ArgumentNullException("dbConnection为空。");
            if (fetclassdef == null)
                throw new ArgumentNullException("feactureClass.Name为空，无法确定数据表名。");
            _adapter = DiffDbAdapterFactory.GetDiffDbAdapter(dbConnection);
            string[] fields = features[0].FieldNames;
            Import(fetclassdef, fields, features, dbConnection, tracker);
        }

        private void Import(MetaFeatureClassDef fetclassdef, string[] fields, Feature[] features, IDbConnection dbConnection, IProgressTracker tracker)
        {
            IDbTransaction tran = dbConnection.BeginTransaction();
            try
            {
                using(IDbCommand cmd = dbConnection.CreateCommand())
                {
                    cmd.Connection = dbConnection;
                    cmd.Transaction = tran;
                    int span = Math.Min(100, features.Length);
                    if (tracker != null)
                        tracker.StartTracking("正在导入要素集\"" + fetclassdef.DataTable + "\"...", features.Length / span + 1);
                    //Create Table
                    if (tracker != null)
                        tracker.Tracking("正在创建数据表\"" + fetclassdef.DataTable + "\"...", 0);
                    string[] fieldTypes = GetFieldTypes(fields, features);
                    _adapter.CreateTable(dbConnection,true, 
                        "OID","SHAPE", 
                        fetclassdef.DataTable,
                        fieldTypes, cmd);
                    //Insert row to BGIS_FeatureClass                 
                    InsertFeatureClassRow(cmd, fetclassdef);
                    //Insert features
                    string flds = string.Empty;
                    for (int i = 0; i < fieldTypes.Length; i++)
                        flds += (fieldTypes[i].Split(' ')[0] + ",");
                    flds = flds.Substring(0, flds.Length - 1);
                    int ret = 0;
                    int idx = -1;
                    foreach (Feature fet in features)
                    {
                        idx++;
                        int n = (idx / span);
                        if (idx % span == 0)
                            tracker.Tracking("正在导入要素(" + (n * span).ToString()+ "/"+features.Length.ToString()+")...",n);
                        if (fet.Geometry == null)
                            continue;
                        string sql = "insert into " + fetclassdef.DataTable + "(" + flds + ") values(" +
                                 fet.OID.ToString() + ",";
                        for (int i = 1; i < fieldTypes.Length - 1; i++)
                            sql += ("'" + fet.GetFieldValue(i - 1) + "',");
                        sql += ("GeomFromText('" + fet.Geometry.ToWkt() + "'))");
                        cmd.CommandText = sql;
                        ret = cmd.ExecuteNonQuery();
                    }
                }
                tran.Commit();
            }
            catch (Exception ex)
            {
                tran.Rollback();
                throw ex;
            }
        }

        private void InsertFeatureClassRow(IDbCommand cmd, MetaFeatureClassDef fetclassdef)
        {
            string sql = "insert into BGIS_FeatureClass(ID,DATASETID,NAME,DATATABLE,SPATIALREF,CREATETIME,MAPSCALE,SOURCE,DESCRIPTION) " +
                             "values('{0}','{1}','{2}','{3}','{4}',{5},{6},'{7}','{8}')";
            cmd.CommandText = string.Format(sql,
                                                                 Guid.NewGuid().ToString(),
                                                                 fetclassdef.DatasetId,
                                                                 fetclassdef.Name,
                                                                 fetclassdef.DataTable,
                                                                 fetclassdef.SpatialRef,
                                                                 _adapter.DateTimeToSql(fetclassdef.DataTime),
                                                                 fetclassdef.MapScale.ToString(),
                                                                 fetclassdef.Source,
                                                                 fetclassdef.Description);
            cmd.ExecuteNonQuery();
        }

        private string[] GetFieldTypes(string[] fields, Feature[] features)
        {
            string[] fieldTypes = new string[fields.Length + 2];
            fieldTypes[0] = "OID INT32";
            for (int i = 0; i < fields.Length; i++)
            {
                int len = fields[i].Length * 2;//unicode
                for (int ifea = 0; ifea < features.Length; ifea++)
                {
                    string v = features[ifea].GetFieldValue(fields[i]);
                    if (v == null)
                        continue;
                    int vLen = v.Length *2;
                    if (len < vLen)
                        len = vLen;
                }
                fieldTypes[i + 1] =fields[i].ToUpper()+  " VARCHAR("+len.ToString()+")";
            }
            fieldTypes[fieldTypes.Length - 1] =  _adapter.GetGeometryFieldDef();
            return fieldTypes;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _adapter = null;
        }

        #endregion
    }
}
