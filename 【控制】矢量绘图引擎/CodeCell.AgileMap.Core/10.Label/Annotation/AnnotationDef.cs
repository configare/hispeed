using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data.Odbc;
using System.Data;
using System.IO;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Core
{
    public class AnnotationDef:IPersistable
    {
        private string _filename = null;
        private string[] _fields = null;
        private const string cstDefaultAngleField = "Angle";
        private const string cstDefaultXField = "X";
        private const string cstDefaultYField = "Y";

        public AnnotationDef()
        { 
        }

        public AnnotationDef(string filename)
            :this()
        {
            _filename = filename;
            ExtractFields();
        }

        internal void GetValidLabelField(int maplevel, out string anglefield, out string xfield, out string yfield)
        {
            anglefield = null;
            xfield = null;
            yfield = null;
            if (maplevel < 0 || _fields == null || _fields.Length == 0)
                return;
            if (Array.IndexOf(_fields, cstDefaultAngleField.ToUpper()) >= 0 &&
               Array.IndexOf(_fields, cstDefaultAngleField.ToUpper()) >= 0 &&
               Array.IndexOf(_fields, cstDefaultAngleField.ToUpper()) >= 0)
            {
                anglefield = cstDefaultAngleField;
                xfield = cstDefaultXField;
                yfield = cstDefaultYField;
            }
            string afld = cstDefaultAngleField + "L"+maplevel.ToString();
            string xfld = cstDefaultXField + "L" + maplevel.ToString();
            string yfld = cstDefaultYField + "L" + maplevel.ToString();
            if (Array.IndexOf(_fields, afld.ToUpper()) >= 0 &&
                Array.IndexOf(_fields, xfld.ToUpper()) >= 0 &&
                Array.IndexOf(_fields, yfld.ToUpper()) >= 0)
            {
                anglefield = afld;
                xfield = xfld;
                yfield = yfld;
            }
        }

        [DisplayName("注记文件名")]
        public string Filename
        {
            get { return _filename; }
            set 
            {
                if (value != null && _filename != value)
                {
                    _filename = value;
                    ExtractFields();
                }
            }
        }

        [Browsable(false)]
        public string[] AllFields
        {
            get { return _fields; }
            set { _fields = value; }
        }

        private void ExtractFields()
        {
            if (string.IsNullOrEmpty(_filename))
                return;
            try
            {
                using (OdbcConnection conn = new OdbcConnection())
                {
                    conn.ConnectionString = ("Driver={Microsoft dBASE Driver (*.dbf)};DriverID=21;DBQ=" + Path.GetDirectoryName(_filename));
                    conn.Open();
                    using (OdbcCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select * from " + Path.GetFileNameWithoutExtension(_filename);
                        using (OdbcDataAdapter ada = new OdbcDataAdapter(cmd))
                        {
                            using (DataTable dt = new DataTable())
                            {
                                ada.Fill(dt);
                                if (dt.Columns == null || dt.Columns.Count == 0)
                                    return;
                                _fields = new string[dt.Columns.Count];
                                for (int i = 0; i < dt.Columns.Count; i++)
                                    _fields[i] = dt.Columns[i].ColumnName;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriterException("AnnotationDef", "ExtractFields", ex);
            }
        }

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("AnnotationDef");
            obj.AddAttribute("filename", _filename != null ? _filename : string.Empty);
            obj.AddAttribute("fields", _fields != null ? string.Join(",", _fields) : string.Empty);
            return obj;
        }

        #endregion
    }
}
