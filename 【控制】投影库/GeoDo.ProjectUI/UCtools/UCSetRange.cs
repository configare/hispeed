using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.ProjectUI.UCtools
{
    public partial class UCSetRange : UserControl
    {
        private Color _definedColor=SystemColors.GradientActiveCaption;
        private Color _otherColor = SystemColors.Control;



        public UCSetRange()
        {
            InitializeComponent();
            groupBox2.Visible = true;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            SetBackColor(tsbtnWhole);
            groupBox2.Visible = true;
            groupBox3.Visible = false;
            groupBox5.Visible = false;
            groupBox7.Visible = false;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            SetBackColor(tsbtnCenter);
            groupBox2.Visible = true;
            groupBox3.Visible = true;
            groupBox5.Visible = false;
            groupBox7.Visible = false;
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            SetBackColor(tsbtnCorners);
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            groupBox5.Visible = true;
            groupBox7.Visible = false;
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            SetBackColor(tsbtnDefined);
            groupBox2.Visible = false;
            groupBox3.Visible = false;
            groupBox5.Visible = false;
            groupBox7.Visible = true;
        }

        private void SetBackColor(ToolStripButton btnClicked)
        {
            tsbtnWhole.BackColor = _otherColor;
            tsbtnCenter.BackColor = _otherColor;
            tsbtnCorners.BackColor = _otherColor;
            tsbtnDefined.BackColor = _otherColor;
            btnClicked.BackColor = _definedColor;
        }
    }
}
