using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;
using System.Drawing;
using System.IO;
using System.ComponentModel;
using AgileMap = CodeCell.AgileMap.Core;
using System.Xml.Linq;
using GeoDo.Core;
using System.Drawing.Imaging;

namespace GeoDo.RSS.Core.VectorDrawing
{
    [XmlConstructor(new string[] { "SpatialRef" })]
    public class VectorHostLayer : Layer, IVectorLayer, IVectorHostLayer,
        IMapRuntimeHost, IProjectionTransform, IAsyncDataArrivedNotify
    {
        private bool _visible = true;
        private IMapRuntime _mapRuntime;
        private string _mcdfile;
        private ICanvas _canvas;
        private RenderArgs _renderArgs;
        private GeoDo.RSS.Core.DrawEngine.ICoordinateTransform _coordinateTranform;
        private EventHandler _onCanvasSizeChanged = null;
        private EventHandler _someDataIsArrivedHandler = null;
        private string _canvasSpatialRef;
        private bool _isEnableDummyRender = true;

        public VectorHostLayer(string canvasSpatialRef)
        {
            _name = _alias = "矢量组";
            _canvasSpatialRef = canvasSpatialRef;
            CreateMapRuntime();
        }

        public VectorHostLayer(string canvasSpatialRef, string mcdfile)
            : this(canvasSpatialRef)
        {
            _mcdfile = mcdfile;
        }

        private void CreateMapRuntime()
        {
            _isDisposed = false;
            _renderArgs = new RenderArgs();
            _mapRuntime = new MapRuntime(this);
            _mapRuntime.ScaleBarArgs.Enabled = false;
            _mapRuntime.AsyncDataArrivedNotify = this;
            _mapRuntime.CanvasSpatialRef = _canvasSpatialRef;
        }

        public void Apply(string mcdfile)
        {
            if (_mapRuntime == null || _isDisposed)
                CreateMapRuntime();
            if (mcdfile != null && File.Exists(mcdfile))
                _mapRuntime.Apply(MapFactory.LoadMapFrom(mcdfile));
            else
                _mapRuntime.Apply(new Map());
            _mapRuntime.RuntimeExchanger.AutoRefreshWhileFinishOneGrid = false;
        }

        [Browsable(false), XmlPersist(false)]
        public EventHandler SomeDataIsArrivedHandler
        {
            get { return _someDataIsArrivedHandler; }
            set { _someDataIsArrivedHandler = value; }
        }

        [Browsable(false), XmlPersist(typeof(MapPropertyConverter))]
        public object Map
        {
            get
            {
                if (_mapRuntime != null)
                    return _mapRuntime.Map;
                return null;
            }
        }

        [Browsable(false), XmlPersist(false)]
        public object MapRuntime
        {
            get { return _mapRuntime; }
        }

        [DisplayName("是否可见"), Category("状态")]
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        public void ClearAll()
        {
            if (Map == null)
                return;
            (Map as IMap).LayerContainer.Clear(true);
        }

        private bool _isDisposed = false;

        public void AddData(string fname, object arguments)
        {
            AgileMap.ILayer lyr = LoadFeatureLayer(fname, arguments);
            if (lyr != null)
            {
                //AppFeatureAttribute(lyr);
                if (_mapRuntime == null || _isDisposed)
                    CreateMapRuntime();
                if (_mapRuntime.Map == null)
                    _mapRuntime.Apply(new Map()); ;
                _mapRuntime.Map.LayerContainer.Append(lyr);
            }
        }

        private AgileMap.ILayer LoadFeatureLayer(string shpFname, object arguments)
        {
            AgileMap.ILayer lyr = null;//IFeatureLayer
            //lyr = TryLoadFeatureLayerFromMcd(shpFname);
            //if (lyr == null)
            lyr = VectorLayerFactory.CreateFeatureLayer(shpFname, arguments);
            TryApplyStyle(lyr, shpFname);
            return lyr;
        }

        /// <summary>
        /// 设置标注和矢量的绘制风格
        /// </summary>
        /// <param name="lyr"></param>
        /// <param name="shpFname"></param>
        private void TryApplyStyle(AgileMap.ILayer lyr, string shpFname)
        {
            string mcdfname = Path.ChangeExtension(shpFname, ".mcd");
            if (!File.Exists(mcdfname))
                return;
            XDocument doc = XDocument.Load(mcdfname);
            var result = doc.Element("Map").Element("Layers").Elements();
            if (result == null)
                return;
            LabelDef labelDef = null;
            IFeatureRenderer featureRender = null;
            GetFeatureRenderFromMcd(doc, out labelDef, out featureRender);
            if (labelDef != null)
                (lyr as CodeCell.AgileMap.Core.IFeatureLayer).LabelDef = labelDef;
            if (featureRender != null)
                (lyr as CodeCell.AgileMap.Core.IFeatureLayer).Renderer = featureRender;
        }

        private void GetFeatureRenderFromMcd(XDocument doc, out LabelDef labelDef, out  IFeatureRenderer featureRender)
        {
            labelDef = null;
            featureRender = null;
            try
            {
                var result = doc.Element("Map").Element("Layers");
                if (result == null)
                    return;
                var layerXmls = result.Elements("Layer");
                if (layerXmls == null || layerXmls.Count() == 0)
                    return;
                var layerXml = layerXmls.First();

                var labelXmls = layerXml.Elements("LabelDef");
                if (labelXmls != null && labelXmls.Count() != 0)
                {
                    XElement labelXml = labelXmls.First();
                    labelDef = LabelDef.FromXElement(labelXml);
                }

                var renderXmls = layerXml.Elements("Renderer");
                if (renderXmls != null && renderXmls.Count() != 0)
                {
                    XElement renderXml = renderXmls.First();
                    XAttribute renderTypeAtt = renderXml.Attribute("type");
                    if (renderTypeAtt != null)
                    {
                        string renderType = renderTypeAtt.Value;
                        if (renderType == "CodeCell.AgileMap.Core.dll,CodeCell.AgileMap.Core.SimpleFeatureRenderer")
                            featureRender = SimpleFeatureRenderer.FromXElement(renderXml);
                        else if (renderType == "CodeCell.AgileMap.Core.dll,CodeCell.AgileMap.Core.SimpleTwoStepFeatureRenderer")
                            featureRender = SimpleTwoStepFeatureRenderer.FromXElement(renderXml);
                        else if (renderType == "CodeCell.AgileMap.Core.dll,CodeCell.AgileMap.Core.UniqueFeatureRenderer")
                            featureRender = UniqueFeatureRenderer.FromXElement(renderXml);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ChangeMap(string mcdfname)
        {
            DisposeMap();
            IMap map = MapFactory.LoadMapFrom(mcdfname);
            if (map == null)
                return;
            if (_mapRuntime == null || _isDisposed)
                CreateMapRuntime();
            _mapRuntime.Apply(map);
        }

        public bool IsEnableDummyRender
        {
            get { return _isEnableDummyRender; }
            set { _isEnableDummyRender = value; }
        }

        public void Render(object sender, IDrawArgs drawArgs)
        {
            if (_mapRuntime == null)
                return;
            if (_canvas == null)
            {
                _canvas = sender as ICanvas;
                Init();
            }
            else
            {
                if (_coordinateTranform == null)
                    _coordinateTranform = _canvas.CoordTransform;
            }
            _renderArgs.BeginRender(drawArgs.Graphics as Graphics);
            try
            {
                if (_isEnableDummyRender)
                    DoRender(_renderArgs);
                else
                    DirectRender(_renderArgs);
            }
            finally
            {
                _renderArgs.EndRender();
            }
        }

        private Bitmap _buffer;
        private bool _isUseDummyMap = true;
        private CoordEnvelope _preEnvelope;
        private Size _preSize;

        private void DoRender(RenderArgs args)
        {
            _isUseDummyMap = _canvas.DummyRenderModeSupport.IsDummyModel;
            if (_buffer == null || _preSize.Width != _canvas.Container.Width || _preSize.Height != _canvas.Container.Height)
            {
                if (_buffer != null)
                    _buffer.Dispose();
                _buffer = new Bitmap(_canvas.Container.Width, _canvas.Container.Height, PixelFormat.Format32bppArgb);
                _preSize = _canvas.Container.Size;
                _isUseDummyMap = false;
            }
            if (_isUseDummyMap)
            {
                RenderUseDummyMap(args, _buffer, _preEnvelope);
            }
            else
            {
                using (Graphics g = Graphics.FromImage(_buffer))
                {
                    g.Clear(Color.Transparent);
                    Graphics og = args.Graphics;
                    args.BeginRender(g);
                    DirectRender(args);
                    _preEnvelope = _canvas.CurrentEnvelope.Clone();
                    og.DrawImage(_buffer, 0, 0);
                }
            }
        }

        private void DirectRender(RenderArgs args)
        {
            if (_mapRuntime.Map.LayerContainer.Layers != null)
            {
                //旋转所有标注
                int angle = _canvas.IsReverseDirection ? 180 : 0;
                foreach (CodeCell.AgileMap.Core.ILayer lyr in _mapRuntime.Map.LayerContainer.Layers)
                    if (lyr is CodeCell.AgileMap.Core.IFeatureLayer)
                        (lyr as CodeCell.AgileMap.Core.IFeatureLayer).LabelDef.Angle = angle;
            }
            _mapRuntime.Render(args);
        }

        private void RenderUseDummyMap(RenderArgs args, Bitmap buffer, CoordEnvelope evp)
        {
            double x1 = evp.MinX;
            double y1 = evp.MinY;
            double x2 = evp.MaxX;
            double y2 = evp.MaxY;
            QuickTransform tran = _canvas.CoordTransform.QuickTransform;
            tran.Transform(ref x1, ref y1);
            tran.Transform(ref x2, ref y2);
            (args.Graphics as Graphics).DrawImage(buffer,
                RectangleF.FromLTRB((float)Math.Min(x1, x2), (float)Math.Min(y1, y2), (float)Math.Max(x1, x2), (float)Math.Max(y1, y2)));
        }

        private void Init()
        {
            _coordinateTranform = _canvas.CoordTransform;
            TrySetSpatialRef(_canvas, _mapRuntime);
            Apply(_mcdfile);
        }

        private void TrySetSpatialRef(ICanvas canvas, IMapRuntime mapRuntime)
        {
            if (canvas.PrimaryDrawObject == null || canvas.PrimaryDrawObject.SpatialRef == null)
                return;
            mapRuntime.CanvasSpatialRef = canvas.PrimaryDrawObject.SpatialRef;
        }

        public void Set(object canvas)
        {
            _canvas = canvas as ICanvas;
            if (_canvas != null)
                Init();
        }

        #region AgileMap MapRuntimeHost

        [Browsable(false), XmlPersist(false)]
        public System.Drawing.Size CanvasSize
        {
            get
            {
                if (_canvas == null || _canvas.Container == null)
                    return Size.Empty;
                return _canvas.Container.Size;
            }
        }

        [DisplayName("坐标类型"), Category("基本信息"), XmlPersist(false)]
        public enumCoordinateType CoordinateType
        {
            get { return enumCoordinateType.Projection; }
        }

        public void DoBeginInvoke(OnRenderIsFinishedHandler notify)
        {
            throw new NotImplementedException();
        }

        [Browsable(false), XmlPersist(false)]
        public Envelope ExtentGeo
        {
            get { return null; }
        }

        [DisplayName("视窗范围"), Category("当前视图"), XmlPersist(false)]
        public Envelope FocusEnvelope
        {
            get
            {
                return new Envelope(_canvas.CurrentEnvelope.MinX, -_canvas.CurrentEnvelope.MaxY, _canvas.CurrentEnvelope.MaxX, -_canvas.CurrentEnvelope.MinY);
            }
        }

        [Browsable(false), XmlPersist(false)]
        public ILocationService LocationService
        {
            get { return null; }
        }

        [Browsable(false), XmlPersist(false)]
        public EventHandler OnCanvasSizeChanged
        {
            get { return _onCanvasSizeChanged; }
            set { _onCanvasSizeChanged = value; }
        }

        [Browsable(false), XmlPersist(false)]
        public IProjectionTransform ProjectionTransform
        {
            get { return this; }
        }

        public void RefreshContainer()
        {
            throw new NotImplementedException();
        }

        [Browsable(false), XmlPersist(false)]
        public bool UseDummyMap
        {
            get
            {
                return false;
                //if (_canvas != null)
                //    return _canvas.DummyRenderModeSupport.IsDummyModel;
                //return false;
            }
        }

        public void ReRender(OnRenderIsFinishedHandler finishNotify)
        {
            //drawing
            throw new NotImplementedException();
        }

        public void ReRender()
        {
            //throw new NotImplementedException();
        }

        public void Render(OnRenderIsFinishedHandler finishNotify)
        {
            throw new NotImplementedException();
        }

        public void Render()
        {
            Render(null);
        }

        #endregion AgileMap MapRuntimeHost

        #region IProjectionTransfrom

        //prj=>geo
        void IProjectionTransform.InverTransform(ShapePoint pt)
        {
            double geoX, geoY;
            _coordinateTranform.Prj2Geo(pt.X, pt.Y, out geoX, out geoY);
            pt.X = geoX;
            pt.Y = geoY;
        }

        void IProjectionTransform.InverTransform(ref PointF pt)
        {
            double geoX, geoY;
            _coordinateTranform.Prj2Geo(pt.X, pt.Y, out geoX, out geoY);
            pt.X = (float)geoX;
            pt.Y = (float)geoY;
        }

        void IProjectionTransform.InverTransform(PointF[] points)
        {
            double geoX, geoY;
            for (int i = 0; i < points.Length; i++)
            {
                _coordinateTranform.Prj2Geo(points[i].X, points[i].Y, out geoX, out geoY);
                points[i].X = (float)geoX;
                points[i].Y = (float)geoY;
            }
        }

        void IProjectionTransform.InverTransform(ShapePoint[] points)
        {
            double geoX, geoY;
            for (int i = 0; i < points.Length; i++)
            {
                _coordinateTranform.Prj2Geo(points[i].X, points[i].Y, out geoX, out geoY);
                points[i].X = geoX;
                points[i].Y = geoY;
            }
        }

        //geo=>prj
        void IProjectionTransform.Transform(ShapePoint pt)
        {
            double prjX, prjY;
            _coordinateTranform.Geo2Prj(pt.X, pt.Y, out prjX, out prjY);
            pt.X = prjX;
            pt.Y = prjY;
        }

        void IProjectionTransform.Transform(ref PointF pt)
        {
            double prjX, prjY;
            _coordinateTranform.Geo2Prj(pt.X, pt.Y, out prjX, out prjY);
            pt.X = (float)prjX;
            pt.Y = (float)prjY;
        }

        void IProjectionTransform.Transform(PointF[] points)
        {
            double prjX, prjY;
            for (int i = 0; i < points.Length; i++)
            {
                _coordinateTranform.Geo2Prj(points[i].X, points[i].Y, out prjX, out prjY);
                points[i].X = (float)prjX;
                points[i].Y = (float)prjY;
            }
        }

        void IProjectionTransform.Transform(ShapePoint[] points)
        {
            double prjX, prjY;
            for (int i = 0; i < points.Length; i++)
            {
                _coordinateTranform.Geo2Prj(points[i].X, points[i].Y, out prjX, out prjY);
                points[i].X = prjX;
                points[i].Y = prjY;
            }
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        #endregion

        private delegate void StrongRefreshHandler();
        private StrongRefreshHandler _strongRefreshHandler;

        public void SomeDataIsArrived()
        {
            if (_strongRefreshHandler == null)
            {
                _strongRefreshHandler = new StrongRefreshHandler(() =>
                {
                    _canvas.Refresh(enumRefreshType.All);
                    if (_someDataIsArrivedHandler != null)
                        _someDataIsArrivedHandler(this, null);

                });
            }
            if (_canvas != null && _canvas.Container != null && !_canvas.Container.IsDisposed)
                _canvas.Container.Invoke(_strongRefreshHandler);
        }

        public override void Dispose()
        {
            _isDisposed = true;
            if (_mapRuntime != null)
            {
                DisposeMap();
                _mapRuntime.Dispose();
                //_mapRuntime = null;
            }
            if (_buffer != null)
            {
                _buffer.Dispose();
                _buffer = null;
            }
            _onCanvasSizeChanged = null;
            _someDataIsArrivedHandler = null;
            _strongRefreshHandler = null;
            _renderArgs = null;
            _canvas = null;
            base.Dispose();
        }

        private void DisposeMap()
        {
            if (_mapRuntime != null && _mapRuntime.Map != null)
            {
                _mapRuntime.Map.Dispose();
                _mapRuntime.Apply(null);
            }
        }
    }
}
