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
    public class CommandRasterPropertyWindow : CommandToolWindow
    {
        public CommandRasterPropertyWindow()
        {
            _id = 9003;
            _name = "RasterPropertyWindow";
            _text = _toolTip = "波段选择窗口";
        }
    }
}
