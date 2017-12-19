using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CodeCell.AgileMap.Core
{
    public interface IConflictor:IDisposable
    {
        bool IsAllowChangePosition { get; set; }
        bool Enabled { get; set; }
        void Reset();
        bool IsConflicted(PointF point,SizeF size);
        bool IsConflicted(float x, float y, SizeF size);
        void HoldPosition(PointF point, SizeF size);
        void HoldPosition(float x, float y, SizeF size);
        void HoldPosition(float x, float y, SizeF size, Matrix transfrom);
    }
}
