using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.MIF.Prds.VGT;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CommandToolSmooth : CommandProductBase
    {
        public CommandToolSmooth()
            : base()
        {
            _id = 6603;
            _name = "ToolSmooth";
            _text = _toolTip = "数据平滑";
        }

        public override void Execute()
        {
            ISmartWindow smartWindow = _smartSession.SmartWindowManager.GetSmartWindow((w) => { return w.GetType() == typeof(frmSmoothToolWindows); });
            if (smartWindow == null)
                smartWindow = new frmSmoothToolWindows();
            _smartSession.SmartWindowManager.DisplayWindow(smartWindow);
        }
    }
}
