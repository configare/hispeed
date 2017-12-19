using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.UI.AddIn.Windows;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    [Export(typeof(ISmartToolWindow)), ExportMetadata("VERSION", "1")]
    public class CloudsatPlotWnd : ToolWindowBase, ISmartToolWindow
    {
        private frmPlot _ctrl;

        public CloudsatPlotWnd()
            : base()
        {
            _id = 29001;
            Text = "Cloudsat剖面图";
            AddControls();
        }

        private void AddControls()
        {
            _ctrl = new frmPlot();
            _ctrl.Dock = DockStyle.Fill;
            this.Controls.Add(_ctrl);
        }

        public void Reset(string filename, Bitmap img, double minx, double maxx, double miny, double maxy, string[] yaxis, Action<int, string> progressTracker)
        {
            _ctrl.Reset(filename, img, minx, maxx, miny, maxy, yaxis, null);
        }
    }
}
