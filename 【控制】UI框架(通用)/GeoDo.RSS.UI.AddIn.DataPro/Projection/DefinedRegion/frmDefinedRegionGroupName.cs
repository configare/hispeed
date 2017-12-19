using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public partial class frmDefinedRegionGroupName : Form
    {
        public frmDefinedRegionGroupName()
        {
            InitializeComponent();
        }

        public string GroupName
        {
            get { return textBox1.Text; }
        }

        public string GroupIdentify
        {
            get { return txtIdentify.Text; }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                errorProvider1.SetError(textBox1, "分组名称不能为空");
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            txtIdentify.Text = Chinese2PY.GetPYString(textBox1.Text);
        }
    }
}
