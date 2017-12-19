using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.DF.MVG
{
    internal class MvgRasterBand : RasterBand, IMvgRasterBand, IBandOperator
    {
        protected MvgHeader _header = null;
        protected FileStream _fileStream = null;
        protected BinaryWriter _binaryWriter = null;
        protected BinaryReader _binaryReader = null;

        public MvgRasterBand(IRasterDataProvider rasterDataProvider, BinaryReader binaryReader, BinaryWriter binaryWriter, FileStream fileStream)
            : base(rasterDataProvider)
        {
            _rasterDataProvider = rasterDataProvider;
            _fileStream = fileStream;
            _binaryReader = binaryReader;
            _binaryWriter = binaryWriter;
            _bandNo = 1;
            _dataType = enumDataType.Int16;
            _header = (rasterDataProvider as IMvgDataProvider).Header;
            _spatialRef = (rasterDataProvider as IMvgDataProvider).Header.SpatialRef;
            _width = (rasterDataProvider as IMvgDataProvider).Width;
            _height = (rasterDataProvider as IMvgDataProvider).Height;
            //_noDataValue
            //CoordTransform
            //MaxByMeasureBits
        }

        public MvgHeader Header
        {
            get { return _header; }
        }

        protected override void DirectRead(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            if (xSize == xBufferSize && ySize == yBufferSize)
                DirectReadMvgNormal(xOffset, yOffset, xSize, ySize, buffer, enumDataType.Int16, xBufferSize, yBufferSize);
            else
                DirectReadMvgSample(xOffset, yOffset, xSize, ySize, buffer, enumDataType.Int16, xBufferSize, yBufferSize);
        }

        private void DirectReadMvgNormal(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType enumDataType, int xBufferSize, int yBufferSize)
        {
            int dataTypeSize = DataTypeHelper.SizeOf(enumDataType.Int16);
            int offset = _header.HeaderSize + yOffset * _width * dataTypeSize;
            int endRow = yOffset + ySize;
            int colOffset = xOffset * dataTypeSize;
            int rowSize = xSize * dataTypeSize;
            int rowOffset = _width * dataTypeSize - rowSize;
            byte[] rowBuffer = new byte[rowSize];
            _fileStream.Seek(offset + colOffset, SeekOrigin.Begin);
            for (int row = yOffset; row < endRow; row++, buffer = IntPtr.Add(buffer, rowSize))
            {
                _fileStream.Read(rowBuffer, 0, rowSize);
                _fileStream.Seek(rowOffset, SeekOrigin.Current);
                Marshal.Copy(rowBuffer, 0, buffer, rowSize);
            }
        }

        private void DirectReadMvgSample(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType enumDataType, int xBufferSize, int yBufferSize)
        {
            int srcDataTypeSize = DataTypeHelper.SizeOf(enumDataType.Int16);
            int rowSize = _width * srcDataTypeSize;
            int offset = _header.HeaderSize + yOffset * rowSize;
            int endRow = yOffset + ySize;
            int colOffset = xOffset * srcDataTypeSize;
            int srcRowBlockSize = xSize * srcDataTypeSize;
            byte[] srcRowBlockBuffer = new byte[srcRowBlockSize];
            int dstRowBlockSize = xBufferSize * srcDataTypeSize;
            byte[] dstRowBlockBuffer = new byte[dstRowBlockSize];
            offset += colOffset;
            float rowScale = ySize / (float)yBufferSize;
            float colScale = xSize / (float)xBufferSize;
            int srcRow = 0;
            for (int dstRow = 0; dstRow < yBufferSize; dstRow++, srcRow = (int)(dstRow * rowScale), buffer = IntPtr.Add(buffer, dstRowBlockSize))
            {
                _fileStream.Seek(offset + srcRow * rowSize, SeekOrigin.Begin);
                _fileStream.Read(srcRowBlockBuffer, 0, srcRowBlockSize);
                for (int dstCol = 0; dstCol < xBufferSize; dstCol++)
                {
                    int srcCol = (int)(dstCol * colScale);
                    int dstIdx = dstCol * srcDataTypeSize;
                    int srcIdx = srcCol * srcDataTypeSize;
                    for (int b = 0; b < srcDataTypeSize; b++)
                        dstRowBlockBuffer[dstIdx + b] = srcRowBlockBuffer[srcIdx + b];
                }
                Marshal.Copy(dstRowBlockBuffer, 0, buffer, dstRowBlockSize);
            }
        }

        protected override void DirectWrite(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            if (xSize == xBufferSize && ySize == yBufferSize)
                DirectWriteNormal(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
            else
                DirectWriteResample(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
        }

        private void DirectWriteNormal(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            int srcDataTypeSize = DataTypeHelper.SizeOf(enumDataType.Int16);
            int rowSize = xSize * srcDataTypeSize;
            long offset = _header.HeaderSize + yOffset * rowSize;
            int colOffset = xOffset * srcDataTypeSize;
            offset += colOffset;
            _fileStream.Seek(offset, SeekOrigin.Begin);
            int rowBlockSize = xSize * srcDataTypeSize;
            int rightBank = xSize * srcDataTypeSize - rowBlockSize;
            byte[] rowBlockBuffer = new byte[rowBlockSize];
            for (int row = 0; row < ySize; row++, _fileStream.Seek(rightBank, SeekOrigin.Current), buffer = IntPtr.Add(buffer, rowBlockSize))
            {
                Marshal.Copy(buffer, rowBlockBuffer, 0, rowBlockSize);
                _fileStream.Write(rowBlockBuffer, 0, rowBlockSize);
            }
        }

        private void DirectWriteResample(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            int srcDataTypeSize = DataTypeHelper.SizeOf(enumDataType.Int16);
            int rowSize = xSize * srcDataTypeSize;
            long offset = _header.HeaderSize + yOffset * rowSize;
            int colOffset = xOffset * srcDataTypeSize;
            offset += colOffset;
            _fileStream.Seek(offset, SeekOrigin.Begin);
            int dstRowBlockSize = xSize * srcDataTypeSize;
            int srcRowBlockSize = xBufferSize * srcDataTypeSize;
            int endRow = yOffset + ySize;
            int rightBank = xSize * srcDataTypeSize - dstRowBlockSize;
            byte[] srcRowBlockBuffer = new byte[srcRowBlockSize];
            byte[] dstRowBlockBuffer = new byte[dstRowBlockSize];
            float rowScale = ySize / (float)yBufferSize;
            float colScale = xSize / (float)xBufferSize;
            IntPtr buffer0 = buffer;
            int srcCol = 0;
            int dstIdx = 0;
            int srcIdx = 0;
            for (int dstRow = 0; dstRow < endRow; dstRow++, _fileStream.Seek(rightBank, SeekOrigin.Current))
            {
                int srcRow = (int)(dstRow / rowScale);
                buffer = IntPtr.Add(buffer0, srcRowBlockSize * srcRow);
                Marshal.Copy(buffer, srcRowBlockBuffer, 0, srcRowBlockSize);
                for (int dstCol = 0; dstCol < xSize; dstCol++)
                {
                    srcCol = (int)(dstCol / colScale);
                    dstIdx = dstCol * srcDataTypeSize;
                    srcIdx = srcCol * srcDataTypeSize;
                    for (int b = 0; b < srcDataTypeSize; b++)
                        dstRowBlockBuffer[dstIdx + b] = srcRowBlockBuffer[srcIdx + b];
                }
                _fileStream.Write(dstRowBlockBuffer, 0, dstRowBlockSize);
            }
        }

        public override void Fill(double noDataValue)
        {
            int dataTypeSize = DataTypeHelper.SizeOf(enumDataType.Int16);
            long offset = _header.HeaderSize + _width * _height * dataTypeSize;
            _fileStream.Seek(offset, SeekOrigin.Begin);
            byte[] bytes = BitConverter.GetBytes((Int16)noDataValue);
            byte[] rowBuffer = new byte[_width * dataTypeSize];
            for (int i = 0; i < _width; i++)
                for (int b = 0; b < dataTypeSize; b++)
                    rowBuffer[i * dataTypeSize + b] = bytes[b];
            for (int i = 0; i < _height; i++)
                _binaryWriter.Write(rowBuffer);
        }

        public override void ComputeMinMax(out double min, out double max, bool isCanApprox, Action<int, string> progressCallback)
        {
            int interleave = isCanApprox ? GeoDo.RSS.Core.DF.Constants.DEFAULT_PIXELES_INTERLEAVE : 1;
            MaxMinValuesComputer.ComputeMinMax(this, interleave, out min, out max, isCanApprox, progressCallback);
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

        public override void ComputeStatistics(out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            int interleave = isCanApprox ? GeoDo.RSS.Core.DF.Constants.DEFAULT_PIXELES_INTERLEAVE : 1;
            StatValuesComputer.Stat(this, interleave, out min, out max, out mean, out stddev, isCanApprox, progressCallback);
        }

        public override void ComputeStatistics(double begin, double end, out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            if (begin < double.Epsilon && end < double.Epsilon)
            {
                ComputeStatistics(out min, out max, out mean, out stddev, isCanApprox, progressCallback);
                return;
            }
            int interleave = isCanApprox ? GeoDo.RSS.Core.DF.Constants.DEFAULT_PIXELES_INTERLEAVE : 1;
            StatValuesComputer.Stat(this, interleave, begin, end, out min, out max, out mean, out stddev, isCanApprox, progressCallback);
        }

        public override void ComputeHistogram(double begin, double end, int buckets, int[] histogram, bool isIncludeOutOfRange, bool isCanApprox, Action<int, string> progressCallback)
        {
            int interleave = isCanApprox ? GeoDo.RSS.Core.DF.Constants.DEFAULT_PIXELES_INTERLEAVE : 1;
            StatValuesComputer.ComputeHistogram(this, buckets, interleave, begin, end, histogram, isCanApprox, progressCallback);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
