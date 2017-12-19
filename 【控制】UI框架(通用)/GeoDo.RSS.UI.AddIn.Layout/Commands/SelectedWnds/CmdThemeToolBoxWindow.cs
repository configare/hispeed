using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls.UI.Docking;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdThemeToolBoxWindow : Command
    {
        private const int WND_DEFAULT_WIDTH = 260;

        public CmdThemeToolBoxWindow()
        {
            _id = 5001;
            _name = "ThemeToolBox";
            _text = _toolTip = "整饰工具箱";
        }

        public override void Execute()
        {
            ISmartToolWindow wnd = _smartSession.SmartWindowManager.SmartToolWindowFactory.GetSmartToolWindow(_id);
            if (wnd != null)
            {
                _smartSession.SmartWindowManager.DisplayWindow(wnd, new WindowPosition(DockStyle.Left, true));
                (wnd as DockWindow).TabStrip.SizeInfo.AbsoluteSize = new System.Drawing.Size(WND_DEFAULT_WIDTH, 0);
            }
        }
    }
}
