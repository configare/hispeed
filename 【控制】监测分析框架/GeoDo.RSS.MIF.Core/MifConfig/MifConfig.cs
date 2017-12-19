using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Core
{
    public class MifConfig : ConfigBase
    {
        private string _sysConfigFile = "";

        public MifConfig()
            : base()
        {
            LoadConfigItems();
        }

        private void LoadConfigItems()
        {
            _sysConfigFile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SystemData\\mifConfig.cfg");
            _editControl = new ucMifConfigEdit();
            _configName = "产品配置";
            _configDescription = "产品输出路径等配置";
            Dictionary<string, string> items = new Dictionary<string, string>();
            if (File.Exists(_sysConfigFile))
            {
                XElement ele = XElement.Load(_sysConfigFile);
                foreach (XElement configItem in ele.Elements("Config"))
                {
                    items.Add(configItem.Attribute("key").Value, configItem.Attribute("value").Value);
                }
            }
            else
            {
                items.Add("Workspace", Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Workspace"));
                items.Add("TEMP", Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "TEMP"));
                items.Add("Report", Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "Report"));
            }
            _configDic.Add("Workspace", items["Workspace"]);
            _configDic.Add("TEMP", items["TEMP"]);
            if (!Directory.Exists(items["TEMP"]))
                Directory.CreateDirectory(items["TEMP"]);
            if (items.ContainsKey("Report"))
            {
                _configDic.Add("Report", items["Report"]);
                if (!Directory.Exists(items["Report"]))
                    Directory.CreateDirectory(items["Report"]);
            }
            else
                _configDic.Add("Report", AppDomain.CurrentDomain.BaseDirectory + "\\Report");
        }

        internal bool Save(out string message)
        {
            message = "";
            try
            {
                XElement ele = new XElement("MifConfig");
                ele.Add(new XElement("Config", new XAttribute("key", "Workspace"), new XAttribute("value", _configDic["Workspace"])));
                ele.Add(new XElement("Config", new XAttribute("key", "TEMP"), new XAttribute("value", _configDic["TEMP"])));
                ele.Add(new XElement("Config", new XAttribute("key", "Report"), new XAttribute("value", _configDic["Report"])));
                ele.Save(_sysConfigFile);
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }
    }

    public interface IConfig
    {
        string ConfigName { get; }
        string ConfigDescription { get; }
        Control EditControl { get; }
    }

    public abstract class ConfigBase : IConfig
    {
        protected string _configName = "";
        protected string _configDescription = "";
        protected Dictionary<string, string> _configDic = new Dictionary<string, string>();
        protected Control _editControl = null;

        public ConfigBase()
        {
        }

        public string ConfigName
        {
            get { return _configName; }
        }

        public string ConfigDescription
        {
            get { return _configDescription; }
        }

        public string GetConfigValue(string identify)
        {
            if (_configDic.ContainsKey(identify))
                return _configDic[identify];
            else
                return null;
        }

        public void SetConfig(string identify, string value)
        {
            if (_configDic.ContainsKey(identify))
                _configDic[identify] = value;
            else
                _configDic.Add(identify, value);
        }

        public Control EditControl
        {
            get { return _editControl; }
        }
    }
}
