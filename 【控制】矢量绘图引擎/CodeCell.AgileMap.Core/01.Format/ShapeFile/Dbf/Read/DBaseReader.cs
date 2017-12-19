using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;

namespace CodeCell.AgileMap.Core
{
    public class DbaseReader : IDisposable,IDbfReader
    {
        private string[] _fields = null;
        private List<string[]> _values = null;
        
        public DbaseReader(string filename)
        {
            using (DataTable dt = ParseDbfToDataTable.ReadDBF(filename))
            {
                _fields = new string[dt.Columns.Count];
                for (int i = 0; i < dt.Columns.Count; i++)
                    _fields[i] = dt.Columns[i].ColumnName.ToUpper();
                //
                _values = new List<string[]>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string[] vs = new string[dt.Columns.Count];
                    for (int j = 0; j < dt.Columns.Count; j++)
                        vs[j] = dt.Rows[i][j].ToString();
                    _values.Add(vs);
                }
            }
        }

        public void Dispose()
        {
            _values.Clear();// = null;
            _fields = null;
        }

        #region IDbfReader Members

        public int RecordCount
        {
            get
            {
                return _values != null ? _values.Count : 0;
            }
        }

        public string[] Fields
        {
            get
            {
                return _fields;
            }
        }

        public List<string[]> Values
        {
            get { return _values; }
        }

        public string[] GetValues(int oid)
        {
            int idx = oid - 1;
            if (_values == null || idx < 0 || idx >= _values.Count)
                return null;
            return _values[idx];
        }

        public string GetValue(int oid, string field)
        {
            int idx = oid - 1;
            if (_fields == null || _values == null || idx < 0 || idx >= _values.Count)
                return null;
            int fidx = Array.IndexOf(_fields, field);
            if (fidx < 0)
                return null;
            return _values[idx][fidx];
        }

        #endregion
    }
}
