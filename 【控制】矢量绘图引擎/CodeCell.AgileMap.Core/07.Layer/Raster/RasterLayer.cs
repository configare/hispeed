using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;

namespace CodeCell.AgileMap.Core
{
    public class RasterLayer : IRasterLayer
    {
        private ScaleRange _displayScaleRange = new ScaleRange(-1,1);// new ScaleRange(205896, 1, true);
        private string _name = null;
        private IRasterClass _class = null;
        private bool _isDisposed = false;
        private bool _visible = true;
        private IFeatureRenderEnvironment _environment = null;
        private IRuntimeProjecter _prj = null;
        private const float cstMinWidthPercent = 0.1f;

        public RasterLayer()
        {
        }

        public RasterLayer(string name)
            : this()
        {
            _name = name;
        }

        public RasterLayer(string name, string filename)
            : this(name)
        {
            _class = new RasterClass(filename);
        }

        public RasterLayer(string name, IRasterClass rasterClass)
            : this(name)
        {
            _class = rasterClass;
            _class.Name = name;
        }

        internal void InternalInit(IFeatureRenderEnvironment environment, IRuntimeProjecter prj)
        {
            _environment = environment;
            _prj = prj;
        }

        public string Id
        {
            get { return _class.ID.ToString(); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public ScaleRange DisplayScaleRange
        {
            get { return _displayScaleRange; }
            set { _displayScaleRange = value; }
        }

        public IClass Class
        {
            get { return _class; }
            set { _class = value as IRasterClass; }
        }

        public bool Disposed
        {
            get { return _isDisposed; }
        }

        public bool VisibleAtScale(int scale)
        {
            if (_displayScaleRange != null && _displayScaleRange.Enable)
                return !(scale > _displayScaleRange.DisplayScaleOfMin || scale < _displayScaleRange.DisplayScaleOfMax);
            return true;
        }

        public bool IsReady
        {
            get { return true; }
        }

        public bool IsRendered
        {
            get { return false; }
        }

        public void Dispose()
        {
            _class.Dispose();
            _class = null;
        }

        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public bool IsTooSmall
        {
            get 
            {
                TryProjectEnvelopeOfClass();
                //如果影像的宽度是当前视窗的占的面积太小则不需要绘制
                if ((_class.FullEnvelope.Width / _environment.ExtentOfProjectionCoord.Width < cstMinWidthPercent) ||
                    (_class.FullEnvelope.Height / _environment.ExtentOfProjectionCoord.Height < cstMinWidthPercent))
                    return true;
                return false;
            }
        }

        private bool _isPrjed = false;
        private void TryProjectEnvelopeOfClass()
        {
            if (!_isPrjed)
            {
                _prj.Project(_class.FullEnvelope);
                _isPrjed = true;
            }
        }
        public void Render(System.Drawing.Graphics g, QuickTransformArgs quickTransform)
        {
            Envelope currentExtent = _environment.ExtentOfProjectionCoord;         
            if (!currentExtent.IsInteractived(_class.FullEnvelope))
                return;        
            IMapRuntime runtime = _environment as IMapRuntime;
            IRasterDataSource ds = _class.DataSource as IRasterDataSource;
            enumCoordinateType coordType = ds.GetCoordinateType();
            Envelope evp = null;
            if (coordType == enumCoordinateType.Geographic)
                evp = ToGeoEnvelope(runtime, currentExtent);
            Bitmap bm = ds.Reader.Read(evp, runtime.Host.CanvasSize.Width, runtime.Host.CanvasSize.Height);
            if (bm != null)
                g.DrawImage(bm, 0, 0);
        }

        private Envelope ToGeoEnvelope(IMapRuntime runtime, Envelope currentExtent)
        {
            ICoordinateTransform coord = runtime as ICoordinateTransform;
            ShapePoint[] pts = currentExtent.Points;
            coord.PrjCoord2GeoCoord(pts);
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            foreach (ShapePoint pt in pts)
            {
                if (pt.X < minX)
                    minX = pt.X;
                if (pt.Y < minY)
                    minY = pt.Y;
                if (pt.X > maxX)
                    maxX = pt.X;
                if (pt.Y > maxY)
                    maxY = pt.Y;
            }
            Envelope envelope = new Envelope();
            envelope.MinX = minX;
            envelope.MinY = minY;
            envelope.MaxX = maxX;
            envelope.MaxY = maxY;
            return envelope;
        }

        #region IPersistable Members

        public PersistObject ToPersistObject()
        {
            PersistObject obj = new PersistObject("RasterLayer");
            obj.AddAttribute("name", _name != null ? _name : string.Empty);
            obj.AddAttribute("visible", _visible.ToString());
            //displayScaleRange
            if (_displayScaleRange == null)
                _displayScaleRange = new ScaleRange(-1, 1);
            obj.AddSubNode((_displayScaleRange as IPersistable).ToPersistObject());
            //feature class
            if (_class != null)
                obj.AddSubNode((_class as IPersistable).ToPersistObject());
            return obj;
        }

        #endregion

        public static IRasterLayer FromXElement(XElement ele)
        {
            if (ele == null)
                return null;
            string name = ele.Attribute("name").Value;
            bool visible = bool.Parse(ele.Attribute("visible").Value);
            ScaleRange displayScaleRange = ScaleRange.FromXElement(ele.Element("DisplayScaleRange"));
            IRasterClass rasterclass = AgileMap.Core.RasterClass.FromXElement(ele.Element("RasterClass"));
            rasterclass.Name = name;
            IRasterLayer lyr = new RasterLayer(name, rasterclass);
            (lyr as ILayerDrawable).Visible = visible;
            return lyr;
        }

    }
}
