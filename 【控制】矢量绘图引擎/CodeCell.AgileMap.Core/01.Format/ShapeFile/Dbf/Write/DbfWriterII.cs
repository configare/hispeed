using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using System.IO;
using System.Data;
using CodeCell.Bricks.Runtime;

namespace CodeCell.AgileMap.Core
{
    public class DbfWriterII
    {
        private OdbcConnection _conn = null;
        private OdbcCommand _cmd = null;
        private string _dbfFilename = null;
        private string _dbfshortname = null;
        private string[] _fields = null;

        public DbfWriterII(string dbfFilename)
        {
            _dbfFilename = dbfFilename;
            string fileName = Path.GetFileNameWithoutExtension(_dbfFilename);
            if (fileName.Length > 8)
            {
                if (!fileName.Substring(0, 8).Contains("."))
                    _dbfshortname = Path.GetDirectoryName(_dbfFilename) + "\\"
                        + fileName.Substring(0, 8) + ".dbf";
                else
                {
                    string[] stringArray = fileName.Split(new char[] { '.' });
                    _dbfshortname = Path.GetDirectoryName(_dbfFilename) + "\\"
                        + stringArray[0] + ".dbf";
                }
            }
            else
                _dbfshortname = _dbfFilename;
        }

        public void BeginWrite()
        {
            string dir = Path.GetDirectoryName(_dbfFilename);
            //if dbffile exists,delete it!
            if (File.Exists(_dbfFilename))
                File.Delete(_dbfFilename);
            if (File.Exists(_dbfshortname))
                File.Delete(_dbfshortname);
            _conn = new OdbcConnection();
            _conn.ConnectionString = ("Driver={Microsoft dBASE Driver (*.dbf)};DriverID=21;DBQ=" + dir);
            _conn.Open();
        }

        //public void Write(Feature[] features)
        //{
        //    Write(features, null);
        //}

        public void Write(Feature[] features, Action<int, string> progress)
        {
            if (_conn.State != ConnectionState.Open)
                _conn.Open();
            string tablename = Path.GetFileNameWithoutExtension(_dbfshortname);
            //是否table已存在
            if (!File.Exists(_dbfshortname))
            {
                //create table
                int fieldsCount = 0;
                if (features.Length > 0)
                    fieldsCount = features[0].FieldNames.Length;
                if (fieldsCount > 0)
                {
                    _fields = new string[fieldsCount];
                    string sql = "create table [" + tablename + "] (";
                    for (int i = 0; i < fieldsCount - 1; i++)
                    {
                        _fields[i] = features[0].FieldNames[i];
                        sql += _fields[i];
                        sql += " varchar(254),";
                    }
                    _fields[fieldsCount - 1] = features[0].FieldNames[fieldsCount - 1];
                    sql += features[0].FieldNames[fieldsCount - 1];
                    sql += " varchar(254));";
                    _cmd = new OdbcCommand(sql, _conn);
                    _cmd.ExecuteScalar();
                }
            }
            string insertSql = "", tmpItems, tmpValues;
            //测试dbf数据库不支持事务，故无法采用事务的方法获取
            //OdbcTransaction myTrans = null;
            try
            {
                //启动一个事务
                //myTrans = _conn.BeginTransaction();
                //_cmd.Transaction = myTrans;

                //write dbf contents
                tmpItems = String.Join(",", _fields);
                for (int i = 0; i < features.Length; i++)
                {
                    if (progress != null)
                        progress(i, null);
                    tmpValues = "'" + String.Join("','", features[i].FieldValues) + "'";
                    insertSql = string.Format("insert into {0}.dbf ({1}) values ({2});", tablename, tmpItems, tmpValues);
                    _cmd.CommandText = insertSql;
                    _cmd.ExecuteNonQuery();
                }
                //myTrans.Commit();
            }
            catch (Exception ex)
            {
                //if (myTrans != null)
                //    myTrans.Rollback();
                Log.WriterError(ex.Message);
            }
        }

        public void EndWrite()
        {
            if (_conn != null)
                _conn.Close();
            if (_cmd != null)
                _cmd.Dispose();
            if (File.Exists(_dbfFilename))
                WriteDbfEncode(_dbfFilename);
            if (Path.GetFileNameWithoutExtension(_dbfFilename).Length > 8)
            {
                if (File.Exists(_dbfshortname))
                    File.Move(_dbfshortname, _dbfFilename);
            }
        }

        private void WriteDbfEncode(string dbfFilename)
        {
            using (FileStream fs = new FileStream(dbfFilename, FileMode.Open, FileAccess.ReadWrite))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    fs.Seek(29, SeekOrigin.Begin);
                    fs.WriteByte(77);
                }
            }
        }
    }
}
