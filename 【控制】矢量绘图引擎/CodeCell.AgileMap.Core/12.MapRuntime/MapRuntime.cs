using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using CodeCell.Bricks.Runtime;
using System.Diagnostics;


namespace CodeCell.AgileMap.Core
{
    public class MapRuntime : IMapRuntime, IMapRuntimeRenderable, IFeatureRenderEnvironment, ICoordinateTransform, IDisposable
    {
        private const float cstMaxResolution = 0.5f;
        internal IMapRuntimeHost _environment = null;
        private IProjectionTransform _projectionTransform = null;
        private RuntimeExchanger _gridExchanger = null;
        private OnTransformChangedHandler _transformChanged = null;
        private int _scale = 0;
        private Matrix _worldTransform = new Matrix();
        private Matrix _invertWorldTransform = new Matrix();
        private Matrix _transformIdentity = new Matrix();
        private RectangleF _viewport = new RectangleF();
        private Envelope _viewportProjection = new Envelope();
        public const float cstMetersPerInch = 2.54f / 100f;
        //private const float cstMinResolution = 
        private Matrix _preMapTransform = null;
        private Bitmap _dummyBitmap = null;
        private Envelope _dummyEnvelope = null;
        private PointF[] _dummyPoints = new PointF[2];
        private IMap _map = null;
        private ILabelRender _labelRender = null;
        private QuickTransformArgs _quickTransfrom = null;
        private Matrix _emptyRotateMatrix = new Matrix();
        private int _id = 0;
        private static int maxId = 0;
        private IConflictor _conflictorForSymbol = null;
        private IConflictor _conflictorForLabel = null;
        private int _dpi = 96;
        //Events
        private OnMapScaleChangedHandler _onMapScaleChangedHandler = null;
        private OnViewExtentChangedHandler _onViewExtentChanged = null;
        //
        private bool _isDisposed = false;
        //比例尺条的绘制参数
        private ScaleBarArgs _scaleBarArgs = new ScaleBarArgs();
        //用户临时图层
        protected ILightLayerContainer _handinessLayerContainer = null;
        //定位焦点临时层
        protected ILocatingFocusLayer _locatingFocusLayer = null;
        protected ILocationIconLayer _locationIconLayer = null;
        //查询结果容器
        protected IQueryResultContainer _queryResultContainer = null;
        //数据到达通知
        protected IAsyncDataArrivedNotify _asyncDataArrivedNotify;
        //画布投影参数
        protected string _canvasSpatialRef;

        public MapRuntime(IMapRuntimeHost environment)
        {
            _environment = environment;
            _projectionTransform = _environment.ProjectionTransform;
            _labelRender = new LabelRender();
            _id = maxId;
            maxId++;
            _handinessLayerContainer = new LightLayerContainer(this as IMapRuntime);
            _locatingFocusLayer = new LocationFocusLayer();
            _locatingFocusLayer.Init(this as IMapRuntime);
            _locationIconLayer = new LocationIconLayer();
            _locationIconLayer.Init(this as IMapRuntime);
        }

        /// <summary>
        /// MapControl的空间参考已经发生改变
        /// </summary>
        public void ChangeProjectionTransform()
        {
            _projectionTransform = _environment.ProjectionTransform;
        }

        #region IMapRuntime Members

        public string CanvasSpatialRef
        {
            get { return _canvasSpatialRef; }
            set { _canvasSpatialRef = value; }
        }

        public IAsyncDataArrivedNotify AsyncDataArrivedNotify
        {
            get { return _asyncDataArrivedNotify; }
            set { _asyncDataArrivedNotify = value; }
        }

        public IMapRuntimeHost Host
        {
            get { return _environment; }
            set { _environment = value; }
        }

        public IQueryResultContainer QueryResultContainer
        {
            get { return _queryResultContainer; }
            set { _queryResultContainer = value; }
        }

        public ILocatingFocusLayer LocatingFocusLayer
        {
            get { return _locatingFocusLayer; }
            set
            {
                _locatingFocusLayer = value;
                if (_locatingFocusLayer != null)
                    _locatingFocusLayer.Init(this as IMapRuntime);
            }
        }

        public ILocationIconLayer LocationIconLayer
        {
            get { return _locationIconLayer; }
            set
            {
                _locationIconLayer = value;
                if (_locationIconLayer != null)
                    _locationIconLayer.Init(this as IMapRuntime);
            }
        }

        public ILightLayerContainer LightLayerContainer
        {
            get { return _handinessLayerContainer; }
        }

        public ScaleBarArgs ScaleBarArgs
        {
            get { return _scaleBarArgs; }
        }

        public OnMapScaleChangedHandler OnMapScaleChanged
        {
            get { return _onMapScaleChangedHandler; }
            set { _onMapScaleChangedHandler = value; }
        }

        public OnViewExtentChangedHandler OnViewExtentChanged
        {
            get { return _onViewExtentChanged; }
            set { _onViewExtentChanged = value; }
        }

        public int DPI
        {
            get { return _dpi; }
            set { _dpi = value; }
        }

        public RectangleF ActualViewport
        {
            get { return _viewportProjection.ToRectangleF(); }
        }

        public int Scale
        {
            get { return _scale; }
            set
            {
                if (Scale * _environment.CanvasSize.Width < (cstMaxResolution * _environment.CanvasSize.Width))
                    return;
                _scale = value;
            }
        }

        public ILocationService LocationService
        {
            get { return _environment.LocationService; }
        }

        public IMapRefresh MapRefresh
        {
            get { return _environment as IMapRefresh; }
        }

        public IRuntimeExchanger RuntimeExchanger
        {
            get { return _gridExchanger; }
        }

        public IMap Map
        {
            get { return _map; }
        }

        public void Apply(IMap map)
        {
            if (map == null)
                return;
            if (_map != null)
            {
                _map.Dispose();
                _map = null;
            }
            _map = map;
            (_map as Map).InternalInit(_projectionTransform, this as IFeatureRenderEnvironment);
            //
            if (_gridExchanger != null)
            {
                _gridExchanger.Enabled = false;
                _gridExchanger.Dispose();
                _gridExchanger = null;
            }
            //
            _gridExchanger = new RuntimeExchanger(this as IFeatureRenderEnvironment,
                _environment as IMapRefresh, _map.LayerContainer,_asyncDataArrivedNotify);
            //
            (_map as Map).InternalApplyLayers();
            //
            if (_conflictorForSymbol != null)
                _conflictorForSymbol.Dispose();
            _conflictorForSymbol = new SimpleConflictor(_map.ConflictDefForSymbol, _environment);
            if (_conflictorForLabel != null)
                _conflictorForLabel.Dispose();
            _conflictorForLabel = new PixelConflictor(_map.ConflictDefForLabel, _environment);
        }



        #endregion

        #region IFeatureRenderEnvironment Members

        public IConflictor ConflictorForSymbol
        {
            get { return _conflictorForSymbol; }
        }

        public IConflictor ConflictorForLabel
        {
            get { return _conflictorForLabel; }
        }

        public bool IsMouseBusy
        {
            get { return _environment.UseDummyMap; }
        }

        public Envelope ExtentOfProjectionCoord
        {
            get
            {
                if (_environment.CanvasSize == Size.Empty)
                    return null;
                return _viewportProjection;
            }
        }

        public OnTransformChangedHandler OnTransformChanged
        {
            get { return _transformChanged; }
            set { _transformChanged = value; }
        }

        public int CurrentScale
        {
            get { return _scale; }
        }

        public int CurrentLevel
        {
            get { return LevelDefinition.GetLevelByScale(_scale); }
        }

        public ICoordinateTransform CoordinateTransform
        {
            get { return this as ICoordinateTransform; }
        }

        #endregion

        #region IVectorMapRenderable Members

        public SmoothingMode SmoothingMode
        {
            get { return _map.MapArguments.SmoothingMode; }
            set
            {
                _map.MapArguments.SmoothingMode = value;
            }
        }

        public void Render(RenderArgs arg)
        {
            if (_map == null || _isDisposed)
                return;
            ILayerContainer layerContainer = _map.LayerContainer;
            if (layerContainer.IsEmpty() || _environment.CanvasSize == Size.Empty)
                return;
#if DEBUG
            int time = Environment.TickCount;
#endif
            bool transformIsChanged = _preMapTransform == null || !MathHelper.MatrixIsSame(_preMapTransform, arg.Graphics.Transform);
            bool envelopeIsChanged = _preFocusEnvelope == null || !_preFocusEnvelope.IsEquals(_environment.FocusEnvelope);
            transformIsChanged = transformIsChanged || envelopeIsChanged;
            bool needComputeViewportPrjAgain = true;
            //for web
            transformIsChanged = true;
            //
            if (transformIsChanged)
            {
                if (_transformChanged != null)
                    _transformChanged(this, null, null);
                UpdateViewportAndTransformMatrix();
                needComputeViewportPrjAgain = false;
            }
            Matrix oldM = arg.Graphics.Transform;
            bool needRember = false;
            try
            {
                arg.Graphics.Transform = _transformIdentity;
                if (_environment.UseDummyMap && _dummyBitmap != null)
                    RenderUseDummyBitmap(arg);
                else
                {
                    if (transformIsChanged || layerContainer.LayerIsChanged || arg.IsReRender || _dummyBitmap == null)
                    {
                        ReRender(arg, needComputeViewportPrjAgain);
                        needRember = true;
                        (layerContainer as LayerContainer).ResetLayerIsChanged();
                    }
                    else
                    {
                        RenderUseDummyBitmap(arg);
                    }
                }
            }
            finally
            {
                //画比例尺条
                if (_scaleBarArgs != null && _scaleBarArgs.Enabled)
                {
                    DrawScaleBar(arg.Graphics);
                }
                //画临时图层
                DrawHandinessLayers(arg);
                //
                arg.Graphics.Transform = oldM;
                arg.IsReRender = false;
                arg.IsRendering = false;
            }
            if (needRember)
                RemeberPreTransform(arg);
            if (envelopeIsChanged)
            {
                if (_onViewExtentChanged != null)
                    _onViewExtentChanged.BeginInvoke(this, _environment.FocusEnvelope, null, null);
            }
#if DEBUG
            time = Environment.TickCount - time;
            //Console.WriteLine("lost:" + time.ToString());
            //_environment.Container.FindForm().Text = time.ToString();
#endif
        }

        private void DrawHandinessLayers(RenderArgs arg)
        {
            //用户自定义临时层
            if (_handinessLayerContainer != null && _handinessLayerContainer.Layers != null)
            {
                foreach (ILightLayer lyr in _handinessLayerContainer.Layers)
                {
                    if (lyr.Enabled)
                        lyr.Render(arg);
                }
            }
            //系统临时层
            if (_locationIconLayer != null)
                _locationIconLayer.Render(arg);
            if (_locatingFocusLayer != null)
                _locatingFocusLayer.Render(arg);
        }

        private void RemeberPreTransform(RenderArgs arg)
        {
            if (_preMapTransform != null)
                _preMapTransform.Dispose();
            _preMapTransform = arg.Graphics.Transform.Clone();
        }

        private void ReRender(RenderArgs arg, bool needComputeViewport)
        {
            if (_dummyBitmap != null)
                _dummyBitmap.Dispose();
            _dummyBitmap = new Bitmap(_environment.CanvasSize.Width, _environment.CanvasSize.Height);
            if (needComputeViewport) //for web
                ComputeViewportProjection();
            _dummyEnvelope = ExtentOfProjectionCoord.Clone() as Envelope;
            //
            ILayerContainer layerContainer = _map.LayerContainer;
            //计算用于快速坐标变换的参数
            ComputeQuickArgs();
            //构造缓存画布
            using (Graphics g = Graphics.FromImage(_dummyBitmap))
            {
                g.SmoothingMode = _map.MapArguments.SmoothingMode;
                g.Clear(_map.MapArguments.BackColor);
                //重置冲突检测
                if (_conflictorForSymbol != null && _conflictorForSymbol.Enabled)
                    _conflictorForSymbol.Reset();
                //
                if (_conflictorForLabel != null && _conflictorForLabel.Enabled)
                {
                    _conflictorForLabel.Reset();
                    _labelRender.Begin(_conflictorForLabel, this as IFeatureRenderEnvironment);
                }
                else
                {
                    _labelRender.Begin(null, this as IFeatureRenderEnvironment);
                }
                //绘制影像层
                DrawRasterlayers(g);

                //绘制几何形状
                DrawAllGeometries(layerContainer, g);
                //绘制标注
                DrawAllLabels(layerContainer, g);
                #region debug
                //(_conflictorForLabel as PixelConflictor).Save();
                #endregion
            }
            arg.Graphics.DrawImage(_dummyBitmap, 0, 0);
        }

        private void DrawScaleBar(Graphics g)
        {
            int wbar = _scaleBarArgs.BarWidth;
            int hbar = _scaleBarArgs.BarHeight;
            //
            float m = (float)((wbar / DPI) * cstMetersPerInch * _scale / 1000f);
            string text = null;
            if (m < 1)
                text = ((int)(m * 1000)).ToString() + "米";
            else
                text = ((int)m).ToString() + "公里";
            //
            //RectangleF rect = new RectangleF(_environment.Container.Width - wbar - 10, _environment.Container.Height - hbar - 10, wbar, hbar);
            RectangleF rect = new RectangleF(_environment.CanvasSize.Width - wbar - 10, _environment.CanvasSize.Height - hbar - 10, wbar, hbar);
            using (Pen p = new Pen(Color.FromArgb(255, 255, 255)))
            {
                //
                SizeF sizef = g.MeasureString(text, _scaleBarArgs.Font);
                PointF pt = new PointF(rect.Right - (int)sizef.Width, rect.Top - (int)sizef.Height);
                LabelRender.DrawStringWithBorder(text, g, pt, _scaleBarArgs.Font, _scaleBarArgs.FontBrush, _scaleBarArgs.MaskBrush);
                //
                p.Alignment = PenAlignment.Center;
                g.DrawRectangle(p, rect.X, rect.Y, rect.Width, rect.Height);
                rect.Inflate(-1, -1);
                p.Color = Color.Black;
                g.DrawRectangle(p, rect.X, rect.Y, rect.Width, rect.Height);
                using (Brush b = new SolidBrush(Color.FromArgb(170, 203, 238)))
                {
                    rect.Inflate(-0.5f, -0.5f);
                    g.FillRectangle(b, rect);
                }
            }
        }

        private void DrawAllGeometries(ILayerContainer layerContainer, Graphics g)
        {
            int currentScale = _scale;
            try
            {
                ILayer[] layers = layerContainer.Layers;
                IFeatureLayer fetlyr = null;
                List<ILayerDrawable> twostepsLayers = new List<ILayerDrawable>();
                IFeatureRenderer preRender = null;
                foreach (ILayerDrawable lyr in layers)
                {
                    if (!(lyr is IFeatureLayer))
                        continue;
                    fetlyr = lyr as IFeatureLayer;
                    if (preRender is IFeatureTwoStepRenderer && !(fetlyr.Renderer is IFeatureTwoStepRenderer))
                        FinishTwoStep(g, twostepsLayers);
                    preRender = fetlyr.Renderer;
                    (lyr as FeatureLayer)._isRendered = false;
                    if (!lyr.Visible)
                        continue;
                    //如果图层未初始化
                    if (!fetlyr.IsReady)
                        continue;
                    //判断整个图层是否在可视区域之外
                    IFeatureClass fetclass = fetlyr.Class as IFeatureClass;
                    if (!_viewportProjection.IsInteractived(fetclass.FullEnvelope))
                        continue;
                    //判断在当前比例尺下标注是否显示
                    if (!fetlyr.VisibleAtScale(currentScale))
                        continue;
                    //绘制几何形状
                    if (fetlyr.Renderer is IFeatureTwoStepRenderer)
                    {
                        twostepsLayers.Add(lyr);
                        //第一阶段
                        (fetlyr.Renderer as IFeatureTwoStepRenderer).StepType = enumTwoStepType.Outline;
                    }
                    //渲染几何形状
                    lyr.Render(g, _quickTransfrom);
                    (lyr as FeatureLayer)._isRendered = true;
                    //
                    g.Flush(FlushIntention.Sync);
                }
                if (twostepsLayers.Count > 0)//last layer
                    FinishTwoStep(g, twostepsLayers);
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
            }
        }

        private void DrawRasterlayers(Graphics g)
        {
            //foreach (ILayer lyr in _map.LayerContainer.Layers)
            //{
            //    IRasterLayer layer = lyr as IRasterLayer;
            //    if (layer == null)
            //        continue;
            //    if (!layer.Visible || layer.IsTooSmall || !layer.VisibleAtScale(_scale))
            //        continue;
            //     layer.Render(g, null);
            //}
        }

        private void FinishTwoStep(Graphics g, List<ILayerDrawable> twostepsLayers)
        {
            if (twostepsLayers.Count > 0)
            {
                IFeatureLayer fetlyr = null;
                foreach (ILayerDrawable layer in twostepsLayers)
                {
                    fetlyr = layer as IFeatureLayer;
                    (fetlyr.Renderer as IFeatureTwoStepRenderer).StepType = enumTwoStepType.Fill;
                    layer.Render(g, _quickTransfrom);
                    g.Flush(FlushIntention.Sync);
                }
                twostepsLayers.Clear();
            }
        }

        private void DrawAllLabels(ILayerContainer layerContainer, Graphics g)
        {
            IOutsideIndicator outsideIndicator = null;
            int currentScale = _scale;
            //
            foreach (ILayerDrawable lyr in layerContainer.Layers)
            {
                if (!lyr.Visible)
                    continue;
                IFeatureLayer fetLayer = lyr as IFeatureLayer;
                if (fetLayer == null)
                    continue;
                //如果图层未初始化
                if (!fetLayer.IsReady)
                    continue;
                //判断整个图层是否在可视区域之外
                IFeatureClass fetclass = fetLayer.Class as IFeatureClass;
                if (!fetclass.FullEnvelope.IsInteractived(_viewportProjection))
                    continue;
                //判断几何形状是否显示，如果不显示，标注也不显示
                if (!fetLayer.VisibleAtScale(currentScale))
                    continue;
                //判断在当前比例尺下标注是否显示
                if (!fetLayer.LabelDef.VisibleAtScale(currentScale))
                    continue;
                //获取外部标识对象
                outsideIndicator = (fetLayer as ISupportOutsideIndicator).OutsideIndicator;
                if (outsideIndicator.IsOutside)
                    continue;
                //判断是否启用标注
                if (fetLayer.LabelDef == null || !fetLayer.LabelDef.EnableLabeling)
                    continue;
                //判断网格集合是否为空
                if (fetclass == null || fetclass.Grids == null || fetclass.Grids.Length == 0)
                    continue;
                //只对点层进行标注冲突检测
                bool conflictIsChanged = false;
                if (_conflictorForLabel != null && _conflictorForLabel.Enabled)
                {
                    conflictIsChanged = true;
                    //如果线采用注记形式，那么不做冲突检测
                    _conflictorForLabel.Enabled = !(fetclass.ShapeType == enumShapeType.Polyline && fetLayer.LabelDef.LabelSource == enumLabelSource.Annotation);
                }
                //逐网格进行标注
                int gridCount = fetclass.Grids.Length;
                for (int i = 0; i < gridCount; i++)
                {
                    if (i > gridCount - 1)
                        continue;
                    //判断网格是否可见
                    outsideIndicator = (fetclass.Grids[i] as ISupportOutsideIndicator).OutsideIndicator;
                    if (outsideIndicator.IsOutside)
                        continue;
                    //判断网格内要素是否为空
                    Feature[] fets = fetclass.Grids[i].VectorFeatures.ToArray();
                    if (fets == null || fets.Length == 0)
                        continue;
                    //解决透明背景下，绘制文本出现黑色杂点的问题
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                    foreach (Feature fet in fets)
                    {
                        //判断要素是否可见
                        outsideIndicator = (fet as ISupportOutsideIndicator).OutsideIndicator;
                        if (outsideIndicator.IsOutside)
                            continue;
                        //画标注
                        _labelRender.Draw(_emptyRotateMatrix, g, fetLayer.LabelDef as LabelDef, fetLayer.Renderer.CurrentSymbol, fet, _quickTransfrom);
                    }
                }
                //
                if (conflictIsChanged)
                    _conflictorForLabel.Enabled = true;
                g.Flush(FlushIntention.Sync);
            }
        }

        private void RenderUseDummyBitmap(RenderArgs arg)
        {
            _dummyPoints[0].X = (float)_dummyEnvelope.MinX;
            _dummyPoints[0].Y = (float)_dummyEnvelope.MaxY;
            _dummyPoints[1].X = (float)_dummyEnvelope.MaxX;
            _dummyPoints[1].Y = (float)_dummyEnvelope.MinY;
            _worldTransform.TransformPoints(_dummyPoints);
            PointF lt = _dummyPoints[0];
            PointF rb = _dummyPoints[1];
            arg.Graphics.DrawImage(_dummyBitmap, lt.X, lt.Y, rb.X - lt.X, rb.Y - lt.Y);
        }

        private PointF[] prjPt1ForQuickArgs = new PointF[2];
        private PointF[] screenPtsForQuickArgs = new PointF[2];
        private float prjDlatXForQuickArgs = 0;
        private float prjDlatYForQuickArgs = 0;
        //private void ComputeQuickArgs_OLD()
        //{
        //    if (_quickTransfrom == null)
        //    {
        //        _quickTransfrom = new QuickTransformArgs();
        //        ShapePoint geoPt1 = new ShapePoint(70, 0);
        //        ShapePoint geoPt2 = new ShapePoint(140, 70);
        //        ShapePoint[] geoPts = new ShapePoint[] { geoPt1, geoPt2 };
        //        GeoCoord2PrjCoord(geoPts);
        //        //
        //        prjPt1ForQuickArgs = geoPts[0].ToPointF();
        //        prjPt2ForQuickArgs = geoPts[1].ToPointF();
        //        screenPtsForQuickArgs = new PointF[2];
        //        prjDlatXForQuickArgs = 1f / (prjPt1ForQuickArgs.X - prjPt2ForQuickArgs.X);
        //        prjDlatYForQuickArgs = 1f / (prjPt1ForQuickArgs.Y - prjPt2ForQuickArgs.Y);
        //    }
        //    //Stopwatch sw = new Stopwatch();
        //    //sw.Start();
        //    //
        //    screenPtsForQuickArgs[0] = prjPt1ForQuickArgs;
        //    screenPtsForQuickArgs[1] = prjPt2ForQuickArgs;
        //    PrjCoord2PixelCoord(screenPtsForQuickArgs);
        //    //
        //    _quickTransfrom.kLon = (screenPtsForQuickArgs[0].X - screenPtsForQuickArgs[1].X) * prjDlatXForQuickArgs; //(screenPts[0].X - screenPts[1].X) / (prjPt1.X - prjPt2.X);
        //    _quickTransfrom.kLat = (screenPtsForQuickArgs[0].Y - screenPtsForQuickArgs[1].Y) * prjDlatYForQuickArgs; //(screenPts[0].Y - screenPts[1].Y) / (prjPt1.Y - prjPt2.Y);
        //    _quickTransfrom.bLon = screenPtsForQuickArgs[0].X - _quickTransfrom.kLon * prjPt1ForQuickArgs.X;
        //    _quickTransfrom.bLat = screenPtsForQuickArgs[0].Y - _quickTransfrom.kLat * prjPt1ForQuickArgs.Y;
        //    //sw.Stop();
        //    //long losttimes = sw.ElapsedTicks;
        //}

        private void ComputeQuickArgs()
        {
            if (_quickTransfrom == null)
                _quickTransfrom = new QuickTransformArgs();
            screenPtsForQuickArgs[0] = new Point(0, 0);
            screenPtsForQuickArgs[1] = new Point(this.Host.CanvasSize.Width, this.Host.CanvasSize.Height);
            prjPt1ForQuickArgs[0] = screenPtsForQuickArgs[0];
            prjPt1ForQuickArgs[1] = screenPtsForQuickArgs[1];
            this.PixelCoord2PrjCoord(prjPt1ForQuickArgs);
            prjDlatXForQuickArgs = 1f / (prjPt1ForQuickArgs[0].X - prjPt1ForQuickArgs[1].X);
            prjDlatYForQuickArgs = 1f / (prjPt1ForQuickArgs[0].Y - prjPt1ForQuickArgs[1].Y);
            _quickTransfrom.kLon = (screenPtsForQuickArgs[0].X - screenPtsForQuickArgs[1].X) * prjDlatXForQuickArgs; //(screenPts[0].X - screenPts[1].X) / (prjPt1.X - prjPt2.X);
            _quickTransfrom.kLat = (screenPtsForQuickArgs[0].Y - screenPtsForQuickArgs[1].Y) * prjDlatYForQuickArgs; //(screenPts[0].Y - screenPts[1].Y) / (prjPt1.Y - prjPt2.Y);
            _quickTransfrom.bLon = screenPtsForQuickArgs[0].X - _quickTransfrom.kLon * prjPt1ForQuickArgs[0].X;
            _quickTransfrom.bLat = screenPtsForQuickArgs[0].Y - _quickTransfrom.kLat * prjPt1ForQuickArgs[0].Y;
        }

        #endregion

        private int _preScale = 1;
        private void ComputeScale()
        {
            _scale = ComputeScale(_viewport);
            if (_scale == 0)
                return;
            if (_scale != _preScale)
            {
                if (_onMapScaleChangedHandler != null)
                    _onMapScaleChangedHandler(_environment, _scale);
                _preScale = _scale;
            }
        }

        private int ComputeScale(RectangleF vport)
        {
            //double pixelMeters = _environment.Container.Width * (1f / _dpi) * cstMetersPerInch;
            double pixelMeters = _environment.CanvasSize.Width * (1f / _dpi) * cstMetersPerInch;
            double viewMeters = vport.Width;
            if (pixelMeters == 0)//
            {
                return 0;
            }
            return (int)(viewMeters / pixelMeters);
        }

        private Envelope _preFocusEnvelope = null;
        private void UpdateViewportAndTransformMatrix()
        {
            Envelope crtRectOfMap = _environment.FocusEnvelope;
            if (_environment.CoordinateType == enumCoordinateType.Geographic)
            {
                PointF[] geoPts = new PointF[2];
                geoPts[0] = new PointF((float)crtRectOfMap.MinX, (float)crtRectOfMap.MaxY);
                geoPts[1] = new PointF((float)crtRectOfMap.MaxX, (float)crtRectOfMap.MinY);
                _projectionTransform.Transform(geoPts);
                PointF prjLeftUp = geoPts[0];
                PointF prjRightDown = geoPts[1];
                RectangleF oldViewport = _viewport;
                _viewport = RectangleF.FromLTRB(prjLeftUp.X, -prjLeftUp.Y, prjRightDown.X, -prjRightDown.Y);
            }
            else if (_environment.CoordinateType == enumCoordinateType.Projection)
            {
                _viewport = RectangleF.FromLTRB((float)crtRectOfMap.MinX, (float)crtRectOfMap.MinY, (float)crtRectOfMap.MaxX, (float)crtRectOfMap.MaxY);
            }
            else
            {
                throw new NotSupportedException("VectorMapRuntim不支持的坐标类型\"" + _environment.CoordinateType.ToString() + "\"。");
            }
            UpdateWorldTransform();
            ComputeScale();
            ComputeViewportProjection();
            _preFocusEnvelope = crtRectOfMap.Clone() as Envelope;
        }

        private void ComputeViewportProjection()
        {
            //PointF[] pts = new PointF[] { new PointF(0, 0), new PointF(_environment.Container.Width, _environment.Container.Height) };
            PointF[] pts = new PointF[] { new PointF(0, 0), new PointF(_environment.CanvasSize.Width, _environment.CanvasSize.Height) };
            _invertWorldTransform.TransformPoints(pts);
            _viewportProjection.MinX = Math.Min(pts[0].X, pts[1].X);
            _viewportProjection.MinY = Math.Min(pts[0].Y, pts[1].Y);
            _viewportProjection.MaxX = Math.Max(pts[0].X, pts[1].X);
            _viewportProjection.MaxY = Math.Max(pts[0].Y, pts[1].Y);
        }

        private void UpdateWorldTransform()
        {
            Size controlSize = _environment.CanvasSize;
            if (controlSize.IsEmpty || _viewport.IsEmpty)
                return;
            float pixelAspectRatio = controlSize.Width / (float)controlSize.Height;
            float projectedAspectRatio = Math.Abs(_viewport.Width / _viewport.Height);
            RectangleF adjustedViewport = _viewport;
            if (pixelAspectRatio > projectedAspectRatio)
                adjustedViewport.Inflate(
                    (pixelAspectRatio * adjustedViewport.Height - adjustedViewport.Width) / 2f,
                    0);
            else if (pixelAspectRatio < projectedAspectRatio)
                adjustedViewport.Inflate(
                    0,
                    (adjustedViewport.Width / pixelAspectRatio - adjustedViewport.Height) / 2f);
            _worldTransform.Reset();
            _worldTransform.Translate(-adjustedViewport.X, adjustedViewport.Y, MatrixOrder.Append);
            _worldTransform.Scale(controlSize.Width / adjustedViewport.Width,
                                  controlSize.Height / -adjustedViewport.Height,
                                  MatrixOrder.Append);
            _invertWorldTransform = _worldTransform.Clone();
            _invertWorldTransform.Invert();
        }

        public bool IsExceedMinResolution(RectangleF evpPrj)
        {
            return false;
        }

        public bool IsExceedMaxResolution(RectangleF evpPrj)
        {
            if (evpPrj.Width / _environment.CanvasSize.Width < cstMaxResolution)
                return true;
            return false;
        }

        public void HitTest(PointF pixelPt, out Feature feature, out RectangleF rect)
        {
            feature = null;
            rect = RectangleF.Empty;
            if (_map == null)
                return;
            ILayer[] lyrs = _map.LayerContainer.Layers;
            if (lyrs == null || lyrs.Length == 0)
                return;
            foreach (IFeatureLayer y in lyrs)
            {
                if (!(y is IFeatureLayer))
                    continue;
                if (!_map.LayerContainer.IsSelectable(y.Name))
                    continue;
                BaseFeatureRenderer r = y.Renderer as BaseFeatureRenderer;
                if (r._currentFeatureRects.Count > 0)
                {
                    foreach (Feature fet in r._currentFeatureRects.Keys)
                    {
                        rect = r._currentFeatureRects[fet];
                        if (rect.Contains(pixelPt))
                        {
                            feature = fet;
                            return;
                        }
                    }
                }
            }
        }

        public Feature[] HitTest(RectangleF rect)
        {
            if (_map == null)
                return null;
            ILayer[] lyrs = _map.LayerContainer.Layers;
            if (lyrs == null || lyrs.Length == 0)
                return null;
            List<Feature> fets = new List<Feature>();
            foreach (IFeatureLayer y in lyrs)
            {
                if (!(y is IFeatureLayer))
                    continue;
                if (!_map.LayerContainer.IsSelectable(y.Name))
                    continue;
                BaseFeatureRenderer r = y.Renderer as BaseFeatureRenderer;
                if (r._currentFeatureRects.Count > 0)
                {
                    foreach (Feature fet in r._currentFeatureRects.Keys)
                    {
                        rect = r._currentFeatureRects[fet];
                        if (rect.Contains(rect))
                        {
                            fets.Add(fet);
                        }
                    }
                }
            }
            return fets.Count > 0 ? fets.ToArray() : null;
        }

        public Feature HitTestByPrj(ShapePoint prjPoint)
        {
            ILayer[] lyrs = _map.LayerContainer.Layers;
            if (lyrs == null || lyrs.Length == 0)
                return null;
            foreach (IFeatureLayer y in lyrs)
            {
                if (!(y is IFeatureLayer))
                    continue;
                if (!_map.LayerContainer.IsSelectable(y.Name))
                    continue;
                BaseFeatureRenderer r = y.Renderer as BaseFeatureRenderer;
                if (r._currentFeatureRects.Count > 0)
                {
                    foreach (Feature fet in r._currentFeatureRects.Keys)
                    {
                        if (fet.Geometry == null)
                            continue;
                        if (fet.Geometry.Contains(prjPoint))
                            return fet;
                    }
                }
            }
            return null;
        }

        public Feature[] HitTestByPrj(Envelope prjRect)
        {
            ILayer[] lyrs = _map.LayerContainer.Layers;
            if (lyrs == null || lyrs.Length == 0)
                return null;
            List<Feature> fets = new List<Feature>();
            foreach (IFeatureLayer y in lyrs)
            {
                if (!(y is IFeatureLayer))
                    continue;
                if (!_map.LayerContainer.IsSelectable(y.Name))
                    continue;
                BaseFeatureRenderer r = y.Renderer as BaseFeatureRenderer;
                if (r._currentFeatureRects.Count > 0)
                {
                    foreach (Feature fet in r._currentFeatureRects.Keys)
                    {
                        if (fet.Geometry == null)
                            continue;
                        if (prjRect.Contains(fet.Geometry.Envelope))
                            fets.Add(fet);
                    }
                }
            }
            return fets.Count > 0 ? fets.ToArray() : null;
        }

        #region ICoordinateTransform Members

        public void PixelCoord2PrjCoord(Point[] points)
        {
            _invertWorldTransform.TransformPoints(points);
        }

        public void PixelCoord2PrjCoord(PointF[] points)
        {
            _invertWorldTransform.TransformPoints(points);
        }

        public void PixelCoord2PrjCoord(ref Point point)
        {
            Point[] pts = new Point[1] { point };
            PixelCoord2PrjCoord(pts);
            point.X = pts[0].X;
            point.Y = pts[0].Y;
        }

        public double GetPixelTolerance(int pixel)
        {
            Point pt1 = new Point(0, 0);
            Point pt2 = new Point(1, 0);
            Point[] pts = new Point[] { pt1, pt2 };
            PixelCoord2PrjCoord(pts);
            return Math.Abs(pts[1].X - pts[0].X);
        }

        public void GeoCoord2PixelCoord(PointF[] points)
        {
            _projectionTransform.Transform(points);
            //
            _worldTransform.TransformPoints(points);
        }

        public unsafe PointF[] GeoCoord2PixelCoord(ShapePoint[] points)
        {
            PointF[] ptfs = new PointF[points.Length];
            fixed (PointF* ptr = ptfs)
            {
                PointF* pt = ptr;
                for (int i = 0; i < ptfs.Length; i++, pt++)
                {
                    pt->X = (float)points[i].X;
                    pt->Y = (float)points[i].Y;
                }
            }
            GeoCoord2PixelCoord(ptfs);
            return ptfs;
        }

        public unsafe void GeoCoord2PrjCoord(ShapePoint[] points)
        {
            _projectionTransform.Transform(points);
        }

        public PointF[] PrjCoord2PixelCoord(ShapePoint[] points)
        {
            PointF[] pts = new PointF[points.Length];
            for (int i = 0; i < pts.Length; i++)
                pts[i] = new PointF((float)points[i].X, (float)points[i].Y);
            _worldTransform.TransformPoints(pts);
            return pts;
        }

        public void PrjCoord2PixelCoord(PointF[] points)
        {
            _worldTransform.TransformPoints(points);
        }

        public void PrjCoord2GeoCoord(ShapePoint[] points)
        {
            _projectionTransform.InverTransform(points);
        }

        public void PrjCoord2GeoCoord(PointF[] points)
        {
            _projectionTransform.InverTransform(points);
        }

        public void PrjCoord2GeoCoord(ref PointF pointf)
        {
            _projectionTransform.InverTransform(ref pointf);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            _isDisposed = true;
            if (_map != null)
            {
                _map.Dispose();
                //_map = null;
            }
            if (_dummyBitmap != null)
            {
                _dummyBitmap.Dispose();
                _dummyBitmap = null;
            }
            if (_invertWorldTransform != null)
            {
                _invertWorldTransform.Dispose();
                _invertWorldTransform = null;
            }
            if (_worldTransform != null)
            {
                _worldTransform.Dispose();
                _worldTransform = null;
            }
            if (_gridExchanger != null)
            {
                _gridExchanger.Dispose();
                _gridExchanger = null;
            }
            if (_transformIdentity != null)
            {
                _transformIdentity.Dispose();
                _transformIdentity = null;
            }
            if (_emptyRotateMatrix != null)
            {
                _emptyRotateMatrix.Dispose();
                _emptyRotateMatrix = null;
            }
            if (_scaleBarArgs != null)
            {
                _scaleBarArgs.Dispose();
                _scaleBarArgs = null;
            }
        }

        #endregion
    }
}
