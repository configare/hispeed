using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
    public interface IRenderable
    {
        void Render(object sender,IDrawArgs drawArgs);
    }
}
