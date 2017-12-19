using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.Comm;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.SNW
{
    public class SubProductCOMPSNW : CmaMonitoringSubProduct
    {

        public SubProductCOMPSNW(SubProductDef subProductDef)
            : base(subProductDef)
        {
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "COMP")
            {
                return CompareAlgorithm();
            }
            return null;
        }

        private IExtractResult CompareAlgorithm()
        {
            string[] files = GetStringArray("SelectedPrimaryFiles");
            string file = _argumentProvider.GetArg("SelectedPrimaryFiles").ToString();
            if (files == null&&string.IsNullOrEmpty(file))
                return null;
            if (files != null && files.Length < 2)
                return null;
            if (files==null&&!string.IsNullOrEmpty(file))
            {
                string[] fnames = file.Split('*');
                if (fnames != null && fnames.Length > 1)
                {
                    foreach (string item in fnames)
                        if (!File.Exists(item))
                            return null;
                    files = fnames;
                }
                else
                    return null;
            }
            //文件列表排序
            string[] dstFiles = SortFileName(files);
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string productIdentify = _subProductDef.ProductDef.Identify;
            ExtractResultArray results = new ExtractResultArray(productIdentify + _identify);
            List<IPixelFeatureMapper<Int16>> rasterList = new List<IPixelFeatureMapper<short>>();
            List<RasterIdentify> rstIdentifys = new List<RasterIdentify>();
            for (int i = 0; i < dstFiles.Length - 1; i++)
            {
                IPixelFeatureMapper<Int16> rasterResult = MakeCompareRaster<float, Int16>(productIdentify, dstFiles[i], dstFiles[i + 1], (fstFileValue, sedFileValue) =>
                {
                    if (fstFileValue == 1f && sedFileValue == 1f)
                        return 1;
                    else if (fstFileValue == 0f && sedFileValue == 1f)
                        return 4;
                    else if (fstFileValue == 1f && sedFileValue == 0f)
                        return 5;
                    else return 0;
                },true);
                if (rasterResult == null)
                    continue;
                rasterList.Add(rasterResult);
                rstIdentifys.Add(new RasterIdentify(new string[]{dstFiles[i],dstFiles[i+1]}));
            }
            if (rasterList.Count == 0)
                return null;
            else
            {
                for (int i = 0; i < rasterList.Count;i++ )
                {
                    RasterIdentify rid = GetRasterIdentifyID(rstIdentifys[i]);
                    IInterestedRaster<Int16> iir = new InterestedRaster<Int16>(rid, rasterList[i].Size, rasterList[i].CoordEnvelope, null);
                    iir.Put(rasterList[i]);
                    iir.Dispose();
                    FileExtractResult res = new FileExtractResult(outFileIdentify, iir.FileName);
                    res.SetDispaly(false);
                    results.Add(res);
                }
            }
            return results;
        }

        private RasterIdentify GetRasterIdentifyID(RasterIdentify rasterId)
        {
            RasterIdentify rst = rasterId;
            //if(rasterId.ObritTiems.Length>1)
            //   rst.OrbitDateTime = rasterId.ObritTiems.Last();
            rst.ProductIdentify = _subProductDef.ProductDef.Identify;
            rst.SubProductIdentify = _subProductDef.Identify;
            GetOutFileIdentify(ref rst);
            rst.IsOutput2WorkspaceDir = true;
            return rst;
        }

        private void GetOutFileIdentify(ref RasterIdentify rst)
        {
            object obj = _argumentProvider.GetArg("OutFileIdentify");
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
                rst.SubProductIdentify = obj.ToString();
        }
    }
}
