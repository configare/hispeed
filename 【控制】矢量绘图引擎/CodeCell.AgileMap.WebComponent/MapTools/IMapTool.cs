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

namespace CodeCell.AgileMap.WebComponent
{
    public interface IMapTool:IMapCommand
    {
        void MouseDown(object sender, MouseButtonEventArgs e);
        void MouseRightDown(object sender, MouseButtonEventArgs e);
        void MouseUp(object sender, MouseButtonEventArgs e);
        void MouseMove(object sender, MouseEventArgs e);
        void MouseWheel(object sender, MouseWheelEventArgs e);
    }
}
