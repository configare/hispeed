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
    public partial class PeriodMergeTips : Form
    {
        private static string _periodPrdsArgsXml = AppDomain.CurrentDomain.BaseDirectory + @"SystemData\ProductArgs\CLD\PeriodPrdsArgs.xml";
        private static string _dataBaseXml = AppDomain.CurrentDomain.BaseDirectory + @"SystemData\ProductArgs\CLD\CPDataBaseArgs.xml";
        InputArg _args;
        string _sensor;
        private Action<int, string> _state = null;
        Thread runTaskThread = null;

        public PeriodMergeTips(string sensor)
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
            if (InvokeRequired)
            {
                this.Invoke(new Action<int, string>((arg1, arg2) =>
                {
                    txtTips.Text = arg2.ToString();
                    if (arg1 <= 100)
                    {
                        this.progressBar1.Value = arg1;
                        if (arg1 == 100 || arg1 == 0)
                            this.btnOK.Enabled = true;
                    }
                }), p, text);
            }
            else
            {
                txtTips.Text = text.ToString();
                if (p <= 100)
                {
                    this.progressBar1.Value = p;
                    if (p == 100 || p == 0)
                        this.btnOK.Enabled = true;
                }
            }
        }

        public  bool ParseArgsXml()
        {
            if (File.Exists(_dataBaseXml))
            {
                DataBaseArg arg = DataBaseArg.ParseXml(_dataBaseXml);
                string rootpath = null;
                if (_sensor.ToUpper() == "MODIS")
                    rootpath = arg.OutputDir;
                else if (_sensor.ToUpper() == "AIRS")
                    rootpath = arg.AIRSRootPath;
                else if (_sensor.ToUpper() == "ISCCP")
                    rootpath = arg.ISCCPRootPath;
                else if (_sensor.ToUpper() == "CLOUDSAT")
                    rootpath = arg.CloudSATRootPath;
                else
                    return false;
                if (!Directory.Exists(rootpath))
                    return false;
                txtInputDir.Text = rootpath;
                txtOutputDir.Text = rootpath;
            }
            else
            {
                MessageBox.Show("数据库配置文件不存在！");
            }
            if (File.Exists(_periodPrdsArgsXml))
            {
                _args = InputArg.ParsePeriodArgsXml(_periodPrdsArgsXml);
                _args.InputDir=txtInputDir.Text;
                _args.OutputDir = txtOutputDir.Text;
                string temp="";
                foreach (string type in _args.StatisticsTypes)
                {
                    temp += type + ",";
                }
                txtStaticsTypes.Text = temp.Remove(temp.Length - 1);
                temp = "";
                foreach (string type in _args.PeriodTypes)
                {
                    temp += type + ",";
                }
                txtPeriodTypes.Text = temp.Remove(temp.Length - 1);
                if (_args.OverWriteHistoryFiles)
                    cbxOverlap.Checked = true;
                return true;
            }
            else
                return false;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                btnOK.Enabled = false;
                btnCancel.Enabled = false;
                runTaskThread = new Thread(new ThreadStart(this.DoProcess));
                runTaskThread.IsBackground = true;
                runTaskThread.Start();
                return;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                btnCancel.Enabled = true;
            }
        }

        private void DoProcess()
        {
            _args.OverWriteHistoryFiles = cbxOverlap.Checked;
            CLDParaPeriodicCompute frm = new CLDParaPeriodicCompute(_sensor, _args);
            try
            {
                frm.Compute(_state as Action<int, string>);
            }
            catch (Exception ex)
            {
                Action<int, string> process = _state as Action<int, string>;
                if (process != null)
                    process(0, ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (runTaskThread != null)
                runTaskThread.Abort();
            this.Close();
        }

        private void btnEdit_Click(object sender, EventArgs e)
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
    }
}
