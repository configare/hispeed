using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IRasterLayer:ILayer,ILayerDrawable
    {
        bool IsTooSmall { get; }
    }
}
