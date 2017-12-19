using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.ProjectUI;
namespace TestForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GeoDo.ProjectUI.FrmProject pro = new GeoDo.ProjectUI.FrmProject();
            pro.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FrmProject1cs pro = new GeoDo.ProjectUI.FrmProject1cs();
            pro.Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FrmProject3 pro = new FrmProject3();
            pro.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FrmProject4 pro = new FrmProject4();
            pro.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FrmProject5 pro = new FrmProject5();
            pro.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {

            FrmProject6 pro = new FrmProject6();
            pro.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            FrmProject7 pro = new FrmProject7();
            pro.Show();
        }
    }
}
