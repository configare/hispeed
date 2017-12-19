using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;

namespace GeoDo.RSS.MIF.Core
{
    public static class PixelIndexMapperFactory
    {
        public static IPixelIndexMapper CreatePixelIndexMapper(string name,int width,int height,CoordEnvelope coordEnvelope,ISpatialReference spatialRef)
        {
            return new MatrixPixelIndexMapper(name, width, height,coordEnvelope,spatialRef);
        }
    }
}
