using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.ComponentModel;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    public class PencilToolLayer : Layer, IFlyLayer, IPencilToolLayer
    {
        protected bool _visible = false;
        protected enumPencilType _pencilType = enumPencilType.FreeCurve;
        protected Action<GeometryOfDrawed> _pencilDrawedResultFinished;
        protected Action<PointF> _controlPointIsAdded;
        protected List<Point> _vertexts = new List<Point>();
        protected List<Point> _controls = new List<Point>();
        protected bool _isDrawing = false;
        private Point _prePoint;
        private Point _crtPoint;
        private Color _penColor = Color.Yellow;
        private Pen _pen = new Pen(Color.Yellow);

        public PencilToolLayer()
        {
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public void Reset()
        {
            _vertexts.Clear();
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            switch (_pencilType)
            {
                case enumPencilType.FreeCurve:
                    RenderFreeCurve(sender, drawArgs);
                    break;
                case enumPencilType.Rectangle:
                    RenderRectangle(sender, drawArgs);
                    break;
                case enumPencilType.Polygon:
                    RenderPolygon(sender, drawArgs);
                    break;
                case enumPencilType.Circle:
                    RenderCircle(sender, drawArgs);
                    break;
                case enumPencilType.ControlFreeCurve:
                    RenderControlFreeCurve(sender, drawArgs);
                    break;
            }
        }

        private void RenderControlFreeCurve(object sender, IDrawArgs drawArgs)
        {
            if (_vertexts.Count < 3)
                return;
            Graphics g = drawArgs.Graphics as Graphics;
            g.SmoothingMode = SmoothingMode.HighQuality;
            using (GraphicsPath path = GetGraphicsPath())
            {
                g.DrawPath(_pen, path);
            }
            //画拐点
            for (int i = 0; i < _controls.Count; i++)
            {
                g.FillEllipse(Brushes.Yellow, new Rectangle(_controls[i].X - 3, _controls[i].Y - 3, 6, 6));
            }
        }

        private void RenderCircle(object sender, IDrawArgs drawArgs)
        {
            if (!_isDrawing || _prePoint.IsEmpty || _crtPoint.IsEmpty)
                return;
            Graphics g = drawArgs.Graphics as Graphics;
            using (GraphicsPath path = GetGraphicsPath())
            {
                if (path == null)
                    return;
                g.DrawPath(_pen, path);
            }
        }

        private void RenderPolygon(object sender, IDrawArgs drawArgs)
        {
            if (_vertexts.Count < 1)
                return;
            Graphics g = drawArgs.Graphics as Graphics;
            using (GraphicsPath path = GetGraphicsPath())
            {
                g.DrawPath(_pen, path);
            }
        }

        private void RenderRectangle(object sender, IDrawArgs drawArgs)
        {
            if (_prePoint.IsEmpty || _crtPoint.IsEmpty)
                return;
            Graphics g = drawArgs.Graphics as Graphics;
            using (GraphicsPath path = GetGraphicsPath())
            {
                g.DrawPath(_pen, path);
            }
        }

        private void RenderFreeCurve(object sender, IDrawArgs drawArgs)
        {
            if (_vertexts.Count < 3)
                return;
            Graphics g = drawArgs.Graphics as Graphics;
            using (GraphicsPath path = GetGraphicsPath())
            {
                g.DrawPath(_pen, path);
            }
        }

        private GraphicsPath GetGraphicsPath()
        {
            GraphicsPath path = new GraphicsPath();
            switch (_pencilType)
            {
                case enumPencilType.Circle:
                    Rectangle rect = new Rectangle(Math.Min(_prePoint.X, _crtPoint.X),
                            Math.Min(_prePoint.Y, _crtPoint.Y),
                            Math.Abs(_prePoint.X - _crtPoint.X),
                            Math.Abs(_prePoint.Y - _crtPoint.Y));
                    if (Control.ModifierKeys == Keys.Shift)
                        rect.Width = rect.Height;
                    path.AddEllipse(rect);
                    break;
                case enumPencilType.FreeCurve:
                    if (_vertexts.Count < 3)
                        return null;
                    path.AddCurve(_vertexts.ToArray());
                    break;
                case enumPencilType.ControlFreeCurve:
                    if (_vertexts.Count < 3)
                        return null;
                    path.AddCurve(_vertexts.ToArray());
                    break;
                case enumPencilType.Polygon:
                    for (int i = 0; i < _vertexts.Count - 1; i++)
                    {
                        path.AddLine(_vertexts[i], _vertexts[i + 1]);
                    }
                    path.AddLine(_vertexts[_vertexts.Count - 1], _crtPoint);
                    if (_vertexts.Count > 2)
                        path.AddLine(_crtPoint, _vertexts[0]);
                    break;
                case enumPencilType.Rectangle:
                    Rectangle ellrect = new Rectangle(Math.Min(_prePoint.X, _crtPoint.X),
                        Math.Min(_prePoint.Y, _crtPoint.Y), Math.Abs(_prePoint.X - _crtPoint.X),
                        Math.Abs(_prePoint.Y - _crtPoint.Y));
                    path.AddRectangle(ellrect);
                    break;
            }
            return path;
        }

        public void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
            switch (_pencilType)
            {
                case enumPencilType.Point:
                    EventPoint(sender, eventType, e);
                    break;
                case enumPencilType.FreeCurve:
                    EventFreeCurve(sender, eventType, e);
                    break;
                case enumPencilType.Rectangle:
                    EventRectangle(sender, eventType, e);
                    break;
                case enumPencilType.Polygon:
                    EventPolygon(sender, eventType, e);
                    break;
                case enumPencilType.Circle:
                    EventCircle(sender, eventType, e);
                    break;
                case enumPencilType.ControlFreeCurve:
                    EventControlFreeCurve(sender, eventType, e);
                    break;
            }
        }

        private void EventPoint(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
            if (eventType != enumCanvasEventType.MouseDown)
                return;
            GeometryOfDrawed geometry = new GeometryOfDrawed();
            geometry.ShapeType = _pencilType.ToString();
            ICoordinateTransform tran = (sender as ICanvas).CoordTransform;
            float row = 0, col = 0;
            tran.Screen2Raster(e.ScreenX, e.ScreenY, out row, out col);
            geometry.RasterPoints = new PointF[] { new PointF(row, col) };
            if (this._pencilDrawedResultFinished != null)
                this._pencilDrawedResultFinished(geometry);
        }

        private void EventControlFreeCurve(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
        reSwitchLine:
            switch (eventType)
            {
                case enumCanvasEventType.MouseDown:
                    if (Control.MouseButtons == MouseButtons.Right)
                    {
                        eventType = enumCanvasEventType.DoubleClick;
                        goto reSwitchLine;
                    }
                    if (Control.MouseButtons == MouseButtons.Left && !_isDrawing)
                    {
                        _vertexts.Clear();
                        _controls.Clear();
                        _vertexts.Add(new Point(e.ScreenX, e.ScreenY));
                        _controls.Add(new Point(e.ScreenX, e.ScreenY));
                        if (_controlPointIsAdded != null)
                        {
                            PointF pt = new PointF(_controls[_controls.Count - 1].X, _controls[_controls.Count - 1].Y);
                            ToRasterPointF(ref pt,sender as ICanvas);
                            _controlPointIsAdded(pt);
                        }
                        _isDrawing = true;
                    }
                    else if (Control.MouseButtons == MouseButtons.Left)
                    {
                        _controls.Add(new Point(e.ScreenX, e.ScreenY));
                        if (_controlPointIsAdded != null)
                        {
                            PointF pt = new PointF(_controls[_controls.Count - 1].X, _controls[_controls.Count - 1].Y);
                            ToRasterPointF(ref pt, sender as ICanvas);
                            _controlPointIsAdded(pt);
                        }
                    }
                    break;
                case enumCanvasEventType.MouseMove:
                    if (_isDrawing)
                    {
                        _vertexts.Add(new Point(e.ScreenX, e.ScreenY));
                        (sender as ICanvas).Refresh(enumRefreshType.FlyLayer);
                    }
                    break;
                case enumCanvasEventType.DoubleClick:
                    if (_isDrawing)
                    {
                        //if (_vertexts.Count > 1)//闭合          //线，不需要闭合
                        //    _vertexts.Add(_vertexts[0]);
                        (sender as ICanvas).Refresh(enumRefreshType.FlyLayer);
                        NotifyResult((sender as ICanvas).CoordTransform, sender as ICanvas);
                        _vertexts.Clear();
                        _controls.Clear();
                        _isDrawing = false;
                    }
                    break;
            }
        }

        private void EventCircle(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
            switch (eventType)
            {
                case enumCanvasEventType.MouseDown:
                    if (!_isDrawing)
                    {
                        _prePoint = new Point(e.ScreenX, e.ScreenY);
                        _isDrawing = true;
                    }
                    break;
                case enumCanvasEventType.MouseMove:
                    if (_isDrawing)
                    {
                        _crtPoint = new Point(e.ScreenX, e.ScreenY);
                        (sender as ICanvas).Refresh(enumRefreshType.FlyLayer);
                    }
                    break;
                case enumCanvasEventType.MouseUp:
                    if (_isDrawing)
                    {
                        if (_prePoint.X == e.ScreenX && _prePoint.Y == e.ScreenY)//鼠标未移动，不绘制
                        {
                            _crtPoint = Point.Empty;
                            _prePoint = Point.Empty;
                            _isDrawing = false;
                            return;
                        }
                        (sender as ICanvas).Refresh(enumRefreshType.FlyLayer);
                        NotifyResult((sender as ICanvas).CoordTransform, sender as ICanvas);
                        _crtPoint = Point.Empty;
                        _prePoint = Point.Empty;
                        _isDrawing = false;
                    }
                    break;
            }
        }

        private void EventPolygon(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
        reSwitchLine:
            switch (eventType)
            {
                case enumCanvasEventType.MouseDown:
                    if (Control.MouseButtons == MouseButtons.Right)
                    {
                        eventType = enumCanvasEventType.DoubleClick;
                        goto reSwitchLine;
                    }
                    if (!_isDrawing)
                    {
                        _vertexts.Clear();
                        _vertexts.Add(new Point(e.ScreenX, e.ScreenY));
                        _isDrawing = true;
                    }
                    else
                    {
                        _vertexts.Add(new Point(e.ScreenX, e.ScreenY));
                        (sender as ICanvas).Refresh(enumRefreshType.FlyLayer);
                    }
                    break;
                case enumCanvasEventType.MouseMove:
                    if (_isDrawing)
                    {
                        _crtPoint = new Point(e.ScreenX, e.ScreenY);
                        (sender as ICanvas).Refresh(enumRefreshType.FlyLayer);
                    }
                    break;
                case enumCanvasEventType.DoubleClick:
                    _vertexts.Add(new Point(e.ScreenX, e.ScreenY));
                    _isDrawing = false;
                    (sender as ICanvas).Refresh(enumRefreshType.FlyLayer);
                    NotifyResult((sender as ICanvas).CoordTransform, sender as ICanvas);
                    _vertexts.Clear();
                    break;
            }

        }

        private void EventRectangle(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
        reSwitchLine:
            switch (eventType)
            {
                case enumCanvasEventType.MouseDown:
                    if (Control.MouseButtons == MouseButtons.Right)
                    {
                        eventType = enumCanvasEventType.MouseUp;
                        goto reSwitchLine;
                    }
                    if (!_isDrawing)
                    {
                        _prePoint = new Point(e.ScreenX, e.ScreenY);
                        _isDrawing = true;
                    }
                    break;
                case enumCanvasEventType.MouseMove:
                    if (_isDrawing && Control.MouseButtons == MouseButtons.Left)
                    {
                        _crtPoint = new Point(e.ScreenX, e.ScreenY);
                        (sender as ICanvas).Refresh(enumRefreshType.FlyLayer);
                    }
                    break;
                case enumCanvasEventType.MouseUp:
                    if (_isDrawing)
                    {
                        if (_prePoint.X == e.ScreenX && _prePoint.Y == e.ScreenY)//鼠标未移动，不绘制
                        {
                            _prePoint = Point.Empty;
                            _crtPoint = Point.Empty;
                            _isDrawing = false;
                            return;
                        }
                        (sender as ICanvas).Refresh(enumRefreshType.FlyLayer);
                        NotifyResult((sender as ICanvas).CoordTransform, sender as ICanvas);
                        _prePoint = Point.Empty;
                        _crtPoint = Point.Empty;
                        _isDrawing = false;
                    }
                    break;
            }
        }

        private void EventFreeCurve(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
        reSwitchLine:
            switch (eventType)
            {
                case enumCanvasEventType.MouseDown:
                    if (Control.MouseButtons == MouseButtons.Right)
                    {
                        eventType = enumCanvasEventType.MouseUp;
                        goto reSwitchLine;
                    }
                    if (!_isDrawing)
                    {
                        _vertexts.Clear();
                        _vertexts.Add(new Point(e.ScreenX, e.ScreenY));
                        _isDrawing = true;
                    }
                    break;
                case enumCanvasEventType.MouseMove:
                    if (_isDrawing && Control.MouseButtons == MouseButtons.Left)
                    {
                        _vertexts.Add(new Point(e.ScreenX, e.ScreenY));
                        (sender as ICanvas).Refresh(enumRefreshType.FlyLayer);
                    }
                    break;
                case enumCanvasEventType.MouseUp:
                case enumCanvasEventType.DoubleClick:
                    if (_isDrawing)
                    {
                        if (_vertexts.Count > 1)//闭合
                            _vertexts.Add(_vertexts[0]);
                        (sender as ICanvas).Refresh(enumRefreshType.FlyLayer);
                        NotifyResult((sender as ICanvas).CoordTransform, sender as ICanvas);
                        _vertexts.Clear();
                        _isDrawing = false;
                    }
                    break;
            }
        }

        private void NotifyResult(ICoordinateTransform coordTran, ICanvas canvas)
        {
            if (_pencilDrawedResultFinished == null)
                return;
            using (GraphicsPath path = GetGraphicsPath())
            {
                if (path == null)
                    return;
                GeometryOfDrawed geometry = new GeometryOfDrawed();
                geometry.ShapeType = _pencilType.ToString();
                geometry.RasterPoints = path.PathData.Points;
                //using (StreamWriter sw = new StreamWriter("F:\\1.txt", false, Encoding.Default))
                //{
                //    for (int i = 0; i < geometry.RasterPoints.Length; i++)
                //    {
                //        sw.WriteLine(geometry.RasterPoints[i].ToString());
                //    }
                //}
                float row, col;
                double prjY, prjX;
                for (int i = 0; i < geometry.RasterPoints.Length; i++)
                {
                    if (canvas.PrimaryDrawObject != null)//有活动影像是返回栅格坐标
                    {
                        coordTran.Screen2Raster(geometry.RasterPoints[i].X, geometry.RasterPoints[i].Y, out row, out col);
                        geometry.RasterPoints[i].X = col;
                        geometry.RasterPoints[i].Y = row;
                    }
                    else//无活动影像时返回投影坐标
                    {
                        canvas.CoordTransform.Screen2Prj(geometry.RasterPoints[i].X, geometry.RasterPoints[i].Y, out prjX, out prjY);
                        geometry.RasterPoints[i].X = (float)prjX;
                        geometry.RasterPoints[i].Y = (float)prjY;
                        geometry.IsPrjCoord = true;
                    }
                }
                //记录拐点(例如：海冰冰缘线绘制时使用)
                if (_pencilType == enumPencilType.ControlFreeCurve && _controls.Count > 0)
                {
                    geometry.ControlRasterPoints = new PointF[_controls.Count];
                    for (int i = 0; i < _controls.Count; i++)
                    {
                        if (canvas.PrimaryDrawObject != null)//有活动影像是返回栅格坐标
                        {
                            coordTran.Screen2Raster(_controls[i].X, _controls[i].Y, out row, out col);
                            geometry.ControlRasterPoints[i].X = col;
                            geometry.ControlRasterPoints[i].Y = row;
                        }
                        else//无活动影像时返回投影坐标
                        {
                            canvas.CoordTransform.Screen2Prj(_controls[i].X, _controls[i].Y, out prjX, out prjY);
                            geometry.ControlRasterPoints[i].X = (float)prjX;
                            geometry.ControlRasterPoints[i].Y = (float)prjY;
                            geometry.IsPrjCoord = true;
                        }
                    }
                }
                //
                geometry.Types = path.PathData.Types;
                _pencilDrawedResultFinished(geometry);
            }
        }

        private void ToRasterPointF(ref PointF pt, ICanvas canvas)
        {
            ICoordinateTransform coordTran = canvas.CoordTransform;
            if (canvas.PrimaryDrawObject != null)//有活动影像是返回栅格坐标
            {
                float row = 0, col = 0;
                coordTran.Screen2Raster(pt.X, pt.Y, out row, out col);
                pt.X = col;
                pt.Y = row;
            }
            else//无活动影像时返回投影坐标
            {
                double prjX = 0, prjY = 0;
                canvas.CoordTransform.Screen2Prj(pt.X, pt.Y, out prjX, out prjY);
                pt.X = (float)prjX;
                pt.Y = (float)prjY;
            }
        }

        private Size GetSize(ICanvas canvas)
        {
            if (canvas == null || canvas.PrimaryDrawObject == null)
                return Size.Empty;
            return canvas.PrimaryDrawObject.Size;
        }

        [Browsable(false)]
        public enumPencilType PencilType
        {
            get { return _pencilType; }
            set
            {
                _pencilType = value;
                if (_vertexts.Count > 0)
                    _vertexts.Clear();
                _controls.Clear();
            }
        }

        [Browsable(false)]
        public Color PenColor
        {
            get { return _penColor; }
            set 
            {
                if (_penColor != value)
                {
                    _penColor = value;
                    _pen = new Pen(value);
                }
            }
        }

        [Browsable(false)]
        public Action<GeometryOfDrawed> PencilIsFinished
        {
            get { return _pencilDrawedResultFinished; }
            set { _pencilDrawedResultFinished = value; }
        }

         [Browsable(false)]
        public Action<PointF> ControlPointIsAdded
        {
            get { return _controlPointIsAdded; }
            set { _controlPointIsAdded = value; }
        }

         public override void Dispose()
         {
             base.Dispose();
         }
    }
}
