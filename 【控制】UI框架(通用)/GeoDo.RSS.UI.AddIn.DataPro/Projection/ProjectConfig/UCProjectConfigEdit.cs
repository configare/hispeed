using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public partial class UCProjectConfigEdit : UserControl, IConfigEdit
    {
        private ProjectConfig _prjCfg;

        public UCProjectConfigEdit()
        {
            InitializeComponent();
            this.Load += new EventHandler(UCProjectConfigEdit_Load);
        }

        private bool IsChanged()
        {
            string wk = _prjCfg.GetConfigValue("ProjectDir");
            string isUse = _prjCfg.GetConfigValue("IsUsed");
            if (string.IsNullOrEmpty(wk))
            {
                if (string.IsNullOrEmpty(txtDir.Text))
                    return false;
                else
                    return true;
            }
            return Path.GetFullPath(wk) != Path.GetFullPath(txtDir.Text) || isUse != ckbIsUsed.Checked.ToString();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog diag = new FolderBrowserDialog())
            {
                diag.Description = "设置投影输出文件夹根目录";
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectPath = Path.Combine(diag.SelectedPath, "Prj");
                    txtDir.Text = selectPath;
                }
            }
        }

        private void UCProjectConfigEdit_Load(object sender, EventArgs e)
        {
            _prjCfg = new ProjectConfig();
            txtDir.Text = _prjCfg.GetConfigValue("ProjectDir");
            bool isUsed;
            if (bool.TryParse(_prjCfg.GetConfigValue("IsUsed"), out isUsed))
            {
                txtDir.Enabled = btnOpen.Enabled = ckbIsUsed.Checked = isUsed;
            }
        }

        public bool TryCreateDir(string dir, out string message)
        {
            message = "";
            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch (IOException ex)
                {
                    message = ex.Message;
                    return false;
                }
            }
            else
            {
                bool writeRight = CheckDirectoryAccess(dir);
                if (!writeRight)
                {
                    message = "目录不具有写权限";
                    return false;
                }
            }
            return true;
        }

        private static bool CheckDirectoryAccess(string directory)
        {
            bool success = false;
            string fullPath = directory + "\\tempFile.tmp";
            if (Directory.Exists(directory))
            {
                try
                {
                    using (FileStream fs = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write))
                    {
                        fs.WriteByte(0xff);
                    }
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                        success = true;
                    }
                }
                catch (Exception)
                {
                    success = false;
                }
            }
            return success;
        }

        bool IConfigEdit.TrySaveConfig(out string message)
        {
            if (string.IsNullOrWhiteSpace(txtDir.Text))
            {
                message = "配置路径为空,使用默认投影路径！";
                return true;
            }
            if (!TryCreateDir(txtDir.Text, out message))
                return false;
            if (!IsChanged())
                return true;
            //保存至文件
            _prjCfg.SetConfig("ProjectDir", txtDir.Text);
            _prjCfg.SetConfig("IsUsed", ckbIsUsed.Checked.ToString());
            return _prjCfg.Save(out message);
        }

        private void ckbIsUsed_CheckedChanged(object sender, EventArgs e)
        {
            txtDir.Enabled = btnOpen.Enabled = ckbIsUsed.Checked;
        }

    }
}
