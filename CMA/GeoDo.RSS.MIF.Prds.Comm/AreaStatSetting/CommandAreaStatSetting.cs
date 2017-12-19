using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    [Export(typeof(ICommand))]
    public class CommandAreaStatSetting : Command
    {
        public CommandAreaStatSetting()
            : base()
        {
            _id = 19021;
            _name = "CommandAreaStatSetting";
            _text = "统计分析设置";
        }

        public override void Execute()
        {
            using (frmAreaStatSetting frm = new frmAreaStatSetting())
            {
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.ShowDialog();
            }        
        }
    }
}
