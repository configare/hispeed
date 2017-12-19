using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using Telerik.WinControls.UI.Docking;
using System.Windows.Forms;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.Python
{
    [Export(typeof(ICommand))]
    public class CommandOpenScriptManager : Command
    {
        public CommandOpenScriptManager()
        {
            _id = 20001;
            _name = "PythonManager";
            _text = _toolTip = "脚本管理";
        }

        public override void Execute()
        {
            ISmartToolWindow wnd = _smartSession.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(_id);
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory + "SystemData\\Scripts";
                if (Directory.Exists(dir))
                    (wnd as IPythonManagerWindow).SetupWorkspace(dir);
            }
            if (wnd != null)
                _smartSession.SmartWindowManager.DisplayWindow(wnd, new WindowPosition(DockStyle.Left, true));
        }

        public override void Execute(string argument)
        {
            base.Execute();
        }
    }
}
