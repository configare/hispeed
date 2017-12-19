using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public interface IHookOfAgileMap
    {
        IMapControl MapControl { get; }
    }
}
