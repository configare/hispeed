using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Project
{
    public interface IGeographicCoordSystem:ICloneable
    {
        string Name { get;  }
        AngularUnit AngularUnit { get; }
        PrimeMeridian PrimeMeridian { get; }
        Datum Datum { get; }
        bool IsSame(IGeographicCoordSystem geoCoordSys);
        string ToProj4String();
        string ToProj4ArgsString();
        int ToOracleSpatialSRID();
    }
}
