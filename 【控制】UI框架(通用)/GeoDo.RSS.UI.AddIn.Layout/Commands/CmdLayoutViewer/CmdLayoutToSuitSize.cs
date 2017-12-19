using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdLayoutToSuitSize : Command
    {
        public CmdLayoutToSuitSize()
            : base()
        {
            _id = 6002;
            _name = "LayoutToSuitSize";
            _text = _toolTip = "适合大小";
        }

        public override void Execute()
        {
            ILayoutViewer host = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (host != null)
                host.LayoutHost.ToSuitedSize();
        }
    }
}
