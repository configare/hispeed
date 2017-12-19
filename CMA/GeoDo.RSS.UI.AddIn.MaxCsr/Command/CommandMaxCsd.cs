using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.MaxCsr
{
    [Export(typeof(ICommand))]
    public class CommandMaxCsd : Command
    {
        public CommandMaxCsd()
        {
            _id = 4501;
            _name = "MaxCsd";
            _text = "计算晴空反射率";
            _toolTip = "计算晴空反射率";
        }

        public override void Execute()
        {
            try
            {
                using (frmMaxCsd frm = new frmMaxCsd())
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
