using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
//using Oracle.DataAccess.Client;

namespace CodeCell.AgileMap.Core
{
    public static class DbConnectionFactory
    {
        public static IDbConnection CreateDbConnection(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException("连接字符串为空。");
            string[] parts = connectionString.Split('$');//数据库类型与数据连接字符窜使用$符号分隔
            if (parts.Length == 1)
                return TryEachOne(parts[0]);
            string dbType = parts[0].ToUpper();
            if (dbType == enumSpatialDatabaseType.MySql.ToString().ToUpper())
                return CreateMySqlConnection(parts[1]);
            else if (dbType == enumSpatialDatabaseType.MsSql.ToString().ToUpper())
                return CreateMsSqlConnection(parts[1]);
            //else if (dbType == enumSpatialDatabaseType.Oracle.ToString().ToUpper())
            //    return CreateOracleConnection(parts[1]);
            return null;
        }

        private static IDbConnection TryEachOne(string connstring)
        {
            try
            {
                return CreateMySqlConnection(connstring);
            }
            catch 
            {
                //try
                //{
                //    //return CreateOracleConnection(connstring);
                //}
                //catch 
                //{
                    try
                    {
                        return CreateMsSqlConnection(connstring);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                //}
            }
        }

        //private static IDbConnection CreateOracleConnection(string connstring)
        //{
        //    return new OracleConnection(connstring) as IDbConnection;
        //}

        private static IDbConnection CreateMsSqlConnection(string connstring)
        {
            return new SqlConnection(connstring);
        }

        private static IDbConnection CreateMySqlConnection(string connstring)
        {
            return new MySqlConnection(connstring);
        }
    }
}
