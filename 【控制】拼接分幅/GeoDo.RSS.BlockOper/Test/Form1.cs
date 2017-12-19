using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.BlockOper;
using System.IO;

namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //t();
        }

        private void t()
        {
            string fname = "E:\\masData\\Mas二期投影结果展示\\FY3A_VIRRX_GBAL_L1_20110322_0525_1000M_MS.ldf";
            IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider;
            BlockDef[] blocks = new BlockDef[] { new BlockDef("DXX", 74.0, 33.0, 10.0) };
            RasterClipProcesser s = new RasterClipProcesser();
            IRasterDataProvider[] outs = s.Clip(prd, blocks, 50, "LDF", "E:\\masData\\Clip",null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string fname1 = "E:\\Mas一期光盘数据\\第2张盘（监测产品培训数据）\\06_积雪\\s04_11b160515.ldf";
            //string fname2 = "E:\\Mas一期光盘数据\\第2张盘（监测产品培训数据）\\06_积雪\\s04_11b170455.ldf";
            //string fname1 = "E:\\FY3A_VIRRX_GBAL_L1_20110322_0520_1000M_MS_PRJ.ldf";
            //string fname2 = "E:\\FY3A_VIRRX_GBAL_L1_20110322_0525_1000M_MS_PRJ.ldf";
            //string fname1 = "E:\\拼接测试数据\\这三个拼接时候出现索引超界错误\\FY3A_VIRRX_GBAL_L1_20090107_0220_1000M_MS_PRJ.ldf";
            //string fname2 = "E:\\拼接测试数据\\这三个拼接时候出现索引超界错误\\FY3A_VIRRX_GBAL_L1_20100624_1139_1000M_MS_PRJ.ldf";
            //string fname1 = "E:\\拼接测试文件\\FY3A_VIRRX_GBAL_L1_20090107_0220_1000M_MS_PRJ_DXX.ldf";
            //string fname2 = "E:\\拼接测试文件\\FY3A_VIRRX_GBAL_L1_20100624_1139_1000M_MS_PRJ_DXX.ldf";
            //string fname3 = "E:\\拼接测试文件\\FY3A_VIRRX_GBAL_L1_20110322_0520_1000M_MS_PRJ_DXX.ldf";
            //string fname4 = "E:\\拼接测试文件\\FY3A_VIRRX_GBAL_L1_20110322_0525_1000M_MS_PRJ_DXX.ldf";
            //string fname5 = "E:\\拼接测试文件\\FY3A_VIRRX_GBAL_L1_20120423_0325_1000M_MS_PRJ_DXX.ldf";
            //string fname6 = "E:\\拼接测试文件\\HM0022014.ldf";
            //string fname1 = @"C:\Documents and Settings\Administrator\桌面\OrbitData\FY3A_VIRRX_3080_L1_20110110_0420_1000M_MS_PRJ_S04.LDF";
            //string fname2 = @"C:\Documents and Settings\Administrator\桌面\OrbitData\FY3A_VIRRX_3090_L1_20110110_0420_1000M_MS_PRJ_S04.LDF";
            //string fname3 = @"C:\Documents and Settings\Administrator\桌面\OrbitData\FY3A_VIRRX_4080_L1_20110110_0420_1000M_MS_PRJ_S04.LDF";
            //string fname4 = @"C:\Documents and Settings\Administrator\桌面\OrbitData\FY3A_VIRRX_4090_L1_20110110_0420_1000M_MS_PRJ_S04.LDF";
            //IRasterDataProvider prd1 = GeoDataDriver.Open(fname1) as IRasterDataProvider;
            //IRasterDataProvider prd2 = GeoDataDriver.Open(fname2) as IRasterDataProvider;
            //IRasterDataProvider prd3 = GeoDataDriver.Open(fname3) as IRasterDataProvider;
            //IRasterDataProvider prd4 = GeoDataDriver.Open(fname4) as IRasterDataProvider;
            //IRasterDataProvider prd5 = GeoDataDriver.Open(fname5) as IRasterDataProvider;
            //IRasterDataProvider prd6 = GeoDataDriver.Open(fname6) as IRasterDataProvider;
            //IRasterDataProvider prd3 = GeoDataDriver.Open(fname3) as IRasterDataProvider;
            //IRasterDataProvider prd4 = GeoDataDriver.Open(fname4) as IRasterDataProvider;
            string dir = @"E:\气象局项目\MAS二期\【控制】代码工程0716\【控制】UI框架(新整理)\SMART\bin\Release\Workspace\VGT\2012-08-22\栅格产品";
            if (!Directory.Exists(dir))
                return ;
            string[] files = Directory.GetFiles(dir, "*.dat", SearchOption.TopDirectoryOnly);
            IRasterDataProvider[] prds = GetProviderFromFiles(files);
            RasterMoasicProcesser processer = new RasterMoasicProcesser();
            IRasterDataProvider drcDataProvider = processer.Moasic<float>(prds, "LDF", "E:\\1.ldf", new CoordEnvelope(70, 140, 10, 60), 0.01f, 0.01f, true, new string[] { "0", "9999", "9998" }, "AVG", null, (srcValue, dstValue) => { return srcValue > dstValue ? srcValue : dstValue; });
        }

        private IRasterDataProvider[] GetProviderFromFiles(string[] files)
        {
            List<IRasterDataProvider> prds = new List<IRasterDataProvider>();
            int length = files.Length;
            IRasterDataProvider temp = null;
            for (int i = 0; i < length; i++)
            {
                temp = GeoDataDriver.Open(files[i]) as IRasterDataProvider;
                if (temp != null)
                    prds.Add(temp);
            }
            return prds.Count == 0 ? null : prds.ToArray();
        }
    }
}
