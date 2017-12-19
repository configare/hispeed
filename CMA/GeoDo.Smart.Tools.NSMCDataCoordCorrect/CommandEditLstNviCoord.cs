#region Version Info
/*========================================================================
* 功能概述：
* 修改lst、nvi角度及投影属性的Command
* 创建者：罗战克     时间：2013-08-23 10:02:14
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;

namespace GeoDo.Smart.Tools.NSMCDataCoordCorrect
{
    /// <summary>
    /// 类名：CommandEditLstNviCoord
    /// 属性描述：修改植被指数、陆表温度投影坐标信息
    /// 创建者：luozhanke   创建日期：2013-08-23 10:02:14
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    [Export(typeof(ICommand))]
    public class CommandEditLstNviCoord : Command
    {
        public CommandEditLstNviCoord()
        {
            _id = 31100;
            _name = "EditLstNviCoord";
            _text = "修改植被指数、陆表温度投影坐标信息";
            _toolTip = "修改植被指数、陆表温度投影坐标信息";
        }

        public override void Execute()
        {
            try
            {
                base.Execute();
                using (Main frm = new Main())
                {
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
