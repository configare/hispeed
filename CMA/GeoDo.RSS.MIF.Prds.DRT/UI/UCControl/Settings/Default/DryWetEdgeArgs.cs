using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public class DryWetEdgeArgs
    {
        private double _maxA;
        private double _maxB;
        private double _minA;
        private double _minB;

        public DryWetEdgeArgs(double minA, double maxA, double minB, double maxB)
        {
            _minA = minA;
            _maxA = maxA;
            _minB = minB;
            _maxB = maxB;
        }

        public double MaxA
        {
            get
            {
                return _maxA;
            }
            set
            {
                _maxA = value;
            }
        }

        public double MaxB
        {
            get
            {
                return _maxB;
            }
            set
            {
                _maxB = value;
            }
        }

        public double MinA
        {
            get
            {
                return _minA;
            }
            set
            {
                _minA = value;
            }
        }

        public double MinB
        {
            get
            {
                return _minB;
            }
            set
            {
                _minB = value;
            }
        }
    }
}
