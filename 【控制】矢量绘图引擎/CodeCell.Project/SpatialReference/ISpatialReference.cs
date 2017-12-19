using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface ISpatialReference
    {
        string Name { get; set; }
        IGeographicCoordSystem GeographicsCoordSystem { get; }
        IProjectionCoordSystem ProjectionCoordSystem { get; }
        string ToProj4String();
        string ToWKTString();
        bool IsSame(ISpatialReference spatialRef);
        CoordinateDomain CoordinateDomain { get; set; }
        int OracleSpatialSRID { get; }
    }
}
