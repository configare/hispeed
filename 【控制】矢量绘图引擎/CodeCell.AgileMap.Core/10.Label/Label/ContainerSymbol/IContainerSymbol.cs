using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    public interface IContainerSymbol:IPersistable,IDisposable
    {
        SizeF Size { get; set; }
        bool IsFixedSize { get; set; }
        SizeF Draw(Graphics g, PointF pt, SizeF size);
    }
}
