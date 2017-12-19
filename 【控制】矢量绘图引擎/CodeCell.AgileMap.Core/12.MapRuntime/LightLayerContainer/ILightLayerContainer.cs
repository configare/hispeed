using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface ILightLayerContainer
    {
        ILightLayer[] Layers { get; }
        void Add(ILightLayer layer);
        void Remove(ILightLayer layer);
        ILightLayer GetLayerByName(string name);
        void Clear();
    }
}
