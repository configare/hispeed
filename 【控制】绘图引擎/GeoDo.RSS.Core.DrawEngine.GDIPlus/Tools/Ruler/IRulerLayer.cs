using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine.GDIPlus
{
    public interface IRulerLayer
    {
        bool IsShowCrossLines { get; set; }
        Color BackColor { get; set; }
        Color CrossLinesColor { get; set; }
    }
}
