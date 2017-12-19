using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GDAL.H5BandPrd
{
    internal interface IBandNameParser
    {
        BandName[] Parse(string bandName);
    }
}
