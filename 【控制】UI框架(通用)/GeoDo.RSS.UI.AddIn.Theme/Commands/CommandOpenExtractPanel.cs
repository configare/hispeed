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
    public class CommandOpenExtractPanel : Command
    {
        private const int WND_DEFAULT_WIDTH = 320;

        public CommandOpenExtractPanel()
        {
            _id = 9019;
            _name = "CommandExtractWindow";
            _text = _toolTip = "监测分析面板";
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
                if((wnd as DockWindow).TabStrip != null)
                    (wnd as DockWindow).TabStrip.SizeInfo.AbsoluteSize = new System.Drawing.Size(WND_DEFAULT_WIDTH, 0);
            }
        }
    }
}
