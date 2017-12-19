using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FOG
{
    /// <summary>
    /// 水路径反演
    /// </summary>
    public class SubProductLWPFOG : CmaMonitoringSubProduct
    {

        public SubProductLWPFOG(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;

            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "0LWPAlgorithm")
            {
                return LWPAlgorithm();
            }
            return null;
        }

        private IExtractResult LWPAlgorithm()
        {
            int optdCH = (int)_argumentProvider.GetArg("OPTDBandNo");
            double OptdZoom = (float)_argumentProvider.GetArg("OPTDZoom");
            double LwpZoom = (float)_argumentProvider.GetArg("LWPZoom");
            double lwpA = (double)_argumentProvider.GetArg("LWPA");
            double lwpB = (double)_argumentProvider.GetArg("LWPB");
            string errorStr = null;
            string optdFile = _argumentProvider.GetArg("mainfiles").ToString();
            if (optdCH == -1 || !File.Exists(optdFile))
            {
                errorStr = "水路径反演生产所用通道未设置完全,请检查!";
                return null;
            }
            using (IRasterDataProvider optdPrd = GeoDataDriver.Open(optdFile) as IRasterDataProvider)
            {
                return CalcOPTDByOneFile(optdPrd, lwpA, lwpB, optdCH, OptdZoom, LwpZoom);
            }
        }

        private IExtractResult CalcOPTDByOneFile(IRasterDataProvider prd, double lwpa, double lwpb, int optdCH, double OptdZoom, double lwpZoom)
        {
            ArgumentProvider ap = new ArgumentProvider(prd, null);
            RasterPixelsVisitor<Int16> rpVisitor = new RasterPixelsVisitor<Int16>(ap);
            IPixelFeatureMapper<Int16>  curLWP = new MemPixelFeatureMapper<Int16>("0LWP", 1000, new Size(prd.Width, prd.Height), prd.CoordEnvelope, prd.SpatialRef);
            rpVisitor.VisitPixel(new int[] { optdCH },
                (index, values) =>
                {
                    curLWP.Put(index, (Int16)Convert.ToSingle(Math.Pow(10, Math.Exp((Math.Log10(values[0] / OptdZoom - lwpa) / lwpb))) * lwpZoom));
                });
            return curLWP;
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
    }
}
