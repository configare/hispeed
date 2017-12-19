using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.RasterTools
{
    public interface IAOIRasterFetcher<T>
    {
        void Fetch(IRasterDataProvider raster, int[] aoi, int[] bandNos, out T[][] banbValues);
    }
}
