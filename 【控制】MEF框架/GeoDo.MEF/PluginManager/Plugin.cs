using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;

namespace GeoDo.MEF
{
    /// <summary>
    /// 插件项
    /// </summary>
    public class Plugin
    {
        //插件定义文件明.绝对路径
        private string _filename = null;
        private string _id = null;
        private string _name = null;
        private string _desc = null;
        //插件需要注册的项，包含注册器和注册目标
        private Tuple<string, string>[] _registers = null;
        //当插件类型包含theme时候需要
        private XTheme _xTheme = null;  //ConfigItems

        public Plugin()
        { }

        public string Filename
        {
            get { return _filename; }
        }

        public string ID
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public string Description
        {
            get { return _desc; }
        }

        public Tuple<string, string>[] Registers
        {
            get { return _registers; }
        }

        public XTheme XTheme
        {
            get { return _xTheme; }
        }

        /// <summary>
        /// 获取指定名字的配置项
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public XThemeItem GetConfigItem(string itemName)
        {
            if (_xTheme == null || _xTheme.Items == null || _xTheme.Items.Length == 0)
                return null;
            for (int i = 0; i < _xTheme.Items.Length; i++)
            {
                if (_xTheme.Items[i].Name == itemName)
                {
                    return _xTheme.Items[i];
                }
            }
            return null;
        }

        public static Plugin Parse(string pluginXml)
        {
            pluginXml = Path.GetFullPath(pluginXml);
            XElement xplugin = XElement.Load(pluginXml, LoadOptions.None);
            XElement root = xplugin;
            if (root.Name.LocalName != "Package")
                return null;
            string nameSpace = root.Name.NamespaceName;
            XAttribute xid = root.Attribute("id");
            XAttribute xname = root.Attribute("name");
            XAttribute xdescription = root.Attribute("description");
            var xRegRoot = root.Element(XName.Get("Register", nameSpace));
            Tuple<string, string>[] regs = ParseRegister(xRegRoot);
            var xThemeRoot = root.Element(XName.Get("Theme",nameSpace));
            XThemeItem[] items = ParseThemeConfig(pluginXml, xThemeRoot);
            return new Plugin
            {
                _id = (xid == null ? null : xid.Value),
                _name = (xname == null ? null : xname.Value),
                _desc = (xdescription == null ? null : xdescription.Value),
                _filename = pluginXml,
                _registers = regs,
                _xTheme = new XTheme { Items = items }
            };
        }

        /// <summary>
        /// 解析配置项
        /// </summary>
        /// <param name="pluginXml"></param>
        /// <param name="xThemeRoot"></param>
        /// <returns></returns>
        private static XThemeItem[] ParseThemeConfig(string pluginXml, XElement xThemeRoot)
        {
            if (xThemeRoot == null)
                return null;
            string pluginPath = Path.GetDirectoryName(pluginXml);
            List<XThemeItem> items = new List<XThemeItem>();
            var xThemes = xThemeRoot.Elements();
            foreach (XElement ele in xThemes)
            {
                string name = ele.Name.LocalName;
                string lingin = ele.Attribute("linkin").Value;
                if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(lingin))
                    continue;
                lingin = pluginPath + lingin;
                XThemeItem themeItem = new XThemeItem { Name = name, Linkin = lingin };
                if (!string.IsNullOrWhiteSpace(lingin))
                {
                    string linkinfullname = Path.GetFullPath(lingin);
                    themeItem.LinkinFullname = linkinfullname;
                }
                else if (ele.HasElements)
                {
                    XElement content = ele.Elements().First();
                    themeItem.Content = content;
                }
                items.Add(themeItem);
            }
            return items.ToArray();
        }

        /// <summary>
        /// 解析要注册的项
        /// </summary>
        /// <param name="xRegRoot"></param>
        /// <returns></returns>
        private static Tuple<string, string>[] ParseRegister(XElement xRegRoot)
        {
            if (xRegRoot == null)
                return null;
            string nameSpace = xRegRoot.Name.NamespaceName;
            var xRegs = xRegRoot.Elements(XName.Get("Plugin", nameSpace));
            List<Tuple<string, string>> regs = new List<Tuple<string, string>>();
            foreach (XElement ele in xRegs)
            {
                string reg = ele.Attribute("register").Value;
                string dll = ele.Attribute("value").Value;
                if (string.IsNullOrWhiteSpace(reg) || string.IsNullOrWhiteSpace(dll))
                    continue;
                dll = Path.GetFullPath(System.AppDomain.CurrentDomain.BaseDirectory + dll);
                regs.Add(new Tuple<string, string>(reg, dll));
            }
            return regs.ToArray();
        }
    }
}
