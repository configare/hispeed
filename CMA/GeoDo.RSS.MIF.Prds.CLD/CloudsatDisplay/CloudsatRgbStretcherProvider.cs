using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.RasterDrawing;
using GeoDo.RSS.UI.WinForm;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class CloudsatRgbStretcherProvider : RgbStretcherProvider, IRgbStretcherProvider
    {
        public new object GetStretcher(string fname, string colorTableName, out ColorMapTable<int> colorMapTable)
        {
            colorMapTable = null;
            ProductColorTable pct = null;
            if (string.IsNullOrWhiteSpace(colorTableName))
                colorTableName = "Cloudsat.2B-GEOPROF.Radar_Reflectivity";
            pct = ProductColorTableFactory.GetColorTable(colorTableName);
            switch (colorTableName)
            {
                case "Cloudsat.2B-GEOPROF.Radar_Reflectivity":
                    return base.GetStretcher(RSS.Core.DF.enumDataType.Int16, pct, out colorMapTable);
                default:
                    return base.GetStretcher(RSS.Core.DF.enumDataType.Int16, pct, out colorMapTable);
            }
        }
    }
}
