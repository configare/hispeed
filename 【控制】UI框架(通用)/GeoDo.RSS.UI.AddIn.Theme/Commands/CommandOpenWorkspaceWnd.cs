using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using Telerik.WinControls.UI.Docking;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    [Export(typeof(ICommand))]
    public class CommandOpenWorkspaceWnd : Command
    {
        private const int WND_DEFAULT_WIDTH = 320;

        public CommandOpenWorkspaceWnd()
            :base()
        {
            _id = 9020;
            _name = "CommandWorkspaceWindow";
            _text = _toolTip = "监测分析工作空间";
        }

        public override void Execute()
        {
            Execute(null);
        }

        public override void Execute(string argument)
        {
            ISmartToolWindow wnd = _smartSession.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(_id);
            if (wnd != null)
            {
                _smartSession.SmartWindowManager.DisplayWindow(wnd, new WindowPosition(DockStyle.Right, false));
                (wnd as DockWindow).TabStrip.SizeInfo.AbsoluteSize = new System.Drawing.Size(WND_DEFAULT_WIDTH, 0);
            }
        }
    }
}
