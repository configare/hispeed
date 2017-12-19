using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.Bricks.AppFramework
{
    public class ShortcutProviderDefault:IShortcutProvider
    {
        public ShortcutProviderDefault()
        { 
        }

        public static IShortcutProvider LoadFrom(string xmlFile)
        {
            throw new Exception("该方法还未实现！");
        }

        #region IShortcutProvider 成员

        public Shortcut GetShortcut(string commandName)
        {
            switch (commandName)
            {
                case "cmdDelete":
                    return Shortcut.Del;
            }
            return 0;
        }

        #endregion
    }
}
