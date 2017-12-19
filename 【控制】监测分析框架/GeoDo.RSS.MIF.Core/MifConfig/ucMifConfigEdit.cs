using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace GeoDo.RSS.MIF.Core
{
    public partial class ucMifConfigEdit : UserControl, IConfigEdit
    {
        private MifConfig _mifConfig = null;

        public ucMifConfigEdit()
        {
            InitializeComponent();
            this.Load += new EventHandler(ucMifConfigEdit_Load);
        }

        void ucMifConfigEdit_Load(object sender, EventArgs e)
        {
            _mifConfig = new MifConfig();
            textBox1.Text = _mifConfig.GetConfigValue("Workspace");
            textBox2.Text = _mifConfig.GetConfigValue("TEMP");
            textBox3.Text = _mifConfig.GetConfigValue("Report");
        }

        private void btnWorkspaceDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog diag = new FolderBrowserDialog())
            {
                diag.Description = "设置系统工作文件夹根目录";
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectPath = Path.Combine(diag.SelectedPath, "Workspace");
                    textBox1.Text = selectPath;
                }
            }
        }

        private void btnCacheDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog diag = new FolderBrowserDialog())
            {
                diag.Description = "设置系统缓存文件夹根目录";
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectPath = Path.Combine(diag.SelectedPath, "Temp");
                    textBox2.Text = selectPath;
                }
            }
        }

        public bool TrySaveConfig(out string message)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) || string.IsNullOrWhiteSpace(textBox2.Text))
            {
                message = "配置路径不能为空";
                return false;
            }
            if (!TryCreateDir(textBox1.Text, out message) || !TryCreateDir(textBox2.Text, out message)||
                !TryCreateDir(textBox3.Text, out message))
                return false;
            if (!IsChanged())
                return true;
            //保存至文件
            _mifConfig.SetConfig("Workspace", textBox1.Text);
            _mifConfig.SetConfig("TEMP", textBox2.Text);
            _mifConfig.SetConfig("Report", textBox3.Text);
            return _mifConfig.Save(out message);
        }

        private bool IsChanged()
        {
            string wk = _mifConfig.GetConfigValue("Workspace");
            string chache = _mifConfig.GetConfigValue("TEMP");
            string report = _mifConfig.GetConfigValue("Report");
            return Path.GetFullPath(wk) != Path.GetFullPath(textBox1.Text) || Path.GetFullPath(chache) != Path.GetFullPath(textBox2.Text) ||
                 Path.GetFullPath(report) != Path.GetFullPath(textBox3.Text);
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

        /// <summary>
        /// Checks the ability to create and write to a file in the supplied directory.
        /// </summary>
        /// <param name="directory">String representing the directory path to check.</param>
        /// <returns>True if successful; otherwise false.</returns>
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

        private void btnReportDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog diag = new FolderBrowserDialog())
            {
                diag.Description = "设置报告素材文件夹根目录";
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string selectPath = Path.Combine(diag.SelectedPath, "Report");
                    textBox3.Text = selectPath;
                }
            }
        }
    }
}
