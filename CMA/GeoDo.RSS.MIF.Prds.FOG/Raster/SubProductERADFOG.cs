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
    /// 雾滴尺度产品
    /// </summary>
    public class SubProductERADFOG : CmaMonitoringSubProduct
    {

        public SubProductERADFOG(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;

            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "ERADAlgorithm")
            {
                return ERADAlgorithm();
            }
            return null;
        }

        private IExtractResult ERADAlgorithm()
        {
            int optdCH = (int)_argumentProvider.GetArg("OPTDBandNo");
            double OptdZoom = (float)_argumentProvider.GetArg("OPTDZoom");
            int lwpCH = (int)_argumentProvider.GetArg("LWPBandNo");
            double LwpZoom = (float)_argumentProvider.GetArg("LWPZoom");

            if (_argumentProvider.GetArg("mainfiles") == null || _argumentProvider.GetArg("LWPFile") == null)
                return null;
            string optdFile = _argumentProvider.GetArg("mainfiles").ToString();
            string lwpFile = _argumentProvider.GetArg("LWPFile").ToString();
            if (optdCH == -1 || lwpCH == -1 || !File.Exists(optdFile) || !File.Exists(lwpFile))
            {
                //errorStr = "雾滴尺度产品生产所用通道未设置完全,请检查!";
                return null;
            }

            IRasterDataProvider optdPrd = null;
            IRasterDataProvider lwpPrd = null;
            IVirtualRasterDataProvider vrd = null;
            try
            {
                optdPrd = GeoDataDriver.Open(optdFile) as IRasterDataProvider;
                lwpPrd = GeoDataDriver.Open(lwpFile) as IRasterDataProvider;
                vrd = new VirtualRasterDataProvider(new IRasterDataProvider[] { optdPrd, lwpPrd });
                return CalcOPTDByOneFile(vrd, optdPrd, optdCH, OptdZoom, lwpCH, LwpZoom);
            }
            finally
            {
                optdPrd.Dispose();
                lwpPrd.Dispose();
                vrd.Dispose();
            }
        }

        private IExtractResult CalcOPTDByOneFile(IVirtualRasterDataProvider vrd, IRasterDataProvider optdPrd, int optdCH, double OptdZoom, int lwpCH, double lwpZoom)
        {
            ArgumentProvider ap = new ArgumentProvider(vrd, null);
            RasterPixelsVisitor<Int16> rpVisitor = new RasterPixelsVisitor<Int16>(ap);
            IPixelFeatureMapper<Int16>  curERAD = new MemPixelFeatureMapper<Int16>("ERAD", 1000, new Size(vrd.Width, vrd.Height), vrd.CoordEnvelope, vrd.SpatialRef);
            rpVisitor.VisitPixel(new int[] { optdCH, optdPrd.BandCount + lwpCH },
                (index, values) =>
                {
                    curERAD.Put(index, (Int16)(3 / 2 * ((values[1] / lwpZoom) / 1 * (values[0] / OptdZoom))));
                });
            return curERAD;
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
