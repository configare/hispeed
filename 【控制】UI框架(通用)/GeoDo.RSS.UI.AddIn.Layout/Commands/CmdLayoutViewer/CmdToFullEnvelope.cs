using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Layout.GDIPlus;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    [Export(typeof(ICommand)), ExportMetadata("VERSION", "1")]
    public class CmdToFullEnvelope : Command
    {
        public CmdToFullEnvelope()
            : base()
        {
            _id = 6099;
            _name = "CmdToFullEnvelope";
            _text = _toolTip = "缩放到全图";
        }

        public override void Execute()
        {
            ILayoutViewer view = _smartSession.SmartWindowManager.ActiveViewer as ILayoutViewer;
            if (view == null)
                return;
            if (view.LayoutHost == null || view.LayoutHost.ActiveDataFrame == null)
                return;
            IDataFrameDataProvider df = view.LayoutHost.ActiveDataFrame.Provider as IDataFrameDataProvider;
            if (df != null)
            {
                df.Canvas.SetToFullEnvelope();
                (view.LayoutHost).Render(true);
            }
        }
    }
}
