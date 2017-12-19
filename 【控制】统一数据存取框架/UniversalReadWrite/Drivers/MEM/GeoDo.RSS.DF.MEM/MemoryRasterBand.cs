using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO.MemoryMappedFiles;
using System.IO;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.DF.MEM
{
    public class MemoryRasterBand : RasterBand, IMemoryRasterBand, IBandOperator
    {
        protected MemoryMappedFile _mmf;
        protected long _baseOffset;
        protected long _bandSize;
        protected MemoryMappedViewAccessor _accessor;
        protected int _blockSize;
        protected int _mappedBegin;

        public MemoryRasterBand(IRasterDataProvider prd, MemoryMappedFileAccess access, MemoryMappedFile mmf, int bandNo)
            : base(prd)
        {
            _rasterDataProvider = prd;
            _mmf = mmf;
            _bandNo = bandNo;
            _width = prd.Width;
            _height = prd.Height;
            _dataType = prd.DataType;
            _dataTypeSize = DataTypeHelper.SizeOf(_dataType);
            _coordEnvelope = prd.CoordEnvelope;
            _coordTransofrm = prd.CoordTransform;
            _resolutionX = prd.ResolutionX;
            _resolutionY = prd.ResolutionY;
            _spatialRef = prd.SpatialRef;
            _bandSize = (long)_width * (long)_height * (long)_dataTypeSize;
            _baseOffset = (long)(prd as IMemoryRasterDataProvider).HeaderSize + (long)(bandNo - 1) * (long)_bandSize;
            TryCreateMmfAccessor(_baseOffset, access);
        }

        private void TryCreateMmfAccessor(long offset, MemoryMappedFileAccess access)
        {
            _accessor = _mmf.CreateViewAccessor(offset, _bandSize, access);
        }

        protected override void DirectRead(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            if (xSize == xBufferSize && ySize == yBufferSize)
                DirectReadBSQNormal(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
            else
                DirectReadBSQSample(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
        }

        private void DirectReadBSQNormal(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            IVirtualScan0 virtualScan0 = _rasterDataProvider as IVirtualScan0;
            if (virtualScan0.IsVirtualScan0)
            {
                xOffset += virtualScan0.OffsetX;
                yOffset += virtualScan0.OffsetY;
            }
            int srcDataTypeSize = DataTypeHelper.SizeOf(_dataType);
            int rowSize = _width * srcDataTypeSize;
            int offset = yOffset * rowSize;
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
                _accessor.ReadArray<byte>(offset + srcRow * rowSize, srcRowBlockBuffer, 0, srcRowBlockSize);
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

        private void DirectReadBSQSample(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            int srcDataTypeSize = DataTypeHelper.SizeOf(_dataType);
            int rowSize = _width * srcDataTypeSize;
            long offset = (long)(_bandNo - 1) * _width * _height * srcDataTypeSize + yOffset * rowSize;
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
                //_fileStream.Seek(offset + srcRow * rowSize, SeekOrigin.Begin);
                //_fileStream.Read(srcRowBlockBuffer, 0, srcRowBlockSize);
                _accessor.ReadArray<byte>(offset + srcRow * rowSize, srcRowBlockBuffer, 0, srcRowBlockSize);
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
                DirectWriteBSQNormal(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
            else
                DirectWriteBSQResample(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
        }

        private void DirectWriteBSQNormal(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            IVirtualScan0 virtualScan0 = _rasterDataProvider as IVirtualScan0;
            if (virtualScan0.IsVirtualScan0)
            {
                xOffset += virtualScan0.OffsetX;
                yOffset += virtualScan0.OffsetY;
            }
            if (xOffset == 0 && xSize == _width)
            {
                DirectWriteBSQNormalQuickly(yOffset, ySize, buffer, dataType);
            }
            else
            {
                int srcDataTypeSize = DataTypeHelper.SizeOf(_dataType);
                int rowSize = _width * srcDataTypeSize;
                long offset = yOffset * rowSize + xOffset * srcDataTypeSize;
                int rowBlockSize = xSize * srcDataTypeSize;
                int rightBank = _width * srcDataTypeSize - rowBlockSize;
                byte[] rowBlockBuffer = new byte[rowBlockSize];
                for (int row = 0; row < ySize; row++,
                    offset = offset + rowBlockSize + rightBank,
                    buffer = IntPtr.Add(buffer, rowBlockSize))
                {
                    Marshal.Copy(buffer, rowBlockBuffer, 0, rowBlockSize);
                    _accessor.WriteArray<byte>(offset, rowBlockBuffer, 0, rowBlockSize);
                }
            }
        }

        private void DirectWriteBSQNormalQuickly(int yOffset, int ySize, IntPtr buffer, enumDataType dataType)
        {
            int srcDataTypeSize = DataTypeHelper.SizeOf(_dataType);
            int rowSize = _width * srcDataTypeSize;
            long offset = (long)(_bandNo - 1) * _width * _height * srcDataTypeSize + yOffset * rowSize;
            //_fileStream.Seek(offset, SeekOrigin.Begin);
            int blockSize = _width * srcDataTypeSize * ySize;
            byte[] blockBuffer = new byte[blockSize];
            Marshal.Copy(buffer, blockBuffer, 0, blockSize);
            //_fileStream.Write(blockBuffer, 0, blockSize);
            _accessor.WriteArray<byte>(offset, blockBuffer, 0, blockSize);
        }

        private void DirectWriteBSQResample(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            int srcDataTypeSize = DataTypeHelper.SizeOf(_dataType);
            int rowSize = _width * srcDataTypeSize;
            long offset = (long)(_bandNo - 1) * _width * _height * srcDataTypeSize + yOffset * rowSize;
            int colOffset = xOffset * srcDataTypeSize;
            offset += colOffset;
            //_fileStream.Seek(offset, SeekOrigin.Begin);
            int dstRowBlockSize = xSize * srcDataTypeSize;
            int srcRowBlockSize = xBufferSize * srcDataTypeSize;
            int rightBank = _width * srcDataTypeSize - dstRowBlockSize;
            byte[] srcRowBlockBuffer = new byte[srcRowBlockSize];
            byte[] dstRowBlockBuffer = new byte[dstRowBlockSize];
            float rowScale = ySize / (float)yBufferSize;
            float colScale = xSize / (float)xBufferSize;
            IntPtr buffer0 = buffer;
            int srcCol = 0;
            int dstIdx = 0;
            int srcIdx = 0;
            for (int dstRow = 0; dstRow < ySize; dstRow++/*, _fileStream.Seek(rightBank, SeekOrigin.Current)*/)
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
                //_fileStream.Write(dstRowBlockBuffer, 0, dstRowBlockSize);
                _accessor.WriteArray<byte>(offset + dstRow * rightBank, dstRowBlockBuffer, 0, dstRowBlockSize);
            }
        }

        public override void Fill(double noDataValue)
        {
            byte[] bytes = null;
            switch (_dataType)
            {
                case enumDataType.Byte:
                    bytes = BitConverter.GetBytes((byte)noDataValue);
                    break;
                case enumDataType.UInt16:
                    bytes = BitConverter.GetBytes((UInt16)noDataValue);
                    break;
                case enumDataType.Int16:
                    bytes = BitConverter.GetBytes((Int16)noDataValue);
                    break;
                case enumDataType.UInt32:
                    bytes = BitConverter.GetBytes((UInt32)noDataValue);
                    break;
                case enumDataType.Int32:
                    bytes = BitConverter.GetBytes((Int32)noDataValue);
                    break;
                case enumDataType.UInt64:
                    bytes = BitConverter.GetBytes((UInt64)noDataValue);
                    break;
                case enumDataType.Int64:
                    bytes = BitConverter.GetBytes((Int64)noDataValue);
                    break;
                case enumDataType.Float:
                    bytes = BitConverter.GetBytes((float)noDataValue);
                    break;
                case enumDataType.Double:
                    bytes = BitConverter.GetBytes((double)noDataValue);
                    break;
            }
            byte[] rowBuffer = new byte[_width * _dataTypeSize];
            for (int i = 0; i < _width; i++)
                for (int b = 0; b < _dataTypeSize; b++)
                    rowBuffer[i * _dataTypeSize + b] = bytes[b];
            for (int i = 0; i < _height; i++)
            {
                _accessor.WriteArray<byte>(i * rowBuffer.Length, rowBuffer, 0, rowBuffer.Length);
            }

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
            if (_accessor != null)
            {
                try
                {
                    _accessor.Dispose();
                }
                catch
                {
                }
                _accessor = null;
            }
            _mmf = null;
            base.Dispose();
        }
    }
}
