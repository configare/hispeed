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
    public class SelectToolEllipse : MapToolEllpise, ISelectTool
    {
        public event OnSelectToolFinishedHandler OnSelectToolFinished = null;

        public SelectToolEllipse()
            : base()
        {
            _name = "圆形";
            _image = "/CodeCell.AgileMap.WebComponent;component/Images/SelectByCirlce.png";
        }

        protected override void HandleRectBox(Point prjLeftUp, Point prjRightBottom)
        {
            if(OnSelectToolFinished != null)
            {
                double cx = (prjLeftUp.X + prjRightBottom.X)/2;
                double cy = (prjLeftUp.Y + prjRightBottom.Y)/2;
                double w = Math.Abs(prjRightBottom.X - prjLeftUp.X);
                double h = Math.Abs(prjRightBottom.Y - prjLeftUp.Y);
                OnSelectToolFinished(this,
                    new GeoEllipse(new GeoPoint(cx, cy), new Size(w / 2, h / 2)));
            }
        }
    }
}
