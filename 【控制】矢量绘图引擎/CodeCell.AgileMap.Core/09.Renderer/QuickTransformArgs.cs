using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public class QuickTransformArgs
    {
        private float _kLon = 0;
        private float _kLat = 0;
        private float _bLon = 0;
        private float _bLat = 0;
        //private float _preKLon = -1;
        //private float _preKLat = -1;
        //public bool IsOnlyTranslateLon = false;
        //public bool IsOnlyTranslateLat = false;
        //private bool _isFirst = true;

        public QuickTransformArgs()
        {
        }

        public float kLon
        {
            get { return _kLon; }
            set
            {
                //if(!_isFirst)
                //    _preKLon = _kLon;
                _kLon = value;
            }
        }

        public float bLon
        {
            get { return _bLon; }
            set { _bLon = value; }
        }

        public float kLat
        {
            get { return _kLat; }
            set
            {
                //if (!_isFirst)
                //    _preKLat = _kLat;
                _kLat = value;
            }
        }

        public float bLat
        {
            get { return _bLat; }
            set { _bLat = value; }
        }

        //public void Update()
        //{
        //    if (_isFirst)
        //    {
        //        _isFirst = false;
        //        return;
        //    }
        //    IsOnlyTranslateLon = Math.Abs(_preKLon - _kLon) < float.Epsilon;
        //    IsOnlyTranslateLat = Math.Abs(_preKLat - _kLat) < float.Epsilon;
        //}
    }
}
