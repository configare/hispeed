using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;

namespace GeoVis.GeoCore
{

    class GeoNormalImage : IGeoImage
    {
        private IntPtr _img;

        #region GeoImage dll

        [DllImport("GeoLImage.dll")]
        public static extern int OpenImgFile(string filename, ref IntPtr img);
        [DllImport("GeoLImage.dll")]
        public static extern void CloseImgFile(IntPtr img);

        [DllImport("GeoLImage.dll")]
        private static extern void CreateOverview(IntPtr lfileID, GeoVis.GeoCore.GeoImage.LPConvertProgress p);


        [DllImport("GeoLImage.dll")]
        public static extern unsafe void ReadImgBlock(IntPtr img, int level, int row, int col, void* buf);

        [DllImport("GeoLImage.dll")]
        public static unsafe extern void ReadImgDataBlock(IntPtr img, int x, int y, int width, int height, int oWidth, int oHeight, void* buf);

        [DllImport("GeoLImage.dll")]
        public static extern void ReadImgInfo(IntPtr img, ref int width, ref int height, ref int bands,
            ref int bits);

        [DllImport("GeoLImage.dll")]
        public static extern void ReadImgDataType(IntPtr img, ref LDataType dataType);

        [DllImport("GeoLImage.dll")]
        public static extern bool TranslateToMap(string pszProjection, double lon, double lat, ref double x, ref double y);

        [DllImport("GeoLImage.dll")]
        public static extern bool TranslateToGeoLatLon(string pszProjection, double x, double y, ref double lon, ref double lat);

        [DllImport("GeoLImage.dll")]
        public static extern unsafe void ReadImgGeoInfo(IntPtr img, ref string psj, double* padf);

        #endregion

        private unsafe void UpdateGeo()
        {

            ReadImgInfo(_img, ref _width, ref _height, ref _bands, ref _bitsPerPixel);
            ReadImgDataType(_img, ref _dataType);
            //--georef
            double[] padf = new double[6];
            fixed (void* p = padf)
            {
                double* ptr = (double*)p;
                ReadImgGeoInfo(_img, ref _psjDesp, ptr);
            }

            _minX = padf[0];
            _maxX = _minX + padf[1] * _width;
            _minY = padf[3] + padf[5] * _height;
            _maxY = padf[3];

            _colCount = (int)Math.Ceiling((double)_width / 512);
            _rowCount = (int)Math.Ceiling((double)_height / 512);

            _levels = 1;
            int nTileX = ColCount;
            int nTileY = RowCount;

            while (nTileX > 1 && nTileY > 1)
            {
                nTileX = (nTileX + 1) >> 1;
                nTileY = (nTileY + 1) >> 1;
                _levels++;
            };

            CurGeoRegion = new GeoRegion(MinX, MaxX, MinY, MaxY);

            if (MaxY > 91 || _minY < -91 || _maxX > 181 || MinX < -181 || (CurGeoRegion.LatitudeSpan < float.Epsilon && CurGeoRegion.MaxLat < float.Epsilon))
            {
                if (ProjectDescription != "")
                {
                    IsProjected = true;
                    double minx = 0, miny = 0, maxx = 0, maxy = 0;
                    TranslateToGeoLatLon(ProjectDescription, _minX,
                    _maxY, ref minx, ref maxy);
                    TranslateToGeoLatLon(ProjectDescription, _maxX,
                    _minY, ref maxx, ref miny);//--如果异常情况，或者投影参数错误，添加处理方法
                    CurGeoRegion = new GeoRegion(minx, maxx, miny, maxy);
                }
                else
                {
                    _minX = 0;
                    _maxX = 1;
                    _minY = 0;
                    _maxY = (double)Height / Width;
                    CurGeoRegion = new GeoRegion(MinX, MaxX, MinY, MaxY);
                }
            }

            _resX = (_maxX - _minX) / _width;
            _rexY = (_maxY - _minY) / _height;

        }

        public override unsafe int ReadTile(int band, int level, int row, int col, byte[] buffer, int offset, LCompressFormat outFormat)
        {
            if (_img == IntPtr.Zero)
                return -1;

            fixed (void* ptr = buffer)
            {
                ReadImgBlock(_img, level, row, col, ptr);
            }

            return 512 * 512 * Bands * BitsPerPixel / 8;
        }

        public override unsafe bool WriteTile(int band, int level, int row, int col, byte[] buffer, int offset, int count)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public string Open(string url)
        {
            OpenImgFile(url, ref _img);
            UpdateGeo();
            return "";
        }


        public override void Dispose()
        {
            if (_img != IntPtr.Zero)
            {
                CloseImgFile(_img);
                _img = IntPtr.Zero;
            }
        }

        public override void CreateOverview(GeoVis.GeoCore.GeoImage.LPConvertProgress p)
        {
            if (_img == IntPtr.Zero)
                return;
            CreateOverview(_img, p);
        }
    }
}
