using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class LayerContainer:LayerGroup,ILayerContainer
    {
        public LayerContainer()
        { 
        }

        public IVectorHostLayer VectorHost
        {
            get 
            {
                if (_layers == null || _layers.Count == 0)
                    return null;
                foreach (object lyr in _layers)
                    if (lyr != null && lyr is IVectorHostLayer)
                        return lyr as IVectorHostLayer;
                return null;
            }
        }

        public override void Dispose()
        {
            if(_layers == null || _layers.Count ==0)
                return ;
            foreach (ILayer lyr in _layers)
                lyr.Dispose();
            _layers = null;
            base.Dispose();
        }
    }
}
