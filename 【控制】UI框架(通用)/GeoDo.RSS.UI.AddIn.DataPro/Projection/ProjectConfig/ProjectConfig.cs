#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/11/18 11:05:12
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.IO;
using System.Xml.Linq;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    /// <summary>
    /// 类名：ProjectConfig
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/11/18 11:05:12
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class ProjectConfig : ConfigBase
    {
        string _prjConfigFile;

        public ProjectConfig()
            : base()
        {
            LoadConfigItems();
        }

        private void LoadConfigItems()
        {
            _prjConfigFile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SystemData\\prjConfig.cfg");
            _editControl = new UCProjectConfigEdit();
            _configName = "投影配置";
            _configDescription = "投影输出路径配置";
            Dictionary<string, string> items = new Dictionary<string, string>();
            if (File.Exists(_prjConfigFile))
            {
                XElement ele = XElement.Load(_prjConfigFile);
                foreach (XElement configItem in ele.Elements("Config"))
                {
                    items.Add(configItem.Attribute("key").Value, configItem.Attribute("value").Value);
                }
            }
            else
            {
                items.Add("ProjectDir", "");
                items.Add("IsUsed", "false");
            }
            _configDic.Add("ProjectDir", items["ProjectDir"]);
            _configDic.Add("IsUsed", items["IsUsed"]);
        }

        internal bool Save(out string message)
        {
            message = "";
            try
            {
                XElement ele = new XElement("ProjectConfig");
                ele.Add(new XElement("Config", new XAttribute("key", "ProjectDir"), new XAttribute("value", _configDic["ProjectDir"])));
                ele.Add(new XElement("Config", new XAttribute("key", "IsUsed"), new XAttribute("value", _configDic["IsUsed"])));
                ele.Save(_prjConfigFile);
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }
    }
}
