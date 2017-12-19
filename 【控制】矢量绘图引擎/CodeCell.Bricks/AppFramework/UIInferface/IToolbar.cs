using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.Bricks.AppFramework
{
    public interface IToolbar:IItem
    {
        /// <summary>
        /// IItem只能是ICommand,ITool,IControlItem
        /// </summary>
        IItem[] Items { get;}
        /// <summary>
        /// 当前工具
        /// </summary>
        ICommand CurrentTool { get;set;}
        /// <summary>
        /// 查找工具
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ICommand FindCommand(Guid id);
        /// <summary>
        /// 查找工具
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ICommand FindCommand(string name);
        /// <summary>
        /// 查找工具
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        ICommand FindCommand(IItem item);
        /// <summary>
        /// 停靠位置
        /// </summary>
        DockStyle DockStyle { get; }
    }
}
