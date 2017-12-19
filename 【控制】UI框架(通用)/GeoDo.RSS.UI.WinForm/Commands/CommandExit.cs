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
    public class CommandExit : Command
    {
        public CommandExit()
            : base()
        {
            _id = -1;
            _text = _name = "退出";
        }

        public override void Execute()
        {
            if(MsgBox.ShowQuestionYesNo("确定要退出应用程序吗?") == DialogResult.Yes)
                (_smartSession.SmartWindowManager.MainForm as Form).Close();
        }
    }
}
