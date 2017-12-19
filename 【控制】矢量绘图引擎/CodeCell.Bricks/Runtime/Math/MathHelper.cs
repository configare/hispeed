using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CodeCell.Bricks.Runtime
{
    public static class MathHelper
    {
        public static float GetAngle(PointF bPoint, PointF ePoint)
        {
            double dx = ePoint.X - bPoint.X;
            double dy = ePoint.Y - bPoint.Y;
            double LineLength = Math.Sqrt(dx * dx + dy * dy);
            double angle = Math.Asin(dy / LineLength) * 180 / Math.PI;
            if (dx < 0 && dy > 0)
                angle = 180 - angle;
            else if (dx < 0 && dy < 0)
                angle = 180 + Math.Abs(angle);
            return (float)angle;
        }

        public static float GetAngle(PointF bPoint, PointF ePoint, out int width)
        {
            float dx = ePoint.X - bPoint.X;
            float dy = ePoint.Y - bPoint.Y;
            float LineLength = (float)Math.Sqrt(dx * dx + dy * dy);
            float angle = (float)(Math.Asin(dy / LineLength) * 180 / Math.PI);
            if (dx < 0 && dy > 0)
                angle = 180 - angle;
            else if (dx < 0 && dy < 0)
                angle = 180 + Math.Abs(angle);
            width = (int)LineLength;
            return angle;
        }

        public static PointF[] GetPointsAtLine(PointF bPoint, PointF ePoint, float width, int step)
        {
            float dx = ePoint.X - bPoint.X;
            float dy = ePoint.Y - bPoint.Y;
            float x = 0, y = 0;
            float sina = dy / (float)width;
            float cosa = dx / (float)width;
            List<PointF> pts = new List<PointF>();
            PointF pt = new PointF();
            for (int L = 1; L < width; L += step)
            {
                x = bPoint.X + L * cosa;
                y = bPoint.Y + L * sina;
                pt.X = x;
                pt.Y = y;
                pts.Add(pt);
            }
            return pts.ToArray();
        }

        public static PointF GetOnePointAtLine(PointF bPoint, PointF ePoint, float width, int distance)
        {
            float dx = ePoint.X - bPoint.X;
            float dy = ePoint.Y - bPoint.Y;
            float x = 0, y = 0;
            float sina = dy / (float)width;
            float cosa = dx / (float)width;
            PointF pt = new PointF();
            x = bPoint.X + distance * cosa;
            y = bPoint.Y + distance * sina;
            pt.X = x;
            pt.Y = y;
            return pt;
        }

        public static void GenerateLogSpiral(float centerX, float centerY, float radius, float sides, float coils, float rotation,PointF[] buffers)
        {
            int pointIdx = 0;
            //设置螺旋线的起点
            buffers[pointIdx] = new PointF(centerX, centerY);
            buffers[pointIdx].X = centerX;
            buffers[pointIdx].Y = centerY;
            pointIdx++;
            // How far to rotate around center for each side.
            float aroundStep = coils / sides;// 0 to 1 based.
            // Convert aroundStep to radians.
            float aroundRadians = aroundStep * 2 * (float)Math.PI;
            // Convert rotation to radians.
            rotation *= (float)(2 * Math.PI);
            // For every side, step around and away from center.
            for (int i = 1; i <= sides; i++)
            {
                // How far away from center
                float away = (float)Math.Pow(radius, i / sides);
                //
                // How far around the center.
                float around = i * aroundRadians + rotation;
                //
                // Convert 'around' and 'away' to X and Y.
                float x = centerX + (float)Math.Cos(around) * away;
                float y = centerY + (float)Math.Sin(around) * away;
                // Now that you know it, do it.
                buffers[pointIdx].X = x;
                buffers[pointIdx].Y = y;
                pointIdx++;
            }
        }

        public static bool MatrixIsSame(Matrix a, Matrix b)
        {
            for (int i = 0; i < a.Elements.Length; i++)
            {
                if (Math.Abs(a.Elements[i] - b.Elements[i]) > float.Epsilon)
                    return false;
            }
            return true;
        }

        public static PointF GetMiddlePoint(PointF p1, PointF p2)
        {
            LineSpliter spliter = new LineSpliter(p1.X, p1.Y, p2.X, p2.Y);
            return spliter.GetMiddlePoint();
        }
    }
}
