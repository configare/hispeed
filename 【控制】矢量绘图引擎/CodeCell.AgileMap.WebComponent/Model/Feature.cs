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
using System.Collections.Generic;

namespace CodeCell.AgileMap.WebComponent
{
    public class Feature:AgileMapEntity
    {
        private static int MaxOID = 0 ;
        private int _oid = 0;
        private Dictionary<string, string> _properties = new Dictionary<string, string>();
        private GeoShape _geometry = null;
        private object _xamlObj = null;
        private Symbol _symbol = null;
        private DynamicFeatureLayer _dLayer = null;
        private AgileMapServiceProxy.Envelope _envelope = null;

        public Feature()
            : base()
        {
            _oid = MaxOID;
            MaxOID++;
        }

        public Feature(int oid)
            :base()
        {
            _oid = oid;
        }

        public AgileMapServiceProxy.Envelope Envelope
        {
            get { return _envelope; }
            set { _envelope = value; }
        }

        public Symbol Symbol
        {
            get { return _symbol; }
            set 
            {
                _symbol = value;
                UpdatePrjLocationOfSymbol();
                ApplySymbol();
            }
        }

        internal void SetDynamicLayer(DynamicFeatureLayer dLayer)
        {
            _dLayer = dLayer;
        }

        public void UpdatePrjLocationOfSymbol()
        {
            if (_symbol != null && _symbol is MarkerSymbol && _geometry != null && _geometry is GeoPoint && _geometry.CoordType == enumCoordTypes.Projection)
            {
                GeoPoint pt = _geometry as GeoPoint ;
                (_symbol as MarkerSymbol).PrjLocation = new Point(pt.X, pt.Y);
            }
        }

        public void ApplySymbol()
        {
            if (_symbol == null || _dLayer == null)
                return;
            if (_symbol is MarkerSymbol && _geometry is GeoShape)
            {
                Canvas canvas = _dLayer.Canvas;
                UIElement ele = null;
                if (_xamlObj != null)
                {
                    ele = _xamlObj as UIElement;
                    canvas.Children.Remove(ele);
                    if (_dLayer.UIElements.Contains(ele))
                        _dLayer.UIElements.Remove(ele);
                }
                GeoPoint pt = _geometry as GeoPoint;
                MarkerSymbol mark = _symbol as MarkerSymbol;
                mark.PrjLocation = new Point(pt.X, pt.Y);
                mark.UpatePixelLocation(-1);
                ele = mark.UIElement;
                canvas.Children.Add(ele);
                _xamlObj = ele;
                _dLayer.UIElements.Add(ele);
                _dLayer.AttachEventToFeature(this);
            }
            else if (_xamlObj is Path)
            {
                Path pth = _xamlObj as Path;
                if (_symbol is LineSymbol)
                {
                    pth.Stroke = (_symbol as LineSymbol).LineBrush;
                    pth.StrokeThickness = (_symbol as LineSymbol).LineWidth;
                }
                else if (_symbol is FillSymbol)
                {
                    pth.Stroke = (_symbol as FillSymbol).OutlineSymbol.LineBrush;
                    pth.StrokeThickness = (_symbol as FillSymbol).OutlineSymbol.LineWidth;
                    pth.Fill = (_symbol as FillSymbol).FillBrush;
                }
            }
        }

        public object XamlObject
        {
            get 
            {
                if (_xamlObj == null)
                {
                    _xamlObj = ToShape();
                }
                return _xamlObj; 
            }
            set { _xamlObj = value; }
        }

        public int OID
        {
            get { return _oid; }
        }

        public Dictionary<string, string> Properties
        {
            get { return _properties; }
        }

        public string PrimaryPropertyName
        {
            get 
            {
                if (_properties == null || _properties.Count == 0)
                    return _oid.ToString();
                foreach (string key in _properties.Keys)
                {
                    string v = _properties[key].ToUpper();
                    string k = key.ToUpper();
                    if (k == "名称" || k == "编号" || k == "NAME" || k == "CODE"
                        || k == "CONTINENT" || k == "CHINESE_CH")
                        return v;
                }
                return _oid.ToString();
            }
        }

        public string GetPropertyValue(string propertyName)
        {
            if (propertyName != null)
            {
                if (_properties.ContainsKey(propertyName))
                    return _properties[propertyName];
            }
            return null;
        }

        public string PrimaryDisplayValue
        {
            get 
            {
                return GetPrimaryPropertyValue();
            }
        }

        private string GetPrimaryPropertyValue()
        {
            if (_properties == null || _properties.Count == 0)
                return _oid.ToString();
            if (PrimaryPropertyName != null)
            {
                if (_properties.ContainsKey(PrimaryPropertyName))
                    return _properties[PrimaryPropertyName];
            }
            foreach (string key in _properties.Keys)
            {
                string v = _properties[key].ToUpper();
                string k = key.ToUpper();
                if (k == "名称" || k == "编号" || k == "NAME" || k == "CODE"
                    || k == "CONTINENT" || k == "CHINESE_CH")
                    return v;
            }
            return _oid.ToString();
        }

        public GeoShape Geometry
        {
            get { return _geometry; }
            set { _geometry = value; }
        }

        public Shape ToShape()
        {
            if (_geometry == null)
                return null;
            if (_geometry is GeoPoint)
            {
                Path pthPoint = new Path();
                pthPoint.Data = _geometry.ToGeometry();
                return pthPoint;
            }
            else if (_geometry is GeoPolyline)
            {
                Path pthPolyline = new Path();
                pthPolyline.Data = _geometry.ToGeometry();
                return pthPolyline;
            }
            else if (_geometry is GeoPolygon)
            {
                Path pthPolygon = new Path();
                pthPolygon.Data = _geometry.ToGeometry();
                return pthPolygon;
            }
            else if (_geometry is GeoPieArea)
            {
                Path pthPolygon = new Path();
                pthPolygon.Data = _geometry.ToGeometry();
                return pthPolygon;
            }
            else if (_geometry is GeoRectangle)
            {
                Path pthPolygon = new Path();
                pthPolygon.Data = _geometry.ToGeometry();
                return pthPolygon;
            }
            else if (_geometry is GeoEllipse)
            {
                Path pthPolygon = new Path();
                pthPolygon.Data = _geometry.ToGeometry();
                return pthPolygon;
            }
            throw new Exception("不支持直接渲染几何类型\"" + _geometry.GetType().ToString()+"\"");
        }

        public static Feature FromFeatureInfo(CodeCell.AgileMap.WebComponent.AgileMapServiceProxy.FeatureInfo fetInfo)
        {
            Feature fet = new Feature(fetInfo.OID);
            //fet.Geometry = GetGeometryByFeatureInfo(fetInfo.Shape);
            if (fetInfo.Properties != null)
                foreach (string key in fetInfo.Properties.Keys)
                    fet.Properties.Add(key, fetInfo.Properties[key]);
            fet.Envelope = fetInfo.Envelope;
            return fet;
        }
    }
}
