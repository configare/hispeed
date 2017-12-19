using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Prds.Comm;
using System.IO;
using System.Xml.Linq;
using System.Drawing;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public class SubProductSTATBAG : CmaMonitoringSubProduct
    {

        public SubProductSTATBAG(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            string[]  files = GetStringArray("SelectedPrimaryFiles");
            if (files == null || files.Length == 0)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "STATAlgorithm")
            {
                return STATAlgorithm(files);
            }
            return null;
        }

        private IExtractResult STATAlgorithm(string[] files)
        {
            string outFileIdentify = _argumentProvider.GetArg("OutFileIdentify").ToString();
            if (outFileIdentify == "CCCA")
            {
                return base.AreaStatResult<Int16>("蓝藻", "BAG", (v) => { return v==1;});
            }
            Dictionary<string, string> templates = BAGStatisticHelper.GetAOITemplateList();
            if (templates == null || templates.Count == 0)
            {
                return AreaStatResult<Int16>("蓝藻", "BAG", (v) => { return v == 1; });
            }
            else
            {
                Dictionary<string,int[]> aoiValues=new Dictionary<string,int[]>();
                Size size;
                CoordEnvelope envelope;
                GetArgument(out size,out envelope,files);
                foreach (string templateName in templates.Keys)
                {
                    //分湖区
                    Dictionary<string, int[]> aois = VectorTemplateToAOIConvertor.GetFeatureAOIIndex(templates[templateName], "NAME", envelope, size);
                    //整个湖区
                    if (aois != null && aois.Count != 0)
                    {
                        foreach (string key in aois.Keys)
                        {
                            aoiValues.Add(key, aois[key]);
                        }
                    }
                }
                if (aoiValues.Count > 0)
                {
                    _argumentProvider.SetArg("AOI", aoiValues);
                    return AreaStatResult((v) => { return v == 1; }, files);
                }
                else
                    return AreaStatResult<Int16>("蓝藻", "BAG", (v) => { return v == 1; });
            }
        }

        private void GetArgument(out Size size,out CoordEnvelope envelope,string[] files)
        {
            float maxResultion = 0f;
            IRasterDataProvider prd = null;
            envelope = new CoordEnvelope(double.MaxValue, double.MinValue, double.MaxValue, double.MinValue);
            foreach (string file in files)
            {
                try
                {
                    prd = GeoDataDriver.Open(file) as IRasterDataProvider;
                    maxResultion = Math.Max(maxResultion, prd.ResolutionX);
                    envelope = envelope.Union(new CoordEnvelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinX + prd.ResolutionX * prd.Width,
                                              prd.CoordEnvelope.MinY, prd.CoordEnvelope.MinY + prd.ResolutionY * prd.Height));
                }
                catch
                {
                    throw new ArgumentException("选择的文件：“" + Path.GetFileName(file) + "”无法进行面积统计。");
                }
                finally
                {
                    if (prd != null)
                    {
                        prd.Dispose();
                        prd = null;
                    }
                }
            }
            size = new Size((int)(envelope.Width / maxResultion), (int)(envelope.Height / maxResultion));
        }

        public IExtractResult AreaStatResult(Func<Int16,bool> filter,string[] files)
        {
            object aioObj = _argumentProvider.GetArg("AOI");
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string extInfos = GetStringArgument("extinfo");
            string title = "蓝藻当前区域面积统计";
            StatResultItem[] areaResult = AreaStat(aioObj, filter,files);
            if (areaResult == null)
                return null;
            IStatResult results = StatResultItemToIStatResult.ItemsToResults(areaResult, files);
            string fileName = StatResultToFile(files, results, "BAG", outFileIdentify, title, extInfos, 1,false);
            //StatResultToFile(files, areaResult, productIdentify, outFileIdentify, title, extInfos, 1);
            return new FileExtractResult(outFileIdentify, fileName);
        }

        private StatResultItem[] AreaStat(object aoiObj, Func<Int16, bool> filter,string[] files)
        {
            if (files == null || files.Length == 0)
                return null;
            IStatAnalysisEngine<Int16> exe = new StatAnalysisEngine<Int16>();
            if (aoiObj as Dictionary<string, int[]> != null)
            {
                Dictionary<string, int[]> aoi = aoiObj as Dictionary<string, int[]>;
                List<StatResultItem> items = new List<StatResultItem>();
                IRasterDataProvider prd = null;
                try
                {
                    prd = GeoDataDriver.Open(files[0]) as IRasterDataProvider;
                    if (aoi == null || aoi.Count == 0)
                        return null;
                    StatResultItem[] item = exe.StatAreaCustom(prd, aoi, filter);
                    if (item != null && item.Length > 0)
                        items.AddRange(item);
                    //
                    RasterOperator<Int16> oper = new RasterOperator<Int16>();
                    int count = oper.Count(prd, null, filter);
                    StatResultItem sri = new StatResultItem();
                    sri.Name = "整个湖区";
                    sri.Value = Math.Round(count * BAGStatisticHelper.CalPixelArea(prd.ResolutionX), 3);
                    items.Add(sri);
                    return items.ToArray();
                }
                catch
                {
                    throw new ArgumentException("选择的文件：“" + Path.GetFileName(files[0]) + "”无法进行自定义面积统计。");
                }
                finally
                {
                    if (prd != null)
                    {
                        prd.Dispose();
                        prd = null;
                    }
                }
            }
            else
                return null;
        }
    }
}
