using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Help
{
    [Export(typeof(ICommand))]
    class CommandAbout:Command
    {
        public CommandAbout()
            : base()
        {
            _id = 21005;
            _text = _toolTip = "关于..";
        }

        public override void Execute()
        {
            //using (frmAboutGeodo frm = new frmAboutGeodo())
            using (frmAbout frm = new frmAbout())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                    return;
            }
        }
    }
}
