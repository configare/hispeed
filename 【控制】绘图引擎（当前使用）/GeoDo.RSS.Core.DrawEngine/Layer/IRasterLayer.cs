using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface IRasterLayer:IRenderLayer
    {
        bool IsNeedReplaceColor { get; set; }
        Color OldColor { get; set; }
        Color NewColor { get; set; }
        object Drawing { get; }
    }
}
