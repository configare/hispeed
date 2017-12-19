using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    internal class LightLayerContainer:ILightLayerContainer
    {
        private List<ILightLayer> _layers = null;
        private IMapRuntime _runtime = null;
        
        public LightLayerContainer(IMapRuntime runtime)
        {
            _runtime = runtime;
        }

        #region IHandinessLayerContainer 成员

        public ILightLayer[] Layers
        {
            get { return _layers != null && _layers.Count > 0 ? _layers.ToArray() : null; }
        }

        public void Add(ILightLayer layer)
        {
            if (_layers == null)
                _layers = new List<ILightLayer>();
            _layers.Add(layer);
            layer.Init(_runtime);
        }

        public void Remove(ILightLayer layer)
        {
            if (layer == null || _layers == null || !_layers.Contains(layer))
                return;
        }

        public ILightLayer GetLayerByName(string name)
        {
            if (name == null || _layers == null || _layers.Count == 0)
                return null;
            foreach (ILightLayer hand in _layers)
            {
                if (hand.Name == null)
                    continue;
                if (hand.Name.ToUpper() == name.ToUpper())
                    return hand;
            }
            return null;
        }

        public void Clear()
        {
            if (_layers != null)
                _layers.Clear();
        }

        #endregion
    }
}
