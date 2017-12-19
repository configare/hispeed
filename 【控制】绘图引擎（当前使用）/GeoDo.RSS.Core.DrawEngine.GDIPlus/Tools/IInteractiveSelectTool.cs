using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    public delegate void InteractivePointHandlerScreen(object sender,PointF screenPoint);
    public delegate void InteractivePointHandlerPrj(object sender, CoordPoint prjPoint);
    public delegate void InteractiveRectHandlerScreen(object sender, RectangleF screenPoint);
    public delegate void InteractiveRectHandlerPrj(object sender, CoordEnvelope prjPoint);

    public interface IInteractiveSelectTool
    {
        InteractivePointHandlerScreen InteractivePoinScreen { get; set; }
        InteractivePointHandlerPrj InteractivePoinPrj { get; set; }
        InteractiveRectHandlerScreen InteractiveRectScreen { get; set; }
        InteractiveRectHandlerPrj InteractiveRectPrj { get; set; }
    }
}
