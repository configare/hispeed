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
//using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DF;

namespace GeoDo.Smart.RasterSmooth
{
    // "数据平滑处理";
    public partial class frmSmooth : Form
    {      
        //private ISmartSession _session = null;
        int _iMax = 10000; //最大有效值
        int _iMin = 0;     //最小有效值
        int _iCore = 10000;  //放大倍数
        int _iInvalid = 6;   //连续异常值个数下限
        int _inInvalid = 20000;   //连续异常值个数下限
        int _outInvalid = 60000;   //连续异常值个数下限
        double _absValue = 0.2;
        int _bandIndex = 1;  //通道
        //private EventHandler _onWindowClosed;

        public frmSmooth()
        {
            InitializeComponent();
        }

        private void btnAddFiles_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Multiselect = true;
                dlg.Filter = "局地文件(*.ldf)|*.ldf;|raw(*.raw)|*.raw;|dat(*.dat)|*.dat";
                dlg.FilterIndex = 3;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    List<string> files = new List<string>();
                    foreach (string filename in dlg.FileNames)
                    {
                        files.Add(filename);
                    }
                    files.Sort();
                    foreach (string filename in files)
                    {
                        AddFile(filename);
                    }
                }
            }
        }

        private void AddFile(string filename)
        {
            if (!listFiles.Items.Contains(filename))
            {
                listFiles.Items.Add(filename);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (listFiles.SelectedItems == null || listFiles.SelectedItems.Count == 0)
                return;
            for (int i = listFiles.SelectedItems.Count - 1; i >= 0; i--)
                listFiles.Items.Remove(listFiles.SelectedItems[i]);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MsgBox.ShowQuestionYesNo("是否确认清空所有数据？") == DialogResult.Yes)
                listFiles.Items.Clear();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            MoveFileItem(-1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p">偏移量</param>
        private void MoveFileItem(int p)
        {
            int selectIndex = listFiles.SelectedIndex;
            if (selectIndex == -1 || selectIndex + p < 0 || selectIndex + p >= listFiles.Items.Count)
                return;
            string filename = listFiles.Items[selectIndex + p].ToString();
            listFiles.Items[selectIndex + p] = listFiles.Items[selectIndex];
            listFiles.Items[selectIndex] = filename;
            listFiles.SelectedItems.Clear();
            listFiles.SelectedIndex = selectIndex + p;
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            MoveFileItem(1);
        }

        private void listFiles_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (Char)Keys.Delete)
            {
                btnRemove_Click(sender, e);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                EnableBtn(false);
                string msginfos;
                if (ArgsOK(out msginfos))
                    DoSmooth();
                else
                    MsgBox.ShowInfo(msginfos);
            }
            catch (Exception ex)
            {
                MsgBox.ShowInfo(ex.Message);
            }
            finally
            {
                EnableBtn(true);
            }
        }

        private void EnableBtn(bool enabled)
        {
            btnStart.Enabled = enabled;
            btnCancel.Enabled = enabled;
        }

        private bool ArgsOK(out string msginfos)
        {
            msginfos = "";
            int iMax = 10000; //最大有效值
            int iMin = 0;     //最小有效值
            int iCore = 10000;  //放大倍数
            int iInvalid = 6;   //连续异常值个数下限
            int inInvalid = 20000;   //连续异常值个数下限
            int outInvalid = 60000;  //连续异常值个数下限
            double absValue = 0.2;   //差值大小
            int bandIndex = 0;
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                msginfos = "输入数据的最大有效值设置为空";
                textBox1.Focus();
                return false;
            }
            if (String.IsNullOrEmpty(textBox2.Text))
            {
                msginfos = "输入数据的最小有效值设置为空";
                textBox2.Focus();
                return false;
            }
            if (String.IsNullOrEmpty(textBox3.Text))
            {
                msginfos = "输入数据的放大倍数设置为空";
                textBox3.Focus();
                return false;
            }
            if (String.IsNullOrEmpty(textBox4.Text))
            {
                msginfos = "连续异常值个数下限设置为空";
                textBox4.Focus();
                return false;
            }
            if (String.IsNullOrEmpty(textBox5.Text))
            {
                msginfos = "输入数据的无效数据(背景数据)值设置为空";
                textBox5.Focus();
                return false;
            }
            if (String.IsNullOrEmpty(textBox6.Text))
            {
                msginfos = "输出数据的无效数据(背景数据)值设置为空";
                textBox6.Focus();
                return false;
            }
            if (String.IsNullOrEmpty(textBox7.Text))
            {
                msginfos = "前后两次差值设置为空";
                textBox7.Focus();
                return false;
            }
            if (String.IsNullOrEmpty(textBox8.Text))
            {
                msginfos = "平滑数据的通道号不能为空";
                textBox8.Focus();
                return false;
            }
            if (!int.TryParse(textBox1.Text, out iMax))
            {
                msginfos = "输入数据的最大有效值输入的值不是整数";
                textBox1.Focus();
                return false;
            }
            if (!int.TryParse(textBox2.Text, out iMin))
            {
                msginfos = "输入数据的最小有效值输入的值不是整数";
                textBox2.Focus();
                return false;
            }
            if (!int.TryParse(textBox3.Text, out iCore))
            {
                msginfos = "输入数据的放大倍数输入的值不是整数";
                textBox3.Focus();
                return false;
            }
            if (!int.TryParse(textBox4.Text, out iInvalid))
            {
                msginfos = "连续异常值个数下限输入的值不是整数";
                textBox4.Focus();
                return false;
            }
            if (!int.TryParse(textBox5.Text, out inInvalid))
            {
                msginfos = "输入数据的无效数据(背景数据)值不是整数";
                textBox5.Focus();
                return false;
            }
            if (!int.TryParse(textBox6.Text, out outInvalid))
            {
                msginfos = "输出数据的无效数据(背景数据)值输入的值不是整数";
                textBox6.Focus();
                return false;
            }
            if (!double.TryParse(textBox7.Text, out absValue))
            {
                msginfos = "前后两次差值输入的值不是有效数值";
                textBox7.Focus();
                return false;
            }
            if (!int.TryParse(textBox8.Text, out bandIndex) || bandIndex <= 0)
            {
                msginfos = "平滑数据的通道号必须为大于0的整数";
                textBox8.Focus();
                return false;
            }
            _iMax = iMax;
            _iMin = iMin;
            _iCore = iCore;
            _iInvalid = iInvalid;
            _inInvalid = inInvalid;
            _outInvalid = outInvalid;
            _absValue = absValue;
            _bandIndex = bandIndex;
            return true;
        }

        private unsafe void DoSmooth()
        {
            if (listFiles.Items.Count == 0)
            {
                MsgBox.ShowInfo("选择的文件为空");
                return;
            }
            List<string> filesIn = new List<string>();
            List<string> filesOut = new List<string>();
            foreach (string file in listFiles.Items)
            {
                filesIn.Add(file);
            }
            string msgInfos;
            //int iMax = 10000; //最大有效值
            //int iMin = 0;     //最小有效值
            //int iCore = 10000;  //放大倍数
            //int iInvalid = 6;   //连续异常值个数下限
            //int inInvalid = 20000;   //连续异常值个数下限
            //int outInvalid = 60000;   //连续异常值个数下限
            //double absValue = 0.2;
            string dir = Path.GetDirectoryName(filesIn[0]) + "/out";
            Action<int, string> progress = new Action<int, string>(ChangeProgress);
            IRasterDataProvider[] rasterIn = null;
            IRasterDataProvider[] rasterOut = null;
            Smooth smooth = new Smooth();
            try
            {
                if (!smooth.CheckRaster(filesIn.ToArray(), _bandIndex, dir, _outInvalid, out rasterIn, out rasterOut, out msgInfos))
                {
                    MsgBox.ShowInfo(msgInfos);
                    return;
                }
                InitProgress();
                //Task task = new Task(() =>
                //{
                    smooth.SmoothFiles(rasterIn, rasterOut, _iMax, _iMin, _iCore, _iInvalid, _inInvalid, _outInvalid, _absValue, _bandIndex, progress, out msgInfos);
                //});
                //task.Start();
                //task.Wait();
            }
            finally
            {
                if (rasterOut != null && rasterOut.Length != 0)
                {
                    foreach (IRasterDataProvider raster in rasterOut)
                    {
                        raster.Dispose();
                    }
                }
                if (rasterIn != null && rasterIn.Length != 0)
                {
                    foreach (IRasterDataProvider raster in rasterIn)
                    {
                        raster.Dispose();
                    }
                }
                rasterOut = null;
                rasterIn = null;
                FinishProgress();
            }
        }

        private void ChangeProgress(int progress, string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int, string>((iprogress, itxt) =>
                {
                    lbMsg.Text = itxt;
                    this.toolStripProgressBar1.Value = iprogress;
                }), progress, text);
            }
            if (progress > this.toolStripProgressBar1.Value && progress <= 100)
            {
                lbMsg.Text = text;
                this.toolStripProgressBar1.Value = progress;
                Application.DoEvents();
            }
        }

        private void InitProgress()
        {
            lbMsg.Text = "";
            this.toolStripProgressBar1.Value = 0;
            Application.DoEvents();
        }

        private void FinishProgress()
        {
            lbMsg.Text = "";
            this.toolStripProgressBar1.Value = 100;
            Application.DoEvents();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTop_Click(object sender, EventArgs e)
        {
            int selectIndex = listFiles.SelectedIndex;
            int count = listFiles.Items.Count;
            MoveTo(-selectIndex);
        }

        private void btnBottom_Click(object sender, EventArgs e)
        {
            int selectIndex = listFiles.SelectedIndex;
            int count = listFiles.Items.Count;
            MoveTo(count - selectIndex-1);
        }

        private void MoveTo(int p)//小于零向上移动，大于零向下移动。
        {
            int selectIndex = listFiles.SelectedIndex;
            if (selectIndex == -1 || selectIndex + p < 0 || selectIndex + p >= listFiles.Items.Count)
                return;
            if (p < 0)
            {
                for (int i = p; i < 0; i++)
                {
                    string filename = listFiles.Items[selectIndex + i].ToString();
                    listFiles.Items[selectIndex + i] = listFiles.Items[selectIndex];
                    listFiles.Items[selectIndex] = filename;
                }
            }
            else if(p > 0)
            { 
                for (int i = p; i > 0; i--)
                {
                    string filename = listFiles.Items[selectIndex + i].ToString();
                    listFiles.Items[selectIndex + i] = listFiles.Items[selectIndex];
                    listFiles.Items[selectIndex] = filename;
                }
            }
            listFiles.SelectedItems.Clear();
            listFiles.SelectedIndex = selectIndex + p;
        }
    }
}
