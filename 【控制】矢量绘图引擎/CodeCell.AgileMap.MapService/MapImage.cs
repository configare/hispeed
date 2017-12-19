using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Runtime.Serialization;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace CodeCell.AgileMap.MapService
{
    [DataContract]
    public class MapImage
    {
        private string _imgurl = null;
        private double _left = 0;
        private double _bottom = 0;
        private double _width = 0;
        private double _height = 0;

        public MapImage(RectangleF bounds, string imageurl)
        {
            _left = bounds.Left;
            _bottom = bounds.Top;
            _width = bounds.Width;
            _height = bounds.Height;
            _imgurl = imageurl;
          }

        [DataMember]
        public string ImageUrl
        {
            get { return _imgurl; }
            set { _imgurl = value; }
        }

        [DataMember]
        public double Left
        {
            get { return _left; }
            set { _left = value; }
        }

        [DataMember]
        public double Bottom
        {
            get { return _bottom; }
            set { _bottom = value; }
        }

        [DataMember]
        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }

        [DataMember]
        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }
    }
}