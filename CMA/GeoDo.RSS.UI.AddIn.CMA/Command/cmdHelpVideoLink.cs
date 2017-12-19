#region Version Info
/*========================================================================
* 功能概述：视频帮助命令
* 
* 创建者：admin     时间：2013-08-30 10:47:20
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
using System.Diagnostics;
using System.Windows.Forms;
using System.Security.Policy;

namespace GeoDo.RSS.UI.AddIn.CMA
{
    /// <summary>
    /// 类名：cmdHelpVideoLink
    /// 属性描述：视频帮助命令
    /// 创建者：罗战克   创建日期：2013-08-30 10:47:20
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    [Export(typeof(ICommand))]
    public class cmdHelpVideoLink : Command
    {
        public cmdHelpVideoLink()
        {
            _id = 21006;
            _name = _text = "视频链接";
        }

        public override void Execute(string argument)
        {
            if (string.IsNullOrWhiteSpace(argument))
                return;
            try
            {
                Process.Start(argument);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示");
            }
        }
    }
}
