using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using GeoDo.RSS.Core.DF;
using System.IO;
using GeoDo.RSS.DF.MicapsData;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class ucSvd2 : UserControl
    {
        private Action<int, string> _state = null;
        private string _datatype = "CLDGroundObserveData";

        public ucSvd2()
        {
            InitializeComponent();
            InitTask();
            txtOutputDir.Text = Path.Combine(txtOutputDir.Text, DateTime.Now.ToString("yyyyMMdd"));
        }

        private void InitTask()
        {
            Control.CheckForIllegalCrossThreadCalls = false;//关闭该异常检测的方式来避免异常的出现
            _state = new Action<int, string>(InvokeProgress);
        }

        private void InvokeProgress(int p, string text)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<int, string>((arg1, arg2) =>
                {
                    toolStripStatusLabel1.Text = arg2.ToString();
                    if (arg1>=0&&arg1 <= 100)
                    {
                        //toolStripProgressBar1.Value = arg1;
                        this.progressBar1.Value = arg1;
                    }
                }), p, text);
            }
            else
            {
                toolStripStatusLabel1.Text = text.ToString();
                if (p >= 0 && p <= 100)
                {
                    this.progressBar1.Value = p;
                }
            }
        }

        private void AddFileFromDb(ListBox listbox)
        {
            ///
        }

        private void AddFileFromLocal(ListBox listbox, ComboBox comboBox)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "场数据(*.dat;*.ldf;*.gpc;*.000)|*.dat;*.ldf;*.gpc;*.000|所有文件(*.*)|*.*";
                dlg.Multiselect = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    AddFileToListBox(dlg.FileNames, listbox, comboBox);
                }
            }
        }

        private void AddFileToListBox(string[] filename, ListBox listbox, ComboBox comboBox)
        {
            listbox.Items.Clear();
            for (int i = 0; i < filename.Length; i++)
            {
                listbox.Items.Add(filename[i]);
            }
            comboBox.Items.Clear();
            if (listbox.Items.Count > 0)
            {
                if (Path.GetExtension(filename[0])==".000")
                {
                    MicapsDataProcess mp = new MicapsDataProcess();
                    string[] displayNames = mp.DisplayNames;
                    int ct = displayNames.Length;
                    if ( ct!= 0)
                    {
                        foreach (string name in displayNames)
                        {
                            comboBox.Items.Add(name);
                        }
                        comboBox.SelectedIndex = 0;
                    }
                }
                else
                {
                    using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filename[0]) as IRasterDataProvider)
                    {
                        if (dataPrd.BandCount > 0)
                        {
                            int bandCount = dataPrd.BandCount;
                            for (int i = 1; i <= bandCount; i++)
                            {
                                comboBox.Items.Add("band" + i);
                            }
                            comboBox.SelectedIndex = 0;
                        }
                    }
                }
            }
        }

        private string[] GetFileNames(ListBox listbox)
        {
            string[] files = new string[listbox.Items.Count];
            for (int i = 0; i < listbox.Items.Count; i++)
            {
                files[i] = listbox.Items[i] as string;
            }
            return files;
        }

        private int GetBandNo(ComboBox combox,bool isMicaps=false)
        {
            if (isMicaps)
            {
                return MicapsDataProcess.Index(combox.SelectedItem.ToString());
            } 
            else
                return combox.SelectedIndex + 1;
        }

        private void InvokeEnable(bool enable)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action<bool>((arg) => { toolStrip1.Enabled = arg; }), enable);
            }
            else
            {
                toolStrip1.Enabled = enable;
            }
        }

        private void DoWork()
        {
            string[] lfiles = GetFileNames(listBox1);
            string[] rfiles = GetFileNames(listBox2);
            if (!CheckFiles(lfiles, rfiles))
                return;
            int leftBandNum = GetBandNo(comboBox1, Path.GetExtension(lfiles[0])==".000");
            int rightBandNum = GetBandNo(comboBox2, Path.GetExtension(rfiles[0]) == ".000");
            double leftRatio, rightRatio;
            TryGetRatio(txtleftRatio, out leftRatio);
            TryGetRatio(txtrightRatio, out rightRatio);
            string outdir = txtOutputDir.Text;
            bool lisMicaps = false, risMicaps = false;
            if (Path.GetExtension(lfiles[0]) == ".000")
                lisMicaps = true;
            if (Path.GetExtension(rfiles[0]) == ".000")
                risMicaps = true;
            string[] fillvalueR = null;
            if (!string.IsNullOrEmpty(txtFillvaluesR.Text))
            {
                string[] parts = txtFillvaluesR.Text.Split(',');
                double fv;
                foreach (string part in parts)
                {
                    if (double.TryParse(part, out fv))
                        throw new ArgumentException("填充值设置无效！" + part);
                }
                if (parts.Length > 0)
                {
                    fillvalueR = parts;
                }
            }
            string[] fillvalueL = null;
            if (!string.IsNullOrEmpty(txtFillvaluesL.Text))
            {
                string[] parts = txtFillvaluesL.Text.Split(',');
                double fv;
                foreach (string part in parts)
                {
                    if (double.TryParse(part, out fv))
                        throw new ArgumentException("填充值设置无效！" + part);
                }
                if (parts.Length > 0)
                {
                    fillvalueL = parts;
                }
            }
            var context = TaskScheduler.FromCurrentSynchronizationContext();
            var token = Task.Factory.CancellationToken;
            Task task = Task.Factory.StartNew((arg) =>
            {
                Task.Factory.StartNew(() =>
                {
                    toolStrip1.Enabled = false;
                }, token, TaskCreationOptions.None, context);
                try
                {
                    CloudParaFileStatics st = new CloudParaFileStatics();
                    string[] result = st.FilesSVDStat(lfiles, leftBandNum, fillvalueL,rfiles, rightBandNum,fillvalueR, leftRatio, rightRatio, outdir, arg as Action<int, string>, lisMicaps, risMicaps);
                    if (result!=null)
                    {
                        foreach (string file in result)
                        {
                            if (Path.GetFileName(file).Contains("累计方差贡献"))
                            {
                                Process.Start(file);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                    Action<int, string> process = arg as Action<int, string>;
                    if (process != null)
                        process(0, ex.Message);
                }
            }, _state);
            task.ContinueWith((t) =>
            {
                toolStrip1.Enabled = true;
            }, context);
        }

        #region 启动额外线程的改造
        private string [] _lfiles,_fillvalueL, _rfiles,_fillvalueR;
        int _leftBandNum, _rightBandNum;
        double _leftRatio, _rightRatio;
        string _outdir;
        bool _lisMicaps, _risMicaps;
        private void DoPublicWork()
        {
            _lfiles = GetFileNames(listBox1);
            _rfiles = GetFileNames(listBox2);
            if (!CheckFiles(_lfiles, _rfiles))
                return;            
            _leftBandNum = GetBandNo(comboBox1, Path.GetExtension(_lfiles[0]) == ".000");
            _rightBandNum = GetBandNo(comboBox2, Path.GetExtension(_rfiles[0]) == ".000");
            TryGetRatio(txtleftRatio, out _leftRatio);
            TryGetRatio(txtrightRatio, out _rightRatio);
            _outdir = txtOutputDir.Text;
            if (Path.GetExtension(_lfiles[0]) == ".000")
                _lisMicaps = true;
            if (Path.GetExtension(_rfiles[0]) == ".000")
                _risMicaps = true;
            if (!string.IsNullOrEmpty(txtFillvaluesR.Text))
            {
                string[] parts = txtFillvaluesR.Text.Split(',');
                if (parts.Length > 0)
                {
                    _fillvalueR = parts;
                }
            }
            if (!string.IsNullOrEmpty(txtFillvaluesL.Text))
            {
                string[] parts = txtFillvaluesL.Text.Split(',');
                if (parts.Length > 0)
                {
                    _fillvalueL = parts;
                }
            }
            try
            {
                toolStrip1.Enabled = false;
                Thread runTaskThread = new Thread(new ThreadStart(this.DoProcess));
                runTaskThread.IsBackground = true;
                runTaskThread.Start();
            }
            catch (Exception ex)
            {
                Action<int, string> process = _state as Action<int, string>;
                if (process != null)
                    process(0, ex.Message);
            }
            finally
            {
                toolStrip1.Enabled = true;
            }
        }


        private void DoProcess()
        {
            try
            {
                CloudParaFileStatics st = new CloudParaFileStatics();
                string[] result = st.FilesSVDStat(_lfiles, _leftBandNum, _fillvalueL, _rfiles, _rightBandNum, _fillvalueR, _leftRatio, _rightRatio, _outdir, _state as Action<int, string>, _lisMicaps, _risMicaps);
                if (result != null)
                {
                    foreach (string file in result)
                    {
                        if (Path.GetFileName(file).Contains("累计方差贡献"))
                        {
                            Process.Start(file);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                _state(0,ex.Message);
            }
        }
        #endregion
        private bool TryGetRatio(TextBox txtleftRatio, out double ratio)
        {
            return double.TryParse(txtleftRatio.Text, out ratio);
        }

        private bool CheckFiles(string[] lfiles, string[] rfiles)
        {
            if (lfiles == null || rfiles == null || lfiles.Length == 0 || rfiles.Length == 0)
                return false;
            if (lfiles.Length != rfiles.Length)
                throw new ArgumentException("左右场数据序列长度不一致，请重新选择！");
            if (comboBox1.SelectedIndex < 0 || comboBox2.SelectedIndex < 0)
                return false;
            return true;
        }

        private void ClearFiles(ListBox listbox, ComboBox comboBox)
        {
            listbox.Items.Clear();
            comboBox.SelectedIndex = -1;
            comboBox.Items.Clear();
        }

        private void CalcSample(ListBox listbox, TextBox textBox)
        {
            try
            {
                if (listbox.Items.Count > 0)
                {
                    textBox.Text = "";
                    textBox.Enabled = true;
                    if (Path.GetExtension(listbox.Items[0].ToString())==".000")
                    {
                        textBox.Text = "1";
                        textBox.Enabled = false;
                        return;
                    }
                    if (StatRegionSet.UseRegion || StatRegionSet.UseVectorAOIRegion)
                    {
                        double dst = CloudParaFileStatics.CalcSample(listbox.Items[0].ToString(), true);
                        textBox.Text = dst.ToString("f1");
                    }
                    else
                    {
                        double dst = CloudParaFileStatics.CalcSample(listbox.Items[0].ToString());
                        textBox.Text = dst.ToString("f1");
                    }
                }
            }
            catch (System.Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void ChangedOutDir()
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    txtrightRatio.Text = dlg.SelectedPath;
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                AddFileFromLocal(listBox1, comboBox1);
                CalcSample(listBox1, txtleftRatio);
            }
            catch (System.Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            try
            {
                AddFileFromLocal(listBox2, comboBox2);
                CalcSample(listBox2, txtrightRatio);
            }
            catch (System.Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                ClearFiles(listBox1, comboBox1);
                txtleftRatio.Text = "";
            }
            catch (System.Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void toolStripButton9_Click(object sender, EventArgs e)
        {
            try
            {
                ClearFiles(listBox2, comboBox2);
                txtrightRatio.Text = "";
            }
            catch (System.Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void tsbDoAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                //DoWork();
                DoPublicWork();
            }
            catch (SystemException ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }


        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            try
            {
                using (CLDDataRetrieval form = new CLDDataRetrieval())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        foreach (string dbfile in DataReader.FileSelectedList.ToArray())
                        {
                            //if (!File.Exists(dbfile))
                            //    throw new FileLoadException(dbfile+"文件不存在，请确认归档路径设置正确!");
                        }
                        listBox2.Items.Clear();
                        AddFileToListBox(DataReader.FileSelectedList.ToArray(), listBox2, comboBox2);
                        CalcSample(listBox2, txtrightRatio);
                    }
                }
            }
            catch (System.Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void txtOutputDir_DoubleClick(object sender, EventArgs e)
        {
            ChangedOutDir();
        }

        private void txtleftRatio_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtleftRatio.Text))
                return;
            string[] lfiles = GetFileNames(listBox1);
            string[] rfiles = GetFileNames(listBox2);
            if (lfiles == null || lfiles.Length == 0)
                return;
            if (Path.GetExtension(lfiles[0]) == ".000")
            {
                int width = GetMicapsWidth(lfiles[0], rfiles.Length < 1 ? null : rfiles[0]);
                txtLeftSize.Text = "X:" + width + ",Y:1";
                return;
            }
            int leftBandNum = GetBandNo(comboBox1);
            double ratio;
            if (TryGetRatio(txtleftRatio, out ratio))
            {
                Size size;
                CloudParaFileStatics.RatioSize(lfiles[0], leftBandNum, ratio, out size);
                txtLeftSize.Text = "X:" + size.Width + ",Y:" + size.Height;
            }
        }
        private int GetMicapsWidth(string micapsfile,string rasterfile)
        {
            int width=0;
            MicapsDataReader dr = MicapsDataReaderFactory.GetVectorFeatureDataReader(micapsfile, new object[1] { _datatype }) as MicapsDataReader;
            Envelope env = null;
            if (StatRegionSet.UseRegion)
                env = StatRegionSet.Envelope;
            else if (rasterfile!=null&&Path.GetExtension(rasterfile) != ".000")
            {
                using (IRasterDataProvider datap = GeoDataDriver.Open(rasterfile) as IRasterDataProvider)
                {
                    CoordEnvelope cenv = datap.CoordEnvelope;
                    env = new Envelope(cenv.MinX, cenv.MinY, cenv.MaxX, cenv.MaxY);
                }
            }
            if (env == null)
                width = dr.Length;
            else
                width = dr.GetFeatures(env).Length;
            return width;
        }

        private void txtrightRatio_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtrightRatio.Text))
                return;
            string[] lfiles = GetFileNames(listBox2);
            string[] rfiles = GetFileNames(listBox1);
            if (lfiles == null || lfiles.Length == 0)
                return;
            if (Path.GetExtension(lfiles[0]) == ".000")
            {
                int width = GetMicapsWidth(lfiles[0], rfiles.Length<1 ? null : rfiles[0]);
                txtRightSize.Text = "X:" + width + ",Y:1";
                return;
            }
            int leftBandNum = GetBandNo(comboBox2);
            double ratio;
            if (TryGetRatio(txtrightRatio, out ratio))
            {
                Size size;
                CloudParaFileStatics.RatioSize(lfiles[0], leftBandNum, ratio, out size);
                txtRightSize.Text = "X:" + size.Width + ",Y:" + size.Height;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            try
            {
                using (CLDDataRetrieval form = new CLDDataRetrieval())
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        foreach (string dbfile in DataReader.FileSelectedList.ToArray())
                        {
                            //if (!File.Exists(dbfile))
                            //    throw new FileLoadException(dbfile + "文件不存在，请确认归档路径设置正确!");
                        }
                        listBox1.Items.Clear();
                        AddFileToListBox(DataReader.FileSelectedList.ToArray(), listBox1, comboBox1);
                        CalcSample(listBox1, txtleftRatio);
                    }
                }
            }
            catch (System.Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string dir = txtOutputDir.Text;
                if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                {
                    Process.Start(dir);
                }
            }
            catch (System.Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void btnBrowseDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    txtOutputDir.Text = dlg.SelectedPath;
            }
        }
    }
}
