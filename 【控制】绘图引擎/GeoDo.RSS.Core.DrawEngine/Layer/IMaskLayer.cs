using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface IMaskLayer:ILayer
    {
        void Update(Color maskColor, Size rasterSize, CoordEnvelope coordEnvelope, bool isGeoCoord, int[] visibleRegion);
    }
}
