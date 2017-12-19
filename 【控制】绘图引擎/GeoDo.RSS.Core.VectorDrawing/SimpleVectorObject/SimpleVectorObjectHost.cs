using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;
using System.Drawing;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public class SimpleVectorObjectHost : ISimpleVectorObjectHost, IDisposable
    {
        private ICanvas _canvas;
        private IMapRuntime _mapRuntime;
        private MemoryDataSource _datasource;
        private CodeCell.AgileMap.Core.IFeatureLayer _layer;
        private Dictionary<ISimpleVectorObject, Feature> _features = new Dictionary<ISimpleVectorObject, Feature>();
        private string[] _fldNames = new string[] { "name" };
        private static int OID = 0;
        private LabelDef _labeldef;

        public SimpleVectorObjectHost(string name, ICanvas canvas)
            : this(name, canvas, null)
        {
        }

        public SimpleVectorObjectHost(string name, ICanvas canvas, string[] fieldNames)
        {
            if (fieldNames != null)
                _fldNames = fieldNames;
            _canvas = canvas;
            _mapRuntime = (_canvas.LayerContainer.VectorHost as IVectorHostLayer).MapRuntime as IMapRuntime;
            _datasource = new CodeCell.AgileMap.Core.MemoryDataSource(name, enumShapeType.Polygon, _fldNames);
            CodeCell.AgileMap.Core.IFeatureClass fetClass = new CodeCell.AgileMap.Core.FeatureClass(_datasource);
            CodeCell.AgileMap.Core.IFeatureLayer fetLayer = new CodeCell.AgileMap.Core.FeatureLayer(name, fetClass);
            _layer = fetLayer;
            IMap map = (_canvas.LayerContainer.VectorHost as IVectorHostLayer).Map as IMap;
            if (map != null)
            {
                map.LayerContainer.Append(fetLayer);
            }
            SetLabel(fetLayer);
            SetRenderer(fetLayer);
        }

        private void SetRenderer(CodeCell.AgileMap.Core.IFeatureLayer fetLayer)
        {
            IFeatureRenderer render = fetLayer.Renderer;
            SimpleFeatureRenderer sr = render as SimpleFeatureRenderer;
            IFillSymbol sym = sr.Symbol as IFillSymbol;
            sym.OutlineSymbol.Color = Color.Blue;
        }

        private void SetLabel(CodeCell.AgileMap.Core.IFeatureLayer fetLayer)
        {
            LabelDef df = fetLayer.LabelDef as LabelDef;
            if (df == null)
                return;
            df.EnableLabeling = true;
            df.ForeColor = Color.Red;
            df.Fieldname = "name";
            _labeldef = df;
        }

        public LabelDef LabelSetting
        {
            get { return _labeldef; }
        }

        public ISymbol Symbol
        {
            get { return _layer.Renderer.CurrentSymbol; }
            set { _layer.Renderer = new SimpleFeatureRenderer(value); }
        }

        public ISimpleVectorObject[] GetAllVectorObjects()
        {
            return _features.Count > 0 ? _features.Keys.ToArray() : null;
        }

        public Feature[] GetAllFeatures()
        {
            return _features.Count > 0 ? _features.Values.ToArray() : null;
        }

        public void RemoveFeature(int OID)
        {
            foreach (ISimpleVectorObject obj in _features.Keys)
            {
                if (obj.OID == OID)
                {
                    _datasource.Remove(_features[obj]);
                    _features.Remove(obj);
                    return;
                }
            }
        }

        public void Add(ISimpleVectorObject obj)
        {
            if (obj == null)
                return;
            if (_features.ContainsKey(obj))
            {
                _datasource.Remove(_features[obj]);
                _features.Remove(obj);
                Add(obj);
            }
            else
            {
                Feature fet = GetFeature(obj);
                if (fet == null)
                    return;
                _datasource.AddFeatures(new Feature[] { fet });
                _features.Add(obj, fet);
            }
            if (obj is SimpleVectorObject)
                (obj as SimpleVectorObject).SetHost(this);
            _canvas.Refresh(enumRefreshType.All);
        }

        //public void AddFeature(Feature feature)
        //{
        //    if (feature == null)
        //        return;
        //}

        private Feature GetFeature(ISimpleVectorObject obj)
        {
            Shape geometry = obj.Geometry;
            if (geometry == null)
                return null;
            Feature fet = new Feature(OID++, geometry, _fldNames, obj.AttValues, null);
            obj.OID = fet.OID;
            return fet;
        }

        public void Remove(Func<ISimpleVectorObject, bool> where)
        {
            List<Feature> fets = new List<Feature>();
            List<ISimpleVectorObject> objs = new List<ISimpleVectorObject>();
            foreach (ISimpleVectorObject obj in _features.Keys)
            {
                if (where(obj))
                {
                    fets.Add(_features[obj]);
                    objs.Add(obj);
                }
            }
            if (fets.Count > 0)
            {
                for (int i = 0; i < fets.Count; i++)
                {
                    _features.Remove(objs[i]);
                    _datasource.Remove(fets[i]);
                }
            }
        }

        public void Dispose()
        {
            if (_canvas != null)
            {
                if (_canvas.LayerContainer.VectorHost != null)
                {
                    IMap map = (_canvas.LayerContainer.VectorHost as IVectorHostLayer).Map as IMap;
                    if (map != null && map.LayerContainer != null)
                    {
                        map.LayerContainer.Remove(_layer);
                    }
                }
                _canvas = null;
            }
            if (_mapRuntime != null)
            {
                _mapRuntime = null;
            }
            if (_datasource != null)
            {
                _datasource.Dispose();
                _datasource = null;
            }
            if (_labeldef != null)
            {
                _labeldef.Dispose();
                _labeldef = null;
            }
            if (_layer != null)
            {
                _layer.Dispose();
                _layer = null;
            }
            OID = 0;
        }
    }
}
