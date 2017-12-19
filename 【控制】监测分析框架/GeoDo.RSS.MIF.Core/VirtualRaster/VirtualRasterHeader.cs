using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Core
{
    public class VirtualRasterHeader
    {
        private VirtualRasterHeader()
        { }

        public CoordEnvelope CoordEnvelope { get; private set; }
        public float ResolutionX { get; private set; }
        public float ResolutionY { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public static VirtualRasterHeader Create(CoordEnvelope coordEnvelope, float resolutionX, float resolutionY)
        {
            VirtualRasterHeader vr = new VirtualRasterHeader();
            vr.CoordEnvelope = coordEnvelope;
            vr.ResolutionX = resolutionX;
            vr.ResolutionY = resolutionY;
            vr.Width = RasterRectOffset.GetInteger(coordEnvelope.Width / resolutionX);
            vr.Height = RasterRectOffset.GetInteger(coordEnvelope.Height / resolutionY);
            return vr;
        }

        public static VirtualRasterHeader Create(CoordEnvelope coordEnvelope, int width, int height)
        {
            VirtualRasterHeader vr = new VirtualRasterHeader();
            vr.CoordEnvelope = coordEnvelope;
            vr.Width = width;
            vr.Height = height;
            vr.ResolutionX = (float)(coordEnvelope.Width / width);
            vr.ResolutionY = (float)(coordEnvelope.Height / height);
            return vr;
        }

        public static VirtualRasterHeader Create(CoordPoint leftBottom, int width, int height, float resolutionX, float resolutionY)
        {
            VirtualRasterHeader vr = new VirtualRasterHeader();
            vr.Width = width;
            vr.Height = height;
            vr.ResolutionX = resolutionX;
            vr.ResolutionY = resolutionY;
            vr.CoordEnvelope = CoordEnvelope.FromLBWH(leftBottom.X, leftBottom.Y, resolutionX * width, resolutionY * height);
            return vr;
        }

        public static VirtualRasterHeader Create(IRasterDataProvider raster)
        {
            VirtualRasterHeader vr = new VirtualRasterHeader();
            vr.CoordEnvelope = raster.CoordEnvelope;
            vr.Width = raster.Width; //RasterOffsetHelper.GetInteger(raster.CoordEnvelope.Width / raster.ResolutionX);
            vr.Height = raster.Height;
            vr.ResolutionX = raster.ResolutionX;//(float)(raster.CoordEnvelope.Width / raster.Width);
            vr.ResolutionY = raster.ResolutionY;
            return vr;
        }
    }
}
