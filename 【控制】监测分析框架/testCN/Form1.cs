using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.ICE;
using GeoDo.RSS.Core.DF;
using System.Drawing.Imaging;
using GeoDo.RSS.MIF.Prds.FOG;

namespace testCN
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btSeaIceNDSI_Click(object sender, EventArgs e)
        {
            ExtractProductIdentify exPro = new ExtractProductIdentify();
            exPro.ThemeIdentify = "CMA";
            exPro.ProductIdentify = "ICE";
            exPro.SubProductIdentify = "DBLV";

            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            ThemeDef theme = MonitoringThemeFactory.GetThemeDefByIdentify("CMA");
            ProductDef pro = theme.GetProductDefByIdentify("ICE");
            SubProductDef sub = pro.GetSubProductDefByIdentify("DBLV");
            AlgorithmDef alg = sub.GetAlgorithmDefByIdentify("NDSIAlgorithm_NOAA");

            IArgumentProvider arg = MonitoringThemeFactory.GetArgumentProvider(exPro, "NDSIAlgorithm_NOAA", "FY3A", "VIRR");
            arg.SetArg("NDSIAlgorithm_NOAA", alg);
            IRasterDataProvider prd = GetRasterDataProvider("ICE");
            arg.DataProvider = prd;
            IMonitoringSubProduct bin = new SubProductBinaryICE(sub);
            IPixelIndexMapper result = bin.Make(null) as IPixelIndexMapper;
            string saveName = "e:\\seaice1.png";
            CreatBitmap(prd, result.Indexes.ToArray(), saveName);
        }

        private void CreatBitmap(IRasterDataProvider prd, int[] idxs, string saveName)
        {
            IBinaryBitmapBuilder builder = new BinaryBitmapBuilder();
            Size bmSize = new Size(prd.Width / 2, prd.Height / 2);
            Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, Color.Red, Color.White);
            builder.Fill(idxs, new Size(prd.Width, prd.Height), ref bitmap);
            bitmap.Save(saveName, ImageFormat.Png);
        }

        private IRasterDataProvider GetRasterDataProvider(string type)
        {
            if (type == "ICE")
            {
                string fname = @"C:\Users\Administrator\Desktop\数据\2_海冰\FY3A_VIRRX_GBAL_L1_20110116_0225_1000M_MS_PRJ_DXX.LDF";
                return GeoDataDriver.Open(fname) as IRasterDataProvider;
            }
            if (type == "FOG")
            {
                string fname = @"C:\Users\Administrator\Desktop\数据\3_大雾\FY3A_MERSI_GBAL_L1_20120328_0140_1000M_MS_PRJ_DXX.LDF";
                return GeoDataDriver.Open(fname) as IRasterDataProvider;
            }
            return null;
        }

        private void btFogBB_Click(object sender, EventArgs e)
        {
            ExtractProductIdentify exPro = new ExtractProductIdentify();
            exPro.ThemeIdentify = "CMA";
            exPro.ProductIdentify = "FOG";
            exPro.SubProductIdentify = "DBLV";

            ExtractAlgorithmIdentify exAlg = new ExtractAlgorithmIdentify();
            exAlg.CustomIdentify = null;
            exAlg.Satellite = "FY3A";
            exAlg.Sensor = "MERSI";
            exAlg.Resolution = null;

            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            ThemeDef theme = MonitoringThemeFactory.GetThemeDefByIdentify("CMA");
            ProductDef pro = theme.GetProductDefByIdentify("FOG");
            SubProductDef sub = pro.GetSubProductDefByIdentify("DBLV");
            AlgorithmDef alg = sub.GetAlgorithmDefByIdentify("EasyAlgorithm");

            IArgumentProvider arg = MonitoringThemeFactory.GetArgumentProvider(exPro, "EasyAlgorithm", "FY3A", "MERSI");
            arg.SetArg("EasyAlgorithm", alg);
            IRasterDataProvider prd = GetRasterDataProvider("FOG");
            arg.DataProvider = prd;
            IMonitoringSubProduct bin = new SubProductBinaryFOG(sub);
            IPixelIndexMapper result = bin.Make(null) as IPixelIndexMapper;
            RasterIdentify id = new RasterIdentify();
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FOG";
            id.SubProductIdentify = "DBLV";
            id.Satellite = "FY3A";
            id.Sensor = "MERSI";
            id.Resolution = "1000M";
            id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            id.GenerateDateTime = DateTime.Now;
            IInterestedRaster<Int16> iir = new InterestedRaster<Int16>(id, new Size(prd.Width, prd.Height), prd.CoordEnvelope);
            iir.Put(result.Indexes.ToArray(), 1);
            iir.Dispose();
        }

        private void btCSR_Click(object sender, EventArgs e)
        {
            //IArgumentProviderFactory fac1 = MifEnvironment.ActiveArgumentProviderFactory;

            ThemeDef theme1 = MonitoringThemeFactory.GetThemeDefByIdentify("CMA");  // fac1.GetThemeDefByIdentify("CMA");
            ProductDef pro1 = theme1.GetProductDefByIdentify("FOG");

            //ProductDef pro1 = theme1.GetProductDefByIdentify("ICE");

            MonitoringProduct mp = new MonitoringProductFOG();

            //MonitoringProduct mp = new MonitoringProductICE(pro1);
            mp = mp;

            ExtractProductIdentify exPro = new ExtractProductIdentify();
            exPro.ThemeIdentify = "CMA";
            exPro.ProductIdentify = "FOG";
            exPro.SubProductIdentify = "0CSR";

            ExtractAlgorithmIdentify exAlg = new ExtractAlgorithmIdentify();
            exAlg.CustomIdentify = null;
            exAlg.Satellite = "FY3A";
            exAlg.Sensor = "MERSI";
            exAlg.Resolution = null;

            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            ThemeDef theme = MonitoringThemeFactory.GetThemeDefByIdentify("CMA"); //fac.GetThemeDefByIdentify("CMA");
            ProductDef pro = theme.GetProductDefByIdentify("FOG");
            SubProductDef sub = pro.GetSubProductDefByIdentify("0CSR");
            AlgorithmDef alg = sub.GetAlgorithmDefByIdentify("BaseOrbitAlgorithm");

            IArgumentProvider arg = MonitoringThemeFactory.GetArgumentProvider(exPro, "BaseOrbitAlgorithm", "FY3A", "MERSI");
            arg.SetArg("BaseOrbitAlgorithm", alg);
            IRasterDataProvider prd = GetRasterDataProvider("FOG");
            arg.DataProvider = prd;
            arg.SetArg("OrbitFile", new string[] { prd.fileName });
            IMonitoringSubProduct bin = new SubProductCSRFOG(sub);
            IPixelFeatureMapper<UInt16> result = bin.Make(null) as IPixelFeatureMapper<UInt16>;
            result.Dispose();

        }

        private void btOPTD_Click(object sender, EventArgs e)
        {
            ProductColorTable[] tables = ProductColorTableFactory.GetAllColorTables();
            ProductColorTable table = ProductColorTableFactory.GetColorTable("FOG", "TIMS");


            ExtractProductIdentify exPro = new ExtractProductIdentify();
            exPro.ThemeIdentify = "CMA";
            exPro.ProductIdentify = "FOG";
            exPro.SubProductIdentify = "OPTD";

           
            ThemeDef theme =MonitoringThemeFactory.GetThemeDefByIdentify("CMA");
            ProductDef pro = theme.GetProductDefByIdentify("FOG");
            SubProductDef sub = pro.GetSubProductDefByIdentify("OPTD");
            AlgorithmDef alg = sub.GetAlgorithmDefByIdentify("OPTDAlgorithm");

            IArgumentProvider arg = MonitoringThemeFactory.GetArgumentProvider(exPro, "OPTDAlgorithm", "FY3A", "MERSI");
            arg.SetArg("OPTDAlgorithm", alg);
            IRasterDataProvider prd = GetRasterDataProvider("FOG");
            arg.DataProvider = prd;
            arg.SetArg("CSRFile", @"E:\code\SMARTII\SRC\【控制】监测分析框架\testCN\bin\Release\TEMP\FOG_0CSR_FY3A_MERSI_10M_20120614001422_20120615001422.dat");
            arg.SetArg("DBLVFile", @"E:\code\SMARTII\SRC\【控制】监测分析框架\testCN\bin\Release\TEMP\FOG_DBLV_FY3A_MERSI_1000M_20120614005135_20120615005135.dat");
            IMonitoringSubProduct bin = new SubProductOPTDFOG(sub);
            IPixelFeatureMapper<Int16> result = bin.Make(null) as IPixelFeatureMapper<Int16>;
            result.Dispose();
        }

        private void btImage_Click(object sender, EventArgs e)
        {
            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            ThemeDef theme = MonitoringThemeFactory.GetThemeDefByIdentify("CMA");
            ProductDef pro = theme.GetProductDefByIdentify("FOG");
            SubProductDef sub = pro.GetSubProductDefByIdentify("0IMG");
            AlgorithmDef alg = sub.GetAlgorithmDefByIdentify("0IMGAlgorithm");

            ExtractProductIdentify exPro = new ExtractProductIdentify();
            exPro.ThemeIdentify = "CMA";
            exPro.ProductIdentify = "FOG";
            exPro.SubProductIdentify = "0IMG";

            IArgumentProvider arg = MonitoringThemeFactory.GetArgumentProvider(exPro, "0IMGAlgorithm", "FY3A", "MERSI");
            IMonitoringSubProduct subPro = new SubProductIMGFOG(sub);
            subPro.ResetArgumentProvider("FY3A", "MERSI");
            arg.SetArg("AOI", "");
            arg.SetArg("OutFileIdentify", "STBI");
            subPro.Make(null);
        }
    }
}
