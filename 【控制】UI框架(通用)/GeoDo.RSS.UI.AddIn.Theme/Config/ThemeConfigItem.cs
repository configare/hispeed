using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public class ThemeConfigItem
    {
        public string ThemeConfigFile;
        public bool Visible = true;

        public ThemeConfigItem() { }

        public ThemeConfigItem(string themeConfigFile, bool visible)
        {
            ThemeConfigFile = themeConfigFile;
            Visible = visible;
        }
    }
}
