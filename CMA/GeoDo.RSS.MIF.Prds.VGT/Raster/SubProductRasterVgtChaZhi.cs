using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.Drawing;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class SubProductRasterVgtChaZhi : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductRasterVgtChaZhi()
            : base()
        {

        }

        public SubProductRasterVgtChaZhi(SubProductDef subProductDef)
            : base(subProductDef)
        {
            _identify = subProductDef.Identify;
            _name = subProductDef.Name;
            if (subProductDef.Algorithms != null)
            {
                _algorithmDefs = subProductDef.Algorithms.ToList();
            }
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            string algorith = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algorith != "CHAZ")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
            if (_argumentProvider.GetArg("mainfiles") == null)
            {
                PrintInfo("请选择植被指数(被减数)数据。");
                return null;
            }
            string bjianshu = _argumentProvider.GetArg("mainfiles").ToString();
            if (!File.Exists(bjianshu))
            {
                PrintInfo("所选择的数据:\"" + bjianshu + "\"不存在。");
                return null;
            }
            if (_argumentProvider.GetArg("jianshu") == null)
            {
                PrintInfo("请选择植被指数(减数)数据。");
                return null;
            }
            string jianshu = _argumentProvider.GetArg("jianshu").ToString();
            if (!File.Exists(jianshu))
            {
                PrintInfo("所选择的数据:\"" + jianshu + "\"不存在。");
                return null;
            }
            if (_argumentProvider.GetArg("NdviCH1") == null)
            {
                PrintInfo("参数\"NdviCH1\"为空。");
                return null;
            }
            if (_argumentProvider.GetArg("NdviCH2") == null)
            {
                PrintInfo("参数\"NdviCH2\"为空。");
                return null;
            }

            int ch1 = (int)_argumentProvider.GetArg("NdviCH1");
            int ch2 = (int)_argumentProvider.GetArg("NdviCH2");
            if (ch1 < 0 || ch2 < 0)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            //Create virtualProvider
            Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            filePrdMap.Add("beijianshu", new FilePrdMap(bjianshu, 1, new VaildPra(float.MinValue, float.MaxValue), new int[] { ch1 }));
            filePrdMap.Add("jianshu", new FilePrdMap(jianshu, 1, new VaildPra(float.MinValue, float.MaxValue), new int[] { ch2 }));

            //为防止用户选择的波段超出数据波段的界限
            if (filePrdMap["beijianshu"].BandCount < 1 || filePrdMap["jianshu"].BandCount < 1)
            {
                PrintInfo("请选择正确的栅格数据进行差值计算。");
                return null;
            }
            if (filePrdMap["beijianshu"].BandCount < ch1 || filePrdMap["jianshu"].BandCount < ch2)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            IVirtualRasterDataProvider prd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
            if (prd == null)
                throw new Exception("数据间无相交部分,无法创建虚拟数据提供者!");
            IRasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(new ArgumentProvider(prd, null));
            IPixelFeatureMapper<Int16> result = new MemPixelFeatureMapper<Int16>("CHAZ", prd.Width * prd.Height, new Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.SpatialRef);
            visitor.VisitPixel(new int[] { filePrdMap["beijianshu"].StartBand, filePrdMap["jianshu"].StartBand },
                                (idx, values) =>
                                {
                                    result.Put(idx, (Int16)(values[0] - values[1]));
                                }
                );
            return result;
        }

        private void PrintInfo(string info)
        {
            if (_contextMessage != null)
                _contextMessage.PrintMessage(info);
            else
                Console.WriteLine(info);
        }
    }
}
