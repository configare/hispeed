using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace GeoDo.RSS.DF.GRIB
{
    internal class GRIB1Band : RasterBand, IGRIB1Band, IBandOperator
    {
        protected FileStream _fileStream = null;
        protected long _dataoffset;
        protected bool _isBmsExist;
        protected int _decscale = 0;
        protected double _firstlat;
        protected double _lastlat;
        protected double _firstlon;
        protected double _lastlon;
        protected double _xresolution;
        protected int _scanMode = 0;
        private bool _thinnedGrid;
        private int[] _thinnedXNums;
        private int _thinnedGridNum;
        private bool _xReverse = false;
        private bool _yReverse = false;

        public GRIB1Band(IRasterDataProvider rasterDataProvider, FileStream fileStream)
            : base(rasterDataProvider)
        {
            _rasterDataProvider = rasterDataProvider;
            _fileStream = fileStream;
            GRIB1DataProvider grib1DataProvider = rasterDataProvider as GRIB1DataProvider;
            _bandNo = 1;
            _resolutionX = grib1DataProvider.Definition.LonResolution;
            _resolutionY = grib1DataProvider.Definition.LatResolution;
            _coordEnvelope = grib1DataProvider.Definition.GetCoordEnvelope();
            _dataType = grib1DataProvider.DataType;
            _spatialRef = grib1DataProvider.SpatialRef;
            _width = (rasterDataProvider as IGRIB1DataProvider).Width;
            _height = (rasterDataProvider as IGRIB1DataProvider).Height;
            _dataoffset = grib1DataProvider.DataOffset;
            _isBmsExist = grib1DataProvider.IsBmsExist;
            _firstlat = grib1DataProvider.Definition.FirstLatitude;
            _lastlat = grib1DataProvider.Definition.EndLatitude;
            _firstlon = grib1DataProvider.Definition.FirstLongitude;
            _lastlon = grib1DataProvider.Definition.EndLongitude;
            _xresolution = grib1DataProvider.Definition.LonResolution;
            _decscale = grib1DataProvider.Decscale;
            _scanMode = grib1DataProvider.ScanMode;
            _thinnedGrid = grib1DataProvider.ThinnedGrid;
            _thinnedGridNum = grib1DataProvider.ThinnedGridNum;
            _thinnedXNums = grib1DataProvider.ThinnedXNums;
            _xReverse = grib1DataProvider.XReverse;
            _yReverse = grib1DataProvider.YReverse;
        }

        public override void ComputeHistogram(double begin, double end, int buckets, int[] histogram, bool isIncludeOutOfRange, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override void ComputeMinMax(double begin, double end, out double min, out double max, bool isCanApprox, Action<int, string> progressCallback)
        {
            min = max = 0;
            if (begin < double.Epsilon && end < double.Epsilon)
            {
                ComputeMinMax(out min, out max, isCanApprox, progressCallback);
                return;
            }
            int interleave = isCanApprox ? GeoDo.RSS.Core.DF.Constants.DEFAULT_PIXELES_INTERLEAVE : 1;
            MaxMinValuesComputer.ComputeMinMax(this, interleave, begin, end, out min, out max, isCanApprox, progressCallback);
        }

        public override void ComputeMinMax(out double min, out double max, bool isCanApprox, Action<int, string> progressCallback)
        {
            int interleave = isCanApprox ? GeoDo.RSS.Core.DF.Constants.DEFAULT_PIXELES_INTERLEAVE : 1;
            MaxMinValuesComputer.ComputeMinMax(this, interleave, out min, out max, isCanApprox, progressCallback);
        }

        public override void ComputeStatistics(double begin, double end, out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        public override void ComputeStatistics(out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            throw new NotImplementedException();
        }

        protected override void DirectRead(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            if (xSize == xBufferSize && ySize == yBufferSize)
                DirectReadGRIB1Normal(xOffset, yOffset, xSize, ySize, buffer, enumDataType.Float, xBufferSize, yBufferSize);
            else
                DirectReadGRIB1Sample(xOffset, yOffset, xSize, ySize, buffer, enumDataType.Float, xBufferSize, yBufferSize);
        }

        private void DirectReadGRIB1Normal(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType enumDataType, int xBufferSize, int yBufferSize)
        {
            float[] data = Read(xOffset, yOffset, xSize, ySize);
            Marshal.Copy(data, 0, buffer, data.Length);
        }

        private void DirectReadGRIB1Sample(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType enumDataType, int xBufferSize, int yBufferSize)
        {
            throw new NotImplementedException();
            //float rowScale = ySize / (float)yBufferSize;
            //float colScale = xSize / (float)xBufferSize;
        }

        /// <summary>
        /// Binary Data Section (BDS)
        /// 数据区域读取
        /// 1、头偏移，_dataoffset，数据区域开始位置偏移的字节数。
        /// 2、数据区域结构
        ///     字节数 字节位置 说明
        ///     bit[8*3] 01-03, bytesLength， (octets，字节数)
        ///     bit[8] 04, unusedbits, 前4bit："flag"，后4bit：(number of unused bits at end of this section)
        ///             flag的含义；
        ///     bit[8*2] 05-06, binscale, 二进制比例因子,The binary scale factor (E)
        ///     bit[8*4] 07-10, refvalue, reference value = minimum value
        ///     bit[8] 11, numbits, 每个像元使用的bit个数
        ///     
        ///     bit[nnn] 12-nnn ,Variable, depending on octet 4; zero filled to an even number of octets.依赖第四个字节的变量值
        ///     
        /// </summary>
        /// <param name="xoffset"></param>
        /// <param name="yoffset"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        private float _refValue;
        private int _binScale;
        private int _numBits;
        private bool _isConstant;

        public float[] Read(int xoffset, int yoffset, int width, int height)
        {
            try
            {
                if (_fileStream == null)
                    return null;
                if (width > _width | height > _height)
                    return null;
                if (_isBmsExist)
                    throw new NotSupportedException("不支持BitMapSection存在情况下的数据读取");

                _fileStream.Seek(_dataoffset, SeekOrigin.Begin);
                int sectionBytesLength = GribNumberHelper.Uint3(_fileStream);   //整个区段的字节长度
                int flagsAndUnusedbits = _fileStream.ReadByte();
                if ((flagsAndUnusedbits & 192) != 0)                            //&1100,取前两位，仅支持是flag[0],flag[1]都是0的情况
                    throw new NotSupportedException("BDS: (octet 4, 1st half) not grid point data and simple packing ");
                int unusedbits = flagsAndUnusedbits & 15;                       //&1111,取后四位的数值

                _binScale = GribNumberHelper.Int2(_fileStream);
                _refValue = ComputeReferenceValue(_fileStream); //GribNumberHelper.Float4(_fileStream);

                _numBits = _fileStream.ReadByte();
                if (_numBits == 0)
                {
                    _isConstant = true;
                    return null;
                }
                float refRenamed = (float)(Math.Pow(10.0, -_decscale) * _refValue);
                float scale = (float)(Math.Pow(10.0, -_decscale) * Math.Pow(2.0, _binScale));

                //long firstOffset = _fileStream.Position;
                //points = new GRIB_Point[((dataLength - 11) * 8 - unusedbits) / numbits];  //当前情况下，整个数据区域的大小
                var bufRetDatas = new float[height * width];
                var allDataBytesLength = (int)Math.Ceiling(((sectionBytesLength - 11) * 8 - unusedbits) / 8d);   //当前情况下，整个二进制数据所占用的字节数
                int realDataLength = width * height * _numBits / 8 + 1;
                var bufferBytes = new byte[allDataBytesLength]; //new byte[Math.Max(allDataBytesLength, realDataLength)]; //

                _fileStream.Read(bufferBytes, 0, allDataBytesLength);
                uint val;
                using (BitStream bs = new BitStream(new MemoryStream(bufferBytes)))
                {
                    bs.Position = 0;
                    if (_thinnedGrid)
                    {
                        return ReadDataWithThinnedValue(sectionBytesLength, unusedbits, refRenamed, scale, width, height, xoffset, yoffset, bs);
                    }

                    for (int j = yoffset; j < yoffset + height; j++)    //缓存数据，上下调整前的第j行
                    {
                        if (_firstlat < _lastlat)           //记录方向为从南到北
                        {
                            bs.Position = ((_height - j - 1) * _width + xoffset) * _numBits;// +bitOffset;
                            for (int i = xoffset; i < xoffset + width; i++)
                            {
                                bs.Read(out val, 0, _numBits);
                                if (_firstlon == 0 && _lastlon > 300) //最大经度超过180在地图上将实际在西半球，经度为大于-180，但日本C数据最大为200°，不再分区域显示，故将界限定为300，影响X区域和Y区域的数据存储
                                {
                                    int halfLon = (int)((_lastlon - _firstlon) / _xresolution);
                                    if (i < halfLon)
                                        bufRetDatas[halfLon + i + width * (j - yoffset)] = (float)Math.Round(refRenamed + scale * val, 6);
                                    else
                                        bufRetDatas[i - halfLon + width * (j - yoffset)] = (float)Math.Round(refRenamed + scale * val, 6);
                                }
                                else
                                {
                                    bufRetDatas[i + width * (j - yoffset)] = (float)Math.Round(refRenamed + scale * val, 6);
                                }
                            }
                        }
                        else
                        {
                            if (j == yoffset)
                            {
                                //_fileStream.Seek((_width * yoffset + xoffset) * _numBits / 8, SeekOrigin.Current);  //起始位置字节流向后偏移的位置，(_width*yoffset+xoffset)表示偏移的点个数
                                //numbits表示每个点的数值所占的位数，除以8表示每个点占的字节数，seek(函数以字节为单位)
                                bs.Position += (_width * yoffset + xoffset) * _numBits;
                            }
                            else
                                //_fileStream.Seek((_width - width) * _numBits / 8, SeekOrigin.Current);
                                bs.Position += (_width - width) * _numBits;
                            for (int i = xoffset; i < xoffset + width; i++)
                            {
                                bs.Read(out val, 0, _numBits);
                                if (_firstlon == 0 && _lastlon > 300)
                                {
                                    int halfLon = (int)((_lastlon - 180 + _xresolution) / _xresolution);
                                    if (i < halfLon)
                                        bufRetDatas[halfLon + i + width * (j - yoffset)] = (float)Math.Round(refRenamed + scale * val, 6);
                                    else
                                        bufRetDatas[i - halfLon + width * (j - yoffset)] = (float)Math.Round(refRenamed + scale * val, 6);
                                }
                                else
                                    bufRetDatas[i + width * (j - yoffset)] = (float)Math.Round(refRenamed + scale * val, 6);
                            }
                        }
                    }
                }
                ScanModeApplier.ApplyScanMode(_scanMode, width, ref bufRetDatas);
                //if (_thinnedGrid)
                //{
                //    return ReadDataWithThinnedValue(bufRetDatas, width, height);
                //}
                return bufRetDatas;
            }
            finally
            {
                _fileStream.Seek(0, SeekOrigin.Begin);
            }
        }

        //private float[] ReadDataWithThinnedValue(float[] middleDatas, int width, int height)
        private float[] ReadDataWithThinnedValue(int sectionBytesLength, int unusedbits, float refRenamed,
                                                 float scale, int width, int height, int xoffset, int yoffset, BitStream bs)
        {
            //首先按顺序读出二进制流中的所有数据
            uint val;
            int middleDataLength = ((sectionBytesLength - 11) * 8 - unusedbits) / _numBits;
            var middleDatas = new float[middleDataLength];
            for (int k = 0; k < middleDataLength; k++)
            {
                bs.Read(out val, 0, _numBits);
                middleDatas[k] = (float)Math.Round(refRenamed + scale * val, 6);
            }

            //按照给出的每行格点数，进行赋值及插值
            double[,] array = new double[height, width];
            int num = 0;
            for (int i = 0; i < height; i++)
            {
                int num2 = _thinnedXNums[i];
                for (int j = 0; j < width; j++)
                {
                    double num3 = (double)j * ((double)num2 - 1.0) / (double)(width - 1);
                    //判断该行的格点数与数据宽度的比值是否为整数,如果为整数，则从临近点取值
                    if (Math.Abs(num3 - (double)((int)num3)) < 1E-10)
                    {
                        array[i, j] = middleDatas[num + (int)num3];
                    }
                    else //
                    {
                        double num4 = middleDatas[num + (int)num3];
                        double num5 = middleDatas[num + (int)num3 + 1];
                        double num6 = -999999.0;
                        double num7 = -999999.0;
                        if (num3 > 1.0)
                        {
                            num6 = middleDatas[num + (int)num3 - 1];
                        }
                        if (num3 < (double)(num2 - 2))
                        {
                            num7 = middleDatas[num + (int)num3 + 2];
                        }
                        if (num6 < -999998.0 || num7 < -999998.0)
                        {
                            array[i, j] = num4 * ((double)((int)num3) + 1.0 - num3) + num5 * (num3 - (double)((int)num3));
                        }
                        else
                        {
                            double value2 = num3 - (double)((float)((int)num3));
                            array[i, j] = ComputeValue2(value2, num6, num4, num5, num7);
                        }
                    }
                }
                num += num2;
            }

            bool yReverse = false;
            if (_firstlat < _lastlat)
                yReverse = true;
            float[] values = new float[width * height];
            for (int i = yoffset; i < yoffset + height; i++)
            {
                for (int j = xoffset; j < xoffset + width; j++)
                {
                    if (yReverse)
                        values[(height - i - 1) * width + j] = (float)Math.Round(array[i, j], 2);
                    else
                        values[i * width + j] = (float)Math.Round(array[i, j], 2);
                }
            }
            return values;
        }

        //计算第i行的点偏移
        private int ComputePointOffset(int i, bool isYReverse)
        {
            if (i > _height - 1)
                return -1;
            int numTotal = 0;
            for (int k = 0; k < i; k++)
                numTotal += _thinnedXNums[k];
            if (!isYReverse)
            {
                return _thinnedGridNum - numTotal - _thinnedXNums[i];
            }
            else
            {
                return numTotal;
            }
        }

        private double ComputeValue2(double STX, double ETX, double ENQ, double BS, double ACK)
        {
            if (Math.Abs(STX) < 1E-10)
                return ENQ;
            if (Math.Abs(STX - 1.0) < 1E-10)
                return BS;
            return (1.0 - STX) * (ENQ + STX * (0.5 * (BS - ETX) + STX * (0.5 * (BS + ETX) - ENQ))) +
                STX * (BS + (1.0 - STX) * (0.5 * (ENQ - ACK) + (1.0 - STX) * (0.5 * (ENQ + ACK) - BS)));
        }

        private float ComputeReferenceValue(FileStream fs)
        {
            int a0 = fs.ReadByte();
            int a1 = fs.ReadByte();
            int a2 = fs.ReadByte();
            int a3 = fs.ReadByte();

            int s, a, b;
            if ((a0 & 128) == 128)
                s = 1;
            else
                s = 0;
            a0 &= 127;
            a = Convert.ToInt32(a0);
            b = a1 * (int)Math.Pow(2, 16) + a2 * (int)Math.Pow(2, 8) + a3;
            return (float)(Math.Pow(-1, s) * Math.Pow(2, -24) * b * Math.Pow(16, (a - 64)));
        }

        protected override void DirectWrite(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            throw new NotImplementedException();
        }

        public override void Fill(double noDataValue)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
