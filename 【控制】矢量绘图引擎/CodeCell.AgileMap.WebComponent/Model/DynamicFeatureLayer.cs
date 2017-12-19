using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace CodeCell.AgileMap.WebComponent
{
    public class DynamicFeatureLayer : FeatureLayer,ISymbolSelector
    {
        class FeatureUserState
        {
            public Feature Feature = null;
            public string LayerId = null;
       
            public FeatureUserState(Feature feature, string layerId)
            {
                Feature = feature;
                LayerId = layerId;
            }
        }
        //
        private ObservableCollection<Feature> _features = new ObservableCollection<Feature>();
        private Canvas _dynamicLayerCanvas = null;
        private bool _isAttachedEvent = false;
        private Symbol _symbol = null;
        private ISymbolSelector _symbolSelector = null;
        private List<object> _uiElements = new List<object>();

        internal DynamicFeatureLayer(Canvas dynamicLayerElement)
            : base()
        {
            _symbolSelector = this;//default symbolselector
            _dynamicLayerCanvas = dynamicLayerElement;
            _features.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_features_CollectionChanged);
        }

        void _features_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            GenerateXamlObjects(e);
            if (OnFeatureCollectionChanged != null)
                OnFeatureCollectionChanged(this);
        }

        public Symbol Symbol
        {
            get { return _symbol; }
            set { _symbol = value; }
        }

        public ISymbolSelector SymbolSelector
        {
            get { return _symbolSelector; }
            set { _symbolSelector = value; }
        }

        public Canvas Canvas
        {
            get { return _dynamicLayerCanvas; }
        }

        public override bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
                SetVisible(base.Visible);
            }
        }

        private void SetVisible(bool vis)
        {
            if (vis)
                foreach (UIElement ele in _uiElements)
                    ele.Visibility = Visibility.Visible;
            else
                foreach (UIElement ele in _uiElements)
                    ele.Visibility = Visibility.Collapsed;
        }

        internal List<object> UIElements
        {
            get { return _uiElements; }
        }

        public ObservableCollection<Feature> Features
        {
            get { return _features; }
        }

        public void RefreshPointFeaturesLocation()
        {
            if (_features == null || _features.Count == 0)
                return;
            foreach (Feature fet in _features)
            {
                if (fet.Symbol != null && fet.Symbol is MarkerSymbol)
                    (fet.Symbol as MarkerSymbol).UpatePixelLocation(_map.MapControl.Resolution);
            }
        }

        public override Feature[] Query(QueryFilter filter)
        {
            return base.Query(filter);
        }

        private void GenerateXamlObjects(NotifyCollectionChangedEventArgs e)
        {
            AttachGeo2PrjCompleteEvents();
            if (e.OldItems != null && e.OldItems.Count > 0)
            {
                foreach (Feature fet in e.OldItems)
                {
                    if (fet.XamlObject != null)
                    {
                        _dynamicLayerCanvas.Children.Remove(fet.XamlObject as UIElement);
                        if (_uiElements.Contains(fet.XamlObject))
                            _uiElements.Remove(fet.XamlObject);
                    }
                }
            }
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (Feature fet in e.NewItems)
                {
                    if (fet.Geometry == null)
                        continue;
                    if (fet.Geometry.CoordType == enumCoordTypes.Geographics)
                        TryConvertGeoCoord2PrjCoord(fet);
                    else
                        GenerateXamlObjAndAddSilverlightVisibleTree(fet);
                }
            }
        }

        private void AttachGeo2PrjCompleteEvents()
        {
            if (!_isAttachedEvent)
            {
                _map.ServerAgent.MapServiceClient.Geo2PrjCompleted += new EventHandler<AgileMapServiceProxy.Geo2PrjCompletedEventArgs>(MapServiceClient_Geo2PrjCompleted);
                _isAttachedEvent = true;
            }
        }

        void MapServiceClient_Geo2PrjCompleted(object sender, AgileMapServiceProxy.Geo2PrjCompletedEventArgs e)
        {
            if (e.UserState == null || !(e.UserState is FeatureUserState))
                return;
            FeatureUserState ustate = e.UserState as FeatureUserState;
            if (ustate.LayerId != Id)
                return;
            ObservableCollection<AgileMapServiceProxy.PointF> prjedPoints = e.Result;
            IProjectableArguments arguments = ustate.Feature.Geometry as IProjectableArguments;
            int pointCount = 0;
            if (arguments.Points != null)
            {
                pointCount = arguments.Points.Length;
                for (int i = 0; i < arguments.Points.Length; i++)
                {
                    arguments.Points[i].X = prjedPoints[i].x;
                    arguments.Points[i].Y = prjedPoints[i].y;
                }
            }
            if (arguments.SingleValues != null)
                for (int i = pointCount; i < prjedPoints.Count; i++)
                    arguments.SingleValues[i - pointCount] = prjedPoints[i].x;
            GenerateXamlObjAndAddSilverlightVisibleTree(ustate.Feature);
        }

        private void TryConvertGeoCoord2PrjCoord(Feature fet)
        {
            /*
             * 1.调用地图服务中的坐标转换接口进行地理坐标到投影坐标的转换
             * 2.在异步转换事件中，生成XamlObject,往画布中添加
             */
            GeoShape geometry = fet.Geometry;
            if (fet.Geometry == null)
                return;
            IProjectableArguments arguments = fet.Geometry as IProjectableArguments;
            if (arguments == null)
                throw new NotImplementedException("几何类型\"" + geometry.GetType().ToString() + "\"未实现IProjectableArguments,无法进行投影转换。");
            //
            if ((arguments.Points == null && arguments.Points.Length == 0) &&
                (arguments.SingleValues == null && arguments.SingleValues.Length == 0))
                return;
            List<AgileMapServiceProxy.PointF> points = new List<AgileMapServiceProxy.PointF>();
            if (arguments.Points != null)
            {
                foreach (GeoPoint pt in arguments.Points)
                {
                    AgileMapServiceProxy.PointF ptf = new AgileMapServiceProxy.PointF();
                    ptf.x = (float)pt.X;
                    ptf.y = (float)pt.Y;
                    points.Add(ptf);
                }
            }
            int valueCount = 0;
            if (arguments.SingleValues != null)
            {
                valueCount = arguments.SingleValues.Length;
                foreach (double v in arguments.SingleValues)
                {
                    AgileMapServiceProxy.PointF ptf = new AgileMapServiceProxy.PointF();
                    ptf.x = (float)v;
                    ptf.y = 0;
                    points.Add(ptf);
                }
            }
            //
            _map.ServerAgent.MapServiceClient.Geo2PrjAsync(new ObservableCollection<AgileMapServiceProxy.PointF>(points),
                new FeatureUserState(fet, Id));
        }

        private void GenerateXamlObjAndAddSilverlightVisibleTree(Feature fet)
        {
            if (_symbolSelector != null && fet.Symbol == null)
            {
                Symbol sym  = _symbolSelector.GetSymbol(fet);
                if (sym is MarkerSymbol)
                    sym = (sym as MarkerSymbol).Clone();
                fet.Symbol = sym;
            }
            //
            UIElement fetUIElement = null;
            fet.SetDynamicLayer(this);
            if (fet.Symbol is MarkerSymbol && fet.Geometry is GeoPoint)
            {
                fet.ApplySymbol();
                fetUIElement = fet.XamlObject as UIElement;
            }
            else
            {
                TrySetDefaultSymbol(fet);
                if (fet.XamlObject != null)
                {
                    if (fet.XamlObject is Path)
                    {
                        Path pth = fet.XamlObject as Path;
                        pth.Data.Transform = _map.MapControl.Prj2PixelTransform;
                        fet.ApplySymbol();
                    }
                    fetUIElement = fet.XamlObject as UIElement;
                    _dynamicLayerCanvas.Children.Add(fetUIElement);
                    _uiElements.Add(fetUIElement);
                    AttachEventToFeature(fet);
                }
            }
        }

        internal void AttachEventToFeature(Feature fet)
        {
            UIElement uiElement = fet.XamlObject as UIElement;
            if (uiElement == null)
                return;
            (uiElement as FrameworkElement).Tag = fet;
            uiElement.MouseLeftButtonDown += new MouseButtonEventHandler(fetUIElement_MouseLeftButtonDown);
            uiElement.MouseLeftButtonUp += new MouseButtonEventHandler(fetUIElement_MouseLeftButtonUp);
            uiElement.MouseRightButtonDown += new MouseButtonEventHandler(fetUIElement_MouseRightButtonDown);
            uiElement.MouseRightButtonUp += new MouseButtonEventHandler(fetUIElement_MouseRightButtonUp);
            uiElement.MouseEnter += new MouseEventHandler(fetUIElement_MouseEnter);
            uiElement.MouseLeave += new MouseEventHandler(fetUIElement_MouseLeave);
        }

        void fetUIElement_MouseLeave(object sender, MouseEventArgs e)
        {
            if (OnMouseOverFeature != null)
                OnMouseOverFeature(this,
                                    (sender as FrameworkElement).Tag as Feature,
                                    enumMouseDirections.MouseLeave,
                                    e);
        }

        void fetUIElement_MouseEnter(object sender, MouseEventArgs e)
        {
            if (OnMouseOverFeature != null)
                OnMouseOverFeature(this,
                                    (sender as FrameworkElement).Tag as Feature,
                                    enumMouseDirections.MouseEnter,
                                    e);
        }

        void fetUIElement_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (OnFeatureClicked != null)
                OnFeatureClicked(this,
                                 (sender as FrameworkElement).Tag as Feature,
                                 enumMouseButtons.RightButton,
                                 enumMouseActions.MouseUp,
                                 e);
        }

        void fetUIElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (OnFeatureClicked != null)
                OnFeatureClicked(this,
                                 (sender as FrameworkElement).Tag as Feature,
                                 enumMouseButtons.LeftButton,
                                 enumMouseActions.MouseUp,
                                 e);
        }

        void fetUIElement_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (OnFeatureClicked != null)
                OnFeatureClicked(this,
                                 (sender as FrameworkElement).Tag as Feature,
                                 enumMouseButtons.RightButton,
                                 enumMouseActions.MouseDown,
                                 e);
        }

        void fetUIElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (OnFeatureClicked != null)
                OnFeatureClicked(this,
                                 (sender as FrameworkElement).Tag as Feature,
                                 enumMouseButtons.LeftButton,
                                 enumMouseActions.MouseDown,
                                 e);
        }

        private void TrySetDefaultSymbol(Feature fet)
        {
            if (fet.Symbol == null)
            {
                if (fet.Geometry is GeoPoint)
                    fet.Symbol = new FillSymbol();
                if (fet.Geometry is GeoPolyline)
                    fet.Symbol = new LineSymbol();
                else if (fet.Geometry is GeoPolygon)
                    fet.Symbol = new FillSymbol();
                else if (fet.Geometry is GeoPieArea)
                    fet.Symbol = new FillSymbol();
                else if (fet.Geometry is GeoRectangle)
                    fet.Symbol = new FillSymbol();
                else if (fet.Geometry is GeoEllipse)
                    fet.Symbol = new FillSymbol();
            }
        }

        public Symbol GetSymbol(Feature feature)
        {
            return _symbol;
        }
    }
}
