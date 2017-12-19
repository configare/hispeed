using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI.Docking;
using GeoDo.RSS.Core.UI;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.RasterDrawing;

namespace GeoDo.RSS.UI.AddIn.Windows
{
    [Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")]
    public partial class RasterPropertyWindow : ToolWindowBase
    {
        public RasterPropertyWindow()
            :base()
        {
            _id = 9003;
            Text = "波段选择";
            (_content as RasterPropertyWndContent).OnApplyClicked += new EventHandler(RasterPropertyWindow_OnApplyClicked);
       }

        void RasterPropertyWindow_OnApplyClicked(object sender, EventArgs e)
        {
            ICanvasViewer viewer = _session.SmartWindowManager.ActiveViewer as ICanvasViewer;
            if (viewer == null)
                return;
            IRasterDrawing drawing = viewer.ActiveObject as IRasterDrawing;
            if (drawing == null)
                return;
            drawing.SelectedBandNos = (_content as RasterPropertyWndContent).SelectedBandNos;
            viewer.Canvas.Refresh(Core.DrawEngine.enumRefreshType.All);
        }

        protected override IToolWindowContent GetToolWindowContent()
        {
            return new RasterPropertyWndContent();
        }
    }
}
