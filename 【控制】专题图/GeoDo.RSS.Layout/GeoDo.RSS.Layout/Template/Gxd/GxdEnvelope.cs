using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
    public class GxdEnvelope
    {
        public double MinX;
        public double MaxX;
        public double MinY;
        public double MaxY;

        public GxdEnvelope(double minx, double maxx, double miny, double maxy)
        {
            MinX = minx;
            MaxX = maxx;
            MinY = miny;
            MaxY = maxy;
        }

        public bool IsEmpty()
        {
            return Math.Abs(MinX) < double.Epsilon &&
                Math.Abs(MaxX) < double.Epsilon &&
                Math.Abs(MinY) < double.Epsilon &&
                Math.Abs(MaxY) < double.Epsilon;
        }

        public override string ToString()
        {
            return MinX.ToString() + "," + MaxX.ToString() + "," + MinY.ToString() + "," + MaxY.ToString();
        }

        public static GxdEnvelope From(string gxdEnvelopeString)
        {
            if (string.IsNullOrEmpty(gxdEnvelopeString))
                return null;
            string[] parts = gxdEnvelopeString.Split(',');
            if (parts.Length != 4)
                return null;
            double minX, maxX, minY, maxY;
            if (!double.TryParse(parts[0], out minX))
                return null;
            if (!double.TryParse(parts[1], out maxX))
                return null;
            if (!double.TryParse(parts[2], out minY))
                return null;
            if (!double.TryParse(parts[3], out maxY))
                return null;
            return new GxdEnvelope(minX, maxX, minY, maxY);
        }
    }
}
