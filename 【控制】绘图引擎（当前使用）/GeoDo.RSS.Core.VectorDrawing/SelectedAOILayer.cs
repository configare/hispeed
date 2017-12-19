using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.IO;
using CodeCell.AgileMap.Core;
using System.ComponentModel;
using System.Windows.Forms;

namespace GeoDo.RSS.Core.VectorDrawing
{

    public class SelectedAOILayer : Layer, IRenderLayer, ISelectedAOILayer, IAOILayer, IControlLayer
    {
        protected bool _visible = true;
        protected bool _edit = false;
        protected List<object> _geometrys = new List<object>();
        protected IAOIContainerLayer _aoiContainerLayer = null;
        protected Pen _pen;
        protected float _lineWidth = 1;
        protected const int EDIT_BOX_HALF_WIDTH = 6;
        protected const int EDIT_BOX_WIDTH = 12;
        //
        class EditObject
        {
            public bool IsSelected = false;
        }
        //
        enum enumEditAction
        {
            DragObj,
            DrawVertext
        }

        public SelectedAOILayer()
            : base()
        {
            _name = _alias = "选中的感兴趣区域";
            _pen = new Pen(Color.Yellow, _lineWidth);
            _pen.DashStyle = DashStyle.Dash;
        }

        [DisplayName("是否显示"), Category("状态")]
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        [DisplayName("线颜色"), Category("绘制设置")]
        public Color Color
        {
            get { return _pen.Color; }
            set
            {
                _pen.Dispose();
                _pen = new Pen(value, _lineWidth);
            }
        }

        [DisplayName("线宽"), Category("绘制设置")]
        public float LineWidth
        {
            get { return _lineWidth; }
            set
            {
                if (value < 1 || value > 5)
                    return;
                _lineWidth = value;
                Color c = _pen.Color;
                _pen.Dispose();
                _pen = new Pen(c, value);
            }
        }

        [DisplayName("编辑"), Category("绘制设置")]
        public bool Edit
        {
            get { return _edit; }
            set { _edit = value; }
        }

        [Browsable(false)]
        public object FirstAOI
        {
            get
            {
                if (_geometrys == null || _geometrys.Count == 0)
                    return null;
                return _geometrys[0];
            }
        }

        [Browsable(false)]
        public IEnumerable<object> AOIs
        {
            get { return _geometrys; }
        }

        public IAOIContainerLayer AOIContaingerLayer
        {
            get { return _aoiContainerLayer; }
            set { _aoiContainerLayer = value; }
        }

        public void Reset()
        {
            _geometrys.Clear();
        }

        public void AddSelectedAOI(object geometry)
        {
            _geometrys.Add(geometry);
        }

        public void RemoveSelectedAOI(object[] geometry)
        {
            if (_geometrys == null || _geometrys.Count == 0)
                return;
            foreach (object item in geometry)
            {
                if (_geometrys.Contains(item))
                    _geometrys.Remove(item);
            }
        }

        public void RemoveAOI(int index)
        {
            _geometrys.RemoveAt(index);
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            if (_geometrys.Count == 0)
                return;
            Graphics g = drawArgs.Graphics as Graphics;
            ICanvas canvas = sender as ICanvas;
            foreach (object geo in _geometrys)
            {
                GraphicsPath path = ToGraphicsPath(geo, canvas);
                if (path == null)
                    continue;

                if ((geo is GeometryOfDrawed) && (geo as GeometryOfDrawed).ShapeType == "Cross")
                {
                    g.DrawLine(_pen, new PointF(path.PathPoints[0].X, path.PathPoints[0].Y), new PointF(path.PathPoints[2].X, path.PathPoints[2].Y));
                    g.DrawLine(_pen, new PointF(path.PathPoints[1].X, path.PathPoints[1].Y), new PointF(path.PathPoints[3].X, path.PathPoints[3].Y));
                }
                else
                    g.DrawPath(_pen, path);
            }
        }

        private GraphicsPath ToGraphicsPath(object obj, ICanvas canvas)
        {
            if (obj is GeometryOfDrawed)
                return ToGraphicsPath(obj as GeometryOfDrawed, canvas);
            else if (obj is Feature)
                return ToGraphicsPath(obj as Feature, canvas);
            return null;
        }

        private GraphicsPath ToGraphicsPath(GeometryOfDrawed geo, ICanvas canvas)
        {
            if (geo == null || geo.RasterPoints.Length == 0)
                return null;
            float x, y;
            int ix, iy;
            int count = geo.RasterPoints.Length;
            PointF[] pts = pts = new PointF[count];
            for (int i = 0; i < count; i++)
            {
                if (geo.IsPrjCoord)
                {
                    canvas.CoordTransform.Prj2Screen(geo.RasterPoints[i].X, geo.RasterPoints[i].Y, out ix, out iy);
                    pts[i].X = ix;
                    pts[i].Y = iy;
                }
                else
                {
                    canvas.CoordTransform.Raster2Screen(geo.RasterPoints[i].Y, geo.RasterPoints[i].X, out x, out y);
                    pts[i].X = x;
                    pts[i].Y = y;
                }
            }

            if ((geo is GeometryOfDrawed) && (geo as GeometryOfDrawed).ShapeType == "Cross")
            {
                PointF[] vertexts;
                if ((geo as GeometryOfDrawed).ControlRasterPoints.Count() == 1)
                {
                    vertexts = new PointF[]{ new PointF(pts[0].X-50, pts[0].Y), new PointF(pts[0].X, pts[0].Y-50), 
                                                      new PointF(pts[0].X+50, pts[0].Y), new PointF(pts[0].X, pts[0].Y+50)};
                }
                else
                {
                    vertexts = new PointF[]{ new PointF(pts[0].X-10, pts[0].Y), new PointF(pts[0].X, pts[0].Y-10), 
                                                      new PointF(pts[0].X+10, pts[0].Y),  new PointF(pts[0].X, pts[0].Y+10)};
                }
                return new GraphicsPath(vertexts, geo.Types);
            }

            return new GraphicsPath(pts.Take(count).ToArray(), geo.Types);
        }

        private GraphicsPath ToGraphicsPath(Feature feature, ICanvas canvas)
        {
            if (feature == null)
                return null;
            ShapePolygon ply = feature.Geometry as ShapePolygon;
            if (ply == null || ply.Rings == null || ply.Rings.Length == 0)
                return null;
            GeoDo.RSS.Core.DrawEngine.ICoordinateTransform tran = canvas.CoordTransform;
            double prjX, prjY;
            int screenX, screenY;
            GraphicsPath path = new GraphicsPath();
            foreach (ShapeRing ring in ply.Rings)
            {
                PointF[] pts = new PointF[ring.Points.Length];
                if (pts == null || pts.Length == 0)
                    continue;
                for (int i = 0; i < pts.Length; i++)
                {
                    if (!feature.Projected)
                        tran.Geo2Prj(ring.Points[i].X, ring.Points[i].Y, out prjX, out prjY);
                    else
                    {
                        prjX = ring.Points[i].X;
                        prjY = ring.Points[i].Y;
                    }
                    tran.Prj2Screen(prjX, prjY, out screenX, out screenY);
                    pts[i].X = screenX;
                    pts[i].Y = screenY;
                }
                path.AddPolygon(pts.ToArray());
            }
            return path;
        }

        public override void Dispose()
        {
            _geometrys.Clear();
            base.Dispose();
        }

        public void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
            if (!_edit)
                return;
            if (_aoiContainerLayer == null || _aoiContainerLayer.AOIs == null || _aoiContainerLayer.AOIs.Count() == 0)
                return;
            switch (eventType)
            {
                case enumCanvasEventType.MouseDown:
                    object[] selectedObj = FindSelectedObject(sender, e);
                    if (selectedObj != null)
                        foreach (object obj in selectedObj)
                        {
                            if (_geometrys.Contains(obj))
                                _geometrys.Remove(obj);
                            else
                                _geometrys.Add(obj);
                        }
                    (sender as ICanvas).Refresh(enumRefreshType.All);
                    break;
                case enumCanvasEventType.MouseMove:
                    break;
                case enumCanvasEventType.MouseUp:
                    break;
            }
        }

        private object[] FindSelectedObject(object sender, DrawingMouseEventArgs e)
        {
            object[] aios = _aoiContainerLayer.AOIs == null ? null : _aoiContainerLayer.AOIs.ToArray();
            List<object> objs = new List<object>();
            Point pt = new Point(e.ScreenX, e.ScreenY);
            ICanvas canvas = sender as ICanvas;
            foreach (object obj in aios)
            {
                GraphicsPath path = ToGraphicsPath(obj, canvas);
                if (path == null)
                    continue;
                //鼠标点击在几何形状内部
                if (path.IsVisible(new Point(e.ScreenX, e.ScreenY)))
                    objs.Add(obj);
            }
            return objs.Count == 0 ? null : objs.ToArray();
        }
    }
}
