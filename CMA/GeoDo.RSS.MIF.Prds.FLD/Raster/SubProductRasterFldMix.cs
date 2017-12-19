using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public class SubProductRasterFldMix : CmaMonitoringSubProduct
    {

        //混合象元计算
        public SubProductRasterFldMix(SubProductDef subProductDef)
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
            if (_argumentProvider == null || _argumentProvider.DataProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.DataProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "0MIX")
            {
                float fldValue = 0;
                float landValue = 0;
                string[] nearInfValues = _argumentProvider.GetArg("NearInfraredValues") as string[];
                if (nearInfValues == null || nearInfValues.Count() != 2)
                    return null;
                if (!float.TryParse(nearInfValues[0], out fldValue) || !float.TryParse(nearInfValues[1], out landValue))
                    return null;
                if (fldValue == landValue)
                    return null;
                IBandNameRaster bandNameRaster  = _argumentProvider.DataProvider as IBandNameRaster;
                int bandNo = TryGetBandNo(bandNameRaster, "NearInfrared");
                double nearInfraredZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
                //
                string[] files = GetStringArray("SelectedPrimaryFiles");
                if (files == null || files.Length > 1)
                    return null;
                string dblvFile = files[0];
                //
                IRasterDataProvider prd = _argumentProvider.DataProvider;
                List<int> dblvAOI = new List<int>();
                string cloudFile = dblvFile.Replace("_DBLV_", "_0CLM_");
                GetDBLVAOIIndex(dblvFile, dblvAOI);
                if (File.Exists(cloudFile))
                    GetDBLVAOIIndex(dblvFile, cloudFile, dblvAOI);
                double resultZoom = 100;
                return CalcMix(dblvFile, _argumentProvider.DataProvider.fileName, nearInfraredZoom, bandNo, 100, dblvAOI.ToArray(), fldValue, landValue, resultZoom);
            }
            return null;
        }

        private void GetDBLVAOIIndex(string dblvFile, List<int> dblvAOI)
        {
            using (IRasterDataProvider provider = GeoDataDriver.Open(dblvFile) as IRasterDataProvider)
            {
                ArgumentProvider ap = new ArgumentProvider(provider, null);
                using (RasterPixelsVisitor<Int16> visitor = new RasterPixelsVisitor<Int16>(ap))
                {
                    visitor.VisitPixel(new int[] { 1 },
                        (index, values) =>
                        {
                            if (values[0] == 1)
                                dblvAOI.Add(index);
                        });
                }
            }
        }

        private void GetDBLVAOIIndex(string dblvFile, string cloudFile, List<int> dblvAOI)
        {
            Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            filePrdMap.Add("dblvFile", new FilePrdMap(dblvFile, 1, new VaildPra(float.MinValue, float.MaxValue), new int[] { 1 }));
            filePrdMap.Add("cloudFile", new FilePrdMap(cloudFile, 1, new VaildPra(float.MinValue, float.MaxValue), new int[] { 1 }));
            ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            IVirtualRasterDataProvider vrd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
            int minWnd = 3, maxWnd = 3;
            if (vrd == null)
            {
                if (filePrdMap != null && filePrdMap.Count > 0)
                {
                    foreach (FilePrdMap value in filePrdMap.Values)
                    {
                        if (value.Prd != null)
                            value.Prd.Dispose();
                    }
                }
                return;
            }
            try
            {
                List<int> temp = new List<int>();
                temp.AddRange(dblvAOI);
                dblvAOI.Clear();
                ArgumentProvider ap = new ArgumentProvider(vrd, null);
                Size size = new Size(vrd.Width, vrd.Height);
                int dblvBand = filePrdMap["dblvFile"].StartBand;
                int cloudBand = filePrdMap["cloudFile"].StartBand;
                int[] tempAOI = temp.ToArray();
                Rectangle aoiRect = AOIHelper.ComputeAOIRect(tempAOI, size);
                int[] wndBandNos = new int[] { dblvBand, cloudBand };
                bool isCloudEgde = false;
                using (IRasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(ap))
                {
                    visitor.VisitPixelWnd(aoiRect, tempAOI, wndBandNos, wndBandNos, minWnd, maxWnd,
                              (pixelIdx, crtWndSize, bandValues, wndValues) =>
                              {
                                  return false;
                              },
                             (pixelIdx, crtWndSize, bandValues, wndValues) =>
                             {
                                 isCloudEgde = IsCloudEdgeMethod(bandValues, wndValues, 1);
                                 if (isCloudEgde)
                                     temp.Remove(pixelIdx);
                             });
                }
                dblvAOI.AddRange(temp);
                temp = null;
            }
            finally
            {
                if (vrd != null)
                    vrd.Dispose();
            }
        }

        private IExtractResult CalcMix(string dblvFile, string ldfFile, double bandZoom, int bandNo, double resultZoom, int[] dblvAOI, float waterNir, float LandNir, double zoom)
        {
            Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            filePrdMap.Add("dblvFile", new FilePrdMap(dblvFile, 1, new VaildPra(float.MinValue, float.MaxValue), new int[] { 1 }));
            filePrdMap.Add("ldfFile", new FilePrdMap(ldfFile, bandZoom, new VaildPra(float.MinValue, float.MaxValue), new int[] { bandNo }));
            ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            IVirtualRasterDataProvider vrd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
            int minWnd = 3, maxWnd = 3;
            if (vrd == null)
            {
                if (filePrdMap != null && filePrdMap.Count > 0)
                {
                    foreach (FilePrdMap value in filePrdMap.Values)
                    {
                        if (value.Prd != null)
                            value.Prd.Dispose();
                    }
                }
                return null;
            }
            try
            {
                ArgumentProvider ap = new ArgumentProvider(vrd, null);
                RasterPixelsVisitor<float> rpVisitor = new RasterPixelsVisitor<float>(ap);
                Size size = new Size(vrd.Width, vrd.Height);
                MemPixelFeatureMapper<Int16>  result = new MemPixelFeatureMapper<Int16>("0MIX", 1000, size, vrd.CoordEnvelope, vrd.SpatialRef);
                int dblvBand = filePrdMap["dblvFile"].StartBand;
                int ldfBand = filePrdMap["ldfFile"].StartBand;
                int[] wndBandNos = new int[] { dblvBand, ldfBand };
                Rectangle aoiRect = AOIHelper.ComputeAOIRect(dblvAOI, size);
                bool isEdge = false;
                using (IRasterPixelsVisitor<float> visitor = new RasterPixelsVisitor<float>(ap))
                {
                    visitor.VisitPixelWnd(aoiRect, dblvAOI, wndBandNos, wndBandNos, minWnd, maxWnd,
                              (pixelIdx, crtWndSize, bandValues, wndValues) =>
                              {
                                  return false;
                              },
                             (pixelIdx, crtWndSize, bandValues, wndValues) =>
                             {
                                 isEdge = IsEdgeMethod(bandValues, wndValues, 0);
                                 if (isEdge)
                                     result.Put(pixelIdx, GetResult(bandValues, wndValues, waterNir, LandNir, zoom));
                             });
                }
                return result;
            }
            finally
            {
                if (vrd != null)
                    vrd.Dispose();
                if (filePrdMap != null && filePrdMap.Count > 0)
                {
                    foreach (FilePrdMap value in filePrdMap.Values)
                    {
                        if (value.Prd != null)
                            value.Prd.Dispose();
                    }
                }
            }
        }

        private Int16 GetResult(float[] bandValues, float[][] wndValues, float waterNir, float landNir, double zoom)
        {
            return (Int16)((1 - (bandValues[1] - waterNir) / (landNir - waterNir)) * zoom);
        }

        private bool IsEdgeMethod(float[] bandValues, float[][] wndValues, int band)
        {
            int length = wndValues[0].Length;
            for (int i = 0; i < length; i++)
            {
                if (wndValues[band][i] != 1)
                    return true;
            }
            return false;
        }

        private bool IsCloudEdgeMethod(float[] bandValues, float[][] wndValues, int band)
        {
            int length = wndValues[0].Length;
            for (int i = 0; i < length; i++)
            {
                if (wndValues[band][i] == 1)
                    return true;
            }
            return false;
        }
    }
}
