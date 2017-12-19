using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using System.Diagnostics;
using GeoDo.Project;

namespace TestDst
{
    public partial class SandDust : Form
    {
        public SandDust()
        {
            InitializeComponent();
        }

        IPixelIndexMapper extractResult;
        int[] idxs;
        IPixelFeatureMapper<UInt16> result;
        private void 沙尘能见度_Click(object sender, EventArgs e)
        {
            //首先进行沙尘判识，得出判识出的沙尘区域
            string fname = @"E:\第二张盘\01_沙尘\2011年04月30日\FY3A_VIRRX_GBAL_L1_20110430_0305_1000M_MS_PRJ_DXX.LDF";
            //Dictionary<string, object> args = new Dictionary<string, object>();
            //args.Add("a", 28);
            //args.Add("b", 78);
            //args.Add("c", 245);
            //args.Add("d", 293);
            //args.Add("f", 0);
            //args.Add("g", 20);
            //args.Add("h", 15);
            //args.Add("i", 250);
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            //ArgumentsProvider argprd = new ArgumentsProvider(prd, args);
            //string extractExpress = "((band2/10f) > var_a) && (band2/10f < var_b) && (band5/10f > var_c) && (band5/10f < var_d) && "
            //                      + "(band6/10f > var_a) && ((band6 - band2)>var_f)  && ((band6/10f - band5/10f + var_i)>var_h)";
            //band2:可见光，0.525~0.575（波长范围）
            //band5:远红外，10.3~11.55
            //band6:短波红外，1.60~1.69
            //int[] exBandNos = new int[] { 2, 5, 6 };
            //IThresholdExtracter<UInt16> thrExtracter = new SimpleThresholdExtracter<UInt16>();
            //thrExtracter.Reset(argprd, exBandNos, extractExpress);
            //extractResult = new MemPixelIndexMapper("SAND", 1000);
            //thrExtracter.Extract(extractResult);
            //idxs = extractResult.Indexes.ToArray(); //获取到判识结果

            //将判识结果作为AOI传入进行能见度计算
            string express = "(UInt16)Math.Round(1000 * Math.Pow(Math.E,(var_visibleA + var_visibleB * band1/10f + var_visibleC * band2/10f + var_visibleD * band6/10f + var_visibleE * band4/10f + var_visibleF *(band6/10f - band4/10f + var_shortFar))),0)";
            //express = " 80 ";
            //string express = "1000*Math.Pow(Math.E,(44.7603 +0.181571 * band2/10f -0.332972 * band4/10f + 0.122736 * band6/10f -0.144287 * band5/10f -0.114465 *(band6/10f - band5/10f + 253)))";
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("visibleA", 44.7603);
            args.Add("visibleB", 0.181571);
            args.Add("visibleC", -0.332972);
            args.Add("visibleD", 0.122736);
            args.Add("visibleE", -0.144287);
            args.Add("visibleF", -0.114465);
            args.Add("shortFar", 253);
            int[] bandNos = new int[] { 1,2,6,4 };
            ArgumentProvider argProvider = new ArgumentProvider(prd, args);
            //argProvider.AOI = idxs;
            IRasterExtracter<UInt16, UInt16> extracter = new SimpleRasterExtracter<UInt16, UInt16>();
            extracter.Reset(argProvider, bandNos, express);
            result = new MemPixelFeatureMapper<UInt16>("Visibility", 1000, new Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.SpatialRef);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            extracter.Extract(result);
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
            iir.Put(result);
            iir.Dispose();
            sw.Stop();
            Text = sw.ElapsedMilliseconds.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UInt16[] values = new UInt16[] { 295, 698, 315, 463 };
            UInt16 result = (UInt16)Math.Pow(Math.E, (44.7603 + 0.181571 * values[0] / 10f - 0.332972 * values[1] / 10f + 0.122736 * values[2] / 10f - 0.144287 * values[3] / 10f - 0.114465 * (values[2] / 10f - values[3] / 10f + 253)));
            UInt16 s = result;
        }

        private void 沙尘能见度AOI_Click(object sender, EventArgs e)
        {
            //首先进行沙尘判识，得出判识出的沙尘区域
            string fname = @"H:\产品使用数据\01_沙尘\2011年04月30日\FY3A_VIRRX_GBAL_L1_20110430_0305_1000M_MS_PRJ_DXX.LDF";
            Dictionary<string, object> extractArgs = new Dictionary<string, object>();
            extractArgs.Add("a", 28);
            extractArgs.Add("b", 78);
            extractArgs.Add("c", 245);
            extractArgs.Add("d", 293);
            extractArgs.Add("f", 0);
            extractArgs.Add("g", 20);
            extractArgs.Add("h", 15);
            extractArgs.Add("i", 250);
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            ArgumentProvider argprd = new ArgumentProvider(prd, extractArgs);
            string extractExpress = "((band2*) > var_a) && (band2/10f < var_b) && (band5/10f > var_c) && (band5/10f < var_d) && "
                                  + "(band6/10f > var_a) && ((band6 - band2)>var_f)  && ((band6/10f - band5/10f + var_i)>var_h)";
            //band2:可见光，0.525~0.575（波长范围）
            //band5:远红外，10.3~11.55
            //band6:短波红外，1.60~1.69
            int[] exBandNos = new int[] { 2, 5, 6 };
            IThresholdExtracter<UInt16> thrExtracter = new SimpleThresholdExtracter<UInt16>();
            thrExtracter.Reset(argprd, exBandNos, extractExpress);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            //extractResult = new MemPixelIndexMapper("SAND", 1000);
            thrExtracter.Extract(extractResult);
            idxs = extractResult.Indexes.ToArray(); //获取到判识结果

            //将判识结果作为AOI传入进行能见度计算
            string express = "(UInt16)Math.Round(1000 * Math.Pow(Math.E,(var_visibleA + var_visibleB * band1/10f + var_visibleC * band2/10f + var_visibleD * band6/10f + var_visibleE * band4/10f + var_visibleF *(band6/10f - band4/10f + var_shortFar))),0)";
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("visibleA", 44.7603);
            args.Add("visibleB", 0.181571);
            args.Add("visibleC", -0.332972);
            args.Add("visibleD", 0.122736);
            args.Add("visibleE", -0.144287);
            args.Add("visibleF", -0.114465);
            args.Add("shortFar", 253);
            int[] bandNos = new int[] { 1, 2, 6, 4 };
            ArgumentProvider argProvider = new ArgumentProvider(prd, args);
            argProvider.AOI = idxs;
            IRasterExtracter<UInt16, UInt16> extracter = new SimpleRasterExtracter<UInt16, UInt16>();
            extracter.Reset(argProvider, bandNos, express);
            result = new MemPixelFeatureMapper<UInt16>("Visibility", 1000, new Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.SpatialRef);
           
            extracter.Extract(result);
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
            iir.Put(result);
            iir.Dispose();
            sw.Stop();
            Text = sw.ElapsedMilliseconds.ToString();
        }

        private void XML解析测试_Click(object sender, EventArgs e)
        {
            //ExtractThemesParser parser = new ExtractThemesParser();
            //ThemeDef[] themes = parser.Parse();
        }
    }
}
