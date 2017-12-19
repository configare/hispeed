using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.Core.UI;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public partial class frmAutoGeneratorSetting : Form
    {
        public frmAutoGeneratorSetting()
        {
            InitializeComponent();
        }

        public AutoGeneratorSettings Settings
        {
            get 
            {
                return CollectSettings();
            }
            set 
            {
                FillSettings(value);
            }
        }

        private AutoGeneratorSettings CollectSettings()
        {
            AutoGeneratorSettings settings = new AutoGeneratorSettings();
            settings.IsOutputGxd = ckIsOuputGXD.Checked;
            settings.IsOutputPng = ckIsOuputPng.Checked;
            if(ckIsCopyToFolder.Checked)
                settings.FolderOfCopyTo = txtCopyFolder.Text;
            settings.OpenFileAfterFinished = rdOpenAfteFinisehd.Checked;
            settings.NeedSaveSettings = cmIsMemSettings.Checked;
            if (rdOveride.Checked)
                settings.ActionOfExisted = AutoGeneratorSettings.enumActionOfExisted.Overide;
            else if (rdRename.Checked)
                settings.ActionOfExisted = AutoGeneratorSettings.enumActionOfExisted.ReName;
            else if (rdSkip.Checked)
                settings.ActionOfExisted = AutoGeneratorSettings.enumActionOfExisted.Skip;
            return settings;
        }

        private void FillSettings(AutoGeneratorSettings value)
        {
            if (value == null)
                return;
            ckIsOuputGXD.Checked = value.IsOutputGxd;
            ckIsOuputPng.Checked = value.IsOutputPng;
            ckIsCopyToFolder.Checked = string.IsNullOrEmpty(value.FolderOfCopyTo );
            rdOpenAfteFinisehd.Checked = value.OpenFileAfterFinished;
            cmIsMemSettings.Checked = value.NeedSaveSettings;
            switch (value.ActionOfExisted)
            {
                case AutoGeneratorSettings.enumActionOfExisted.Overide:
                    rdOveride.Checked = true;
                    break;
                case AutoGeneratorSettings.enumActionOfExisted.ReName:
                    rdRename.Checked = true;
                    break;
                case AutoGeneratorSettings.enumActionOfExisted.Skip:
                    rdSkip.Checked = true;
                    break;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!ArgIsOK())
                return;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private bool ArgIsOK()
        {
            if (ckIsCopyToFolder.Checked)
            {
                string folder = txtCopyFolder.Text;
                if (string.IsNullOrEmpty(folder))
                {
                    MsgBox.ShowInfo("复制文件目标文件夹没有设置！");
                    txtCopyFolder.Focus();
                    txtCopyFolder.SelectAll();
                    return false;
                }
                try
                {
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                }
                catch (Exception ex)
                {
                    MsgBox.ShowError(ex.Message);
                    txtCopyFolder.Focus();
                    txtCopyFolder.SelectAll();
                    return false;
                }
            }
            return true;
        }

        private void ckIsCopyToFolder_CheckedChanged(object sender, EventArgs e)
        {
            txtCopyFolder.Enabled = ckIsCopyToFolder.Checked;
            btnOpenFolder.Enabled = txtCopyFolder.Enabled;
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtCopyFolder.Text = dlg.SelectedPath;
                }
            }
        }
    }
}
