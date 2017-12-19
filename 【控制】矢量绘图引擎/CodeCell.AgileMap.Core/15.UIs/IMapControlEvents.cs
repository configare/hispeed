using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IMapControlEvents
    {
        OnMapScaleChangedHandler OnMapScaleChanged { get; set; }
        OnViewExtentChangedHandler OnViewExtentChanged { get; set; }
    }
}
