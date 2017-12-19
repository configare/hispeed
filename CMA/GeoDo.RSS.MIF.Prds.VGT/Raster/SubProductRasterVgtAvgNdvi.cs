using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    //平均值合成
    public class SubProductRasterVgtAvgNdvi_old : CmaMonitoringSubProduct
    {
        private IContextMessage _contextMessage = null;

        public SubProductRasterVgtAvgNdvi_old()
            : base()
        {
        }

        public SubProductRasterVgtAvgNdvi_old(SubProductDef subProductDef)
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
            if (algorith != "0AVG")
            {
                PrintInfo("指定的算法\"" + algorith + "\"没有实现。");
                return null;
            }
            string[] fileNames = GetStringArray("SelectedPrimaryFiles");
            if (fileNames == null || fileNames.Count() == 0)
            {
                PrintInfo("请选择参与平均值合成的数据。");
                return null;
            }
            foreach (string f in fileNames)
                if (!File.Exists(f))
                {
                    PrintInfo("所选择的数据:\"" + f + "\"不存在。");
                    return null;
                }

            Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            for (int i = 0; i < fileNames.Length; i++)
            {
                //为防止用户选择的波段超出数据波段的界限
                FilePrdMap map = new FilePrdMap(fileNames[i], 1, new VaildPra(float.MinValue, float.MaxValue), new int[] { 1 });
                if (map.BandCount < 1)
                {
                    PrintInfo("请选择正确的数据进行平均值合成。");
                    return null;
                }
                filePrdMap.Add("ndviFile" + i.ToString(), map);
            }

            ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            IVirtualRasterDataProvider prd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
            if (prd == null)
                throw new Exception("数据间无相交部分,无法创建虚拟数据提供者!");
            IRasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(new ArgumentProvider(prd, null));

            List<int> bands = new List<int>();
            for (int i = 0; i < fileNames.Length; i++)
            {
                bands.Add(filePrdMap["ndviFile" + i.ToString()].StartBand);
            }
            IPixelFeatureMapper<Int16> result = new MemPixelFeatureMapper<Int16>("0AVG", prd.Width * prd.Height, new Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.SpatialRef);
            visitor.VisitPixel(bands.ToArray(),
                (idx, values) =>
                {
                    result.Put(idx, (Int16)(values.Average(
                            (value) =>
                            {
                                return value;
                            }
                        )));
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
