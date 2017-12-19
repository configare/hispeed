using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CodeCell.AgileMap.WebComponent.AgileMapServiceProxy;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace CodeCell.AgileMap.WebComponent
{
    public partial class MapControl : UserControl, IMapControl
    {
        private MapServiceClient _mapsrv = null;
        private enumMapTools _currentToolType = enumMapTools.Pan;
        private IMapTool _currentMapTool = null;
        private PrjRectangleF _viewport = new PrjRectangleF();
        private Transform _prj2PixelTransform = null;
        private Transform _pixel2PrjTransform = null;
        private const float cstMetersPerInch = 2.54f / 100f;
        private double _resolutionX = 0;
        private double _resolutionY = 0;
        private IMapServerAgent _mapServerAgent = null;
        private Map _map = null;
        private IQueryResultContainer _queryResultContainer = null;
        private double _offsetX = 0;
        private double _offsetY = 0;
        private ICurrentCoordDisplayer _currentCoordDisplayer = null;
        private readonly string cstInsideRequestIdentify = null;
        private OnMapControlViewportChangedHandler _onMapControlViewportChanged = null;
        private int _scale = 0;
        private bool _isFirstLoad = true;

        public MapControl()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(MapControl_Loaded);
            this.SizeChanged += new SizeChangedEventHandler(MapControl_SizeChanged);
            MouseMove += new MouseEventHandler(MapControl_MouseMove);
            MarkerSymbol.mapControl = this;
            cstInsideRequestIdentify = "mapcontrol_" + Guid.NewGuid();
            MapToolFactory.Init(this);
        }

        public Canvas Canvas
        {
            get { return canvas; }
        }

        public ICurrentCoordDisplayer CurrentCoordDisplayer
        {
            get { return _currentCoordDisplayer; }
            set { _currentCoordDisplayer = value; }
        }

        public OnMapControlViewportChangedHandler OnMapControlViewportChanged
        {
            get { return _onMapControlViewportChanged; }
            set { _onMapControlViewportChanged = value; }
        }

        void MapControl_MouseMove(object sender, MouseEventArgs e)
        {
            Point prjPt = Pixel2Prj(e.GetPosition(this));
            Point pixelPt = Prj2Pixel(prjPt);
            _currentCoordDisplayer.DisplayCoord(prjPt);
        }

        void MapControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActualWidth < double.Epsilon || ActualHeight < double.Epsilon)
                return;
            canvas.Width = ActualWidth;
            canvas.Height = ActualHeight;
            mapimage.Width = ActualWidth;
            mapimage.Height = ActualHeight;
            cliprect.Rect = new Rect(0, 0, ActualWidth, ActualHeight);
            RefreshMap();
        }

        private void CreateMapServiceClient()
        {
            _mapsrv = _mapServerAgent.MapServiceClient;
        }

        void MapControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isFirstLoad)
                return;
            try
            {
                canvas.Tag = this as IMapControl;
                SetMapToolByCurrentType();
                SetViewportByGeo(-180, 180, -90, 90);
                _isFirstLoad = false;
            }
            catch
            { }
        }

        public IQueryResultContainer QueryResultContainer
        {
            get { return _queryResultContainer; }
            set 
            {
                _queryResultContainer = value;
                if (_queryResultContainer != null)
                    _queryResultContainer.SetMapControl(this);
            }
        }

        public Map Map
        {
            get { return _map; }
        }

        public IMapServerAgent MapServerAgent
        {
            get { return _mapServerAgent; }
            set
            {
                _mapServerAgent = value;
                if (_mapServerAgent != null)
                {
                    CreateMapServiceClient();
                    CreateMap();
                    AttachEventsToMapServiceClient();
                    RefreshMap();
                }
            }
        }

        private void CreateMap()
        {
            using (MapCreator c = new MapCreator())
            {
                _map = c.CreateMap(_mapServerAgent, canvas, this);
            }
        }

        public double Scale
        {
            get { return _scale; }
        }

        public double Resolution
        {
            get { return _resolutionX; }
        }

        public enumMapTools CurrentMapToolType
        {
            get { return _currentToolType; }
            set
            {
                _currentToolType = value;
                SetMapToolByCurrentType();
            }
        }

        public IMapTool CurrentMapTool
        {
            get { return _currentMapTool; }
            set
            {
                if (_currentMapTool != null)
                    (_currentMapTool as IMapToolInternal).Deactive();
                _currentMapTool = value;
                if (_currentMapTool != null)
                {
                    (_currentMapTool as IMapToolInternal).Active();
                    _currentToolType = enumMapTools.Custom;
                }
                else
                    _currentToolType = enumMapTools.None;
            }
        }

        private void SetMapToolByCurrentType()
        {
            IMapCommand cmd = MapToolFactory.GetMapTool(_currentToolType);
            if(!(cmd is IMapTool))
                return ;
            if (_currentMapTool != null)
                (_currentMapTool as IMapToolInternal).Deactive();
            _currentMapTool = cmd as IMapTool;
            if (_currentMapTool == null)
                _currentToolType = enumMapTools.None;
            else
                (_currentMapTool as IMapToolInternal).Active();
        }

        public IMapCommand FindSystemMapTool(enumMapTools maptooltype)
        {
            return MapToolFactory.GetMapTool(maptooltype);
        }

        private void ComputeResolutionAndPrjTransform(bool needComputeResolution)
        {
            if (mapimage.ActualWidth < double.Epsilon || mapimage.ActualHeight < double.Epsilon)
                return;
            if (_viewport.Width < double.Epsilon || _viewport.Height < double.Epsilon)
                return;
            if (needComputeResolution)
                ComputeResolution();
            ComputePrjTransfrom();
            RefreshLocationOfMarkerFeatures();
        }

        private void ComputeResolution()
        {
            _resolutionX = _viewport.Width / mapimage.ActualWidth;
            _resolutionY = _viewport.Height / mapimage.ActualHeight;
            _scale = ComputeScale();
        }

        private int ComputeScale()
        {
            double pixelMeters = mapimage.ActualWidth * (1f / 96) * cstMetersPerInch;
            double viewMeters = _viewport.Width;
            if (pixelMeters == 0)//
            {
                return 0;
            }
            return (int)(viewMeters / pixelMeters);
        }

        private void ComputePrjTransfrom()
        {
            PrjRectangleF newViewport = _viewport;
            //如果_offsetX与_offsetY不为0，则表示地图正在漫游
            newViewport.Offset(-_offsetX, _offsetY);
            if (_onMapControlViewportChanged != null)
                _onMapControlViewportChanged(this, _viewport, newViewport);
            TranslateTransform translateMatrix = null;
            ScaleTransform scaleMatrix = null;
            if (_prj2PixelTransform == null)
            {
                TransformGroup tran = new TransformGroup();
                translateMatrix = new TranslateTransform();
                scaleMatrix = new ScaleTransform();
                ScaleTransform negMatrix = new ScaleTransform();
                negMatrix.ScaleX = 1;
                negMatrix.ScaleY = -1;

                tran.Children.Add(negMatrix);
                tran.Children.Add(translateMatrix);
                tran.Children.Add(scaleMatrix);
                //
                translateMatrix.X = -newViewport.MinX;//left
                translateMatrix.Y = newViewport.MaxY;//top
                scaleMatrix.ScaleX = 1d / _resolutionX;
                scaleMatrix.ScaleY = 1d / _resolutionY;
                //
                _prj2PixelTransform = tran;
            }
            else
            {
                translateMatrix = (_prj2PixelTransform as TransformGroup).Children[1] as TranslateTransform;
                scaleMatrix = (_prj2PixelTransform as TransformGroup).Children[2] as ScaleTransform;
                //
                translateMatrix.X = -newViewport.MinX;
                translateMatrix.Y = newViewport.MaxY;
                scaleMatrix.ScaleX = 1d / _resolutionX;
                scaleMatrix.ScaleY = 1d / _resolutionY;
            }
            _pixel2PrjTransform = _prj2PixelTransform.Inverse as Transform;
        }

        public Point Prj2Pixel(Point prjPt)
        {
            //double x = (prjPt.X - _viewport.MinX) / _resolutionX;
            //double y = (_viewport.MaxY - prjPt.Y) / _resolutionY;
            //return new Point(x, y);
            if (_prj2PixelTransform == null)
                return new Point();
            return _prj2PixelTransform.Transform(prjPt);
        }

        public Point Pixel2Prj(Point pixelPt)
        {
            //double x = _viewport.MinX + pixelPt.X * _resolutionX;
            //double y = _viewport.MaxY - pixelPt.Y * _resolutionY;
            //return new Point(x, y);
            if (_pixel2PrjTransform == null)
                return new Point();
            return _pixel2PrjTransform.Transform(pixelPt);
        }

        public PrjRectangleF Viewport
        {
            get { return _viewport; }
            set
            {
                _viewport = value;
            }
        }

        public Transform Prj2PixelTransform
        {
            get { return _prj2PixelTransform; }
        }

        private void AttachEventsToMapServiceClient()
        {
            _mapsrv.GeoEnvelope2PrjEnvelopeCompleted += new EventHandler<GeoEnvelope2PrjEnvelopeCompletedEventArgs>(_mapsrv_GeoEnvelope2PrjEnvelopeCompleted);
            _mapsrv.GetMapImageCompleted += new EventHandler<GetMapImageCompletedEventArgs>(_mapsrv_GetMapImageCompleted);
            _mapsrv.Geo2PrjCompleted += new EventHandler<Geo2PrjCompletedEventArgs>(_mapsrv_Geo2PrjCompleted);
            _mapsrv.IdentifyCompleted += new EventHandler<IdentifyCompletedEventArgs>(_mapsrv_IdentifyCompleted);
        }

        bool IsResponseFromInsideRequested(object userState)
        {
            if (userState == null || userState.ToString() != cstInsideRequestIdentify)
                return false;
            return true;
        }

        void _mapsrv_Geo2PrjCompleted(object sender, Geo2PrjCompletedEventArgs e)
        {
            if (e.UserState == null)
                return;
            ObservableCollection<AgileMapServiceProxy.PointF> pts = e.Result;
            if (pts == null || pts.Count == 0)
                return;
            if (e.UserState.ToString() == "goto")
                GotoByPrj(pts[0]);
            else if (e.UserState.ToString().Contains("radius:"))
            {
                double radius = double.Parse(e.UserState.ToString().Replace("radius:", string.Empty));
                GotoByPrj(pts[0], radius);
            }
        }

        private void GotoByPrj(PointF pt, double radius)
        {
            _viewport.MinX = pt.x - radius;
            _viewport.MaxX = pt.x + radius;
            _viewport.MinY = pt.y - radius;
            _viewport.MaxY = pt.y + radius;
            RefreshMap();
        }

        private void GotoByPrj(PointF pt)
        {
            double cx = (_viewport.MinX + _viewport.MaxX) / 2;
            double cy = (_viewport.MinY + _viewport.MaxY) / 2;
            double offsetX = pt.x - cx;
            double offsetY = pt.y - cy;
            _viewport.Offset(offsetX, offsetY);
            RefreshMap();
        }

        void _mapsrv_GeoEnvelope2PrjEnvelopeCompleted(object sender, GeoEnvelope2PrjEnvelopeCompletedEventArgs e)
        {
            if (!IsResponseFromInsideRequested(e.UserState))
                return;
            string rect = e.Result;
            if (string.IsNullOrEmpty(rect))
                return;
            string[] ds = rect.Split(',');
            _viewport.MinX = double.Parse(ds[0]);
            _viewport.MaxX = _viewport.MinX + double.Parse(ds[2]);
            _viewport.MinY = double.Parse(ds[1]);
            _viewport.MaxY = _viewport.MinY + double.Parse(ds[3]);
            ComputeResolutionAndPrjTransform(true);
            RefreshMap();
        }

        void _mapsrv_GetMapImageCompleted(object sender, GetMapImageCompletedEventArgs e)
        {
            if (!IsResponseFromInsideRequested(e.UserState))
                return;
            MapImage retMapImage = e.Result;
            if (retMapImage != null)
            {
                //reset pan offset
                mapimage.RenderTransform = new TranslateTransform();
                _offsetX = 0;
                _offsetY = 0;
                //
                BitmapImage bi = new BitmapImage(new Uri(retMapImage.ImageUrl));
                mapimage.Source = bi;
                _viewport.MinX = retMapImage.Left;
                _viewport.MinY = retMapImage.Bottom;
                _viewport.MaxX = retMapImage.Left + retMapImage.Width;
                _viewport.MaxY = retMapImage.Bottom + retMapImage.Height;
                ComputeResolutionAndPrjTransform(true);
            }
        }

        public void RefreshMap()
        {
            if (mapimage.ActualWidth < double.Epsilon || mapimage.ActualHeight < double.Epsilon)
                return;
            if (_mapsrv == null)
                return;
            Transform oldTransform = mapimage.RenderTransform;
            TranslateTransform oldtran = oldTransform as TranslateTransform;
            double x = 0;
            double y = 0;
            if (oldtran != null)
            {
                x = oldtran.X * _resolutionX;
                y = oldtran.Y * _resolutionY;
            }
            _viewport.Offset(-x, y);
            _mapsrv.GetMapImageAsync(_viewport.MinX,
                                     _viewport.MinY,
                                     _viewport.Width,
                                     _viewport.Height,
                                     (int)mapimage.ActualWidth,
                                     (int)mapimage.ActualHeight,
                                     GetInvisableLayerIds(),
                                     cstInsideRequestIdentify);
        }

        private ObservableCollection<string> GetInvisableLayerIds()
        {
            ObservableCollection<string> lyrIds = new ObservableCollection<string>();
            foreach (Layer lyr in _map.Layers)
                if (lyr is ServiceFeatureLayer && !lyr.Visible)
                    lyrIds.Add(lyr.Id);
            return lyrIds;
        }

        public void SetViewportByGeo(double minLon, double maxLon, double minLat, double maxLat)
        {
            _mapsrv.GeoEnvelope2PrjEnvelopeAsync(minLon, maxLon, minLat, maxLat, cstInsideRequestIdentify);
        }

        public void SetViewportByPrj(PrjRectangleF viewport)
        {
            _viewport = viewport;
            RefreshMap();
        }

        public void Goto(Point geoPt)
        {
            ObservableCollection<AgileMapServiceProxy.PointF> pts = new ObservableCollection<PointF>();
            AgileMapServiceProxy.PointF pt = new PointF();
            pt.x = (float)geoPt.X;
            pt.y = (float)geoPt.Y;
            pts.Add(pt);
            _mapsrv.Geo2PrjAsync(pts, "goto");
        }

        public void PanTo(Point geoPt, double radius)
        {
            ObservableCollection<AgileMapServiceProxy.PointF> pts = new ObservableCollection<PointF>();
            AgileMapServiceProxy.PointF pt = new PointF();
            pt.x = (float)geoPt.X;
            pt.y = (float)geoPt.Y;
            pts.Add(pt);
            _mapsrv.Geo2PrjAsync(pts, "radius:" + radius.ToString());
        }

        private void canvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_currentMapTool != null)
                _currentMapTool.MouseDown(sender, e);
        }

        private void canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_currentMapTool != null)
                _currentMapTool.MouseUp(sender, e);
        }

        private void canvas_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_currentMapTool != null)
                _currentMapTool.MouseRightDown(sender, e);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_currentMapTool != null)
                _currentMapTool.MouseMove(sender, e);
        }

        private void canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_currentMapTool != null)
                _currentMapTool.MouseWheel(sender, e);
        }

        public void Offset(double offsetX, double offsetY)
        {
            Transform oldTransform = mapimage.RenderTransform;
            TranslateTransform oldtran = oldTransform as TranslateTransform;
            TranslateTransform tran = new TranslateTransform();
            double x = offsetX;
            double y = offsetY;
            if (oldtran != null)
            {
                x += oldtran.X;
                y += oldtran.Y;
            }
            tran.X = x;
            tran.Y = y;
            mapimage.RenderTransform = tran;
            //
            _offsetX = x * _resolutionX;
            _offsetY = y * _resolutionY;
            ComputeResolutionAndPrjTransform(false);
        }

        private void RefreshLocationOfMarkerFeatures()
        {
            if (_map == null || _map.Layers == null || _map.Layers.Count == 0)
                return;
            foreach (Layer lyr in _map.Layers)
            {
                if (lyr == null || !(lyr is DynamicFeatureLayer))
                    continue;
                DynamicFeatureLayer dLyr = lyr as DynamicFeatureLayer;
                if (dLyr.Features == null || dLyr.Features.Count == 0)
                    continue;
                foreach (Feature fet in dLyr.Features)
                {
                    if (fet.Symbol == null || !(fet.Symbol is MarkerSymbol))
                        continue;
                    (fet.Symbol as MarkerSymbol).UpatePixelLocation(Resolution);
                }
            }
        }

        void _mapsrv_IdentifyCompleted(object sender, IdentifyCompletedEventArgs e)
        {
            if (e.UserState == null || !(e.UserState is IdentifyState) || e.Result == null)
                return;
            IdentifyState state = e.UserState as IdentifyState;
            foreach (FeatureInfo fetinfo in e.Result)
                state.Fets.Add(Feature.FromFeatureInfo(fetinfo));
        }

        public void Identify(ObservableCollection<Feature> fets, Point prjPt, double tolerance)
        { 
            PointF pt = new PointF();
            pt.x = (float)prjPt.X ;
            pt.y = (float)prjPt.Y ;
            _mapsrv.IdentifyAsync(null, pt, tolerance,new IdentifyState(fets));
        }
    }

    class IdentifyState
    {
        public ObservableCollection<Feature> Fets = null;

        public IdentifyState(ObservableCollection<Feature> fets)
        {
            Fets = fets;
        }
    }
}
