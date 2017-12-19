using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public enum enumCoordType
    {
        /// <summary>
        /// 像素坐标(对矢量数据这个无效)
        /// </summary>
        Raster,
        /// <summary>
        /// 地理坐标（大地坐标）
        /// </summary>
        GeoCoord,
        /// <summary>
        /// 投影坐标
        /// </summary>
        PrjCoord
    }
}
