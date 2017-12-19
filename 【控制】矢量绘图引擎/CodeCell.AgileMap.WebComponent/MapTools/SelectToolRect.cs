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
    public class SelectToolRect : MapToolRect, ISelectTool
    {
        public event OnSelectToolFinishedHandler OnSelectToolFinished = null;

        public SelectToolRect()
            :base()
        {
            _name = "矩形";
            _image = "/CodeCell.AgileMap.WebComponent;component/Images/SelectByRectangle.png";
        }

        protected override void HandleRectBox(Point prjLeftUp, Point prjRightBottom)
        {
            if (OnSelectToolFinished != null)
            {
                GeoRectangle rect = new GeoRectangle(new GeoPoint(prjLeftUp.X, prjRightBottom.Y),
                    new Size(Math.Abs(prjRightBottom.X - prjLeftUp.X),
                        Math.Abs(prjLeftUp.Y - prjRightBottom.Y)));
                OnSelectToolFinished(this, rect);
            }
        }
    }
}
