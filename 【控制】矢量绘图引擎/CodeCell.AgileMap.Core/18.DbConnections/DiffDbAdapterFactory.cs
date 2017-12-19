using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
//using Oracle.DataAccess.Client;

namespace CodeCell.AgileMap.Core
{
    public static class DiffDbAdapterFactory
    {
        public static DiffDbAdapter GetDiffDbAdapter(IDbConnection dbConnection)
        {
            if (dbConnection is SqlConnection)
            {
                return new DiffDbAdapterSql();
            }
            //else if (dbConnection is OracleConnection)
            //{
            //    return new DiffDbAdapterOracle();
            //}
            else if (dbConnection is MySqlConnection)
            {
                return new DiffDbAdapterMySQL();
            }
            else
                throw new NotSupportedException("不支持的数据库类型("+dbConnection.ToString()+")。");
        }
    }
}
