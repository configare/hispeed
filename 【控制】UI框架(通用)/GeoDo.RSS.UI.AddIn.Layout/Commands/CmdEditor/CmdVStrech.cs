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
    public class CmdVStrech : GeoDo.RSS.Core.UI.Command
    {
        public CmdVStrech()
            : base()
        {
            _id = 6035;
            _name = "VStrech";
            _text = _toolTip = "纵向填充";
        }

        public override void Execute()
        {
            ILayoutViewer view = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (view == null)
                return;
            ILayoutHost host = view.LayoutHost;
            if (host == null)
                return;
            host.Aligment(enumElementAligment.VerticalStrech);
            host.Render();
        }
    }
}
