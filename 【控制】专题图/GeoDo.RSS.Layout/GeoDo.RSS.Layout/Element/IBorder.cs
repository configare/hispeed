
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Layout
{
    public interface IBorder : ISizableElement
    {
        bool IsShowBorderLine { get; set; }
        Color BackColor { set; }
    }
}
