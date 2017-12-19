using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.ProjectDefine;
using GeoDo.Project;

namespace GeoDo.FileProject
{
    public partial class frmProjectionSetting : Form
    {
        private ProjectionSetting _set = null;
        private TreeNode _gllNode = null;

        public frmProjectionSetting()
        {
            InitializeComponent();
            LoadUserControl();
            LoadSetting();
        }

        private void LoadUserControl()
        {
            this.Text = "投影选项";
        }

        private void LoadSetting()
        {
            _set = new ProjectionSetting();
            checkBox1.Checked = _set.IsOpenProjected;
            checkBox2.Checked = _set.IsQuicklyProject;
        }

        private void ShowMorePrj()
        {
            SpatialReferenceSelection frm = new SpatialReferenceSelection();
            frm.ShowDialog(); 
        }

        private void SaveSetting()
        {
            _set.SaveToXML();
        }

        private void btnMorePrj_Click(object sender, EventArgs e)
        {
            ShowMorePrj();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSetting();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancle_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnNewTemplete_Click(object sender, EventArgs e)
        {
            frmSelectBands frm = new frmSelectBands(null, null);
            frm.ShowDialog();
        }
    }
}
