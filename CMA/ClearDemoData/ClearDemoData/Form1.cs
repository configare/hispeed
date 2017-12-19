using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ClearDemoData
{
    public partial class Form1 : Form
    {
        private string _intiFile;

        public Form1()
        {
            InitializeComponent();
            InitForm();
        }

        private void InitForm()
        {
            _intiFile = AppDomain.CurrentDomain.BaseDirectory + "\\InitFormFile.txt";
            if (!File.Exists(_intiFile))
                return;
            string[] context = File.ReadAllLines(_intiFile, Encoding.Default);
            if (context == null || context.Length != 3)
                return;
            txtDemo.Text = context[0];
            txtPython.Text = context[1];
            txtWorkspace.Text = context[2];
        }

        private void btDemo_Click(object sender, EventArgs e)
        {
            SelectDir(txtDemo);
        }

        private void SelectDir(TextBox txtBox)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
                txtBox.Text = dialog.SelectedPath;
        }

        private void btPython_Click(object sender, EventArgs e)
        {
            SelectDir(txtPython);
        }

        private void btWorkspace_Click(object sender, EventArgs e)
        {
            SelectDir(txtWorkspace);
        }

        private void btSetting_Click(object sender, EventArgs e)
        {
            if (!EnvIsOK())
                return;
            try
            {
                File.WriteAllLines(_intiFile, new string[] { txtDemo.Text, txtPython.Text, txtWorkspace.Text }, Encoding.Default);
                MessageBox.Show("保存信息成功!");
            }
            catch
            {
                MessageBox.Show("保存信息失败!");
            }
        }

        private bool EnvIsOK()
        {
            if (string.IsNullOrEmpty(txtDemo.Text) || !Directory.Exists(txtDemo.Text))
            {
                MessageBox.Show("请填写正确的演示数据路径!");
                return false;
            }
            if (string.IsNullOrEmpty(txtPython.Text) || !Directory.Exists(txtPython.Text))
            {
                MessageBox.Show("请填写正确的Python数据路径!");
                return false;
            }
            if (string.IsNullOrEmpty(txtWorkspace.Text) || !Directory.Exists(txtWorkspace.Text))
            {
                MessageBox.Show("请填写正确的工作空间数据路径!");
                return false;
            }
            return true;
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (!EnvIsOK())
                return;
            ClearDemoDataInfo();
            ClearPythonDataInfo();
            ClearWorkspaceDataInfo();
            MessageBox.Show("清理完毕!");
        }

        private void ClearWorkspaceDataInfo()
        {
            DeleteDirWithoutAny(txtWorkspace.Text + @"\FIR\", new string[] { "" });
            DeleteDirWithoutAny(txtWorkspace.Text + @"\FLD\", new string[] { "BackWaterFile" });
            DeleteDirWithoutAny(txtWorkspace.Text + @"\FOG\", new string[] { "HistroyCSRFile" });
            DeleteDirWithoutAny(txtWorkspace.Text + @"\SNW\", new string[] { "2013-05-01\\栅格产品" });

            DeleteDirWithoutAny(txtWorkspace.Text + @"\BAG\", new string[] { "" });
            DeleteDirWithoutAny(txtWorkspace.Text + @"\DRT\", new string[] { "" });
            DeleteDirWithoutAny(txtWorkspace.Text + @"\VGT\", new string[] { "" });
            DeleteDirWithoutAny(txtWorkspace.Text + @"\ICE\", new string[] { "" });
            DeleteDirWithoutAny(txtWorkspace.Text + @"\DST\", new string[] { "" });
            DeleteDirWithoutAny(txtWorkspace.Text + @"\UHI\", new string[] { "" });
            DeleteDirWithoutAny(txtWorkspace.Text + @"\LST\", new string[] { "" });
        }

        private void DeleteDirWithoutAny(string basePath, string[] subDir)
        {
            if (!Directory.Exists(basePath))
                return;
            string[] dirs = Directory.GetDirectories(basePath, "*.*", SearchOption.AllDirectories);
            if (dirs == null || dirs.Length == 0)
                return;
            List<string> dirList = new List<string>();
            dirList.AddRange(dirs);
            dirList.Sort(DescDir);
            string tempPath = string.Empty;
            for (int i = 0; i < subDir.Length; i++)
            {
                for (int j = 0; j < dirList.Count; j++)
                {
                    tempPath = Path.Combine(basePath, subDir[i]);
                    if (tempPath.ToUpper() != dirList[j].ToUpper() && tempPath.IndexOf(dirList[j]) == -1)
                        Directory.Delete(dirList[j], true);
                }
            }
        }

        private void ClearPythonDataInfo()
        {
            DeleteFilesWithoutEXT(txtPython.Text, new string[] { ".HDF" });
        }

        private void ClearDemoDataInfo()
        {
            DeleteFilesWithoutEXT(txtDemo.Text + "\\00_通用功能", new string[] { ".HDF" });
            DeleteFilesWithoutEXT(txtDemo.Text + "\\01_大雾", new string[] { ".HDF" });
            DeleteFilesWithoutEXT(txtDemo.Text + "\\02_沙尘", new string[] { ".HDF", ".JPG", ".JPW", ".XML" });
        }

        private void DeleteFilesWithoutEXT(string path, string[] exts)
        {
            string[] files = null;
            List<string> temp = new List<string>();
            temp.AddRange(exts);
            if (Directory.Exists(path))
            {
                files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                for (int i = 0; i < files.Length; i++)
                {
                    for (int ext = 0; ext < exts.Length; ext++)
                    {
                        if (!temp.Contains(Path.GetExtension(files[i]).ToUpper()))
                            File.Delete(files[i]);
                    }
                }
            }
        }

        public int DescDir(string dir1, string dir2)
        {
            if (dir1.CompareTo(dir2) > 0)
                return -1;
            else
                return 1;
        }
    }
}
