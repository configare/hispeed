using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GeoVis.GeoCore
{
    public class GeoImage : IGeoImage
    {
        #region import ImageIoDLL

        enum LInterleave
        {
            LBSQ,
            LBIP
        }

        struct LImageInfo
        {
            public int width;
            public int height;
            public int bands;
            public int bits;
            public LDataType dataType;

            public int levelCount;
            public int nRows;
            public int nCols;

            public LInterleave Interleave; //波段顺序  BIP，BSQ
            public LCompressFormat LCFormate;   //数据是否压缩格式，目前支持zip，jpeg，png

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
            public double[] padf;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024 - 88)]
            public String szProj;

        }

        [DllImport("ImageIODll.dll")]
        private static extern int OpenLFile(string filename, ref IntPtr lfileID, int status);

        [DllImport("ImageIODll.dll")]
        private static extern void CloseLFile(IntPtr lfileID);

        [DllImport("ImageIODll.dll")]
        private static extern void GetImageInfo(IntPtr lfileID, ref LImageInfo info);

       

        [DllImport("ImageIODll.dll")]
        private static extern int CreateLFile(string filename, LImageInfo info, ref IntPtr lfileID);

        [DllImport("ImageIODll.dll")]
        private static extern unsafe int LTileIO(LFRWFlag lf, IntPtr lfileID, int level, int row, int col, void* buf, int band, ref int count);

        [DllImport("ImageIODll.dll")]
        private static extern unsafe void LFRasterBandRead(IntPtr lfileID, int xOffset, int yOffset, int width, int height,
            int oWidth, int oHeight, int band, void* buf);

        [DllImport("ImageIODll.dll")]
        public static extern void CreateLCompress(ref IntPtr lc, LCompressFormat format, LDataType dataType, int band);

        [DllImport("ImageIODll.dll")]
        public static unsafe extern void Compress(IntPtr lc, byte* dest, ref int destlen, byte* src, int srclen);

        [DllImport("ImageIODll.dll")]
        public static extern unsafe void DeCompress(IntPtr lc, byte* dest, ref int destlen, byte* src, int srclen);

        [DllImport("ImageIODll.dll")]
        public static extern void CloseLCompress(IntPtr lc);


        #endregion

        private IntPtr _imgPtr = IntPtr.Zero;
        private IntPtr _imgCompressPtr = IntPtr.Zero;

        public GeoImage()
        {
        }

        public GeoImage(string path)
        {
            Open(path);

            this.Tag = path;
        }

        public string Open(string uri)
        {
            int r = OpenLFile(uri, ref _imgPtr, 0);
            if (r == -1)
            {
                throw new Exception("影像文件无法打开！");
                return "wrong!";
            }
            else
            {
                LImageInfo info = new LImageInfo();
                GetImageInfo(_imgPtr, ref info);
                _width = info.width;
                _height = info.height;
                _bands = info.bands;
                _cFormat = info.LCFormate;

                _levels = info.levelCount;
                _rowCount = info.nRows;
                _colCount = info.nCols;

                _dataType = info.dataType;
                _psjDesp = info.szProj;

                if (_imgCompressPtr == IntPtr.Zero)
                {
                    CreateLCompress(ref _imgCompressPtr, CFormat, _dataType, _bands);
                }

                //----- 经纬范围,unfinished
                _minX = info.padf[0];
                _maxX = _minX + info.padf[1] * info.width;
                _minY = info.padf[3] + info.padf[5] * info.height;
                _maxY = info.padf[3];

                CurGeoRegion = new GeoRegion(MinX, MaxX, MinY, MaxY);

                if (MaxY > 91 || _minY < -91 || _maxX > 181 || MinX < -181 || (CurGeoRegion.LatitudeSpan < float.Epsilon && CurGeoRegion.MaxLat < float.Epsilon))
                {
                    if (ProjectDescription != "")
                    {
                        IsProjected = true;
                        double minx = 0, miny = 0, maxx = 0, maxy = 0;
                        GeoNormalImage.TranslateToGeoLatLon(ProjectDescription, _minX,
                        _maxY, ref minx, ref maxy);
                        GeoNormalImage.TranslateToGeoLatLon(ProjectDescription, _maxX,
                        _minY, ref maxx, ref miny); //--如果异常情况，或者投影参数错误，添加处理方法
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

                return "";
            }
        }

        public override void Dispose()
        {
            if (_imgPtr != IntPtr.Zero)
                CloseLFile(_imgPtr);

            if (_imgCompressPtr != IntPtr.Zero)
                CloseLCompress(_imgCompressPtr);
            base.Dispose();
        }

        object loclObject = new object();
        public override unsafe int ReadTile(int band, int level, int row, int col, byte[] buffer, int offset, LCompressFormat outFormat)
        {
            if (buffer == null || offset < 0)
                return -1;

            int count = 0;
            fixed (byte* ptr = buffer)
            {
                byte* ptrDat = ptr + offset;

                lock (loclObject)
                    LTileIO(LFRWFlag.LF_Read, _imgPtr, level, row, col, ptrDat, band, ref count);

                if (outFormat != CFormat && count > 0)
                {
                    //--解压缩
                    int destLen = 512 * 512 * _bands * BitsPerPixel / 8;

                    lock (loclObject)
                        DeCompress(_imgCompressPtr, (byte*)ptrDat, ref destLen, (byte*)ptrDat, count);

                    //--压缩  
                    IntPtr lc = IntPtr.Zero;
                    CreateLCompress(ref lc, outFormat, _dataType, _bands);
                    Compress(lc, (byte*)ptrDat, ref count, (byte*)ptrDat, destLen);
                    CloseLCompress(lc);
                }

            }

            return count;
        }
        //..
        public override unsafe bool WriteTile(int band, int level, int row, int col, byte[] buffer, int offset, int count)
        {
            fixed (byte* ptr = buffer)
            {
                byte* ptrDat = ptr + offset;
                LTileIO(LFRWFlag.LF_Write, _imgPtr, level, row, col, ptrDat, band, ref count);
            }
            return true;
        }
        //..

        public unsafe GeoImage ToCompressImage(LCompressFormat s, string filename)
        {
            LImageInfo info = new LImageInfo();
            GetImageInfo(_imgPtr, ref info);

            IntPtr of = IntPtr.Zero;
            info.LCFormate = s;
            CreateLFile(filename, info, ref of);
            int nTileY = info.nRows;
            int nTileX = info.nCols;
            int size = 512 * 512 * (info.bits / 8) * info.bands;
            int bytePerLine = 512 * (info.bits / 8) * info.bands;

            byte[] buf = new byte[size];
            int r = 0;
            fixed (void* ptr = buf)
            {
                for (int nLvl = 0; nLvl < info.levelCount; nLvl++)
                {
                    for (int i = 0; i < nTileY; i++)
                    {
                        for (int j = 0; j < nTileX; j++)
                        {
                            r = ReadTile(1, nLvl, i, j, buf, 0, s);
                            LTileIO(LFRWFlag.LF_Write, of, nLvl, i, j, ptr, 1, ref r);
                        }

                    }

                    nTileX = (nTileX + 1) >> 1;
                    nTileY = (nTileY + 1) >> 1;
                }

            }

            CloseLFile(of);

            GeoImage reader = new GeoImage();
            reader.Open(filename);
            return reader;
        }

        //public override void CreateOverview()
        //{
        //    throw new Exception("The method or operation is not implemented.");
        //}

        public override void CreateOverview(IGeoImage.LPConvertProgress p)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

}
