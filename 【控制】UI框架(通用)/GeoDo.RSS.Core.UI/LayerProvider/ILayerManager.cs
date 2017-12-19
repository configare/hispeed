using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.Core.UI
{
    public interface ILayerManager
    {
        void Apply(ILayersProvider provider);
        void Update();
    }
}
