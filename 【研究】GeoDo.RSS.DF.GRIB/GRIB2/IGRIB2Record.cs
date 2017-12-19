using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GRIB
{
    interface IGRIB2Record
    {
        IGRIB2DataRepresentationSection DRS { get; }
        IGRIB2GridDefinitionSection GDS { get; }
        long GetGdsOffset();
        long GetPdsOffset();
        string Header { get; }
        IGRIB2ProductDefinitionSection PDS { get; }
        GRIB2DataSection DS { get; }
        IGribBitMapSection BMS { get; }
    }
}
