using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace GeoDo.MEF
{
    /// <summary>
    /// 插件扩展产品管理器
    /// </summary>
    public class PluginManager
    {
        /// <summary>
        /// 控制插件发生改变事件通知
        /// </summary>
        public event Action<Plugin, string, bool> OnEnableChanged = null;
        /// <summary>
        /// 默认插件定义文件目录,定义为系统相对路径
        /// </summary>
        public static string PluginDirection = System.AppDomain.CurrentDomain.BaseDirectory + @"Plugins\";

        private static PluginManager _pm = null;
        private Plugin[] _plugins = null;

        private PluginManager()
        {
            LoadPlugins();
        }

        /// <summary>
        /// 插件扩展产品管理器实例
        /// </summary>
        public static PluginManager Instance
        {
            get
            {
                if (_pm == null)
                    _pm = new PluginManager();
                return _pm;
            }
        }

        /// <summary>
        /// 当前加载的插件
        /// </summary>
        public Plugin[] Plugins
        {
            get { return _plugins; }
        }

        private void LoadPlugins()
        {
            _plugins = null;
            if (!Directory.Exists(PluginDirection))
                return;
            string[] pluginDirs = Directory.GetDirectories(PluginDirection);
            List<Plugin> plgins = new List<Plugin>();
            foreach (string pluginDir in pluginDirs)
            {
                string xmlfile = Path.Combine(pluginDir, "plugin.xml");
                if (File.Exists(xmlfile))
                {
                    Plugin plugin = Plugin.Parse(xmlfile);
                    if (plugin != null)
                        plgins.Add(plugin);
                }
            }
            _plugins = plgins.ToArray();
        }

        /// <summary>
        /// 注册当前加载所有插件中指定注册器的注册项
        /// </summary>
        /// <typeparam name="T">注册器类型</typeparam>
        /// <param name="register">注册器</param>
        /// <returns>注册项</returns>
        public T[] Register<T>(string register) where T : class
        {
            if (_plugins == null || _plugins.Length == 0)
                return null;
            //Dictionary<string, string> registed = new Dictionary<string, string>();//记录已经注册过的项目，防止不同插件注册相同项。
            List<T> tls = new List<T>();
            foreach (Plugin plugin in _plugins)
            {
                Tuple<string, string>[] cacheRegs = plugin.Registers;
                if (cacheRegs == null || cacheRegs.Length == 0)
                    continue;
                using (IComponentLoader<T> loader = new ComponentLoader<T>())
                {
                    for (int i = 0; i < cacheRegs.Length; i++)
                    {
                        //if(registed.这里暂没有考虑重复注册项的问题
                        if (cacheRegs[i].Item1 == register)
                        {
                            T[] ts = loader.LoadComponents(cacheRegs[i].Item2);
                            tls.AddRange(ts);
                            //if(registed.ContainsKey(cacheRegs[i].Item1))
                            //    registed[cacheRegs[i].Item1] = 
                            //else
                            //    registed.Add(cacheRegs[i].Item1, cacheRegs[i].Item2);
                        }
                    }
                    return tls.ToArray();
                }
            }
            return null;
        }

        /// <summary>
        /// 注册当前指定插件中指定注册器的注册项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="plugin"></param>
        /// <param name="register"></param>
        /// <returns></returns>
        public T[] Register<T>(Plugin plugin, string register) where T : class
        {
            Tuple<string, string>[] cacheRegs = plugin.Registers;
            if (cacheRegs == null || cacheRegs.Length == 0)
                return null;
            List<T> tl = new List<T>();
            using (IComponentLoader<T> loader = new ComponentLoader<T>())
            {
                for (int i = 0; i < cacheRegs.Length; i++)
                {
                    //if(registed.这里暂没有考虑重复注册项的问题
                    if (cacheRegs[i].Item1 == register)
                    {
                        T[] ts = loader.LoadComponents(cacheRegs[i].Item2);
                        tl.AddRange(ts);
                        //if(registed.ContainsKey(cacheRegs[i].Item1))
                        //    registed[cacheRegs[i].Item1] = 
                        //else
                        //    registed.Add(cacheRegs[i].Item1, cacheRegs[i].Item2);
                    }
                }
            }
            return tl.ToArray();
        }

        /// <summary>
        /// 获取当前加载的所有插件中指定类型的配置内容
        /// </summary>
        /// <param name="itemName">Theme子节点名字【即配置项】</param>
        /// <returns></returns>
        public XThemeItem[] GetConfigItems(string itemName)
        {
            List<XThemeItem> items = new List<XThemeItem>();
            if (_plugins == null || _plugins.Length == 0)
                return null;
            foreach (Plugin plugin in _plugins)
            {
                XThemeItem item = plugin.GetConfigItem(itemName);
                if (item != null)
                {
                    items.Add(item);
                }
            }
            return items.ToArray();
        }

        public void EnableChanged(Plugin plgin, bool enable)
        {
            //重新加载插件
            LoadPlugins();
            //通知更新
            if (OnEnableChanged != null)
                OnEnableChanged(plgin, null, enable);
        }
    }
}
