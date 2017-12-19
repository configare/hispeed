using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface ICanvasViewControl
    {
        //倒转(应对升轨与降轨)
        bool IsReverseDirection { get; set; }
        void ApplyOffset(float offsetScreenX, float offsetScreenY);
        void ZoomInByStep(int steps);
        void ZoomOutByStep(int steps);
        void ZoomInByStep(int hostScreenX, int hostScreenY,int steps);
        void ZoomOutByStep(int hostScreenX, int hostScreenY,int steps);
    }
}
