using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using GeoDo.RSS.MIF.Prds.CLD;

namespace Test
{
    public partial class DataMove : Form
    {
        Thread runTaskThread=null;
        private Action<int, string> _state = null;
        private bool _isDeleteRaw = false;
        private bool _isOverlapExist = false;

        public DataMove()
        {
            InitializeComponent();
            _state = new Action<int, string>(InvokeProgress);
        }

        private void InvokeProgress(int p, string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, string>((arg1, arg2) =>
                {
                    txtTips.Text = arg2.ToString();
                    LogFactory.WriteLine("数据迁移", arg2.ToString());
                    if (arg1 == -5)
                    {
                        this.Activate();
                        DialogResult it = MessageBox.Show(arg2.ToString() + "\r\n！", "异常提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        if (it != DialogResult.OK)
                        {
                            runTaskThread.Abort();
                            this.Close();
                        }
                    }
                    else if (arg1 != -1 && arg1 <= 100)
                    {
                        if (arg1 == 100 || arg1 == 0)
                            this.btnOK.Enabled = true;
                    }
                }), p, text);
            }
            else
            {
                txtTips.Text = text.ToString();
                LogFactory.WriteLine("数据迁移", text.ToString());
                if (p == -5)
                {
                    this.Activate();
                    DialogResult it = MessageBox.Show(text.ToString() + "\r\n请确认网络或磁盘可用！", "异常提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    if (it != DialogResult.OK)
                    {
                        runTaskThread.Abort();
                        this.Close();
                    }
                }
                else if (p != -1 && p <= 100)
                {
                    if (p == 100 || p == 0)
                        this.btnOK.Enabled = true;
                }
            }
        }

        private void btnOpenDir2move_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtDir2move.Text = dialog.SelectedPath;
            }
        }

        private void btnOpenToDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtToDir.Text = dialog.SelectedPath;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (runTaskThread != null)
                runTaskThread.Abort();
            this.Close();
        }

        private bool CheckArgsIsOk()
        {
            if (string.IsNullOrEmpty(txtDir2move.Text) || !Directory.Exists(txtDir2move.Text))
            {
                throw new ArgumentException("请正确输入待迁移数据所在文件夹！", "提示信息");
            }
            if (string.IsNullOrEmpty(txtToDir.Text))
            {
                throw new ArgumentException("请正确输入目标文件夹！", "提示信息");
            }
            if (!Directory.Exists(txtToDir.Text))
                Directory.CreateDirectory(txtToDir.Text);
            DirectoryInfo rawdir = new DirectoryInfo(txtDir2move.Text);
            DirectoryInfo aimdir = new DirectoryInfo(txtToDir.Text);
            if (rawdir.FullName == aimdir.FullName)
            {
                throw new ArgumentException("待迁移文件夹与目标文件夹相同，无法完成迁移！", "提示信息");
            }
            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                btnOK.Enabled = false;
                if (!CheckArgsIsOk())
                    return;
                _isDeleteRaw = cbxDeleteRawData.Checked;
                _isOverlapExist = cbxOverLapExist.Checked;
                //progressBar.Visible = true;
                runTaskThread = new Thread(new ThreadStart(this.CopyDir));
                runTaskThread.IsBackground = true;
                runTaskThread.Start();
            }
            catch (SystemException ex)
            {
                MessageBox.Show("处理出错：" + ex.Message);
                btnOK.Enabled = true;
            }
            finally
            {
                this.Activate();
                //btnOk.Enabled = true;
                //progressBar.Visible = true;
            }
        }

        public void CopyDir()
        {
            string srcPath=txtDir2move.Text;
            string aimPath = txtToDir.Text;
            CopyDir(srcPath, aimPath,_state);
            _state(100, "数据迁移完成！");
        }

        string fname,aimfname;
        private void CopyDir(string srcPath, string aimPath,Action<int, string> state)
        {
            try
            {
                //在指定目录及子目录下查找文件
                DirectoryInfo Dir = new DirectoryInfo(srcPath);
                foreach (DirectoryInfo d in Dir.GetDirectories())     //查找子目录   
                {
                    CopyDir(Path.Combine(Dir.FullName, d.ToString()), Path.Combine(aimPath, d.ToString()),state);
                }
                foreach (FileInfo f in Dir.GetFiles("*.*"))             //查找文件
                {
                    fname = Path.Combine(Dir.FullName, f.ToString());
                    if (!Directory.Exists(aimPath))
                        Directory.CreateDirectory(aimPath);
                    aimfname = Path.Combine(aimPath, f.ToString());
                    try
                    {
                        if (File.Exists(fname))
                        {
                            if (File.Exists(aimfname))
                            {
                                if (_isOverlapExist && _isDeleteRaw)
                                {
                                    File.Delete(aimfname);
                                    File.Move(fname, aimfname);
                                }
                                else if (!_isOverlapExist && _isDeleteRaw)
                                {
                                    File.Delete(fname);
                                }
                                else if (_isOverlapExist && !_isDeleteRaw)
                                {
                                    File.Delete(aimfname);
                                    File.Copy(fname, aimfname, _isOverlapExist);
                                }
                                else
                                    continue;
                            }
                            else
                            {
                                if (_isDeleteRaw)
                                    File.Move(fname, aimfname);
                                else
                                    File.Copy(fname, aimfname, _isOverlapExist);
                            }
                            state(-1,"迁移成功：" + fname +"!");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        state(-1, "迁移失败：" + fname + "!"+ex.Message);
                    }
                }
                if (Directory.Exists(srcPath)&&_isDeleteRaw)
                {
                    Directory.Delete(srcPath);
                    state(-1, "删除成功：" + srcPath + "!");
                }
            }
            catch (Exception e)
            {
                state(-5,e.ToString());
            }
        }
    }
}
