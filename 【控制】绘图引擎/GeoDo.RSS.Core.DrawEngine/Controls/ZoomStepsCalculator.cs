using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class ZoomStepsCalculator
    {
        /*
         * 2012-4-5 
         *     随着分辨率的提高，以倍数方式加长步长，效果是理想的。
         */
        public int GetZoomSteps(ICanvas canvas, int wheelDelta, int flag)
        {
            int steps = 1;
            float scale = canvas.PrimaryObjectScale;
            if (scale < 0.1f)
                steps = 3;
            else if (scale >= 0.1 && scale < 0.2f)
                steps = 6;
            else if (scale >= 0.2 && scale < 0.6f)
                steps = 12;
            else if (scale >= 0.6f && scale < 2f)
                steps = 25;
            else if (scale >= 2f && scale < 4f)
                steps = 50;
            else if (scale >= 4f && scale < 6f)
                steps = 100;
            else if (scale >= 6f && scale < 10f)
                steps = 200;
            else if (scale >= 10f && scale < 20f)
                steps = 400;
            else if (scale >= 20f && scale < 50f)
                steps = 600;
            else if (scale >= 50f)
                steps = 1200;
            return steps;
        }
    }
}
