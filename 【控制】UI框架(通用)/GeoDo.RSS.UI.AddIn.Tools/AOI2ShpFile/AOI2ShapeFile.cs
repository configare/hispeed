using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using System.Drawing;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public class AOI2ShapeFile : IAOI2ShapeFile
    {
        public void Export(Envelope geoEnvelope, Size size, int[] aoi, string shpFileName)
        {
            Feature fet = Export(geoEnvelope, size, aoi);
            if (fet == null)
                return;
            EsriShapeFilesWriter w = new EsriShapeFilesWriter(shpFileName, new Feature[] { fet }, enumShapeType.Polygon);
        }

        public Feature Export(Envelope geoEnvelope, Size size, int[] aoi)
        {
            Dictionary<int, AOIHelper.AOIRowRange> rowRanges = AOIHelper.ComputeRowIndexRange(aoi, size);
            if (rowRanges == null || rowRanges.Count == 0)
                return null;
            int bCol = 0, eCol = 0;
            int width = size.Width;
            double top = geoEnvelope.MaxY;
            double ySpan = geoEnvelope.Height / size.Height;
            double left = geoEnvelope.MinX;
            double xSpan = geoEnvelope.Width / size.Width;
            List<ShapePoint> leftPoints = new List<ShapePoint>();
            List<ShapePoint> retPoints = new List<ShapePoint>();
            Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, size);
            double x = 0, y = 0;
            int iRow = 0;
            foreach (int row in rowRanges.Keys)
            {
                bCol = aoi[rowRanges[row].BeginIndex] % width;
                eCol = aoi[rowRanges[row].EndIndex - 1] % width;
                x = left + bCol * xSpan;
                y = top - row * ySpan;
                leftPoints.Add(new ShapePoint(x, y));
                x = left + eCol * xSpan;
                retPoints.Add(new ShapePoint(x, y)); //right point
                iRow++;
            }
            leftPoints.Reverse();
            retPoints.AddRange(leftPoints);
            //
            ShapePolygon ply = new ShapePolygon(new ShapeRing[] { new ShapeRing(retPoints.ToArray()) });
            Feature fet = new Feature(0, ply, new string[] { "NAME" }, new string[] { "AOI" }, null);
            return fet;
        }
    }
}
