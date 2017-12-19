using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    public partial class frmCustomIceDegree : Form
    {
        public frmCustomIceDegree()
        {
            InitializeComponent();
        }

        public float X;
        public float Y;

        private void btOK_Click(object sender, EventArgs e)
        {
            float tempX = 0.0f;
            float tempY = 0.0f;
            if (string.IsNullOrEmpty(txtX.Text) || string.IsNullOrEmpty(txtY.Text)
                || !float.TryParse(txtY.Text, out tempY)
                || !float.TryParse(txtX.Text, out tempX)
                || tempY == 0 || tempX == 0)
            {
                errorProvider1.SetError(groupBox1, "请检查输入的XY方向值是否正确!");
                return;
            }
            X = tempX;
            Y = tempY;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btCancle_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtX_TextChanged(object sender, EventArgs e)
        {
            if (ckbSynch.Checked)
                txtY.Text = txtX.Text;
        }
    }
}
