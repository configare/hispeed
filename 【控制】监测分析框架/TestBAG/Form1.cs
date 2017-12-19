using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.BAG;
using GeoDo.RSS.Core.DF;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.MIF.Prds.SNW;
using GeoDo.Project;
using GeoDo.RSS.DF.MEM;
using System.IO;

namespace TestBAG
{
    public partial class Form1 : Form
    {
        int[] idxs = null;
        public Form1()
        {
            InitializeComponent();
        }

        public int[] Idxs
        {
            get { return idxs; }
            set { idxs = value; }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
            prdIdentify.ThemeIdentify = "CMA";
            prdIdentify.ProductIdentify = "BAG";
            prdIdentify.SubProductIdentify = "DBLV";
            ExtractAlgorithmIdentify algIdentify = new ExtractAlgorithmIdentify();
            algIdentify.Satellite = "EOST";
            algIdentify.Sensor = "MODIS";
            algIdentify.Resolution = null;//not use

            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            ThemeDef themeDef = MonitoringThemeFactory.GetThemeDefByIdentify("CMA");
            IArgumentProvider prd = MonitoringThemeFactory.GetArgumentProvider(prdIdentify, algIdentify);
            ProductDef bag = themeDef.GetProductDefByIdentify("BAG");
            IMonitoringProduct prbag = new MonitoringProductBag();

            SubProductDef productDef = bag.GetSubProductDefByIdentify("DBLV");
            AlgorithmDef alg = productDef.GetAlgorithmDefByIdentify("BAGExtract");

            IMonitoringSubProduct product = new SubProductBinaryBag(productDef);
            prd.SetArg("BAGExtract", alg);
            prd.DataProvider = GetRasterDataProvider();
            prd.AOI = GetAOI(prd.DataProvider);
            //IExtractResult result = (product as SubProductBinaryBag).Make(prd,null);
            //结果存为图片
            //int[] idxs = (result as IPixelIndexMapper).Indexes.ToArray();
            //IBinaryBitmapBuilder builder = new BinaryBitmapBuilder();
            //Size bmSize = new Size(prd.DataProvider.Width, prd.DataProvider.Height);
            //Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, Color.Red, Color.Transparent);
            //builder.Fill(idxs, new Size(prd.DataProvider.Width, prd.DataProvider.Height), ref bitmap);
            //bitmap.Save("E:\\bag.png", ImageFormat.Png);
            //MessageBox.Show("判识结束");
        }

        private int[] GetAOI(IRasterDataProvider prd)
        {
            using (VectorAOITemplate v = VectorAOITemplateFactory.GetAOITemplate("太湖"))
            {
                Size size;
                Envelope evp = GetEnvelope(out size, out prd);
                return v.GetAOI(evp, size);
            }
        }

        private IRasterDataProvider GetRasterDataProvider()
        {
            //蓝藻
            string fname = @"E:\数据文件\蓝藻\th_2012_05_05_02_44_GZ.ld2";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            return prd;
        }

        private IRasterDataProvider GetRasterDataProvider1()
        {
            //积雪
            string fname = @"E:\\data\\蓝藻\\FY3A_VIRRX_GBAL_L1_20110110_0420_1000M_MS_PRJ_S04.LDF";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            return prd;
        }

        private Envelope GetEnvelope(out Size size, out IRasterDataProvider prd)
        {
            string fname = @"E:\数据文件\蓝藻\th_2012_05_05_02_44_GZ.ld2";
            prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            size = new System.Drawing.Size();
            size.Width = prd.Width;
            size.Height = prd.Height;
            return new Envelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinY, prd.CoordEnvelope.MaxX, prd.CoordEnvelope.MaxY);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
            prdIdentify.ThemeIdentify = "CMA";
            prdIdentify.ProductIdentify = "SNW";
            prdIdentify.SubProductIdentify = "DBLV";
            ExtractAlgorithmIdentify algIdentify = new ExtractAlgorithmIdentify();
            algIdentify.Satellite = "FY3A";
            algIdentify.Sensor = "VIRR";
            algIdentify.Resolution = null;//not use

            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            ThemeDef themeDef = MonitoringThemeFactory.GetThemeDefByIdentify("CMA");
            IArgumentProvider prd = MonitoringThemeFactory.GetArgumentProvider(prdIdentify, algIdentify);
            ProductDef snw = themeDef.GetProductDefByIdentify("SNW");
            IMonitoringProduct prbag = new MonitoringProductBag();

            SubProductDef productDef = snw.GetSubProductDefByIdentify("DBLV");
            AlgorithmDef alg = productDef.GetAlgorithmDefByIdentify("SNWExtract");

            IMonitoringSubProduct product = new SubProductBinarySnw(productDef);
            prd.SetArg("SNWExtract", alg);
            prd.DataProvider = GetRasterDataProvider1();
            //IExtractResult result = product.Make(null,null);
            //结果存为图片
            //int[] idxs = (result as MemPixelIndexMapper).Indexes.ToArray();
            //IBinaryBitmapBuilder builder = new BinaryBitmapBuilder();
            //Size bmSize = new Size(prd.DataProvider.Width, prd.DataProvider.Height);
            //Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, Color.Red, Color.Transparent);
            //builder.Fill(idxs, new Size(prd.DataProvider.Width, prd.DataProvider.Height), ref bitmap);
            //bitmap.Save("E:\\data\\蓝藻\\snow.png", ImageFormat.Png);
            //MessageBox.Show("判识结束");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
            prdIdentify.ThemeIdentify = "CMA";
            prdIdentify.ProductIdentify = "SNW";
            prdIdentify.SubProductIdentify = "0CLM";
            ExtractAlgorithmIdentify algIdentify = new ExtractAlgorithmIdentify();
            algIdentify.Satellite = "FY3A";
            algIdentify.Sensor = "VIRR";
            algIdentify.Resolution = null;//not use

            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            ThemeDef themeDef = MonitoringThemeFactory.GetThemeDefByIdentify("CMA");
            IArgumentProvider prd = MonitoringThemeFactory.GetArgumentProvider(prdIdentify, algIdentify);
            ProductDef snw = themeDef.GetProductDefByIdentify("SNW");
            IMonitoringProduct prbag = new MonitoringProductBag();

            SubProductDef productDef = snw.GetSubProductDefByIdentify("0CLM");
            AlgorithmDef alg = productDef.GetAlgorithmDefByIdentify("CloudExtract");

            IMonitoringSubProduct product = new SubProductBinaryClm(productDef);
            prd.SetArg("CloudExtract", alg);
            prd.DataProvider = GetRasterDataProvider1();
            //IExtractResult result = product.Make(null,null);
            //结果存为图片
            //int[] idxs = (result as MemPixelIndexMapper).Indexes.ToArray();
            //IBinaryBitmapBuilder builder = new BinaryBitmapBuilder();
            //Size bmSize = new Size(prd.DataProvider.Width, prd.DataProvider.Height);
            //Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, Color.Red, Color.Transparent);
            //builder.Fill(idxs, new Size(prd.DataProvider.Width, prd.DataProvider.Height), ref bitmap);
            //bitmap.Save("E:\\data\\蓝藻\\snow_clound.png", ImageFormat.Png);
            //MessageBox.Show("判识结束");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
            prdIdentify.ThemeIdentify = "CMA";
            prdIdentify.ProductIdentify = "BAG";
            prdIdentify.SubProductIdentify = "DBLV";
            ExtractAlgorithmIdentify algIdentify = new ExtractAlgorithmIdentify();
            algIdentify.Satellite = "EOST";
            algIdentify.Sensor = "MODIS";
            algIdentify.Resolution = null;//not use

            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            ThemeDef themeDef = MonitoringThemeFactory.GetThemeDefByIdentify("CMA");
            IArgumentProvider prd = MonitoringThemeFactory.GetArgumentProvider(prdIdentify, algIdentify);
            ProductDef bag = themeDef.GetProductDefByIdentify("BAG");
            IMonitoringProduct prbag = new MonitoringProductBag();

            SubProductDef productDef = bag.GetSubProductDefByIdentify("DBLV");
            AlgorithmDef alg = productDef.GetAlgorithmDefByIdentify("BAGExtract");

            IMonitoringSubProduct product = new SubProductBinaryBag(productDef);
            //prd.SetArg("BAGExtract", alg);
            prd.DataProvider = GetRasterDataProvider();
            prd.AOI = GetAOI(prd.DataProvider);
            //IExtractResult result = (product as SubProductBinaryBag).Make(null,null);
            //结果存为图片
            //idxs = (result as MatrixPixelIndexMapper).Indexes.ToArray();
            //IBinaryBitmapBuilder builder = new BinaryBitmapBuilder();
            //Size bmSize = new Size(prd.DataProvider.Width, prd.DataProvider.Height);
            //Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, Color.Red, Color.Transparent);
            //builder.Fill(idxs, new Size(prd.DataProvider.Width, prd.DataProvider.Height), ref bitmap);
            //bitmap.Save(@"D:\Code\源代码\【控制】监测分析框架\test\bin\Release\TEMP\bag.png", ImageFormat.Png);
            //MessageBox.Show("判识结束.");
            //RasterIdentify id = new RasterIdentify();
            //id.ThemeIdentify = "CMA";
            //id.ProductIdentify = "BAG";
            //id.SubProductIdentify = "DBLV";
            //id.Sensor = "MODIS";
            //id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            //id.GenerateDateTime = DateTime.Now;
            //IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(prd.DataProvider.Width, prd.DataProvider.Height), prd.DataProvider.CoordEnvelope.Clone());
            //iir.Put(idxs, 1);
            //iir.Dispose();
            //MessageBox.Show("判识结果已永久保存.");

        }

        private unsafe void button5_Click(object sender, EventArgs e)
        {
            //先生成全文件NDVI文件
            string fname = @"E:\数据文件\蓝藻\th_2012_05_05_02_44_GZ.ld2";
            IRasterDataProvider dataProvider = GeoDataDriver.Open(fname) as IRasterDataProvider;
            IPixelFeatureMapper<float> ndvi = new MemPixelFeatureMapper<float>("NDVI_Temp", 1000, new Size(dataProvider.Width, dataProvider.Height), dataProvider.CoordEnvelope, dataProvider.SpatialRef);
            IArgumentProvider argprdNDVI = new ArgumentProvider(dataProvider, null);
            //argprdNDVI.AOI = idxs;
            string expressNDVI = "(band2 - band1) / (float)(band2 + band1)";
            int[] bandNos = new int[] { 1, 2 };
            //构造栅格计算判识器
            IRasterExtracter<UInt16, float> extracterNDVI = new SimpleRasterExtracter<UInt16, float>();
            extracterNDVI.Reset(argprdNDVI, bandNos, expressNDVI);
            //判识
            extracterNDVI.Extract(ndvi);
            //NDVI结果永久保存
            RasterIdentify idNDVI = new RasterIdentify();
            idNDVI.ThemeIdentify = "CMA";
            idNDVI.ProductIdentify = "BAG";
            idNDVI.SubProductIdentify = "NDVITemp";
            idNDVI.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            idNDVI.GenerateDateTime = DateTime.Now;
            //NDVISetValue setValue = new NDVISetValue(0.17f, 0.8f);
            //int size = sizeof(NDVISetValue);
            IInterestedRaster<float> iirNDVI = new InterestedRaster<float>(idNDVI, new Size(dataProvider.Width, dataProvider.Height), dataProvider.CoordEnvelope.Clone(), dataProvider.SpatialRef);
            //iirNDVI.SetExtHeader(setValue);
            iirNDVI.Put(ndvi);
            iirNDVI.Dispose();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            string fname = @"E:\数据文件\BAG_NDVITemp_NUL_NUL_NUL_20120619163044_20120620163044.dat";
            IRasterDataProvider dataProvider = GeoDataDriver.Open(fname) as IRasterDataProvider;
            //非蓝藻区域赋-9999
            NDVISetValue setValue1 = (dataProvider as MemoryRasterDataProvider).GetExtHeader<NDVISetValue>();
            double min = setValue1.MinNDVI;
            double max = setValue1.MaxNDVI;
            IPixelFeatureMapper<float> ndvifinal = new MemPixelFeatureMapper<float>("NDVI", 1000, new Size(dataProvider.Width, dataProvider.Height), dataProvider.CoordEnvelope, dataProvider.SpatialRef);
            RasterIdentify idNDVI = new RasterIdentify();
            idNDVI.ThemeIdentify = "CMA";
            idNDVI.ProductIdentify = "BAG";
            idNDVI.SubProductIdentify = "NDVI";
            idNDVI.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            idNDVI.GenerateDateTime = DateTime.Now;
            InterestedRaster<float> iirNDVI = new InterestedRaster<float>(idNDVI, new Size(dataProvider.Width, dataProvider.Height), dataProvider.CoordEnvelope.Clone(), dataProvider.SpatialRef, 8);
            iirNDVI.Put(-9999);
            //IEnumerable<int> ids = (indexProvider as IPixelIndexMapper).Indexes;
            ArgumentProvider ap = new ArgumentProvider(dataProvider, null);
            RasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(ap);
            #region
            ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
            prdIdentify.ThemeIdentify = "CMA";
            prdIdentify.ProductIdentify = "BAG";
            prdIdentify.SubProductIdentify = "DBLV";
            ExtractAlgorithmIdentify algIdentify = new ExtractAlgorithmIdentify();
            algIdentify.Satellite = "EOST";
            algIdentify.Sensor = "MODIS";
            algIdentify.Resolution = null;//not use

            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            ThemeDef themeDef = MonitoringThemeFactory.GetThemeDefByIdentify("CMA");
            IArgumentProvider prd = MonitoringThemeFactory.GetArgumentProvider(prdIdentify, algIdentify);
            ProductDef bag = themeDef.GetProductDefByIdentify("BAG");
            IMonitoringProduct prbag = new MonitoringProductBag();

            SubProductDef productDef = bag.GetSubProductDefByIdentify("DBLV");
            AlgorithmDef alg = productDef.GetAlgorithmDefByIdentify("BAGExtract");

            IMonitoringSubProduct product = new SubProductBinaryBag(productDef);
            //prd.SetArg("BAGExtract", alg);
            prd.DataProvider = GetRasterDataProvider();
            prd.AOI = GetAOI(prd.DataProvider);
            //IExtractResult result = (product as SubProductBinaryBag).Make(prd,null, null);
            //idxs = (result as IPixelIndexMapper).Indexes.ToArray();
            //#endregion
            //visitor.VisitPixel(new int[] { 1 }, (index, values) =>
            //    {
            //        foreach (int item in idxs)
            //        {
            //            if (item == index)
            //            {
            //                ndvifinal.Put(index, values[0]);
            //                break;
            //            }
            //        }
            //    });
            //iirNDVI.SetExtHeader(setValue1);
            //iirNDVI.Put(ndvifinal);
            //iirNDVI.Dispose(); 
            #endregion
        }
        //生成覆盖度文件
        private void button7_Click(object sender, EventArgs e)
        {
            //string fname = @"E:\数据文件\BAG_NDVI_NUL_NUL_NUL_20120620102741_20120621102741.dat";
            //IRasterDataProvider dataProvider = GeoDataDriver.Open(fname) as IRasterDataProvider;
            //IRasterDataProvider[] dataProviders=new RasterDataProvider[] {dataProvider as RasterDataProvider};
            //BagStatisticArea st = new BagStatisticArea(dataProviders);
            //IPixelFeatureMapper<float> result= st.CalcPixelConvertDegree(dataProvider);
            ////永久保存
            //RasterIdentify idNDVI = new RasterIdentify();
            //idNDVI.ThemeIdentify = "CMA";
            //idNDVI.ProductIdentify = "BAG";
            //idNDVI.SubProductIdentify = "NDVITemp";
            //idNDVI.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            //idNDVI.GenerateDateTime = DateTime.Now;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string ndviFileName = @"E:\工程项目（台式服务器）\SMART\【控制】UI框架(新整理)\SMART\bin\Release\TEMP\BAG_NDVI_NULL_NULL_NULL_20120731101801_20120801101801_.dat";
            ndviFileName = @"E:\工程项目（台式服务器）\SMART\【控制】UI框架(新整理)\SMART\bin\Release\Workspace\BAG\2012-08-01\栅格产品\BAG_DBLV_EOST_MODIS_NULL_20120505024500.dat_";
            IRasterDataProvider ndviDataProvider = GeoDataDriver.Open(ndviFileName) as IRasterDataProvider;
            MemoryRasterDataProvider dataProvider = ndviDataProvider as MemoryRasterDataProvider;
            //NDVISetValue setValue1 = (dataProvider).GetExtHeader<NDVISetValue>();
            //double minNDVI = setValue1.MinNDVI;
            //double maxNDVI = setValue1.MaxNDVI;
            //double dst = maxNDVI - minNDVI;
            double minNDVI = 0.17;
            double maxNDVI = 0.81;
            double dst = maxNDVI - minNDVI;
            IPixelFeatureMapper<float> memResult = new MemPixelFeatureMapper<float>("BPCD", 1000, new Size(ndviDataProvider.Width, ndviDataProvider.Height), ndviDataProvider.CoordEnvelope, ndviDataProvider.SpatialRef);
            ArgumentProvider ap = new ArgumentProvider(ndviDataProvider, null);
            RasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(ap);
            visitor.VisitPixel(new int[] { 1 }, (index, values) =>
            {
                if (values[0] == -9999f)
                    memResult.Put(index, -9999);
                else if (dst == 0)
                    memResult.Put(index, -9999);
                else
                    memResult.Put(index, (float)((values[0] - minNDVI) / dst));
            });
            RasterIdentify id = new RasterIdentify(ndviFileName);
            IInterestedRaster<float> iirNDVI = new InterestedRaster<float>(id, new Size(dataProvider.Width, dataProvider.Height), dataProvider.CoordEnvelope.Clone());
            iirNDVI.Put(memResult);
            iirNDVI.Dispose();
            ndviDataProvider.Dispose();
            dataProvider.Dispose();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            string ndviFileName = @"E:\工程项目（台式服务器）\SMART\【控制】UI框架(新整理)\SMART\bin\Release\TEMP\BAG_NDVI_NULL_NULL_NULL_20120731133048_20120801133048_.dat";
            IRasterDataProvider ndviDataProvider = GeoDataDriver.Open(ndviFileName) as IRasterDataProvider;
            MemoryRasterDataProvider dataProvider = ndviDataProvider as MemoryRasterDataProvider;
            NDVISetValue setValue1 = (dataProvider).GetExtHeader<NDVISetValue>();
            //double minNDVI = setValue1.MinNDVI;
            //double maxNDVI = setValue1.MaxNDVI;
            //double dst = maxNDVI - minNDVI;
            double minNDVI = 0.17;
            double maxNDVI = 0.81;
            double dst = maxNDVI - minNDVI;
            IPixelFeatureMapper<float> memResult = new MemPixelFeatureMapper<float>("BPCD", 1000, new Size(ndviDataProvider.Width, ndviDataProvider.Height), ndviDataProvider.CoordEnvelope, ndviDataProvider.SpatialRef);
            ArgumentProvider ap = new ArgumentProvider(ndviDataProvider, null);
            RasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(ap);
            visitor.VisitPixel(new int[] { 1 }, (index, values) =>
            {
                if (values[0] == -9999f)
                    memResult.Put(index, -9999);
                else if (dst == 0)
                    memResult.Put(index, -9999);
                else
                    memResult.Put(index, (float)((values[0] - minNDVI) / dst));
            });
            RasterIdentify id = new RasterIdentify(ndviFileName);
            IInterestedRaster<float> iirNDVI = new InterestedRaster<float>(id, new Size(dataProvider.Width, dataProvider.Height), dataProvider.CoordEnvelope.Clone());
            iirNDVI.Put(memResult);
            iirNDVI.Dispose();
            ndviDataProvider.Dispose();
            dataProvider.Dispose();
        }

        //private void SaveBPCDFile(string ndviFileName, NDVISetValue setValue)
        //{
        //    if (string.IsNullOrEmpty(ndviFileName) || !File.Exists(ndviFileName))
        //        return;
        //    IRasterDataProvider ndviDataProvider = GeoDataDriver.Open(ndviFileName) as IRasterDataProvider;
        //    MemoryRasterDataProvider dataProvider = ndviDataProvider as MemoryRasterDataProvider;
        //    NDVISetValue setValue1 = (dataProvider).GetExtHeader<NDVISetValue>();
        //    double minNDVI = setValue.MinNDVI;
        //    double maxNDVI = setValue.MaxNDVI;
        //    double dst = maxNDVI - minNDVI;
        //    IPixelFeatureMapper<float> memResult = new MemPixelFeatureMapper<float>("BPCD", 1000, new Size(ndviDataProvider.Width, ndviDataProvider.Height), ndviDataProvider.CoordEnvelope, ndviDataProvider.SpatialRef);
        //    ArgumentProvider ap = new ArgumentProvider(ndviDataProvider, null);
        //    RasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(ap);
        //    visitor.VisitPixel(new int[] { 1 }, (index, values) =>
        //    {
        //        if (values[0] == -9999f)
        //            memResult.Put(index, -9999);
        //        else if (dst == 0)
        //            memResult.Put(index, -9999);
        //        else
        //            memResult.Put(index, (float)((values[0] - minNDVI) / dst));
        //    });
        //    //RasterIdentify id = new RasterIdentify();
        //    //id.ThemeIdentify = "CMA";
        //    //id.ProductIdentify = "BAG";
        //    //id.SubProductIdentify = "BPCD";
        //    //id.Satellite = _argumentProvider.DataProvider.DataIdentify.Satellite;
        //    //id.Sensor = _argumentProvider.DataProvider.DataIdentify.Sensor;
        //    //id.Resolution = _argumentProvider.DataProvider.ResolutionX * 100000 + "M";
        //    //id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
        //    //id.GenerateDateTime = DateTime.Now;
        //    //IInterestedRaster<float> iirNDVI = new InterestedRaster<float>(id, new Size(_argumentProvider.DataProvider.Width, _argumentProvider.DataProvider.Height), _argumentProvider.DataProvider.CoordEnvelope.Clone());
        //    //iirNDVI.Put(memResult);
        //    //iirNDVI.Dispose();
        //    //ndviDataProvider.Dispose();
        //    //dataProvider.Dispose();
        //}
    }
}