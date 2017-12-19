using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.FIR;
using GeoDo.RSS.Core.DF;

namespace testFir
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            GeoDataDriver.PreLoading();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //IArgumentProviderFactory fac = MifEnvironment.ActiveArgumentProviderFactory;
            ThemeDef themeDef = MonitoringThemeFactory.GetThemeDefByIdentify("CMA");
            IMonitoringTheme them = new MonitoringThemeCMA(themeDef);
            IMonitoringProduct fir = them.GetProductByIdentify("FIR");
            ExtractProductIdentify prdIdentify = new ExtractProductIdentify();
            prdIdentify.ThemeIdentify = "CMA";
            prdIdentify.ProductIdentify = "FIR";
            prdIdentify.SubProductIdentify = "DBLV";
            ExtractAlgorithmIdentify algIdentify = new ExtractAlgorithmIdentify();
            algIdentify.Satellite = "FY3A";
            algIdentify.Sensor = "VIRR";
            algIdentify.Resolution = null;//not use
            IArgumentProvider arg = MonitoringThemeFactory.GetArgumentProvider(prdIdentify, algIdentify);
            IMonitoringSubProduct bin = fir.GetSubProductByIdentify("DBLV");
            //arg.SetArg(bin.AlgorithmDefs[0].Indetify, bin.AlgorithmDefs[0]);
            arg.DataProvider = GetRasterDataProvider();
            arg.AOI = GetAOI();
            IExtractResult result = bin.Make(null);
        }

        private int[] GetAOI()
        {
            throw new NotImplementedException();
        }

        private IRasterDataProvider GetRasterDataProvider()
        {
            throw new NotImplementedException();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string fname1 = "F:\\FIR_NDVI_FY3A_MERSI_250M_20120610164136_20120611164136.dat";
            string fname2 = "F:\\FIR_NDVI_FY3A_MERSI_250M_20120610164136_20120611164136 - 副本.dat";

            IRasterOperator<float> oper = new RasterOperator<float>();
            oper.Times(new string[] { fname1, fname2 }, new RasterIdentify(), (v1, v2) => { return v1 + v2; });
        }

        int count;
        private void button3_Click(object sender, EventArgs e)
        {
            string fname1 = "F:\\FIR_NDVI_FY3A_MERSI_250M_20120610164136_20120611164136.dat";
            IRasterDataProvider prd = GeoDataDriver.Open(fname1) as IRasterDataProvider;
            IRasterOperator<float> oper = new RasterOperator<float>();
            count = oper.Count(prd, null, (v) => { return true; });
        }

        double area;
        private void button4_Click(object sender, EventArgs e)
        {
            string fname1 = "F:\\FIR_NDVI_FY3A_MERSI_250M_20120610164136_20120611164136.dat";
            IRasterDataProvider prd = GeoDataDriver.Open(fname1) as IRasterDataProvider;
            IRasterOperator<float> oper = new RasterOperator<float>();
            area = oper.Area(prd, null, (v) => { return true; });
        }

        int[] aoi;
        string name;
        private void button5_Click(object sender, EventArgs e)
        {
            IRasterDictionaryTemplate<int> temp = RasterDictionaryTemplateFactory.GetXjRasterTemplate();
            aoi = temp.GetAOI("北京市", 114, 124, 36, 46, new Size(1000, 1000));
            //temp.CodeNameParis
            name = temp.GetPixelName(114d, 39d);
        }
    }
}
