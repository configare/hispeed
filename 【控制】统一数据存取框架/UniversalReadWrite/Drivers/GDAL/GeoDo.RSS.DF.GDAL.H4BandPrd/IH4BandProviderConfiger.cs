using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GDAL.H4BandPrd
{
    internal interface IH4BandProviderConfiger
    {
        BandProviderDef[] GetBandProviderDefs();
    }
}
