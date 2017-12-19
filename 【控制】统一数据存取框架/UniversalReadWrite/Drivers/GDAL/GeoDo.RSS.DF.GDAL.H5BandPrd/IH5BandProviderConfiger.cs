using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GDAL.H5BandPrd
{
    internal interface IH5BandProviderConfiger
    {
        BandProviderDef[] GetBandProviderDefs();
    }
}
