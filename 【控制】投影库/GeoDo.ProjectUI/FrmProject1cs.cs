using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.ProjectUI
{
    public partial class FrmProject1cs : Form
    {
        public FrmProject1cs()
        {
            InitializeComponent();
            btnDo.Image = imageList1.Images[0];
            toolStripButton1.Image = imageList1.Images[1];
            toolStripButton2.Image = imageList1.Images[6];
            toolStripButton3.Image = imageList1.Images[2];
            this.Height = 320;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            gBoxParas.Visible = !gBoxParas.Visible;
            this.Height = gBoxParas.Visible ? 700 : 320;
        }
    }
}
