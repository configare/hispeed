using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.GlobalMosaic
{
    class GeoHeader
    {
        private float _resX;
        private float _resY;
        private float _ltX;
        private float _ltY;
        private int _width;
        private int _height;

        public float ResX
        {
            get { return _resX; }
            set { _resX = value; }
        }

        public float ResY
        {
            get { return _resY; }
            set { _resY = value; }
        }

        public float LtX
        {
            get { return _ltX; }
            set { _ltX = value; }
        }

        public float LtY
        {
            get { return _ltY; }
            set { _ltY = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public GeoHeader(float resX, float resY, float ltX, float ltY, int width, int height)
        {
            // TODO: Complete member initialization
            this._resX = resX;
            this._resY = resY;
            this._ltX = ltX;
            this._ltY = ltY;
            this._width = width;
            this._height = height;
        }

        internal static GeoHeader Parse(string geoString)
        {
            if (string.IsNullOrWhiteSpace(geoString))
                return null;
            string[] geos = geoString.Split(',');
            if (geos.Length != 6)
            {
                return null;
            }
            float resX = 0.01f;
            float resY = 0.01f;
            //下面是全球区域
            //int width = 36000;
            //int height = 18000;
            //float ltX = -180;
            //float ltY = 90;
            //下面是中国区域
            float ltX = 65;
            float ltY = 60;
            int width = 8000;
            int height = 7000;
            float.TryParse(geos[0], out resX);
            float.TryParse(geos[1], out resY);
            float.TryParse(geos[2], out ltX);
            float.TryParse(geos[3], out ltY);
            int.TryParse(geos[4], out width);
            int.TryParse(geos[5], out height);
            return new GeoHeader(resX, resY, ltX, ltY, width, height);
        }
    }
}
