using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.Diagnostics;
using GeoDo.RSS.Core.DF;
using System.Drawing.Imaging;
using CodeCell.AgileMap.Core;
using GeoDo.Project;

namespace test
{
    public partial class SandExtract : Form
    {
        public SandExtract()
        {
            InitializeComponent();
        }

        IPixelIndexMapper result;
        int[] idxs;

        private void SandExtructTest(string fname, Dictionary<string, object> args, string express, int[] bandNos, string saveName)
        {
            //构造参数提供者
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            ArgumentProvider argprd = new ArgumentProvider(prd, args);
            //构造判识表达式
            //构造基于阈值的判识器
            IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();
            extracter.Reset(argprd, bandNos, express);
            //判识
            result = PixelIndexMapperFactory.CreatePixelIndexMapper("DST", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            extracter.Extract(result);
            idxs = result.Indexes.ToArray();
            sw.Stop();
            Text = sw.ElapsedMilliseconds.ToString();
            //判识结果生成二值位图
            IBinaryBitmapBuilder builder = new BinaryBitmapBuilder();
            Size bmSize = new Size(prd.Width / 2, prd.Height / 2);
            Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, Color.Red, Color.White);
            builder.Fill(idxs, new Size(prd.Width, prd.Height), ref bitmap);
            bitmap.Save(saveName, ImageFormat.Png);
            //判识结果永久保存
            RasterIdentify id = new RasterIdentify();
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "SANDDUST";
            id.SubProductIdentify = "2VAL";
            id.Satellite = "FY3A";
            id.Sensor = "MERSI";
            id.Resolution = "1000M";
            id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            id.GenerateDateTime = DateTime.Now;
            IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(prd.Width, prd.Height), prd.CoordEnvelope.Clone());
            iir.Put(idxs, 1);
            iir.Dispose();
        }

        #region 陆地沙尘判识
        //FY3A_MERSI
        private void 简单判识_陆地_Click(object sender, EventArgs e)
        {
            //陆地数据，新疆地区
            //FY3A_MERSI、FY3A_VIRRX
            string fname = @"H:\产品使用数据\01_沙尘\2011年04月06日\FY3A_MERSI_GBAL_L1_20110406_0540_1000M_MS_PRJ_DXX.LDF";
            //FY3A_VIRR
            fname = @"H:\产品使用数据\01_沙尘\2011年04月30日\FY3A_VIRRX_GBAL_L1_20110430_0305_1000M_MS_PRJ_DXX.LDF";
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("a", 28);
            args.Add("b", 78);
            args.Add("c", 245);
            args.Add("d", 293);
            args.Add("f", 0);
            args.Add("g", 20);
            args.Add("h", 15);
            args.Add("i", 250);
            //构造判识表达式
            string express = "((band2/10f) > var_a) && (band2/10f < var_b) && (band5/10f > var_c) && (band5/10f < var_d) && "
                           + "(band6/10f > var_a) && ((band6 - band2)>var_f)  && ((band6/10f - band5/10f + var_i)>var_h)";
            //band2:可见光，0.525~0.575（波长范围）
            //band5:远红外，10.3~11.55
            //band6:短波红外，1.60~1.69
            int[] bandNos = new int[] { 2, 5, 6 };
            string saveName = @"h:\landSand.png";
            SandExtructTest(fname, args, express, bandNos, saveName);
        }

        //NOAA16 18|FY2C 2D 2E ;268s
        private void 陆地_NOAA18_Click(object sender, EventArgs e)
        {
            string fname = @"H:\产品使用数据\01_沙尘\NOAA18_AVHRR_CHINA_L1_20090806_N3_1000M_PRJ_Whole_Clip.LDF";
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("a", 0);
            args.Add("b", 200);
            args.Add("c", 265);
            args.Add("d", 283);
            args.Add("e", 273);
            string express = "(band1/10f > var_a) && (band1/10f<var_b) && (band4/10f >var_c)&&(band4/10f < var_d) && (band3/10f >var_e) &&" +
                             "((band3/10f - band4/10f) > var_a)";
            int[] bandNos = new int[] { 1, 4, 3 };
            string saveName = @"h:\sandLandNOAA18.png";
            SandExtructTest(fname, args, express, bandNos, saveName);
        }

        //MODIS
        private void 陆地_MODIS_Click(object sender, EventArgs e)
        {
            string fname = @"H:\产品使用数据\01_沙尘\TERRA_2010_03_25_03_09_GZ.MOD021KM_PRJ_DXX_Clip.LDF";
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("a", 15);
            args.Add("b", 40);
            args.Add("c", 10);
            args.Add("d", 78);
            args.Add("e", 270);
            args.Add("f", 320);
            args.Add("g", 260);
            args.Add("h", 293);
            string express = "(band1/10f > var_a) && (band1/10f < var_b) && (band6/10f > var_c) && (band6/10f < var_d) &&" +
                             "(band22/10f > var_e) && (band22/10f <var_f) && (band33/10f >var_g) && (band33/10f < var_h)";
            int[] bandNos = new int[] { 1, 6, 22, 33 };
            string saveName = "h:\\sandLandModis.png";
            SandExtructTest(fname, args, express, bandNos, saveName);
        }

        #endregion

        #region 海上沙尘判识
        //使用海上数据
        private void 简单判识_海上_Click(object sender, EventArgs e)
        {
            //海上数据
            string fname = @"H:\产品使用数据\01_沙尘\0513_海上数据\FY3A_MERSI_GBAL_L1_20110513_0220_1000M_MS_PRJ_DXX.LDF";
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("a", 8);
            args.Add("b", 26);
            args.Add("c", 265);
            args.Add("d", 283);
            args.Add("e", -10);
            args.Add("f", 0);
            string express = "(band2/10f > var_a) && (band2/10f < var_b) && (band5/10f > var_c) && (band5/10f < var_d) && "
                           + "(band6/10f > var_a)&& ((band6/10f - band4/10f)>var_e) &&((band2/10f - band4/10f )>var_f)";
            int[] bandNos = new int[] { 2, 5, 6, 4 };
            string saveName = "h:\\sandSeaMersi.png";
            SandExtructTest(fname, args, express, bandNos, saveName);
        }
        #endregion

        #region 云检测
        //成功检测云
        private void 简单判识_云检测_Click(object sender, EventArgs e)
        {
            //构造参数提供者
            string fname = @"H:\01_沙尘\2011年04月06日\FY3A_MERSI_GBAL_L1_20110406_0540_1000M_MS_PRJ_DXX.LDF";
            Dictionary<string, object> args = new Dictionary<string, object>();
            args.Add("a", 200);
            args.Add("b", 1000);
            args.Add("c", 0);
            args.Add("d", 2750);
            string express = "(band3 > var_a) && (band3 < var_b) && (band5 > var_c) && (band5 < var_d)";
            int[] bandNos = new int[] { 3, 5 };
            string saveName = "h:\\CloudSand.png";
            SandExtructTest(fname, args, express, bandNos, saveName);
        }
        #endregion

        int[] landAoi;
        IPixelIndexMapper landResult;
        IPixelIndexMapper seaResult;
        int[] landInd;
        int[] seaIdx;
        private void 海陆模板_Click(object sender, EventArgs e)
        {
            using (VectorAOITemplate v = VectorAOITemplateFactory.GetAOITemplate("海陆模版")) //贝尔湖
            {
                Size size;
                IRasterDataProvider prd;
                Envelope evp = GetEnvelope(out size, out prd);
                Stopwatch sw = new Stopwatch();
                sw.Start();
                landAoi = v.GetAOI(evp, size);
                //
                int[] reverseAOI = AOIHelper.Reverse(landAoi, size);
                //IBinaryBitmapBuilder b = new BinaryBitmapBuilder();
                //Bitmap bm = b.CreateBinaryBitmap(size, Color.Red, Color.Black);
                //b.Fill(reverseAOI, size, ref bm);
                //对陆地区域使用陆地判识算法
                Dictionary<string, object> args = new Dictionary<string, object>();
                //args.Add("a", 28);
                //args.Add("b", 78);
                //args.Add("c", 245);
                //args.Add("d", 293);
                //args.Add("f", 28);
                //args.Add("g", 0);
                //args.Add("h", 15);
                //args.Add("i", 250);
                //构造判识表达式
                //string express = "((band2/10f) > var_a) && (band2/10f < var_b) && (band5/10f > var_c) && (band5/10f < var_d) && "
                //               + "(band6/10f > var_f) && ((band6 - band2)>var_g)  && ((band6/10f - band5/10f + var_i)>var_h)";
                ////band2:可见光，0.525~0.575（波长范围）
                ////band5:远红外，10.3~11.55
                ////band6:短波红外，1.60~1.69
                //int[] bandNos = new int[] { 2, 5, 6 };
                ////构造栅格计算判识器
                //ArgumentsProvider argprd = new ArgumentsProvider(prd, args);
                //argprd.AOI = reverseAOI;
                //IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();
                //extracter.Reset(argprd, bandNos, express);
                ////判识
                //landResult = new MemPixelIndexMapper("SAND", 1000);
                //extracter.Extract(landResult);
                //landInd = landResult.Indexes.ToArray();

                //对海洋区域使用海洋判识算法
                args.Clear();
                args.Add("a", 8);
                args.Add("b", 26);
                args.Add("c", 265);
                args.Add("d", 283);
                args.Add("e", -10);
                args.Add("f", 0);
                string express = "(band2/10f > var_a) && (band2/10f < var_b) && (band5/10f > var_c) && (band5/10f < var_d) && "
                               + "(band6/10f > var_a)&& ((band6/10f - band4/10f)>var_e) &&((band2/10f - band4/10f )>var_f)";
                int[] bandNos = new int[] { 2, 5, 6, 4 };
                ArgumentProvider argprd = new ArgumentProvider(prd, args);
                argprd.AOI = reverseAOI;
                IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();
                extracter.Reset(argprd, bandNos, express);
                //判识
                seaResult = PixelIndexMapperFactory.CreatePixelIndexMapper("DST", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
                extracter.Extract(seaResult);
                seaIdx = seaResult.Indexes.ToArray();

                //
                IBinaryBitmapBuilder builder = new BinaryBitmapBuilder();
                Size bmSize = new Size(prd.Width / 2, prd.Height / 2);
                Bitmap bitmap = builder.CreateBinaryBitmap(bmSize, Color.Red, Color.White);
                builder.Fill(landInd, new Size(prd.Width, prd.Height), ref bitmap);
                bitmap.Save("h:\\陆地沙尘.png", ImageFormat.Png);
                builder.Fill(seaIdx, new Size(prd.Width, prd.Height), ref bitmap);
                bitmap.Save("h:\\海洋沙尘.png", ImageFormat.Png);
                //
                RasterIdentify id = new RasterIdentify();
                id.ThemeIdentify = "CMA";
                id.ProductIdentify = "SAND";
                id.SubProductIdentify = "2VAL";
                id.Satellite = "FY3A";
                id.Sensor = "MERSI";
                id.Resolution = "1000M";
                id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
                id.GenerateDateTime = DateTime.Now;
                IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(prd.Width, prd.Height), prd.CoordEnvelope.Clone());
                iir.Put(landInd, 1);
                iir.Put(seaIdx, 1);
                sw.Stop();
                Text = sw.ElapsedMilliseconds.ToString();
                iir.Dispose();
            }


            ////取海洋矢量作为AOI，使用海洋的判识算法
            //Dictionary<string, object> seaargs = new Dictionary<string, object>();
            //seaargs.Add("sa", 8);
            //seaargs.Add("sb", 26);
            //seaargs.Add("sc", 265);
            //seaargs.Add("sd", 283);
            //seaargs.Add("se", -10);
            //seaargs.Add("sf", 0);
            //string seaExpress = "(band2/10f > var_sa) && (band2/10f < var_sb) && (band5/10f > var_sc) && (band5/10f < var_sd) && "
            //                  + "(band6/10f > var_sa)&& ((band6/10f - band4/10f)>var_se) &&((band2/10f - band4/10f )>var_sf)";
            //int[] seabandNos = new int[] { 2, 5, 6, 4 };
            //ArgumentsProvider seaargprd = new ArgumentsProvider(prd, seaargs);
            ////构造判识表达式
            ////构造基于阈值的判识器
            //IThresholdExtracter<UInt16> seaextracter = new SimpleThresholdExtracter<UInt16>();
            //seaextracter.Reset(seaargprd, seabandNos, seaExpress);
            ////判识
            //IPixelIndexMapper searesult = new MemPixelIndexMapper("SAND", 1000);
            //seaextracter.Extract(searesult);
            //int[] seaidxs = searesult.Indexes.ToArray();


            //将两者判识的idex合并

        }

        private Envelope GetEnvelope(out Size size, out IRasterDataProvider prd)
        {
            string fname = @"H:\产品使用数据\01_沙尘\0513_海上数据\FY3A_MERSI_GBAL_L1_20110513_0220_1000M_MS_PRJ_DXX.LDF";
            prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            size = new System.Drawing.Size();
            size.Width = prd.Width;
            size.Height = prd.Height;
            return new Envelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinY, prd.CoordEnvelope.MaxX, prd.CoordEnvelope.MaxY);
        }

    }
}
