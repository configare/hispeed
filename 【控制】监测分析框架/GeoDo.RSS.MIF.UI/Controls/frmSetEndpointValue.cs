using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.UI
{
    public partial class frmSetEndpointValue : Form
    {
        public frmSetEndpointValue(double minValue, double maxValue)
        {
            InitializeComponent();
            Load += new EventHandler(frmSetEndpointValue_Load);
            doubleTextBox1.KeyDown += new KeyEventHandler(doubleTextBox1_KeyDown);
            doubleTextBox2.KeyDown += new KeyEventHandler(doubleTextBox2_KeyDown);
            doubleTextBox3.KeyDown += new KeyEventHandler(doubleTextBox3_KeyDown);
            doubleTextBox1.Text = minValue.ToString();
            doubleTextBox2.Text = maxValue.ToString();
            doubleTextBox3.Text = FineTuningValue.ToString();
        }

        void doubleTextBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                button1.Focus();
        }

        void doubleTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                doubleTextBox3.Focus();
        }

        void doubleTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                doubleTextBox2.Focus();
        }

        void frmSetEndpointValue_Load(object sender, EventArgs e)
        {
            doubleTextBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        public double MinValue
        {
            get { return double.Parse(doubleTextBox1.Text); }
        }

        public double MaxValue
        {
            get { return double.Parse(doubleTextBox2.Text); }
        }

        public double FineTuningValue
        {
            get { return double.Parse(doubleTextBox3.Text); }
        }

        private void frmSetEndpointValue_Load_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }
    }
}
