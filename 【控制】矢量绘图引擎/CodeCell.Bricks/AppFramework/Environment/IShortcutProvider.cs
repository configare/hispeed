using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.Bricks.AppFramework
{
    /// <summary>
    /// 快捷键提供者
    /// 所有的快捷键由这个接口负责提供
    /// 可以通过这个接口实现所有快捷键的配置
    /// </summary>
    public interface IShortcutProvider
    {
        Shortcut GetShortcut(string commandName);
    }
}
