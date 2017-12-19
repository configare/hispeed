using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    public class RasterMosaicHelper : IRasterMosaicHelper
    {
        public void GetRasterMosaicInfo(string[] files, out CoordEnvelope dstEnvelope, out Size dstSize, out float resolutionX, out float resolutionY)
        {
            dstSize = Size.Empty;
            dstEnvelope = null;
            resolutionX = 0f;
            resolutionY = 0f;
        }
    }
}
