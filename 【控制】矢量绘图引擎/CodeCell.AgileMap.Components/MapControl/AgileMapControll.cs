using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using CodeCell.Bricks.RedoUndo;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.UIs;


namespace CodeCell.AgileMap.Components
{
    public partial class AgileMapControl : UserControl, IMapRuntimeHost,IMapControl,IMouseLocationInfoPrinterManager,IMapControlEvents,IMapRefresh,IMapControlDummySupprot,ILocationService
    {
        private ISpatialReference _spatialReference = null;
        private IProjectionTransform _projectionTransform = null;
        private enumCoordinateType _coordType = enumCoordinateType.Projection;
        private IMapRuntime _vectorMapRuntime = null;
        private RenderArgs _renderArg = new RenderArgs();
        private RectangleF _viewport = RectangleF.Empty;
        private bool _isRenderUseDummyImage = false;
        //鼠标位置信息侦听对象列表
        private List<IMouseLocationInfoPrinter> _infoPrinters = null;
        //坐标转换对象 from _vectorMapRuntime 
        private ICoordinateTransform _coordTransfrom = null;
        //操作栈
        private IOperationStack _operationStack = new OperationStack();
        //容器事件处理队列
        private List<IContainerEventHandler> _containerEventHandlers = new List<IContainerEventHandler>();
        //地图工具
        private Dictionary<enumMapToolType,IMapTool> _mapTools = null;
        private IMapTool _currentMapTool = null;
        //刷新后通知调用方
        private OnRenderIsFinishedHandler _renderFinishedNotify = null;
        private EventHandler _onCanvasSizeChanged = null;
        //书签
        private IBookmarkManager _bookmarkManager = null;

        public AgileMapControl()
        {
            InitializeComponent();
            InitControls();
            AttachEvents();
            BuildProjectionTransform(_spatialReference);
            SetViewportToDefault();
            BuildVectorMapRuntime();
            //关闭操作栈
            _operationStack.Enabled = false;
            InitMapTools();
            KeyDown += new KeyEventHandler(AgileMapControl_KeyDown);
        }

        void AgileMapControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C)
            {
                Clipboard.SetText(_vectorMapRuntime.Scale.ToString());
            }
        }
 
        #region Init Procedures

        private void InitMapTools()
        {
            _mapTools = new Dictionary<enumMapToolType, IMapTool>();
            _mapTools.Add(enumMapToolType.Pan, new MapPanTool());
            _mapTools.Add(enumMapToolType.Identify, new MapIdentifyTool());
            _mapTools.Add(enumMapToolType.ZoomIn, new MapZoomInTool());
            _mapTools.Add(enumMapToolType.ZoomOut, new MapZoomOutTool());
            _currentMapTool = _mapTools[enumMapToolType.Pan];
        }

        private void BuildProjectionTransform(ISpatialReference targetSpatialReference)
        {
            if (_projectionTransform != null)
                _projectionTransform.Dispose();
            _projectionTransform = ProjectionTransformFactory.GetProjectionTransform(new SpatialReference(new GeographicCoordSystem()), targetSpatialReference);
            if(_vectorMapRuntime != null)
                (_vectorMapRuntime as MapRuntime).ChangeProjectionTransform();
        }

        private void InitControls()
        {
            DoubleBuffered = true;
            BackColor = Color.White;
        }

        public void SetViewport(Envelope geoEnvelope)
        {
            PointF prjLeftUp = new PointF((float)geoEnvelope.MinX, (float)geoEnvelope.MaxY);
            _projectionTransform.Transform(ref prjLeftUp);
            PointF prjRightDown = new PointF((float)geoEnvelope.MaxX, (float)geoEnvelope.MinY);
            _projectionTransform.Transform(ref prjRightDown);
            _viewport = RectangleF.FromLTRB(prjLeftUp.X, -prjLeftUp.Y, prjRightDown.X, -prjRightDown.Y);
        }

        private void SetViewportToDefault()
        {
            Envelope envelope = new Envelope(68.9, 3.14, 141.55, 54);//默认为中国区域
            SetViewport(envelope);
         }

        private void BuildVectorMapRuntime()
        {
            _vectorMapRuntime = new MapRuntime(this);
            _coordTransfrom = (_vectorMapRuntime as IFeatureRenderEnvironment).CoordinateTransform;
        }

        private void AttachEvents()
        {
            MouseDoubleClick += new MouseEventHandler(MapControl_MouseDoubleClick);
            MouseWheel += new MouseEventHandler(MapControl_MouseWheel);
            Disposed += new EventHandler(MapControl_Disposed);
            MouseDown += new MouseEventHandler(MapControl_MouseDown);
            MouseMove += new MouseEventHandler(MapControl_MouseMove);
            MouseUp += new MouseEventHandler(MapControl_MouseUp);
            PreviewKeyDown += new PreviewKeyDownEventHandler(MapControl_PreviewKeyDown);
            SizeChanged += new EventHandler(AgileMapControl_SizeChanged);
        }



        void AgileMapControl_SizeChanged(object sender, EventArgs e)
        {
            if (_onCanvasSizeChanged != null)
                _onCanvasSizeChanged(sender, e);
        }

        void MapControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control && e.Shift && e.KeyCode == Keys.L)
            {
                Point[] pts = new Point[] { PointToClient(Control.MousePosition)};
                CoordinateTransfrom.PixelCoord2PrjCoord(pts);
                PointF[] ptfs = new PointF[] { new PointF(pts[0].X,pts[0].Y)};
                CoordinateTransfrom.PrjCoord2GeoCoord(ptfs);
                Clipboard.SetText(ptfs[0].X.ToString("0.####") + " " + ptfs[0].Y.ToString("0.####"));
            }
        }

        private void NotifyEvent(enumContainerEventType eventType, object eventArg)
        {
            if (_containerEventHandlers.Count == 0)
                return;
            bool handled = false ;
            for (int i = 0; i < _containerEventHandlers.Count; i++)
            {
                _containerEventHandlers[i].Handle(this, eventType, eventArg, ref handled);
                if (handled)
                    break;
            }
        }

        #endregion

        #region IMapControl Members

        public IMapRuntime MapRuntime
        {
            get { return _vectorMapRuntime; }
        }

        public ISpatialReference SpatialReference
        {
            get { return _spatialReference; }
            set 
            {
                if (_vectorMapRuntime.Map != null)
                {
                    MsgBox.ShowInfo("在已经加载地图的状态下不能修改空间参考。");
                    return;
                }
                _spatialReference = value; 
                BuildProjectionTransform(_spatialReference);
            }
        }

        public RectangleF FullExtentPrj
        {
            get 
            {
                if (_vectorMapRuntime == null || _vectorMapRuntime.Map == null)
                    return RectangleF.Empty;
                Envelope evp = _vectorMapRuntime.Map.GetFullEnvelope();
                if (evp == null)
                    return RectangleF.Empty;
                return evp.ToRectangleF();
            }
        }

        public RectangleF ExtentPrj
        {
            get { return _viewport; }
            set 
            {
                if (!value.IsEmpty)
                {
                    if (_vectorMapRuntime.IsExceedMaxResolution(value))
                        return;
                    _viewport = value;
                }
            }
        }

        public Envelope ExtentGeo
        {
            get
            {
                PointF leftUp = new PointF(_viewport.Left,-_viewport.Top);
                PointF rightDown = new PointF(_viewport.Right,-_viewport.Bottom);
                PointF[] pts = new PointF[]{leftUp,rightDown};
                CoordinateTransfrom.PrjCoord2GeoCoord(pts);
                return new Envelope(Math.Min(pts[0].X, pts[1].X),
                                    Math.Min(pts[0].Y, pts[1].Y),
                                    Math.Max(pts[0].X, pts[1].X),
                                    Math.Max(pts[0].Y, pts[1].Y));
            }
        }

        public void PanTo(Envelope geoCoord)
        {
            if (geoCoord == null)
                return;
            if (Math.Abs(geoCoord.Width) < double.Epsilon || Math.Abs(geoCoord.Height) < double.Epsilon)
            {
                PanTo(geoCoord.LeftUpPoint);
            }
            else
            {
                PointF prjLeftUp = new PointF((float)geoCoord.MinX, (float)geoCoord.MaxY);
                _projectionTransform.Transform(ref prjLeftUp);
                PointF prjRightDown = new PointF((float)geoCoord.MaxX, (float)geoCoord.MinY);
                _projectionTransform.Transform(ref prjRightDown);
                _viewport = RectangleF.FromLTRB(prjLeftUp.X, -prjLeftUp.Y, prjRightDown.X, -prjRightDown.Y);
            }
        }

        public void PanTo(ShapePoint geoCenterPoint)
        {
            if (geoCenterPoint == null)
                return;
            CoordinateTransfrom.GeoCoord2PrjCoord(new ShapePoint[] { geoCenterPoint });//geo to prj
            PointF oldCenter = new PointF(_viewport.Left + _viewport.Width / 2,
                                          _viewport.Top + _viewport.Height / 2);
            float offsetX = (float)(geoCenterPoint.X - oldCenter.X);
            float offsetY = (float)(-geoCenterPoint.Y - oldCenter.Y);
            _viewport.Offset(offsetX, offsetY);
        }


        public IOperationStack OperationStack
        {
            get { return _operationStack; }
        }

        public int ScaleDenominator 
        {
            get { return _vectorMapRuntime.Scale; }
            set 
            {
                if (value < 1)
                    return;
                _vectorMapRuntime.Scale = value;
                SetScale(_vectorMapRuntime.Scale);
                ReRender();
            }
        }

        private void SetScale(float scale)
        {
            double pixelMeters = Width * (1f / _vectorMapRuntime.DPI) * AgileMap.Core.MapRuntime.cstMetersPerInch;
            float w = (float)(scale * pixelMeters);
            pixelMeters = Height * (1f / _vectorMapRuntime.DPI) * AgileMap.Core.MapRuntime.cstMetersPerInch;
            float h = (float)(scale * pixelMeters);
            float diffWidth = w - _viewport.Width;
            float diffHeight = h - _viewport.Height;
            _viewport.Inflate(diffWidth / 2f, diffHeight / 2f);
        }


        public IMap Map
        {
            get { return _vectorMapRuntime.Map; }
        }

        public void Apply(IMap map)
        {
            _spatialReference = map.MapArguments.TargetSpatialReference;
            BuildProjectionTransform(_spatialReference);
            _vectorMapRuntime.Apply(map);
            if (map.MapArguments != null && map.MapArguments.Extent != null)
            {
                //SetViewport(map.MapArguments.Extent);
            }
        }

        public IMapControlEvents MapControlEvents
        {
            get { return this; }
        }

        public ICoordinateTransform CoordinateTransfrom
        {
            get { return _vectorMapRuntime as ICoordinateTransform; }
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public IRuntimeExchanger Exchanger
        {
            get { return _vectorMapRuntime.RuntimeExchanger; }
        }

        public void SetCurrentMapTool(enumMapToolType standardToolType)
        {
            if (_mapTools.ContainsKey(standardToolType))
                _currentMapTool = _mapTools[standardToolType];
        }

        public void SetCurrentMapTool(IMapTool mapTool)
        {
            if(mapTool != null)
                _currentMapTool = mapTool;
        }

        public IMapTool GetCurrentMapTool()
        {
            return _currentMapTool;
        }

        #endregion

        #region IVectorMapAppEnvironment Members

        public new Control Container
        {
            get { return this; }
        }

        public IProjectionTransform ProjectionTransform
        {
            get { return _projectionTransform; }
        }

        public enumCoordinateType CoordinateType
        {
            get { return _coordType; }
        }

        private Envelope _focusEnvelope = new Envelope();
        public Envelope FocusEnvelope
        {
            get
            {
                _focusEnvelope.MinX = Math.Min(_viewport.Left, _viewport.Right);
                _focusEnvelope.MaxX = Math.Max(_viewport.Left, _viewport.Right);
                _focusEnvelope.MinY = Math.Min(_viewport.Top, _viewport.Bottom);
                _focusEnvelope.MaxY = Math.Max(_viewport.Top, _viewport.Bottom);
                return _focusEnvelope;
            }
        }

        public Size CanvasSize
        {
            get { return this.Size; }
        }

        public EventHandler OnCanvasSizeChanged
        {
            get { return _onCanvasSizeChanged; }
            set { _onCanvasSizeChanged = value; }
        }

        public bool UseDummyMap
        {
            get { return  _isRenderUseDummyImage; }
        }

        public ILocationService LocationService
        {
            get { return this; }
        }

        #endregion

        #region Render procedures

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Render(e);
        }

        private void Render(PaintEventArgs e)
        {
            if (_vectorMapRuntime == null || _vectorMapRuntime.Map == null)
            {
                e.Graphics.Clear(BackColor);
                return;
            }
            try
            {
                _renderArg.BeginRender(e.Graphics);
                e.Graphics.Clear(_vectorMapRuntime.Map.MapArguments.BackColor);
                _vectorMapRuntime.Render(_renderArg);
                //
                if (_currentMapTool != null)
                    _currentMapTool.Render(_renderArg);
            }
            finally
            {
                _renderArg.EndRender();
                //触发渲染完毕的事件通知(异步方式通知)
                if (_renderFinishedNotify != null)
                {
                    this.DoBeginInvoke(_renderFinishedNotify);
                    _renderFinishedNotify = null;
                }
            }
       }

        public void Render()
        {
            Invalidate();
        }

        public void ReRender()
        {
            _renderArg.IsReRender = true;
            Render();
        }

        public void Render(OnRenderIsFinishedHandler finishNotify)
        {
            _renderFinishedNotify = finishNotify;
            Render();
        }

        public void ReRender(OnRenderIsFinishedHandler finishNotify)
        {
            _renderFinishedNotify = finishNotify;
            ReRender();
        }

        #endregion

        #region Events for This

        void MapControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            bool handled = false;
            _currentMapTool.Handle(this, enumContainerEventType.MouseDoubleClick, e, ref handled);
        }

        void MapControl_MouseUp(object sender, MouseEventArgs e)
        {
            bool handled = false ;
            _currentMapTool.Handle(this, enumContainerEventType.MouseUp, e, ref handled);
        }

        void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            PrintInfos(e.Location);
            bool handled = false;
            _currentMapTool.Handle(this, enumContainerEventType.MouseMove, e, ref handled);
            //TryDisplayMouseTip(e.Location);
        }

        private void TryDisplayMouseTip(Point point)
        {
            Feature fet = null;
            RectangleF rect = RectangleF.Empty;
            _vectorMapRuntime.HitTest(point, out fet, out rect);
            if (fet != null)
                this.FindForm().Text = fet.OID.ToString();
        }

        private void PrintInfos(Point location)
        {
            if (_infoPrinters == null || _infoPrinters.Count == 0)
                return;
            Point prjPt = location;
            _coordTransfrom.PixelCoord2PrjCoord(ref prjPt);
            PointF geoPt = prjPt ;
            _coordTransfrom.PrjCoord2GeoCoord(ref geoPt);
            foreach (IMouseLocationInfoPrinter printer in _infoPrinters)
            {
                printer.Print(this, location.X, location.Y, prjPt.X, prjPt.Y, geoPt.X, geoPt.Y);
            }
        }


        void MapControl_MouseDown(object sender, MouseEventArgs e)
        {
            bool handled = false;
            _currentMapTool.Handle(this, enumContainerEventType.MouseDown, e, ref handled);
            TryCloseFocusBubble(e);
        }

        private void TryCloseFocusBubble(MouseEventArgs e)
        {
            if (_vectorMapRuntime == null || _vectorMapRuntime.LocatingFocusLayer == null)
                return;
            if (_vectorMapRuntime.LocatingFocusLayer.BubbleRect.Contains(new PointF(e.X, e.Y)))
                _vectorMapRuntime.LocatingFocusLayer.IsShowBubble = false;
        }

        void MapControl_MouseWheel(object sender, MouseEventArgs e)
        {
            bool handled = false;
            _currentMapTool.Handle(this, enumContainerEventType.MouseWheel, e, ref handled);
        }

        public void SetToDummyRenderMode()
        {
            _isRenderUseDummyImage = true;
        }

        public void ResetToNormalRenderMode()
        {
            _isRenderUseDummyImage = false;
        }

        void MapControl_Disposed(object sender, EventArgs e)
        {
            _vectorMapRuntime.Dispose();
            _renderArg.EndRender();
            if(_infoPrinters != null)
                _infoPrinters.Clear();
        }

        #endregion

        #region IMouseLocationInfoPrinterManager Members

        public void Register(IMouseLocationInfoPrinter printer)
        {
            if (_infoPrinters == null)
                _infoPrinters = new List<IMouseLocationInfoPrinter>();
            if(!_infoPrinters.Contains(printer))
                _infoPrinters.Add(printer);
        }

        public void Remove(IMouseLocationInfoPrinter printer)
        {
            if (_infoPrinters != null && _infoPrinters.Contains(printer))
                _infoPrinters.Remove(printer);
        }

        #endregion

        #region IMapControlEvents Members

        public OnMapScaleChangedHandler OnMapScaleChanged
        {
            get
            {
                return _vectorMapRuntime.OnMapScaleChanged;
            }
            set
            {
                _vectorMapRuntime.OnMapScaleChanged = value;
            }
        }

        public OnViewExtentChangedHandler OnViewExtentChanged
        {
            get { return _vectorMapRuntime.OnViewExtentChanged; }
            set { _vectorMapRuntime.OnViewExtentChanged = value; }
        }

        #endregion

        #region ILocationService 成员

        public void GotoFeature(Feature fet)
        {
            if (fet == null)
                return;
            //锁住的目的是为了避免在定位过程中系统进行地理坐标向投影坐标的转换
            lock (fet)
            {
                ShapePoint pt = fet.Geometry.Centroid.Clone() as ShapePoint;
                if ((fet.Projected && fet.FeatureClass.OriginalCoordinateType == enumCoordinateType.Geographic) ||
                    fet.FeatureClass.OriginalCoordinateType == enumCoordinateType.Projection)
                {
                    CoordinateTransfrom.PrjCoord2GeoCoord(new ShapePoint[] { pt });
                }
                ShapePoint pt0 = pt.Clone() as ShapePoint;
                PanTo(pt);
                ReRender(delegate()
                                {
                                    CoordinateTransfrom.GeoCoord2PrjCoord(new ShapePoint[] { pt0 });
                                    _vectorMapRuntime.LocatingFocusLayer.Focus(pt, 6, 500);
                                }
                        );
            }
        }

        public void RefreshContainer() 
        {
            Refresh();
        }

        public void DoBeginInvoke(OnRenderIsFinishedHandler notify)
        {
            this.BeginInvoke(notify);
        }

        #endregion

        public void HitTest(PointF pixelPoint, out Feature feature, out RectangleF rect)
        {
            _vectorMapRuntime.HitTest(pixelPoint, out feature, out rect);
        }


        public IBookmarkManager BookmarkManager
        {
            get 
            {
                if (_bookmarkManager == null)
                    _bookmarkManager = new BookmarkManager();
                return _bookmarkManager; 
            }
            set { _bookmarkManager = value; }
        }
    }
}
