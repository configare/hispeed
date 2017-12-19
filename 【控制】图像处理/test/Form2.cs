using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.CA;

namespace test
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            Load += new EventHandler(Form2_Load);
        }

        void Form2_Load(object sender, EventArgs e)
        {
            //frmLogEnhance log = new frmLogEnhance();
            frmExponentEnhanceII log = new frmExponentEnhanceII();
            log.Show();
        }
    }
}
