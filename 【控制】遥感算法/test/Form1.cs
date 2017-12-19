using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.RasterTools;
using GeoDo.RSS.Core.DF;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            cbDriverType.SelectedIndex = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtFileName.Text = dlg.FileName;
                }
            }
        }

        private void btnDoBandMath_Click(object sender, EventArgs e)
        {
            try
            {
                IBandMathTool bandMathTool = new BandMathTool();
                using (IRasterDataProvider prd = GeoDataDriver.Open(txtFileName.Text) as IRasterDataProvider)
                {
                    bandMathTool.Compute(prd, txtExpression.Text, cbDriverType.Text, txtOutFilename.Text,
                        (idx, tip) =>
                        {
                            Text = tip + ":" + idx.ToString() + "%";
                        });
                }
                Text = "OK.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnOutFileName_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtOutFilename.Text = dlg.FileName;
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            using (frmSelectExpression frm = new frmSelectExpression())
            {
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtExpression.Text = frm.Expression;
                    //
                    using (frmBandVarMapper frm1 = new frmBandVarMapper())
                    {
                        frm1.SetExpression(txtExpression.Text);
                        UCBandVarSetter.FileBandNames file = new UCBandVarSetter.FileBandNames();
                        file.FileName = @"F:\FY3A\1.LDF";
                        file.BandNames = new UCBandVarSetter.BandName[]
                        {
                            new UCBandVarSetter.BandName(1),
                            new UCBandVarSetter.BandName(2),
                            new UCBandVarSetter.BandName(13),
                            new UCBandVarSetter.BandName(14),
                        };
                        frm1.SetFiles(new UCBandVarSetter.FileBandNames[] { file });
                        if (frm1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            Dictionary<string, int> mappedBandNos = frm1.MappedBandNos;
                            string exp = txtExpression.Text;
                            string[] keys = mappedBandNos.Keys.ToArray().Reverse().ToArray();
                            foreach (string var in keys)
                                exp = exp.Replace(var, "b" + mappedBandNos[var]);
                            txtExpression.Text = exp;
                        }
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            int maxValue = int.Parse(txtMaxValue.Text);
            int minValue = int.Parse(txtMinValue.Text);
            int bin = (int)Math.Ceiling((maxValue - minValue) / (float)255);
            sb.AppendLine("Bin : " + bin.ToString());
            int v = minValue;
            int level = (int)Math.Ceiling((maxValue - minValue) / (float)bin);
            for (int i = 0; i <= level; i++)
            {
                if (v > maxValue)
                    continue;
                sb.AppendLine(i.ToString().PadLeft(3) + ":    " + v.ToString());
                v += bin;
            }
            richTextBox1.Text = sb.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double d = Math.Cos(90);

            using (FileStream fs = new FileStream("f:\\899.dat", FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    int pixelCount = 100 * 100;
                    for (int i = 0; i < pixelCount - 1; i++)
                        bw.Write((Int16)0);
                    bw.Write((Int16)899);
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int v = int.Parse(txtValue.Text);
            //
            int maxValue = int.Parse(txtMaxValue.Text);
            int minValue = int.Parse(txtMinValue.Text);
            int bin = (int)Math.Ceiling((maxValue - minValue) / (float)256);
            //
            int idx = (v - minValue) / bin;
            Text = "Value:" + v.ToString() + "  Index:" + idx.ToString();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            int maxValue = int.Parse(txtMaxValue.Text);
            int minValue = int.Parse(txtMinValue.Text);
            double bin = (maxValue - minValue) / (float)255;
            sb.AppendLine("Bin : " + bin.ToString());
            double v = minValue;
            int level = (int)((maxValue - minValue) / bin);
            for (int i = 0; i <= level; i++)
            {
                if (v > maxValue)
                    continue;
                sb.AppendLine(i.ToString().PadLeft(3) + ":    " + v.ToString("0.######"));
                v += bin;
            }
            richTextBox1.Text = sb.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            double v = double.Parse(txtValue.Text);
            //
            int maxValue = int.Parse(txtMaxValue.Text);
            int minValue = int.Parse(txtMinValue.Text);
            double bin = (maxValue - minValue) / (float)255;
            //
            int idx = (int)((v - minValue) / bin);
            Text = "Value:" + v.ToString() + "  Index:" + idx.ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (IRasterDataProvider prd = GeoDataDriver.Open(txtFileName.Text) as IRasterDataProvider)
            {
                IRasterQuickStatTool stat = new RasterQuickStatTool();
                Dictionary<int, RasterQuickStatResult> results =
                    stat.Compute(prd, null, new int[] { 1, 2, 3, 4, 5 }, (idx, tip) =>
                {
                    Text = tip + ":" + idx.ToString() + "%";
                });
                DisplayStatResult(prd.fileName, results);
            }
        }

        private void DisplayStatResult(string fileName, Dictionary<int, RasterQuickStatResult> results)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(fileName);
            foreach (int bandNo in results.Keys)
            {
                RasterQuickStatResult result = results[bandNo];
                sb.AppendLine("    BandNo " + bandNo.ToString());
                sb.AppendLine("    MinValue:".PadRight(20) + result.MinValue.ToString("0.####"));
                sb.AppendLine("    MaxValue:".PadRight(20) + result.MaxValue.ToString("0.####"));
                sb.AppendLine("    MeanValue:".PadRight(20) + result.MeanValue.ToString("0.####"));
                sb.AppendLine("    Histogram,Bin=".PadRight(20) + result.HistogramResult.Bin.ToString());
                HistogramResult histResult = result.HistogramResult;
                int buckets = histResult.ActualBuckets;
                sb.AppendLine("DN".PadLeft(15) + "Count(Npts)".PadLeft(15) + "Total Count".PadLeft(15) + "Percent".PadLeft(15) + "Acc Percent".PadLeft(15));
                double minValue = result.MinValue;
                double bin = histResult.Bin;
                long accCount = 0;
                double percent = 0;
                double accPercent = 0;
                for (int i = 0; i < buckets; i++)
                {
                    accCount += histResult.Items[i];
                    percent = 100 * histResult.Items[i] / (float)histResult.PixelCount;
                    accPercent += percent;
                    string sLine = (minValue + i * bin).ToString().PadLeft(15) +
                        histResult.Items[i].ToString().PadLeft(15) +
                        accCount.ToString().PadLeft(15) +
                        percent.ToString("0.####").PadLeft(15) +
                        accPercent.ToString("0.####").PadLeft(15);
                    sb.AppendLine(sLine);
                }
            }
            richTextBox1.Text = sb.ToString();
        }

        private string GetBanks(int count)
        {
            string s = string.Empty;
            for (int i = 0; i < count; i++)
                s += " ";
            return s;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            using (frmDataProviderSelector frm = new frmDataProviderSelector())
            {
                frm.StartPosition = FormStartPosition.CenterScreen;
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    IRasterDataProvider dataProvider = null;
                    bool isNew = false;
                    try
                    {
                        dataProvider = frm.DataProvider;
                        int[] bandNos = frm.BandNos;
                        isNew = frm.IsNewDataProvider;
                        DoStat(dataProvider, bandNos);
                    }
                    finally
                    {
                        if (isNew && dataProvider != null)
                            dataProvider.Dispose();
                    }
                }
            }
        }

        private void DoStat(IRasterDataProvider dataProvider, int[] bandNos)
        {
            IRasterQuickStatTool stat = new RasterQuickStatTool();
            Dictionary<int, RasterQuickStatResult> results = stat.Compute(dataProvider, null, bandNos,
                (idx, tip) =>
                {
                    Text = tip + ":" + idx.ToString() + "%";
                });
            using (frmRasterQuickStat frm = new frmRasterQuickStat())
            {
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Apply(dataProvider.fileName, results);
                frm.ShowDialog();
            }
        }

        private void btnScatter_Click(object sender, EventArgs e)
        {
            using (frmScatterVarSelector frm = new frmScatterVarSelector())
            {
                frm.StartPosition = FormStartPosition.CenterScreen;
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    IRasterDataProvider dataProvider = null;
                    bool isNew = false;
                    try
                    {
                        dataProvider = frm.DataProvider;
                        int[] bandNos = new int[] { frm.XBandNo, frm.YBandNo };
                        isNew = frm.IsNewDataProvider;
                        /*using (*/
                        frmScatterGraph frm1 = new frmScatterGraph();//)
                        {
                            frm1.StartPosition = FormStartPosition.CenterScreen;
                            frm1.Reset(dataProvider, bandNos[0], bandNos[1],
                                null,
                                frm.FitObj,
                                (idx, tip) => { this.Text = idx.ToString() + "%"; }
                                );
                            frm1.Show();
                            frm1.Rerender();
                        }
                    }
                    finally
                    {
                        //if (isNew)
                        //    dataProvider.Dispose();
                    }
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            using (frmScatterVarSelector frm = new frmScatterVarSelector())
            {
                frm.StartPosition = FormStartPosition.CenterScreen;
                frm.Apply(GetArrayDataProvider(), null);
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    IRasterDataProvider dataProvider = null;
                    bool isNew = false;
                    dataProvider = frm.DataProvider;
                    int[] bandNos = new int[] { frm.XBandNo, frm.YBandNo };
                    isNew = frm.IsNewDataProvider;
                    frmScatterGraph frm1 = new frmScatterGraph();
                    frm1.StartPosition = FormStartPosition.CenterScreen;
                    //frm1.Reset(dataProvider, bandNos[0], bandNos[1], frm.FitObj,
                    //    (idx, tip) => { this.Text = idx.ToString() + "%"; }
                    //    );
                    frm1.Reset(dataProvider, bandNos[0], bandNos[1],
                        null,
                        new XYAxisEndpointValue(0, 14, 0, 60),
                        frm.FitObj,
                       (idx, tip) => { this.Text = idx.ToString() + "%"; }
                       );
                    frm1.Show();
                    frm1.Rerender();
                }
            }
        }

        private IRasterDataProvider GetArrayDataProvider()
        {
            float[] bandValue1 = new float[] { 7.3f, 7.55f, 7.8f, 8.4f, 10.5f, 12.3f, 12.8f };
            float[] bandValue2 = new float[] { 39.84f, 41.14f, 41.34f, 41.14f, 44, 51, 50.5f };

            int width = 1;
            int height = 7;
            IRasterDataProvider prd = new ArrayRasterDataProvider<float>("统计数组", new float[][] { bandValue1, bandValue2 }, width, height);
            return prd;
        }

        StatusRecorder status = new StatusRecorder(100000000);
        private void button10_Click(object sender, EventArgs e)
        {
            int n = 100000000;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            bool isOK = false;
            status.SetStatus(100, true);
            while (n-- >= 0)
            {
                //status.SetStatus(100, true);
                isOK = status.IsTrue(100);
            }
            sw.Stop();
            MessageBox.Show(sw.ElapsedMilliseconds.ToString());
            Text = status.IsTrue(100).ToString();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            IRasterDataProvider prd = GetRasterDataProvider();
            string fname = prd.fileName;
            //
            IContourGenerateTool tool = new ContourGenerateTool();
            double[] contourValues = new double[] { 0.15d };
            //
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //
            ContourLine[] cLines = tool.Generate(prd.GetRasterBand(1), contourValues,
                (pro, stip) =>
                {
                    Text = pro.ToString() + "%," + stip;
                }
                );
            //
            sw.Stop();
            Text = "GenerateContour: " + sw.ElapsedMilliseconds.ToString();
            //
            DrawContourLine(prd, cLines);
            //
            prd.Dispose();
            //
            Text = "GenerateContour: " + sw.ElapsedMilliseconds.ToString() + ",and drawed.";
        }

        private unsafe void DrawContourLine(IRasterDataProvider prd, ContourLine[] cLines)
        {
            if (cLines == null || cLines.Length == 0)
                return;
            IOverviewGenerator gen = prd as IOverviewGenerator;
            Size size = gen.ComputeSize(1000);
            size = new System.Drawing.Size(prd.Width, prd.Height);
            Bitmap bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
            float scaleX = size.Width / (float)prd.Width;
            float scaleY = size.Height / (float)prd.Height;
            gen.Generate(new int[] { 1, 1, 1 }, ref bitmap);
            //
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                foreach (ContourLine cLine in cLines)
                {
                    if (cLine == null || cLine.Count < 2)
                        continue;
                    //fixed (PointF* ptr0 = cLine.Points.ToArray())
                    //{
                    //    PointF* ptr = ptr0;
                    //    for (int i = 0; i < cLine.Points.Count; i++, ptr++)
                    //    {
                    //        ptr->X = ptr->X * scaleX;
                    //        ptr->Y = ptr->Y * scaleY;
                    //    }
                    //}
                    if ((byte)cLine.ContourValue == 100)
                        g.DrawCurve(Pens.Yellow, cLine.Points.ToArray());
                    else
                        g.DrawCurve(Pens.Red, cLine.Points.ToArray());
                }
            }
            //
            bitmap.Save("f:\\1.bmp", ImageFormat.Bmp);
        }

        private IRasterDataProvider GetRasterDataProvider()
        {
            //byte[] buffer = new byte[] { 3,20,19,14,20,
            //                              17,16,1,17,5,
            //                              20,11,6,21,8,
            //                              5,18,20,30,27,
            //                              30,31,25,8,30
            //                            };
            //int w = 5;
            //int h = 5;
            ////IArrayRasterBand<byte> band = new ArrayRasterBand<byte>(1, buffer, w, h, null);
            ////return new ArrayRasterDataProvider<byte>("", new byte[][] { buffer }, w, h);
            //IRasterDataDriver drv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            //IRasterDataProvider prd = drv.Create("f:\\1.ldf", 5, 5, 1, enumDataType.Byte);
            //GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            //prd.GetRasterBand(1).Write(0, 0, 5, 5, handle.AddrOfPinnedObject(), enumDataType.Byte, 5, 5);
            //handle.Free();
            //return prd;

            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return GeoDataDriver.Open(dlg.FileName, "ComponentID=0000000") as IRasterDataProvider;
                }
            }
            return null;
        }

        int xxx;
        private void button12_Click(object sender, EventArgs e)
        {
            double x = 0.434324d;
            x = 100;
            Text = x.ToString("0.##");
            return;
         
            int n = 1000000;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < n; i++)
            {
                xxx = 10 << (int)Math.Log(Marshal.SizeOf(typeof(PointF)), 2);
            }
            sw.Stop();
            Text = sw.ElapsedMilliseconds.ToString();
            return;
            //
            Text = "list<> before: " + GC.GetTotalMemory(true).ToString();
            MessageBox.Show("press key");
            //
            List<PointF> pts = new List<PointF>();
            for (int i = 0; i < n; i++)
            {
                pts.Add(new PointF());
            }
            //
            GC.Collect();
            Text = "list<> after" + GC.GetTotalMemory(true).ToString();
            MessageBox.Show("press key");
            //
            PointF[] arry = pts.ToArray();
            //
            GC.Collect();
            Text = "to array after:" + GC.GetTotalMemory(true).ToString();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            RasterCut.CutArgument arg = new RasterCut.CutArgument();
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    arg.OutFileName = Path.Combine(Path.GetDirectoryName(dlg.FileName), Path.GetFileNameWithoutExtension(dlg.FileName) + "_Clip_{0}");
                    Cut(arg,dlg.FileName);
                }
            }
        }

        private void Cut(RasterCut.CutArgument arg,string fname)
        {
            IRasterCut cut = new RasterCut();
            using (IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider)
            {
                //
                List<RasterCut.BlockItem> items = new List<RasterCut.BlockItem>();
                int w = 1000;
                int h = 1000;
                int rows = prd.Height / h;
                int cols = prd.Width / w;
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        items.Add(new RasterCut.BlockItem(c * w, r * h, w, h));
                    }
                }
                arg.Items = items.ToArray();
                //
                cut.Cut(prd, arg, (pro, tip) => { Text = "正在裁切("+pro.ToString()+"%)..."; });
            }
        }

        private void button14_Click(object sender, EventArgs e)
        {
            string str = "GeoDo";
            char[] chars = str.ToCharArray();
            using (FileStream fs = new FileStream("f:\\1.bin", FileMode.Create))
            {
                using (BinaryWriter bw = new BinaryWriter(fs, Encoding.Unicode))
                {
                    bw.Write(chars);
                }
            }

            using (FileStream fs = new FileStream("f:\\1.bin", FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs, Encoding.Unicode))
                {
                    chars = br.ReadChars(5);
                }
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    using (IRasterDataProvider prd = GeoDataDriver.Open(dlg.FileName) as IRasterDataProvider)
                    {
                      
                    }
                }
            }
        }
    }
}
