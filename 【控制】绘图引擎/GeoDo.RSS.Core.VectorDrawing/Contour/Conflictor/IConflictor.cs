using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public interface IConflictor:IDisposable
    {
        void Reset();
        void Reset(Size canvasSize);
        bool IsConflicted(PointF point,SizeF size);
        bool IsConflicted(float x, float y, SizeF size);
        void HoldPosition(PointF point, SizeF size);
        void HoldPosition(float x, float y, SizeF size);
        void HoldPosition(float x, float y, SizeF size, Matrix transfrom);
    }
}
