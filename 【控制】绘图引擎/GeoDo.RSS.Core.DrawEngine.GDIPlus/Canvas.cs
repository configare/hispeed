using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.IO;
using GeoDo.Project;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    public class Canvas : ICanvas, ICoordinateTransform,
        ICanvasViewControl, IDummyRenderModeSupport,
        IControlMessageAccepter, IReversedCoordinateTransform
    {
        protected CoordEnvelope _currentEnvelope = null;//set from outside
        protected CoordEnvelope _adjustedEnvelope = null;//viewport's coordinate range
        protected UserControl _container = null;
        protected Matrix _matrix = new Matrix();//prjCoord       => screenCoord
        protected Matrix _inverMatrix = null;     //screenCoord => prjCoord 
        protected float _resolutionX = 0;
        protected float _resolutionY = 0;
        protected float _scale = 1f;
        protected float _offsetX = 0;
        protected float _offsetY = 0;
        protected const float ZOOM_MIN_PERCENT = 0.01f;
        protected const float ZOOM_MAX_PERCENT = 100f;
        protected const float ZOOM_STEP_FACTOR = 0.01f;
        protected float _zoomfactor = ZOOM_STEP_FACTOR;
        protected float _zoomMinPercent = ZOOM_MIN_PERCENT;
        protected float _zoomMaxPercent = ZOOM_MAX_PERCENT;
        protected float _mouseOffsetX = 0;
        protected float _mouseOffsetY = 0;
        protected IControlLayer _currentControlLayer = null;
        protected ILayerContainer _layerContainer = null;
        protected bool _isParallel = false;
        protected IDrawArgs _drawArgs = null;
        protected InterpolationMode _interpolationMode = InterpolationMode.NearestNeighbor;
        protected QuickTransform _quickTransform = null;
        protected CoordEnvelope _testImageBox = CoordEnvelope.FromLBWH(0, 0, 5000, 3000);
        protected enumRefreshType _refreshType = enumRefreshType.All;
        protected CacheBitmapManager _cacheBitmapManager = null;
        protected DummyCacheBitmap _dummyCacheBitmap = null;
        protected bool _isDummyMode = false;
        protected Color _backColor = Color.White;
        protected CanvasSetting _canvasSetting = null;
        protected IPerformanceWatch _performanceWatch = null;
        protected bool _isNeedRerender = false;
        protected IPrimaryDrawObject _primaryDrawObject = null;
        //host mouse center is not move while wheel
        private int _hostScreenX = -1;
        private int _hostScreenY = -1;
        private const int NONE_HOST_POSITION = -1;
        //
        private bool _isRasterLayersUpdated = false;
        private bool _isVectorLayersUpdated = false;
        private bool _isFlyLayersUpdated = false;
        //
        private EventHandler _onEnvelopeChanged;
        private bool _isLinking = false;
        private Image _percentBarImage = null;
        private Font _percentFont = new Font("微软雅黑", 10.5f);
        private List<IPixelInfoSubscriber> _pixelInfoSubscribers = new List<IPixelInfoSubscriber>();
        private bool _isMoving = false;
        private bool _disposed = false;
        //
        private bool _isDrawScalePercent = true;
        private Func<int[]> _AOIGetter;
        private Func<int[]> _maskGetter;
        //
        private IProjectionTransform _defaultPrjTransform;
        //
        private enumDataCoordType _dataCoordType = enumDataCoordType.Geo;
        private object _spatialRefOfViewer;
        private Action _someTileIsArrived;
        //倒转(应对升轨与降轨)
        private bool _isReverseDirection = false;
        //影像适应宽度显示
        private bool _isFitWidth = false;

        public Canvas(UserControl container)
        {
            _container = container;
            InitFields();
            AttachEvents();
            UpdateMatrix();
            SetDefaultProjectionTransform(null);
            SetToChinaEnvelope();
        }

        private void InitFields()
        {
            TryLoadCanvasSetting();
            _layerContainer = new LayerContainer();
            _quickTransform = new QuickTransform();
            _drawArgs = new DrawArgsGDIPlus(_quickTransform);
            _cacheBitmapManager = new CacheBitmapManager(_backColor, Color.Transparent, Color.Transparent,
                new DrawArgsGDIPlus(_quickTransform), new DrawArgsGDIPlus(_quickTransform), new DrawArgsGDIPlus(_quickTransform),
                _interpolationMode);
            _dummyCacheBitmap = new DummyCacheBitmap(PixelFormat.Format24bppRgb, _backColor, _drawArgs, _interpolationMode);
            if (_canvasSetting != null)
                _dummyCacheBitmap.IsEnabled = _canvasSetting.RenderSetting.EnabledDummymode;
            _percentBarImage = new Bitmap(this.GetType().Assembly.GetManifestResourceStream("GeoDo.RSS.Core.DrawEngine.GDIPlus.YellowBar.png"));
        }

        private void TryLoadCanvasSetting()
        {
            string fname = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "GeoDo.RSS.Core.DrawEngine.GDIPlus.cnfg.xml");
            if (File.Exists(fname))
            {
                using (CanvasSettingParser p = new CanvasSettingParser())
                {
                    _canvasSetting = p.Parse(fname);
                }
            }
            if (_canvasSetting != null)
            {
                _isParallel = _canvasSetting.RenderSetting.EnabledParallel;
                _backColor = _canvasSetting.RenderSetting.BackColor;
            }
        }

        private void AttachEvents()
        {
            _container.SizeChanged += new EventHandler(_container_SizeChanged);
            _container.Paint += new PaintEventHandler(_container_Paint);
            _container.MouseWheel += new MouseEventHandler(_container_MouseWheel);
            _container.MouseDown += new MouseEventHandler(_container_MouseDown);
            _container.MouseMove += new MouseEventHandler(_container_MouseMove);
            _container.MouseUp += new MouseEventHandler(_container_MouseUp);
            _container.KeyDown += new KeyEventHandler(_container_KeyDown);
            _container.MouseDoubleClick += new MouseEventHandler(_container_MouseDoubleClick);
        }

        bool FireEvent(enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
            return FireEvent(_layerContainer.Layers, eventType, e);
        }

        bool FireEvent(IList<ILayer> layers, enumCanvasEventType eventType, DrawingMouseEventArgs e)
        {
            foreach (ILayer layer in layers)
            {
                ICanvasEvent ce = layer as ICanvasEvent;
                if (ce != null)
                {
                    ce.Event(this, eventType, e);
                    if (e.IsHandled)
                        return true;
                }
                else if (layer is ILayerGroup)
                    return FireEvent((layer as ILayerGroup).Layers, eventType, e);
            }
            return false;
        }

        void _container_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.G)//原分辨率
            {
                Scale = 1f;
                Refresh(enumRefreshType.All);
                return;
            }
            else if (e.KeyCode == Keys.Q)//全图
            {
                SetToFullEnvelope();
                Refresh(enumRefreshType.All);
            }
            else if (e.KeyCode == Keys.W)//宽度
            {
                SetToFitWidth();
                Refresh(enumRefreshType.All);
            }
            else if (e.KeyCode == Keys.R)//图像反转
            {
                this.IsReverseDirection = !this.IsReverseDirection;
                Refresh(enumRefreshType.All);
            }
            else if (e.KeyCode == Keys.H)//恢复为默认的缩放工具
            {
                this.CurrentViewControl = new DefaultControlLayer();
                Refresh(enumRefreshType.All);
            }
            FireEvent(enumCanvasEventType.KeyDown, new DrawingMouseEventArgs());
        }

        void _container_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MouseDown(e.X, e.Y);
            }
            else if (e.Button == MouseButtons.Right && _currentControlLayer is IPencilToolLayer)
            {
                IPencilToolLayer pencil = _currentControlLayer as IPencilToolLayer;
                if (pencil.PencilType == enumPencilType.ControlFreeCurve)
                {
                    MouseDown(e.X, e.Y);
                }
            }
        }

        private void MouseDown(int x, int y)
        {
            DrawingMouseEventArgs arg = new DrawingMouseEventArgs(x, y);
            if (_currentControlLayer != null)
                (_currentControlLayer as ICanvasEvent).Event(this, enumCanvasEventType.MouseDown, arg);
            FireEvent(enumCanvasEventType.MouseDown, arg);
        }

        void _container_MouseMove(object sender, MouseEventArgs e)
        {
            DrawingMouseEventArgs arg = new DrawingMouseEventArgs(e.X, e.Y);
            if (_currentControlLayer != null)
                (_currentControlLayer as ICanvasEvent).Event(this, enumCanvasEventType.MouseMove, arg);
            if (FireEvent(enumCanvasEventType.MouseMove, arg))
                return;
            _isMoving = e.Button == MouseButtons.Left;
            if (!_isMoving)
                TryNotifyPixelInfoSubscribers(e.Location);
        }

        private void TryNotifyPixelInfoSubscribers(Point pt)
        {
            if (_pixelInfoSubscribers == null)
                return;
            PixelInfo pixelInfo = GetPixelInfo(pt);
            foreach (IPixelInfoSubscriber sub in _pixelInfoSubscribers)
            {
                sub.Notify(pixelInfo);
            }
        }

        private PixelInfo GetPixelInfo(Point pt)
        {
            PixelInfo pInfo = new PixelInfo();
            pInfo.ScreenX = pt.X;
            pInfo.ScreenY = pt.Y;
            if (_isReverseDirection)
            {
                IReversedCoordinateTransform tran = this as IReversedCoordinateTransform;
                tran.Screen2Prj(pt.X, pt.Y, out pInfo.PrjX, out pInfo.PrjY);
                tran.Screen2Raster((float)pt.X, (float)pt.Y, out pInfo.RasterY, out pInfo.RasterX);
            }
            else
            {
                CoordTransform.Screen2Prj(pt.X, pt.Y, out pInfo.PrjX, out pInfo.PrjY);
                CoordTransform.Screen2Raster((float)pt.X, (float)pt.Y, out pInfo.RasterY, out pInfo.RasterX);
            }
            CoordTransform.Prj2Geo(pInfo.PrjX, pInfo.PrjY, out pInfo.GeoX, out pInfo.GeoY);
            return pInfo;
        }

        void _container_MouseDoubleClick(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
            {
                DrawingMouseEventArgs arg = new DrawingMouseEventArgs(e.X, e.Y);
                if (_currentControlLayer != null)
                    (_currentControlLayer as ICanvasEvent).Event(this, enumCanvasEventType.DoubleClick, arg);
                FireEvent(enumCanvasEventType.DoubleClick, arg);
            }
        }

        void _container_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DrawingMouseEventArgs arg = new DrawingMouseEventArgs(e.X, e.Y);
                if (_currentControlLayer != null)
                    (_currentControlLayer as ICanvasEvent).Event(this, enumCanvasEventType.MouseUp, arg);
                FireEvent(enumCanvasEventType.MouseUp, arg);
            }
        }

        void _container_MouseWheel(object sender, MouseEventArgs e)
        {
            DrawingMouseEventArgs arg = new DrawingMouseEventArgs(e.Delta, e.X, e.Y);
            if (_currentControlLayer != null)
                (_currentControlLayer as ICanvasEvent).Event(this, enumCanvasEventType.MouseWheel, arg);
            FireEvent(enumCanvasEventType.MouseWheel, arg);
        }

        void _container_SizeChanged(object sender, EventArgs e)
        {
            if (_container.Width < 1 || _container.Height < 1)
                return;
            UpdateMatrixAndRefresh();
            _cacheBitmapManager.Reset(_container.Width, _container.Height);
            _dummyCacheBitmap.Reset(_container.Width, _container.Height);
        }

        private void UpdateMatrixAndRefresh()
        {
            UpdateMatrix();
            //从 _container.Invalidate()修改为 _container.Refresh()是因为窗口联动时刷新不同步
            //_container.Invalidate();
            if (_isLinking)
                _container.Refresh();
            else
                _container.Invalidate();
            FireEnvelopeChangedEvent();
        }

        public void SetDefaultProjectionTransform(string spatialRef)
        {
            if (spatialRef == null)
                _defaultPrjTransform = ProjectionTransformFactory.GetDefault();
            else
                _defaultPrjTransform = ProjectionTransformFactory.GetProjectionTransform(
                    SpatialReferenceFactory.GetSpatialReferenceByWKT(spatialRef, enumWKTSource.EsriPrjFile),
                    new SpatialReference(new GeographicCoordSystem()));
        }

        public void SetToChinaEnvelope()
        {
            CoordEnvelope evp = new CoordEnvelope(70, 150, 10, 60);
            CoordTransform.Geo2Prj(evp);
            CurrentEnvelope = evp;
        }

        public void SetToFullEnvelope()
        {
            if (_primaryDrawObject != null)
                CurrentEnvelope = _primaryDrawObject.OriginalEnvelope.Clone();
            else
                SetToChinaEnvelope();
        }

        public void SetToFitWidth()
        {
            try
            {
                _isFitWidth = true;
                SetToFullEnvelope();
            }
            finally
            {
                _isFitWidth = false;
            }
        }

        private void Geo2PrjUseDefault(double geoX, double geoY, out double prjX, out double prjY)
        {
            if (_defaultPrjTransform == null)
            {
                prjX = prjY = 0;
                return;
            }
            double[] xs = new double[] { geoX };
            double[] ys = new double[] { geoY };
            _defaultPrjTransform.Transform(xs, ys);
            prjX = xs[0];
            prjY = ys[0];
        }

        private void Prj2GeoUseDefault(double prjX, double prjY, out double geoX, out double geoY)
        {
            if (_defaultPrjTransform == null)
            {
                geoX = geoY = 0;
                return;
            }
            double[] xs = new double[] { prjX };
            double[] ys = new double[] { prjY };
            _defaultPrjTransform.InverTransform(xs, ys);
            geoX = xs[0];
            geoY = ys[0];
        }

        public Action SomeTileIsArrived
        {
            get { return _someTileIsArrived; }
            set { _someTileIsArrived = value; }
        }

        public bool IsRasterCoord
        {
            get
            {
                if (_defaultPrjTransform != null)
                    return false;
                if (_primaryDrawObject == null)
                    return true;
                return _primaryDrawObject.IsRasterCoord;
            }
        }

        public enumDataCoordType DataCoordType
        {
            get { return _dataCoordType; }
            set { _dataCoordType = value; }
        }

        public object SpatialRefOfViewer
        {
            get { return _spatialRefOfViewer; }
            set { _spatialRefOfViewer = value; }
        }

        public Func<int[]> AOIGetter
        {
            get { return _AOIGetter; }
            set { _AOIGetter = value; }
        }

        public Func<int[]> MaskGetter
        {
            get { return _maskGetter; }
            set { _maskGetter = value; }
        }

        public bool IsDrawScalePercent
        {
            get { return _isDrawScalePercent; }
            set { _isDrawScalePercent = value; }
        }

        public bool IsMoving
        {
            get { return _isMoving; }
        }

        public List<IPixelInfoSubscriber> PixelInfoSubscribers
        {
            get { return _pixelInfoSubscribers; }
        }

        public bool IsLinking
        {
            get { return _isLinking; }
            set { _isLinking = value; }
        }

        public Control Container
        {
            get { return _container; }
        }

        public EventHandler OnEnvelopeChanged
        {
            get { return _onEnvelopeChanged; }
            set { _onEnvelopeChanged = value; }
        }

        public IRenderBehavior RenderBehavior
        {
            get { return this; }
        }

        public CanvasSetting CanvasSetting
        {
            get { return _canvasSetting; }
        }

        public IDummyRenderModeSupport DummyRenderModeSupport
        {
            get { return this; }
        }

        public IPerformanceWatch PerformanceWatch
        {
            get { return _performanceWatch; }
            set { _performanceWatch = value; }
        }

        public IControlLayer CurrentViewControl
        {
            get { return _currentControlLayer; }
            set
            {
                if (_currentControlLayer != null)
                {
                    _currentControlLayer.Dispose();
                    _currentControlLayer = null;
                }
                _currentControlLayer = value;
            }
        }

        public void StrongRefresh()
        {
            if (_disposed)
                return;
            _container.Refresh();
        }

        public void Refresh(enumRefreshType refreshType)
        {
            if (_disposed)
                return;
            if (_container.FindForm() == null)
            {
                _refreshType = refreshType;
                if (_isLinking)
                    _container.Refresh();
                else
                    _container.Invalidate();
            }
            else
            {
                _container.Invoke(new EventHandler((sender, e) =>
                {
                    _refreshType = refreshType;
                    //从 _container.Invalidate()修改为 _container.Refresh()是因为窗口联动时刷新不同步
                    //_container.Invalidate();
                    if (_isLinking)
                        _container.Refresh();
                    else
                        _container.Invalidate();
                }
                ));
            }
        }

        private void RenderLayers()
        {
            switch (_refreshType)
            {
                case enumRefreshType.All:
                    break;
                case enumRefreshType.RasterLayer:
                    break;
                case enumRefreshType.VectorLayer:
                    break;
                case enumRefreshType.FlyLayer:
                    break;
                case enumRefreshType.AVILayer:
                    break;
            }
        }

        void _container_Paint(object sender, PaintEventArgs e)
        {
            if (_container.Width < 1 || _container.Height < 1)
                return;
            if (_performanceWatch != null)
                _performanceWatch.BeginWatch("Container Paint");
#if PRINT_INFO
            Console.WriteLine(Environment.TickCount.ToString()+ " : Canvas.Render()");
#endif
            Painting(e);
            if (_performanceWatch != null)
                _performanceWatch.EndWatch("Container Paint");
        }

        private Bitmap _buffer = null;
        private Matrix _bufferMatrix = new Matrix();
        private void Painting(PaintEventArgs e)
        {
            if (!_isReverseDirection)
            {
                Painting(e.Graphics);
            }
            else
            {
                Graphics graphic = e.Graphics;
                if (_buffer == null || _buffer.Width != _container.Width || _buffer.Height != _container.Height)
                {
                    if (_buffer != null)
                        _buffer.Dispose();
                    _buffer = new Bitmap(_container.Width, _container.Height, PixelFormat.Format24bppRgb);
                    _bufferMatrix.Reset();
                    _bufferMatrix.RotateAt(180, new Point(_container.Width / 2, _container.Height / 2));
                }
                using (Graphics g = Graphics.FromImage(_buffer))
                {
                    Painting(g);
                    Matrix oldMatrix = graphic.Transform;
                    graphic.Transform = _bufferMatrix;
                    graphic.DrawImage(_buffer, 0, 0);
                    graphic.Transform = oldMatrix;
                    DrawScalePercent(graphic);
                }
            }
        }

        private void Painting(Graphics g)
        {
            g.InterpolationMode = _interpolationMode;
            if (_canvasSetting != null)
            {
                if (_canvasSetting.RenderSetting != null)
                    g.Clear(_canvasSetting.RenderSetting.BackColor);
            }
            else
            {
                g.Clear(_backColor);
            }
            //
            //TestDrawHostPoint(g);
            //TestDrawCenterPoint(g);
            //
            if (_dummyCacheBitmap.IsEnabled)
            {
                if (_dummyCacheBitmap.IsEnabled && _isDummyMode && _dummyCacheBitmap.IsValid)
                    RenderUseDummyCacheBitmap(g);
                else
                    RenderByDummyCacheBitmap(g);
            }
            else
            {
                if (_isParallel)
                    DrawLayersParallel(g);
                else
                    DrawLayers(g);
            }
            //
            DrawControlLayer(g);
            //Console.WriteLine((_scale * 100).ToString(".") + "%");
            if (_isDrawScalePercent && !_isReverseDirection)
                DrawScalePercent(g);
        }

        public Bitmap ToBitmap()
        {
            Bitmap bm = new Bitmap(_container.Width, _container.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(bm))
            {
                DrawLayers(g);
                return bm;
            }
        }

        public Bitmap ToBitmap(float minPrjX, float maxPrjX, float minPrjY, float maxPrjY)
        {
            CoordEnvelope crtEnvelope = CurrentEnvelope.Clone();
            float scaleX = (float)(crtEnvelope.Width / _container.Width);
            float scaleY = (float)(crtEnvelope.Height / _container.Height);
            int width = (int)((maxPrjX - minPrjX) / scaleX);
            int height = (int)((maxPrjY - minPrjY) / scaleY);
            int offsetX = (int)((minPrjX - crtEnvelope.MinX) / scaleX);
            int offsetY = (int)((crtEnvelope.MaxY - maxPrjY) / scaleY);
            Bitmap bm = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bm))
            {
                using (Matrix m = new Matrix())
                {
                    m.Translate(-offsetX, -offsetY);
                    g.Transform = m;
                    DrawLayers(g);
                }
            }
            return bm;
        }

        public Bitmap FullRasterRangeToBitmap()
        {
            IPrimaryDrawObject primaryObj = _primaryDrawObject;
            if (primaryObj == null)
                return ToBitmap();
            CoordEnvelope originalEnvelope = primaryObj.OriginalEnvelope;
            return ToBitmap((float)originalEnvelope.MinX,
                (float)originalEnvelope.MaxX,
                (float)originalEnvelope.MinY,
                (float)originalEnvelope.MaxY);
        }

        private void DrawScalePercent(Graphics g)
        {
            if (_primaryDrawObject == null)
                return;
            float x = 0;
            float y = 0;
            g.DrawImage(_percentBarImage, x, y, _percentBarImage.Width, _percentBarImage.Height);
            int scale = 0;
            if (_primaryDrawObject != null)
                scale = (int)Math.Round(Scale * 100 * _primaryDrawObject.OriginalResolutionX);
            else
                scale = (int)Math.Round(Scale * 100);
            string txt = scale.ToString() + "%";
            SizeF fontSize = g.MeasureString(txt, _percentFont);
            x = (_percentBarImage.Width - fontSize.Width) / 2f;
            y = (_percentBarImage.Height - fontSize.Height) / 2f;
            g.DrawString(txt, _percentFont, Brushes.Black, x, y);
        }

        private void TestDrawCenterPoint(Graphics g)
        {
            if (_currentEnvelope == null)
                return;
            g.SmoothingMode = SmoothingMode.HighQuality;
            CoordPoint pt = _currentEnvelope.Center;
            double x = pt.X;
            double y = pt.Y;
            _quickTransform.Transform(ref x, ref y);
            g.DrawEllipse(Pens.Red, (float)x - 3, (float)y - 3, 6, 6);
            g.SmoothingMode = SmoothingMode.Default;
        }

        private void TestDrawHostPoint(Graphics g)
        {
            if (_hostScreenX != -1)
                g.DrawEllipse(Pens.Black, _hostScreenX - 3, _hostScreenY - 3, 6, 6);
        }

        private void RenderByDummyCacheBitmap(Graphics graphics)
        {
            if (_dummyCacheBitmap.Bitmap == null)
                _dummyCacheBitmap.Reset(_container.Width, _container.Height);
            Graphics g = _dummyCacheBitmap.BeginUpdate();
            try
            {
                if (_isParallel)
                    DrawLayersParallel(g);
                else
                    DrawLayers(g);
                _dummyCacheBitmap.SetValid(true);
                _dummyCacheBitmap.Envelope = _adjustedEnvelope.Clone();
                //copy rendered content from dummy to container's graphics
                graphics.DrawImage(_dummyCacheBitmap.Bitmap, 0, 0);
            }
            finally
            {
                _dummyCacheBitmap.EndUpdate(g);
            }
        }

        private void RenderUseDummyCacheBitmap(Graphics g)
        {
            CoordEnvelope evp = _dummyCacheBitmap.Envelope;
            double x1 = evp.MinX, y1 = evp.MinY;
            double x2 = evp.MaxX, y2 = evp.MaxY;
            _quickTransform.Transform(ref x1, ref y1);
            _quickTransform.Transform(ref x2, ref y2);
            g.DrawImage(_dummyCacheBitmap.Bitmap, RectangleF.FromLTRB((float)x1, (float)y2, (float)x2, (float)y1));
        }

        private void DrawControlLayer(Graphics graphics)
        {
            _drawArgs.Reset(graphics);
            //draw mouse box 
            if (_currentControlLayer != null && _currentControlLayer is IFlyLayer)
                (_currentControlLayer as IRenderLayer).Render(this, _drawArgs);
            //draw test box
            //DrawCurrentEnvelopeBox(graphics);
        }

        private void DrawLayers(Graphics g)
        {
            bool isHasUpdate = false;
            _drawArgs.Reset(g);
            DrawBackgroudLayers(ref isHasUpdate);
            DrawRasterLayers(ref isHasUpdate);
            DrawVectorLayers(ref isHasUpdate);
            DrawBinaryLayers(ref isHasUpdate);
            DrawAVILayers(ref isHasUpdate);
            DrawMaskLayers(ref isHasUpdate);
            DrawAOILayers(ref isHasUpdate);
            DrawGeoGridLayers(ref isHasUpdate);
            DrawFlyLayers(ref isHasUpdate);
            if (!_isReverseDirection)
                DrawToolBoxLayers(ref isHasUpdate);
        }

        private void DrawLayersParallel(Graphics g)
        {
            Parallel.Invoke(() => DrawRasterLayersParallel(), () => DrawVectorLayersParallel(), () => DrawFlyLayersParallel());
            CopyCacheBitmapToGraphics(g);
        }

        private void CopyCacheBitmapToGraphics(Graphics g)
        {
            if (_isRasterLayersUpdated)
            {
                g.DrawImage(_cacheBitmapManager.RasterCacheBitmap.Bitmap, 0, 0);
                //_cacheBitmapManager.RasterCacheBitmap.Bitmap.Save("d:\\1.bmp",ImageFormat.Bmp);
            }
            if (_isVectorLayersUpdated)
            {
                g.DrawImage(_cacheBitmapManager.VectorCacheBitmap.Bitmap, 0, 0);
                //_cacheBitmapManager.VectorCacheBitmap.Bitmap.Save("d:\\2.png", ImageFormat.Png);
            }
            if (_isFlyLayersUpdated)
            {
                g.DrawImage(_cacheBitmapManager.FlyCacheBitmap.Bitmap, 0, 0);
            }
        }

        private void ResetParallelUpdatedFlag()
        {
            _isRasterLayersUpdated = false;
            _isVectorLayersUpdated = false;
            _isFlyLayersUpdated = false;
        }

        private void DrawMaskLayers(ref bool isHasUpdate)
        {
            RenderLayersByFilter(_drawArgs, lyr => lyr is IMaskLayer, ref isHasUpdate);
        }

        private void DrawBinaryLayers(ref bool isHasUpdate)
        {
            RenderLayersByFilter(_drawArgs, lyr => lyr is IBinaryBitampLayer, ref isHasUpdate);
        }

        private void DrawBackgroudLayers(ref bool isHasUpdate)
        {
            RenderLayersByFilter(_drawArgs, lyr => lyr is IBackgroundLayer, ref isHasUpdate);
        }

        private void DrawRasterLayers(ref bool isHasUpdate)
        {
            RenderLayersByFilter(_drawArgs, lyr => lyr is IRasterLayer, ref isHasUpdate);
        }

        private void DrawVectorLayers(ref bool isHasUpdate)
        {
            RenderLayersByFilter(_drawArgs, lyr => lyr is IVectorLayer, ref isHasUpdate);
        }

        private void DrawGeoGridLayers(ref bool isHasUpdate)
        {
            RenderLayersByFilter(_drawArgs, lyr => lyr is IGridLayer, ref isHasUpdate);
        }

        private void DrawFlyLayers(ref bool isHasUpdate)
        {
            RenderLayersByFilter(_drawArgs, lyr => lyr is IFlyLayer, ref isHasUpdate);
        }

        private void DrawToolBoxLayers(ref bool isHasUpdate)
        {
            RenderLayersByFilter(_drawArgs, lyr => lyr is IToolboxLayer, ref isHasUpdate);
        }

        private void DrawAOILayers(ref bool isHasUpdate)
        {
            RenderLayersByFilter(_drawArgs, lyr => lyr is IAOILayer, ref isHasUpdate);
        }

        private void DrawAVILayers(ref bool isHasUpdate)
        {
            RenderLayersByFilter(_drawArgs, lyr => lyr is IAVILayer, ref isHasUpdate);
        }

        private void DrawRasterLayersParallel()
        {
            _isRasterLayersUpdated = DrawFilteredLayers(_cacheBitmapManager.RasterCacheBitmap, lyr => lyr is IRasterLayer);
        }

        private void DrawVectorLayersParallel()
        {
            _isVectorLayersUpdated = DrawFilteredLayers(_cacheBitmapManager.VectorCacheBitmap, lyr => lyr is IVectorLayer);
        }

        private void DrawFlyLayersParallel()
        {
            _isFlyLayersUpdated = DrawFilteredLayers(_cacheBitmapManager.FlyCacheBitmap, lyr => lyr is IFlyLayer);
        }

        private bool DrawFilteredLayers(CacheBitmap cache, Func<ILayer, bool> filter)
        {
            Graphics g = cache.BeginUpdate();
            cache.DrawArgs.Reset(g);
            bool isHasUpdate = false;
            RenderLayersByFilter(cache.DrawArgs, filter, ref isHasUpdate);
            cache.EndUpdate(g);
            return isHasUpdate;
        }

        private void RenderLayersByFilter(IDrawArgs drawArgs, Func<ILayer, bool> filter, ref bool isHasUpdate)
        {
            RenderLayer(drawArgs, _layerContainer as ILayer, filter, ref isHasUpdate);
        }

        private void RenderLayer(IDrawArgs drawArgs, ILayer layer, Func<ILayer, bool> filter, ref bool isHasUpdate)
        {
            if (layer is ILayerGroup)
            {
                ILayerGroup layerGroup = layer as ILayerGroup;
                foreach (ILayer lyr in layerGroup.Layers)
                    RenderLayer(drawArgs, lyr, filter, ref isHasUpdate);
            }
            else
            {
                IRenderLayer renderLayer = layer as IRenderLayer;
                if (filter(layer) && renderLayer.Visible)
                {
                    renderLayer.Render(this, drawArgs);
                    if (!isHasUpdate)
                        isHasUpdate = true;
                }
            }
        }

        private void DrawCurrentEnvelopeBox(Graphics g)
        {
            DrawBox(g, _currentEnvelope, Pens.Red);
            DrawBox(g, _testImageBox, Pens.Green);
        }

        private void DrawBox(Graphics g, CoordEnvelope evp, Pen pen)
        {
            if (evp == null)
                return;
            PointF[] pts = new PointF[] { evp.LeftBottom.ToPointF(), evp.RightUp.ToPointF() };
            _matrix.TransformPoints(pts);
            float width = Math.Abs(pts[1].X - pts[0].X);
            float height = Math.Abs(pts[1].Y - pts[0].Y);
            float x = pts[0].X;
            float y = Math.Min(pts[0].Y, pts[1].Y);
            g.DrawRectangle(pen, x, y, width, height);
        }

        public IPrimaryDrawObject PrimaryDrawObject
        {
            get { return _primaryDrawObject; }
            set
            {
                _primaryDrawObject = value;
                ResetZoomMinMaxPercent();
            }
        }

        private void ResetZoomMinMaxPercent()
        {
            if (_primaryDrawObject == null)
            {
                _zoomMinPercent = ZOOM_MIN_PERCENT;
                _zoomMaxPercent = ZOOM_MAX_PERCENT;
            }
            else
            {
                float maxResolutionX = _primaryDrawObject.OriginalResolutionX / ZOOM_MAX_PERCENT;//eg:2m
                float minResolutionX = _primaryDrawObject.OriginalResolutionX / ZOOM_MIN_PERCENT; //eg:20000m
                _zoomMaxPercent = (float)(1 / maxResolutionX);
                _zoomMinPercent = (float)(1 / minResolutionX);
            }
        }

        public ILayerContainer LayerContainer
        {
            get { return _layerContainer; }
        }

        public float ZoomFactor
        {
            get { return _zoomfactor; }
        }

        public float ResolutionX
        {
            get
            {
                return _resolutionX;
            }
        }

        public float ResolutionY
        {
            get
            {
                return _resolutionY;
            }
        }

        public float Scale
        {
            get { return _scale; }
            set
            {
                if (_primaryDrawObject == null)
                {
                    if (Math.Abs(_scale - value) > float.Epsilon && _scale > _zoomMinPercent && _scale < _zoomMaxPercent)
                    {
                        _scale = value;
                        //host center not be changed
                        _hostScreenX = _container.Width / 2;
                        _hostScreenY = _container.Height / 2;
                        UpdateMatrixAndRefresh();
                    }
                }
                else
                {
                    float scale = 0;
                    if (_primaryDrawObject.OriginalResolutionX < float.Epsilon)
                        scale = value;
                    else
                        scale = value / _primaryDrawObject.OriginalResolutionX;
                    if (Math.Abs(_scale - scale) > float.Epsilon && _scale > _zoomMinPercent && _scale < _zoomMaxPercent)
                    {
                        _scale = scale;
                        //host center not be changed
                        _hostScreenX = _container.Width / 2;
                        _hostScreenY = _container.Height / 2;
                        UpdateMatrixAndRefresh();
                    }
                }
            }
        }

        public float PrimaryObjectScale
        {
            get
            {
                return _primaryDrawObject != null ? _primaryDrawObject.Scale : _scale;
            }
        }

        public CoordEnvelope CurrentEnvelope
        {
            get { return _adjustedEnvelope; }
            set
            {
                _currentEnvelope = value;
                ResetHostAndMouseOffsets();
                ComputeScaleAndOffsets();
                UpdateMatrix();
                FireEnvelopeChangedEvent();
            }
        }

        private void FireEnvelopeChangedEvent()
        {
            if (_onEnvelopeChanged != null)
                _onEnvelopeChanged(this, null);
        }

        private void ComputeScaleAndOffsets()
        {
            float scaleX = (float)(_container.Width / _currentEnvelope.Width);
            float scaleY = (float)(_container.Height / _currentEnvelope.Height);
            if (_isFitWidth)
                _scale = scaleX;
            else
                _scale = Math.Min(scaleX, scaleY);
            if (_primaryDrawObject != null)
            {
                if (_scale < _zoomMinPercent)
                    _scale = _zoomMinPercent;
                else if (_scale > _zoomMaxPercent)
                    _scale = _zoomMaxPercent;
            }
            _offsetX = (float)(_container.Width / _scale - _currentEnvelope.Width) / 2 - (float)_currentEnvelope.MinX;
            _offsetY = -(float)_currentEnvelope.Height - (float)(_container.Height / _scale - _currentEnvelope.Height) / 2 - (float)_currentEnvelope.MinY;
        }

        private void UpdateMatrix()
        {
            if (_container.Width < 1 || _container.Height < 1 || float.IsInfinity(_scale) || float.IsNaN(_scale))
                return;
            BeforeUpdateMatrix();
            _matrix.Reset();
            _matrix.Scale(_scale, -_scale);
            _matrix.Translate(_offsetX + _mouseOffsetX / _scale + _hostOffsetX,
                _offsetY - _mouseOffsetY / _scale - _hostOffsetY);
            UpdateInvertMatrix();
            UpdateQuickTransformArgs();
            AdjustCurrentEnvelope();
            AfterUpdateMatrix();
            UpdateResolutions();
        }

        private void UpdateResolutions()
        {
            _resolutionX = (float)(_adjustedEnvelope.Width / _container.Width);
            _resolutionY = (float)(_adjustedEnvelope.Height / _container.Height);
        }

        private void UpdateInvertMatrix()
        {
            if (_inverMatrix != null)
                _inverMatrix.Dispose();
            _inverMatrix = _matrix.Clone() as Matrix;
            if (_inverMatrix.IsInvertible)
                _inverMatrix.Invert();
        }

        private bool IsNeedHostMousePostion()
        {
            return _hostScreenX != NONE_HOST_POSITION && _hostScreenY != NONE_HOST_POSITION;
        }

        private double _preHostX = 0, _preHostY = 0;
        private float _hostOffsetX = 0, _hostOffsetY = 0;
        private void BeforeUpdateMatrix()
        {
            if (!IsNeedHostMousePostion())
                return;
            _preHostX = _hostScreenX;
            _preHostY = _hostScreenY;
            _quickTransform.InverTransform(ref _preHostX, ref _preHostY);
        }

        private void AfterUpdateMatrix()
        {
            if (!IsNeedHostMousePostion())
                return;
            double postHostX = _hostScreenX;
            double postHostY = _hostScreenY;
            _quickTransform.InverTransform(ref postHostX, ref postHostY);
            float diffX = (float)(postHostX - _preHostX);
            float diffY = (float)(postHostY - _preHostY);
            _matrix.Translate(diffX, diffY);
            //
            _hostOffsetX += diffX;
            _hostOffsetY += -diffY;
            //
            UpdateInvertMatrix();
            UpdateQuickTransformArgs();
            AdjustCurrentEnvelope();
            ResetHostScreenPoint();
        }

        private void ResetHostAndMouseOffsets()
        {
            //for host mouse position be not moved
            _hostOffsetX = _hostOffsetY = 0;
            //for pan by mouse 
            _mouseOffsetX = _mouseOffsetY = 0;
        }

        private void ResetHostScreenPoint()
        {
            _hostScreenX = _hostScreenY = NONE_HOST_POSITION;
        }

        private void AdjustCurrentEnvelope()
        {
            double x1 = 0, y1 = 0;
            double x2 = _container.Width, y2 = _container.Height;
            _quickTransform.InverTransform(ref x1, ref y1);
            _quickTransform.InverTransform(ref x2, ref y2);
            if (_adjustedEnvelope == null)
                _adjustedEnvelope = new CoordEnvelope();
            _adjustedEnvelope.Reset(x1, y2, x2 - x1, y1 - y2);
        }

        private void UpdateQuickTransformArgs()
        {
            ICoordinateTransform coordTransform = this as ICoordinateTransform;
            double prjX1, prjX2, prjY1, prjY2;
            PointF pt1 = new PointF(0, 0);          //随意取的坐标点
            PointF pt2 = new PointF(_container.Width, _container.Height);  //随意取的坐标点
            coordTransform.Screen2Prj(pt1.X, pt1.Y, out prjX1, out prjY1);
            coordTransform.Screen2Prj(pt2.X, pt2.Y, out prjX2, out prjY2);
            double kx = (pt2.X - pt1.X) / (prjX2 - prjX1);
            double ky = -kx; //(pt2.Y - pt1.Y) / (prjY2 - prjY1);
            double bx = pt1.X - kx * prjX1;
            double by = pt1.Y - ky * prjY1;
            _quickTransform.Reset(kx, ky, bx, by);
        }

        public ICoordinateTransform CoordTransform
        {
            get { return this; }
        }

        QuickTransform ICoordinateTransform.QuickTransform
        {
            get { return _quickTransform; }
        }

        void ICoordinateTransform.Raster2Prj(int row, int col, out double prjX, out double prjY)
        {
            prjX = 0;
            prjY = 0;
            if (_primaryDrawObject != null)
                _primaryDrawObject.Raster2Prj(row, col, out prjX, out prjY);
        }

        void ICoordinateTransform.Raster2Prj(float row, float col, out double prjX, out double prjY)
        {
            prjX = 0;
            prjY = 0;
            if (_primaryDrawObject != null)
                _primaryDrawObject.Raster2Prj(row, col, out prjX, out prjY);
        }

        unsafe void ICoordinateTransform.Raster2Prj(PointF* rasterPoint)
        {
            if (_primaryDrawObject != null)
                _primaryDrawObject.Raster2Prj(rasterPoint);
        }

        void ICoordinateTransform.Raster2Geo(int row, int col, out double geoX, out double geoY)
        {
            geoX = 0;
            geoY = 0;
            if (_primaryDrawObject != null)
                _primaryDrawObject.Raster2Geo(row, col, out geoX, out geoY);
        }

        void ICoordinateTransform.Prj2Raster(double prjX, double prjY, out int row, out int col)
        {
            row = 0;
            col = 0;
            if (_primaryDrawObject != null)
                _primaryDrawObject.Prj2Raster(prjX, prjY, out row, out col);
        }

        void ICoordinateTransform.Screen2Prj(float screenX, float screenY, out double prjX, out double prjY)
        {
            PointF pt = new PointF(screenX, screenY);
            PointF[] pts = new PointF[] { pt };
            _inverMatrix.TransformPoints(pts);
            prjX = pts[0].X;
            prjY = pts[0].Y;
        }

        void ICoordinateTransform.Prj2Screen(double prjX, double prjY, out int screenX, out int screenY)
        {
            PointF pt = new PointF((float)prjX, (float)prjY);
            PointF[] pts = new PointF[] { pt };
            _matrix.TransformPoints(pts);
            screenX = (int)pts[0].X;
            screenY = (int)pts[0].Y;
        }

        unsafe void ICoordinateTransform.Prj2Screen(PointF[] points)
        {
            _matrix.TransformPoints(points);
        }

        void ICoordinateTransform.Screen2Raster(int screenX, int screenY, out int row, out int col)
        {
            row = 0;
            col = 0;
            if (_primaryDrawObject != null)
            {
                _primaryDrawObject.Screen2Raster(screenX, screenY, out row, out col);
            }
        }

        void ICoordinateTransform.Raster2Screen(int row, int col, out int screenX, out int screenY)
        {
            screenX = 0;
            screenY = 0;
            if (_primaryDrawObject != null)
            {
                _primaryDrawObject.Raster2Screen(row, col, out screenX, out screenY);
            }
        }

        void ICoordinateTransform.Screen2Raster(float screenX, float screenY, out int row, out int col)
        {
            row = 0;
            col = 0;
            if (_primaryDrawObject != null)
            {
                _primaryDrawObject.Screen2Raster(screenX, screenY, out row, out col);
            }
        }

        void ICoordinateTransform.Screen2Raster(float screenX, float screenY, out float row, out float col)
        {
            row = 0;
            col = 0;
            if (_primaryDrawObject != null)
            {
                _primaryDrawObject.Screen2Raster(screenX, screenY, out row, out col);
            }
        }

        void ICoordinateTransform.Raster2Screen(float row, float col, out float screenX, out float screenY)
        {
            screenX = 0;
            screenY = 0;
            if (_primaryDrawObject != null)
            {
                _primaryDrawObject.Raster2Screen(row, col, out screenX, out screenY);
            }
        }

        void ICoordinateTransform.Geo2Prj(double geoX, double geoY, out double prjX, out double prjY)
        {
            prjX = 0;
            prjY = 0;
            if (_primaryDrawObject != null)
                _primaryDrawObject.Geo2Prj(geoX, geoY, out prjX, out prjY);
            else
                Geo2PrjUseDefault(geoX, geoY, out prjX, out prjY);
        }

        void ICoordinateTransform.Geo2Prj(CoordEnvelope envelope)
        {
            double x1 = envelope.MinX;
            double y1 = envelope.MaxY;
            double x2 = envelope.MaxX;
            double y2 = envelope.MinY;
            double prjX1, prjX2, prjY1, prjY2;
            CoordTransform.Geo2Prj(x1, y1, out prjX1, out prjY1);
            CoordTransform.Geo2Prj(x2, y2, out prjX2, out prjY2);
            envelope.Reset(prjX1, prjY2, prjX2 - prjX1, prjY1 - prjY2);
        }

        void ICoordinateTransform.Prj2Geo(double prjX, double prjY, out double geoX, out double geoY)
        {
            geoX = 0;
            geoY = 0;
            if (_primaryDrawObject != null)
                _primaryDrawObject.Prj2Geo(prjX, prjY, out geoX, out geoY);
            else
                Prj2GeoUseDefault(prjX, prjY, out geoX, out geoY);
        }

        void IControlMessageAccepter.AcceptKeyDown(object sender, KeyEventArgs e)
        {
            _container_KeyDown(sender, e);
        }

        void IControlMessageAccepter.AcceptMouseDown(object sender, MouseEventArgs e)
        {
            _container_MouseDown(sender, e);
        }

        void IControlMessageAccepter.AcceptMouseMove(object sender, MouseEventArgs e)
        {
            _container_MouseMove(sender, e);
        }

        void IControlMessageAccepter.AcceptMouseUp(object sender, MouseEventArgs e)
        {
            _container_MouseUp(sender, e);
        }

        void IControlMessageAccepter.AcceptMouseWheel(object sender, MouseEventArgs e)
        {
            _container_MouseWheel(sender, e);
        }

        void IReversedCoordinateTransform.Screen2Prj(float screenX, float screenY, out double prjX, out double prjY)
        {
            if (_isReverseDirection)
            {
                screenX = _container.Width - screenX;
                screenY = _container.Height - screenY;
            }
            PointF pt = new PointF(screenX, screenY);
            PointF[] pts = new PointF[] { pt };
            _inverMatrix.TransformPoints(pts);
            prjX = pts[0].X;
            prjY = pts[0].Y;
        }

        void IReversedCoordinateTransform.Prj2Screen(double prjX, double prjY, out int screenX, out int screenY)
        {
            PointF pt = new PointF((float)prjX, (float)prjY);
            PointF[] pts = new PointF[] { pt };
            _matrix.TransformPoints(pts);
            screenX = (int)pts[0].X;
            screenY = (int)pts[0].Y;
            if (_isReverseDirection)
            {
                screenX = _container.Width - screenX;
                screenY = _container.Height - screenY;
            }
        }

        void IReversedCoordinateTransform.Screen2Raster(int screenX, int screenY, out int row, out int col)
        {
            row = 0;
            col = 0;
            if (_primaryDrawObject != null)
            {
                if (_isReverseDirection)
                {
                    screenX = _container.Width - screenX;
                    screenY = _container.Height - screenY;
                }
                _primaryDrawObject.Screen2Raster(screenX, screenY, out row, out col);
            }
        }

        void IReversedCoordinateTransform.Screen2Raster(float screenX, float screenY, out int row, out int col)
        {
            row = 0;
            col = 0;
            if (_primaryDrawObject != null)
            {
                if (_isReverseDirection)
                {
                    screenX = _container.Width - screenX;
                    screenY = _container.Height - screenY;
                }
                _primaryDrawObject.Screen2Raster(screenX, screenY, out row, out col);
            }
        }

        void IReversedCoordinateTransform.Raster2Screen(int row, int col, out int screenX, out int screenY)
        {
            screenX = 0;
            screenY = 0;
            if (_primaryDrawObject != null)
            {
                _primaryDrawObject.Raster2Screen(row, col, out screenX, out screenY);
                if (_isReverseDirection)
                {
                    screenX = _container.Width - screenX;
                    screenY = _container.Height - screenY;
                }
            }
        }

        void IReversedCoordinateTransform.Screen2Raster(float screenX, float screenY, out float row, out float col)
        {
            row = 0;
            col = 0;
            if (_primaryDrawObject != null)
            {
                if (_isReverseDirection)
                {
                    screenX = _container.Width - screenX;
                    screenY = _container.Height - screenY;
                }
                _primaryDrawObject.Screen2Raster(screenX, screenY, out row, out col);
            }
        }

        void IReversedCoordinateTransform.Raster2Screen(float row, float col, out float screenX, out float screenY)
        {
            screenX = 0;
            screenY = 0;
            if (_primaryDrawObject != null)
            {
                _primaryDrawObject.Raster2Screen(row, col, out screenX, out screenY);
                if (_isReverseDirection)
                {
                    screenX = _container.Width - screenX;
                    screenY = _container.Height - screenY;
                }
            }
        }


        bool IDummyRenderModeSupport.IsDummyModel
        {
            get { return _isDummyMode; }
        }

        bool IDummyRenderModeSupport.IsEnabledDummyCache
        {
            get { return _dummyCacheBitmap.IsEnabled; }
            set { _dummyCacheBitmap.IsEnabled = value; }
        }

        void IDummyRenderModeSupport.SetToDummyRenderMode()
        {
            //if (_dummyCacheBitmap.IsEnabled)
                _isDummyMode = true;
        }

        void IDummyRenderModeSupport.SetToNomralRenderMode()
        {
            _isDummyMode = false;
        }

        bool IRenderBehavior.IsParallel
        {
            get { return _isParallel; }
            set { _isParallel = value; }
        }

        #region ICanvasViewControl 成员

        public bool IsReverseDirection
        {
            get { return _isReverseDirection; }
            set { _isReverseDirection = value; }
        }

        public void ApplyOffset(float offsetScreenX, float offsetScreenY)
        {
            if (_isReverseDirection)
            {
                offsetScreenX *= -1;
                offsetScreenY *= -1;
            }
            if (Math.Abs(offsetScreenX) < float.Epsilon && Math.Abs(offsetScreenY) < float.Epsilon)
                return;
            _mouseOffsetX += offsetScreenX;
            _mouseOffsetY += offsetScreenY;
            UpdateMatrixAndRefresh();
        }

        public void ZoomInByStep(int steps)
        {
            ZoomInByStep(-1, -1, steps);
        }

        public void ZoomInByStep(int hostScreenX, int hostScreenY, int steps)
        {
            if (_isReverseDirection)
            {
                hostScreenX = _container.Width - hostScreenX;
                hostScreenY = _container.Height - hostScreenY;
            }
            _hostScreenX = hostScreenX;
            _hostScreenY = hostScreenY;
            if (_primaryDrawObject == null)
                _scale += _scale * 0.1f;
            else
            {
                /*
                 * 修改了比例<0的情况
                 */
                //_scale *= (_primaryDrawObject.Scale / (_primaryDrawObject.Scale - _zoomfactor * steps));
                float zoomf = _zoomfactor;
                float m = _primaryDrawObject.Scale - zoomf * steps;
                while (m < 0)
                {
                    zoomf -= 0.005f;
                    m = _primaryDrawObject.Scale - zoomf * steps;
                }
                _scale *= (_primaryDrawObject.Scale / m);
            }
            if (_primaryDrawObject != null)
            {
                if (_scale > _zoomMaxPercent)
                    _scale = _zoomMaxPercent;
            }
            else
            {
            }
            UpdateMatrixAndRefresh();
        }

        public void ZoomOutByStep(int steps)
        {
            ZoomOutByStep(-1, -1, steps);
        }

        public void ZoomOutByStep(int hostScreenX, int hostScreenY, int steps)
        {
            if (_isReverseDirection)
            {
                hostScreenX = _container.Width - hostScreenX;
                hostScreenY = _container.Height - hostScreenY;
            }
            _hostScreenX = hostScreenX;
            _hostScreenY = hostScreenY;
            if (_primaryDrawObject == null)
                _scale -= _scale * 0.1f;
            else
                _scale *= (_primaryDrawObject.Scale / (_primaryDrawObject.Scale + _zoomfactor * steps));
            if (_primaryDrawObject != null)
            {
                if (_scale < _zoomMinPercent)
                    _scale = _zoomMinPercent;
            }
            else
            {
            }
            UpdateMatrixAndRefresh();
        }

        private float AdjustZoomfactor(float zoomfactor)
        {
            if (_primaryDrawObject != null)
                return _scale * (_primaryDrawObject.Scale / (_primaryDrawObject.Scale + zoomfactor));
            return zoomfactor;
        }

        #endregion

        public void Dispose()
        {
            _disposed = true;
            _primaryDrawObject = null;
            _onEnvelopeChanged = null;
            _someTileIsArrived = null;
            _pixelInfoSubscribers.Clear();
            if (_container != null)
            {
                _container.SizeChanged -= new EventHandler(_container_SizeChanged);
                _container.Paint -= new PaintEventHandler(_container_Paint);
                _container.MouseWheel -= new MouseEventHandler(_container_MouseWheel);
                _container.MouseDown -= new MouseEventHandler(_container_MouseDown);
                _container.MouseMove -= new MouseEventHandler(_container_MouseMove);
                _container.MouseUp -= new MouseEventHandler(_container_MouseUp);
                _container.KeyDown -= new KeyEventHandler(_container_KeyDown);
                _container.MouseDoubleClick -= new MouseEventHandler(_container_MouseDoubleClick);
                _container = null;
            }
            if (_cacheBitmapManager != null)
            {
                _cacheBitmapManager.Dispose();
                _cacheBitmapManager = null;
            }
            if (_dummyCacheBitmap != null)
            {
                _dummyCacheBitmap.Dispose();
                _dummyCacheBitmap = null;
            }

            if (_layerContainer != null)
            {
                _layerContainer.Dispose();
                _layerContainer = null;
            }
            if (_percentBarImage != null)
            {
                _percentBarImage.Dispose();
                _percentBarImage = null;
            }
            if (_percentFont != null)
            {
                _percentFont.Dispose();
                _percentFont = null;
            }
            if (_drawArgs != null)
            {
                _drawArgs.Dispose();
                _drawArgs = null;
            }
        }
    }
}
