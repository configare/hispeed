using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.DF.GDAL;

namespace GeoDo.RSS.DF.NOAA
{
    public interface ID1A5DataProvider : IGDALRasterDataProvider
    {
        D1A5Header Header { get; }
        void ReadVisiCoefficient(ref double[,] operCoef, ref double[,] testCoef, ref double[,] beforeSendCoef);
        //void ReadIRCoefficient(ref double[,] operCoef, ref double[,] beforeSendCoef);
    }
}
