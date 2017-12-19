using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public partial class frmProjectionMode : Form
    {
        private int _projectionMode = 1;

        private frmProjectionMode()
        {
            InitializeComponent();
        }

        public frmProjectionMode(bool hasAOI,bool hasBlock)
            :this()
        {
            btnAOI.Enabled = hasAOI;
            btnBlock.Enabled = hasBlock;
            if (hasAOI)
                btnAOI.Focus();
            else if (hasBlock)
                btnBlock.Focus();
        }

        /// <summary>
        /// 0:整规
        /// 1:AOI
        /// 2:分幅
        /// 3:交互
        /// </summary>
        public int ProjectionMode
        {
            get { return _projectionMode; }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
            Application.DoEvents();
        }

        private void btnFullFile_Click(object sender, EventArgs e)
        {
            Access(0);
        }

        private void btnAOI_Click(object sender, EventArgs e)
        {
            Access(1);
        }

        private void btnBlock_Click(object sender, EventArgs e)
        {
            Access(2);
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            Access(3);
        }

        private void Access(int projectionMode)
        {
            _projectionMode = projectionMode;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
            Application.DoEvents();
        }
    }
}
