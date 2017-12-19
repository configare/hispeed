#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-4-10 11:29:43
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
using GeoDo.MicroWaveSnow.AviPlayer;
using AviFile;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Theme;
using System.ComponentModel.Composition;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：cmdAVIPlayer
    /// 属性描述：
    /// 创建者：LiXJ   创建日期：2014-4-10 11:29:43
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class cmdAVIPlayer : CommandProductBase
    {
        public cmdAVIPlayer()
             : base()
        {
            _id = 36605;
            _name = "cmdAVIPlayer";
            _text = _toolTip = "制作AVI动画";
        }

        public override void Execute()
        {
            AVIControlWindow frm = new AVIControlWindow();
            frm.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            frm.Show();
        }
    }
}
