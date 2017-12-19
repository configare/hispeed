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
    public partial class FrmProject4 : Form
    {
        private bool _btn3Open = false;
        private bool _btn4Open = false;
        private bool _btn2Open = false;

        public FrmProject4()
        {
            InitializeComponent();
            if (DesignMode)
                return;
            this.Height = 390;
            groupBox6.Visible = true;
            groupBox8.Visible = true;
            radioButton6.Checked = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            _btn3Open = !_btn3Open;
            this.Height = _btn3Open ? 620 : 390;
            panel4.Visible = _btn3Open;
            panel5.Visible = false;
            treeView1.Visible = false;
            _btn2Open = false;
            _btn4Open = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _btn4Open = !_btn4Open;
            this.Height = _btn4Open ? 620 : 390;
            panel5.Visible = _btn4Open;
            panel4.Visible = false;
            treeView1.Visible = false;
            _btn3Open = false;
            _btn2Open = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _btn2Open = !_btn2Open;
            this.Height = _btn2Open ? 620 : 390;
            treeView1.Visible = _btn2Open;
            panel4.Visible = false;
            panel5.Visible = false;
            _btn3Open = false;
            _btn4Open = false;
        }

        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            groupBox6.Visible = true;
            groupBox8.Visible = true;
            groupBox9.Visible = false;
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            groupBox6.Visible = true;
            groupBox8.Visible = true;
            groupBox9.Visible = false;
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            groupBox6.Visible = false;
            groupBox8.Visible = false;
            groupBox9.Visible = true;
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            groupBox6.Visible = false;
            groupBox8.Visible = false;
            groupBox9.Visible = false;
        }
    }
}
