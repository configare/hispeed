#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/25 8:49:27
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DF.GRIB
{
    /// <summary>
    /// 格点场定义
    /// </summary>
    public class GRIB_Definition
    {
        private float _firstLongitude;
        private float _firstLatitude;
        private float _endLongitude;
        private float _endLatitude;
        private float _minLongitude;
        private float _minLatitude;
        private float _maxLongitude;
        private float _maxLatitude;
        private float _lonResolution;
        private float _latResolution;
        private int _width;
        private int _height;
        private string _valueName;
        private float _invalidValue;

        public GRIB_Definition(float firstLon, float firstLat, float endLon, float endLat, float resolution, int width, int height, string valueNames, float invalidValue)
        {
            _firstLongitude = firstLon;
            _firstLatitude = firstLat;
            _endLatitude = endLat;
            _endLongitude = endLon;
            _latResolution = _lonResolution = resolution;
            _width = width;
            _height = height;
            _valueName = valueNames;
            _invalidValue = invalidValue;
            if (_firstLatitude < _endLatitude)
            {
                _minLatitude = _firstLatitude;
                _maxLatitude = _endLatitude + _latResolution;
            }
            else
            {
                _minLatitude = _endLatitude - _latResolution;
                _maxLatitude = _firstLatitude;
            }
            if (_firstLongitude == 180 && _endLongitude == -90)
            {
                _minLongitude = -180f;
                _maxLongitude = -92.5f;
            }
            else if (_firstLongitude == 0 && _endLongitude > 355)
            {
                _minLongitude = -180f;
                _maxLongitude = 180f;
            }
            else
            {
                _minLongitude = _firstLongitude;
                _maxLongitude = _endLongitude + _lonResolution;
            }
        }

        public GRIB_Definition(float firstLon, float firstLat, float endLon, float endLat, float lonResolution, float latResolution, int width, int height, string valueNames, float invalidValue)
            : this(firstLon, firstLat, endLon, endLat, lonResolution, width, height, valueNames, invalidValue)
        {
            _latResolution = latResolution;
            _lonResolution = lonResolution;
        }

        public float InvalidValue
        {
            get { return _invalidValue; }
        }

        public float FirstLongitude
        {
            get { return _firstLongitude; }
        }

        public float FirstLatitude
        {
            get { return _firstLatitude; }
        }

        public float EndLongitude
        {
            get { return _endLongitude; }
        }

        public float EndLatitude
        {
            get { return _endLatitude; }
        }

        public float LonResolution
        {
            get { return _lonResolution; }
        }

        public float LatResolution
        {
            get { return _latResolution; }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public string ValueName
        {
            get { return _valueName; }
            set { _valueName = value; }
        }

        public float MinLatitude
        {
            get { return _minLatitude; }
        }

        public float MinLongitude
        {
            get { return _minLongitude; }
        }

        public float MaxLatitude
        {
            get { return _maxLatitude; }
        }

        public float MaxLongitude
        {
            get { return _maxLongitude; }
        }

        public void ToGeoCoord(ref float longitude, ref float latitude, int index)
        {
            int col = index / _height;
            longitude = _minLongitude + _lonResolution * col;
            latitude = _minLatitude + _latResolution * (index - col * _height);
        }

        public CoordEnvelope GetCoordEnvelope()
        {
            return new CoordEnvelope(_minLongitude, _maxLongitude, _minLatitude, _maxLatitude);
        }
    }
}
