using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoVis.GeoCore;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace CodeCell.AgileMap.Core
{
    public class RstReader:IRasterReader,IDisposable
    {
        protected Envelope _fullEnvelope = null;
        protected ISpatialReference _sref = null;
        protected enumCoordinateType _coordType = enumCoordinateType.Geographic;
        private IGeoImage _geoImage = null;
        private int _byteCountPerPixel = 0;

        public RstReader(string filename)
        {
            _geoImage = IGeoImage.GeoOpen(filename);
            GeoImage g = _geoImage as GeoImage;
            _fullEnvelope = new Envelope(g.MinX, g.MinY, g.MaxX, g.MaxY);
            _coordType = g.IsProjected ? enumCoordinateType.Projection : enumCoordinateType.Geographic;
            _sref = SpatialReferenceFactory.GetSpatialReferenceByWKT(g.ProjectDescription, enumWKTSource.GDAL);
            _byteCountPerPixel = GetBandCount(_geoImage.DataType);
        }

        private int GetBandCount(LDataType dataType)
        {
            switch (dataType)
            {
                case LDataType.L_RGB:
                    return 3;
                case LDataType.L_ARGB:
                    return 4;
                default:
                    throw new NotSupportedException("不支持的数据类型\""+dataType+"\"!");
            }
        }

        public Envelope GetFullEnvelope()
        {
            return _fullEnvelope;
        }

        public enumCoordinateType GetCoordinateType()
        {
            return _coordType;
        }

        public ISpatialReference GetSpatialReference()
        {
            return _sref;
        }

        public bool IsReady
        {
            get { return true; }
        }

        public void BeginRead()
        {
        }

        public void EndRead()
        {
        }

        private byte[] _buffer = null;
        private int _preWidht = 0;
        private int _preHeight = 0;
        private Bitmap _cachebitmap = null;
        public System.Drawing.Bitmap Read(Envelope evp, int width, int height)
        {
            if (BuildBuffer(width, height))
                _cachebitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            if (Read(evp, width, height, ref _cachebitmap,false))
                return _cachebitmap;
            return null;
        }

        public bool Read(Envelope evp, int width, int height, ref Bitmap bitmap)
        {
            return Read(evp, width, height, ref bitmap, true);
        }

        public bool Read(Envelope evp, int width, int height, ref Bitmap bitmap, bool isReBuildBuffer)
        {
            int offsetX = (int)((evp.MinX - _fullEnvelope.MinX) / _geoImage.ResolutionX);
            int offsetY = (int)((_fullEnvelope.MaxY - evp.MaxY) / _geoImage.ResolutionY);
            int w = (int)(evp.Width / _geoImage.ResolutionX);
            int h = (int)(evp.Height / _geoImage.ResolutionY);
            //if(isReBuildBuffer)
                BuildBuffer(width, height);
            bool isOK = _geoImage.ReadBlockData(1, offsetX, offsetY, w, h, _buffer, 0, width, height);
            if (isOK)
            {
                return (this as IRasterReader).BytesToBitmap(_buffer, width, height, _byteCountPerPixel, ref bitmap);
            }
            return false;
        }

        private bool BuildBuffer(int width, int height)
        {
            if (_buffer == null || _preWidht != width || _preHeight != height)
            {
                if (_buffer != null)
                {
                    _buffer = null;
                    GC.Collect();
                }
                _buffer = new byte[width * height * _byteCountPerPixel];
                _preWidht = width;
                _preHeight = height;
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            if (_cachebitmap != null)
            {
                _cachebitmap.Dispose();
                _cachebitmap = null;
            }
            _geoImage.Dispose();
            _geoImage = null;
        }
    }
}
