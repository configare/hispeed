using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class LayerGroup : Layer, ILayerGroup
    {
        protected List<ILayer> _layers = new List<ILayer>();

        public LayerGroup()
            : base()
        {
        }

        public LayerGroup(string name, string alias)
        {
            _name = name;
            _alias = alias;
        }

        [Browsable(false)]
        public List<ILayer> Layers
        {
            get { return _layers; }
        }

        public ILayer GetByName(string name)
        {
            foreach (ILayer lyr in _layers)
                if (lyr.Name != null && lyr.Name == name)
                    return lyr;
            return null;
        }

        public ILayer Get(Func<ILayer, bool> where)
        {
            foreach (ILayer lyr in _layers)
                if (where(lyr))
                    return lyr;
            return null;
        }

        public void Adjust(ILayer layer, int index)
        {
            if (!_layers.Contains(layer))
                return;
            int idx = _layers.IndexOf(layer);
            _layers.Remove(layer);
            if (index < 1)
                _layers.Insert(0, layer);
            else if (index >= _layers.Count)
                _layers.Add(layer);
            else
                _layers.Insert(idx, layer);
        }

        public bool IsEmpty()
        {
            return _layers.Count == 0;
        }

        public override void Dispose()
        {
            if (_layers == null || _layers.Count == 0)
                return;
            foreach (ILayer layer in _layers)
                layer.Dispose();
            _layers = null;
            base.Dispose();
        }
    }
}
