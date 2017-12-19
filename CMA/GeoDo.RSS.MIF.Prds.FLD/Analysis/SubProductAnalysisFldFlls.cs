using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using GeoDo.RSS.MIF.Core;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.Core.DF;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    //泛滥历时面积
    public class SubProductAnalysisFldFlls : CmaMonitoringSubProduct
    {

        public SubProductAnalysisFldFlls(SubProductDef subProductDef)
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
            if (_argumentProvider == null || _argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "FLLS")
            {
                string[] files = GetStringArray("SelectedPrimaryFiles");
                if (files == null || files.Count() == 0)
                    return null;
                foreach (string file in files)
                {
                    if (!File.Exists(file))
                        return null;
                }
                IRasterOperator<Int16> roper = new RasterOperator<Int16>();
                IInterestedRaster<Int16> timeResult = null;
                DataIdentify di = GetDataIdentify();
                timeResult = roper.Times(files, CreatRasterIndetifyId(files, "FLD", "FLLS", di, null, null), (dstValue, srcValue) =>
                {
                    //泛滥水体值==4
                    if (srcValue == 4)
                        return ++dstValue;
                    else
                        return dstValue;
                });
                if (timeResult != null && !string.IsNullOrEmpty(timeResult.FileName) && File.Exists(timeResult.FileName))
                {
                    //发生[1,5)次
                    StatResultItem[] lowResults = null;
                    //发生[5,10)次
                    StatResultItem[] midResults = null;
                    //发生十次及十次以上
                    StatResultItem[] highResults = null;
                    IStatAnalysisEngine<Int16> exe = new StatAnalysisEngine<Int16>();
                    using (IRasterDataProvider prd = timeResult.HostDataProvider)
                    {
                        lowResults = exe.StatArea(prd, "省级行政区+土地利用类型", (srcValue) =>
                                    {
                                        if (srcValue >= 1 && srcValue < 5)
                                            return true;
                                        else
                                            return false;
                                    });
                        if (files.Count() >= 5)
                        {
                            midResults = exe.StatArea(prd, "省级行政区+土地利用类型", (srcValue) =>
                                        {
                                            if (srcValue >= 5 && srcValue < 10)
                                                return true;
                                            else
                                                return false;
                                        });
                        }
                        if (files.Count() >= 10)
                        {
                            highResults = exe.StatArea(prd, "省级行政区+土地利用类型", (srcValue) =>
                                        {
                                            if (srcValue >= 10)
                                                return true;
                                            else
                                                return false;
                                        });
                        }
                    }
                    List<StatResultItem> resultList = new List<StatResultItem>();
                    if (lowResults != null && lowResults.Count() > 0)
                    {
                        foreach (StatResultItem item in lowResults)
                            item.Name += "_1-5次";
                        resultList.AddRange(lowResults);
                    }
                    if (midResults != null && midResults.Count() > 0)
                    {
                        foreach (StatResultItem item in midResults)
                            item.Name += "_5-10次";
                        resultList.AddRange(midResults);
                    }
                    if (highResults != null && highResults.Count() > 0)
                    {
                        foreach (StatResultItem item in midResults)
                            item.Name += "_10次以上";
                        resultList.AddRange(highResults);
                    }
                    FileExtractResult fileResult = null;
                    if (resultList != null && resultList.Count() > 0)
                    {
                        string filename = StatResultToFile(files, resultList.ToArray(), "FLD", "FLLS", "水情泛滥历时面积", null);
                        fileResult = new FileExtractResult("FLLS", filename);
                    }
                    timeResult.Dispose();
                    File.Delete(timeResult.FileName);
                    return fileResult;
                }
                return null;
            }
            return null;
        }
    }
}
