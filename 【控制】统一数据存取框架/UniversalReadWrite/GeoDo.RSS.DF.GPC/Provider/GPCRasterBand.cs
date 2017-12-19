using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;

namespace GeoDo.RSS.DF.GPC
{
    public class GPCRasterBand : RasterBand, IBandOperator
    {
        /// <summary>
        /// 等经纬度网格最多纬度分隔，72，2.5度
        /// </summary>
        private const byte MAXLAT = 72;
        /// <summary>
        /// 等经纬度网格最多经度分隔，144，2.5度
        /// </summary>
        private const byte MAXLON = 144;
        private const float DLAT = 2.5f;
        private float[] _bufferData = null;

        public GPCRasterBand(IRasterDataProvider rasterDataProvider, int bandNo)
            : base(rasterDataProvider)
        {
            _rasterDataProvider = rasterDataProvider;
            _bandNo = bandNo;
            //if (_args = "GLL") 
            //以下参数为读取为等经纬度时候匹配的参数
            _noDataValue = 255;
            _width = 144;
            _height = 72;
            _resolutionX = 2.5f;
            _resolutionY = 2.5f;
            _coordEnvelope = new CoordEnvelope(-180d, 180d, -90d, 90d);
            _spatialRef = SpatialReference.GetDefault();
            _dataType = enumDataType.Float;//这个根据实际
            _dataTypeSize = DataTypeHelper.SizeOf(_dataType);
        }

        #region 从数据提供者(数据集)中读取指定位置的数据块
        protected override void DirectRead(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            int length = MAXLAT * MAXLON;
            if (_bufferData == null)
            {
                _bufferData = new float[length];
                GCPRow[] gpcRows = (_rasterDataProvider as GPCRasterDataProvider).GCPRows;
                int[] cellIndexs = (_rasterDataProvider as GPCRasterDataProvider).CellIndexLut;
                int[] rowIndexs = (_rasterDataProvider as GPCRasterDataProvider).RowIndexLut;
                if (cellIndexs == null || rowIndexs == null)
                    return;
                if (dataType == enumDataType.Float)//目前全部返回float类型数值
                {
                    for (int i = 0; i < MAXLAT; i++)
                    {
                        for (int j = 0; j < MAXLON; j++)
                        {
                            int index = i * MAXLON + j;
                            int row = rowIndexs[index];
                            int cell = cellIndexs[index];
                            if (row < 67 && cell < 99)
                                _bufferData[index] = D2Data2Phys.Data2Phys(gpcRows[row].GridCell[cell][_bandNo - 1], _bandNo);//这里取到的是索引值，后面还需要转换为物理量
                        }
                    }
                }
            }
            if (xSize == xBufferSize && ySize == yBufferSize)
            {
                if (xOffset == 0 && yOffset == 0 && xSize == MAXLON && ySize == MAXLAT)
                    ReadNormalAll(buffer, length);
                else
                    DirectReadBSQNormal(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
            }
            else
            {
                DirectReadBSQSample(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
            }
        }
        
        private void ReadNormalAll(IntPtr buffer, int length)
        {
            GCHandle handle = GCHandle.Alloc(_bufferData);
            try
            {
                buffer = IntPtr.Add(buffer, 0);
                Marshal.Copy(_bufferData, 0, buffer, length);
            }
            finally
            {
                handle.Free();
            }
        }

        private void DirectReadBSQSample(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            throw new NotImplementedException();
        }

        private void DirectReadBSQNormal(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            int srcDataTypeSize = DataTypeHelper.SizeOf(_dataType);
            int bufferRowBlockSize = srcDataTypeSize * xSize;
            int offset = yOffset * _width;//起始行偏移量
            int endRow = yOffset + ySize;
            int colOffset = xOffset;
            int rowBlockSize = xSize;
            //int rightBank = _width - rowBlockSize;
            int curOffset = offset + colOffset;
            for (int row = yOffset; row < endRow; row++, buffer = IntPtr.Add(buffer, bufferRowBlockSize))
            {
                Marshal.Copy(_bufferData, curOffset, buffer, rowBlockSize);
                curOffset += _width;
            }
        }

        #endregion

        protected override void DirectWrite(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            throw new NotImplementedException();
        }

        public override void Fill(double noDataValue)
        {
            throw new NotImplementedException();
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
            if (_bufferData != null)
                _bufferData = null;
            base.Dispose();
        }
    }
}
