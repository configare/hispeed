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
    public class SelectToolPolygon : MapToolPolygon, ISelectTool
    {
        public event OnSelectToolFinishedHandler OnSelectToolFinished = null;

        public SelectToolPolygon()
            : base()
        {
            _name = "多边形";
            _image = "/CodeCell.AgileMap.WebComponent;component/Images/SelectByPolygon.png";
        }

        protected override void OnPolygonIsFinished(GeoPolygon ply)
        {
            if (OnSelectToolFinished != null)
                OnSelectToolFinished(this, ply);
        }
    }
}
