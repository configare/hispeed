using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface ILayerFlashControler:IDisposable
    {
        bool IsFlashing { get; }
        void AutoFlash(int interleave);
        void Flash();
        void Stop();
        void Reset(ICanvas canvas, ILayer layer);
    }
}
