using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
    public class CanvasEventArgs
    {
        public int ScreenX;
        public int ScreenY;
        public float LayoutX;
        public float LayoutY;
        public int WheelDelta;
        public bool IsHandled;
        public object E;
    }
}
