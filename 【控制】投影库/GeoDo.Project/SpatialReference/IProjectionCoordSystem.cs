using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Project
{
    public interface IProjectionCoordSystem:ICloneable
    {
        string NameDes { get; set; }
        NameMapItem Name { get; }
        NameValuePair[] Parameters { get; }
        AngularUnit Unit { get; }
        string ToProj4String();
        bool IsSame(IProjectionCoordSystem prjCoordSystem);
        NameValuePair GetParaByName(string enviParaName);
        int ToOracleSpatialSRID();
    }
}
