using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public class ControlTool:IControlTool
    {
        public ControlTool()
        { 
        }

        public virtual void Event(object sender, enumCanvasEventType eventType, CanvasEventArgs e)
        {
        }

        public virtual void Render(object sender, IDrawArgs drawArgs)
        {
        }

        public virtual Cursor Cursor
        {
            get { return Cursors.Default; }
        }
    }
}
