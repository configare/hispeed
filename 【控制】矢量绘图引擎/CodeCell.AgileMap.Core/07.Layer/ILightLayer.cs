using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface ILightLayer
    {
        string Name { get; }
        bool Enabled { get; }
        void Init(IMapRuntime runtime);
        void Render(RenderArgs arg);
    }
}
