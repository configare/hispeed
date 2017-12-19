using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.RasterTools
{
    public interface IContourPersisit
    {
        void Write(ContourLine[] cntLines,ContourPersist.enumCoordType coordType,ContourLine.ContourEnvelope envelope,string spatialRef, string fname);
        ContourLine[] Read(string fname, out ContourPersist.enumCoordType coordType, out ContourLine.ContourEnvelope envelope, out string spatialRef);
    }
}
