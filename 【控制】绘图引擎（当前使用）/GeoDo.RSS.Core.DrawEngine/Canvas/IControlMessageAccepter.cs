using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface IControlMessageAccepter
    {
        void AcceptMouseDown(object sender, MouseEventArgs e);
        void AcceptMouseMove(object sender, MouseEventArgs e);
        void AcceptMouseUp(object sender, MouseEventArgs e);
        void AcceptMouseWheel(object sender, MouseEventArgs e);
        void AcceptKeyDown(object sender, KeyEventArgs e);
    }
}
