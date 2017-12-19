using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IProjectionCoordSystem
    {
        string NameDes { get; set; }
        NameMapItem Name { get; }
        NameValuePair[] Parameters { get; }
        AngularUnit Unit { get; }
        string ToProj4String();
        bool IsSame(IProjectionCoordSystem prjCoordSystem);
        int ToOracleSpatialSRID();
    }
}
