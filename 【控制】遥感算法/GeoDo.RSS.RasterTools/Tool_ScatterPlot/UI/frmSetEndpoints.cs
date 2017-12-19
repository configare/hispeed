using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.RasterTools
{
    public partial class frmSetEndpoints : Form
    {
        public frmSetEndpoints()
        {
            InitializeComponent();
        }

        public void SetEndPoints(double minX,double maxX,double minY,double maxY)
        {
            txtMinX.Text = minX.ToString("0.####");
            txtMaxX.Text = maxX.ToString("0.####");
            txtMinY.Text = minY.ToString("0.####");
            txtMaxY.Text = maxY.ToString("0.####");
        }

        public double MinX
        {
            get { return double.Parse(txtMinX.Text); }
        }

        public double MaxX
        {
            get { return double.Parse(txtMaxX.Text); }
        }

        public double MinY
        {
            get { return double.Parse(txtMinY.Text); }
        }

        public double MaxY
        {
            get { return double.Parse(txtMaxY.Text); }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            double v;
            if (!double.TryParse(txtMinX.Text, out v))
            {
                txtMinX.Focus();
                txtMinX.SelectAll();
                return;
            }
            if (!double.TryParse(txtMaxX.Text, out v))
            {
                txtMaxX.Focus();
                txtMaxX.SelectAll();
                return;
            }
            if (!double.TryParse(txtMinY.Text, out v))
            {
                txtMinY.Focus();
                txtMinY.SelectAll();
                return;
            }
            if (!double.TryParse(txtMaxY.Text, out v))
            {
                txtMaxY.Focus();
                txtMaxY.SelectAll();
                return;
            }
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }
    }
}
