using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class frmPlot : UserControl
    {
        public ucCloudsatPlot plots = null;

        public frmPlot()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
            plots = new ucCloudsatPlot();
            plots.Dock = DockStyle.Fill;
            this.panel1.Controls.Add(plots);
            this.AutoScroll = true;
        }

        void frmPlot_Load(object sender, EventArgs e)
        {
        }

        public void Reset(string filename, Bitmap img, double minx, double maxx, double miny, double maxy, string[] yaxis, Action<int, string> progressTracker)
        {
            plots.Height = img.Height;
            plots.Width = img.Width;
            plots.AddYAxis(yaxis);
            if (img!=null)
                plots.Reset(filename, img, minx, maxx, miny, maxy, null);
            plots.Rerender();
        }

        private void btnSetEndPoints_Click(object sender, EventArgs e)
        {
            using (frmSetEndpoints frm = new frmSetEndpoints())
            {
                frm.StartPosition = FormStartPosition.CenterParent;
                if (frm.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    double minH = frm.MinH;
                    double maxH = frm.MaxH;
                    ;
                }
            }
        }
    }
}
