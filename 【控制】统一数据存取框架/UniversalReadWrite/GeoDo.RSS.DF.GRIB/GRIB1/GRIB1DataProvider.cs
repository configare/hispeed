using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    public class GRIB1DataProvider:RasterDataProvider,IGRIB1DataProvider
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

        public GRIB1DataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, enumDataProviderAccess access)
            : base(fileName, driver)
        {
            ReadToDataProvider();
        }

        public GRIB_Definition Definition
        {
            get { return _definition; }
        }

        public string TimeUnit
        {
            get { return _timeUnit; }
        }

        public DateTime ReferenceTime
        {
            get { return _referenceTime; }
        }

        private void ReadToDataProvider()
        {
            if (string.IsNullOrEmpty(_fileName)||!File.Exists(_fileName))
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
                SetAttributes(gds,pds);
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

        private void SetAttributes(Grib1GridDefinitionSection gds,Grib1ProductDefinitionSection pds)
        {
            if (gds != null)
            {
                _definition = new GRIB_Definition((float)gds.LonFirstPoint, (float)gds.LatEndPoint, (float)gds.Dy, gds.Nx, gds.Ny, pds.Level.Name, float.NaN);
                _height = gds.Ny;
                _resolutionX = (float)gds.Dx;
                _resolutionY = (float)gds.Dy;
                _width = gds.Nx;
            }
            _decscale = pds.DecimalScale;
            _referenceTime = pds.ReferenceTime;
            _timeUnit = pds.TimeUnit;
            _isBmsExist = pds.BmsExists();
            _coordEnvelope = _definition.GetCoordEnvelope();
            _dataType = enumDataType.Float;
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
        private int Bits2UInt(int nb, FileStream raf)
        {
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
