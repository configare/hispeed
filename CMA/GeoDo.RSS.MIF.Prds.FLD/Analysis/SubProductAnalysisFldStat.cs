using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    //判识水体面积统计
    public class SubProductAnalysisFldStat : CmaMonitoringSubProduct
    {

        public SubProductAnalysisFldStat(SubProductDef subProductDef)
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
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() != "STATAlgorithm")
                return null;
            //按照Instance执行统计操作
            string instanceIdentify = _argumentProvider.GetArg("OutFileIdentify") as string;
            if (instanceIdentify != null)
            {
                SubProductInstanceDef instance = FindSubProductInstanceDefs(instanceIdentify);
                if (instance.OutFileIdentify == "0CCC" || instance.OutFileIdentify == "RLUT")     //省市县面积统计
                    return StatRaster<short>(instance, (v) => { return v == 1; }, progressTracker);
            }
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "STATAlgorithm")
            {
                return STATAlgorithm();
            }
            return null;
        }

        private IExtractResult STATAlgorithm()
        {
            string[] files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            string mixFile = files[0].Replace("_DBLV_", "_0MIX_").Replace("_FLOD_", "_0MIX_");
            Dictionary<int, Int16> mixDic = new Dictionary<int, short>();
            if (File.Exists(mixFile))
            {
                using (IRasterDataProvider rd = GeoDataDriver.Open(mixFile) as IRasterDataProvider)
                {
                    ArgumentProvider ap = new ArgumentProvider(rd, null);
                    RasterPixelsVisitor<Int16> rpVisitor = new RasterPixelsVisitor<Int16>(ap);
                    rpVisitor.VisitPixel(new int[] { 1 },
                        (idx, values) =>
                        {
                            if (values[0] != 0)
                                mixDic.Add(idx, values[0]);
                        });
                }
            }
            if (mixDic.Count == 0)
            {
                return AreaStatResult<Int16>("水情", "FLD", (v) => { return v == 1; });
            }
            else
                return AreaStatResult<Int16>("水情", "FLD", (v, idx) =>
                {
                    if (v == 1)
                    {
                        if (mixDic.ContainsKey(idx))
                            return mixDic[idx];
                        else
                            return 100;
                    }
                    else
                        return 0;
                }, 100);
        }
    }
}
