using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.UHI
{
    public partial class UCLayoutShpFileSetting : UserControl
    {
        private string _txtFileName = null;
        private bool _isOutputShp = true;

        public UCLayoutShpFileSetting()
        {
            InitializeComponent();
        }

        public string FileName
        {
            get
            {
                return _txtFileName;
            }
        }

        public bool IsOutputShp
        {
            get
            {
                return _isOutputShp;
            }
        }

        private void rbtNo_CheckedChanged(object sender, EventArgs e)
        {
            btnOpen.Enabled = !rbtNo.Checked;
            _isOutputShp = !rbtNo.Checked;
            if (rbtNo.Checked)
                txtFname.Text = "";
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Multiselect = false;
                dialog.Filter = "矢量点文件(*.txt)|*.txt;所有文件(*.*)|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtFname.Text = dialog.FileName;
                    _txtFileName = txtFname.Text;
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!ArgOk())
            {
                labTip.Visible = true;
                return;
            }
        }

        private bool ArgOk()
        {
            if(rbtYes.Checked)
            {
                if (string.IsNullOrEmpty(txtFname.Text))
                    return false;
            }
            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
        }


    }
}
