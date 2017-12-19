using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Layout
{
    public interface IInteractiveEditable
    {
        enumEditType SupportedEditType { get; }
        void ApplyRotate(float angleDegree);
        void ApplyLocation(float layoutOffsetX, float layoutOffsetY);
        void ApplySize(float layoutXSize, float layoutYSize);
    }
}
