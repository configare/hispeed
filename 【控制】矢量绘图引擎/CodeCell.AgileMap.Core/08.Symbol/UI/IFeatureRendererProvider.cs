using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    internal interface IFeatureRendererProvider
    {
        IFeatureRenderer Renderer { get; }
    }
}
