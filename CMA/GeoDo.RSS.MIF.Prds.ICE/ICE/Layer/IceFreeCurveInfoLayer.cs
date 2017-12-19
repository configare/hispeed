using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.MIF.Prds.ICE
{
    public class IceFreeCurveInfoLayer:Layer,IFlyLayer
    {
        public class InfoItem
        {
            public int No;
            public double Longitude;
            public double Latitude;
            public double Temperature;
            private const string STR_FORMAT = "{0}:{1},{2},  {3}";

            public override string ToString()
            {
                return string.Format(STR_FORMAT,
                    No.ToString().PadLeft(2,' '),
                    Longitude.ToString("0.00".PadLeft(8,' ')),
                    Latitude.ToString("0.00".PadLeft(8,' ')),
                    Temperature.ToString("0.00℃"));
            }
        }

        protected bool _visible = true;
        protected List<InfoItem> _infoItems = new List<InfoItem>();
        protected Font _font = new Font("宋体", 9f);
        private List<CodeCell.AgileMap.Core.Feature> _geoItems = new List<CodeCell.AgileMap.Core.Feature>();

        public IceFreeCurveInfoLayer()
            : base()
        {
            _name = "冰缘线拐点";
        }

        /// <summary>
        /// 拐点信息
        /// </summary>
        public List<InfoItem> InfoItems
        {
            get 
            { 
                return _infoItems; 
            }
        }

        /// <summary>
        /// 冰缘线
        /// </summary>
        public List<CodeCell.AgileMap.Core.Feature> GeoItems
        {
            get { return _geoItems; }
        }

        internal void AddFeature(CodeCell.AgileMap.Core.Feature feature)
        {
            _geoItems.Add(feature);
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            //左上角绘制信息列表
            if (_infoItems.Count == 0)
                return;
            Graphics g = drawArgs.Graphics as Graphics;
            float x = 50;
            float y = 10;
            SizeF fontSize = g.MeasureString("100", _font);
            int rowStep = (int)(fontSize.Height + 2);
            foreach (InfoItem item in _infoItems)
            {
                g.DrawString(item.ToString(), _font, Brushes.Yellow, x, y);
                y += rowStep;
            }

            ICanvas canvas = sender as ICanvas;
            if (_visible)
            {
                ICoordinateTransform coordTran = canvas.CoordTransform;
                QuickTransform qt = drawArgs.QuickTransformArgs;
                //绘制拐点
                foreach (InfoItem item in _infoItems)
                {
                    int row = 0, col = 0;
                    double geoX, geoY;
                    coordTran.Geo2Prj(item.Longitude, item.Latitude, out geoX, out geoY);
                    coordTran.Prj2Screen(geoX, geoY, out col, out row);
                    g.FillEllipse(Brushes.Yellow, new RectangleF(col - 3, row - 3, 6, 6));
                }
                //绘制冰缘线
                if (_geoItems != null)
                {
                    foreach (CodeCell.AgileMap.Core.Feature items in _geoItems)
                    {
                        GraphicsPath path = ToGraphicsPath(items, canvas);
                        g.DrawPath(Pens.Red, path);
                    }
                }
            }
        }

        private GraphicsPath ToGraphicsPath(CodeCell.AgileMap.Core.Feature feature, ICanvas canvas)
        {
            CodeCell.AgileMap.Core.ShapePolyline ply = feature.Geometry as CodeCell.AgileMap.Core.ShapePolyline;
            GeoDo.RSS.Core.DrawEngine.ICoordinateTransform tran = canvas.CoordTransform;
            double prjX, prjY;
            int screenX, screenY;
            GraphicsPath path = new GraphicsPath();
            foreach ( CodeCell.AgileMap.Core.ShapeLineString line in ply.Parts)
            {
                PointF[] pts = new PointF[line.Points.Length];
                for (int i = 0; i < pts.Length; i++)
                {
                    if (!feature.Projected)
                        tran.Geo2Prj(line.Points[i].X, line.Points[i].Y, out prjX, out prjY);
                    else
                    {
                        prjX = line.Points[i].X;
                        prjY = line.Points[i].Y;
                    }
                    tran.Prj2Screen(prjX, prjY, out screenX, out screenY);
                    pts[i].X = screenX;
                    pts[i].Y = screenY;
                }
                path.AddCurve(pts.ToArray());
                //path.AddLines();
                path.StartFigure();
            }
            return path;
        }

        //private unsafe void PolylineToPath(CodeCell.AgileMap.Core.ShapePolyline ply)
        //{
        //    int partCount = ply.Parts.Length;
        //    for (int pi = 0; pi < partCount; pi++)
        //    {
        //        CodeCell.AgileMap.Core.ShapePoint[] prjPts = ply.Parts[pi].Points;
        //        PointF[] ptfs = new PointF[prjPts.Length];
        //        for (int i = 0; i < ptfs.Length; i++)
        //        {
        //            ptfs[i].X = (float)(_quickTransformArgs.kLon * prjPts[i].X + _quickTransformArgs.bLon);
        //            ptfs[i].Y = (float)(_quickTransformArgs.kLat * prjPts[i].Y + _quickTransformArgs.bLat);
        //        }
        //        _path.AddLines(ptfs);
        //        _path.StartFigure();
        //    }
        //}

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public void Event(object sender, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
        }

        public override void Dispose()
        {
            if (_font != null)
            {
                _font.Dispose();
                _font = null;
            }
        }
    }
}
