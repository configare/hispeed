using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using Telerik.WinControls.UI.Docking;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    [Export(typeof(ICommand))]
    public class CommandCursorWindow : CommandToolWindow
    {
        public CommandCursorWindow()
        {
            _id = 9000;
            _name = "CursorWindow";
            _text = _toolTip = "像元信息窗口";
        }

        protected override void DisplayAfter(ISmartToolWindow wnd)
        {
            (wnd as DockWindow).TabStrip.SizeInfo.AbsoluteSize = new System.Drawing.Size(250, 0);
        }
    }
}
