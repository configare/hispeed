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
    public partial class frmSetEndpoints : Form
    {
        private ErrorProvider _error = new ErrorProvider();
        private double _minH = 0;
        private double _maxH = 20;

        public frmSetEndpoints()
        {
            InitializeComponent();
        }

        public double MinH
        {
            get { return _minH; }
            set 
            { 
                _minH = value;
                txtMinH.Text = value.ToString();
            }
        }

        public double MaxH
        {
            get { return _maxH; }
            set
            {
                _maxH = value;
                txtMaxH.Text = value.ToString();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (ArgCheck())
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ArgCheck()
        {
            double minH, maxH;
            if (!double.TryParse(txtMinH.Text, out minH))
            {
                _error.SetError(txtMinH, "不合法");
                return false;
            }
            if (!double.TryParse(txtMaxH.Text, out maxH))
            {
                _error.SetError(txtMaxH, "不合法");
                return false;
            }
            _minH = minH;
            _maxH = maxH;
            return true;
        }
    }
}
