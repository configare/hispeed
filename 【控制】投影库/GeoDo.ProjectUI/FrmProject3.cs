using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;

namespace GeoDo.ProjectUI
{
    public partial class FrmProject3 : Form
    {
        public FrmProject3()
        {
            InitializeComponent();
            if (DesignMode)
                return;
            this.Height = 320;
            tsbAdd.Image = imageList1.Images[0];
            stbRemove.Image = imageList1.Images[1];
            tsbClear.Image = imageList1.Images[2];
            btnDo.Image = imageList1.Images[3];
            btnQuit.Image = imageList1.Images[4];
            btnOutFile.Image = imageList1.Images[6];
            radioButton1.Checked = true;
            groupBox2.Visible = true;
            groupBox3.Visible = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            gBoxParas.Visible = !gBoxParas.Visible;
            this.Height = gBoxParas.Visible ? 616 : 320;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Visible = true;
            groupBox3.Visible = true;
            groupBox5.Visible = false;
            groupBox7.Visible = false;
            textBox12.ReadOnly = true;
            textBox11.ReadOnly = true;
            textBox5.ReadOnly = true;
            textBox6.ReadOnly = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Visible = true;
            groupBox3.Visible = true;
            groupBox5.Visible = false;
            groupBox7.Visible = false;
            textBox12.ReadOnly = false;
            textBox11.ReadOnly = false;
            textBox5.ReadOnly = false;
            textBox6.ReadOnly = false;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            groupBox5.Visible = true;
            groupBox7.Visible = false;
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            groupBox5.Visible = false;
            groupBox7.Visible = true;
        }
    }
}
