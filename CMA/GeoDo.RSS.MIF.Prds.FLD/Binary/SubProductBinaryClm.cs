using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public class SubProductBinaryClm : CmaMonitoringSubProduct
    {

        public SubProductBinaryClm(SubProductDef subProductDef)
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
            IBandNameRaster bandNameRaster = _argumentProvider.DataProvider as IBandNameRaster;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "Cloud")
            {
                string express = "";
                int[] bandNos = null;
                bandNos = new int[3];
                bandNos[0] = TryGetBandNo(bandNameRaster, "Visible");
                bandNos[1] = TryGetBandNo(bandNameRaster, "NearInfrared");
                bandNos[2] = TryGetBandNo(bandNameRaster, "FarInfrared");
                double visibleZoom = (double)_argumentProvider.GetArg("NearInfrared_Zoom");
                double nearZoom = (double)_argumentProvider.GetArg("Visible_Zoom");
                double farZoom = (double)_argumentProvider.GetArg("FarInfrared_Zoom");
                string nearInfraredMin = _argumentProvider.GetArg("NearInfraredMin").ToString();
                string nearInfraredMax = _argumentProvider.GetArg("NearInfraredMax").ToString();
                string nearInfraredVisibleMin = _argumentProvider.GetArg("NearInfraredVisibleMin").ToString();
                string nearInfraredVisibleMax = _argumentProvider.GetArg("NearInfraredVisibleMax").ToString();
                string farInfraredMin = _argumentProvider.GetArg("FarInfraredMin").ToString();
                string farInfraredMax = _argumentProvider.GetArg("FarInfraredMax").ToString();
                express = string.Format(@"(band{0}/{3}>{6}*10)&&(band{0}/{3}<{7}*10)&&(((float)band{1}/{4})/(band{0}/{3})>{8})&&(((float)band{1}/{4})/(band{0}/{3})<{9})&&(band{2}/{5}>{10}*10)&&(band{2}/{5}<{11}*10)", bandNos[0], bandNos[1], bandNos[2], visibleZoom, nearZoom, farZoom, nearInfraredMin, nearInfraredMax, nearInfraredVisibleMin, nearInfraredVisibleMax, farInfraredMin, farInfraredMax);
                IThresholdExtracter<UInt16> extracter = new SimpleThresholdExtracter<UInt16>();
                extracter.Reset(_argumentProvider, bandNos, express);
                IRasterDataProvider prd = _argumentProvider.DataProvider;
                IPixelIndexMapper resultFLD = PixelIndexMapperFactory.CreatePixelIndexMapper("FLD", prd.Width, prd.Height, prd.CoordEnvelope, prd.SpatialRef);
                extracter.Extract(resultFLD);
                return resultFLD;
            }
            else
            {
                return null;
            }
        }
    }
}
