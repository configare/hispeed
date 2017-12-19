using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.MemoryMappedFiles;
using System.IO;
using GeoDo.RSS.MIF.Core;
using GeoDo.Project;
using System.Diagnostics;
using System.Threading.Tasks;
using GeoDo.RSS.Core.DF;
using System.Linq.Expressions;
using System.Drawing.Imaging;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.MIF.UI;

namespace test
{
    public partial class Form1 : Form, IComparer<int>, IContextMessage
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(
            //    "f:\\mmf.dat", FileMode.Create, "mmf", ((long)10000 * (long)10000)))
            //{
            //    MemoryMappedViewAccessor acc = mmf.CreateViewAccessor();
            //}
            string filename = @"G:\1.xlsx";
            IStatResult result = new StatResult("统计时间:", new string[] { "时间", "待验证数据", "验证数据" }, new string[][] { new string[] { "2013/3/1", "5.4", "4.9" }, new string[] { "2013/3/2", "3.9", "2.8" }, new string[] { "2013/3/3", "8.9", "8.8" }, new string[] { "2013/3/4", "7.8", "7.4" } });
            try
            {
                using (StatResultToChartInExcelFile excelControl = new StatResultToChartInExcelFile())
                {
                    excelControl.Init(masExcelDrawStatType.xlXYScatter);
                    excelControl.Add("气溶胶产品数据对比", result, true,1,false,result.Columns[1],result.Columns[2
                        ]);
                    if (!filename.ToUpper().EndsWith(".XLSX"))
                        filename += ".XLSX";
                    excelControl.SaveFile(filename);
                }
            }
            catch(Exception ex)
            {

            }
            
        }

        InterestedRaster<byte> rst;
        private void button2_Click(object sender, EventArgs e)
        {
            RasterIdentify id = new RasterIdentify();
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "2VAL";
            id.Satellite = "FY3A";
            id.Sensor = "MERSI";
            id.Resolution = "250M";
            id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            id.GenerateDateTime = DateTime.Now;
            //
            rst = new InterestedRaster<byte>(id,
                new Size(2000, 2000),
                new GeoDo.RSS.Core.DF.CoordEnvelope(110, 130, 24, 54),
                GeoDo.Project.SpatialReference.GetDefault());
            //
            rst.Reset();
            //
            rst.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int n = 10000 * 10000;
            //using (IPixelFeatureMapper<RectangleF> fm = new PixelFeatureMapper<RectangleF>("test",n,true))
            using (IPixelFeatureMapper<byte> fm = new MemPixelFeatureMapper<byte>("test", n, new Size(1000, 1000), null, null))
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                for (int k = 0; k < n; k++)
                    fm.Put(k, 0);
                foreach (int idx in fm.Indexes)
                {
                    //Console.WriteLine(idx.ToString());
                    //    Console.WriteLine(fm.GetValueByIndex(i++).ToString());
                    //    PointF f = fm.GetValueByIndex(i++);
                }
                sw.Stop();
                Console.WriteLine(sw.ElapsedMilliseconds);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string fname = @"f:\\FY3A_Mersi_2010_06_24_10_00_1000M_L1B_PRJ_Whole.LDF";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            ArgumentProvider argPrd = new ArgumentProvider(prd, null);
            //ThresholdExtracter<UInt16> ext = new ThresholdExtracter<ushort>(argPrd);
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //ext.Extract(new int[] { 1, 2, 3 }, new Action<int, UInt16[]>(Extract));
            //sw.Stop();
            //Text = sw.ElapsedMilliseconds.ToString();
        }

        //(values[0] > 300) && (values[1] > 230)
        private void Extract(int idx, UInt16[] values)
        {
            // (band1 > a) && (band2 > b)
            if (values[0] > 100 && values[1] > 200)
                ;

            //IFuncGenerator<UInt16> v;
            //v.GetBoolFunc("", null)(idx, values);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string fname = @"f:\\FY3A_Mersi_2010_06_24_10_00_1000M_L1B_PRJ_Whole.LDF";
            //fname = @"f:\\FY3A_MERSI_2010_06_24_10_00_1000M_L1B - 副本.HDF";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            ArgumentProvider argPrd = new ArgumentProvider(prd, null);
            RasterPixelsVisitor<UInt16> ext = new RasterPixelsVisitor<ushort>(argPrd);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            ext.VisitPixelWnd(new int[] { 1 }, new int[] { 5 },
                3, 9, new Func<int, int, ushort[], ushort[][], bool>(IsNeedIncWndSize),
                new Action<int, int, ushort[], ushort[][]>(Extract));
            sw.Stop();
            Text = sw.ElapsedMilliseconds.ToString();
        }

        private void Extract(int pixelIndex, int wndSize, UInt16[] pixelValues, UInt16[][] wndValues)
        {
            return;
            if (pixelIndex == 2049)
                Console.WriteLine(pixelIndex.ToString() + ":");
            bool isEmpty = false;
            foreach (UInt16 v in wndValues[0])
            {
                if (v == 0)
                {
                    isEmpty = true;
                    break;
                }
            }
            if (!isEmpty)
            {
                foreach (UInt16 v in wndValues[0])
                    Console.WriteLine(v.ToString());
            }
            //Console.WriteLine(wndValues[0][1]);
        }

        private bool IsNeedIncWndSize(int pixelIndex, int wndSize, UInt16[] pixelValues, UInt16[][] wndValues)
        {
            return false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SortedSet<int> pixels = new SortedSet<int>(this);
            pixels.Add(3);
            pixels.Add(1);

        }

        public int Compare(int x, int y)
        {
            return y - x;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            PercentPixelFilterUInt16 u = new PercentPixelFilterUInt16(1, 0.04f, false);
            string fname = @"f:\\FY3A_Mersi_2010_06_24_10_00_1000M_L1B_PRJ_Whole.LDF";
            //fname = @"f:\\FY3A_MERSI_2010_06_24_10_00_1000M_L1B - 副本.HDF";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            int[] aoi = null;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            aoi = u.Filter(prd, AOIHelper.ComputeAOIRect(aoi, new Size(prd.Width, prd.Height)), aoi);
            sw.Stop();
            Text = sw.ElapsedMilliseconds.ToString();
        }

        private void button8_Click(object sender, EventArgs e)
        {

            string fname = "f:\\4_大昭寺_IMG_GE.tif";
            //fname = "f:\\4600_3400.jpg";
            //fname = "f:\\Penguins.jpg";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            int[] aoi = null;
            PercentPixelFilterByte u = new PercentPixelFilterByte(1, 0.04f, false);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            aoi = u.Filter(prd, AOIHelper.ComputeAOIRect(aoi, new Size(prd.Width, prd.Height)), aoi);
            sw.Stop();
            Text = sw.ElapsedMilliseconds.ToString();
        }

        private void button9_Click(object sender, EventArgs e)
        {
        }

        private void button10_Click(object sender, EventArgs e)
        {
        }

        private void button11_Click(object sender, EventArgs e)
        {
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("a", 300);
            args.Add("b", 230);
            ArgumentProvider argPrd = new ArgumentProvider(null, args);
            string express = "(band1 > var_a) && (band2 > var_b)";
            int[] visitBandNos = new int[] { 6, 2, 1 };
            IExtractFuncProvider<UInt16> funprd = ExtractFuncProviderFactory.CreateExtractFuncProvider<UInt16>(visitBandNos, express, argPrd);
            Func<int, UInt16[], bool> f = funprd.GetBoolFunc();

        }

        private void button12_Click(object sender, EventArgs e)
        {
            BandnameRefTable table = BandRefTableHelper.GetBandRefTable("FY3A", "VIRR");
        }

        IPixelIndexMapper result;
        int[] idxs;
        private void button13_Click(object sender, EventArgs e)
        {
            //构造参数提供者
            string fname = @"f:\\FY3A_Mersi_2010_06_24_10_00_1000M_L1B_PRJ_Whole.LDF";
            fname = @"F:\MERSI\评审火情用\EI2040714.ldf";
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("a", 315);
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            ArgumentProvider argprd = new ArgumentProvider(prd, args);
            //构造判识表达式
            string express = "(band3 / 10f > var_a)";
            int[] bandNos = new int[] { 3 };
            //构造基于阈值的判识器
            IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();
            extracter.Reset(argprd, bandNos, express);
            //判识
            //result = PixelIndexMapperFactory.CreatePixelIndexMapper("Fire",prd.Width,prd.Height);
            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //extracter.Extract(result);
            //idxs = result.Indexes.ToArray();
            //sw.Stop();
            //Text = sw.ElapsedMilliseconds.ToString();
            //判识结果生成二值位图
            IBinaryBitmapBuilder builder = new BinaryBitmapBuilder();
            Size bmSize = new Size(prd.Width, prd.Height);
            Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, Color.Red, Color.Transparent);
            builder.Fill(idxs, new Size(prd.Width, prd.Height), ref bitmap);
            bitmap.Save("f:\\1.png", ImageFormat.Png);
            //判识结果永久保存
            RasterIdentify id = new RasterIdentify();
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "2VAL";
            id.Satellite = "FY3A";
            id.Sensor = "MERSI";
            id.Resolution = "250M";
            id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            id.GenerateDateTime = DateTime.Now;
            IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(prd.Width, prd.Height), prd.CoordEnvelope.Clone());
            iir.Put(idxs, 1);
            iir.Dispose();
            //sw.Stop();
            //Text = sw.ElapsedMilliseconds.ToString();
        }

        IPixelFeatureMapper<float> resultNDVI;
        private void button14_Click(object sender, EventArgs e)
        {
            //构造参数提供者
            string fname = @"f:\\FY3A_Mersi_2010_06_24_10_00_1000M_L1B_PRJ_Whole.LDF";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            ArgumentProvider argprd = new ArgumentProvider(prd, null);
            //构造判识表达式
            //string express = "(band4 + band3) == 0? 0f : (band4 - band3) / (float)(band4 + band3)";
            string express = "NDVI(band4,band3)";
            int[] bandNos = new int[] { 4, 3 };
            //构造栅格计算判识器
            IRasterExtracter<UInt16, float> extracter = new SimpleRasterExtracter<UInt16, float>();
            extracter.Reset(argprd, bandNos, express);
            //判识
            resultNDVI = new MemPixelFeatureMapper<float>("NDVI", 1000, new Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.SpatialRef);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            extracter.Extract(resultNDVI);
            //sw.Stop();
            //Text = sw.ElapsedMilliseconds.ToString();
            //判识结果生成二值位图
            //IBinaryBitmapBuilder builder = new BinaryBitmapBuilder();
            //Size bmSize = new Size(prd.Width / 2, prd.Height / 2);
            //Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, Color.Red, Color.Transparent);
            //builder.Fill(idxs, new Size(prd.Width, prd.Height), ref bitmap);
            //bitmap.Save("f:\\1.png", ImageFormat.Png);
            //判识结果永久保存
            RasterIdentify id = new RasterIdentify();
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "NDVI";
            id.Satellite = "FY3A";
            id.Sensor = "MERSI";
            id.Resolution = "250M";
            id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            id.GenerateDateTime = DateTime.Now;
            IInterestedRaster<float> iir = new InterestedRaster<float>(id, new Size(prd.Width, prd.Height), prd.CoordEnvelope.Clone());
            iir.Put(resultNDVI);
            iir.Dispose();
            sw.Stop();
            Text = sw.ElapsedMilliseconds.ToString();

            int count = iir.Count(aoi, (v) => { return v == 1; });
            iir.Count(aoi, (v) => { return (int)v; });
        }

        private void button15_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "ESRI Shape Files(*.shp)|*.shp";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(dlg.FileName) as IVectorFeatureDataReader;
                    Feature fet = dr.FetchFirstFeature();
                    fet = dr.FetchFeature((f) => { return f.GetFieldValue("CNTRY_NAME") == "China"; });
                    Vector2Bitmap(fet.Geometry as ShapePolygon);
                }
            }
        }

        private void Vector2Bitmap(ShapePolygon shapePolygon)
        {
            //矢量转成位图
            Dictionary<ShapePolygon, Color> vectors = new Dictionary<ShapePolygon, Color>();
            vectors.Add(shapePolygon, Color.Red);
            IVector2BitmapConverter c = new Vector2BitmapConverter();
            int width = (int)(shapePolygon.Envelope.Width / 0.1f);
            int height = (int)(shapePolygon.Envelope.Height / 0.1f);
            Size size = new System.Drawing.Size(width, height);
            Bitmap buffer = new Bitmap(size.Width, size.Height, PixelFormat.Format24bppRgb);
            c.ToBitmap(vectors, Color.White, shapePolygon.Envelope, size, ref buffer);
            buffer.Save("f:\\1.bmp", ImageFormat.Bmp);
            //位图转成栅格索引
            Bitmap2RasterConverter b2r = new Bitmap2RasterConverter();
            int[] idxs = b2r.ToRaster(buffer, Color.Red);
            //栅格索引转成位图
            BinaryBitmapBuilder b = new BinaryBitmapBuilder();
            buffer = b.CreateBinaryBitmap(size, Color.Red, Color.White);
            b.Fill(idxs, size, ref buffer);
            buffer.Save("f:\\2.bmp", ImageFormat.Bmp);
        }

        int[] aoi;
        private void button16_Click(object sender, EventArgs e)
        {
            using (VectorAOITemplate v = VectorAOITemplateFactory.GetAOITemplate("贝尔湖")) //贝尔湖
            {
                Size size;
                IRasterDataProvider prd;
                Envelope evp = GetEnvelope(out size, out prd);
                Stopwatch sw = new Stopwatch();
                sw.Start();
                aoi = v.GetAOI(evp, size);
                //
                int[] reverseAOI = AOIHelper.Reverse(aoi, size);
                IBinaryBitmapBuilder b = new BinaryBitmapBuilder();
                Bitmap bm = b.CreateBinaryBitmap(size, Color.Red, Color.Black);
                b.Fill(reverseAOI, size, ref bm);
                //
                string express = "NDVI(band1,band2)";
                int[] bandNos = new int[] { 1, 2 };
                //构造栅格计算判识器
                ArgumentProvider argprd = new ArgumentProvider(prd, null);
                argprd.AOI = aoi;
                IRasterExtracter<UInt16, float> extracter = new SimpleRasterExtracter<UInt16, float>();
                extracter.Reset(argprd, bandNos, express);
                //判识
                resultNDVI = new MemPixelFeatureMapper<float>("NDVI", 1000, new Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.SpatialRef);
                extracter.Extract(resultNDVI);
                //
                RasterIdentify id = new RasterIdentify();
                id.ThemeIdentify = "CMA";
                id.ProductIdentify = "FIR";
                id.SubProductIdentify = "NDVI";
                id.Satellite = "FY3A";
                id.Sensor = "MERSI";
                id.Resolution = "250M";
                id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
                id.GenerateDateTime = DateTime.Now;
                IInterestedRaster<float> iir = new InterestedRaster<float>(id, new Size(prd.Width, prd.Height), prd.CoordEnvelope.Clone());
                iir.Put(resultNDVI);
                iir.Dispose();
            }
        }

        private Envelope GetEnvelope(out Size size, out IRasterDataProvider prd)
        {
            string fname = @"f:\\FY3A_MERSI_GBAL_L1_20110501_0250_0250M_MS_PRJ_Whole.LDF";
            fname = @"F:\MERSI\评审火情用\EI2040714.ldf";
            prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            size = new System.Drawing.Size();
            size.Width = prd.Width;
            size.Height = prd.Height;
            return new Envelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinY, prd.CoordEnvelope.MaxX, prd.CoordEnvelope.MaxY);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            IVirtualRasterDataProvider prd = new VirtualRasterDataProvider(null);
            IRasterPixelsVisitor<UInt16> visitor = new RasterPixelsVisitor<UInt16>(new ArgumentProvider(prd, null));
            IPixelFeatureMapper<float> result = new MemPixelFeatureMapper<float>("NVI", 10000, new Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.SpatialRef);
            visitor.VisitPixel(new int[] { 1, 11, 21 },
                (idx, values) =>
                {
                    result.Put(idx, values[0] * values[1] * values[2]);
                }
                );
            //
            //IInterestedRaster<float> rst = new InterestedRaster<float>(null, Size.Empty, null);
            //rst.Put(result);
            //rst.Dispose();
        }

        private void xml解析测试_Click(object sender, EventArgs e)
        {
            //ExtractThemesParser parser = new ExtractThemesParser();
            //ThemeDef[] themes = parser.Parse();
        }

        IArgumentProvider argPrd;
        private void button18_Click(object sender, EventArgs e)
        {
            //IRasterDataProvider prd = null;
            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            //ExtractProductIdentify pid = new ExtractProductIdentify();
            //pid.ThemeIdentify = "CMA";
            //pid.ProductIdentify = "FIR";
            //pid.SubProductIdentify = "DBLV";
            //ExtractAlgorithmIdentify algId = new ExtractAlgorithmIdentify();
            //algId.Satellite = prd.DataIdentify.Satellite;
            //algId.Sensor = prd.DataIdentify.Sensor;
            //argPrd = fac.GetArgumentProvider(pid, algId);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            ThemeDef themeDef = MonitoringThemeFactory.GetThemeDefByIdentify("CMA");
            ProductDef prd = themeDef.GetProductDefByIdentify("BAG");

        }

        private void button20_Click(object sender, EventArgs e)
        {

        }

        private void button20_Click_1(object sender, EventArgs e)
        {
            using (Form frm = new Form())
            {

                UCWorkspace wks = new UCWorkspace();
                wks.SetDoubleClickHandler((obj) =>
                {
                    this.Text = obj != null ? Path.GetFileName(obj.ToString()) : string.Empty;
                });
                wks.Apply(GetWorkspaceDef());
                wks.Dock = DockStyle.Fill;
                frm.Controls.Add(wks);
                Button btn = new Button();
                btn.Text = "Get Selected Item";
                btn.Width = 200;
                btn.Height = 30;
                btn.Tag = wks as IWorkspace;
                btn.Click += new EventHandler(btn_Click);
                frm.Controls.Add(btn);
                btn.BringToFront();
                frm.ShowDialog();
            }
        }

        void btn_Click(object sender, EventArgs e)
        {
            IWorkspace wks = (sender as Button).Tag as IWorkspace;
            ICatalog catalog = wks.ActiveCatalog;
            if (catalog != null)
            {
                catalog.AddItem(new CatalogItem("f:\\FOG_DBLV_NUL_NUL_NUL_00010101000000_00010101000000.dat", null, null));
                //ICatalogItem[] items = catalog.GetSelectedItems();
                //if (items != null)
                //{
                //    foreach (ICatalogItem it in items)
                //        Console.WriteLine(it.FileName.ToString());
                //}
                //else
                //{
                //    this.Text = catalog.Definition.Text +" selected is null!";
                //}
            }
            else
            {
                this.Text = "ActiveCatalog is null";
            }
        }

        private WorkspaceDef GetWorkspaceDef()
        {
            WorkspaceDef[] wks = (new WorkspaceDefinitionParser()).Parse();
            return wks[0];
        }

        private void button21_Click(object sender, EventArgs e)
        {
            WorkspaceDefinitionParser p = new WorkspaceDefinitionParser();
            WorkspaceDef[] wks = p.Parse();
        }

        private void button22_Click(object sender, EventArgs e)
        {
            ProductColorTable ct = ProductColorTableFactory.GetColorTable("FOG", "DBLV");
            object[] sts = ProductColorTableFactory.GetStretcher<UInt16>(ct);
        }

        public void PrintMessage(string message)
        {
            Console.WriteLine(message);
        }

        private void button23_Click(object sender, EventArgs e)
        {
            byte max = byte.MaxValue;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            //CreateArray();
            ReuseArray();
            sw.Stop();
            Text = sw.ElapsedMilliseconds.ToString();
        }

        private void ReuseArray()
        {
            byte[] buffer = new byte[1000 * 1000];
            int count = 100;
            for (int i = 0; i < count; i++)
            {
                Array.Clear(buffer, 0, buffer.Length);
            }
        }

        private void CreateArray()
        {
            int count = 100;
            for (int i = 0; i < count; i++)
            {
                byte[] buffer = new byte[1000 * 1000];
            }
        }

        private void button24_Click(object sender, EventArgs e)
        {
            RasterStatByRaster.totest();
            return;
            GeoDo.RSS.MIF.Core.test.totest2();
            return;
            RasterNdvi rn = new RasterNdvi();
            rn.Calc();
        }
    }
}
