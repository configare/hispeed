using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.Bricks.ModelFabric
{
    internal interface IInternalSelection
    {
        void ClearSelection();
        ActionElement[] Selection { get; }
        ActionElement Select(Point point);
        ActionElement[] Select(RectangleF rectangle);
        ActionElement GetActionElementAt(Point point,out int anchorIndex);
    }
}
