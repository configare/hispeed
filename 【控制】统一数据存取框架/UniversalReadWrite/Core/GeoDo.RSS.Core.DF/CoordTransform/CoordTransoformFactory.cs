using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DF
{
    public static class CoordTransoformFactory
    {
        public static ICoordTransform GetCoordTransform(object spatialRef,double[] GTs,int width,int height)
        {
            if (spatialRef == null)
                return new UndressedCoordTransform(width, height);
            else
                return new RasterCoordTransform(GTs);
        }

        public static ICoordTransform GetCoordTransform(Point pt1, Point pt2, double[] coordPt1, double[] coordPt2)
        {
            return new LinearCoordTransform(pt1, pt2, coordPt1, coordPt2);
        }
    }
}
