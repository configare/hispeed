using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FOG
{
    /// <summary>
    /// 光学厚度
    /// </summary>
    public class SubProductOPTDFOG : CmaMonitoringSubProduct
    {

        public SubProductOPTDFOG(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;

            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "OPTDAlgorithm")
            {
                return OPTDAlgorithm();
            }
            return null;
        }

        private IExtractResult OPTDAlgorithm()
        {
            int visibleCH = (int)_argumentProvider.GetArg("Visible");
            double OptdZoom = (float)_argumentProvider.GetArg("OptdZoom");
            double visibleZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
            float extinctionCoefficient = (float)_argumentProvider.GetArg("ExtinctionCoefficient");

            if (_argumentProvider.GetArg("CSRFile") == null || _argumentProvider.GetArg("DBLVFile") == null || _argumentProvider.GetArg("CurrentRasterFile") == null)
                return null;
            string csrFile = _argumentProvider.GetArg("CSRFile").ToString();
            string dvlbFile = _argumentProvider.GetArg("DBLVFile").ToString();
            string CurrentRasterFile = _argumentProvider.GetArg("CurrentRasterFile").ToString();
            using (IRasterDataProvider rasterProvider = RasterDataDriver.Open(CurrentRasterFile) as IRasterDataProvider)
            {
                if (rasterProvider != null)
                {
                    IBandNameRaster bandNameRaster = rasterProvider as IBandNameRaster;
                    visibleCH = TryGetBandNo(bandNameRaster, "Visible");
                }
            }

            string errorStr = null;
            if (visibleCH == -1 || !File.Exists(csrFile) || !File.Exists(CurrentRasterFile) || !File.Exists(dvlbFile))
            {
                errorStr = "光学厚度生产所用通道未设置完全,请检查!";
                return null;
            }

            IVirtualRasterDataProvider vrd = null;
            Dictionary<string, FilePrdMap> filePrdMap = new Dictionary<string, FilePrdMap>();
            filePrdMap.Add("CurrentRasterFile", new FilePrdMap(CurrentRasterFile, 1, new VaildPra(UInt16.MinValue, UInt16.MaxValue), new int[] { visibleCH }));
            filePrdMap.Add("csrFile", new FilePrdMap(csrFile, 1, new VaildPra(UInt16.MinValue, UInt16.MaxValue), new int[] { 1 }));
            filePrdMap.Add("dvlbFile", new FilePrdMap(dvlbFile, 1, new VaildPra(UInt16.MinValue, UInt16.MaxValue), new int[] { 1 }));

            ITryCreateVirtualPrd tryVPrd = new TryCreateVirtualPrdByMultiFile();
            vrd = tryVPrd.CreateVirtualRasterPRD(ref filePrdMap);
            if (vrd == null)
            {
                if (filePrdMap != null && filePrdMap.Count() > 0)
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
                return CalcOPTDByOneFile(filePrdMap, vrd, extinctionCoefficient, visibleCH, visibleZoom, OptdZoom);
            }
            finally
            {
                if (filePrdMap != null && filePrdMap.Count() > 0)
                {
                    foreach (FilePrdMap value in filePrdMap.Values)
                    {
                        if (value.Prd != null)
                            value.Prd.Dispose();
                    }
                }
                if (vrd != null)
                    vrd.Dispose();
            }
        }

        private IExtractResult CalcOPTDByOneFile(Dictionary<string, FilePrdMap> filePrdMap, IVirtualRasterDataProvider vrd, float extinctionCoefficient, int visibleCH, double visibleZoom, double OptdZoom)
        {
            try
            {
                ArgumentProvider ap = new ArgumentProvider(vrd, null);
                RasterPixelsVisitor<float> rpVisitor = new RasterPixelsVisitor<float>(ap);
                IPixelFeatureMapper<Int16>  curOPTD = new MemPixelFeatureMapper<Int16>("OPTD", 1000, new Size(vrd.Width, vrd.Height), vrd.CoordEnvelope, vrd.SpatialRef);
                float upRef = 0f;
                rpVisitor.VisitPixel(new int[] { filePrdMap["CurrentRasterFile"].StartBand,
                                                 filePrdMap["csrFile"].StartBand,
                                                 filePrdMap["dvlbFile"].StartBand},
                    (index, values) =>
                    {
                        if (values[2] == 1f)
                        {
                            upRef = values[0] - Convert.ToSingle(values[1] * Math.Pow((1 - (values[0] / visibleZoom)), 2));
                            curOPTD.Put(index, (Int16)(upRef * (1 / extinctionCoefficient) / ((visibleZoom - upRef) * 3) * OptdZoom));
                        }
                        else
                            curOPTD.Put(index, 0);
                    });
                return curOPTD;
            }
            finally
            {
                vrd.Dispose();
            }
        }

        private RasterIdentify CreatRasterIndetifyId(string subProductindentify, RasterIdentify baseId)
        {
            RasterIdentify id = new RasterIdentify();
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FOG";
            id.SubProductIdentify = subProductindentify;
            id.Satellite = _argumentProvider.DataProvider.DataIdentify.Satellite;
            id.Sensor = _argumentProvider.DataProvider.DataIdentify.Sensor;
            id.Resolution = _argumentProvider.DataProvider.ResolutionX * 100000 + "M";
            id.OrbitDateTime = baseId == null ? DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0)) : baseId.OrbitDateTime;
            id.GenerateDateTime = baseId == null ? DateTime.Now : baseId.GenerateDateTime;
            return id;
        }

        private float GetNDVI(UInt16[] values, int shortInfraredCH, int visibleCH)
        {
            return (values[shortInfraredCH] + values[visibleCH]) == 0 ? 0f : (float)(values[shortInfraredCH] - values[visibleCH]) / (values[shortInfraredCH] + values[visibleCH]);
        }
    }
}
