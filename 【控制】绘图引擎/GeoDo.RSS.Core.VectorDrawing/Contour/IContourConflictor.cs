using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using GeoDo.RSS.RasterTools;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public interface IContourConflictor:IDisposable
    {
        void ChangeSize(Size size);
        void Reset();
        bool IsConflicted(ref PointF pt);
        void HoldPosition(ref PointF pt);
    }

    public class ContourConflictor : IContourConflictor
    {
        StatusRecorder _statusRecorder;
        int _width;
        int _count;

        public ContourConflictor(Size size)
        {
            ChangeSize(size);
        }

        public void ChangeSize(Size size)
        {
            _width = size.Width;
            _count = size.Width * size.Height;
            _statusRecorder = new StatusRecorder(size.Width * size.Height);
        }

        public unsafe void Reset()
        {
            _statusRecorder.Reset();
        }

        public bool IsConflicted(ref PointF pt)
        {
            int idx = (int)(pt.Y * _width + pt.X);
            if (idx >= _count || idx<0)
                return false;
            return _statusRecorder.IsTrue(idx);
        }

        public void HoldPosition(ref PointF pt)
        {
            int idx = (int)(pt.Y * _width + pt.X);
            if (idx >= _count || idx < 0)
                return;
            _statusRecorder.SetStatus(idx,true);
        }

        public void Dispose()
        {
            _statusRecorder.Dispose();
        }
    }

}
