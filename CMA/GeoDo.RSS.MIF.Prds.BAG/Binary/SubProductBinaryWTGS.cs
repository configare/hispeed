using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public class SubProductBinaryWTGS : CmaMonitoringSubProduct
    {
        public SubProductBinaryWTGS(SubProductDef productDef)
            :base(productDef)
        {
            _isBinary = true;
            _identify = productDef.Identify;
            _algorithmDefs = new List<AlgorithmDef>(productDef.Algorithms);
        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            if (_argumentProvider == null||_argumentProvider.DataProvider==null)
                return null;
            string algname = _argumentProvider.GetArg("AlgorithmName").ToString();
            if (string.IsNullOrEmpty(algname))
                return null;
            if (algname == "AquaticExtract")
            {
                int visiBandNo = (int)_argumentProvider.GetArg("Visible");
                int sIBandNo = (int)_argumentProvider.GetArg("ShortInfrared");
                int fIBandNo = (int)_argumentProvider.GetArg("FarInfrared");
                double visiBandZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
                double siBandZoom = (double)_argumentProvider.GetArg("ShortInfrared_Zoom");
                double fiBandZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");
                if (visiBandNo <= 0 || sIBandNo <= 0 || fIBandNo <= 0 || visiBandZoom == 0 || siBandZoom == 0 || fiBandZoom == 0)
                    return null;
                string express = string.Format(@"((band{2}/{5}f-band{0}/{3}f)==0 || {3}==0||{4}==0||{5}==0)? 
                                 false :((float)(band{1}/{4}f-band{0}/{3}f)/(band{2}/{5}f-band{0}/{3}f)< var_AquaExtractMax 
                                 && (float)(band{1}/{4}f-band{0}/{3}f)/(band{2}/{5}f-band{0}/{3}f)>var_AquaExtractMin)",
                                 visiBandNo, sIBandNo, fIBandNo, visiBandZoom, siBandZoom, fiBandZoom);
                int[] bandNos = new int[] { visiBandNo, sIBandNo , fIBandNo};
                IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();
                extracter.Reset(_argumentProvider, bandNos, express);
                int width = _argumentProvider.DataProvider.Width;
                int height = _argumentProvider.DataProvider.Height;
                IPixelIndexMapper memResult = PixelIndexMapperFactory.CreatePixelIndexMapper("BAG", width,height,_argumentProvider.DataProvider.CoordEnvelope,_argumentProvider.DataProvider.SpatialRef);
                extracter.Extract(memResult);
                return memResult;
            }
            return null;
        }

        private IFileExtractResult SaveResultToFile(IRasterDataProvider dataProvider, IPixelIndexMapper result)
        {
            RasterIdentify id = new RasterIdentify();
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "BAG";
            id.SubProductIdentify = "WTGS";
            id.Sensor = dataProvider.DataIdentify.Sensor;
            id.Satellite = dataProvider.DataIdentify.Satellite;
            id.OrbitDateTime = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));
            id.GenerateDateTime = DateTime.Now;
            IInterestedRaster<UInt16> iir = new InterestedRaster<UInt16>(id, new Size(dataProvider.Width, dataProvider.Height), dataProvider.CoordEnvelope.Clone());
            int[] idxs = result.Indexes.ToArray();
            iir.Put(idxs, 1);
            iir.Dispose();
            return new FileExtractResult("WTGS", iir.FileName);
        }
    }
}
