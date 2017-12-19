using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    public class RasterProfileData
    {
        private Point[] _locations;
        private double[] _dataValues;
        private double _maxValue;
        private double _minValue;

        public RasterProfileData(Point[] locations, double[] dataValues)
        {
            _locations = locations;
            _dataValues = dataValues;
            if (dataValues.Length > 0)
            {
                _maxValue = _dataValues.Max();
                _minValue = _dataValues.Min();
            }
        }

        public Point[] Locations
        {
            get { return _locations; }
        }

        public double[] DataValues
        {
            get { return _dataValues; }
        }

        public double MaxValue
        {
            get { return _maxValue; }
        }

        public double MinValue
        {
            get { return _minValue; }
        }
    }
}
