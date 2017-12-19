using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CodeCell.Bricks.AppFramework
{
    public interface ITool:ICommand
    {
        bool MouseDown(object sender, MouseEventArgs e);
        bool MouseMove(object sender, MouseEventArgs e);
        bool MouseUp(object sender, MouseEventArgs e);
        bool MouseWheel(object sender, MouseEventArgs e);
        bool MouseDoubleClick(object sender, MouseEventArgs e);
        void Render(object drawArg);
        Cursor Cursor { get;}
    }
}
