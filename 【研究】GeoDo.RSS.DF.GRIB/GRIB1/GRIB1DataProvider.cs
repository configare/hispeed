using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Runtime.InteropServices;
using GeoDo.Project;

namespace GeoDo.RSS.DF.GRIB
{
    public class GRIB1DataProvider : RasterDataProvider, IGRIB1DataProvider
    {
        string _fileHeader;
        private int _edition;
        private long _dataOffset;
        private GRIB_Definition _definition;
        private FileStream _fs;
        private int _decscale = 0;
        private DateTime _referenceTime;
        private string _timeUnit;
        private bool _isBmsExist;
        private float _level;
        private double _firstlat;
        private double _firstlon;
        private double _lastlat;
        private double _lastlon;
        private float _resolution;
        private int _gridtype;
        private ISpatialReference _spatialReff = null;
        private int _scanMode = 0;
        private bool _thinnedGrid;
        private int[] _thinnedXNums;
        private int _thinnedGridNum;
        private bool _xReverse = false;
        private bool _yReverse = false;

        public GRIB1DataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, enumDataProviderAccess access)
            : base(fileName, driver)
        {
            ReadToDataProvider();
            _bandCount = 1;
            LoadBands();
        }

        #region properties
        /// <summary>
        /// x方向上的点是否反转
        /// </summary>
        public bool XReverse
        {
            get { return _xReverse; }
        }

        /// <summary>
        /// y方向上的点是否反转
        /// </summary>
        public bool YReverse
        {
            get { return _yReverse; }
        }

        public bool ThinnedGrid
        {
            get { return _thinnedGrid; }
        }

        public int[] ThinnedXNums
        {
            get { return _thinnedXNums; }
        }

        public int ThinnedGridNum
        {
            get { return _thinnedGridNum; }
        }

        public GRIB_Definition Definition
        {
            get { return _definition; }
        }

        public string TimeUnit
        {
            get { return _timeUnit; }
        }

        public bool IsBmsExist
        {
            get { return _isBmsExist; }
        }

        public long DataOffset
        {
            get { return _dataOffset; }
        }

        public ISpatialReference SpatialRef
        {
            get { return _spatialReff; }
        }

        public DateTime ReferenceTime
        {
            get { return _referenceTime; }
        }

        public int Decscale
        {
            get { return _decscale; }
        }

        public int ScanMode
        {
            get { return _scanMode; }
        }
        #endregion

        private void ReadToDataProvider()
        {
            if (string.IsNullOrEmpty(_fileName) || !File.Exists(_fileName))
                return;
            long startOffset = -1;
            GribIndicatorSection iSection;
            Grib1ProductDefinitionSection pds;
            Grib1GridDefinitionSection gds = null;
            _fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
            if (SeekHeader(_fs, _fs.Length, out startOffset))
            {
                iSection = new GribIndicatorSection(_fs);
                long EOR = _fs.Position + iSection.GribLength - iSection.SectionLength;
                pds = new Grib1ProductDefinitionSection(_fs);
                if (pds.GdsExists())
                {
                    gds = new Grib1GridDefinitionSection(_fs);
                    _scanMode = gds.ScanMode;
                }
                if (pds.Center == 98)
                {
                    int length = (int)GribNumberHelper.Uint3(_fs);
                    if ((length + _fs.Position) < EOR)
                    {
                        _dataOffset = _fs.Position - 3;
                    }
                    else
                    {
                        _dataOffset = _fs.Position - 2;
                    }
                }
                else
                {
                    _dataOffset = _fs.Position;
                }
                SetAttributes(gds, pds);
                _edition = iSection.GribEdition;
            }
        }


        /// <summary>
        /// 搜寻记录头
        /// </summary>
        /// <param name="raf">文件读取参数对象</param>
        /// <param name="stop"></param>
        /// <param name="startOffset">距离开始的偏移量</param>
        /// <returns>是否找到记录头</returns>
        private bool SeekHeader(FileStream fs, long stop, out long startOffset)
        {
            // 搜寻记录头
            StringBuilder hdr = new StringBuilder();
            int match = 0;
            startOffset = -1;
            while (fs.Position < stop)
            {
                // 代码必须是 "G" "R" "I" "B"
                char c = (char)fs.ReadByte();
                hdr.Append((char)c);
                if (c == 'G')
                {
                    match = 1;
                    startOffset = fs.Position - 1;
                }
                else if ((c == 'R') && (match == 1))
                {
                    match = 2;
                }
                else if ((c == 'I') && (match == 2))
                {
                    match = 3;
                }
                else if ((c == 'B') && (match == 3))
                {
                    _fileHeader = hdr.ToString();
                    return true;
                }
                else
                {
                    match = 0;
                }
            }
            return false;
        }
        private void SetAttributes(Grib1GridDefinitionSection gds, Grib1ProductDefinitionSection pds)
        {
            if (gds != null)
            {
                _definition = new GRIB_Definition((float)gds.LonFirstPoint, (float)gds.LatFirstPoint, (float)gds.LonEndPoint, (float)gds.LatEndPoint,
                                                  (float)gds.Dx, (float)gds.Dy, gds.Nx, gds.Ny, pds.Level.Name, pds.Level.Value1);
                _level = pds.Level.Value1;
                _firstlat = gds.LatFirstPoint;
                _firstlon = gds.LonFirstPoint;
                _lastlat = gds.LatEndPoint;
                _lastlon = gds.LonEndPoint;
                //日本的J区域网格为“瘦网格”，即每行格点数不同，
                //已通过GDS中的_thinnedGrid值计算dx和Nx
                //if (pds.Center == 34 && _firstlat == 0 && _firstlon == 60 && _lastlat == 90 && _lastlon == 150) //日本J区域
                //{
                //    _resolutionX = (float)gds.Dy;
                //    _resolutionY = (float)gds.Dy;
                //    _height = gds.Ny;
                //    _width = gds.Ny;
                //}
                //else
                {
                    _resolutionX = (float)gds.Dx;
                    _resolutionY = (float)gds.Dy;
                    _height = gds.Ny;
                    _width = gds.Nx;
                }
                _resolution = gds.Resolution;
                _coordEnvelope = _definition.GetCoordEnvelope();
                _gridtype = gds.GridType;
                _thinnedGrid = gds.ThinnedGrid;
                _thinnedGridNum = gds.ThinnedGridNum;
                _thinnedXNums = gds.ThinnedXNums;
                _xReverse = gds.XReverse;
                _yReverse = gds.YReverse;
            }
            else if (pds.Center == 7)//美国KWBC,
            {
                _resolutionX = 5.0f;
                _gridtype = 0;
                if (pds.Grid_Id == 21)//A区域
                {
                    _definition = new GRIB_Definition(180f, 0f, 0f, 90f, 5f, 2.5f, 37, 36, null, -9999);
                    _width = _definition.Width;
                    _height = _definition.Height;
                    _firstlat = 0;
                    _firstlon = 0;
                    _lastlat = 90;
                    _resolutionY = 2.5f;
                    _coordEnvelope = new CoordEnvelope(0, 180, 0, 90);
                }
                else if (pds.Grid_Id == 22)//B区域
                {
                    _definition = new GRIB_Definition(-180f, 0f, 0f, 90f, 5f, 2.5f, 37, 36, null, -9999);
                    _width = _definition.Width;
                    _height = _definition.Height;
                    _firstlat = 0;
                    _firstlon = -180;
                    _lastlat = 90;
                    _resolutionY = 2.5f;
                    _coordEnvelope = new CoordEnvelope(-180, 0, 0, 90);
                }
                else if (pds.Grid_Id == 23)//C区域
                {
                    _definition = new GRIB_Definition(0f, -90f, 180f, 0f, 5f, 2.5f, 37, 36, null, -9999);
                    _width = _definition.Width;
                    _height = _definition.Height;
                    _firstlat = -90;
                    _firstlon = 0;
                    _lastlat = 0;
                    _resolutionY = 2.5f;
                    _coordEnvelope = new CoordEnvelope(0, 180, -90, 0);
                }
                else if (pds.Grid_Id == 24)//D区域
                {
                    _definition = new GRIB_Definition(-180f, -90f, -90f, 0f, 5f, 2.5f, 37, 36, null, -9999);
                    _width = _definition.Width;
                    _height = _definition.Height;
                    _firstlat = -90;
                    _firstlon = -180;
                    _lastlat = 0;
                    _resolutionY = 2.5f;
                    _coordEnvelope = new CoordEnvelope(-180, 0, -90, 0);
                }
                else if (pds.Grid_Id == 25)//北半球
                {
                    _definition = new GRIB_Definition(-180f, 0f, 180f, 90f, 5f, 5f, 72, 18, null, -9999);
                    _width = _definition.Width;
                    _height = _definition.Height;
                    _firstlat = 0;
                    _firstlon = -180;
                    _lastlat = 90;
                    _resolutionY = 5.0f;
                    _coordEnvelope = new CoordEnvelope(-180, 180, 0, 90);
                }
                else if (pds.Grid_Id == 26)//南半球
                {
                    _definition = new GRIB_Definition(-180f, -90f, 180f, 0f, 5f, 5f, 72, 18, null, -9999);
                    _width = _definition.Width;
                    _height = _definition.Height;
                    _firstlat = -90;
                    _firstlon = -180;
                    _lastlat = 0;
                    _resolutionY = 5.0f;
                    _coordEnvelope = new CoordEnvelope(-180, 180, -90, 0);
                }
            }
            _decscale = pds.DecimalScale;
            _referenceTime = pds.ReferenceTime;
            _timeUnit = pds.TimeUnit;
            _isBmsExist = pds.BmsExists();
            _dataType = enumDataType.Float;
            using (SpatialReferenceBuilder builder = new SpatialReferenceBuilder())
            {
                int prjType = GetPrjTypeByGridType(_gridtype);
                _spatialReff = builder.GetSpatialRef(prjType, _firstlat, _lastlat, 0);
            }
        }

        private int GetPrjTypeByGridType(int gridtype)
        {

            //gridType:
            //0: "Latitude/Longitude Grid";            1: "Mercator Projection Grid";
            //2: "Gnomonic Projection Grid";           3: "Lambert Conformal";
            //4: "Gaussian Latitude/Longitude";        5: "Polar Stereographic projection Grid";
            //6: "Universal Transverse Mercator";      7: "Simple polyconic projection";
            //8: "Albers equal-area, secant or tangent, conic or bi-polar, projection";
            //9: "Miller's cylindrical projection";    10: "Rotated latitude/longitude grid";
            //13: "Oblique Lambert conformal, secant or tangent, conical or bipolar, projection";
            //14: "Rotated Gaussian latitude/longitude grid";
            //20: "Stretched latitude/longitude grid";  24: "Stretched Gaussian latitude/longitude grid";
            //30: "Stretched and rotated latitude/longitude grids";
            //34: "Stretched and rotated Gaussian latitude/longitude grids";
            //50: "Spherical Harmonic Coefficients";    60: "Rotated spherical harmonic coefficients";
            //70: "Stretched spherical harmonics";      80: "Stretched and rotated spherical harmonic coefficients";
            //90: "Space view perspective or orthographic";
            //201: "Arakawa semi-staggered E-grid on rotated latitude/longitude grid-point array";
            //202: "Arakawa filled E-grid on rotated latitude/longitude grid-point array";
            //prjType:
            //0: 不投影, 1: 等角投影 2: 麦卡托投影, 3: 兰布托投影  4: 极射赤面投影, 5: 艾尔伯斯投影 
            switch (gridtype)
            {
                case 0: return 0;
                case 1: return 2;
                case 2: return 9999;
                case 3: return 3;
                case 4: return 0;
                case 5: return 4;
                case 6: return 2;
                case 7: return 7;
                case 8: return 5;
                case 9: return 9;
                case 10: return 0;
                case 13: return 3;
                case 14: return 0;
                case 20: return 0;
                case 24: return 0;
                case 30: return 0;
                case 34: return 0;
                default:
                    return gridtype;
            }
        }

        public GRIB_Point[] Read()
        {
            try
            {
                if (_fs != null)
                {
                    IGribBitMapSection bms = null;
                    GRIB_Point[] points = null;
                    bool isConstant = false;
                    _fs.Seek(_dataOffset, SeekOrigin.Begin);
                    if (_isBmsExist)
                        bms = new Grib1BitMapSection(_fs);
                    // octets 1-3 (section _length)
                    int dataLength = GribNumberHelper.Uint3(_fs);

                    // octet 4, 1st half (packing flag)
                    int unusedbits = _fs.ReadByte();

                    if ((unusedbits & 192) != 0)
                        throw new NotSupportedException("BDS: (octet 4, 1st half) not grid point data and simple packing ");

                    // octet 4, 2nd half (number of unused bits at end of this section)
                    unusedbits = unusedbits & 15;

                    // octets 5-6 (binary scale factor)
                    int binscale = GribNumberHelper.Int2(_fs);

                    // octets 7-10 (reference point = minimum value)
                    float refvalue = GribNumberHelper.Float4(_fs);

                    // octet 11 (number of bits per value)
                    int numbits = _fs.ReadByte();
                    if (numbits == 0)
                        isConstant = true;
                    float refRenamed = (float)(Math.Pow(10.0, -_decscale) * refvalue);
                    float scale = (float)(System.Math.Pow(10.0, -_decscale) * System.Math.Pow(2.0, binscale));

                    if (_firstlat == 0 & _lastlat == 90)
                    {
                        if (bms != null)
                        {
                            bool[] bitmap = bms.Bitmap;
                            points = new GRIB_Point[bitmap.Length];
                            for (int i = 0; i < bitmap.Length; i++)
                            {
                                int m = (_height - (int)(i / _width) - 1) * _width + (i % _width);
                                if (bitmap[i])
                                {
                                    if (!isConstant)
                                    {
                                        points[m] = new GRIB_Point();
                                        points[m].Index = i;
                                        points[m].Value = refRenamed + scale * Bits2UInt(numbits, _fs);
                                    }
                                    else
                                    {
                                        // rdg - added this to handle a constant valued parameter
                                        points[m] = new GRIB_Point();
                                        points[m].Index = i;
                                        points[m].Value = refRenamed;
                                    }
                                }
                                else
                                {
                                    points[m] = new GRIB_Point();
                                    points[m].Index = i;
                                    points[m].Value = -9999f;
                                }
                            }
                        }
                        else
                        {
                            if (!isConstant)
                            {
                                points = new GRIB_Point[((dataLength - 11) * 8 - unusedbits) / numbits];
                                for (int i = 0; i < points.Length; i++)
                                {
                                    int m = (_height - (int)(i / _width) - 1) * _width + (i % _width);
                                    points[m] = new GRIB_Point();
                                    points[m].Index = i;
                                    points[m].Value = refRenamed + scale * Bits2UInt(numbits, _fs);
                                }
                            }
                            else
                            {
                                // constant valued - same min and max
                                int x = 0, y = 0;
                                _fs.Seek(_fs.Position - 53, SeekOrigin.Begin); // return to start of GDS
                                dataLength = (int)GribNumberHelper.Uint3(_fs);
                                if (dataLength == 42)
                                {
                                    // Lambert/Mercator offset
                                    _fs.Seek(3, SeekOrigin.Current);
                                    x = GribNumberHelper.Int2(_fs);
                                    y = GribNumberHelper.Int2(_fs);
                                }
                                else
                                {
                                    _fs.Seek(7, SeekOrigin.Current);
                                    dataLength = GribNumberHelper.Uint3(_fs);
                                    if (dataLength == 32)
                                    {
                                        // Polar sterographic
                                        _fs.Seek(3, SeekOrigin.Current);
                                        x = GribNumberHelper.Int2(_fs);
                                        y = GribNumberHelper.Int2(_fs);
                                    }
                                    else
                                    {
                                        x = y = 1;
                                    }
                                }
                                points = new GRIB_Point[x * y];
                                for (int i = 0; i < points.Length; i++)
                                {
                                    int m = (_height - (int)(i / _width) - 1) * _width + (i % _width);
                                    points[m] = new GRIB_Point();
                                    points[m].Index = i;
                                    points[m].Value = refRenamed;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (bms != null)
                        {
                            bool[] bitmap = bms.Bitmap;
                            points = new GRIB_Point[bitmap.Length];
                            for (int i = 0; i < bitmap.Length; i++)
                            {
                                if (bitmap[i])
                                {
                                    if (!isConstant)
                                    {
                                        points[i] = new GRIB_Point();
                                        points[i].Index = i;
                                        points[i].Value = refRenamed + scale * Bits2UInt(numbits, _fs);
                                    }
                                    else
                                    {
                                        // rdg - added this to handle a constant valued parameter
                                        points[i] = new GRIB_Point();
                                        points[i].Index = i;
                                        points[i].Value = refRenamed;
                                    }
                                }
                                else
                                {
                                    points[i] = new GRIB_Point();
                                    points[i].Index = i;
                                    points[i].Value = -9999f;
                                }
                            }
                        }
                        else
                        {
                            if (!isConstant)
                            {
                                points = new GRIB_Point[((dataLength - 11) * 8 - unusedbits) / numbits];
                                for (int i = 0; i < points.Length; i++)
                                {
                                    points[i] = new GRIB_Point();
                                    points[i].Index = i;
                                    points[i].Value = refRenamed + scale * Bits2UInt(numbits, _fs);
                                }
                            }
                            else
                            {
                                // constant valued - same min and max
                                int x = 0, y = 0;
                                _fs.Seek(_fs.Position - 53, SeekOrigin.Begin); // return to start of GDS
                                dataLength = (int)GribNumberHelper.Uint3(_fs);
                                if (dataLength == 42)
                                {
                                    // Lambert/Mercator offset
                                    _fs.Seek(3, SeekOrigin.Current);
                                    x = GribNumberHelper.Int2(_fs);
                                    y = GribNumberHelper.Int2(_fs);
                                }
                                else
                                {
                                    _fs.Seek(7, SeekOrigin.Current);
                                    dataLength = GribNumberHelper.Uint3(_fs);
                                    if (dataLength == 32)
                                    {
                                        // Polar sterographic
                                        _fs.Seek(3, SeekOrigin.Current);
                                        x = GribNumberHelper.Int2(_fs);
                                        y = GribNumberHelper.Int2(_fs);
                                    }
                                    else
                                    {
                                        x = y = 1;
                                    }
                                }
                                points = new GRIB_Point[x * y];
                                for (int i = 0; i < points.Length; i++)
                                {
                                    points[i] = new GRIB_Point();
                                    points[i].Index = i;
                                    points[i].Value = refRenamed;
                                }
                            }
                        }
                    }
                    return points;
                }
                return null;
            }
            finally
            {
                _fs.Seek(0, SeekOrigin.Begin);
            }
        }

        /// <summary> 
        /// Convert bits (nb) to Unsigned Int .
        /// </summary>
        /// <param name="nb"></param>
        /// <param name="raf"></param>
        public int Bits2UInt(int nb, FileStream raf)
        {
            byte[] buffers = new byte[2];
            raf.Read(buffers, 0, 2);
            raf.Seek(-2, SeekOrigin.Current);
            uint val;
            using (BitStream bs = new BitStream(new MemoryStream(buffers)))
            {
                bs.Position = 0;
                bs.Read(out val, 0, nb);
            }
            if (buffers[0] == 3 && buffers[1] == 0)
                Console.WriteLine("");

            int bitsLeft = nb;
            int result = 0;

            int bitBuffer = raf.ReadByte();
            int bitPosition = 8;

            while (true)
            {
                int shift = bitsLeft - bitPosition;
                if (shift > 0)
                {
                    // Consume the entire buffer
                    result |= bitBuffer << shift;
                    bitsLeft -= bitPosition;

                    // Get the next byte from the RandomAccessFile
                    bitBuffer = raf.ReadByte();
                    bitPosition = 8;
                }
                else
                {
                    // Consume a portion of the buffer
                    result |= bitBuffer >> -shift;
                    bitPosition -= bitsLeft;
                    bitBuffer &= 0xff >> (8 - bitPosition); // mask off consumed bits
                    //if (val != result)
                    //    throw new Exception();
                    return result;
                }
            }
        }

        public GRIB_Point[] Read(CoordEnvelope geoEnvelope)
        {
            throw new NotImplementedException();
        }

        public void Read(IntPtr buffer)
        {
            throw new NotImplementedException();
        }

        public void Read(IntPtr buffer, CoordEnvelope geoEnvelope)
        {
            throw new NotImplementedException();
        }

        public void StatMinMax(GRIB_Point[] pts, out GRIB_Point minPoint, out GRIB_Point maxPoint)
        {
            throw new NotImplementedException();
        }

        public IArrayRasterDataProvider ToArrayDataProvider(GRIB_Point[] points)
        {
            throw new NotImplementedException();
        }

        private void LoadBands()
        {
            _rasterBands.Add(new GRIB1Band(this, _fs));
        }

        public override void AddBand(enumDataType dataType)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            _fs.Dispose();
            base.Dispose();
        }
    }
}
