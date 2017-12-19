using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.RasterTools
{
    public static class ExtMethods
    {
        public static Rectangle ToRasterRect(this IRasterDataProvider dataProvider, CoordEnvelope dataCoordEnvelope)
        {
            CoordEnvelope evp = dataProvider.CoordEnvelope.Intersect(dataCoordEnvelope);
            if (evp == null || evp.IsEmpty())
                return Rectangle.Empty;
            double resX = dataProvider.CoordEnvelope.Width / dataProvider.Width;
            double resY = dataProvider.CoordEnvelope.Height / dataProvider.Height;
            double x = (evp.MinX - dataCoordEnvelope.MinX) / resX;
            double y = (evp.MaxY - dataCoordEnvelope.MaxY) / resY;
            double w = evp.Width / resX;
            double h = evp.Height / resY;
            return new Rectangle((int)x, (int)y, (int)w, (int)h);
        }

        public static CoordEnvelope ToCoordEnvelope(this IRasterDataProvider dataProvider, Rectangle rect)
        {
            Rectangle srcRect = new Rectangle(0, 0, dataProvider.Width, dataProvider.Height);
            rect.Intersect(srcRect);
            if (rect.IsEmpty)
                return null;
            double resX = dataProvider.CoordEnvelope.Width / dataProvider.Width;
            double resY = dataProvider.CoordEnvelope.Height / dataProvider.Height;
            double minX = dataProvider.CoordEnvelope.MinX + resX * rect.Left;
            double maxY = dataProvider.CoordEnvelope.MaxY - resY * rect.Top;
            double w = rect.Width * resX;
            double h = rect.Height * resY;
            return new CoordEnvelope(minX, minX + w, maxY - h, maxY);
        }
    }
}
