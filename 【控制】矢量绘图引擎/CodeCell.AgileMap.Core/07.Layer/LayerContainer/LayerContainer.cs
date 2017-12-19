using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    internal class LayerContainer : ILayerContainer, IDisposable, IRuntimeProjecter
    {
        private List<ILayer> _layers = new List<ILayer>();
        private OnAddLayerHandler _onAddLayer = null;
        private OnRemoveLayerHandler _onRemoveLayer = null;
        private bool _layerIsChanged = false;
        private IProjectionTransform _projectionTransform = null;
        private IFeatureRenderEnvironment _environment = null;
        private List<string> _selectableLayers = new List<string>();

        public LayerContainer(IProjectionTransform projectionTransform, IFeatureRenderEnvironment environment)
        {
            _projectionTransform = projectionTransform;
            _environment = environment;
        }

        internal void ResetLayerIsChanged()
        {
            _layerIsChanged = false;
        }

        #region IFeatureLayerContainer Members

        public string CanvasSpatialRef
        {
            get { return (_environment as IMapRuntime).CanvasSpatialRef; }
        }

        public bool LayerIsChanged
        {
            get { return _layerIsChanged; }
        }

        public OnAddLayerHandler OnAddFeatureLayer
        {
            get { return _onAddLayer; }
            set { _onAddLayer = value; }
        }

        public OnRemoveLayerHandler OnRemoveFeatureLayer
        {
            get { return _onRemoveLayer; }
            set { _onRemoveLayer = value; }
        }

        public void Insert(int index, ILayer featureLayer)
        {
            if (featureLayer == null || index < 0 || _layers.Contains(featureLayer))
                return;
            try
            {
                if (index > _layers.Count - 1)
                    Append(featureLayer);
                else
                {
                    _layers.Insert(index, featureLayer);
                    TryConvertToProject(featureLayer);
                    if (_onAddLayer != null)
                        _onAddLayer(this, featureLayer);
                }
            }
            finally
            {
                _layerIsChanged = true;
            }
        }

        public void Append(ILayer featureLayer)
        {
            if (_layers == null)
                return;
            if (!_layers.Contains(featureLayer))
            {
                _layers.Add(featureLayer);
                TryConvertToProject(featureLayer);
                if (_onAddLayer != null)
                    _onAddLayer(this, featureLayer);
                _layerIsChanged = true;
            }
        }

        public void Remove(ILayer featureLayer)
        {
            if (_layers == null)
                return;
            if (_layers.Contains(featureLayer))
            {
                _layers.Remove(featureLayer);
                if (_onRemoveLayer != null)
                    _onRemoveLayer(this, featureLayer);
                featureLayer.Dispose();
                _layerIsChanged = true;
            }
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index > _layers.Count - 1 || _layers == null)
                return;
            ILayer layer = _layers[index];
            Remove(layer);
        }

        public void AdjustOrder(ILayer mouseStartLayer, ILayer mouseStopLayer)
        {
            if (mouseStartLayer == null || mouseStopLayer == null || _layers == null)
                return;
            int sIdx = _layers.IndexOf(mouseStartLayer);
            int tIdx = _layers.IndexOf(mouseStopLayer);
            if (sIdx < 0 || tIdx < 0)
                return;
            if (sIdx == tIdx)
                return;
            _layers.Remove(mouseStartLayer);
            Insert(tIdx, mouseStartLayer);
        }

        public void Clear(bool needDisposeLayers)
        {
            if (_layers == null || _layers.Count == 0)
                return;
            if (needDisposeLayers)
            {
                Dispose();
            }
            else
                _layers.Clear();
        }

        public ILayer GetLayerByName(string name)
        {
            if (_layers == null || _layers.Count == 0 || string.IsNullOrEmpty(name))
                return null;
            foreach (ILayer lyr in _layers)
            {
                if (string.IsNullOrEmpty(lyr.Name))
                    continue;
                if (lyr.Name.ToUpper() == name.ToUpper())
                    return lyr;
            }
            return null;
        }

        public ILayer GetLayerById(string id)
        {
            if (_layers == null || _layers.Count == 0 || string.IsNullOrEmpty(id))
                return null;
            foreach (ILayer lyr in _layers)
            {
                if (lyr.Id.ToUpper() == id)
                    return lyr;
            }
            return null;
        }

        #endregion

        #region IFeatureLayerProvider Members

        public bool IsEmpty()
        {
            return _layers == null || _layers.Count == 0;
        }

        public ILayer[] Layers
        {
            get { return _layers != null && _layers.Count > 0 ? _layers.ToArray() : null; }
        }

        public IFeatureLayer[] FeatureLayers
        {
            get
            {
                List<IFeatureLayer> lyrs = new List<IFeatureLayer>();
                foreach (ILayer lyr in _layers)
                    if (lyr is IFeatureLayer)
                        lyrs.Add(lyr as IFeatureLayer);
                return lyrs.ToArray();
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_layers != null && _layers.Count > 0)
            {
                foreach (ILayer layer in _layers)
                {
                    if (layer != null)
                        layer.Dispose();
                }
                _layers.Clear();
            }
            if (_projectionTransform != null)
            {
                _projectionTransform = null;
            }
        }

        #endregion

        #region IRuntimeProjecter Members

        private void TryConvertToProject(ILayer featureLayer)
        {
            if (featureLayer == null)
                return;
            if (featureLayer is FeatureLayer)
            {
                (featureLayer as FeatureLayer).InternalInit(_environment);
                IFeatureClass fetclass = featureLayer.Class as IFeatureClass;
                if (fetclass == null || fetclass.CoordinateType == enumCoordinateType.Projection)
                    return;
                fetclass.Project(this as IRuntimeProjecter, enumCoordinateType.Projection);

            }
            //else if (featureLayer is RasterLayer)
            //{
            //    (featureLayer as RasterLayer).InternalInit(_environment,this as IRuntimeProjecter);
            //    return;
            //}
        }

        public void Project(ShapePoint point)
        {
            _projectionTransform.Transform(point);
        }

        public void Project(ShapePoint[] points)
        {
            _projectionTransform.Transform(points);
        }

        public void Project(Envelope envelope)
        {
            if (envelope == null)
                return;
            ShapePoint[] pts = envelope.Points;
            Project(pts);
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
            envelope.MinX = minX;
            envelope.MinY = minY;
            envelope.MaxX = maxX;
            envelope.MaxY = maxY;
        }

        #endregion


        public void SetSelectableLayers(string[] layerNames)
        {
            _selectableLayers.Clear();
            if (layerNames != null || layerNames.Length > 0)
                _selectableLayers.AddRange(layerNames);
        }


        public bool IsSelectable(string layerName)
        {
            if (_selectableLayers == null || _selectableLayers.Count == 0 || string.IsNullOrEmpty(layerName))
                return true;
            return _selectableLayers.Contains(layerName);
        }
    }
}
