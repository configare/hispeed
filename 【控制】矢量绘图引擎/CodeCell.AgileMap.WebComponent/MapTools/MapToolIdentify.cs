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
using System.Collections.ObjectModel;

namespace CodeCell.AgileMap.WebComponent
{
    public class MapToolIdentify:MapToolWheel
    {
        private double _tolerance = 6;

         public MapToolIdentify()
            : base()
        {
            _name = "拾取";
            _image = "/CodeCell.AgileMap.WebComponent;component/Images/ToolIdentify.png";
        }

        public override void Active()
        {
        }

        /// <summary>
        /// 拾取容差，像素单位
        /// </summary>
        public double Tolerance
        {
            get { return _tolerance; }
            set 
            {
                if (_tolerance < 1)
                    _tolerance = 1;
                _tolerance = value; 
            }
        }

        public override void MouseDown(object sender, MouseButtonEventArgs e)
        {
            _mapcontrol.QueryResultContainer.Clear();
            Point pt = e.GetPosition(_canvas);
            pt = _mapcontrol.Pixel2Prj(pt);
            ObservableCollection<Feature> fets = _mapcontrol.QueryResultContainer.QueryResult;
            _mapcontrol.Identify(fets, pt, _tolerance * _mapcontrol.Resolution);
        }
    }
}
