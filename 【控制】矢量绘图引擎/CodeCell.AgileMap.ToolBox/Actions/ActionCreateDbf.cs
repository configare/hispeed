using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.ModelFabric;
using System.IO;
using System.Data.Odbc;
using CodeCell.AgileMap.Components;


namespace CodeCell.AgileMap.Core
{
    [ActionAttribute("创建dBase数据库", "GIS工具", "创建dBase数据库(dbf文件),版本与ESRI Shape Files的dbf一致")]
    public class ActionCreateDbf:ActionBase
    {
        private string _filename = null;
        private FieldDef[] _fieldDefs = null;

        public ActionCreateDbf()
        {
            Name = "创建dBase数据库";
        }

        [Binding("文件名称",enumBindingDirection.InputOutput,typeof(ArgDbfFile),false)]
        public string Filename
        {
            get { return _filename; }
            set { _filename = value; }
        }

        [Binding("字段列表", enumBindingDirection.InputOutput, typeof(ArgFieldDefs), false)]
        public FieldDef[] FieldDefs
        {
            get { return _fieldDefs; }
            set { _fieldDefs = value; }
        }

        protected override bool ConditionsIsEnough(IContextMessage contextMessage)
        {
            if (string.IsNullOrEmpty(_filename))
            {
                contextMessage.AddError("没有设置要创建的dbf文件名称。");
                return false;
            }
            if (_fieldDefs == null || _fieldDefs.Length == 0)
            {
                contextMessage.AddError("没有设置地段列表。");
                return false;
            }
            return true;
        }

        protected override void Sleep(IContextMessage contextMessage)
        {
        }

        public override void Reset()
        {
            _filename = null;
            _fieldDefs = null;
        }

        protected override bool Execute(IContextMessage contextMessage)
        {
            string dir = Path.GetDirectoryName(_filename);
            int fieldsCount = _fieldDefs.Length;
            string tablename = Path.GetFileNameWithoutExtension(_filename);
            OdbcConnection conn = null;
            try
            {
                //if dbffile exists,delete it!
                if (File.Exists(_filename))
                    File.Delete(_filename);
                conn = new OdbcConnection();
                conn.ConnectionString = ("Driver={Microsoft dBASE Driver (*.dbf)};DriverID=21;DBQ=" + dir);
                conn.Open();
                //fields
                string fields = null;
                foreach (FieldDef fld in _fieldDefs)
                {
                    switch (fld.FieldType)
                    { 
                        case enumFieldTypes.Text:
                            fields +=( fld.FieldName + " varchar("+fld.FieldLength.ToString()+"),");
                            break;
                        case enumFieldTypes.Int32:
                            fields += (fld.FieldName + " varchar(12),");
                            break;
                        case enumFieldTypes.Int64:
                            fields += (fld.FieldName + " varchar(24),");
                            break;
                        case enumFieldTypes.Float:
                            fields += (fld.FieldName + " varchar(32),");
                            break;
                        case enumFieldTypes.Double:
                            fields += (fld.FieldName + " varchar(32),");
                            break;
                        default:
                            fields += (fld.FieldName + " varchar(32),");
                            break;
                    }
                }
                fields = fields.Substring(0, fields.Length - 1);
                //create table
                string sql = "create table [" + tablename+ "] ("+fields+");";
                using (OdbcCommand cmd = new OdbcCommand(sql, conn))
                {
                    cmd.ExecuteScalar();
                }
                return true;
            }
            catch
            {
                throw;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
                if (Path.GetFileNameWithoutExtension(_filename).Length > 8)
                {
                    string dbfshortname = Path.GetDirectoryName(_filename) + "\\" + Path.GetFileNameWithoutExtension(_filename).Substring(0, 8) + ".dbf";
                    if (File.Exists(dbfshortname))
                        File.Move(dbfshortname, _filename);
                }
            }
        }

        public override void Dispose()
        {
            Reset();
        }
    }
}
