#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：admin     时间：2013-09-13 15:41:56
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
using GeoDo.RSS.UI.AddIn.Windows;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    /// <summary>
    /// 类名：pageSearchCondition
    /// 属性描述：栅格数据查询条件窗口
    /// 创建者：罗战克   创建日期：2013-09-13 15:41:56
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    [Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")]
    public partial class pageSearchCondition : ToolWindowBase, ISmartToolWindow
    {
        public pageSearchCondition()
            : base()
        {
            _id = 39003;
            Text = "栅格数据查询条件窗口";
        }

        protected override IToolWindowContent GetToolWindowContent()
        {
            return new ucSearchCondition();
        }
        
        public IToolWindowContent ToolWindowContent
        {
            get { return _content; }
        }

        public void SetArgument(string argument)
        {
            if (_content == null)
            {
                _content = GetToolWindowContent();
            }
            (_content as ucSearchCondition).SetFileType(argument);
        }
    }
}
