using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CodeCell.Bricks.AppFramework
{
    public interface IControlItem:ICommand
    {
        /// <summary>
        /// 控件尺寸
        /// </summary>
        Size Size { get;}
        /// <summary>
        /// 控件对象
        /// 必须是Toolbar对象可以接收的组件(即：从ToolStripItem派生的组件)
        /// </summary>
        object Control { get;}
        /// <summary>
        /// 如果是ToolStripDrioDown类型，则该值有效
        /// </summary>
        IItem[] Items { get;}
    }
}
