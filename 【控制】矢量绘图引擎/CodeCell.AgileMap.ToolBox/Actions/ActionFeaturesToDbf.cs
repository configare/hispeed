using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.ModelFabric;
using System.Data.Odbc;
using System.IO;
using System.Data;
using CodeCell.AgileMap.Components;


namespace CodeCell.AgileMap.Core
{
    [ActionAttribute("矢量->dBase", "GIS工具", "将点矢量转换为dBase的dbf数据库,主要用于保存注记")]
    public class ActionFeaturesToDbf:ActionBase
    {
        private string _shapefile = null;
        private string _dbffile = null;
        private FieldMap[] _fieldMaps = null;

        public ActionFeaturesToDbf()
        {
            Name = "矢量->dBase";
        }

        [Binding("矢量文件名",enumBindingDirection.InputOutput,typeof(ArgShpFile),false)]
        public string ShapeFile
        {
            get { return _shapefile; }
            set { _shapefile = value; }
        }

        [Binding("dBase文件名", enumBindingDirection.InputOutput, typeof(ArgDbfFile), false)]
        public string DbfFile
        {
            get { return _dbffile; }
            set { _dbffile = value; }
        }

        [Binding("字段映射",enumBindingDirection.InputOutput,typeof(ArgFieldMaps),false)]
        public FieldMap[] FieldMaps
        {
            get { return _fieldMaps; }
            set { _fieldMaps = value; }
        }

        protected override bool ConditionsIsEnough(IContextMessage contextMessage)
        {
            if (string.IsNullOrEmpty(_shapefile))
            {
                contextMessage.AddError("没有设置矢量文件名。");
                return false;
            }
            if (string.IsNullOrEmpty(_dbffile))
            {
                contextMessage.AddError("没有设置目标dBase文件名。");
                return false;
            }
            if (_fieldMaps == null || _fieldMaps.Length == 0)
            {
                contextMessage.AddError("没有设置字段映射关系。");
                return false;
            }
            return true;
        }

        protected override void Sleep(IContextMessage contextMessage)
        {
        }

        public override void Reset()
        {
            _dbffile = null;
            _shapefile = null;
            _fieldMaps = null;
        }

        protected override bool Execute(IContextMessage contextMessage)
        {
            IVectorFeatureDataReader vdr = null;
            OdbcConnection conn = null;
            OdbcCommand cmd = null;
            try
            {
                if (!ResFieldIsOK(out vdr))
                {
                    contextMessage.AddError("字段映射关系中源字段设置有错误。");
                    return false;
                }
                if (!DesFieldIsOK(out conn, out cmd))
                {
                    contextMessage.AddError("字段映射关系中目标字段设置有错误。");
                    return false;
                }
                ImportFeatures(conn, cmd, vdr);
                return true;
            }
            finally
            {
                if (vdr != null)
                    vdr.Dispose();
                if (cmd != null)
                    cmd.Dispose();
                if (conn != null)
                    conn.Dispose();
            }
        }

        private void ImportFeatures(OdbcConnection conn, OdbcCommand cmd, IVectorFeatureDataReader vdr)
        {
            Feature[] features = vdr.Features;
            if (features == null)
                return;
            string sql = "insert into "+Path.GetFileNameWithoutExtension(_dbffile)+"( ";
            foreach (FieldMap map in _fieldMaps)
                sql += (map.DesField.FieldName.ToString() + ",");
            sql = sql.Substring(0, sql.Length - 1) + ") values ";
            string fetsql = null;
            foreach (Feature fet in features)
            {
                string status = fet.GetFieldValue("status").ToUpper();
                if (status != null && status == "1")
                    continue;
                fetsql = sql;
                string values = "(";
                foreach (FieldMap map in _fieldMaps)
                {
                    if (map.ResField.FieldName.ToUpper() == "SHAPE.X")
                        values += ("'" + fet.Geometry.Centroid.X.ToString() + "',");
                    else if (map.ResField.FieldName.ToUpper() == "SHAPE.Y")
                        values += ("'" + fet.Geometry.Centroid.Y.ToString() + "',");
                    else
                        values += ("'" + fet.GetFieldValue(map.ResField.FieldName) + "',");
                }
                values = values.Substring(0, values.Length - 1) + ")";
                fetsql += values;
                //
                cmd.CommandText = fetsql;
                int ret = cmd.ExecuteNonQuery();
            }
        }

        private bool DesFieldIsOK(out OdbcConnection conn,out OdbcCommand cmd)
        {
            conn = new OdbcConnection();
            conn.ConnectionString = ("Driver={Microsoft dBASE Driver (*.dbf)};DriverID=21;DBQ=" + Path.GetDirectoryName(_dbffile));
            conn.Open();
            cmd = conn.CreateCommand();
            cmd.CommandText = "select * from " + Path.GetFileNameWithoutExtension(_dbffile);
            using (OdbcDataAdapter ada = new OdbcDataAdapter(cmd))
            {
                using (DataTable dt = new DataTable())
                {
                    ada.Fill(dt);
                    foreach (FieldMap map in _fieldMaps)
                    {
                        bool isOK = false ;
                        foreach (DataColumn c in dt.Columns)
                        {
                            if (c.ColumnName.ToUpper() == map.DesField.FieldName.ToUpper())
                            {
                                isOK = true;
                                break;
                            }
                        }
                        if (!isOK)
                            return false;
                    }
                }
            }
            return true;
        }

        private bool ResFieldIsOK(out IVectorFeatureDataReader vdr)
        {
            vdr = null;
            IUniversalVectorDataReader udr = VectorDataReaderFactory.GetUniversalDataReader(_shapefile);
            if (udr == null)
                return false;
            vdr = udr as IVectorFeatureDataReader;
            string[] fields = vdr.Fields;
            if (fields == null || fields.Length == 0)
                return false;
            for (int i = 0; i < fields.Length; i++)
                fields[i] = fields[i].ToUpper();
            foreach (FieldMap map in _fieldMaps)
            {
                if (map.ResField.FieldName.ToUpper() == "SHAPE.X")
                    continue;
                if (map.ResField.FieldName.ToUpper() == "SHAPE.Y")
                    continue;
                if (Array.IndexOf(fields, map.ResField.FieldName.ToUpper()) < 0)
                    return false;
            }
            return true;
        }

        public override void Dispose()
        {
            Reset();
        }
    }
}
