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
    public partial class UCSetOutRange2 : UserControl
    {
        public UCSetOutRange2()
        {
            InitializeComponent();
            groupBox2.Visible = true;
            groupBox3.Visible = true;
            groupBox2.Visible = true;
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            groupBox2.Visible = true;
            groupBox3.Visible = false;
            groupBox5.Visible = false;
            groupBox7.Visible = false;
            //if(tabPage1.==true)
        }
    }
}
