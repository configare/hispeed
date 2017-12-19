using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;

namespace GeoDo.RSS.MIF.Core
{
    public class Vector2BitmapConverter : IVector2BitmapConverter
    {
        public unsafe void ToBitmapUseRasterCoord(PointF[] points, byte[] types, Color trueColor, Color emptyColor, Envelope dstEnvelope, Size size, ref Bitmap buffer)
        {
            if (points == null || points.Length == 0 || types == null || types.Length == 0 || dstEnvelope == null || size.IsEmpty)
                return;
            float resolutionX = (float)dstEnvelope.Width / (float)size.Width;
            float resolutionY = (float)dstEnvelope.Height / (float)size.Height;
            fixed (PointF* ptrf = points)
            {
                PointF* ptr = ptrf;
                for (int i = 0; i < points.Length; i++)
                {
                    ptr->X = ptr->X  / resolutionX;
                    ptr->Y = ptr->Y / resolutionY;
                    ptr++;
                }
            }
            using (Graphics g = Graphics.FromImage(buffer))
            {
                using (GraphicsPath pth = new GraphicsPath(points,types))
                {
                    using (SolidBrush brush = new SolidBrush(trueColor))
                    {
                        g.FillPath(brush, pth);
                    }
                }
            }
        }

        public unsafe void ToBitmap(PointF[] points, byte[] types, Color trueColor, Color emptyColor, Envelope dstEnvelope, Size size, ref Bitmap buffer)
        {
            if (points == null || points.Length == 0 || types == null || types.Length == 0 || dstEnvelope == null || size.IsEmpty)
                return;
            ShapePoint leftUpCoord = dstEnvelope.LeftUpPoint;
            float resolutionX = (float)dstEnvelope.Width / (float)size.Width;
            float resolutionY = (float)dstEnvelope.Height / (float)size.Height;
            float leftUpX = (float)leftUpCoord.X;
            float leftUpY = (float)leftUpCoord.Y;
            fixed (PointF* ptrf = points)
            {
                PointF* ptr = ptrf;
                for (int i = 0; i < points.Length; i++)
                {
                    ptr->X = ((float)ptr->X - leftUpX) / resolutionX;
                    ptr->Y = -((float)ptr->Y - leftUpY) / resolutionY;
                    ptr++;
                }
            }
            using (Graphics g = Graphics.FromImage(buffer))
            {
                using (GraphicsPath pth = new GraphicsPath(points, types))
                {
                    using (SolidBrush brush = new SolidBrush(trueColor))
                    {
                        g.FillPath(brush, pth);
                    }
                }
            }
        }

        public unsafe void ToBitmap(Dictionary<ShapePolygon, Color> vectors, Color emptyColor, Envelope dstEnvelope, Size size, ref Bitmap buffer)
        {
            if (vectors == null || vectors.Count == 0 || dstEnvelope == null || size.Width == 0 || size.Height == 0 || buffer == null)
                return;
            ShapePoint leftUpCoord = dstEnvelope.LeftUpPoint;
            float resolutionX = (float)dstEnvelope.Width / (float)size.Width;
            float resolutionY = (float)dstEnvelope.Height / (float)size.Height;
            float leftUpX = (float)leftUpCoord.X;
            float leftUpY = (float)leftUpCoord.Y;
            using (Graphics g = Graphics.FromImage(buffer))
            {
                //g.Clear(emptyColor);
                g.InterpolationMode = InterpolationMode.NearestNeighbor;
                //g.SmoothingMode = SmoothingMode.HighQuality;
                foreach (ShapePolygon ply in vectors.Keys)
                {
                    if (!dstEnvelope.IsInteractived(ply.Envelope))
                        continue;
                    using (GraphicsPath pth = new GraphicsPath())
                    {
                        ShapeRing[] rings = ply.Rings;
                        foreach (ShapeRing ring in rings)
                        {
                            if (!dstEnvelope.IsInteractived(ring.Envelope))
                                continue;
                            PointF[] pts = new PointF[ring.Points.Length];
                            fixed (PointF* ptrf = pts)
                            {
                                PointF* ptr = ptrf;
                                ShapePoint[] shpts = ring.Points;
                                foreach (ShapePoint pt in shpts)
                                {
                                    ptr->X = ((float)pt.X - leftUpX) / resolutionX;
                                    ptr->Y = -((float)pt.Y - leftUpY) / resolutionY;
                                    ptr++;
                                }
                            }
                            pth.AddPolygon(pts);
                        }
                        using (SolidBrush brush = new SolidBrush(vectors[ply]))
                        {
                            g.FillPath(brush, pth);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {

        }
    }
}
