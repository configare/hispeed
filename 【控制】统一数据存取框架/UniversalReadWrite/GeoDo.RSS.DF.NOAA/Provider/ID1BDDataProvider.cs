using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.DF.GDAL;

namespace GeoDo.RSS.DF.NOAA
{
    public interface ID1BDDataProvider:IGDALRasterDataProvider
    {
        D1BDHeader Header { get; }
        void ReadVisiCoefficient(ref double[,] operCoef, ref double[,] testCoef, ref double[,] beforeSendCoef);
        void ReadIRCoefficient(ref double[,] operCoef, ref double[,] beforeSendCoef);
    }
}
