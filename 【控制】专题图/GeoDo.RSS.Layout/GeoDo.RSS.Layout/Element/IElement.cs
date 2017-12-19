using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Layout
{
    public interface IElement:IRenderable,IDisposable,IPersitable
    {
        string Name { get; set; }
        bool Visible { get; set; }
        bool IsLocked { get; set; }
        bool IsSelected { get; set; }
        bool IsHited(float layoutX,float layoutY);
        bool IsHited(RectangleF layoutRect);
        Image Icon { get; }
    }
}
