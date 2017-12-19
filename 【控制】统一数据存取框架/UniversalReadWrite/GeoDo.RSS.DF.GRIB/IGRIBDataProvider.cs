#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/25 8:51:55
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DF.GRIB
{
    public interface IGRIBDataProvider : IGeoDataProvider
    {
        GRIB_Definition Definition { get; }
        GRIB_Point[] Read();
        GRIB_Point[] Read(CoordEnvelope geoEnvelope);
        void Read(IntPtr buffer);
        void Read(IntPtr buffer, CoordEnvelope geoEnvelope);
        void StatMinMax(GRIB_Point[] pts, out GRIB_Point minPoint, out GRIB_Point maxPoint);
        IArrayRasterDataProvider ToArrayDataProvider(GRIB_Point[] points);
    }
}
