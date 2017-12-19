using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.MathAlg;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.RasterTools
{
    public partial class frmScatterGraph : Form,IScatterPlotTool
    {
        public frmScatterGraph()
        {
            InitializeComponent();
            SizeChanged += new EventHandler(frmScatterGraph_SizeChanged);
        }

        void frmScatterGraph_SizeChanged(object sender, EventArgs e)
        {
        }

        public void Reset(Core.DF.IRasterDataProvider dataProvider, int xBandNo, int yBandNo,int[] aoi,LinearFitObject fitObj, Action<int, string> progressTracker)
        {
            ucScatterGraph1.Reset(dataProvider, xBandNo, yBandNo,aoi,fitObj,progressTracker);
        }

        public void Reset(IRasterDataProvider dataProvider, int xBandNo, int yBandNo,int[] aoi,XYAxisEndpointValue endpointValues, LinearFitObject fitObj, Action<int, string> progressTracker)
        {
            ucScatterGraph1.Reset(dataProvider, xBandNo, yBandNo,aoi, endpointValues, fitObj, progressTracker);
        }

        public void Reset(Core.DF.IRasterDataProvider dataProvider, int xBandNo, int yBandNo, double[] xBandInvalidValue, double[] yBandInvalidValue, int[] aoi, LinearFitObject fitObj, Action<int, string> progressTracker)
        {
            ucScatterGraph1.Reset(dataProvider, xBandNo, yBandNo,xBandInvalidValue,yBandInvalidValue, aoi, fitObj, progressTracker);
        }

        public void Rerender()
        {
            this.Refresh();
            ucScatterGraph1.Rerender();
        }

        private void frmScatterGraph_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                ucScatterGraph1.AbortRendering();
            }
        }

        private void frmScatterGraph_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
           
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "Bitmap Files(*.bmp)|*.bmp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    ucScatterGraph1.ExportBitmap(dlg.FileName);
                }
            }
        }

        private void ucScatterGraph1_LinearFitFinished(object sender, EventArgs e)
        {
            txtFitResult.Text = string.Empty;
            LinearFitObject fitObj = sender as LinearFitObject;
            if (fitObj != null)
            {
                txtFitResult.Text = string.Format("f(x) = {0} + {1}x", fitObj.a.ToString("0.####"), fitObj.b.ToString("0.####")) + " , R² = " + fitObj.r2.ToString("0.####");
                if (txtFitResult.Text.Contains("+ -"))
                    txtFitResult.Text = txtFitResult.Text.Replace("+ -", "- ");
            }
        }

        private void txtFitResult_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void btnSetEndPoints_Click(object sender, EventArgs e)
        {
            using (frmSetEndpoints frm = new frmSetEndpoints())
            {
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.SetEndPoints(ucScatterGraph1._minValueXBand, ucScatterGraph1._maxValueXBand, ucScatterGraph1._minValueYBand, ucScatterGraph1._maxValueYBand);
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ucScatterGraph1.SetEndPointValues(frm.MinX, frm.MaxX, frm.MinY, frm.MaxY);
                }
            }
        }
    }
}
