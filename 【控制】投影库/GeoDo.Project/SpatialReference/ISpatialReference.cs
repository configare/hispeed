using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Project
{
    public interface ISpatialReference:ICloneable
    {
        string Name { get; set; }
        IGeographicCoordSystem GeographicsCoordSystem { get; }
        IProjectionCoordSystem ProjectionCoordSystem { get; }
        string ToProj4String();
        string ToWKTString();
        string ToEnviProjectionInfoString();
        void ToEnviProjectionInfoString(out float[] argValues, out string units);
        bool IsSame(ISpatialReference spatialRef);
        CoordinateDomain CoordinateDomain { get; set; }
        int OracleSpatialSRID { get; }
    }
}
