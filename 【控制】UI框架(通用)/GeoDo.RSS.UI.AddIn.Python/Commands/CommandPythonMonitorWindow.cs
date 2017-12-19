using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using Telerik.WinControls.UI.Docking;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Python
{
    [Export(typeof(ICommand))]
    public partial class CommandOpenScriptMonitor : Command
    {
        public CommandOpenScriptMonitor()
        {
            _id = 20002;
            _name = "PythonMonitorWindow";
            _text = _toolTip = "脚本监测";
        }

        public override void Execute()
        {
            ISmartToolWindow wnd = _smartSession.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(_id);
            if (wnd != null)
                _smartSession.SmartWindowManager.DisplayWindow(wnd, new WindowPosition(DockStyle.Bottom, false));
        }

        public override void Execute(string argument)
        {
            Execute();
        }
    }
}
