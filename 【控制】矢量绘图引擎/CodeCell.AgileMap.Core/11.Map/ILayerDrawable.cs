using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    public interface ILayerDrawable
    {
        bool Visible { get; set; }
        void Render(Graphics g,QuickTransformArgs quickTransform);
    }
}
