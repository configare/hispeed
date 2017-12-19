using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CodeCell.Bricks.AppFramework
{
    /// <summary>
    /// 所有菜单、工具栏、命令、工具都实现该接口
    /// </summary>
    public interface IItem
    {
        string ToolTips { get;}
        string Name { get; set; }
        string Text { get;}
        Guid Id { get;}
        bool Visible { get; set; }
        bool AllowDock { get; set; }
        ToolStripItemDisplayStyle DisplayStyle { get; }
        IItem[] Children { get; }
    }
}
