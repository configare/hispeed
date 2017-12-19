using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.WinForm
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandConfig : Command
    {
        public CommandConfig()
            : base()
        {
            _id = 2;
            _text = _name = "系统配置";
        }

        public override void Execute()
        {
            using (frmConfig frm = new frmConfig())
            {
                frm.StartPosition = FormStartPosition.CenterParent;
                if (frm.ShowDialog(_smartSession.SmartWindowManager.MainForm as IWin32Window) == DialogResult.OK)
                { 
                }
            }
        }
    }
}
