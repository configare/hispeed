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
    public class MapCommandRefresh:MapCommand
    {
        public MapCommandRefresh()
            : base()
        {
            _name = "刷新";
            _image = "/CodeCell.AgileMap.WebComponent;component/Images/cmdRefresh.png";
        }

        public override void Click()
        {
            _mapcontrol.RefreshMap();
        }
    }
}
