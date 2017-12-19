using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    internal interface IInternalTransformControl
    {
        void Offset(int offetX, int offsetY);
        void Scale(float scale);
    }
}
