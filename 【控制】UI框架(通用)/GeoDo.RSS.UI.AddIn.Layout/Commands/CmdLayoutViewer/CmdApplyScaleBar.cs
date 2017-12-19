using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdApplyScaleBar : Command
    {
        public CmdApplyScaleBar()
            : base()
        {
            _id = 6666;
            _name = "ApplyScaleBar";
            _text = _toolTip = "应用比例尺";
        }

        public override void Execute(string argument)
        {
            ILayoutViewer viewer = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (viewer == null)
                return;
            if (string.IsNullOrEmpty(argument))
                return;
            ILayoutHost host = viewer.LayoutHost;
            if (host == null)
                return ;
            if (host.ActiveDataFrame == null)
                return ;
            float scale = float.Parse(argument);
            if (scale <= 0)
                return;
            host.ActiveDataFrame.LayoutScale = scale;
            viewer.LayoutHost.Render(true);
        }
    }
}
