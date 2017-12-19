using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
//using GeoImagePlugin;
using System.Threading;
using System.IO;
using System.Net;
using System.Xml;
using System.Web;
using System.Drawing;
using System.Globalization;
//using GeoVis.GeoInfo;

namespace GeoVis.GeoCore
{
    
    public abstract class IGeoImage
    {
        #region Variables

        protected int _width;
        protected int _height;
        protected int _bands;
        protected LDataType _dataType;
        protected int _bitsPerPixel;

        protected double _minX;
        protected string _uri;

        protected bool _cacheExists = true;

        private object _tag;

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public string URI
        {
            get { return _uri; }
            set { _uri = value; }
        }

        public double MinX
        {
            get { return _minX; }
            set { _minX = value; }
        }
        protected double _minY;

        public double MinY
        {
            get { return _minY; }
            set { _minY = value; }
        }
        protected double _maxX;

        public double MaxX
        {
            get { return _maxX; }
            set { _maxX = value; }
        }
        protected double _maxY;

        public double MaxY
        {
            get { return _maxY; }
            set { _maxY = value; }
        }

        protected double _resX;

        public double ResolutionX
        {
            get { return _resX; }
            set { _resX = value; }
        }
        protected double _rexY;

        public double ResolutionY
        {
            get { return _rexY; }
            set { _rexY = value; }
        }

        protected int _levels;
        protected int _rowCount;
        protected int _colCount;

        protected LCompressFormat _cFormat;

        public LCompressFormat CFormat
        {
            get { return _cFormat; }
            set { _cFormat = value; }
        }
        
        protected string _psjDesp;

        public string ProjectDescription
        {
            get { return _psjDesp; }
            set { _psjDesp = value; }
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public int Height
        {
            get
            {
                return _height;
            }
        }

        private bool _isProjected;

        public bool IsProjected
        {
            get { return _isProjected; }
            set { _isProjected = value; }
        }

        public int Bands { get { return _bands; } }
       
        public LDataType DataType 
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        public int BitsPerPixel
        {
            get
            {
                switch (DataType)
                {
                    case LDataType.L_Unknown:
                        break;
                    case LDataType.L_Byte:
                        _bitsPerPixel = 8;
                        break;
                    case LDataType.L_UInt16:
                        _bitsPerPixel = 16;
                        break;
                    case LDataType.L_Int16:
                        _bitsPerPixel = 16;
                        break;
                    case LDataType.L_UInt32:
                        _bitsPerPixel = 32;
                        break;
                    case LDataType.L_Int32:
                        _bitsPerPixel = 32;
                        break;
                    case LDataType.L_Float32:
                        _bitsPerPixel = 32;
                        break;
                    case LDataType.L_Float64:
                        _bitsPerPixel = 64;
                        break;
                    case LDataType.L_CInt16:
                        break;
                    case LDataType.L_CInt32:
                        break;
                    case LDataType.L_CFloat32:
                        break;
                    case LDataType.L_CFloat64:
                        break;
                    case LDataType.L_TypeCount:
                        break;
                    case LDataType.L_RGB:
                        _bitsPerPixel = 24;
                        break;
                    case LDataType.L_ARGB:
                        _bitsPerPixel = 32;
                        break;
                    default:
                        break;
                }
                return _bitsPerPixel;
            }
        }

        private ITransformer _transformer;

        public ITransformer Transformer
        {
            get { return _transformer; }
            set { _transformer = value; }
        }

        public int LevelCount { get { return _levels; } }
        public int RowCount { get { return _rowCount; } }
        public int ColCount { get { return _colCount; } }
        public GeoRegion CurGeoRegion = GeoRegion.Global;
       
        #endregion

        public delegate int ReadTileEventHandler(string url, int band, int level, int row,
            int col, LCompressFormat format, byte[] buffer, int offset);

        public ReadTileEventHandler ReadTileEvent;

        public static IGeoImage GeoOpen(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;

             if (System.IO.Path.GetExtension(path) == ".rst")
            {
                try
                {
                    GeoImage img = new GeoImage(path);
                    img.Tag = path;
                    return img;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                GeoNormalImage img = new GeoNormalImage();
                if ("" != img.Open(path))
                {
                    img.Dispose();
                    return null;
                }
                img.Tag = path;
                return img;
            }
        }

        public virtual int ReadTile(int band, int level, int row, int col, byte[] buffer, int offset, LCompressFormat outFormat)
        {
            int r = -1;
            if (ReadTileEvent != null)
                r = ReadTileEvent(_uri, band, level, row, col, outFormat, buffer, offset);
            return r;
        }

        public abstract bool WriteTile(int band, int level, int row, int col, byte[] buffer, int offset, int count);

        public virtual void Dispose()
        { }

        public unsafe virtual bool ReadBlockData(int band, int xOffset, int yOffset, int width, int height, byte[] buffer, int offset,
           int oWidth, int oHeight)
        {
            return ReadBlockData(band, xOffset, yOffset, width, height, buffer, offset,oWidth, oHeight, false);
        }

        public unsafe virtual bool ReadBlockData(int band, int xOffset, int yOffset, int width, int height, byte[] buffer, int offset,
            int oWidth, int oHeight,bool is8bits)
        {
            int BlockSize = 512;
            int _levelCount = this._levels;

            int nTgLevel = (int)((Math.Log(Math.Abs((double)width / oWidth * 1.2)) / Math.Log((float)2)));	//
            nTgLevel = Math.Max(nTgLevel, 0);
            nTgLevel = Math.Min(nTgLevel, _levelCount - 1);

            int nOutBmpW = (int)(((width) / (double)(1 << nTgLevel)) + 0.5);
            int nOutBmpH = (int)(((height) / (double)(1 << nTgLevel)) + 0.5);

            int ax = xOffset;
            int ay = yOffset;

            xOffset = Math.Max(xOffset, 0);
            yOffset = Math.Max(yOffset, 0);

            int nBeginX = (int)Math.Floor((double)xOffset / BlockSize);
            int nEndX = (int)Math.Floor(((double)ax + width - 1) / BlockSize);
            int nBeginY = (int)Math.Floor((double)yOffset / BlockSize);
            int nEndY = (int)Math.Floor(((double)ay + height - 1) / BlockSize);
            nBeginX = Math.Max(nBeginX, 0);
            nBeginY = Math.Max(nBeginY, 0);
            nBeginX = nBeginX >> nTgLevel;
            nEndX = nEndX >> nTgLevel;
            nBeginY = nBeginY >> nTgLevel;
            nEndY = nEndY >> nTgLevel;

            int nLeftOffset = (int)((((double)ax / (1 << nTgLevel)) - nBeginX * BlockSize) + 0.5);		//为正整数
            int nTopOffset = (int)((((double)ay / (1 << nTgLevel)) - nBeginY * BlockSize) + 0.5);			//为正整数

            nLeftOffset = Math.Max(nLeftOffset, 0);
            nTopOffset = Math.Max(nTopOffset, 0);


            int anLeftOffset = (int)((((double)ax / (1 << nTgLevel)) - nBeginX * BlockSize) + 0.5);		//为正整数
            int anTopOffset = (int)((((double)ay / (1 << nTgLevel)) - nBeginY * BlockSize) + 0.5);			//为正整数

            int bytes = this.BitsPerPixel / 8 * Bands;
            int bytesPerBand = bytes;

            byte[] pTmpBlock = new byte[BlockSize * BlockSize * bytes];

            byte[] pTmpDat = new byte[nOutBmpW * nOutBmpH * bytes];
            int dstX, dstY, tileX, tileY, rows, cols;
            bool f = _cacheExists;
            _cacheExists = false;
            fixed (byte* dst = pTmpDat)
            {
                fixed (byte* src = pTmpBlock)
                {
                    for (int i = nBeginY; i <= nEndY; i++)
                        for (int j = nBeginX; j <= nEndX; j++)
                        {
                            int m = ReadTile(band, nTgLevel, i, j, pTmpBlock, 0, LCompressFormat.LNone);
                            if (m == 0)
                            { }
                            dstY = Math.Max(0, (i - nBeginY) * BlockSize - anTopOffset);
                            dstX = Math.Max(0, (j - nBeginX) * BlockSize - anLeftOffset);

                            tileY = (i == nBeginY ? nTopOffset : 0);
                            tileX = (j == nBeginX ? nLeftOffset : 0);


                            rows = Math.Min(nOutBmpH - dstY, BlockSize - tileY);
                            cols = Math.Min(nOutBmpW - dstX, BlockSize - tileX) * bytesPerBand;

                            byte* pDst = dst;
                            pDst += (dstY * nOutBmpW + dstX) * bytesPerBand;
                            byte* pSrc = src;
                            pSrc += (tileY * BlockSize + tileX) * bytesPerBand;
                            for (int y = 0; y < rows; y++)
                            {
                                for (int x = 0; x < cols; x++)
                                    pDst[x] = pSrc[x];
                                pDst += nOutBmpW * bytesPerBand;
                                pSrc += BlockSize * bytesPerBand;
                            }
                        }
                }
            }
            _cacheExists = f;

           

            if (oWidth != nOutBmpW || oHeight != nOutBmpH)
            {
                ImageHelper imghelper = new ImageHelper(Bands, BitsPerPixel);
                //BicubicResample(buffer, pTmpDat, oWidth, oHeight, nOutBmpW, nOutBmpH);
                imghelper.BilinearResample(buffer, pTmpDat, oWidth, oHeight, nOutBmpW, nOutBmpH);
                // Resample(buffer, pTmpDat, oWidth, oHeight, nOutBmpW, nOutBmpH);
            }
            else
            {
                int l = buffer.Length;
                for (int i = 0; i < l; i++)
                    buffer[i] = pTmpDat[i];
            }

             if (is8bits && BitsPerPixel == 16)
            {
                ImageHelper.ConvertTo8bits(buffer, oWidth, oHeight, Bands, _dataType, buffer);
            }

            return true;

        }
        public delegate void LPConvertProgress(int percent);

        public abstract void CreateOverview(GeoVis.GeoCore.GeoImage.LPConvertProgress p);
        

    }
  
}
