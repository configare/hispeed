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
    /// 创建者：chenn   创建日期：2014/5/30 14:09:12
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class AdjustConfig : ConfigBase
    {
        string _adjustConfigFile;

        public AdjustConfig()
            : base()
        {
            LoadConfigItems();
        }

        private void LoadConfigItems()
        {
            _adjustConfigFile = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SystemData\\adjustConfig.cfg");
            _editControl = new UCAdjustConfigEdit();
            _configName = "平移校正配置";
            _configDescription = "平移校正相关配置";
            Dictionary<string, string> items = new Dictionary<string, string>();
            if (File.Exists(_adjustConfigFile))
            {
                XElement ele = XElement.Load(_adjustConfigFile);
                foreach (XElement configItem in ele.Elements("Config"))
                {
                    items.Add(configItem.Attribute("key").Value, configItem.Attribute("value").Value);
                }
            }
            else
            {
                items.Add("IsOpenResult", "true");
            }
            _configDic.Add("IsOpenResult", items["IsOpenResult"]);
        }

        internal bool Save(out string message)
        {
            message = "";
            try
            {
                XElement ele = new XElement("AdjustConfig");
                ele.Add(new XElement("Config", new XAttribute("key", "IsOpenResult"), new XAttribute("value", _configDic["IsOpenResult"])));
                ele.Save(_adjustConfigFile);
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
