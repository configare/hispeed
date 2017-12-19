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

    public class AOIContainerLayer : Layer, IRenderLayer, IAOIContainerLayer, IAOILayer, IControlLayer
    {
        protected bool _isOnlyOneAOI = false;
        protected bool _visible = true;
        protected List<object> _geometrys = new List<object>();
        private Dictionary<object, EditObject> _editObjects = new Dictionary<object, EditObject>();
        protected Pen _pen;
        protected float _lineWidth = 1;
        protected bool _isAllowEdit = false;
        protected const int EDIT_BOX_HALF_WIDTH = 6;
        protected const int EDIT_BOX_WIDTH = 12;
        private enumEditAction _editAction = enumEditAction.DragObj;
        private EditObject _currentEditObj = null;
        private int _crtVertextIndex = -1;
        private Point _prePoint;
        private AOIGeometryIsUpdatedHandler _aoiGeometryIsUpdated;
        //
        class EditObject
        {
            public bool IsSelected = false;
            public GraphicsPath Path;
            public object Geometry;
        }
        //
        enum enumEditAction
        {
            DragObj,
            DrawVertext
        }

        public AOIContainerLayer()
            : base()
        {
            _name = _alias = "感兴趣区域";
            _pen = new Pen(Color.Yellow, _lineWidth);
            _pen.DashStyle = DashStyle.Dash;
            //_pen.DashPattern = new float[] { 5, 5 };
        }

         [DisplayName("只允许一个AOI"), Category("状态")]
        public bool IsOnlyOneAOI
        {
            get { return _isOnlyOneAOI; }
            set { _isOnlyOneAOI = value; }
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
                //_pen.DashPattern = new float[] { 5, 5 };
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
                //_pen.DashPattern = new float[] { 5, 5 };
            }
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

        [Browsable(false)]
        public bool IsAllowEdit
        {
            get { return _isAllowEdit; }
            set { _isAllowEdit = value; }
        }

        [Browsable(false)]
        public AOIGeometryIsUpdatedHandler AOIGeometryIsUpdated
        {
            get { return _aoiGeometryIsUpdated; }
            set { _aoiGeometryIsUpdated = value; }
        }

        public void Reset()
        {
            _geometrys.Clear();
            _editObjects.Clear();
        }

        public void AddAOI(object geometry)
        {
            if (_isOnlyOneAOI)
                Reset();
            _geometrys.Add(geometry);
        }

        public void RemoveAOI(int index)
        {
            _geometrys.RemoveAt(index);
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
                //
                EditObject eobj;
                if (!_editObjects.ContainsKey(geo))
                {
                    eobj = new EditObject();
                    eobj.Geometry = geo;
                    _editObjects.Add(geo, eobj);
                }
                else
                    eobj = _editObjects[geo];
                if (eobj.Path != null)
                    eobj.Path.Dispose();
                eobj.Path = path;
                //
                if (_isAllowEdit)
                {
                    Brush boxBrush = eobj.IsSelected ? Brushes.Green : Brushes.Red;

                    if ((geo is GeometryOfDrawed) && (geo as GeometryOfDrawed).ShapeType == "Cross")
                    {
                        for (int i = 0; i < path.PointCount; i++)
                        {
                            g.FillRectangle(boxBrush,
                                path.PathPoints[i].X - EDIT_BOX_HALF_WIDTH/2,
                                path.PathPoints[i].Y - EDIT_BOX_HALF_WIDTH/2,
                                EDIT_BOX_HALF_WIDTH, EDIT_BOX_HALF_WIDTH);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < path.PointCount; i++)
                        {
                            g.FillRectangle(boxBrush,
                                path.PathPoints[i].X - EDIT_BOX_HALF_WIDTH,
                                path.PathPoints[i].Y - EDIT_BOX_HALF_WIDTH,
                                EDIT_BOX_WIDTH, EDIT_BOX_WIDTH);
                        }
                    }
                }
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

        public void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
            if (!_isAllowEdit)
                return;
            switch (eventType)
            {
                case enumCanvasEventType.MouseDown:
                    EditObject eobj = FindEditObject(sender,e);
                    if (eobj != null)
                    {
                        _prePoint = new Point(e.ScreenX, e.ScreenY);
                        _currentEditObj = eobj;
                    }
                    else
                        _currentEditObj = null;
                    (sender as ICanvas).Refresh(enumRefreshType.All);
                    break;
                case enumCanvasEventType.MouseMove:
                    if (_currentEditObj != null && Control.MouseButtons == MouseButtons.Left)
                    {
                        if (_editAction == enumEditAction.DragObj )
                        {
                            DoDrag(sender,e.ScreenX - _prePoint.X, e.ScreenY - _prePoint.Y);
                            _prePoint = new Point(e.ScreenX, e.ScreenY);
                        }
                        else if (_editAction == enumEditAction.DrawVertext)
                        {
                            DoDragVertext(sender, e.ScreenX - _prePoint.X, e.ScreenY - _prePoint.Y);
                            _prePoint = new Point(e.ScreenX, e.ScreenY);
                        }
                    }
                    break;
                case enumCanvasEventType.MouseUp:
                    _prePoint = Point.Empty;
                    _currentEditObj = null;
                    _crtVertextIndex = -1;
                    break;
            }
        }

        private void DoDragVertext(object sender, int offsetX, int offsetY)
        {
            if (_currentEditObj == null)
                return;
            //
            if (_currentEditObj.Geometry is GeometryOfDrawed)
                DoDragVertext(sender, _currentEditObj.Geometry as GeometryOfDrawed, offsetX, offsetY);
            else if (_currentEditObj.Geometry is Feature)
                DoDragVertext(sender, _currentEditObj.Geometry as Feature, offsetX, offsetY);
            //
            if (_aoiGeometryIsUpdated != null)
                _aoiGeometryIsUpdated(this, _currentEditObj.Geometry);
            //
            (sender as ICanvas).Refresh(enumRefreshType.All);
        }

        private void DoDragVertext(object sender, Feature feature, int offsetX, int offsetY)
        {
            //
        }

        private void DoDragVertext(object sender, GeometryOfDrawed geometry, int offsetX, int offsetY)
        {
            ICanvas canvas = sender as ICanvas;
            float scaleX = (float)(canvas.CurrentEnvelope.Width / canvas.Container.Width);
            float scaleY = (float)(canvas.CurrentEnvelope.Height / canvas.Container.Height);
            float dltX = offsetX * scaleX;
            float dltY = -offsetY * scaleY;
            if (geometry.ShapeType == "Rectangle")
            {
                if (geometry.IsPrjCoord)
                {
                    switch (_crtVertextIndex)
                    {
                        case 0:
                            geometry.RasterPoints[0].X += dltX;
                            geometry.RasterPoints[3].X += dltX;
                            geometry.RasterPoints[0].Y += dltY;
                            geometry.RasterPoints[1].Y += dltY;
                            break;
                        case 1:
                            geometry.RasterPoints[1].X += dltX;
                            geometry.RasterPoints[2].X += dltX;
                            geometry.RasterPoints[0].Y += dltY;
                            geometry.RasterPoints[1].Y += dltY;
                            break;
                        case 2:
                            geometry.RasterPoints[1].X += dltX;
                            geometry.RasterPoints[2].X += dltX;
                            geometry.RasterPoints[3].Y += dltY;
                            geometry.RasterPoints[2].Y += dltY;
                            break;
                        case 3:
                            geometry.RasterPoints[0].X += dltX;
                            geometry.RasterPoints[3].X += dltX;
                            geometry.RasterPoints[3].Y += dltY;
                            geometry.RasterPoints[2].Y += dltY;
                            break;
                    }
                }
            }
        }

        private void DoDrag(object sender, int offsetX, int offsetY)
        {
            if (_currentEditObj == null)
                return;
            //
            if (_currentEditObj.Geometry is GeometryOfDrawed)
                DoDrag(sender, _currentEditObj.Geometry as GeometryOfDrawed, offsetX, offsetY);
            else if (_currentEditObj.Geometry is Feature)
                DoDrag(sender, _currentEditObj.Geometry as Feature, offsetX, offsetY);
            //
            if (_aoiGeometryIsUpdated != null)
                _aoiGeometryIsUpdated(this, _currentEditObj.Geometry);
            //
            (sender as ICanvas).Refresh(enumRefreshType.All);
        }

        private void DoDrag(object sender, Feature feature, int offsetX, int offsetY)
        {
            ICanvas canvas = sender as ICanvas;
        }

        private void DoDrag(object sender, GeometryOfDrawed geometry, int offsetX, int offsetY)
        {
            ICanvas canvas = sender as ICanvas;
            float scaleX = (float)(canvas.CurrentEnvelope.Width / canvas.Container.Width);
            float scaleY = (float)(canvas.CurrentEnvelope.Height / canvas.Container.Height);
            float dltX = offsetX * scaleX;
            float dltY = -offsetY * scaleY;
            if (geometry.IsPrjCoord)
            {
                for (int i = 0; i < geometry.RasterPoints.Length; i++)
                {
                    geometry.RasterPoints[i].X += dltX;
                    geometry.RasterPoints[i].Y += dltY;
                }
            }
            else
            {
                float screenX_origin = 0;
                float screenY_origin = 0;
                float screenX_other = 0;
                float screenY_other = 0;
                canvas.CoordTransform.Raster2Screen(0, 0, out screenX_origin, out screenY_origin);
                canvas.CoordTransform.Raster2Screen(100, 100, out screenX_other, out screenY_other);
                scaleX = (float)(100 / (screenX_other - screenX_origin));
                scaleY = (float)(100 / (screenY_other - screenY_origin));
                dltX = offsetX * scaleX;
                dltY = offsetY * scaleY; 

                for (int i = 0; i < geometry.RasterPoints.Length; i++)
                {
                    geometry.RasterPoints[i].X += dltX;
                    geometry.RasterPoints[i].Y += dltY;
                }
            }
        }

        private EditObject FindEditObject(object sender, DrawingMouseEventArgs e)
        {
            Point pt = new Point(e.ScreenX, e.ScreenY);
            EditObject returnObj = null;
            foreach (object obj in _editObjects.Keys)
            {
                EditObject eobj = _editObjects[obj];
                if (eobj.Path == null)
                    continue;
                //鼠标点击的顶点
                for (int i = 0; i < eobj.Path.PointCount; i++)
                {
                    RectangleF rect =new RectangleF(eobj.Path.PathPoints[i].X - EDIT_BOX_HALF_WIDTH,
                            eobj.Path.PathPoints[i].Y - EDIT_BOX_HALF_WIDTH,
                            EDIT_BOX_WIDTH, EDIT_BOX_WIDTH);
                    if (rect.Contains(pt))
                    {
                        _crtVertextIndex = i;
                        eobj.IsSelected = true;
                        returnObj = eobj;
                        _editAction = enumEditAction.DrawVertext;
                        goto endLine;
                    }
                }
                //鼠标点击在几何形状内部
                if (eobj.Path.IsVisible(new Point(e.ScreenX, e.ScreenY)))
                {
                    eobj.IsSelected = true;
                    returnObj = eobj;
                    _editAction = enumEditAction.DragObj;
                }
                else
                {
                    eobj.IsSelected = false;
                }
            }
            endLine:
            return returnObj;
        }

        public override void Dispose()
        {
            _geometrys.Clear();
            _editObjects.Clear();
            base.Dispose();
        }
    }
}
