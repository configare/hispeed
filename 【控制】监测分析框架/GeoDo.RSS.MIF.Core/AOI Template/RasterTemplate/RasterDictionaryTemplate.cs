using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    public abstract class RasterDictionaryTemplate<T> : IRasterDictionaryTemplate<T>,
        IRasterTemplateProvider,IDisposable
    {
        protected T[] _pixelValues = null;
        protected Dictionary<T, string> _pixelNames = null;
        protected HdrFile _hdrInfo = null;
        protected int _lines = 0;
        protected int _samples = 0;
        protected double _baseLatitude = 0;
        protected double _baseLongitude = 0;
        protected double _resolutionX = 0;
        protected double _resolutionY = 0;
        protected int _scaleFactor = 1;

        protected void ExtractFieldsFromHdr()
        {
            if (_hdrInfo == null || _hdrInfo.MapInfo == null)
                return;
            _lines = _hdrInfo.Lines;
            _samples = _hdrInfo.Samples;
            _baseLatitude = _hdrInfo.MapInfo.BaseMapCoordinateXY.Latitude;
            _baseLongitude = _hdrInfo.MapInfo.BaseMapCoordinateXY.Longitude;
            _resolutionX = _hdrInfo.MapInfo.XYResolution.Longitude;
            _resolutionY = _hdrInfo.MapInfo.XYResolution.Latitude;
        }

        public Dictionary<T, string> CodeNameParis
        {
            get { return _pixelNames; }
        }

        public string[] Names
        {
            get
            {
                if (_pixelNames == null)
                    return null;
                IEnumerable<string> names = _pixelNames.Values;
                if (names == null || names.Count() == 0)
                    return null;
                return names.ToArray();
            }
        }

        public string GetPixelName(T pixelValue)
        {
            return _pixelNames.ContainsKey(pixelValue) ? _pixelNames[pixelValue] : string.Empty;
        }

        public string GetPixelName(double geoX, double geoY)
        {
            int row = 0, col = 0;
            GetRowCol(geoX, geoY, out row, out col);
            if (row < 0 || row >= _lines)
                return string.Empty;
            if (col < 0 || col >= _samples)
                return string.Empty;
            if (_scaleFactor == 1)
            {
                return GetPixelName(_pixelValues[row * _samples + col]);
            }
            else
            {
                int sample = _samples / _scaleFactor;
                row = row / _scaleFactor;
                col = col / _scaleFactor;
                return GetPixelName(_pixelValues[row * sample + col]);
            }
        }

        public T GetCode(double geoX, double geoY)
        {
            int row = 0, col = 0;
            GetRowCol(geoX, geoY, out row, out col);
            if (row < 0 || row >= _lines)
                return default(T);
            if (col < 0 || col >= _samples)
                return default(T);
            if (_scaleFactor == 1)
            {
                return _pixelValues[row * _samples + col];
            }
            else
            {
                int sample = _samples / _scaleFactor;
                row = row / _scaleFactor;
                col = col / _scaleFactor;
                return _pixelValues[row * sample + col];
            }
        }

        public string GetPixelName(int row, int col)
        {
            T idx;
            if (_scaleFactor == 1)
            {
                idx = _pixelValues[row * _samples + col];
                return GetPixelName(idx);
            }
            else
            {
                int sample = _samples / _scaleFactor;
                row = row / _scaleFactor;
                col = col / _scaleFactor;
                idx = _pixelValues[row * sample + col];
                return GetPixelName(idx);
            }
        }

        private void GetRowCol(double geoX, double geoY, out int row, out int col)
        {
            row = (int)((_baseLatitude - geoY) / _resolutionY);
            col = (int)((geoX - _baseLongitude) / _resolutionX);
        }

        public int[] GetAOI(string name, double minX, double maxX, double minY, double maxY, Size outSize)
        {
            double resX = (maxX - minX) / outSize.Width;
            double resY = (maxY - minY) / outSize.Height;
            double x = minX, y = maxY;
            List<int> idxes = new List<int>();
            int idx = 0;
            string name1 = null;
            for (int r = 0; r < outSize.Height; r++, x = minX)
            {
                y -= r * resY;
                for (int c = 0; c < outSize.Width; c++, idx++)
                {
                    x += c * resX;
                    name1 = GetPixelName(x, y);
                    if (name == string.Empty || name1.Length < name.Length)
                        continue;
                    if (name1.Substring(0, name.Length) == name)
                        idxes.Add(idx);
                }
            }
            return idxes.Count > 0 ? idxes.ToArray() : null;
        }

        public int[] GetAOIByKey(T code, double minX, double maxX, double minY, double maxY, Size outSize)
        {
            double resX = (maxX - minX) / outSize.Width;
            double resY = (maxY - minY) / outSize.Height;
            double x = minX, y = maxY;
            List<int> idxes = new List<int>();
            int idx = 0;
            T codeC;
            for (int r = 0; r < outSize.Height; r++, x = minX)
            {
                y -= r * resY;
                for (int c = 0; c < outSize.Width; c++, idx++)
                {
                    x += c * resX;
                    codeC = GetPixelValue(x, y);
                    //if (name == string.Empty || name1.Length < name.Length)
                    //    continue;
                    //if (name1.Substring(0, name.Length) == name)
                    //    idxes.Add(idx);
                    if (codeC!=null&&codeC.Equals(code))
                        idxes.Add(idx);
                }
            }
            return idxes.Count > 0 ? idxes.ToArray() : null;
        }

        public T GetPixelValue(double geoX, double geoY)
        {
            int row = 0, col = 0;
            GetRowCol(geoX, geoY, out row, out col);
            if (row < 0 || row >= _lines)
                return default(T);
            if (col < 0 || col >= _samples)
                return default(T);
            if (_scaleFactor == 1)
            {
                return _pixelValues[row * _samples + col];
            }
            else
            {
                int sample = _samples / _scaleFactor;
                row = row / _scaleFactor;
                col = col / _scaleFactor;
                return _pixelValues[row * sample + col];
            }
        }

        #region IRasterTemplateProvider 成员


        #endregion



        public void Dispose()
        {
            _pixelNames = null;
            _pixelValues = null;
        }
    }
}
