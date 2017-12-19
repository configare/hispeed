using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Help
{
    public partial class frmAboutGeodo : Form
    {
        public frmAboutGeodo()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
