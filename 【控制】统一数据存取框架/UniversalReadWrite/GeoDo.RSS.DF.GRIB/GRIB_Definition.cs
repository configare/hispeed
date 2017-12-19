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
        private float _minLongitude;
        private float _minLatitude;
        private float _lonResolution;
        private float _latResolution;
        private int _width;
        private int _height;
        private string _valueName;
        private float _invalidValue;

        public GRIB_Definition(float minLon, float minLat, float resolution, int width, int height, string valueNames, float invalidValue)
        {
            _minLongitude = minLon;
            _minLatitude = minLat;
            _latResolution = _lonResolution = resolution;
            _width = width;
            _height = height;
            _valueName = valueNames;
            _invalidValue = invalidValue;
        }

        public GRIB_Definition(float minLon, float minLat, float lonResolution, float latResolution, int width, int height, string valueNames, float invalidValue)
            : this(minLon, minLat, lonResolution, width, height, valueNames, invalidValue)
        {
            _latResolution = latResolution;
            _lonResolution = lonResolution;
        }

        public float InvalidValue
        {
            get { return _invalidValue; }
        }

        public float MinLongitude
        {
            get { return _minLongitude; }
        }

        public float MinLatitude
        {
            get { return _minLatitude; }
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

        public void ToGeoCoord(ref float longitude, ref float latitude, int index)
        {
            int col = index / _height;
            longitude = _minLongitude + _lonResolution * col;
            latitude = _minLatitude + _latResolution * (index - col * _height);
        }

        public CoordEnvelope GetCoordEnvelope()
        {
            return new CoordEnvelope(_minLongitude, _minLongitude + _width * _lonResolution,
                _minLatitude, _minLatitude + _height * _latResolution);
        }
    }
}
