using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.DST;
using GeoDo.RSS.Core.DF;
using System.Drawing.Imaging;
using CodeCell.AgileMap.Core;

namespace TestDst
{
    public partial class Form1 : Form
    {
        private ExtractProductIdentify _exPro = null;
        private ExtractAlgorithmIdentify _exAlg = null;
        //private IArgumentProviderFactory MonitoringThemeFactory = null;

        public Form1()
        {
            InitializeComponent();
        }

        SubProductDef _sub;
        ProductDef _pro;

        #region extracts
        private void FY3陆地_Click(object sender, EventArgs e)
        {
            InitExIdentify();
            _exAlg.CustomIdentify = "陆地";
            _exAlg.Satellite = "FY3A";
            _exAlg.Sensor = "VIRR";

            AlgorithmDef alg = _sub.GetAlgorithmDefByIdentify("FY3Land");
            IArgumentProvider arg = MonitoringThemeFactory.GetArgumentProvider(_exPro, _exAlg);
            IRasterDataProvider prd = GetRasterDataProviderVIRR();
           // arg.DataProvider = prd;
            //arg.AOI = ApplyAOI(prd, true);
           // arg.SetArg("AlgorithmName","FY3Land");
            SubProductBinaryDst bin = new SubProductBinaryDst(_sub);
            bin.ResetArgumentProvider("FY3Land", prd.DataIdentify.Satellite, prd.DataIdentify.Sensor, "陆地");
            bin.ArgumentProvider.DataProvider = prd;
            bin.ArgumentProvider.SetArg("AlgorithmName", "FY3Land");
            IPixelIndexMapper result = bin.Make( null) as IPixelIndexMapper;

            string saveName = @"E:\沙尘判识VIRR.png";
            CreatBitmap(prd, result.Indexes.ToArray(), saveName);

            RasterIdentify id = new RasterIdentify();
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "DST";
            id.SubProductIdentify = "2VAL";
            id.Satellite = "FY3A";
            id.Sensor = "VIRR";
            id.Resolution = "1000M";
            id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            id.GenerateDateTime = DateTime.Now;
            IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(prd.Width, prd.Height), prd.CoordEnvelope.Clone());
            iir.Put(result.Indexes.ToArray(), 1);
            iir.Dispose();

            MessageBox.Show("判识完成，输出文件：E:\\data\\dst\\output\\沙尘判识VIRR.png");
            
        }

        private void NOAA18陆地_Click(object sender, EventArgs e)
        {
            InitExIdentify();
            _exAlg.CustomIdentify = "陆地";
            _exAlg.Satellite = "NOAA18";
            _exAlg.Sensor = "AVHRR";

            AlgorithmDef alg = _sub.GetAlgorithmDefByIdentify("NOAA18Land");
            IArgumentProvider arg = MonitoringThemeFactory.GetArgumentProvider(_exPro, _exAlg);
            IRasterDataProvider prd = GetRasterDataProviderNOAA18();
            arg.DataProvider = prd;
            arg.AOI = GetAOI(prd, true);
            arg.SetArg("NOAA18Land", alg);
            SubProductBinaryDst bin = new SubProductBinaryDst(_sub);

            IPixelIndexMapper result = bin.Make( null) as IPixelIndexMapper;
            string saveName = @"E:\data\dst\output\2sandNoaa18land.png";
            CreatBitmap(prd, result.Indexes.ToArray(), saveName);
        }

        private void NOAA18海洋_Click(object sender, EventArgs e)
        {
            InitExIdentify();
            _exAlg.CustomIdentify = "海洋";
            _exAlg.Satellite = "NOAA18";
            _exAlg.Sensor = "AVHRR";

            AlgorithmDef alg = _sub.GetAlgorithmDefByIdentify("NOAA18Sea");
            IArgumentProvider arg = MonitoringThemeFactory.GetArgumentProvider(_exPro, _exAlg);
            IRasterDataProvider prd = GetRasterDataProviderNOAA18();
            arg.DataProvider = prd;
            arg.AOI = GetAOI(prd, false);
            arg.SetArg("NOAA18Sea", alg);
            SubProductBinaryDst bin = new SubProductBinaryDst(_sub);

            IPixelIndexMapper result = bin.Make( null) as IPixelIndexMapper;
            string saveName = @"E:\data\dst\output\3sandNoaa18Sea.png";
            CreatBitmap(prd, result.Indexes.ToArray(), saveName);
        }

        private void MODIS陆地_Click(object sender, EventArgs e)
        {
            InitExIdentify();
            _exAlg.CustomIdentify = "陆地";
            _exAlg.Satellite = "EOST";
            _exAlg.Sensor = "MODIS";

            AlgorithmDef alg = _sub.GetAlgorithmDefByIdentify("EOSLand");
            IArgumentProvider arg = MonitoringThemeFactory.GetArgumentProvider(_exPro, _exAlg);
            IRasterDataProvider prd = GetRasterDataProviderMODIS();
            arg.DataProvider = prd;
            arg.AOI = GetAOI(prd, true);
            arg.SetArg("EOSLand", alg);
            SubProductBinaryDst bin = new SubProductBinaryDst(_sub);

            IPixelIndexMapper result = bin.Make( null) as IPixelIndexMapper;
            string saveName = @"E:\data\dst\output\2sandEOSland.png";
            CreatBitmap(prd, result.Indexes.ToArray(), saveName);
        }

        private void MODIS海洋_Click(object sender, EventArgs e)
        {
            InitExIdentify();
            _exAlg.CustomIdentify = "海洋";
            _exAlg.Satellite = "EOSA";
            _exAlg.Sensor = "MODIS";

            AlgorithmDef alg = _sub.GetAlgorithmDefByIdentify("EOSSea");
            IArgumentProvider arg = MonitoringThemeFactory.GetArgumentProvider(_exPro, _exAlg);
            IRasterDataProvider prd = GetRasterDataProviderMODIS();
            arg.DataProvider = prd;
            //arg.AOI = ApplyAOI(prd, false);
            arg.SetArg("EOSSea", alg);
            SubProductBinaryDst bin = new SubProductBinaryDst(_sub);

            IPixelIndexMapper result = bin.Make( null) as IPixelIndexMapper;
            string saveName = @"E:\data\dst\output\3sandEOSsea.png";
            CreatBitmap(prd, result.Indexes.ToArray(), saveName);
        }

        private void MERSI海洋_Click(object sender, EventArgs e)
        {
            InitExIdentify();
            _exAlg.CustomIdentify = "海洋";
            _exAlg.Satellite = "FY3A";
            _exAlg.Sensor = "MERSI";

            AlgorithmDef alg = _sub.GetAlgorithmDefByIdentify("FY3Sea");
            IArgumentProvider arg = MonitoringThemeFactory.GetArgumentProvider(_exPro, _exAlg);
            IRasterDataProvider prd = GetRasterDataProviderMESI();
            arg.DataProvider = prd;
            // arg.AOI = ApplyAOI(prd,false);
            arg.SetArg("FY3Sea", alg);
            SubProductBinaryDst bin = new SubProductBinaryDst(_sub);

            IPixelIndexMapper result = bin.Make( null) as IPixelIndexMapper;
            string saveName = @"E:\data\dst\output\2sandMersiSea.png";
            CreatBitmap(prd, result.Indexes.ToArray(), saveName);
        }

        private void NOAA17陆地_Click(object sender, EventArgs e)
        {
            InitExIdentify();
            _exAlg.CustomIdentify = "陆地";
            _exAlg.Satellite = "NOAA17";
            _exAlg.Sensor = "AVHRR";

            AlgorithmDef alg = _sub.GetAlgorithmDefByIdentify("NOAA17Land");
            IArgumentProvider arg = MonitoringThemeFactory.GetArgumentProvider(_exPro, _exAlg);
            IRasterDataProvider prd = GetRasterDataProviderMESI();
            arg.DataProvider = prd;
            // arg.AOI = ApplyAOI(prd,false);
            arg.SetArg("NOAA17Land", alg);
            SubProductBinaryDst bin = new SubProductBinaryDst(_sub);

            IPixelIndexMapper result = bin.Make( null) as IPixelIndexMapper;
            string saveName = @"E:\data\dst\output\3sandnoaaland.png";
            CreatBitmap(prd, result.Indexes.ToArray(), saveName);
        }

        private void NOAA17海洋_Click(object sender, EventArgs e)
        {
            InitExIdentify();
            _exAlg.CustomIdentify = "海洋";
            _exAlg.Satellite = "NOAA17";
            _exAlg.Sensor = "AVHRR";

            AlgorithmDef alg = _sub.GetAlgorithmDefByIdentify("NOAA17Sea");
            IArgumentProvider arg = MonitoringThemeFactory.GetArgumentProvider(_exPro, _exAlg);
            IRasterDataProvider prd = GetRasterDataProviderMESI();
            arg.DataProvider = prd;
            // arg.AOI = ApplyAOI(prd,false);
            arg.SetArg("NOAA17Sea", alg);
            SubProductBinaryDst bin = new SubProductBinaryDst(_sub);

            IPixelIndexMapper result = bin.Make( null) as IPixelIndexMapper;
            string saveName = @"E:\data\dst\output\3sandnoaaSea.png";
            CreatBitmap(prd, result.Indexes.ToArray(), saveName);
        }
        #endregion

        #region 能见度计算
        //通过实时判识结果获取AOI
        private void 能见度计算_Click(object sender, EventArgs e)
        {
            InitExIdentify();
            _exAlg.CustomIdentify = "陆地";
            _exAlg.Satellite = "FY3A";
            _exAlg.Sensor = "VIRR";
            AlgorithmDef alg = _sub.GetAlgorithmDefByIdentify("FY3Land");
            IArgumentProvider arg = MonitoringThemeFactory.GetArgumentProvider(_exPro, _exAlg);
            IRasterDataProvider prd = GetRasterDataProviderVIRR();
            arg.DataProvider = prd;
            arg.AOI = GetAOI(prd, true);
            arg.SetArg("FY3Land", alg);
            SubProductBinaryDst bin = new SubProductBinaryDst(_sub);
            IPixelIndexMapper result = bin.Make( null) as IPixelIndexMapper;
            int[] indxs = result.Indexes.ToArray();

            _exPro.SubProductIdentify = "VISY";
            _sub = _pro.GetSubProductDefByIdentify("VISY");
            _exAlg.CustomIdentify = null;
            AlgorithmDef visiAlg = _sub.GetAlgorithmDefByIdentify("Visibility");
            IArgumentProvider visiArg = MonitoringThemeFactory.GetArgumentProvider(_exPro, _exAlg);
            visiArg.DataProvider = prd;
            visiArg.AOI = indxs;
            visiArg.SetArg("Visibility", visiAlg);
            SubProductRasterDst raster = new SubProductRasterDst(_sub);
            IPixelFeatureMapper<UInt16> rasterResult = raster.Make( null) as IPixelFeatureMapper<UInt16>;

            //
            RasterIdentify id = new RasterIdentify();
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "SAND";
            id.SubProductIdentify = "VISIBILITY";
            id.Satellite = "FY3A";
            id.Sensor = "VIRRX";
            id.Resolution = "1000M";
            id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            id.GenerateDateTime = DateTime.Now;
            IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(prd.Width, prd.Height), prd.CoordEnvelope.Clone());
            iir.Put(rasterResult);
            iir.Dispose();

            MessageBox.Show("能见度计算完成！");
        }

        //通过读取历史判识文件获取AOI
        private void 能见度计算file_Click(object sender, EventArgs e)
        {
            InitExIdentify();
            _exPro.SubProductIdentify = "VISY";
            _sub = _pro.GetSubProductDefByIdentify("VISY");
            _exAlg.Satellite = "FY3A";
            _exAlg.Sensor = "VIRR";
            AlgorithmDef visiAlg = _sub.GetAlgorithmDefByIdentify("Visibility");
            IArgumentProvider visiArg = MonitoringThemeFactory.GetArgumentProvider(_exPro, _exAlg);

            IRasterDataProvider prd = GetRasterDataProviderBinaryFile();
            Dictionary<string, object> args = new Dictionary<string, object>();
            IArgumentProvider argPrd = new ArgumentProvider(prd, args);
            RasterPixelsVisitor<UInt16> raster = new RasterPixelsVisitor<UInt16>(argPrd);
            List<int> idxs = new List<int>();
            raster.VisitPixel(new int[] { 1 }, (index, value) =>
                {
                    if (value[0] != 0)
                        idxs.Add(index);
                });

            visiArg.DataProvider = GetRasterDataProviderVIRR();
            visiArg.AOI = idxs.ToArray();
            visiArg.SetArg("Visibility", visiAlg);
            SubProductRasterDst subraster = new SubProductRasterDst(_sub);
            IPixelFeatureMapper<UInt16> rasterResult = subraster.Make( null) as IPixelFeatureMapper<UInt16>;

            //
            RasterIdentify id = new RasterIdentify();
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "SAND";
            id.SubProductIdentify = "VISIBILITY";
            id.Satellite = "FY3A";
            id.Sensor = "VIRRX";
            id.Resolution = "1000M";
            id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            id.GenerateDateTime = DateTime.Now;
            IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(prd.Width, prd.Height), prd.CoordEnvelope.Clone());
            iir.Put(rasterResult);
            iir.Dispose();

            MessageBox.Show("能见度计算完成！");
        }

        private void 能见度计算file2_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region private functions
        private void InitExIdentify()
        {
            _exPro = new ExtractProductIdentify();
            _exPro.ThemeIdentify = "CMA";
            _exPro.ProductIdentify = "DST";
            _exPro.SubProductIdentify = "DBLV";
            _exAlg = new ExtractAlgorithmIdentify();
            _exAlg.Resolution = null;

            //MonitoringThemeFactory = MifEnvironment.ActiveArgumentProviderFactory;
            ThemeDef theme = MonitoringThemeFactory.GetThemeDefByIdentify("CMA");
            _pro = theme.GetProductDefByIdentify("DST");
            _sub = _pro.GetSubProductDefByIdentify("DBLV");
        }

        private int[] GetAOI(IRasterDataProvider prd, bool isLand)
        {
            using (VectorAOITemplate v = VectorAOITemplateFactory.GetAOITemplate("海陆模版"))
            {
                Size size;
                Envelope evp = GetEnvelope(out size, prd);
                int[] landAoi = v.GetAOI(evp, size);  //陆地
                int[] reverseAOI = AOIHelper.Reverse(landAoi, size); //海洋
                return isLand ? landAoi : reverseAOI;
            }
        }

        private Envelope GetEnvelope(out Size size, IRasterDataProvider prd)
        {
            size = new System.Drawing.Size();
            size.Width = prd.Width;
            size.Height = prd.Height;
            return new Envelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinY, prd.CoordEnvelope.MaxX, prd.CoordEnvelope.MaxY);
        }

        private void CreatBitmap(IRasterDataProvider prd, int[] idxs, string saveName)
        {
            IBinaryBitmapBuilder builder = new BinaryBitmapBuilder();
            Size bmSize = new Size(prd.Width / 2, prd.Height / 2);
            Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, Color.Red, Color.White);
            builder.Fill(idxs, new Size(prd.Width, prd.Height), ref bitmap);
            bitmap.Save(saveName, ImageFormat.Png);
        }
        #endregion

        #region getRasterDataProvider
        private IRasterDataProvider GetRasterDataProviderBinaryFile()
        {
            string fname = AppDomain.CurrentDomain.BaseDirectory + "TEMP\\DST_2VAL_FY3A_MERSI_1000M_20110613183625_20110614183625.dat";
            return GeoDataDriver.Open(fname) as IRasterDataProvider;
        }

        private IRasterDataProvider GetRasterDataProviderVIRR()
        {
            string fname = @"E:\第二张盘\01_沙尘\2011年04月30日\FY3A_VIRRX_GBAL_L1_20110430_0305_1000M_MS_PRJ_DXX.LDF";
            return GeoDataDriver.Open(fname) as IRasterDataProvider;
        }

        private IRasterDataProvider GetRasterDataProviderNOAA18()
        {
            string fname = @"E:\data\dst\NOAA18_AVHRR_CHINA_L1_20090806_N3_1000M_PRJ_Whole_Clip.LDF";
            return GeoDataDriver.Open(fname) as IRasterDataProvider;
        }

        private IRasterDataProvider GetRasterDataProviderMODIS()
        {
            string fname = @"E:\data\dst\TERRA_2010_03_25_03_09_GZ.MOD021KM_PRJ_DXX_Clip.LDF";
            return GeoDataDriver.Open(fname) as IRasterDataProvider;
        }

        private IRasterDataProvider GetRasterDataProviderMESI()
        {
            string fname = @"E:\data\dst\FY3A_MERSI_GBAL_L1_20110513_0220_1000M_MS_PRJ_DXX.LDF";
            return GeoDataDriver.Open(fname) as IRasterDataProvider;
        }
        #endregion

        string _fname = @"E:\data\dst\DST_2VAL_FY3A_VIRR_1000M_20110614091419_20110615091419.dat";
        private void 矢量统计测试_Click(object sender, EventArgs e)
        {
            //fname = "F:\\FIR_NDVI_FY3A_MERSI_250M_20120610164136_20120611164136.dat";
            IAOITemplateStat<UInt16> stat = new AOITemplateStat<UInt16>();
            string fullname = VectorAOITemplate.FindVectorFullname("省级行政区域_面.shp");
            StatResultItem[] items = stat.CountByVector(_fname, fullname, "NAME", (x) => x == 1);
        }

        private void 统计行政区划_Click(object sender, EventArgs e)
        {
            //fname = @"D:\MAS_Workspace\OutputItem\SandDust\MultiValueGraph\20110615\DST_DBLV_FY3A_VIRR_1000M_DXX_P001_20110430030500.MVG";
            IStatAnalysisEngine<UInt16> stat = new StatAnalysisEngine<UInt16>();
            StatResultItem[] results = stat.StatArea(_fname, "省级行政区划", x => x == 1);
        }

        private void 显示_Click(object sender, EventArgs e)
        {
            IStatAnalysisEngine<UInt16> stat = new StatAnalysisEngine<UInt16>();
            StatResultItem[] results = stat.StatArea(_fname, "省级行政区划", x => x == 1);
            //IStatResultWindow window =  new frm
        }

        MonitoringProductDst dst;
        private void button1_Click(object sender, EventArgs e)
        {
            InitExIdentify();
            dst = new MonitoringProductDst();
        }

        private void button2_Click(object sender, EventArgs e)
        {
           // int[] aoi = AOITemplateFactory.MakeAOI("raster:行政区划:河北省", 70,141,15,56,new Size(7100,4100));
           // int[] aoi2 = AOITemplateFactory.MakeAOI("raster:土地利用类型:林地", 70, 141, 15, 56, new Size(7100, 4100));
            //int[] aoi3 = AOITemplateFactory.MakeAOI("vector:太湖", 119, 121, 30, 32, new Size(74, 62));
            int[] aoi3 = AOITemplateFactory.MakeAOI("vector:洞庭湖", 112.083749, 113.178749, 28.691249, 29.588749, new Size(110, 90));
            int[] aoi = aoi3;
            Envelope en = new Envelope(20, 30,60, 90);

        }

        private void 分级行政区划_Click(object sender, EventArgs e)
        {
            string fname = @"E:\王羽\历史数据\FOG_DBLV_FY3A_MERSI_1000M_20120614005135_20120615005135.dat";
            using (IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider)
            {
                StatResultItem[] items = null;
                IStatAnalysisEngine<UInt16> exe = new StatAnalysisEngine<UInt16>();
                items = exe.StatArea(prd, "分级行政区划", (v) => { return v == 1; });
            }     
        }

 
    }
}
