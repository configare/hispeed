using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GeoDo.RSS.RasterTools
{
    public partial class UCHistogramValues : UserControl
    {
        private Dictionary<int, RasterQuickStatResult> _results = null;
        private string _statStrings = string.Empty;

        public UCHistogramValues()
        {
            InitializeComponent();
        }

        public void Reset()
        {
            richTextBox1.Text = string.Empty;
            btnStatItems.DropDownItems.Clear();
        }

        public void Apply(string fileName, Dictionary<int, RasterQuickStatResult> results)
        {
            if (results == null || results.Count == 0)
                return;
            _results = results;
            DisplayStatResult(fileName, results);
        }

        private void DisplayStatResult(string fileName, Dictionary<int, RasterQuickStatResult> results)
        {
            StringBuilder sb = new StringBuilder();
            btnStatItems.DropDownItems.Add(GetItem("所有波段",-1));
            btnStatItems.DropDownItems.Add(new ToolStripSeparator());
            sb.AppendLine("文件:" + fileName);
            sb.AppendLine("    -".PadRight(76,'-'));
            sb.AppendLine("波段".PadLeft(9) + "最小值".PadLeft(10) + "最大值".PadLeft(10) + "平均值".PadLeft(10) + "方差".PadLeft(11) + "标准差".PadLeft(10));
            foreach (int bandNo in results.Keys)
            {
                btnStatItems.DropDownItems.Add(GetItem("波段 " + bandNo.ToString(), bandNo));
                RasterQuickStatResult result = results[bandNo];
                sb.AppendLine(bandNo.ToString().PadLeft(11) + result.MinValue.ToString("0.####").PadLeft(13) + result.MaxValue.ToString("0.####").PadLeft(13) + result.MeanValue.ToString("0.####").PadLeft(13) + result.Stddev.ToString("0.####").PadLeft(13) + Math.Sqrt(result.Stddev).ToString("0.####").PadLeft(13));
            }
            _statStrings = sb.ToString();
            richTextBox1.Text = _statStrings;
            PrintHistogram(results.Keys.ToArray()[0], results[results.Keys.ToArray()[0]]);
            (btnStatItems.DropDownItems[2] as ToolStripMenuItem).Checked = true;
        }

        private void PrintHistogram(int bandNo, RasterQuickStatResult result)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("    -".PadRight(76, '-'));
            sb.AppendLine("    波段 " + bandNo.ToString() + " 直方图:");
            sb.AppendLine("间隔:".PadLeft(12) + result.HistogramResult.Bin.ToString("0.######"));
            HistogramResult histResult = result.HistogramResult;
            int buckets = histResult.ActualBuckets;
            sb.AppendLine("计数值".PadLeft(12) + "像元个数".PadLeft(11) + "累计像元个数".PadLeft(9) + "百分比".PadLeft(12) + "累计百分比".PadLeft(10));
            double minValue = result.MinValue;
            double bin = histResult.Bin;
            long accCount = 0;
            double percent = 0;
            double accPercent = 0;
            if (bin!=0)
            {
                for (int i = 0; i < buckets; i++)
                {
                    accCount += histResult.Items[i];
                    percent = 100 * histResult.Items[i] / (float)histResult.PixelCount;
                    accPercent += percent;
                    string sLine = (minValue + i * bin).ToString("0.######").PadLeft(15) +
                        histResult.Items[i].ToString().PadLeft(15) +
                        accCount.ToString().PadLeft(15) +
                        percent.ToString("0.####").PadLeft(15) +
                        accPercent.ToString("0.####").PadLeft(15);
                    sb.AppendLine(sLine);
                }
            } 
            else
            {
                accCount += histResult.Items[0];
                percent = 100 * histResult.Items[0] / (float)histResult.PixelCount;
                accPercent += percent;
                string sLine = minValue.ToString("0.######").PadLeft(15) +
                histResult.Items[0].ToString().PadLeft(15) +
                accCount.ToString().PadLeft(15) +
                percent.ToString("0.####").PadLeft(15) +
                accPercent.ToString("0.####").PadLeft(15);
                sb.AppendLine(sLine);
            }
            richTextBox1.Text += sb.ToString();
        }

        private ToolStripItem GetItem(string text, int bandNo)
        {
            ToolStripMenuItem it = new ToolStripMenuItem(text);
            it.Tag = bandNo;
            it.Click += new EventHandler(it_Click);
            return it;
        }

        void it_Click(object sender, EventArgs e)
        {
            foreach (object item in btnStatItems.DropDownItems)
            {
                if(item is ToolStripMenuItem)
                    (item as ToolStripMenuItem).Checked = false;
            }
            ToolStripMenuItem it = sender as ToolStripMenuItem;
            it.Checked = true;
            int bandNo = int.Parse(it.Tag.ToString());
            if (bandNo == -1)
            {
                richTextBox1.Text = _statStrings;
                foreach (int b in _results.Keys)
                    PrintHistogram(b, _results[b]);
            }
            else
            {
                richTextBox1.Text = _statStrings;
                PrintHistogram(bandNo, _results[bandNo]);
            }
        }

        private void btnExportToFile_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "文件文件(*.txt)|*.txt";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllText(dlg.FileName, richTextBox1.Text,Encoding.Default);
                }
            }
        }
    }
}
