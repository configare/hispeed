using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GeoDo.RSS.MIF.Core
{
    public partial class frmAddNewCustomAOISolution : Form
    {
        private string[] _SolutionName;
        private bool _isAddMode = false;
        private bool _isDeleteMode = false;
        List<string> _deleteslList = new List<string>();
        private string _addedSolution;
        public Form ParentForm = null;

        public frmAddNewCustomAOISolution(string[] solutionNames)
        {
            InitializeComponent();
            Load += new EventHandler(frmAddNewCustomAOISolution_Load);
            _SolutionName = solutionNames;
            if (_SolutionName != null)
            {
                foreach (string slname in _SolutionName)
                    listView1.Items.Add(slname);
            }
        }

        void frmAddNewCustomAOISolution_Load(object sender, EventArgs e)
        {
            if (ParentForm != null)
                this.Location = new Point(ParentForm.Location.X + ParentForm.Width, ParentForm.Location.Y);
        }

        public bool IsAddMode
        {
            get
            {
                return _isAddMode;
            }
        }

        public bool IsDeleteMode
        {
            get
            {
                return _isDeleteMode;
            }
        }

        public string[] DeletedSolutions
        {
            get { return _deleteslList.ToArray(); }
        }

        public string NewCustomAOISolutionName
        {
            get { return _addedSolution; }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtNewSolutionName.Text) || string.IsNullOrWhiteSpace(txtNewSolutionName.Text))
                    throw new ArgumentException("请输入正确的方案名称!");
                foreach (ListViewItem item in listView1.Items)
                {
                    if (txtNewSolutionName.Text.ToUpper() == item.Text.ToUpper())
                        throw new ArgumentException("已存在相同的方案名称！");
                }
                listView1.Items.Add(txtNewSolutionName.Text);
                _addedSolution = txtNewSolutionName.Text;
                _isAddMode = true;
                btnAdd.Enabled = false;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("请选择待删除的方案名称！");
                return;
            }
            foreach (ListViewItem item in listView1.SelectedItems)
            {
                listView1.Items.Remove(item);
                _isDeleteMode = true;
                if (!_deleteslList.Contains(item.Text))
                    _deleteslList.Add(item.Text);
            }
            listView1.Update();
        }
    }
}
