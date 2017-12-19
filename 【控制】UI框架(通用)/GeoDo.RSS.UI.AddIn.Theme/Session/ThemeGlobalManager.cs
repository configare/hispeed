using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.UI;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public static class ThemeGlobalManager
    {
        public static void Register(ThemeConfigItem[] items)
        {
            if (items == null || items.Length == 0)
                return;
            foreach (ThemeConfigItem it in items)
            {
                try
                {
                    if (!File.Exists(it.ThemeConfigFile))
                        return;
                    if (it.Visible)
                        Register(it.ThemeConfigFile);
                }
                catch(Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        public static void Register(string fname)
        {
            MonitoringThemeFactory.MergerTheme(fname);
        }
    }
}
