using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CodeCell.Bricks.AppFramework
{
    public interface ICommand:IItem
    {
        Image Image { get;}
        bool BeginGroup { get;}
        bool Enabled { get;}
        /// <summary>
        /// 对于ITool该属性无效
        /// 对于ICommand只有将其当到菜单上才有效
        /// </summary>
        bool Checked { get;}
        void Click();
        void Init(IHook hook);
        IHook Hook { get;}
    }
}
