using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using Telerik.WinControls.UI.Docking;
using System.Windows.Forms;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    [Export(typeof(ICommand))]
    public class CommandOverviewWnd : CommandToolWindow
    {
        public CommandOverviewWnd()
        {
            _id = 9007;
            _name = "CommandOverviewWnd";
            _text = _toolTip = "鹰眼视图";
        }

        public override void Execute()
        {
            ISmartToolWindow wnd = _smartSession.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(_id);
            if (wnd != null)
            {
                Form mainFrm = _smartSession.SmartWindowManager.MainForm as Form;
                int wndWidth = 260;
                int wndHeight = 200;
                int x = mainFrm.Right - wndWidth - 7;
                int y = mainFrm.Bottom - wndHeight - 8;
                Rectangle rect = new Rectangle(x, y, wndWidth, wndHeight);
                _smartSession.SmartWindowManager.DisplayWindow(wnd, new WindowPosition(System.Windows.Forms.DockStyle.None, false),rect);
            }
        }

        public override void Execute(string argument)
        {
            Execute();
        }
    }
}
