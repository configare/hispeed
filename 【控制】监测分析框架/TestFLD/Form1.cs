using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.FLD;
using GeoDo.RSS.MIF.Prds.VGT;
using GeoDo.RSS.Core.DF;
using System.Drawing.Imaging;
using CodeCell.AgileMap.Core;


namespace TestFLD
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            //ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
            //prdIdentify.ThemeIdentify = "CMA";
            //prdIdentify.ProductIdentify = "FLD";
            //prdIdentify.SubProductIdentify = "DBLV";

            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            //ThemeDef themeDef = fac.GetThemeDefByIdentify("CMA");
            //IMonitoringTheme them = new MonitoringThemeCMA(themeDef);
            //ProductDef flddef = themeDef.GetProductDefByIdentify("FLD");
            ////IMonitoringProduct fld = them.GetProductByIdentify("FLD");
            ////IMonitoringProduct fld = new MonitoringProductFld(flddef);


            //IArgumentProvider prd = fac.GetArgumentProvider(prdIdentify, "SunDay1", "FY3A", "MERSI");
            ////IMonitoringSubProduct bin = fld.GetSubProductByIdentify("DBLV");
            //SubProductDef productDef = flddef.GetSubProductDefByIdentify("DBLV");
            //AlgorithmDef alg = productDef.GetAlgorithmDefByIdentify("SunDay1");
            ////prd.SetArg("SunDay2", bin.AlgorithmDefs[0]);

            //IMonitoringSubProduct product = new SubProductBinaryFld(productDef);
            ////(product as SubProductBinaryFld).AlgDef = alg;
            //prd.SetArg("SunDay1", alg);
            //prd.DataProvider = GeoDataDriver.Open(@"D:\data\FY3A_MERSI_20100429_0300_DT.LDF") as IRasterDataProvider;
            //prd.AOI = null;
            //IExtractResult result = product.Make(null);

            ////int[] idxs = (result as I).Indexes.ToArray();
            //IBinaryBitmapBuilder builder = new BinaryBitmapBuilder();
            //Size bmSize = new Size(prd.DataProvider.Width, prd.DataProvider.Height);
            //Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, Color.Red, Color.Transparent);
            ////builder.Fill(idxs, new Size(prd.DataProvider.Width, prd.DataProvider.Height), ref bitmap);
            //string file = @"E:\data\water\out\晴空条件下水体判识1.png";
            //bitmap.Save(file, ImageFormat.Png);

            //RasterIdentify id = new RasterIdentify();
            //id.ThemeIdentify = "CMA";
            //id.ProductIdentify = "FLD";
            //id.SubProductIdentify = "SunDay1";
            //id.Satellite = "FY3A";
            //id.Sensor = "MERSI";
            //id.Resolution = "250M";
            //id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            //id.GenerateDateTime = DateTime.Now;
            //IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(prd.DataProvider.Width, prd.DataProvider.Height), prd.DataProvider.CoordEnvelope.Clone());
            ////iir.Put(idxs, 1);
            //iir.Dispose();
            //MessageBox.Show("判识结束！" + file + "\r\n" + iir.FileName);
        }

        private Envelope GetEnvelope(out Size size, out IRasterDataProvider prd)
        {
            string fname = @"E:\data\water\in\FY3A_MERSI_GBAL_L1_20100728_0305_0250M_MS_PRJ_洞庭湖流域.LDF";
            prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            size = new System.Drawing.Size();
            size.Width = prd.Width;
            size.Height = prd.Height;
            return new Envelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinY, prd.CoordEnvelope.MaxX, prd.CoordEnvelope.MaxY);
        }

        int[] aoi;
        private int[] GetAOI()
        {
            VectorAOITemplate template = VectorAOITemplateFactory.GetAOITemplate("洞庭湖");
            Size size;
            IRasterDataProvider prd;
            Envelope evp = GetEnvelope(out size, out prd);

            aoi = template.GetAOI(evp, size);
            //
            int[] reverseAOI = AOIHelper.Reverse(aoi, size);
            return reverseAOI;
        }

        private IRasterDataProvider GetRasterDataProvider()
        {
            //string fname = @"E:\data\water\in\FY3A_MERSI_GBAL_L1_20100728_0305_0250M_MS_PRJ_洞庭湖流域.LDF";
            string fname = @"D:\data\FY3A_MERSI_20100429_0300_DT.LDF";
            //string fname = @"D:\水情测试\fy3a_virr.LDF";
            return GeoDataDriver.Open(fname) as IRasterDataProvider;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
            //prdIdentify.ThemeIdentify = "CMA";
            //prdIdentify.ProductIdentify = "VGT";
            //prdIdentify.SubProductIdentify = "NDVI";
            //ExtractAlgorithmIdentify algIdentify = new ExtractAlgorithmIdentify();
            //algIdentify.Satellite = "FY3A";
            //algIdentify.Sensor = "MERSI";
            //algIdentify.Resolution = null;//not use

            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            //ThemeDef themeDef = fac.GetThemeDefByIdentify("CMA");
            //IMonitoringTheme them = new MonitoringThemeCMA(themeDef);
            //ProductDef flddef = themeDef.GetProductDefByIdentify("VGT");
            ////IMonitoringProduct fld = them.GetProductByIdentify("FLD");
            ////IMonitoringProduct fld = new MonitoringProductFld(flddef);

            //IArgumentProvider prd = fac.GetArgumentProvider(prdIdentify, algIdentify);
            ////IMonitoringSubProduct bin = fld.GetSubProductByIdentify("DBLV");
            //SubProductDef productDef = flddef.GetSubProductDefByIdentify("NDVI");
            //AlgorithmDef alg = productDef.GetAlgorithmDefByIdentify("NDVI");
            ////prd.SetArg("SunDay2", bin.AlgorithmDefs[0]);

            //IMonitoringSubProduct product = new SubProductRasterVgtNdvi(productDef);
            //prd.SetArg("NDVI", alg);
            ////(product as SubProductRasterVgtNdvi).AlgDef = alg;
            //prd.DataProvider = GetRasterDataProvider();
            //prd.AOI = null;
            //IExtractResult result = product.Make(null, null);

            //RasterIdentify id = new RasterIdentify();
            //id.ThemeIdentify = "CMA";
            //id.ProductIdentify = "VGT";
            //id.SubProductIdentify = "NDVI";
            //id.Satellite = "FY3A";
            //id.Sensor = "MERSI";
            //id.Resolution = "250M";
            //id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            //id.GenerateDateTime = DateTime.Now;
            //IInterestedRaster<float> iir = new InterestedRaster<float>(id, new Size(prd.DataProvider.Width, prd.DataProvider.Height), prd.DataProvider.CoordEnvelope.Clone());
            //iir.Put((IPixelFeatureMapper<float>)result);
            //iir.Dispose();
            //MessageBox.Show("计算结束！"+ iir.FileName);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
            //prdIdentify.ThemeIdentify = "CMA";
            //prdIdentify.ProductIdentify = "VGT";
            //prdIdentify.SubProductIdentify = "0RVI";
            //ExtractAlgorithmIdentify algIdentify = new ExtractAlgorithmIdentify();
            //algIdentify.Satellite = "FY3A";
            //algIdentify.Sensor = "MERSI";
            //algIdentify.Resolution = null;//not use

            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            //ThemeDef themeDef = fac.GetThemeDefByIdentify("CMA");
            //IMonitoringTheme them = new MonitoringThemeCMA(themeDef);
            //ProductDef flddef = themeDef.GetProductDefByIdentify("VGT");

            //IArgumentProvider prd = fac.GetArgumentProvider(prdIdentify, algIdentify);
            //SubProductDef productDef = flddef.GetSubProductDefByIdentify("0RVI");
            //AlgorithmDef alg = productDef.GetAlgorithmDefByIdentify("0RVI");

            //IMonitoringSubProduct product = new SubProductRasterVgtRvi(productDef);
            //prd.SetArg("0RVI", alg);
            ////(product as SubProductRasterVgtRvi).AlgDef = alg;
            //prd.DataProvider = GetRasterDataProvider();
            //prd.AOI = null;
            //IExtractResult result = product.Make(null, null);

            //RasterIdentify id = new RasterIdentify();
            //id.ThemeIdentify = "CMA";
            //id.ProductIdentify = "VGT";
            //id.SubProductIdentify = "RVI";
            //id.Satellite = "FY3A";
            //id.Sensor = "MERSI";
            //id.Resolution = "250M";
            //id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            //id.GenerateDateTime = DateTime.Now;
            //IInterestedRaster<float> iir = new InterestedRaster<float>(id, new Size(prd.DataProvider.Width, prd.DataProvider.Height), prd.DataProvider.CoordEnvelope.Clone());
            //iir.Put((IPixelFeatureMapper<float>)result);
            //iir.Dispose();
            //MessageBox.Show("计算结束！" + iir.FileName);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
            //prdIdentify.ThemeIdentify = "CMA";
            //prdIdentify.ProductIdentify = "VGT";
            //prdIdentify.SubProductIdentify = "0DVI";
            //ExtractAlgorithmIdentify algIdentify = new ExtractAlgorithmIdentify();
            //algIdentify.Satellite = "FY3A";
            //algIdentify.Sensor = "MERSI";
            //algIdentify.Resolution = null;//not use

            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            //ThemeDef themeDef = fac.GetThemeDefByIdentify("CMA");
            //IMonitoringTheme them = new MonitoringThemeCMA(themeDef);
            //ProductDef flddef = themeDef.GetProductDefByIdentify("VGT");

            //IArgumentProvider prd = fac.GetArgumentProvider(prdIdentify, algIdentify);
            //SubProductDef productDef = flddef.GetSubProductDefByIdentify("0DVI");
            //AlgorithmDef alg = productDef.GetAlgorithmDefByIdentify("0DVI");
            ////prd.SetArg("SunDay2", bin.AlgorithmDefs[0]);

            //IMonitoringSubProduct product = new SubProductRasterVgtDvi(productDef);
            ////(product as SubProductRasterVgtDvi).AlgDef = alg;
            //prd.SetArg("0DVI", alg);
            //prd.DataProvider = GetRasterDataProvider();
            //prd.AOI = null;
            //IExtractResult result = product.Make(null, null);

            //RasterIdentify id = new RasterIdentify();
            //id.ThemeIdentify = "CMA";
            //id.ProductIdentify = "VGT";
            //id.SubProductIdentify = "DVI";
            //id.Satellite = "FY3A";
            //id.Sensor = "MERSI";
            //id.Resolution = "250M";
            //id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            //id.GenerateDateTime = DateTime.Now;
            //IInterestedRaster<float> iir = new InterestedRaster<float>(id, new Size(prd.DataProvider.Width, prd.DataProvider.Height), prd.DataProvider.CoordEnvelope.Clone());
            //iir.Put((IPixelFeatureMapper<float>)result);
            //iir.Dispose();
            //MessageBox.Show("计算结束！" + iir.FileName);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
            //prdIdentify.ThemeIdentify = "CMA";
            //prdIdentify.ProductIdentify = "VGT";
            //prdIdentify.SubProductIdentify = "0EVI";
            //ExtractAlgorithmIdentify algIdentify = new ExtractAlgorithmIdentify();
            //algIdentify.Satellite = "FY3A";
            //algIdentify.Sensor = "MERSI";
            //algIdentify.Resolution = null;//not use

            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            //ThemeDef themeDef = fac.GetThemeDefByIdentify("CMA");
            //IMonitoringTheme them = new MonitoringThemeCMA(themeDef);
            //ProductDef flddef = themeDef.GetProductDefByIdentify("VGT");

            //IArgumentProvider prd = fac.GetArgumentProvider(prdIdentify, algIdentify);
            //SubProductDef productDef = flddef.GetSubProductDefByIdentify("0EVI");
            //AlgorithmDef alg = productDef.GetAlgorithmDefByIdentify("0EVI");

            //IMonitoringSubProduct product = new SubProductRasterVgtEvi(productDef);
            ////(product as SubProductRasterVgtEvi).AlgDef = alg;
            //prd.SetArg("0EVI", alg);
            //prd.DataProvider = GetRasterDataProvider();
            //prd.AOI = null;
            //IExtractResult result = product.Make(null, null);

            //RasterIdentify id = new RasterIdentify();
            //id.ThemeIdentify = "CMA";
            //id.ProductIdentify = "VGT";
            //id.SubProductIdentify = "EVI";
            //id.Satellite = "FY3A";
            //id.Sensor = "MERSI";
            //id.Resolution = "250M";
            //id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            //id.GenerateDateTime = DateTime.Now;
            //IInterestedRaster<float> iir = new InterestedRaster<float>(id, new Size(prd.DataProvider.Width, prd.DataProvider.Height), prd.DataProvider.CoordEnvelope.Clone());
            //iir.Put((IPixelFeatureMapper<float>)result);
            //iir.Dispose();
            //MessageBox.Show("计算结束！" + iir.FileName);
        }

        /// <summary>
        /// 晴空条件下判识2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            //ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
            //prdIdentify.ThemeIdentify = "CMA";
            //prdIdentify.ProductIdentify = "FLD";
            //prdIdentify.SubProductIdentify = "DBLV";
            //ExtractAlgorithmIdentify algIdentify = new ExtractAlgorithmIdentify();
            //algIdentify.Satellite = "FY3A";
            //algIdentify.Sensor = "MERSI";
            //algIdentify.Resolution = null;//not use

            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            //ThemeDef themeDef = fac.GetThemeDefByIdentify("CMA");
            //IMonitoringTheme them = new MonitoringThemeCMA(themeDef);
            //ProductDef flddef = themeDef.GetProductDefByIdentify("FLD");
            ////IMonitoringProduct fld = them.GetProductByIdentify("FLD");
            ////IMonitoringProduct fld = new MonitoringProductFld(flddef);

            //IArgumentProvider prd = fac.GetArgumentProvider(prdIdentify, "SunDay2", "FY3A", "MERSI");
            ////IMonitoringSubProduct bin = fld.GetSubProductByIdentify("DBLV");
            //SubProductDef productDef = flddef.GetSubProductDefByIdentify("DBLV");
            //AlgorithmDef alg = productDef.GetAlgorithmDefByIdentify("SunDay2");
            ////prd.SetArg("SunDay2", bin.AlgorithmDefs[0]);

            //IMonitoringSubProduct product = new SubProductBinaryFld(productDef);
            ////(product as SubProductBinaryFld).AlgDef = alg;
            //prd.SetArg("SunDay2", alg);
            //prd.DataProvider = GetRasterDataProvider();
            //prd.AOI = null;
            //IExtractResult result = product.Make(null, null);

            ////int[] idxs = (result as MemPixelIndexMapper).Indexes.ToArray();
            //IBinaryBitmapBuilder builder = new BinaryBitmapBuilder();
            //Size bmSize = new Size(prd.DataProvider.Width, prd.DataProvider.Height);
            //Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, Color.Red, Color.Transparent);
            ////builder.Fill(idxs, new Size(prd.DataProvider.Width, prd.DataProvider.Height), ref bitmap);
            //string file=@"E:\data\water\out\晴空条件下水体判识2.png";
            //bitmap.Save(file, ImageFormat.Png);

            //RasterIdentify id = new RasterIdentify();
            //id.ThemeIdentify = "CMA";
            //id.ProductIdentify = "FLD";
            //id.SubProductIdentify = "SunDay2";
            //id.Satellite = "FY3A";
            //id.Sensor = "MERSI";
            //id.Resolution = "250M";
            //id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            //id.GenerateDateTime = DateTime.Now;
            //IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(prd.DataProvider.Width, prd.DataProvider.Height), prd.DataProvider.CoordEnvelope.Clone());
            ////iir.Put(idxs, 1);
            //iir.Dispose();
            //MessageBox.Show("判识结束！" + file + "\r\n" + iir.FileName);
        }

        /// <summary>
        /// 水体混合像元计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            //ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
            //prdIdentify.ThemeIdentify = "CMA";
            //prdIdentify.ProductIdentify = "FLD";
            //prdIdentify.SubProductIdentify = "0Mix";
            //ExtractAlgorithmIdentify algIdentify = new ExtractAlgorithmIdentify();
            //algIdentify.Satellite = "FY3A";
            //algIdentify.Sensor = "MERSI";
            //algIdentify.Resolution = null;//not use

            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            //ThemeDef themeDef = fac.GetThemeDefByIdentify("CMA");
            //IMonitoringTheme them = new MonitoringThemeCMA(themeDef);
            //ProductDef flddef = themeDef.GetProductDefByIdentify("FLD");

            //IArgumentProvider prd = fac.GetArgumentProvider(prdIdentify, algIdentify);
            //SubProductDef productDef = flddef.GetSubProductDefByIdentify("0Mix");
            //AlgorithmDef alg = productDef.GetAlgorithmDefByIdentify("Mix");
            ////prd.SetArg("SunDay2", bin.AlgorithmDefs[0]);

            //IMonitoringSubProduct product = new SubProductRasterFldMix(productDef);
            ////(product as SubProductRasterFldMix).AlgDef = alg;
            //prd.SetArg("Mix", alg);
            //prd.DataProvider = GetRasterDataProvider();
            //prd.AOI = null;
            //IExtractResult result = product.Make(null, null);

            //RasterIdentify id = new RasterIdentify();
            //id.ThemeIdentify = "CMA";
            //id.ProductIdentify = "FLD";
            //id.SubProductIdentify = "Mix";
            //id.Satellite = "FY3A";
            //id.Sensor = "MERSI";
            //id.Resolution = "250M";
            //id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            //id.GenerateDateTime = DateTime.Now;
            //IInterestedRaster<float> iir = new InterestedRaster<float>(id, new Size(prd.DataProvider.Width, prd.DataProvider.Height), prd.DataProvider.CoordEnvelope.Clone());
            //iir.Put((IPixelFeatureMapper<float>)result);
            //iir.Dispose();
            //MessageBox.Show("计算结束！" + iir.FileName);
        }

        private void button8_Click(object sender, EventArgs e)
        {
           // ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
           // prdIdentify.ThemeIdentify = "CMA";
           // prdIdentify.ProductIdentify = "FLD";
           // prdIdentify.SubProductIdentify = "DBLV";
           // ExtractAlgorithmIdentify algIdentify = new ExtractAlgorithmIdentify();
           // algIdentify.Satellite = "FY3A";
           // algIdentify.Sensor = "MERSI";
           // algIdentify.Resolution = null;//not use

           // IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
           // ThemeDef themeDef = fac.GetThemeDefByIdentify("CMA");
           // IMonitoringTheme them = new MonitoringThemeCMA(themeDef);
           // ProductDef flddef = themeDef.GetProductDefByIdentify("FLD");
           // //IMonitoringProduct fld = them.GetProductByIdentify("FLD");
           // //IMonitoringProduct fld = new MonitoringProductFld(flddef);

           // IArgumentProvider prd = fac.GetArgumentProvider(prdIdentify, "ThinCloud", "FY3A", "MERSI");
           // //IMonitoringSubProduct bin = fld.GetSubProductByIdentify("DBLV");
           // SubProductDef productDef = flddef.GetSubProductDefByIdentify("DBLV");
           // AlgorithmDef alg = productDef.GetAlgorithmDefByIdentify("ThinCloud");
           // //prd.SetArg("SunDay2", bin.AlgorithmDefs[0]);

           // IMonitoringSubProduct product = new SubProductBinaryFld(productDef);
           // //(product as SubProductBinaryFld).AlgDef = alg;
           // prd.SetArg("ThinCloud", alg);
           // prd.DataProvider = GetRasterDataProvider();
           // prd.AOI = null;
           // IExtractResult result = product.Make(null, null);

           // //int[] idxs = (result as MemPixelIndexMapper).Indexes.ToArray();
           // IBinaryBitmapBuilder builder = new BinaryBitmapBuilder();
           // Size bmSize = new Size(prd.DataProvider.Width, prd.DataProvider.Height);
           // Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, Color.Red, Color.Transparent);
           // //builder.Fill(idxs, new Size(prd.DataProvider.Width, prd.DataProvider.Height), ref bitmap);
           //string file=@"E:\data\water\out\薄云条件下水体判识.png";
           //bitmap.Save(file, ImageFormat.Png);

           // RasterIdentify id = new RasterIdentify();
           // id.ThemeIdentify = "CMA";
           // id.ProductIdentify = "FLD";
           // id.SubProductIdentify = "ThinCloud";
           // id.Satellite = "FY3A";
           // id.Sensor = "MERSI";
           // id.Resolution = "250M";
           // id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
           // id.GenerateDateTime = DateTime.Now;
           // IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(prd.DataProvider.Width, prd.DataProvider.Height), prd.DataProvider.CoordEnvelope.Clone());
           // //iir.Put(idxs, 1);
           // iir.Dispose();
           // MessageBox.Show("判识结束！" + file + "\r\n" + iir.FileName);
        }

        private void button9_Click(object sender, EventArgs e)
        {
           // ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
           // prdIdentify.ThemeIdentify = "CMA";
           // prdIdentify.ProductIdentify = "FLD";
           // prdIdentify.SubProductIdentify = "DBLV";
           // ExtractAlgorithmIdentify algIdentify = new ExtractAlgorithmIdentify();
           // algIdentify.Satellite = "FY3A";
           // algIdentify.Sensor = "VIRR";
           // algIdentify.Resolution = null;//not use

           // IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
           // ThemeDef themeDef = fac.GetThemeDefByIdentify("CMA");
           // IMonitoringTheme them = new MonitoringThemeCMA(themeDef);
           // ProductDef flddef = themeDef.GetProductDefByIdentify("FLD");
           // //IMonitoringProduct fld = them.GetProductByIdentify("FLD");
           // //IMonitoringProduct fld = new MonitoringProductFld(flddef);

           // IArgumentProvider prd = fac.GetArgumentProvider(prdIdentify, "Fog", "FY3A", "MERSI");
           // //IMonitoringSubProduct bin = fld.GetSubProductByIdentify("DBLV");
           // SubProductDef productDef = flddef.GetSubProductDefByIdentify("DBLV");
           // AlgorithmDef alg = productDef.GetAlgorithmDefByIdentify("Fog");
           // //prd.SetArg("SunDay2", bin.AlgorithmDefs[0]);

           // IMonitoringSubProduct product = new SubProductBinaryFld(productDef);
           // //(product as SubProductBinaryFld).AlgDef = alg;
           // prd.SetArg("Fog", alg);
           // prd.DataProvider = GetRasterDataProvider();
           // prd.AOI = null;
           // IExtractResult result = product.Make(null, null);

           // //int[] idxs = (result as MemPixelIndexMapper).Indexes.ToArray();
           // IBinaryBitmapBuilder builder = new BinaryBitmapBuilder();
           // Size bmSize = new Size(prd.DataProvider.Width, prd.DataProvider.Height);
           // Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, Color.Red, Color.Transparent);
           // //builder.Fill(idxs, new Size(prd.DataProvider.Width, prd.DataProvider.Height), ref bitmap);
           //string file=@"E:\data\water\out\薄雾条件下水体判识.png";
           //bitmap.Save(file, ImageFormat.Png);
           // RasterIdentify id = new RasterIdentify();
           // id.ThemeIdentify = "CMA";
           // id.ProductIdentify = "FLD";
           // id.SubProductIdentify = "Fog";
           // id.Satellite = "FY3A";
           // id.Sensor = "MERSI";
           // id.Resolution = "250M";
           // id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
           // id.GenerateDateTime = DateTime.Now;
           // IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(prd.DataProvider.Width, prd.DataProvider.Height), prd.DataProvider.CoordEnvelope.Clone());
           // //iir.Put(idxs, 1);
           // iir.Dispose();
           // MessageBox.Show("判识结束！" + file + "\r\n" + iir.FileName);
        }

        private void button10_Click(object sender, EventArgs e)
        {
           // ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
           // prdIdentify.ThemeIdentify = "CMA";
           // prdIdentify.ProductIdentify = "FLD";
           // prdIdentify.SubProductIdentify = "DBLV";
           // ExtractAlgorithmIdentify algIdentify = new ExtractAlgorithmIdentify();
           // algIdentify.Satellite = "FY3A";
           // algIdentify.Sensor = "MERSI";
           // algIdentify.Resolution = null;//not use

           // IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
           // ThemeDef themeDef = fac.GetThemeDefByIdentify("CMA");
           // IMonitoringTheme them = new MonitoringThemeCMA(themeDef);
           // ProductDef flddef = themeDef.GetProductDefByIdentify("FLD");
           // //IMonitoringProduct fld = them.GetProductByIdentify("FLD");
           // //IMonitoringProduct fld = new MonitoringProductFld(flddef);


           // IArgumentProvider prd = fac.GetArgumentProvider(prdIdentify, "NDVI", "FY3A", "MERSI");
           // //IMonitoringSubProduct bin = fld.GetSubProductByIdentify("DBLV");
           // SubProductDef productDef = flddef.GetSubProductDefByIdentify("DBLV");
           // AlgorithmDef alg = productDef.GetAlgorithmDefByIdentify("NDVI");
           // //prd.SetArg("SunDay2", bin.AlgorithmDefs[0]);

           // IMonitoringSubProduct product = new SubProductBinaryFld(productDef);
           // //(product as SubProductBinaryFld).AlgDef = alg;
           // prd.SetArg("NDVI", alg);
           // prd.DataProvider = GetRasterDataProvider();
           // prd.AOI = null;
           // IExtractResult result = product.Make(null, null);

           // //int[] idxs = (result as MemPixelIndexMapper).Indexes.ToArray();
           // IBinaryBitmapBuilder builder = new BinaryBitmapBuilder();
           // Size bmSize = new Size(prd.DataProvider.Width, prd.DataProvider.Height);
           // Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, Color.Red, Color.Transparent);
           // //builder.Fill(idxs, new Size(prd.DataProvider.Width, prd.DataProvider.Height), ref bitmap);
           //string file=@"E:\data\water\out\植被指数水体判识.png";
           //bitmap.Save(file, ImageFormat.Png);
           // RasterIdentify id = new RasterIdentify();
           // id.ThemeIdentify = "CMA";
           // id.ProductIdentify = "FLD";
           // id.SubProductIdentify = "NDVI";
           // id.Satellite = "FY3A";
           // id.Sensor = "MERSI";
           // id.Resolution = "250M";
           // id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
           // id.GenerateDateTime = DateTime.Now;
           // IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(prd.DataProvider.Width, prd.DataProvider.Height), prd.DataProvider.CoordEnvelope.Clone());
           // //iir.Put(idxs, 1);
           // iir.Dispose();
           // MessageBox.Show("判识结束！" + file + "\r\n" + iir.FileName);
        }

        /// <summary>
        /// 水体云判识
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            //ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
            //prdIdentify.ThemeIdentify = "CMA";
            //prdIdentify.ProductIdentify = "FLD";
            //prdIdentify.SubProductIdentify = "0CLM";

            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            //ThemeDef themeDef = fac.GetThemeDefByIdentify("CMA");
            //IMonitoringTheme them = new MonitoringThemeCMA(themeDef);
            //ProductDef flddef = themeDef.GetProductDefByIdentify("FLD");

            //IArgumentProvider prd = fac.GetArgumentProvider(prdIdentify, "Cloud", "FY3A", "MERSI");
            //SubProductDef productDef = flddef.GetSubProductDefByIdentify("0CLM");
            //AlgorithmDef alg = productDef.GetAlgorithmDefByIdentify("Cloud");

            //IMonitoringSubProduct product = new SubProductBinaryClm(productDef);
            //(product as SubProductBinaryClm).AlgDef = alg;
            //prd.SetArg("Cloud", alg);
            //prd.DataProvider = GetRasterDataProvider();
            //prd.AOI = null;
            //IExtractResult result = product.Make(null, null);

            ////int[] idxs = (result as MemPixelIndexMapper).Indexes.ToArray();
            //IBinaryBitmapBuilder builder = new BinaryBitmapBuilder();
            //Size bmSize = new Size(prd.DataProvider.Width, prd.DataProvider.Height);
            //Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, Color.Red, Color.Transparent);
            ////builder.Fill(idxs, new Size(prd.DataProvider.Width, prd.DataProvider.Height), ref bitmap);
            //string file=@"E:\data\water\out\云监测.png";
            //bitmap.Save(file, ImageFormat.Png);

            //RasterIdentify id = new RasterIdentify();
            //id.ThemeIdentify = "CMA";
            //id.ProductIdentify = "FLD";
            //id.SubProductIdentify = "Cloud";
            //id.Satellite = "FY3A";
            //id.Sensor = "MERSI";
            //id.Resolution = "250M";
            //id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            //id.GenerateDateTime = DateTime.Now;
            //IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(prd.DataProvider.Width, prd.DataProvider.Height), prd.DataProvider.CoordEnvelope.Clone());
            ////iir.Put(idxs, 1);
            //iir.Dispose();
            //MessageBox.Show("判识结束！" + file + "\r\n" + iir.FileName);
        }

        /// <summary>
        /// 泛滥缩小水体计算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            //IMonitoringSubProduct product = new SubProductRasterFldFlood();
            //IArgumentProvider argprd = new ArgumentProvider();
            //argprd.SetArg("BackWaterFile", @"E:\data\water\in\b.dat");
            //argprd.DataProvider = GeoDataDriver.Open(@"E:\data\water\in\c.dat") as IRasterDataProvider;
            //IExtractResult result = product.Make(null, null);
            //RasterIdentify id = new RasterIdentify();
            //id.ThemeIdentify = "CMA";
            //id.ProductIdentify = "FLD";
            //id.SubProductIdentify = "BcWt";
            //id.Satellite = "FY3A";
            //id.Sensor = "MERSI";
            //id.Resolution = "250M";
            //id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            //id.GenerateDateTime = DateTime.Now;
            //IInterestedRaster<float> iir = new InterestedRaster<float>(id, new Size(argprd.DataProvider.Width, argprd.DataProvider.Height), argprd.DataProvider.CoordEnvelope.Clone());
            //iir.Put((IPixelFeatureMapper<float>)result);
            //iir.Dispose();
            //MessageBox.Show("计算结束！" + iir.FileName);
        }

        private void button13_Click(object sender, EventArgs e)
        {
            //IMonitoringSubProduct product = new SubProductAnalysisFldXzqdl();
            //IArgumentProvider argprd = new ArgumentProvider();
            //argprd.DataProvider = GeoDataDriver.Open(@"D:\MAS数据\第2张盘（监测产品培训数据）\04_水情\2010-04-29\FY3A_MERSI_20100429_0300_DT.LDF") as IRasterDataProvider;
        }

    }
}
