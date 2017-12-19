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
    public class CommandFeatureListWnd : CommandToolWindow
    {
        public CommandFeatureListWnd()
        {
            _id = 9008;
            _name = "CommandFeatureListWnd";
            _text = _toolTip = "矢量特征列表";
        }

        protected override void DisplayAfter(ISmartToolWindow wnd)
        {
            base.DisplayAfter(wnd);
        }
    }
}
