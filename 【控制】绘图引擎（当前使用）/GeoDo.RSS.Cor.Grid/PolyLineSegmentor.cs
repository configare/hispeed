using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.Grid
{
    /// <summary>
    /// 本类用于对折线、曲线进行切割，去掉范围外的部分
    /// </summary>
    public static class PolyLineSegmentor
    {
        /// <summary>
        /// 依据指定的切割范围分割折线,范围外的扔掉
        /// </summary>
        /// <param name="pts">组成一条线段的点</param>
        /// <param name="splitPts"></param>
        public static void SplitWithClipBounds(PointF[] pts, RectangleF rect, out PointF[][] splitLines)
        {
            splitLines = null;
            try
            {
                List<PointF[]> splitLinesTemp = new List<PointF[]>();
                List<PointF> line = null;
                for (int i = 0; i < pts.Length; i++)
                {
                    if (rect.Contains(pts[i]))
                    {
                        if (line == null)   //入界
                        {
                            line = new List<PointF>();
                            if (i == 0)
                                line.Add(pts[i]);
                            else
                            {//不是第一个点，计算与前一个点的交点，加入线
                                double inctX = 0;
                                double inctY = 0;
                                if (GetIntersectionPoint(rect, pts[i - 1].X, pts[i - 1].Y, pts[i].X, pts[i].Y, out inctX, out inctY))
                                {
                                    line.Add(new PointF((float)inctX, (float)inctY));
                                    line.Add(pts[i]);
                                }
                            }
                        }
                        else if (i == pts.Length - 1)
                        {
                            line.Add(pts[i]);
                            if (line.Count >= 2)
                                splitLinesTemp.Add(line.ToArray());
                            line = null;
                        }
                        else
                            line.Add(pts[i]);
                    }
                    else if (line != null && line.Count != 0)  //出界,并且不是第一个点，计算与前一个点的交点，加入线
                    {
                        double inctX = 0;
                        double inctY = 0;
                        if (GetIntersectionPoint(rect, line[line.Count - 1].X, line[line.Count - 1].Y, pts[i].X, pts[i].Y, out inctX, out inctY))
                        {
                            line.Add(new PointF((float)inctX, (float)inctY));
                            if (line.Count >= 2)
                                splitLinesTemp.Add(line.ToArray());
                        }
                        line = null;
                    }
                    else if (i > 0) //两个点都在界外，但是有可能两个点的连线有部分在界内，计算与前一个点的交点，加入线
                    {
                        double inctX = 0;
                        double inctY = 0;
                        if (GetIntersectionPoint(rect, pts[i - 1].X, pts[i - 1].Y, pts[i].X, pts[i].Y, out inctX, out inctY))
                        {
                            line = new List<PointF>();
                            line.Add(new PointF((float)pts[i - 1].X, (float)pts[i - 1].Y));
                            line.Add(new PointF((float)inctX, (float)inctY));
                            line.Add(new PointF((float)pts[i].X, (float)pts[i].Y));
                            splitLinesTemp.Add(line.ToArray());
                        }
                        line = null;
                    }
                }
                if ((splitLinesTemp == null || splitLinesTemp.Count == 0) && line != null && line.Count != 0)
                    splitLinesTemp.Add(line.ToArray());
                splitLines = (splitLinesTemp.Count == 0 ? null : splitLinesTemp.ToArray());
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private static bool GetIntersectionPoint(RectangleF rect, float x1, float y1, float x2, float y2, out double inctX, out double inctY)
        {
            double a, b, c;
            GetLinePara(x1, y1, x2, y2, out a, out b, out c);
            double a2, b2, c2;
            inctX = 0;
            inctY = 0;
            GetLinePara(rect.X, rect.Y, rect.X, rect.Bottom, out a2, out b2, out c2);   //左边线
            if (GetIntersectionPoint(a, b, c, a2, b2, c2, ref inctX, ref inctY) &&
                InLine(x1, y1, x2, y2, inctX, inctY) && InLine(rect.X, rect.Y, rect.X, rect.Bottom, inctX, inctY))
            {
                return true;
            }
            GetLinePara(rect.X, rect.Y, rect.Right, rect.Y, out a2, out b2, out c2);     //上边线
            if (GetIntersectionPoint(a, b, c, a2, b2, c2, ref inctX, ref inctY) &&
                InLine(x1, y1, x2, y2, inctX, inctY) && InLine(rect.X, rect.Y, rect.Right, rect.Y, inctX, inctY))
            {
                return true;
            }
            GetLinePara(rect.Right, rect.Y, rect.Right, rect.Bottom, out a2, out b2, out c2);   //右边线
            if (GetIntersectionPoint(a, b, c, a2, b2, c2, ref inctX, ref inctY) &&
                InLine(x1, y1, x2, y2, inctX, inctY) && InLine(rect.Right, rect.Y, rect.Right, rect.Bottom, inctX, inctY))
            {
                return true;
            }
            GetLinePara(rect.X, rect.Bottom, rect.Right, rect.Bottom, out a2, out b2, out c2);   //下边线
            if (GetIntersectionPoint(a, b, c, a2, b2, c2, ref inctX, ref inctY) &&
                InLine(x1, y1, x2, y2, inctX, inctY) && InLine(rect.X, rect.Bottom, rect.Right, rect.Bottom, inctX, inctY))
            {
                return true;
            }
            return false;
        }

        private static bool InLine(float x1, float y1, float x2, float y2, double inctX, double inctY)
        {
            return ((inctX >= x1 && inctX <= x2) || (inctX <= x1 && inctX >= x2))
                && ((inctY >= y1 && inctY <= y2) || (inctY <= y1 && inctY >= y2));
        }

        //线段ab和线段cd的交点
        private static bool GetInterPoint(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy, ref double x, ref double y)
        {
            var area_abc = (ax - cx) * (by - cy) - (ay - cy) * (bx - cx);   //三角面积的2倍
            var area_abd = (ax - dx) * (by - dy) - (ay - dy) * (bx - dx);
            if (area_abc * area_abd >= 0)
                return false;
            var area_cda = (cx - ax) * (dy - ay) - (cy - ay) * (dx - ax);
            var area_cdb = area_cda + area_abc - area_abd;
            if (area_cda * area_cdb >= 0)
                return false;
            var t = area_cda / (area_abd - area_abc);
            var dtx = t * (bx - ax);
            var dty = t * (by - ay);
            x = ax + dtx;
            y = ay + dty;
            return true;
        }

        /// <summary>
        /// 计算直线方程aX+bY+c = 0的参数a,b,c
        /// </summary>
        private static void GetLinePara(float x1, float y1, float x2, float y2, out double a, out double b, out double c)
        {
            if (x1 == x2)
            {
                a = 1;
                b = 0;
                c = -x1;
            }
            else
            {
                b = 1;
                a = -(y1 - y2) / (x1 - x2);
                c = -(y1 + a * x1);
            }
        }

        /// <summary>
        /// 根据直线方程，计算两条直线的交点
        /// flag = 0; 不想交
        /// flag = 1; 相交点位于线段内
        /// flag = 2; 相交点位于延长线
        /// </summary>
        private static bool GetIntersectionPoint(double a1, double b1, double c1, double a2, double b2, double c2, ref double x, ref double y)
        {
            //flag = 0;
            if (a1 == a2 && b1 == b2)//平行
                return false;
            if (b1 == 0 && a2 == 0)
            {
                x = -c1;
                y = -c2;
            }
            else if (a1 == 0 && b2 == 0)
            {
                x = -c2;
                y = -c1;
            }
            else
            {
                //应处理与边界求交点时，交点的x值或者y值是一个无限循环浮点数，如630.99999999999989 时应为631
                x = Math.Round((b2 * c1 - b1 * c2) / (a2 * b1 - a1 * b2), 7);
                y = Math.Round((a1 * c2 - a2 * c1) / (a2 * b1 - a1 * b2), 7);
            }
            return true;
        }

    }
}
