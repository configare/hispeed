using System;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.DST
{
    public partial class FormNewFileName : Form
    {
        public FormNewFileName()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        public string FileName {get { return textBox1.Text; } set { textBox1.Text = value; }}
    }
}
