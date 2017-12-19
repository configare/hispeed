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
    public class AdjustSizeArgument
    {
        private double _minResolution = 3000;
        private double _maxResolution = 50;
        private double _minSize = 5;
        private double _maxSize = 100;
        private bool _enabled = false;
        private double _factor = 1;

        public AdjustSizeArgument()
        {
            ComputeFactor();
        }

        public AdjustSizeArgument(double minResolution,double maxResolution,double minSize,double maxSize)
        {
            _minSize = minSize;
            _maxSize = maxSize;
            _minResolution = minResolution;
            _maxResolution = maxResolution;
            ComputeFactor();
        }

        private void ComputeFactor()
        {
            if (_minResolution > 500000)
                throw new Exception("支持的最小分辨率最大值为33000!");
            if (_maxResolution < 0.001)
                throw new Exception("支持的最大分辨率的最小值为0.001！");
            if (_minResolution <= _maxResolution)
                throw new Exception("最小分辨率的值必须大于最大分辨率的值！");
            if (_minSize < 0)
                throw new Exception("符号最小尺寸必须大于等于零！");
            if (_maxSize <= _minSize)
                throw new Exception("符号最大尺寸必须大于最小尺寸！");
            _factor = (_maxSize - _minSize) / (_minResolution - _maxResolution);
        }

        public bool IsValidRange(double resolution,out double retResolution)
        {
            if (resolution > _minResolution)
                retResolution = _minResolution;
            else if (resolution < _maxResolution)
                retResolution = _maxResolution;
            else
                retResolution = resolution;
            return resolution > _maxResolution && resolution < _minResolution;
        }

        public double Factor
        {
            get { return _factor; }
        }

        public bool IsEnabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public double MinResolution
        {
            get { return _minResolution; }
        }

        public double MaxResolution
        {
            get { return _maxResolution; }
        }

        public double MinSize
        {
            get { return _minSize; }
        }

        public double MaxSize
        {
            get { return _maxSize; }
        }
    }
}
