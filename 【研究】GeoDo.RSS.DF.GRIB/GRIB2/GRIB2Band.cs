using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.DF.GRIB
{
    internal class GRIB2Band : RasterBand
    {
        private IGRIB2Record _record = null;
        private FileStream _fs = null;
        private static bool _isValued = true;
        /// <summary> Buffer for one byte which will be processed bit by bit.</summary>
        private int _bitBuf = 0;
        /// <summary> Current bit position in <tt>bitBuf</tt>.</summary>
        private int _bitPos = 0;
        protected double _firstlat;
        protected double _lastlat;
        protected double _firstlon;
        protected double _lastlon;

        public GRIB2Band(IRasterDataProvider rasterDataProvider, FileStream fs, IGRIB2Record record, int bandNo, string productParam)
            : base(rasterDataProvider)
        {
            _rasterDataProvider = rasterDataProvider;
            _fs = fs;
            if (_fs == null)
                _fs = new FileStream(rasterDataProvider.fileName, FileMode.Open, FileAccess.Read);
            GRIB2DataProvider grib2DataProvider = rasterDataProvider as GRIB2DataProvider;
            _bandNo = bandNo;
            _resolutionX = grib2DataProvider.Definition.LonResolution;
            _resolutionY = grib2DataProvider.Definition.LatResolution;
            _firstlat = grib2DataProvider.Definition.FirstLatitude;
            _lastlat = grib2DataProvider.Definition.EndLatitude;
            _firstlon = grib2DataProvider.Definition.FirstLongitude;
            _lastlon = grib2DataProvider.Definition.EndLongitude;
            _coordEnvelope = grib2DataProvider.CoordEnvelope;
            _dataType = grib2DataProvider.DataType;
            _spatialRef = grib2DataProvider.SpatialRef;
            _width = grib2DataProvider.Width;
            _height = grib2DataProvider.Height;
            _description = productParam;
            _record = record;
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
                DirectReadGRIB2Normal(xOffset, yOffset, xSize, ySize, buffer, enumDataType.Float, xBufferSize, yBufferSize);
            else
                DirectReadGRIB2Sample(xOffset, yOffset, xSize, ySize, buffer, enumDataType.Float, xBufferSize, yBufferSize);
        }

        private void DirectReadGRIB2Normal(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType enumDataType, int xBufferSize, int yBufferSize)
        {
            float[] data = ReadNormal(xOffset, yOffset, xBufferSize, yBufferSize);
            Marshal.Copy(data, 0, buffer, data.Length);
        }

        private float[] ReadNormal(int xOffset, int yOffset, int xbufferSize, int ybufferSize)
        {
            if (_record == null)
                return null;
            if (xbufferSize > _width | ybufferSize > _height)
                return null;
            int dataTemplate = _record.DRS.DataTemplate;
            switch (dataTemplate)
            {
                case 0:  //Grid point data - simple packing
                    return ReadGridPointBySimplePack(xOffset, yOffset, xbufferSize, ybufferSize);
                case 1:  //Matrix value - simple packing
                    return null;
                case 2:  //Grid point data - complex packing
                    return ReadGridPointByComplexPack(xOffset, yOffset, xbufferSize, ybufferSize);
                case 3:  //Grid point data - complex packing and spatial differencing
                    return ReadGridPointByComplexPackAndSpatial(xOffset, yOffset, xbufferSize, ybufferSize);
                default:
                    return null;
            }
        }

        private void DirectReadGRIB2Sample(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType enumDataType, int xBufferSize, int yBufferSize)
        {
            throw new NotImplementedException();
        }

        protected override void DirectWrite(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            throw new NotImplementedException();
        }

        public override void Fill(double noDataValue)
        {
            throw new NotImplementedException();
        }

        private float[] ReadGridPointBySimplePack(int xoffset, int yoffset, int width, int height)
        {
            uint val;
            float[] bufRetDatas;
            float num;
            float num2;
            bool[] bitmap;
            byte[] _bufferBytes;
            int numberOfBits;
            float referenceValue;
            GetFsValues(out bufRetDatas, out num, out num2, out bitmap, out _bufferBytes, out numberOfBits, out referenceValue);
            if (bitmap == null)
            {
                ReadDataWithoutBitmap(xoffset, yoffset, width, height, bufRetDatas, num, num2, _bufferBytes, numberOfBits, referenceValue);
            }
            else
            {
                ReadDataWithBitmap(xoffset, yoffset, bufRetDatas, num, num2, bitmap, _bufferBytes, numberOfBits, referenceValue);
            }

            int scanMode = _record.GDS.ScanMode;
            ScanModeApplier.ApplyScanMode(scanMode, width, ref bufRetDatas);
            return bufRetDatas;
        }

        private void ReadDataWithBitmap(int xoffset, int yoffset, float[] bufRetDatas, float num, float num2, bool[] bitmap, byte[] _bufferBytes, int numberOfBits, float referenceValue)
        {
            uint val;
            using (BitStream bs = new BitStream(new MemoryStream(_bufferBytes)))
            {
                for (int j = yoffset; j < _height; j++)
                {
                    if (_firstlat < _lastlat)           //记录方向为从南到北
                        bs.Position = ((_height - j - 1) * _width + xoffset) * numberOfBits;
                    else
                        bs.Position = (j * _width + xoffset) * numberOfBits;
                    for (int i = 0; i < _width; i++)
                    {
                        bs.Read(out val, 0, numberOfBits);
                        if (bitmap[i + _width * (j - yoffset)])
                            bufRetDatas[i + _width * (j - yoffset)] = (referenceValue + val * num2) / num;
                        else
                            bufRetDatas[i + _width * (j - yoffset)] = referenceValue / num;
                    }
                }
            }
        }

        private void ReadDataWithoutBitmap(int xoffset, int yoffset, int width, int height, float[] bufRetDatas, float num, float num2, byte[] _bufferBytes, int numberOfBits, float referenceValue)
        {
            uint val;
            using (BitStream bs = new BitStream(new MemoryStream(_bufferBytes)))
            {
                for (int j = yoffset; j < yoffset + height; j++)    
                {
                    if (_firstlat < _lastlat)           //记录方向为从南到北
                        bs.Position = ((_height - j - 1) * _width + xoffset) * numberOfBits;
                    else                              //记录方向从北到南
                        bs.Position = (j * _width + xoffset) * numberOfBits;
                    for (int i = xoffset; i < xoffset + width; i++)
                    {
                        bs.Read(out val, 0, numberOfBits);
                        bufRetDatas[i + _width * (j - yoffset)] = (referenceValue + val * num2) / num;
                    }
                }
            }
        }

        private void GetFsValues(out float[] bufRetDatas, out float num, out float num2, out bool[] bitmap, out byte[] _bufferBytes, out int numberOfBits, out float referenceValue)
        {
            bufRetDatas = new float[_height * _width];

            // Decimal scale factor (D).
            num = (float)Math.Pow(10, _record.DRS.DecimalScaleFactor);
            //Binary scale factor (E).
            num2 = (float)Math.Pow(2, _record.DRS.BinaryScaleFactor);
            bitmap = null;
            if (_record.BMS != null)
                bitmap = _record.BMS.Bitmap;
            int dataSectionDataLength = _record.DS.DataLength - 5;
            _bufferBytes = new byte[dataSectionDataLength];
            numberOfBits = _record.DRS.NumberOfBits;
            // Reference value (R) (IEEE 32-bit floating-point value).
            referenceValue = _record.DRS.ReferenceValue;

            _fs.Seek(_record.DS.DataOffset, SeekOrigin.Begin);
            _fs.Read(_bufferBytes, 0, dataSectionDataLength);
        }

        private float[] ReadGridPointByComplexPackAndSpatial(int xOffset, int yOffset, int width, int height)
        {
            int mvm = _record.DRS.MissingValueManagement;
            float pmv = _record.DRS.PrimaryMissingValue;
            int NG = _record.DRS.NumberOfGroups;
            int g1 = 0, gMin = 0, h1 = 0, h2 = 0, hMin = 0;
            // [6-ww]   1st values of undifferenced scaled values and minimums
            int os = _record.DRS.OrderSpatial;
            int ds = _record.DRS.DescriptorSpatial;
            _bitPos = 0; _bitBuf = 0;
            int sign;
            // ds is number of bytes, convert to bits -1 for sign bit
            ds = ds * 8 - 1;
            if (os == 1)
            {
                // first order spatial differencing g1 and gMin
                sign = Bits2UInt(1, _fs);
                g1 = Bits2UInt(ds, _fs);
                if (sign == 1)
                {
                    g1 *= (-1);
                }
                sign = Bits2UInt(1, _fs);
                gMin = Bits2UInt(ds, _fs);
                if (sign == 1)
                {
                    gMin *= (-1);
                }
            }
            else if (os == 2)
            {
                //second order spatial differencing h1, h2, hMin
                sign = Bits2UInt(1, _fs);
                h1 = Bits2UInt(ds, _fs);
                if (sign == 1)
                {
                    h1 *= (-1);
                }
                sign = Bits2UInt(1, _fs);
                h2 = Bits2UInt(ds, _fs);
                if (sign == 1)
                {
                    h2 *= (-1);
                }
                sign = Bits2UInt(1, _fs);
                hMin = Bits2UInt(ds, _fs);
                if (sign == 1)
                {
                    hMin *= (-1);
                }
            }
            else
                return null;
            // [ww +1]-xx  Get reference values for groups (X1's)
            int[] X1 = new int[NG];
            int nb = _record.DRS.NumberOfBits;
            _bitPos = 0;
            _bitBuf = 0;
            for (int i = 0; i < NG; i++)
            {
                X1[i] = Bits2UInt(nb, _fs);
            }
            // [xx +1 ]-yy Get number of bits used to encode each group
            int[] NB = new int[NG];
            nb = _record.DRS.BitsGroupWidths;
            _bitPos = 0;
            _bitBuf = 0;
            for (int i = 0; i < NG; i++)
            {
                NB[i] = Bits2UInt(nb, _fs);
            }
            // [yy +1 ]-zz Get the scaled group lengths using formula
            //     Ln = ref + Kn * len_inc, where n = 1-NG,
            //          ref = referenceGroupLength, and  len_inc = lengthIncrement
            int[] L = new int[NG];
            int countL = 0;
            int ref_Renamed = _record.DRS.ReferenceGroupLength;
            int len_inc = _record.DRS.LengthIncrement;
            nb = _record.DRS.BitsScaledGroupLength;
            _bitPos = 0;
            _bitBuf = 0;
            for (int i = 0; i < NG; i++)
            {
                // NG
                L[i] = ref_Renamed + (Bits2UInt(nb, _fs) * len_inc);
                countL += L[i];
            }
            // [zz +1 ]-nn get X2 values and add X1[ i ] + X2
            float[] data = new float[countL];
            // used to check missing values when X2 is packed with all 1's
            int[] bitsmv1 = new int[31];
            for (int i = 0; i < 31; i++)
            {
                bitsmv1[i] = (int)System.Math.Pow((double)2, (double)i) - 1;
            }
            int count = 0;
            int X2;
            _bitPos = 0;
            _bitBuf = 0;
            for (int i = 0; i < NG - 1; i++)
            {
                for (int j = 0; j < L[i]; j++)
                {
                    if (NB[i] == 0)
                    {
                        if (mvm == 0)
                        {
                            // X2 = 0
                            data[count++] = X1[i];
                        }
                        else if (mvm == 1)
                        {
                            data[count++] = pmv;
                        }
                    }
                    else
                    {
                        X2 = Bits2UInt(NB[i], _fs);
                        if (mvm == 0)
                        {
                            data[count++] = X1[i] + X2;
                        }
                        else if (mvm == 1)
                        {
                            // X2 is also set to missing value is all bits set to 1's
                            if (X2 == bitsmv1[NB[i]])
                            {
                                data[count++] = pmv;
                            }
                            else
                            {
                                data[count++] = X1[i] + X2;
                            }
                        }
                    }
                }
            }
            // process last group
            int last = _record.DRS.LengthLastGroup;
            for (int j = 0; j < last; j++)
            {
                // last group
                if (NB[NG - 1] == 0)
                {
                    if (mvm == 0)
                    {
                        // X2 = 0
                        data[count++] = X1[NG - 1];
                    }
                    else if (mvm == 1)
                    {
                        data[count++] = pmv;
                    }
                }
                else
                {
                    X2 = Bits2UInt(NB[NG - 1], _fs);
                    if (mvm == 0)
                    {
                        data[count++] = X1[NG - 1] + X2;
                    }
                    else if (mvm == 1)
                    {
                        // X2 is also set to missing value is all bits set to 1's
                        if (X2 == bitsmv1[NB[NG - 1]])
                        {
                            data[count++] = pmv;
                        }
                        else
                        {
                            data[count++] = X1[NG - 1] + X2;
                        }
                    }
                }
            }
            if (os == 1)
            {
                // g1 and gMin this coding is a sort of guess, no doc
                float sum = 0;
                if (mvm == 0)
                {
                    // no missing values
                    for (int i = 1; i < data.Length; i++)
                    {
                        data[i] += gMin; // add minimum back
                    }
                    data[0] = g1;
                    for (int i = 1; i < data.Length; i++)
                    {
                        sum += data[i];
                        data[i] = data[i - 1] + sum;
                    }
                }
                else
                {
                    // contains missing values
                    float lastOne = pmv;
                    // add the minimum back and set g1
                    int idx = 0;
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i] != pmv)
                        {
                            if (idx == 0)
                            {
                                // set g1
                                data[i] = g1;
                                lastOne = data[i];
                                idx = i + 1;
                            }
                            else
                            {
                                data[i] += gMin;
                            }
                        }
                    }
                    if (lastOne == pmv)
                        return data;
                    for (int i = idx; i < data.Length; i++)
                    {
                        if (data[i] != pmv)
                        {
                            sum += data[i];
                            data[i] = lastOne + sum;
                            lastOne = data[i];
                        }
                    }
                }
            }
            else if (os == 2)
            {
                //h1, h2, hMin
                float hDiff = h2 - h1;
                float sum = 0;
                if (mvm == 0)
                {
                    // no missing values
                    for (int i = 2; i < data.Length; i++)
                    {
                        data[i] += hMin; // add minimum back
                    }
                    data[0] = h1;
                    data[1] = h2;
                    sum = hDiff;
                    for (int i = 2; i < data.Length; i++)
                    {
                        sum += data[i];
                        data[i] = data[i - 1] + sum;
                    }
                }
                else
                {
                    // contains missing values
                    int idx = 0;
                    float lastOne = pmv;
                    // add the minimum back and set h1 and h2
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i] != pmv)
                        {
                            if (idx == 0)
                            {
                                // set h1
                                data[i] = h1;
                                sum = 0;
                                lastOne = data[i];
                                idx++;
                            }
                            else if (idx == 1)
                            {
                                // set h2
                                data[i] = h1 + hDiff;
                                sum = hDiff;
                                lastOne = data[i];
                                idx = i + 1;
                            }
                            else
                            {
                                data[i] += hMin;
                            }
                        }
                    }
                    if (lastOne == pmv)
                        return data;
                    for (int i = idx; i < data.Length; i++)
                    {
                        if (data[i] != pmv)
                        {
                            sum += data[i];
                            data[i] = lastOne + sum;
                            lastOne = data[i];
                        }
                    }
                }
            } // end h1, h2, hMin
            // formula used to create values,  Y * 10**D = R + (X1 + X2) * 2**E
            int D = _record.DRS.DecimalScaleFactor;
            float DD = (float)System.Math.Pow((double)10, (double)D);
            float R = _record.DRS.ReferenceValue;
            int E = _record.DRS.BinaryScaleFactor;
            float EE = (float)System.Math.Pow((double)2.0, (double)E);
            if (mvm == 0)
            {
                // no missing values
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = (R + data[i] * EE) / DD;
                }
            }
            else
            {
                // missing value == 1
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i] != pmv)
                    {
                        data[i] = (R + data[i] * EE) / DD;
                    }
                }
            }
            int scanMode = _record.GDS.ScanMode;
            ScanModeApplier.ApplyScanMode(scanMode, width, ref data);
            return data;
        }

        private float[] ReadGridPointByComplexPack(int xOffset, int yOffset, int width, int height)
        {
            int mvm = _record.DRS.MissingValueManagement;
            float pmv = _record.DRS.PrimaryMissingValue;
            int NG = _record.DRS.NumberOfGroups;
            // 6-xx  Get reference values for groups (X1's)
            int[] X1 = new int[NG];
            int nb = _record.DRS.NumberOfBits;
            _bitPos = 0;
            _bitBuf = 0;
            for (int i = 0; i < NG; i++)
            {
                X1[i] = Bits2UInt(nb, _fs);
            }
            // [xx +1 ]-yy Get number of bits used to encode each group
            int[] NB = new int[NG];
            nb = _record.DRS.BitsGroupWidths;
            _bitPos = 0;
            _bitBuf = 0;
            for (int i = 0; i < NG; i++)
            {
                NB[i] = Bits2UInt(nb, _fs);
            }
            // [yy +1 ]-zz Get the scaled group lengths using formula
            //     Ln = ref + Kn * len_inc, where n = 1-NG,
            //          ref = referenceGroupLength, and  len_inc = lengthIncrement
            int[] L = new int[NG];
            int countL = 0;
            int ref_Renamed = _record.DRS.ReferenceGroupLength;
            int len_inc = _record.DRS.LengthIncrement;
            nb = _record.DRS.BitsScaledGroupLength;
            _bitPos = 0;
            _bitBuf = 0;
            for (int i = 0; i < NG; i++)
            {
                // NG
                L[i] = ref_Renamed + (Bits2UInt(nb, _fs) * len_inc);
                countL += L[i];
            }
            // [zz +1 ]-nn get X2 values and calculate the results Y using formula
            //                Y * 10**D = R + (X1 + X2) * 2**E

            int D = _record.DRS.DecimalScaleFactor;
            float DD = (float)System.Math.Pow((double)10, (double)D);
            float R = _record.DRS.ReferenceValue;
            int E = _record.DRS.BinaryScaleFactor;
            float EE = (float)System.Math.Pow((double)2.0, (double)E);
            float[] data = new float[countL];
            int count = 0;
            int[] bitsmv1 = new int[31];
            for (int i = 0; i < 31; i++)
            {
                bitsmv1[i] = (int)System.Math.Pow((double)2, (double)i) - 1;
            }
            int X2;
            _bitPos = 0;
            _bitBuf = 0;
            for (int i = 0; i < NG - 1; i++)
            {
                for (int j = 0; j < L[i]; j++)
                {
                    if (NB[i] == 0)
                    {
                        if (mvm == 0)
                        {
                            // X2 = 0
                            data[count++] = (R + X1[i] * EE) / DD;
                        }
                        else if (mvm == 1)
                        {
                            data[count++] = pmv;
                        }
                    }
                    else
                    {
                        X2 = Bits2UInt(NB[i], _fs);
                        if (mvm == 0)
                        {
                            data[count++] = (R + (X1[i] + X2) * EE) / DD;
                        }
                        else if (mvm == 1)
                        {
                            // X2 is also set to missing value is all bits set to 1's
                            if (X2 == bitsmv1[NB[i]])
                            {
                                data[count++] = pmv;
                            }
                            else
                            {
                                data[count++] = (R + (X1[i] + X2) * EE) / DD;
                            }
                        }
                    }
                }
            }
            // process last group
            int last = _record.DRS.LengthLastGroup;
            for (int j = 0; j < last; j++)
            {
                // last group
                if (NB[NG - 1] == 0)
                {
                    if (mvm == 0)
                    {
                        // X2 = 0
                        data[count++] = (R + X1[NG - 1] * EE) / DD;
                    }
                    else if (mvm == 1)
                    {
                        data[count++] = pmv;
                    }
                }
                else
                {
                    X2 = Bits2UInt(NB[NG - 1], _fs);
                    if (mvm == 0)
                    {
                        data[count++] = (R + (X1[NG - 1] + X2) * EE) / DD;
                    }
                    else if (mvm == 1)
                    {
                        // X2 is also set to missing value is all bits set to 1's
                        if (X2 == bitsmv1[NB[NG - 1]])
                        {
                            data[count++] = pmv;
                        }
                        else
                        {
                            data[count++] = (R + (X1[NG - 1] + X2) * EE) / DD;
                        }
                    }
                }
            } // end for j
            int scanMode = _record.GDS.ScanMode;
            ScanModeApplier.ApplyScanMode(scanMode, width, ref data);
            return data;
        }

        private int Bits2UInt(int nb, FileStream raf)
        {
            int bitsLeft = nb;
            int result = 0;
            if (_bitPos == 0)
            {
                _bitBuf = raf.ReadByte();
                _bitPos = 8;
            }
            while (true)
            {
                int shift = bitsLeft - _bitPos;
                if (shift > 0)
                {
                    // Consume the entire buffer
                    result |= _bitBuf << shift;
                    bitsLeft -= _bitPos;

                    // Get the next byte from the RandomAccessFile
                    _bitBuf = raf.ReadByte();
                    _bitPos = 8;
                }
                else
                {
                    // Consume a portion of the buffer
                    result |= _bitBuf >> -shift;
                    _bitPos -= bitsLeft;
                    _bitBuf &= 0xff >> (8 - _bitPos); // mask off consumed bits
                    return result;
                }
            }
        }
    }
}
