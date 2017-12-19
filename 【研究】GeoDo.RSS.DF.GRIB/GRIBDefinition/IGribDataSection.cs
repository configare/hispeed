using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GRIB
{
    /// <summary>
    /// Grib数据段
    /// </summary>
    public interface IGribDataSection
    {
        GRIB_Point[] Points { get; }
    }
}
