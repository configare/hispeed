using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;


namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class PeriodComp : Form
    {
        private static string _periodPrdsArgsXml = AppDomain.CurrentDomain.BaseDirectory + @"SystemData\ProductArgs\CLD\PeriodPrdsArgs.xml";
        private static string _dataBaseXml = AppDomain.CurrentDomain.BaseDirectory + @"SystemData\ProductArgs\CLD\CPDataBaseArgs.xml";
        InputArg _args;
        string _sensor;
        private Action<int, string> _state = null;
        Thread runTaskThread = null;
        private string _periodPrdsArgs = null;
        private DataBaseArg _dbargs;
        private int _singleyear;
        private string[] _checkedSetNames = null;

        public PeriodComp(string sensor)
        {
            InitializeComponent();
            _sensor = sensor;
            InitTask();
        }

        private void InitTask()
        {
            _state = new Action<int, string>(InvokeProgress);
        }

        private void InvokeProgress(int p, string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, string>((arg1, arg2) =>
                {
                    txtTips.Text = arg2.ToString().Replace("\t", "");
                    LogFactory.WriteLine(string.Format("{0}周期合成", _sensor), arg2.ToString());
                    if (arg1 == -5)
                    {
                        this.Activate();
                        LogFactory.WriteLine(string.Format("{0}周期合成Error", _sensor), arg2.ToString().Replace("\t", ""));
                        DialogResult it = MessageBox.Show(arg2.ToString() + "\r\n！", "异常提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                        if (it != DialogResult.OK)
                        {
                            if (runTaskThread != null)
                                runTaskThread.Abort();
                            this.progressBar1.Value = 0;
                            this.btnOK.Enabled = true;
                            txtTips.Text = "周期合成取消！";
                            LogFactory.WriteLine(string.Format("{0}周期合成", _sensor), "周期合成取消！");
                            //this.Close();
                        }
                    }
                    else if (arg1 != -1 && arg1 <= 100)
                    {
                        this.progressBar1.Value = arg1;
                        if (arg1 == 100 || arg1 == 0)
                            this.btnOK.Enabled = true;
                    }
                }), p, text);
            }
            else
            {
                txtTips.Text = text.ToString().Replace("\t", "");
                LogFactory.WriteLine(string.Format("{0}周期合成", _sensor), text.ToString());
                if (p == -5)
                {
                    this.Activate();
                    LogFactory.WriteLine(string.Format("{0}周期合成Error", _sensor), text.ToString().Replace("\t", ""));
                    DialogResult it = MessageBox.Show(text.ToString() + "\r\n请确认网络或磁盘可用！", "异常提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                    if (it != DialogResult.OK)
                    {
                        if (runTaskThread!=null)
                            runTaskThread.Abort();
                        this.progressBar1.Value = 0;
                        this.btnOK.Enabled = true;
                        txtTips.Text = "周期合成取消！";
                        LogFactory.WriteLine(string.Format("{0}周期合成", _sensor), "周期合成取消！");
                        //this.Close();
                    }
                }
                else if (p != -1 && p <= 100)
                {
                    this.progressBar1.Value = p;
                    if (p == 100 || p == 0)
                        this.btnOK.Enabled = true;
                }
            }
        }

        private void ResetDataTimeAndSets()
        {
            string[] setsNames = QueryPrds2dataset(_sensor);
            if (setsNames != null)
            {
                string daydir = Path.Combine(txtInputDir.Text, "日拼接产品");
                List<string> validsets = new List<string>();
                foreach (string set in setsNames)
                {
                    if (Directory.Exists(Path.Combine(daydir, set.Replace("_", ""))) && !validsets.Contains(set))
                        validsets.Add(set);
                }
                if (validsets.Count > 0)
                {
                    long[] ids = new long[validsets.Count];
                    this.ucCheckBoxListDataSet.ResetContent(ids, validsets.ToArray());
                }
                else
                    this.ucCheckBoxListDataSet.ResetContent(null, null);
            }
            else
                this.ucCheckBoxListDataSet.ResetContent(null, null);
        }

        private string[] QueryPrds2dataset(string sensor)
        {
            ConnectMySqlCloud con = new ConnectMySqlCloud();
            if (sensor.ToUpper() == "MOD06" )
            {
                this.combxYears.Items.Clear();
                for (int i = 2000; i <= DateTime.Now.Year; i++)
                {
                    this.combxYears.Items.Add(i.ToString());
                }
                return con.QueryMOD06setsWithPrdsID();
            }
            else if (sensor.ToUpper() == "MYD06")
            {
                this.combxYears.Items.Clear();
                for (int i = 2000; i <= DateTime.Now.Year; i++)
                {
                    this.combxYears.Items.Add(i.ToString());
                }
                return con.QueryMYD06setsWithPrdsID();
            }

            else if (sensor.ToUpper() == "AIRS")
            {
                this.combxYears.Items.Clear();
                for (int i = 2002; i <= DateTime.Now.Year; i++)
                {
                    this.combxYears.Items.Add(i.ToString());
                }
                return con.QueryAIRSsetsWithPrdsID();
            }
            return null;
        }

        public bool ParseArgsXml()
        {
            if (File.Exists(_dataBaseXml))
            {
                _dbargs = DataBaseArg.ParseXml(_dataBaseXml);
                string rootpath = null;
                if (_sensor.ToUpper() == "MOD06" || _sensor.ToUpper() == "MYD06")
                    rootpath = _dbargs.OutputDir;
                else if (_sensor.ToUpper() == "AIRS")
                    rootpath = _dbargs.AIRSRootPath;
                else if (_sensor.ToUpper() == "ISCCP")
                    rootpath = _dbargs.ISCCPRootPath;
                else
                    return false;
                if (!Directory.Exists(rootpath))
                    throw new ArgumentException("配置文件路径" + rootpath + "未找到,请重试！");
                txtInputDir.Text = rootpath;
                txtOutputDir.Text = rootpath;
                if (_sensor.ToUpper() == "MOD06")
                    radibtnMOD06.Checked = true;
                else if (_sensor.ToUpper() == "AIRS")
                    radibtnAIRS.Checked = true;
                else if (_sensor.ToUpper() == "MYD06")
                    radibtnMYD06.Checked = true;
            }
            else
            {
                MessageBox.Show("数据库配置文件不存在！");
            }
            if (File.Exists(_periodPrdsArgsXml))
            {
                _args = InputArg.ParsePeriodArgsXml(_periodPrdsArgsXml);
                _args.InputDir = txtInputDir.Text;
                _args.OutputDir = txtOutputDir.Text;
                if (_args.StatisticsTypes.Contains("AVG"))
                    cbxAVG.Checked = true;
                if (_args.StatisticsTypes.Contains("MIN"))
                    cbxMin.Checked = true;
                if (_args.StatisticsTypes.Contains("MAX"))
                    cbxMax.Checked = true;
                if (_args.PeriodTypes.Contains("TEN"))
                    cbxTen.Checked = true;
                if (_args.PeriodTypes.Contains("MONTH"))
                {
                    cbxTen.Checked = true;
                    cbxMonth.Checked = true;
                }
                if (_args.PeriodTypes.Contains("YEAR"))
                {
                    cbxTen.Checked = true;
                    cbxMonth.Checked = true;
                    cbxYear.Checked = true;
                }
                if (_args.OverWriteHistoryFiles)
                    cbxOverlap.Checked = true;
                return true;
            }
            else
                return false;
        }

        private bool CheckArgsIsOk()
        {
            if (string.IsNullOrEmpty(txtInputDir.Text) || !Directory.Exists(txtInputDir.Text))
            {
                throw new ArgumentException("请正确输入日拼接所在文件夹！", "提示信息");
            }
            if (string.IsNullOrEmpty(txtOutputDir.Text) || !Directory.Exists(txtOutputDir.Text))
            {
                throw new ArgumentException("请正确输入周期合成所在文件夹！", "提示信息");
            }
            string aimdir = Path.Combine(txtOutputDir.Text, "周期合成产品");
            if (!Directory.Exists(aimdir))
                Directory.CreateDirectory(aimdir);
            if (!cbxAVG.Checked && !cbxMax.Checked && !cbxMin.Checked)
                throw new ArgumentException("请选择周期合成计算类型！", "提示信息");
            if (!cbxMonth.Checked && !cbxYear.Checked)
                throw new ArgumentException("请选择周期合成周期类型！", "提示信息");
            if (combxYears.SelectedIndex == -1)
                throw new ArgumentException("请正确选择年份！", "提示信息");
            //if(ucMonths.IsAllChecked()==false&&ucMonths.Checkeds.Length<1)
            //    throw new ArgumentException("请正确选择月份！", "提示信息");
            if (ucCheckBoxListDataSet.CheckedNames.Length < 1)
                throw new ArgumentException("请正确选择数据集！", "提示信息");
            return true;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                btnOK.Enabled = false;
                if (!CheckArgsIsOk())
                    return;
                //将界面结果输出到配置文件
                if (!int.TryParse(combxYears.SelectedItem.ToString(), out _singleyear))
                    throw new Exception("请选择正确的数据年份！");
                _checkedSetNames = ucCheckBoxListDataSet.CheckedNames;
                _state(-1, "数据年份：" + _singleyear);
                if (cbxYear.Checked)
                {
                    _state(-1, "周期类型：旬、月、年");
                    _periodPrdsArgs = "YEAR";
                }
                else if (cbxMonth.Checked)
                {
                    _state(-1, "周期类型：旬、月");
                    _periodPrdsArgs = "MONTH";
                }
                List<string> statp = new List<string>();
                if (cbxAVG.Checked)
                    statp.Add("AVG");
                if (cbxMax.Checked)
                    statp.Add("MAX");
                if (cbxMin.Checked)
                    statp.Add("MIN");
                _args.StatisticsTypes = statp.ToArray();
                _args.InputDir = txtInputDir.Text;
                _args.OutputDir = txtOutputDir.Text;
                runTaskThread = new Thread(new ThreadStart(this.DoProcess));
                runTaskThread.IsBackground = true;
                runTaskThread.Start();
                _state(-1, "周期合成计算开始...");
                return;
            }
            catch (System.Exception ex)
            {
                Action<int, string> process = _state as Action<int, string>;
                if (process != null)
                    process(-5, ex.Message);
                MessageBox.Show(ex.Message);
                btnOK.Enabled = true;
            }
            finally
            {
                btnCancel.Enabled = true;
            }
        }

        private void DoProcess()
        {
            try
            {
                _args.OverWriteHistoryFiles = cbxOverlap.Checked;
                PeriodicComputeAlg frm = new PeriodicComputeAlg(_sensor, _args);
                frm._periodPrdsArgs = _periodPrdsArgs;
                frm._validsets = _checkedSetNames;
                frm._singleyear = _singleyear;
                frm.Compute(_state as Action<int, string>);
            }
            catch (Exception ex)
            {
                Action<int, string> process = _state as Action<int, string>;
                if (process != null)
                    process(-5, ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            try
            {
                if (runTaskThread != null)
                    runTaskThread.Abort();
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(_periodPrdsArgsXml))
                {
                    string dir = Path.GetDirectoryName(_periodPrdsArgsXml);
                    if (!string.IsNullOrEmpty(dir))
                        System.Diagnostics.Process.Start("Explorer.exe", "/select," + _periodPrdsArgsXml);
                }
                else
                {
                    MessageBox.Show("配置文件不存在，打开文件位置失败！");
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnBrowseInDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtInputDir.Text = dialog.SelectedPath;
            }
        }

        private void btnBrowseOutDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtOutputDir.Text = dialog.SelectedPath;
            }
        }

        private void radibtnAIRS_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radibtnAIRS.Checked)
                {
                    _sensor = "AIRS";
                    txtInputDir.Text = _dbargs.AIRSRootPath;
                    txtOutputDir.Text = _dbargs.AIRSRootPath;
                    ResetDataTimeAndSets();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void radibtnMOD06_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radibtnMOD06.Checked)
                {
                    _sensor = "MOD06";
                    txtInputDir.Text = _dbargs.OutputDir;
                    txtOutputDir.Text = _dbargs.OutputDir;
                    ResetDataTimeAndSets();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void radibtnMYD06_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (radibtnMYD06.Checked)
                {
                    _sensor = "MYD06";
                    txtInputDir.Text = _dbargs.OutputDir;
                    txtOutputDir.Text = _dbargs.OutputDir;
                    ResetDataTimeAndSets();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cbxMonth_CheckedChanged(object sender, EventArgs e)
        {
            if (!cbxMonth.Checked)
            {
                cbxYear.Checked = false;
            }
        }

        private void cbxYear_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxYear.Checked)
            {
                cbxMonth.Checked = true;
            }
        }
    }

    }
