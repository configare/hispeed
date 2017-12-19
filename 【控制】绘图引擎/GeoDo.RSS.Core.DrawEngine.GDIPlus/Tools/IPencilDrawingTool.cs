using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    public interface IPencilDrawingTool:IFlyLayer
    {
        AOI GetAOI();
    }
}
