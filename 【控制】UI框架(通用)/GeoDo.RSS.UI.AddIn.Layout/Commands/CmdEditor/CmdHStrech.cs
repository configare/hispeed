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
    public class CmdHStrech : GeoDo.RSS.Core.UI.Command
    {
        public CmdHStrech()
            : base()
        {
            _id = 6034;
            _name = "HStrech";
            _text = _toolTip = "横向填充";
        }

        public override void Execute()
        {
            ILayoutViewer view = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (view == null)
                return;
            ILayoutHost host = view.LayoutHost;
            if (host == null)
                return;
            host.Aligment(enumElementAligment.HorizontalStrech);
            host.Render();
        }
    }
}
