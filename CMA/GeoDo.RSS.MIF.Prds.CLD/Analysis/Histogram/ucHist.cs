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
    public partial class ucHist : UserControl
    {
        private Action<int, string> _state = null;
        private ISmartSession _smartSession;

        public ucHist(ISmartSession smartSession)
        {
            InitializeComponent();
            InitTask();
            _smartSession = smartSession;
        }

        private void InitTask()
        {
            //_state = new Action<int, string>(InvokeProgress);
            _state = new Action<int, string>((arg1, arg2) =>
            {
                toolStripStatusLabel1.Text = arg2.ToString();
                if (arg1 != -1 && arg1 <= 100)
                {
                    this.progressBar1.Value = arg1;
                }
            });
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

        private void AddFileFromLocal(ListBox listbox, ListView lv)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "场数据(*.dat;*.ldf;*.gpc;*.000)|*.dat;*.ldf;*.gpc;*.000|所有文件(*.*)|*.*";
                dlg.Multiselect = true;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    AddFileToListBox(dlg.FileNames, listbox, lv);
                }
                _state(0, "");
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
            _state(0, "");
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
                        ListViewItem it = new ListViewItem(name);
                        it.Tag = MicapsDataProcess.Index(name);
                        lvBands.Items.Add(it);
                    }
                }
            }
            else
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
            try
            {
                bool reverse;//标识最大是否<最小值
                if (!CheckArgsIsOk(out reverse))
                    return;
                string[] lfiles = GetFileNames(listBox1);
                if (lfiles.Length < 1)
                    throw new ArgumentException("待统计文件为空！");
                int[] leftBandNum = GetBandNo(lvBands);
                if (leftBandNum == null)
                    throw new ArgumentException("待统计波段为空！");
                if (checkBox1.Checked && leftBandNum.Length > 1)
                    throw new ArgumentException("分区段统计只适用于单一波段，请重新选择！");
                string[] fillvalue = null;
                if (!string.IsNullOrEmpty(txtFillvalues.Text))
                {
                    string[] parts = txtFillvalues.Text.Split(',');
                    double fv;
                    foreach (string part in parts)
                    {
                        if (double.TryParse(part, out fv))
                            throw new ArgumentException("填充值设置无效！" + part);
                    }
                    if (parts.Length > 0)
                        fillvalue = parts;
                }
                CloudParaFileStatics st = new CloudParaFileStatics();
                Dictionary<int, RasterQuickStatResult> results=null ;
                if (Path.GetExtension(lfiles[0]) == ".000")
                {
                    if (!checkBox1.Checked)//全部统计
                        results = st.MicapsHistoStat(lfiles, leftBandNum, fillvalue, _state as Action<int, string>);
                    else//分区段
                    {
                        if (!reverse)
                            results = st.MicapsHistoStatBetween(lfiles, leftBandNum, fillvalue, txtMin.Text, txtMax.Text, _state as Action<int, string>);
                        else
                            results = st.MicapsHistoStatBetween(lfiles, leftBandNum, fillvalue, txtMax.Text, txtMin.Text, _state as Action<int, string>);
                    }
                }
                else
                {
                    if (!checkBox1.Checked)//全部统计
                    {
                        results = st.FilesHistoStat(lfiles, leftBandNum, fillvalue, _state as Action<int, string>);
                    } 
                    else//分区段
                    {
                        if (!reverse)
                            results = st.FilesHistoStatBetween(lfiles, leftBandNum, fillvalue, txtMin.Text, txtMax.Text, _state as Action<int, string>);
                        else
                            results = st.FilesHistoStatBetween(lfiles, leftBandNum, fillvalue, txtMax.Text, txtMin.Text, _state as Action<int, string>);
                    }
                }
                if (results !=null)
                {
                    frmRasterQuickStat frm = new frmRasterQuickStat();//)
                    {
                        frm.Owner = _smartSession.SmartWindowManager.MainForm as Form;
                        frm.StartPosition = FormStartPosition.CenterScreen;
                        frm.Apply(lfiles[0], results);
                        frm.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Action<int, string> process = _state as Action<int, string>;
                if (process != null)
                    process(0, ex.Message);
            }
        }

        private bool CheckArgsIsOk(out bool reverse)
        {
            reverse = false;
            if (listBox1.Items.Count <1)
                throw new ArgumentException("待统计文件不能为空！");
            if (lvBands.CheckedItems.Count<1)
                throw new ArgumentException("待统计波段不能为空！");
            if (checkBox1.Checked)
            {
                if (string.IsNullOrEmpty(txtMin.Text) || string.IsNullOrEmpty(txtMax.Text))
                    throw new ArgumentException("自定义区间最大最小值不能为空！");
                double min, max;
                if (!double.TryParse(txtMin.Text, out min) || !double.TryParse(txtMax.Text, out max))
                    throw new ArgumentException("请输入正确的自定义区间最大最小值！");
                if (min > max)
                    reverse = true;
            }
            return true;
        }

        private void ClearFiles(ListBox listbox, ListView lv)
        {
            listbox.Items.Clear();
            lv.Items.Clear();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            try
            {
                AddFileFromLocal(listBox1, lvBands);
            }
            catch (System.Exception ex)
            {
                _state(0,ex.Message);
                //toolStripStatusLabel1.Text = ;
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
                //toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void tsbDoAnalysis_Click(object sender, EventArgs e)
        {
            try
            {
                this.DoWork();
                //Thread runTaskThread = new Thread(new ThreadStart(this.DoWork));
                //runTaskThread.IsBackground = true;
                //runTaskThread.Start();
            }
            catch(SystemException ex)
            {
                _state(0, ex.Message);
                //toolStripStatusLabel1.Text = ex.Message;
            }
            finally
            {
                //progressBar1.Value = 0;
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
                _state(0, ex.Message);
                //toolStripStatusLabel1.Text = ex.Message;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                txtMin.Enabled = true;
                txtMax.Enabled = true;
            } 
            else
            {
                txtMin.Enabled = false;
                txtMax.Enabled = false;
                //txtMin.Text = "";
                //txtMax.Text = "";
            }
        }
    }
}
