using System;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    //数据库操作的单例
    public class DBConnect
    {
        private ConnectMySqlCloud _connection;
        private Object _obj = new Object();

        private static DBConnect _instance = null;

        //Initialize values
        private DBConnect()
        {
            //UserPreferences config = UserPreferences.GetInstance();
            //StringBuilder connectionString = new StringBuilder();
            //connectionString.Append("SERVER=").Append(config.Server).Append(";");
            //connectionString.Append("DATABASE=").Append(config.Database).Append(";");
            //connectionString.Append("UID=").Append(config.Uid).Append(";");
            //connectionString.Append("PASSWORD=").Append(config.Password).Append(";");
            //_connection = new MySqlConnection(connectionString.ToString());
            _connection = new ConnectMySqlCloud();
        }

        public static DBConnect GetInstance()
        {
            if (_instance == null)
                _instance = new DBConnect();
            return _instance;
        }

        //open connection to database
        public bool OpenConnection()
        {
            try
            {
                if (_connection.ConnOpend() || _connection.OpenConn())
                    return true;
                return false;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        //执行一条sql，对返回结果回调某个函数
        public void Retrieval(string query, Action<MySqlDataReader> action)
        {
            lock (_obj)
            {
                MySqlCommand cmd = new MySqlCommand(query, _connection.Connection);
                MySqlDataReader dataReader = null;
                try
                {
                    dataReader = cmd.ExecuteReader();
                    action(dataReader);
                    dataReader.Close();
                }
                finally
                {
                    if (dataReader != null && !dataReader.IsClosed)
                    {
                        dataReader.Close();
                    }
                }
            }
        }

        //Close connection
        public bool CloseConnection()
        {
            try
            {
                if (_connection.ConnOpend())
                    _connection.Closeconn();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
    }
}
