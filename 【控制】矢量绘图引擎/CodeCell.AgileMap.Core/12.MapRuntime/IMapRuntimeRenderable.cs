using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

namespace CodeCell.AgileMap.Core
{
    public interface IMapRuntimeRenderable
    {
        SmoothingMode SmoothingMode { get; set; }
        void Render(RenderArgs arg);
    }
}
