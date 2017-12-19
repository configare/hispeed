using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Core
{
    public partial class frmApplyCustomAOISolution : Form
    {
        private string [] _slsnames=null;
        public Form ParentForm = null;

        public frmApplyCustomAOISolution(string[] slsNames)
        {
            InitializeComponent();
            Load += new EventHandler(frmApplyCustomAOISolution_Load);
            _slsnames = slsNames;
            if (_slsnames == null)
                return;
            foreach (string sls in _slsnames)
            {
                this.listView1.Items.Add(sls);
            }
        }

        void frmApplyCustomAOISolution_Load(object sender, EventArgs e)
        {
            if (ParentForm != null)
                this.Location = new Point(ParentForm.Location.X + ParentForm.Width, ParentForm.Location.Y);
        }

        public string SelectedSolutionName
        {
            get
            {
                return this.listView1.SelectedItems[0].Text;
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count!=1)
            {
                MessageBox.Show("请正确选择解决方案名称!");
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
