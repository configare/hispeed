using System;
using System.Xml.Linq;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    //读取数据库配置文件的单例
    public class UserPreferences
    {
        private const string DB_CONFIG_PATH = "./SystemData/ProductArgs/CLD/CPDataBaseArgs.xml";

        private readonly string _server;
        private readonly string _database;
        private readonly string _uid;
        private readonly string _password;

        private static UserPreferences _instance = null;

        public string Server { get { return _server; } }
        public string Database { get { return _database; } }
        public string Uid { get { return _uid; } }
        public string Password { get { return _password; } }

        private UserPreferences()
        {
            XDocument config = XDocument.Load(DB_CONFIG_PATH);
            _server = config.Root.Element("Server").Value;
            _database = config.Root.Element("Database").Value;
            _uid = config.Root.Element("Uid").Value;
            _password = config.Root.Element("Passwords").Value;
        }

        public static UserPreferences GetInstance()
        {
            if (_instance == null)
                _instance = new UserPreferences();
            return _instance;
        }
    }
}
