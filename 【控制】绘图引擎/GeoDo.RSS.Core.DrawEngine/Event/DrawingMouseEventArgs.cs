using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public class DrawingMouseEventArgs
    {
        private bool _isHandled = false;
        private int _screenX = 0;
        private int _screenY = 0;
        private int _rasterX = 0;
        private int _rasterY = 0;
        private double _prjX = 0;
        private double _prjY = 0;
        private double _geoX = 0;
        private double _geoY = 0;
        private int _wheelDelta = 0;

        public DrawingMouseEventArgs()
        { 
        }

        public DrawingMouseEventArgs(int wheelDelta)
        {
            _wheelDelta = wheelDelta;
          }

        public DrawingMouseEventArgs(int wheelDelta,int screenX,int screenY)
        {
            _wheelDelta = wheelDelta;
            _screenX = screenX;
            _screenY = screenY;
        }

        public DrawingMouseEventArgs(int screenX,int screenY)
        {
            _screenX = screenX;
            _screenY = screenY;
        }

        public DrawingMouseEventArgs(int rasterX, int rasterY, object hostblank)
        {
            _rasterX = rasterX;
            _rasterY = rasterY;
        }

        public DrawingMouseEventArgs(int screenX, int screenY, int rasterX, int rasterY)
            :this(screenX,screenY)
        {
            _rasterX = rasterX;
            _rasterY = rasterY;
        }

        public DrawingMouseEventArgs(int screenX, int screenY, int rasterX, int rasterY, double prjX, double prjY)
            : this(screenX, screenY, rasterX, rasterY)
        {
            _prjX = prjX;
            _prjY = prjY;
        }

        public DrawingMouseEventArgs(int screenX, int screenY, int rasterX, int rasterY, double prjX, double prjY, double geoX, double geoY)
            : this(screenX, screenY, rasterX, rasterY, prjX, prjY)
        {
            _geoX = geoX;
            _geoY = geoY;
        }

        public bool IsHandled
        {
            get { return _isHandled; }
            set { _isHandled = value; }
        }

        public int WheelDelta
        {
            get { return _wheelDelta; }
        }

        public int ScreenX
        {
            get { return _screenX; }
        }

        public int ScreenY
        {
            get { return _screenY; }
        }

        public int RasterX
        {
            get { return _rasterX; }
        }

        public int RasterY
        {
            get { return _rasterY; }
        }

        public double PrjX
        {
            get { return _prjX; }
        }

        public double PrjY
        {
            get { return _prjY; }
        }

        public double GeoX
        {
            get { return _geoX; }
        }

        public double GeoY
        {
            get { return _geoY; }
        }
    }
}
