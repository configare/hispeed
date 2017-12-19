using System;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Xml.Linq;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    //搜索出一条记录，回调某种函数
    public delegate void SqlDelegate(MySqlDataReader dataReader);

    public class DBConnect
    {
        public static string _dataBaseXml = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\ProductArgs\MWS\Retrieval.xml";
        private readonly string brightnessTemperatureDir;
        private readonly string periodDayDir;
        private readonly string snowCloudDir;
        private readonly string output;
        private const string SMART_XML = "./SystemData/mifConfig.cfg";
        private const string DEFAULT = "./TEMP/";

        public string BrightnessTemperatureDir { get { return brightnessTemperatureDir; } }
        public string PeriodDayDir { get { return periodDayDir; } }
        public string SnowCloudDir { get { return snowCloudDir; } }
        public string Output { get { return output; } }

        private MySqlConnection connection;
        public MySqlConnection Connection { get { return connection; } }
        private static Object _obj = new Object();

        public DBConnect()
        {
            XDocument myXml = XDocument.Load(_dataBaseXml);
            string server = myXml.Root.Element("appSettings").Element("server").Value;
            string database = myXml.Root.Element("appSettings").Element("database").Value;
            string uid = myXml.Root.Element("appSettings").Element("uid").Value;
            string password = myXml.Root.Element("appSettings").Element("password").Value;
            brightnessTemperatureDir = myXml.Root.Element("appSettings").Element("brightnessTemperatureDir").Value;
            periodDayDir = myXml.Root.Element("appSettings").Element("periodDayDir").Value;
            snowCloudDir = myXml.Root.Element("appSettings").Element("snowCloudDir").Value;
            if (File.Exists(SMART_XML))
            {
                XDocument smartXml = XDocument.Load(SMART_XML);
                output = smartXml.Root.Element("Config").Attribute("value").Value;
            }
            else
            {
                output = DEFAULT;
            }
            if (!brightnessTemperatureDir.EndsWith("\\"))
                brightnessTemperatureDir += "\\";
            if (!periodDayDir.EndsWith("\\"))
                periodDayDir += "\\";
            if (!snowCloudDir.EndsWith("\\"))
                snowCloudDir += "\\";
            if (!output.EndsWith("\\"))
                output += "\\";
            StringBuilder connectionString = new StringBuilder();
            connectionString.Append("SERVER=").Append(server).Append(";");
            connectionString.Append("DATABASE=").Append(database).Append(";");
            connectionString.Append("UID=").Append(uid).Append(";");
            connectionString.Append("PASSWORD=").Append(password).Append(";");
            connection = new MySqlConnection(connectionString.ToString());
        }

        public bool ConnOpend()
        {
            if (connection.State == System.Data.ConnectionState.Open)
            {
                return true;
            }
            return false;
        }

        private void ExceptionCatch(MySqlException ex)
        {
            switch (ex.Number)
            {
                case 0:
                    throw new Exception("Cannot connect to server.");
                case 1045:
                    throw new Exception("Invalid username/password, please try again");
                case 1042:
                    throw new Exception("MySQL服务未启动！Unable to connect to any of the specified MySQL hosts.");
                default:
                    throw ex;
            }
        }

        public bool OpenConnection()
        {
            try
            {
                if (ConnOpend())
                    return true;
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                ExceptionCatch(ex);
                return false;
            }
        }

        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }

        public void Retrieval(string query, SqlDelegate pFunc)
        {
            lock (_obj)
            {
                if (OpenConnection() == true)
                {
                    try
                    {
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        MySqlDataReader dataReader = cmd.ExecuteReader();
                        pFunc(dataReader);
                        dataReader.Close();
                    }
                    catch (MySqlException ex)
                    {
                        ExceptionCatch(ex);
                    }
                    finally
                    {
                        if (this.ConnOpend())
                            this.CloseConnection();
                    }
                }
            }
        }


    }
}
