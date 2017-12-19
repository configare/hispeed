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
    public partial class FrmProject7 : Form
    {
        public FrmProject7()
        {
            InitializeComponent();
            panela1.Visible = false;
            panelb1.Visible = false;
            panelc1.Visible = false;
            paneld1.Visible = false;
            panele1.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetVisible(panela1);
            panela.Height = panela1.Visible ? 250 : 30;
            this.Height = panela1.Visible ? SetPlusHeight(220) : SetMinusHeight(220);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetVisible(panelb1);
            panelb.Height = panelb1.Visible ? 100 : 30;
            this.Height = panelb1.Visible ? SetPlusHeight(70) : SetMinusHeight(70);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SetVisible(panelc1);
            panelc.Height = panelc1.Visible ? 250 : 30;
            this.Height = panelc1.Visible ? SetPlusHeight(220) : SetMinusHeight(220);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SetVisible(paneld1);
            paneld.Height = paneld1.Visible ? 250 : 30;
            this.Height = paneld1.Visible ? SetPlusHeight(220) : SetMinusHeight(220);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SetVisible(panele1);
            panele.Height = panele1.Visible ? 300 : 30;
            this.Height = panele1.Visible ? SetPlusHeight(270) : SetMinusHeight(270);
        }

        private int SetPlusHeight(int value)
        {
            return this.Height + value;
        }

        private int SetMinusHeight(int value)
        {
            return this.Height - value;
        }

        private void SetVisible(Panel panel)
        {
            panel.Visible = !panel.Visible;
        }
    }
}
