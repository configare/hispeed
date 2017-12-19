using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using GeoDo.RSS.Core.DrawEngine.GDIPlus;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    /// <summary>
    /// 量测工具图层
    /// </summary>
    public class MeasureLayer : Layer, IFlyLayer
    {
        private bool _visible = true;
        private List<List<CoordPoint>> _previousParts = new List<List<CoordPoint>>();
        private List<CoordPoint> _coordinates = new List<CoordPoint>();
        private bool _isDrawing = false;
        private Point _mousePosition;
        private List<double[]> _previousDistances = new List<double[]>();   //历史每段的距离
        private List<double> _curDistances = new List<double>();            //当前段距离
        private double _previousDistance;
        private double _mouseDistance;
        private CoordPoint _preControlPointGeo;

        public MeasureLayer()
            :base()
        {
            _name = "MeasureLayer";
            _alias = "量测图层";
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            if (!_visible)
                return;
            ICanvas canvas = sender as ICanvas;
            ICoordinateTransform coordTran = canvas.CoordTransform;
            QuickTransform qt = drawArgs.QuickTransformArgs;
            Graphics g = drawArgs.Graphics as Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Brush blue = new SolidBrush(Color.FromArgb(60, 0, 0, 255));

            Pen bluePen = new Pen(Color.Blue, 2F);
            Pen redPen = new Pen(Color.Red, 3F);
            redPen.DashStyle = DashStyle.Dot;
            Brush redBrush = new SolidBrush(Color.Red);
            Brush yelloBrush = new SolidBrush(Color.Yellow);
            Font font = new Font("微软雅黑", 10.0f);
            SizeF distanceFontSize = g.MeasureString("12111.0", font);
            Brush textBackgroundBrush = new SolidBrush(Color.FromArgb(100, 250, 250, 250));
            Pen textBackUpPen = new Pen(Color.FromArgb(125, 125, 125));

            if (_previousParts != null && _previousParts.Count > 0)
            {
                GraphicsPath previous = new GraphicsPath();
                previous.FillMode = FillMode.Winding;
                for (int i = 0; i < _previousParts.Count; i++)
                {
                    List<PointF> points = new List<PointF>();
                    List<CoordPoint> part = _previousParts[i];
                    for (int j = 0; j < part.Count; j++)
                    {
                        int col = 0, row = 0;
                        coordTran.Prj2Screen(part[j].X, part[j].Y, out col, out row);
                        points.Add(new PointF(col, row));
                    }
                    g.DrawLines(bluePen, points.ToArray());
                    for (int j = 0; j < points.Count; j++)
                    {
                        g.FillEllipse(yelloBrush, new RectangleF(points[j].X - 3, points[j].Y - 3, 6, 6));
                        if (j == 0)
                        {
                            SizeF textRect = g.MeasureString("起点",font);
                            g.FillRectangle(textBackgroundBrush, points[j].X + 5, points[j].Y - distanceFontSize.Height / 2, textRect.Width, textRect.Height);
                            g.DrawRectangle(textBackUpPen, points[j].X + 5, points[j].Y - distanceFontSize.Height / 2, textRect.Width, textRect.Height);
                            g.DrawString("起点", font, redBrush, points[j].X + 5, points[j].Y - distanceFontSize.Height / 2);
                        }
                        else if (j == _previousDistances[i].Length - 1)
                        {
                            SizeF textRect = g.MeasureString("总长" + FormatDistance(_previousDistances[i][j]), font);
                            g.FillRectangle(textBackgroundBrush, points[j].X + 5, points[j].Y - distanceFontSize.Height / 2, textRect.Width, textRect.Height);
                            g.DrawRectangle(textBackUpPen, points[j].X + 5, points[j].Y - distanceFontSize.Height / 2, textRect.Width, textRect.Height);
                            g.DrawString("总长" + FormatDistance(_previousDistances[i][j]), font, redBrush, points[j].X + 5, points[j].Y - distanceFontSize.Height / 2);
                        }
                        else
                        {
                            SizeF textRect = g.MeasureString(FormatDistance(_previousDistances[i][j]), font);
                            g.FillRectangle(textBackgroundBrush, points[j].X + 5, points[j].Y - distanceFontSize.Height / 2, textRect.Width, textRect.Height);
                            g.DrawRectangle(textBackUpPen, points[j].X + 5, points[j].Y - distanceFontSize.Height / 2, textRect.Width, textRect.Height);
                            g.DrawString(FormatDistance(_previousDistances[i][j]), font, redBrush, points[j].X + 5, points[j].Y - distanceFontSize.Height / 2);
                        }
                    }
                }
            }

            List<PointF> curpoints = new List<PointF>();
            if (_coordinates != null)
            {
                int srceenX = 0, srceenY = 0;
                for (int j = 0; j < _coordinates.Count; j++)
                {
                    coordTran.Prj2Screen(_coordinates[j].X, _coordinates[j].Y, out srceenX, out srceenY);
                    curpoints.Add(new PointF(srceenX, srceenY));
                }
                if (curpoints.Count > 1)
                {
                    g.DrawLines(bluePen, curpoints.ToArray());
                    for (int j = 0; j < curpoints.Count; j++)
                    {
                        PointF pt = curpoints[j];
                        g.FillRectangle(redBrush, new RectangleF(pt.X - 2.5f, pt.Y - 2.5f, 5f, 5f));
                        if (j == 0)
                        {
                            g.DrawString("起点", font, redBrush, pt.X + 5, pt.Y - distanceFontSize.Height / 2);
                        }
                        else
                        {
                            g.DrawString(FormatDistance(_curDistances[j]), font, redBrush, pt.X + 5, pt.Y - distanceFontSize.Height / 2);
                        }
                    }
                }

                if (curpoints.Count > 0 && _mouseDistance > 0)// && _standBy == false && hasMouse)
                {
                    g.DrawLine(redPen, curpoints[curpoints.Count - 1], _mousePosition);
                    SizeF size = g.MeasureString("文字", font);
                    g.DrawString(string.Format("{0}", FormatDistance(_mouseDistance)), font, redBrush, _mousePosition.X + 2, _mousePosition.Y + size.Height);
                    g.DrawString("双击结束", font, redBrush, _mousePosition.X + 2, _mousePosition.Y + size.Height + size.Height);
                    //if (_areaMode && points.Count > 1)
                    //{
                    //    g.DrawLine(redPen, points[0], _mousePosition);
                    //}
                }
                //if (points.Count > 1 && _areaMode && (_previousParts == null || _previousParts.Count == 0))
                //{
                //    if (hasMouse && !_standBy)
                //    {
                //        points.Add(_mousePosition);
                //    }

                //    if (points.Count > 2)
                //    {
                //        g.FillPolygon(blue, points.ToArray());
                //    }
                //}
            }
            bluePen.Dispose();
            redPen.Dispose();
            redBrush.Dispose();
            yelloBrush.Dispose();
            font.Dispose();
            textBackgroundBrush.Dispose();
            textBackUpPen.Dispose();
        }

        private string FormatDistance(double distance)
        {
            if (distance < 1000)
                return distance.ToString("f1") + "米";
            else
                return (distance / 1000).ToString("f1") + "千米";
        }

        [Category("状态"), DisplayName("是否可见")]
        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;
            }
        }

        public void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
            if (!_enabled)
                return;
            switch (eventType)
            {
                case enumCanvasEventType.MouseDown:
                    if (_coordinates == null)
                    {
                        _coordinates = new List<CoordPoint>();
                        _curDistances = new List<double>();
                    }
                    PointF pt = new PointF(e.ScreenX, e.ScreenY);
                    ToRasterPointF(ref pt, sender as ICanvas);
                    CoordPoint perCoord = new CoordPoint(pt.X, pt.Y);
                    CoordPoint controlPointGeo = new CoordPoint(perCoord.X, perCoord.Y);
                    PrjToGeo(controlPointGeo, (sender as ICanvas).CoordTransform);
                    double distance =0;
                    if (_preControlPointGeo != null)//非起始点
                    {
                        distance = GetDist(_preControlPointGeo, controlPointGeo);
                        if (distance == 0)//和前一个点重合,放弃该点
                            break;
                        _previousDistance = _previousDistance + distance;
                    }
                    _coordinates.Add(perCoord);
                    _curDistances.Add(_previousDistance);
                    _preControlPointGeo = controlPointGeo;
                    _isDrawing = true;
                    break;
                case enumCanvasEventType.MouseMove:
                    if (_isDrawing)
                    {
                        if (_coordinates == null || _coordinates.Count == 0)
                        {
                            return;
                        }
                        _mousePosition = new Point(e.ScreenX, e.ScreenY);
                        PointF mousePosition2 = new PointF(e.ScreenX, e.ScreenY);
                        CoordPoint mouseCoord = SrceenToGeo(mousePosition2, (sender as ICanvas).CoordTransform);
                        _mouseDistance = GetDist(_preControlPointGeo, mouseCoord);
                        (sender as ICanvas).Refresh(enumRefreshType.FlyLayer);
                    }
                    break;
                case enumCanvasEventType.DoubleClick:
                    if (_isDrawing)
                    {
                        _previousParts.Add(_coordinates);
                        _previousDistances.Add(_curDistances.ToArray());
                        _coordinates = new List<CoordPoint>();
                        _curDistances = new List<double>();
                        _mousePosition = Point.Empty;
                        _isDrawing = false;
                        _preControlPointGeo = null;
                        _previousDistance = 0f;
                        _enabled = false;
                    }
                    break;
            }
        }

        private CoordPoint SrceenToGeo(PointF pt, ICoordinateTransform coordTran)
        {
            double prjX = 0, prjY = 0;
            double geoX = 0, geoY = 0;
            coordTran.Screen2Prj(pt.X, pt.Y, out prjX, out prjY);
            coordTran.Prj2Geo(prjX, prjY, out geoX, out geoY);
            return new CoordPoint(geoX, geoY);
        }

        private double GetDist(CoordPoint c1, CoordPoint c2)
        {
            double dist = MIF.Core.AreaCountHelper.ComputeSpheralSurfaceDistance(c1.X, c1.Y, c2.X, c2.Y);
            return dist;
        }

        private void PrjToGeo(CoordPoint pt, ICoordinateTransform coordTran)
        {
            double prjX = 0, prjY = 0;
            coordTran.Prj2Geo(pt.X, pt.Y, out prjX, out prjY);
            pt.X = prjX;
            pt.Y = prjY;
        }

        private void ToRasterPointF(ref PointF pt, ICanvas canvas)
        {
            ICoordinateTransform coordTran = canvas.CoordTransform;
            //if (canvas.PrimaryDrawObject != null)//有活动影像是返回栅格坐标
            //{
            //    float row = 0, col = 0;
            //    coordTran.Screen2Raster(pt.X, pt.Y, out row, out col);
            //    pt.X = col;
            //    pt.Y = row;
            //}
            //else//无活动影像时返回投影坐标
            {
                double prjX = 0, prjY = 0;
                coordTran.Screen2Prj(pt.X, pt.Y, out prjX, out prjY);
                pt.X = (float)prjX;
                pt.Y = (float)prjY;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
