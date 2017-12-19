using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.FileProject;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    [Export(typeof(ICommand))]
    public class CmdProjectionSetting : Command
    {
        public CmdProjectionSetting()
        {
            _id = 4000;
            _name = "ProjectionSetting";
            _text = "投影配置";
            _toolTip = "投影配置";
        }

        public override void Execute()
        {
            frmProjectionSetting frm = new frmProjectionSetting();
            if (frm.ShowDialog(_smartSession.SmartWindowManager.MainForm as System.Windows.Forms.Form) == System.Windows.Forms.DialogResult.OK)
            { 
            }
        }

        public override void Execute(string argument)
        {
            base.Execute(argument);
        }
    }
}
