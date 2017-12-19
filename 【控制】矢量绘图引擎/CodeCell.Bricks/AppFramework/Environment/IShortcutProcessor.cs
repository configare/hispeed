using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.Bricks.AppFramework
{
    /// <summary>
    /// 快捷键执行器
    /// 所有的按键都被发送到该执行器
    /// </summary>
    public interface IShortcutProcessor
    {
        void AddShortcutFilter(IShortcutFilter filter);
        void AddShortcutFilters(IShortcutFilterCollection filters);
        void PreviewKeyDown(PreviewKeyDownEventArgs previewKeyDownEventArgs);
    }
}
