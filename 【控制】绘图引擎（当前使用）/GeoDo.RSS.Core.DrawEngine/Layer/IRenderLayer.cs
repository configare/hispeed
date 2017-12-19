using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface IRenderLayer:ILayer
    {
        bool Visible { get; set; }
        void Render(object sender,IDrawArgs drawArgs);
    }
}
