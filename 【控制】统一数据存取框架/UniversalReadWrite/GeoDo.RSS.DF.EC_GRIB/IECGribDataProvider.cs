using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DF.EC_GRIB
{
    public interface IECGribDataProvider : IGeoDataProvider
    {
        string Parameter { get; }
        int DataLength { get; }
        float[] Values { get; }
    }
}
