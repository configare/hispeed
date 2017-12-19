using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Project
{
    internal interface ISpatialReferenceStringParser
    {
        ISpatialReference Parse(string wkt);
    }
}
