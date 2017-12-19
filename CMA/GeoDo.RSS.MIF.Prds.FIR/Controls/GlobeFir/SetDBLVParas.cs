using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public partial class SetDBLVParas : Form
    {
        public double _resl = 1;

        public SetDBLVParas(string outfname)
        {
            InitializeComponent();
            txtOutname.Text = outfname;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            double resl;
            if (double.TryParse(txtOutResl.Text,out resl))
            {
                _resl = resl;
            }
        }
    }
}
