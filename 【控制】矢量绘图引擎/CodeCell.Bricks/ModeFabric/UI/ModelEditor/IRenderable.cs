using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.Bricks.ModelFabric
{
    public interface IRenderable
    {
        void Render(RenderArg arg);
        bool IsHited(Point point);
    }
}
