using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Project
{
    /// <summary>
    /// 有效的坐标范围
    /// 例如：Mecator   -180 ~ 180,-85 ~ 85
    /// </summary>
    [Serializable]
    public class CoordinateDomain:ICloneable
    {
        private double _minX = -179.999999d;
        private double _maxX = 179.999999d;
        private double _minY = -89.999999d;
        private double _maxY = 89.999999d;

        public CoordinateDomain()
        { 
        }

        public CoordinateDomain(double minX, double maxX, double minY, double maxY)
        {
            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
        }

        public CoordinateDomain(double minX, double maxX)
        {
            _minX = minX;
            _maxX = maxX;
        }

        public CoordinateDomain(double minY, double maxY,object nullarg)
        {
            _minY = minY;
            _maxY = maxY;
        }

        public double MinX 
        {
            get { return _minX; }
            set { _minY = value; }
        }

        public double MaxX
        {
            get { return _maxX; }
            set { _maxX = value; }
        }

        public double MinY
        {
            get { return _minY; }
            set { _minY = value; }
        }

        public double MaxY
        {
            get { return _maxY; }
            set { _maxY = value; }
        }

        public void CorrectX(ref double x)
        {
            if (x < _minX)
                x = _minX;
            else if (x > _maxX)
                x = _maxX;
        }

        public void CorrectY(ref double y)
        {
            if (y < _minY)
                y = _minY;
            else if (y > _maxY)
                y = _maxY;
        }

        public object Clone()
        {
            return new CoordinateDomain(_minX, _maxX, _minX, _maxY);
        }
    }
}
