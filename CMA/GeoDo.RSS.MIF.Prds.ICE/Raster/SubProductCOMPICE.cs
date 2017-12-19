using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    public class SubProductCOMPICE : CmaMonitoringSubProduct
    {
        public SubProductCOMPICE(SubProductDef subProductDef)
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
            string compareFile = _argumentProvider.GetArg("CompareFile").ToString();
            string file = _argumentProvider.GetArg("MainFile").ToString();
            if (file == null&&string.IsNullOrEmpty(file))
                return null;
            string outFileIdentify = GetStringArgument("OutFileIdentify");
            string productIdentify = _subProductDef.ProductDef.Identify;
            IPixelFeatureMapper<Int16> rasterResult = MakeCompareRaster<float, Int16>(productIdentify, compareFile, file, (fstFileValue, sedFileValue) =>
                {
                    if (fstFileValue > 0f && sedFileValue == 1f)
                        return 1;
                    else if (fstFileValue == 0f && sedFileValue == 1f)
                        return 4;
                    else if (fstFileValue > 0f && sedFileValue == 0f)
                        return 5;
                    else return 0;
                },false);
            if (rasterResult == null)
                return null;
            else
            {
                RasterIdentify rid = GetRasterIdentifyID(new RasterIdentify(new string[] { compareFile, file }));
                IInterestedRaster<Int16> iir = new InterestedRaster<Int16>(rid, rasterResult.Size, rasterResult.CoordEnvelope, null);
                iir.Put(rasterResult);
                iir.Dispose();
                FileExtractResult result = new FileExtractResult(outFileIdentify, iir.FileName);
                result.SetDispaly(false);
                return result;
            }
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
