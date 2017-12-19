using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class frmUniformDataManage : Form
    {
        private static string _dataBaseXml = ConnectMySqlCloud._dataBaseXml;
        private string _dataDocDir;
        private Action<string> _state;
        ConnectMySqlCloud _DBcon;
        private string[] _UniformDataDir = new string[] { "原始数据", "5分钟段产品", "日拼接产品", "周期合成产品"};
        private Dictionary<string,string> _data2DBTable = new Dictionary<string,string>();
        private DataBaseArg arg;
        Thread runTaskThread = null;

        public frmUniformDataManage()
        {
            InitializeComponent();
            InitSetting();
            InitTask();
        }

        private void InitSetting()
        {
            _data2DBTable.Add("ISCCP", "cp_isccpd2_tb");
            _data2DBTable.Add("MODIS", "cp_modismod06_tb");
            _data2DBTable.Add("AIRS", "cp_airs_tb");
            _data2DBTable.Add("CLOUDSAT", "cp_cloudsat_tb");
            _data2DBTable.Add("日拼接产品", "cp_daymergeproducts_tb");
            _data2DBTable.Add("周期合成产品", "cp_periodicsynthesis_tb");
            if (!File.Exists(_dataBaseXml))
            {
                MessageBox.Show("数据库配置文件不存在，请先配置数据库！");
                return;
            }
            _DBcon = new ConnectMySqlCloud(_dataBaseXml);
            arg = DataBaseArg.ParseXml(_dataBaseXml);
            _dataDocDir = arg.OutputDir;
        }

        private void InitTask()
        {
            Control.CheckForIllegalCrossThreadCalls = false;//关闭该异常检测的方式来避免异常的出现
            _state = new Action<string>(InvokeProgress);
        }

        private void InvokeProgress(string text)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<string>((arg) =>
                {
                    txtErrorLog.AppendText(arg.ToString() + "\r\n");
                }), text);
            }
            else
            {
                txtErrorLog.AppendText(text.ToString() + "\r\n");
            }
        }

        private void txtErrorLog_TextChanged(object sender, EventArgs e)
        {
            txtErrorLog.Focus();
            txtErrorLog.Select(txtErrorLog.Text.Length, 0);
            txtErrorLog.ScrollToCaret();
        }

        private void frmUniformDataManage_Load(object sender, EventArgs e)
        {
            try
            {
                DirectoryInfo basedir = new DirectoryInfo(_dataDocDir);
                List<string> validprdDir = new List<string>();
                foreach (DirectoryInfo childdir in basedir.GetDirectories())
                {
                    string subdir =childdir.Name;
                    if (_UniformDataDir.Contains(subdir))
                    {
                        validprdDir.Add(subdir);
                    }
                }
                int validcount =validprdDir.Count;
                if (validcount!=0)
                {
                    long[] ids = new long[validcount];
                    this.ucRadioBoxList1.ResetContent(ids, validprdDir.ToArray());
                } 
                else
                {
                    this.ucRadioBoxList1.ResetContent(null, null);
                    MessageBox.Show("选择的目录下没有可清理的数据！");
                    return;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                btnOK.Enabled = false;
                if (!Directory.Exists(_dataDocDir))
                    throw new ArgumentException("归档目录不存在，请重新设置！");
                if (!CheckArgsIsOK())
                    return;
                runTaskThread = new Thread(new ThreadStart(ClearSelectedData));
                runTaskThread.IsBackground = true;
                if (_state != null)
                    _state(string.Format("开始清理归档数据，请稍候..."));
                runTaskThread.Start();
                this.Activate();
            }
            catch (System.Exception ex)
            {
                txtErrorLog.Text += "错误信息:" + ex.Message + "\r\n";
                MessageBox.Show("错误信息:" + ex.Message);
                btnOK.Enabled = true;
            }
            finally
            {
                btnOK.Enabled = true;
            }
        }

        private bool CheckArgsIsOK()
        {
            if (string.IsNullOrEmpty(ucRadioBoxList1.CheckedName))
            {
                throw new ArgumentException("请选择产品类别！");
            }
            if (ucCheckBoxListSets.CheckedIds.Length==0)
            {
                throw new ArgumentException("请选择产品名称！");
            }
            return true;
        }

        private void ClearSelectedData()
        {
            string[] files2clear;
            string dir2becleared;
            string basedir = _dataDocDir;
            string prdsdi = ucRadioBoxList1.CheckedName;
            dir2becleared = Path.Combine(_dataDocDir, prdsdi);
            ScanDir2Files(dir2becleared);
            files2clear = Directory.GetFiles(dir2becleared, "*.*", SearchOption.TopDirectoryOnly);
            ScanDirFilesCount(dir2becleared, files2clear.Length);
            DeleteFiles(files2clear);
            foreach (string set in ucCheckBoxListSets.CheckedNames)
            {
                dir2becleared = Path.Combine(_dataDocDir, prdsdi, set);
                try
                {
                    ScanDir2Files(dir2becleared);
                    files2clear = Directory.GetFiles(dir2becleared, "*.*", SearchOption.AllDirectories);
                    ScanDirFilesCount(dir2becleared, files2clear.Length);
                    if (files2clear.Length == 0)
                        continue;
                    if (prdsdi == "原始数据")
                    {
                        DeleteFiles(files2clear, true, _data2DBTable[set]);
                    }
                    else if (prdsdi == "5分钟段产品")
                    {
                        DeleteFiles(files2clear);
                    }
                    else
                        DeleteFiles(files2clear, true, _data2DBTable[prdsdi]);
                }
                catch (System.Exception ex)
                {
                    if (_state != null)
                        _state(dir2becleared+":"+ex.Message);
                }
            }
            if (_state != null)
                _state("数据清理完成！");
        }

        private void ScanDir2Files(string dir)
        {
            if (_state != null)
                _state("正在扫描\"" + dir + "\"下的待清理文件...");
        }

        private void ScanDirFilesCount(string dir,int length)
        {
            if (_state != null)
                _state("\""+dir + "\"下共" + length + "个待清理文件...");
        }

        private void DeleteFiles(string [] files2clear,bool isUpdateDB=false,string tablename=null)
        {
            foreach (string file in files2clear)
            {
                try
                {
                    System.IO.File.Delete(file);
                    if (_state != null)
                        _state(file + "清理完成！");
                    if (isUpdateDB && tablename != null && _DBcon.IshasRecord(tablename, "ImageName", Path.GetFileName(file)))
                    {
                        _DBcon.DeleteCLDParatableRecord(tablename, "ImageName", Path.GetFileName(file));
                        if (_state != null)
                            _state(file + "数据库记录清理完成！");
                    }
                }
                catch (System.Exception ex)
                {
                    continue;
                }
            }
            if (isUpdateDB&&tablename.ToLower() == "cp_cloudsat_tb")
            {
                _DBcon.UpdateLink();
            } 
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (runTaskThread != null)
                runTaskThread.Abort();
            this.Close();
        }

        private void ucRadioBoxList1_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                DirectoryInfo basedir = new DirectoryInfo(Path.Combine(_dataDocDir, ucRadioBoxList1.CheckedName));
                List<string> validprdDir = new List<string>();
                foreach (DirectoryInfo childdir in basedir.GetDirectories())
                {
                    validprdDir.Add(childdir.Name);
                }
                int validcount = validprdDir.Count;
                if (validcount != 0)
                {
                    long[] ids = new long[validcount];
                    this.ucCheckBoxListSets.ResetContent(ids, validprdDir.ToArray());
                }
                else
                {
                    this.ucCheckBoxListSets.ResetContent(null, null);
                    if (Directory.GetFiles(basedir.FullName,"*.*").Length==0)
                        throw new ArgumentException("当前路径下不存在可清理的文件！");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
