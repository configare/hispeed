using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CodeCell.Bricks.ModelFabric
{
    internal class GeometryHelper
    {
        public static void GetLinkLine(ActionElement fAction, ActionElement tAction, out PointF fpt, out PointF tpt)
        {
            //直线点
            PointF p1 = new PointF(fAction.Location.X + fAction.Size.Width / 2, fAction.Location.Y + fAction.Size.Height / 2);
            fpt = p1;
            PointF p2 = new PointF(tAction.Location.X + tAction.Size.Width / 2, tAction.Location.Y + tAction.Size.Height / 2);
            //椭圆的点
            PointF ep1 = new PointF(tAction.Location.X, tAction.Location.Y + tAction.Size.Height / 2);//左边的点
            PointF ep2 = new PointF(tAction.Location.X + tAction.Size.Width / 2, tAction.Location.Y); //上边的点
            //将这些点变换到直角坐标系
            PointF[] pts = new PointF[] { p1, p2, ep1, ep2 };
            Matrix m = new Matrix();
            m.Scale(1f, -1f);
            m.Translate(-(tAction.Location.X + tAction.Size.Width / 2), -(tAction.Location.Y + tAction.Size.Height / 2));
            m.TransformPoints(pts);
            p1 = pts[0];
            p2 = pts[1];
            ep1 = pts[2];
            ep2 = pts[3];
            //
            double x = 0, y = 0;
            //计算直线的参数
            if (Math.Abs(p2.X - p1.X) < float.Epsilon) //两个椭圆的中心都与Y轴重合
            {
                x = p1.X;
                if (p1.Y < p2.Y)
                    y = ep2.Y - tAction.Size.Height;
                else
                    y = ep2.Y;
            }
            else
            {
                float k = (p2.Y - p1.Y) / (p2.X - p1.X);
                //计算椭圆的参数
                float c = Math.Abs(ep1.X), d = Math.Abs(ep2.Y); //X^2/c + Y^2/d = 1
                c *= c;
                d *= d;
                //计算交点
                double x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                x1 = Math.Sqrt((c * d) / (d + c * k * k));
                x2 = -x1;
                y1 = k * x1;
                y2 = k * x2;
                //坐标反变换到屏幕坐标
                if (p1.X > 0)
                    x = x1 > 0 ? x1 : x2;
                else
                    x = x1 > 0 ? x2 : x1;
                if (p1.Y > 0)
                    y = y1 > 0 ? y1 : y2;
                else
                    y = y1 > 0 ? y2 : y1;
            }
            pts = new PointF[] { new PointF((float)x,(float)y) };
            m.Invert();
            m.TransformPoints(pts);
            tpt = pts[0];
            m.Dispose();
        }
    }
}
