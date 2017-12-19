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

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class ucCorrelate : UserControl
    {
        private Action<int, string> _state = null;
        private string[] _lfiles = null, _rfiles = null,_fillvalueR = null, _fillvalueL = null;
        private int _leftBandNum = 1, _rightBandNum = 1;

        public ucCorrelate()
        {
            InitializeComponent();
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
                    toolStripStatusLabel1.Text = arg2.ToString();
                    if (arg1 <= 100)
                    {
                        //toolStripProgressBar1.Value = arg1;
                        this.progressBar1.Value = arg1;
                    }
                }), p, text);
            }
            else
            {
                toolStripStatusLabel1.Text = text.ToString();
                if (p <= 100)
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
                if (Path.GetExtension(filename[0]) == ".000")
                {
                    MicapsDataProcess mp = new MicapsDataProcess();
                    string[] displayNames = mp.DisplayNames;
                    int ct = displayNames.Length;
                    if (ct != 0)
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
            if (listbox.Items.Count == 0)
                return null;
            string[] files = new string[listbox.Items.Count];
            for (int i = 0; i < listbox.Items.Count; i++)
            {
                files[i] = listbox.Items[i] as string;
            }
            return files;
        }

        private int GetBandNo(ComboBox combox)
        {
            string name = combox.SelectedItem.ToString();
            if (!name.Contains("band"))
            {
                return MicapsDataProcess.Index(name);
            } 
            else
                return combox.SelectedIndex + 1;
        }

        private void tsbDoAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                _fillvalueR = null;
                _fillvalueL = null;
                _lfiles = GetFileNames(listBox1);
                _rfiles = GetFileNames(listBox2);
                if (!CheckFiles(_lfiles, _lfiles))
                    return;
                _leftBandNum = GetBandNo(comboBox1);
                _rightBandNum = GetBandNo(comboBox2);
                if (!string.IsNullOrEmpty(txtFillvaluesR.Text))
                {
                    string[] parts = txtFillvaluesR.Text.Split(',');
                    if (parts.Length > 0)
                    { 
                        List<string> fillvalueR =new List<string>();
                        double fvd=0;
                        foreach (string fv in parts)
                        {
                            if (double.TryParse(fv, out fvd))
                                 fillvalueR.Add(fv);
                        }
                        if (fillvalueR.Count > 0)
                            _fillvalueR = fillvalueR.ToArray();
                    }
                }
                if (!string.IsNullOrEmpty(txtFillvaluesL.Text))
                {
                    string[] parts = txtFillvaluesL.Text.Split(',');
                    if (parts.Length > 0)
                    {
                        List<string> fillvalueL = new List<string>();
                        double fvd = 0;
                        foreach (string fv in parts)
                        {
                            if (double.TryParse(fv, out fvd))
                                fillvalueL.Add(fv);
                        }
                        if (fillvalueL.Count > 0)
                            _fillvalueL = fillvalueL.ToArray();
                    }
                }
                Thread runTaskThread = new Thread(new ThreadStart(this.DoWork));
                runTaskThread.IsBackground = true;
                runTaskThread.Start();
            }
            catch (SystemException ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void DoWork()
        {
            try
            {
                CloudParaFileStatics st = new CloudParaFileStatics();
                long scL, scR;
                double correla;
                bool lisMicaps = false, risMicaps = false;
                if (Path.GetExtension(_lfiles[0]) == ".000")
                    lisMicaps = true;
                if (Path.GetExtension(_rfiles[0]) == ".000")
                    risMicaps = true;
                correla = st.FilesCorrelateStat(_lfiles, _leftBandNum, _fillvalueL, lisMicaps, _rfiles, _rightBandNum, _fillvalueR, risMicaps, _state, out scL, out scR);
                txtOutputDir.Text = correla.ToString();
                txtSampleCountsL.Text = scL.ToString();
                txtSampleCountsR.Text = scR.ToString();
            }
            catch (System.Exception ex)
            {
                Action<int, string> process = _state as Action<int, string>;
                if (process != null)
                    process(0, ex.Message);
            }
        }

        private bool CheckFiles(string[] lfiles, string[] rfiles)
        {
            if (lfiles == null || rfiles == null || lfiles.Length == 0 || rfiles.Length == 0)
                throw new ArgumentException("请正确选择左右场数据！");
            //if (lfiles.Length != rfiles.Length)
            //    return false;
            if (comboBox1.SelectedIndex == -1)
                throw new ArgumentException("请正确选择左场波段！");
            if (comboBox2.SelectedIndex == -1)
                throw new ArgumentException("请正确选择右场波段！");
            return true;
        }

        private void ClearFiles(ListBox listbox, ComboBox comboBox)
        {
            listbox.Items.Clear();
            comboBox.SelectedIndex = -1;
            comboBox.Items.Clear();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                AddFileFromLocal(listBox1, comboBox1);
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
            }
            catch (System.Exception ex)
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
                            //    throw new FileLoadException(dbfile + "文件不存在，请确认归档路径设置正确!");
                        }
                        listBox2.Items.Clear();
                        AddFileToListBox(DataReader.FileSelectedList.ToArray(), listBox2, comboBox2);
                    }
                }
            }
            catch (System.Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
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
                    }
                }
            }
            catch (System.Exception ex)
            {
                toolStripStatusLabel1.Text = ex.Message;
            }            
        }
    }
}
