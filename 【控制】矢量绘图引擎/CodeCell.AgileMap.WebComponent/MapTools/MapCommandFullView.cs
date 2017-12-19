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
    public class MapCommandFullView:MapCommand
    {
        public MapCommandFullView()
            : base()
        {
            _name = "全图";
            _image = "/CodeCell.AgileMap.WebComponent;component/Images/cmdFullView.png";
        }

        public override void  Click()
        {
            _mapcontrol.SetViewportByGeo(-180, 180, -90, 90);
        }
    }
}
