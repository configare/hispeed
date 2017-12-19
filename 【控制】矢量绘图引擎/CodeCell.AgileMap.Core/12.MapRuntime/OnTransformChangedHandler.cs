using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Drawing2D;

namespace CodeCell.AgileMap.Core
{
    public delegate void OnTransformChangedHandler(object sender,Matrix newTransform,Matrix oldTransform);
}
