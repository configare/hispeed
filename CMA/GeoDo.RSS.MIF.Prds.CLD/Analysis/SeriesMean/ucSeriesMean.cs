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
using GeoDo.RSS.RasterTools;
using GeoDo.RSS.Core.UI;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class ucSeriesMean : UserControl
    {
        private Action<int, string> _state = null;
        private ISmartSession _smartSession;

        public ucSeriesMean(ISmartSession smartSession)
        {
            InitializeComponent();
            InitTask();
            _smartSession = smartSession;
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
                    if (arg1 != -1 && arg1 <= 100)
                    {
                        //toolStripProgressBar1.Value = arg1;
                        this.progressBar1.Value = arg1;
                    }
                }), p, text);
            }
            else
            {
                toolStripStatusLabel1.Text = text.ToString();
                if (p != -1 && p <= 100)
                {
                    this.progressBar1.Value = p;
                }
            }
        }

        private void AddFileFromLocal(ListBox listbox, ListView lv)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "场数据(*.dat;*.ldf;*.gpc;)|*.dat;*.ldf;*.gpc;|所有文件(*.*)|*.*";
                dlg.Multiselect = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    AddFileToListBox(dlg.FileNames, listbox, lv);
                }
            }
        }

        private void AddFileToListBox(string[] filename, ListBox listbox, ListView lv)
        {
            listbox.Items.Clear();
            for (int i = 0; i < filename.Length; i++)
            {
                listbox.Items.Add(filename[i]);
            }
            lv.Items.Clear();
            if (listbox.Items.Count > 0)
            {
                FillBandNos(filename[0]);
            }
        }

        private void FillBandNos(string filename)
        {
            if (Path.GetExtension(filename) == ".000")
            {
                MicapsDataProcess mp = new MicapsDataProcess();
                string[] displayNames = mp.DisplayNames;
                long [] indexs =MicapsDataProcess.Index(displayNames);
                int ct = displayNames.Length;
                if (ct != 0)
                {
                    foreach (string name in displayNames)
                    {
                        comboBox1.Items.Add(name);
                    }
                    comboBox1.SelectedIndex = 0;

                    foreach (string name in displayNames)
                    {
                        ListViewItem it = new ListViewItem(name);
                        it.Tag = MicapsDataProcess.Index(name);
                        lvBands.Items.Add(it);
                    }
                }
            }
            else
            {
                try
                {
                    using (IRasterDataProvider dataPrd = GeoDataDriver.Open(filename) as IRasterDataProvider)
                    {
                        if (dataPrd.BandCount > 0)
                        {
                            int bandCount = dataPrd.BandCount;
                            for (int i = 1; i <= bandCount; i++)
                            {
                                ListViewItem it = new ListViewItem("Band " + i.ToString());
                                it.Tag = i;
                                lvBands.Items.Add(it);
                                comboBox1.Items.Add("band" + i);
                            }
                            comboBox1.SelectedIndex = 0;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    _state(0,ex.Message);
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

        private int[] GetBandNo(ListView  listbands)
        {
            List<int> bandNos = new List<int>();
            foreach (ListViewItem it in lvBands.Items)
            {
                if (it.Checked)
                {
                    int bNo = (int)it.Tag;
                    bandNos.Add(bNo);
                }
            }
            return bandNos.Count > 0 ? bandNos.ToArray() : null;
        }

        private int GetBandNo(ComboBox combox, bool isMicaps = false)
        {
            if (isMicaps)
            {
                return MicapsDataProcess.Index(combox.SelectedItem.ToString());
            }
            else
                return combox.SelectedIndex + 1;
        }

        private void DoWork()
        {
            try
            {
                if (!CheckArgsIsOk())
                    return;
                string[] lfiles = GetFileNames(listBox1);
                if (lfiles.Length < 1)
                    throw new ArgumentException("待统计文件为空！");
                //int[] leftBandNum = new int[] { };
                //int[] leftBandNum = GetBandNo(lvBands);
                //if (leftBandNum == null)
                //    throw new ArgumentException("待统计波段为空！");
                int bandno = GetBandNo(this.comboBox1);
                if (bandno <=0)
                    throw new ArgumentException("待统计波段无效！");
                string[] fillvalue = null;
                if (!string.IsNullOrEmpty(txtFillvalues.Text))
                {
                    try
                    {
                        string[] parts = txtFillvalues.Text.Split(',');
                        double fv;
                        foreach (string part in parts)
                        {
                            if(!double.TryParse(part,out fv))
                                throw new ArgumentException("填充值设置无效！" + part);
                        }
                        if (parts.Length > 0)
                            fillvalue = parts;
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                }
                CloudParaFileStatics st = new CloudParaFileStatics();
                string[] files = st.FilesSeriesMeanStat(lfiles, bandno, fillvalue, txtOutputDir.Text, _state as Action<int, string>);
                _state(100, "长时间序列均值统计完成！");
                foreach (string file in files)
                    if (File.Exists(file))
                {
                    Process.Start(file);
                }
            }
            catch (Exception ex)
            {
                Action<int, string> process = _state as Action<int, string>;
                if (process != null)
                    process(0, ex.Message);
            }
        }

        private bool CheckArgsIsOk()
        {
            if (listBox1.Items.Count <1)
                throw new ArgumentException("待统计文件不能为空！");
            //if (lvBands.CheckedItems.Count<1)
            //    throw new ArgumentException("待统计波段不能为空！");
            if (comboBox1.SelectedIndex < 0)
                throw new ArgumentException("请重新选择正确的波段！");
            return true;
        }

        private void ClearFiles(ListBox listbox, ListView lv)
        {
            listbox.Items.Clear();
            lv.Items.Clear();
            comboBox1.Items.Clear();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                AddFileFromLocal(listBox1, lvBands);
            }
            catch (System.Exception ex)
            {
                _state(0, ex.Message);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            try
            {
                ClearFiles(listBox1, lvBands);
            }
            catch (System.Exception ex)
            {
                _state(0, ex.Message);
            }
        }

        private void tsbDoAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                this.DoWork();
            }
            catch(SystemException ex)
            {
                _state(0, ex.Message);
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
                        AddFileToListBox(DataReader.FileSelectedList.ToArray(), listBox1, lvBands);
                    }
                }
            }
            catch(SystemException ex)
            {
                _state(0,ex.Message);
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

        private void button1_Click(object sender, EventArgs e)
        {
                        string dir = txtOutputDir.Text;
            if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
            {
                Process.Start(dir);
            }
        }
    }
}
