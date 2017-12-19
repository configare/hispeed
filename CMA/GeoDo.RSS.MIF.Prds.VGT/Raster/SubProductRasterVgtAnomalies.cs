using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class SubProductRasterVgtAnomalies_old : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductRasterVgtAnomalies_old()
            : base()
        {
        }

        public SubProductRasterVgtAnomalies_old(SubProductDef subProductDef)
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
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
            {
                PrintInfo("参数\"AlgorithmName\"为空。");
                return null;
            }
            string algorith = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (algorith != "ANMI")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
            if (_argumentProvider.GetArg("NDVIFile") == null)
            {
                PrintInfo("请选择植被指数数据。");
                return null;
            }
            string ndvi = _argumentProvider.GetArg("NDVIFile").ToString();
            if (!File.Exists(ndvi))
            {
                PrintInfo("所选择的数据:\"" + ndvi + "\"不存在。");
                return null;
            }
            if (_argumentProvider.GetArg("NdviCH") == null)
            {
                PrintInfo("参数\"NdviCH\"为空。");
                return null;
            }
            int ndviCh = (int)_argumentProvider.GetArg("NdviCH");

            if (_argumentProvider.GetArg("NDVIAvgFile") == null)
            {
                PrintInfo("请选择植被指数年均值数据。");
                return null;
            }
            string avg = _argumentProvider.GetArg("NDVIAvgFile").ToString();
            if (!File.Exists(avg))
            {
                PrintInfo("所选择的数据:\"" + avg + "\"不存在。");
                return null;
            }
            if (_argumentProvider.GetArg("NdviAvgCH") == null)
            {
                PrintInfo("参数\"NdviAvgCH\"为空。");
                return null;
            }
            int avgCH = (int)_argumentProvider.GetArg("NdviAvgCH");
            if (ndviCh < 1 || avgCH < 1)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            //Create virtualProvider
            Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            filePrdMap.Add("NDVIFile", new FilePrdMap(ndvi, 1000, new VaildPra(Int16.MinValue, Int16.MaxValue), new int[] { ndviCh }));
            filePrdMap.Add("NDVIAvgFile", new FilePrdMap(avg, 1000, new VaildPra(Int16.MinValue, Int16.MaxValue), new int[] { avgCH }));

            //为防止用户选择的波段超出数据波段的界限
            if (filePrdMap["NDVIFile"].BandCount < ndviCh || filePrdMap["NDVIAvgFile"].BandCount < avgCH)
            {
                PrintInfo("获取波段序号失败,可能是波段映射表配置错误或判识算法波段参数配置错误。");
                return null;
            }

            ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            IVirtualRasterDataProvider prd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
            if (prd == null)
                throw new Exception("数据间无相交部分,无法创建虚拟数据提供者!");

            IRasterPixelsVisitor<Int16> visitor = new RasterPixelsVisitor<Int16>(new ArgumentProvider(prd, null));
            IPixelFeatureMapper<Int16> result = new MemPixelFeatureMapper<Int16>("ANMI", prd.Width * prd.Height, new System.Drawing.Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.SpatialRef);
            visitor.VisitPixel(new int[] { filePrdMap["NDVIFile"].StartBand,
                                           filePrdMap["NDVIAvgFile"].StartBand},
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
