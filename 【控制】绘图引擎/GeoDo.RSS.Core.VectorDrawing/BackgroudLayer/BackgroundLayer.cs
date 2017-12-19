using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;
using CodeCell.AgileMap.Core;
using System.IO;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using GeoDo.Core;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public class BackgroundLayer : Layer, IBackgroundLayer
    {
        protected bool _visible = true;
        protected SolidBrush _landBrush = new SolidBrush(Color.FromArgb(255, 255, 255));
        protected Color _seaColor = Color.FromArgb(160, 190, 228);
        protected SolidBrush _seaBrush = new SolidBrush(Color.FromArgb(160, 190, 228));
        protected List<Feature> _features = null;
        protected List<Feature> _waterBodyFeatures = new List<Feature>();
        protected GeoDo.RSS.Core.DrawEngine.ICoordinateTransform _coordTransform;
        protected string _shpFileName = AppDomain.CurrentDomain.BaseDirectory + @"数据引用\基础矢量\矢量模版\海陆模版.shp";
        protected string[] _interestRegions;
        protected GraphicsPath _graphicsPath = new GraphicsPath();

        public BackgroundLayer()
        {
            _name = _alias = "海陆背景";
            _landBrush = new SolidBrush(Color.FromArgb(255, 255, 255));
            _seaColor = Color.FromArgb(160, 190, 228);
            _seaBrush = new SolidBrush(Color.FromArgb(160, 190, 228));
        }

        public BackgroundLayer(string shpFileName)
            :this()
        {
            _shpFileName = shpFileName;
            _name = _alias = "海陆背景";
        }

        public BackgroundLayer(string shpFileName, string[] interestRegions)
            :this(shpFileName)
        {
            _interestRegions = interestRegions;
        }

        private void LoadFeatures(string shpFileName)
        {
            if (!File.Exists(shpFileName))
                throw new FileNotFoundException(shpFileName);
            //
            using (IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(shpFileName) as IVectorFeatureDataReader)
            {
                Feature[] fets = dr.FetchFeatures();
                if (fets != null || fets.Length > 0)
                    _features.AddRange(fets);
            }
            //
            TryLoadInterestRegions();
            _shpFileName = shpFileName;
        }

        private void TryLoadInterestRegions()
        {
            if (_interestRegions == null || _interestRegions.Length == 0)
                return;
            foreach (string region in _interestRegions)
            {
                string fname = GetShpFileName(region);
                if (fname != null)
                {
                    using (IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(fname) as IVectorFeatureDataReader)
                    {
                        Feature[] fets = dr.FetchFeatures();
                        if (fets != null || fets.Length > 0)
                            _waterBodyFeatures.AddRange(fets);
                    }
                }
            }
        }

        private string GetShpFileName(string region)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + @"数据引用\关注区域";
            string[] fnames = Directory.GetFiles(dir, region + ".shp", SearchOption.AllDirectories);
            if (fnames != null && fnames.Length > 0)
                return fnames[0];
            return null;
        }

        [DisplayName("是否可见"), Category("状态")]
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        [DisplayName("陆地颜色"), Category("设置"),XmlPersist(typeof(ColorPropertyConverter))]
        public Color LandColor
        {
            get
            {
                if (_landBrush == null)
                    return Color.Empty;
                return _landBrush.Color;
            }
            set
            {
                if (_landBrush != null && _landBrush.Color != value)
                {
                    _landBrush.Color = value;
                }
            }
        }

        [DisplayName("海洋颜色"), Category("设置"), XmlPersist(typeof(ColorPropertyConverter))]
        public Color SeaColor
        {
            get
            {
                return _seaColor;
            }
            set
            {
                _seaColor = value;
                if (_seaBrush != null)
                    _seaBrush.Dispose();
                _seaBrush = new SolidBrush(value);
            }
        }

        [DisplayName("矢量文件"), Category("设置"),XmlPersist(false)]
        public string ShpFileName
        {
            get { return _shpFileName; }
            set { _shpFileName = value; }
        }

        [DisplayName("关注水体"), Description("例如:太湖"), Category("关注区域"), XmlPersist("InterestRegion",typeof(string))]
        public string[] InterestRegions
        {
            get { return _interestRegions; }
            set
            {
                _interestRegions = value;
                _waterBodyFeatures.Clear();
                TryLoadInterestRegions();
            }
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            if (_coordTransform == null)
                _coordTransform = (sender as ICanvas).CoordTransform;
            ICanvas canvas = sender as ICanvas;
            TryLoadFeatures(canvas);
            Graphics g = drawArgs.Graphics as Graphics;
            //draw sea
            g.Clear(_seaColor);
            //draw land
            DrawFeatures(g, drawArgs, _landBrush, _features);
            //draw waterbody
            DrawFeatures(g, drawArgs, _seaBrush, _waterBodyFeatures);
        }

        private void TryLoadFeatures(ICanvas canvas)
        {
            if (_features != null)
                return;
            _features = new List<Feature>();
            if (IsGeoCoord(canvas))
            {
                if (GlobalCacher.VectorDataGlobalCacher != null && GlobalCacher.VectorDataGlobalCacher.IsEnabled)
                {
                    ICachedFeatures cache = GlobalCacher.VectorDataGlobalCacher.GetFeatures("海陆模版");
                    if (cache == null)
                    {
                        LoadFeatures(_shpFileName);
                        cache = new CachedFeatures("海陆模版", _features.ToArray());
                        GlobalCacher.VectorDataGlobalCacher.PutFeatures(cache);
                    }
                    else
                    {
                        _features.AddRange(cache.Features);
                    }
                }
                else
                {
                    LoadFeatures(_shpFileName);
                }
            }
            else
            {
                LoadFeatures(_shpFileName);
            }
            TryLoadInterestRegions();
        }

        private bool IsGeoCoord(ICanvas canvas)
        {
            return canvas.CoordTransform.DataCoordType == enumDataCoordType.Geo;
        }

        private void DrawFeatures(Graphics g, IDrawArgs drawArgs, Brush brush, IList<Feature> features)
        {
            if (_features == null || _features.Count == 0)
                return;
            foreach (Feature fet in features)
            {
                DrawFeature(fet, g, drawArgs, brush);
            }
        }
        
        private void DrawFeature(Feature fet, Graphics g, IDrawArgs drawArgs, Brush brush)
        {
            if (fet == null)
                return;
            if (!fet.Projected)
                ProjectFeature(fet);
            Shape geometry = fet.Geometry;
            if (geometry is ShapePolygon)
            {
                _graphicsPath.Reset();
                ShapePolygon plygon = geometry as ShapePolygon;
                foreach (ShapeRing ring in plygon.Rings)
                {
                    Point[] pts = ToScreenPoints(ring.Points, drawArgs);
                    if (pts != null && pts.Length > 3)
                        _graphicsPath.AddPolygon(pts);
                }
                g.FillPath(brush, _graphicsPath);
            }
        }

        private unsafe Point[] ToScreenPoints(ShapePoint[] shapePoints, IDrawArgs drawArgs)
        {
            Point[] pts = new Point[shapePoints.Length];
            fixed (Point* ptrPointer = pts)
            {
                Point* ptr = ptrPointer;
                double x = 0, y = 0;
                int pointCount = shapePoints.Length;
                for (int i = 0; i < pointCount; i++, ptr++)
                {
                    x = shapePoints[i].X;
                    y = shapePoints[i].Y;
                    drawArgs.QuickTransformArgs.Transform(ref x, ref y);
                    ptr->X = (int)x;
                    ptr->Y = (int)y;
                }
            }
            return pts;
        }

        private void ProjectFeature(Feature fet)
        {
            double prjX = 0, prjY = 0;
            Shape geometry = fet.Geometry;
            if (geometry is ShapePolygon)
            {
                ShapePolygon plygon = geometry as ShapePolygon;
                foreach (ShapeRing ring in plygon.Rings)
                {
                    foreach (ShapePoint pt in ring.Points)
                    {
                        _coordTransform.Geo2Prj(pt.X, pt.Y, out prjX, out prjY);
                        pt.X = prjX;
                        pt.Y = prjY;
                    }
                }
            }
            fet.Projected = true;
        }

        public override void Dispose()
        {
            if (_landBrush != null)
            {
                _landBrush.Dispose();
                _landBrush = null;
            }
            if (_seaBrush != null)
            {
                _seaBrush.Dispose();
                _seaBrush = null;
            }
            if (_graphicsPath != null)
            {
                _graphicsPath.Dispose();
                _graphicsPath = null;
            }
            if (_features != null && _features.Count != 0)
            {
                _features.Clear();
                _features = null;
            }
            _coordTransform = null;
            _waterBodyFeatures.Clear();
            base.Dispose();
        }
    }
}
