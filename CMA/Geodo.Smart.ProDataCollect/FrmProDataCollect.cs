using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.IO;
using System.Text.RegularExpressions;

namespace Geodo.Smart.ProDataCollect
{
    public partial class FrmProDataCollect : Form
    {
        private Dictionary<string, Dictionary<string, string>> _proDic = null;
        private string _argFile = AppDomain.CurrentDomain.BaseDirectory + "\\ProCollectArgs\\saveArgs.txt";
        private DataCollectArgs _collectArgs = null;
        private Dictionary<string, string> _catalogCN = new Dictionary<string, string>();
        private Regex _regex = null;
        private string _regexStr = @"\s*(?<date>\d{14})\s*";

        public FrmProDataCollect()
        {
            InitializeComponent();
            InitFrm();
            InitDate();
        }

        private void InitDate()
        {
            //初始化时间范围
            if (_collectArgs != null)
                ckCycTime.Checked = _collectArgs.isCyctime;
            if (ckCycTime.Checked)
                SetCyctime(DateTime.Now);
            else
            {
                dtpBegin.Value = DateTime.Now.AddDays(1 - (int)DateTime.Now.DayOfWeek);
                dtpEnd.Value = DateTime.Now.AddDays(7 - (int)DateTime.Now.DayOfWeek);
            }
        }

        private void InitFrm()
        {
            if (File.Exists(_argFile))
                _collectArgs = ReadArgs(_argFile);
            _proDic = CatalogCNHelper.GetDic();
            //初始化产品列表
            if (_proDic.Count != 0)
            {
                cbProduct.Items.AddRange(_proDic.Keys.ToArray());
                if (_collectArgs != null && _proDic.ContainsKey(_collectArgs.ProIdentify))
                    cbProduct.Text = _collectArgs.ProIdentify;
                else
                    cbProduct.SelectedIndex = 0;
            }
            //初始化工作空间
            if (_collectArgs != null && !string.IsNullOrEmpty(_collectArgs.WorkspaceDir))
                txtWorksapce.Text = _collectArgs.WorkspaceDir;
            else
            {
                MifConfig config = new MifConfig();
                txtWorksapce.Text = config.GetConfigValue("Workspace");
            }

            //初始化输出目录
            if (_collectArgs != null)
                txtOutdir.Text = _collectArgs.OutDirRoot;

            if (_collectArgs != null)
                ckRemove.Checked = _collectArgs.isRemove;
        }

        private void SetCyctime(DateTime dt)
        {
            switch (cbCycTime.Text)
            {
                case "周":
                    dtpBegin.Value = dt.AddDays(1 - (int)dt.DayOfWeek);
                    dtpEnd.Value = dt.AddDays(7 - (int)dt.DayOfWeek);
                    break;
                case "旬":
                    dtpBegin.Value = dt.Day > 20 ? dt.AddDays(21 - dt.Day) : dt.Day > 10 ? dt.AddDays(11 - dt.Day) : dt.AddDays(1 - dt.Day);
                    dtpEnd.Value = dt.Day > 20 ? dt.AddDays(DateTime.DaysInMonth(dt.Year, dt.Month) - dt.Day) : dt.Day > 10 ? dt.AddDays(20 - dt.Day) : dt.AddDays(10 - dt.Day);
                    break;
                case "月":
                    dtpBegin.Value = dt.AddDays(1 - (int)dt.Day);
                    dtpEnd.Value = dt.AddDays(DateTime.DaysInMonth(dt.Year, dt.Month) - (int)dt.Day);
                    break;
            }
        }

        private DataCollectArgs ReadArgs(string argFile)
        {
            string[] contexts = File.ReadAllLines(argFile, Encoding.Default);
            if (contexts == null || contexts.Length < 7)
                return null;
            _collectArgs = new DataCollectArgs();
            _collectArgs.ProIdentify = contexts[0];
            _collectArgs.SubProCatalogCN = contexts[1];
            _collectArgs.WorkspaceDir = contexts[2];
            _collectArgs.OutDirRoot = contexts[3];
            _collectArgs.isRemove = bool.Parse(contexts[4]);
            _collectArgs.isCyctime = bool.Parse(contexts[5]);
            _collectArgs.Cyctime = contexts[6];
            return _collectArgs;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            _catalogCN.Clear();
            cbsubProduct.Items.Clear();
            Dictionary<string, string> tempDic = _proDic[cbProduct.Text];
            if (tempDic.Count == 0)
                return;
            foreach (string key in tempDic.Keys)
                _catalogCN.Add(tempDic[key], key);
            _catalogCN.OrderBy(bef => bef.Key);
            cbsubProduct.Items.AddRange(_catalogCN.Keys.ToArray());
            if (_collectArgs != null && _catalogCN.ContainsKey(_collectArgs.SubProCatalogCN))
                cbsubProduct.Text = _collectArgs.SubProCatalogCN;
            else
                cbsubProduct.SelectedIndex = 0;
        }

        private void cbCycTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            //SetCyctime(dtpBegin.Value);
        }

        private void ckCycTime_CheckedChanged(object sender, EventArgs e)
        {
            cbCycTime.Enabled = ckCycTime.Checked;
        }

        private void btWorkspace_Click(object sender, EventArgs e)
        {
            SelectFolder(txtWorksapce);
        }

        private void SelectFolder(TextBox tb)
        {
            FolderBrowserDialog dilaog = new FolderBrowserDialog();
            if (dilaog.ShowDialog() == DialogResult.OK)
                tb.Text = dilaog.SelectedPath;
        }

        private void btOutdir_Click(object sender, EventArgs e)
        {
            SelectFolder(txtOutdir);
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (!CheckEnv())
                return;
            string date;
            string filenameWithoutExten;
            string dateStr = "yyyyMMdd";
            if (!Directory.Exists(txtWorksapce.Text + "\\" + cbProduct.Text))
            {
                MessageBox.Show("产品目录不存在!");
                return;
            }
            string[] files = Directory.GetFiles(txtWorksapce.Text + "\\" + cbProduct.Text, "*" + cbProduct.Text + "*" + _catalogCN[cbsubProduct.Text] + "*.*", SearchOption.AllDirectories).Where(
                file =>
                {
                    if (file.Contains("云烟数据"))
                        return false;
                    _regex = new Regex(_regexStr);
                    filenameWithoutExten = Path.GetFileNameWithoutExtension(file);
                    if (_regex.IsMatch(filenameWithoutExten))
                    {
                        date = _regex.Match(filenameWithoutExten).Groups["date"].Value;
                        if (date.CompareTo(dtpBegin.Value.ToString(dateStr) + "000000") > 0 &&
                            date.CompareTo(dtpEnd.Value.AddDays(1).ToString(dateStr) + "000000") < 0)
                            return true;
                        else
                            return false;
                    }
                    else
                        return false;

                }
                ).ToArray();
            if (files == null || files.Length == 0)
            {
                MessageBox.Show("目录下不存在指定时间范围的产品数据!");
                return;
            }

            string dstDir = txtOutdir.Text + "\\" + dtpBegin.Value.ToString("yyyyMMdd") + "-" + dtpEnd.Value.ToString("MMdd") +
                           "\\" + cbProduct.Text + "\\" + cbsubProduct.Text + "\\";
            if (!Directory.Exists(dstDir))
                Directory.CreateDirectory(dstDir);
            string dstFilename;
            files.OrderBy((bef) => bef.First());
            int count = 0;
            if (ckRemove.Checked)
                foreach (string file in files)
                {
                    dstFilename = Path.Combine(dstDir, Path.GetFileName(file));
                    if (!File.Exists(dstFilename))
                    {
                        File.Move(file, dstFilename);
                        count++;
                    }
                }
            else
                foreach (string file in files)
                {
                    dstFilename = Path.Combine(dstDir, Path.GetFileName(file));
                    if (!File.Exists(dstFilename))
                    {
                        File.Copy(file, dstFilename);
                        count++;
                    }
                }
            MessageBox.Show("完成数据拷贝,共处理数据" + count + "个!");
            this.Close();
        }

        private bool CheckEnv()
        {
            if (string.IsNullOrEmpty(cbProduct.Text))
            {
                MessageBox.Show("请选择产品名称!");
                return false;
            }
            if (string.IsNullOrEmpty(cbsubProduct.Text))
            {
                MessageBox.Show("请选择子产品名称!");
                return false;
            }
            if (string.IsNullOrEmpty(txtWorksapce.Text))
            {
                MessageBox.Show("请选择工作空间目录!");
                return false;
            }
            if (string.IsNullOrEmpty(txtOutdir.Text))
            {
                MessageBox.Show("请选择输出目录!");
                return false;
            }
            if (dtpBegin.Value > dtpEnd.Value)
            {
                MessageBox.Show("请正确设定时间范围!");
                return false;
            }
            return true;
        }

        private void btSaveArgs_Click(object sender, EventArgs e)
        {
            if (!CheckEnv())
                return;
            string path = Path.GetDirectoryName(_argFile);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string[] contexts = new string[]
            {
                cbProduct.Text,
                cbsubProduct.Text,
                txtWorksapce.Text,
                txtOutdir.Text,
                ckRemove.Checked.ToString(),
                ckCycTime.Checked.ToString(),
                cbCycTime.Text
            };
            File.WriteAllLines(_argFile, contexts, Encoding.Default);
            MessageBox.Show("参数文件已保存!");
        }

        private void btReadArgs_Click(object sender, EventArgs e)
        {
            if (File.Exists(_argFile))
            {
                InitFrm();
            }
            else
                MessageBox.Show("未保存过参数文件!");
        }

        private void dtpBegin_ValueChanged(object sender, EventArgs e)
        {
            SetCyctime(dtpBegin.Value);
        }
    }

    public class DataCollectArgs
    {
        public string ProIdentify;
        public string SubProCatalogCN;
        public string WorkspaceDir;
        public string OutDirRoot;
        public bool isRemove = false;
        public bool isCyctime = false;
        public string Cyctime;

        public DataCollectArgs()
        { }
    }
}
