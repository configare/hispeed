using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    public interface IVector2BitmapConverter:IDisposable
    {
        void ToBitmap(Dictionary<ShapePolygon, Color> vectors,Color emptyColor, Envelope dstEnvelope, Size size, ref Bitmap buffer);
        void ToBitmap(PointF[] points, byte[] types, Color trueColor, Color emptyColor, Envelope dstEnvelope, Size size, ref Bitmap buffer);
        void ToBitmapUseRasterCoord(PointF[] points, byte[] types, Color trueColor, Color emptyColor, Envelope dstEnvelope, Size size, ref Bitmap buffer);
    }
}
