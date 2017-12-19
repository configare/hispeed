using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace CodeCell.AgileMap.WebComponent
{
    public abstract class MapTool:MapCommand, IMapTool,IMapToolInternal
    {
        public MapTool()
        { 
        }

        public virtual void Active()
        {
        }

        public virtual void Deactive()
        { 
        }

        public override void Click()
        {
            _mapcontrol.CurrentMapTool = this;
        }

        public virtual void MouseDown(object sender, MouseButtonEventArgs e)
        { 
        }

        public virtual void MouseRightDown(object sender, MouseButtonEventArgs e)
        {
        }

        public virtual void MouseUp(object sender, MouseButtonEventArgs e)
        { 
        }

        public virtual void MouseMove(object sender, MouseEventArgs e)
        { 
        }

        public virtual void MouseWheel(object sender, MouseWheelEventArgs e)
        {
        }
    }
}
