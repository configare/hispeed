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
    class CommandContacts : Command
    {
        public CommandContacts()
            : base()
        {
            _id = 21004;
            _text = _toolTip = "联系我们..";
        }

        public override void Execute()
        {
            //using (frmContactGeodo frm = new frmContactGeodo())
            using (frmContact frm = new frmContact())
            {
                if (frm.ShowDialog() == DialogResult.OK)
                    return;
            }
        }
    }
}
