using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Data.Odbc;
using System.IO;
using System.Data;

namespace CodeCell.AgileMap.Core
{
    public class DbfWriter
    {
        private OdbcConnection _conn = null;
        private OdbcCommand _cmd = null;
        private OdbcDataAdapter _ad = null;
        private string _dbfFilename;
        protected string[] _fields = null;
        protected List<string[]> _values = null;

        public DbfWriter(string dbfFilename, Feature[] features)
        {
            string dir = Path.GetDirectoryName(dbfFilename);
            int fieldsCount;
            _dbfFilename = dbfFilename;
            if (features.Length > 0)
            {
                fieldsCount = features[0].FieldNames.Length;
            }
            else
            {
                fieldsCount = 0;
            }
            try
            {
                //if dbffile exists,delete it!
                if (File.Exists(dbfFilename))
                {
                    File.Delete(dbfFilename);
                }
                _conn = new OdbcConnection();
                _conn.ConnectionString = ("Driver={Microsoft dBASE Driver (*.dbf)};DriverID=21;DBQ=" + dir);
                _conn.Open();
                //create table
                if (fieldsCount > 0)
                {
                    _fields = new string[fieldsCount];
                    string sql = "create table [" + Path.GetFileNameWithoutExtension(dbfFilename) + "] (";
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
                this.Write(features);
                this.CloseConnection();
            }
            catch
            {
                throw;
            }
            finally
            {
                if (Path.GetFileNameWithoutExtension(dbfFilename).Length > 8 )
                {
                    string dbfshortname = Path.GetDirectoryName(dbfFilename) + "\\" + Path.GetFileNameWithoutExtension(dbfFilename).Substring(0, 8) + ".dbf";
                    if (File.Exists(dbfshortname))
                        File.Move(dbfshortname, dbfFilename);
                }
            }
        }

        private void Write(Feature[] features)
        {
            //OdbcTransaction transaction = null;
            string insertSql = null;
            string tmpItems = null;
            string tmpValues = null;
            _values = new List<string[]>(features.Length);
            if (_conn.State != ConnectionState.Open)
            {
                _conn.Open();
            }
            try
            {
                //write dbf contents
                tmpItems = String.Join(",", _fields);
                for (int i = 0; i < features.Length; i++)
                {
                    _values.Add(features[i].FieldValues);
                    tmpValues = string.Empty;
                    tmpValues = "'";
                    tmpValues += String.Join("','", features[i].FieldValues);
                    tmpValues += "'";
                    insertSql = "insert into " + Path.GetFileNameWithoutExtension(_dbfFilename) + ".dbf (";
                    insertSql += tmpItems + ") values (";
                    insertSql += tmpValues + ");";
                    _cmd.CommandText = insertSql;
                    _cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                throw;
            }
            finally
            {
               
            }
        }

        private void CloseConnection()
        {
            try
            {
                if (_conn != null)
                    _conn.Close();
                if (_cmd != null)
                    _cmd.Dispose();
                if (_ad != null)
                    _ad.Dispose();
            }
            catch
            {
                throw;
            }
            finally
            {
                
            }
        }
    }
}
