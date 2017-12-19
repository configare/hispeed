using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public struct PixelInfo
    {
        public int ScreenX;
        public int ScreenY;
        public int RasterX;
        public int RasterY;
        public double PrjX;
        public double PrjY;
        public double GeoX;
        public double GeoY;
    }
}
