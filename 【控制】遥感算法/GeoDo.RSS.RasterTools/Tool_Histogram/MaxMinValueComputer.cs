using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.Drawing;

namespace GeoDo.RSS.RasterTools
{
    public static class MaxMinValueComputerFactory
    {
        public static IMaxMinValueComputer GetMaxMinValueComputer(enumDataType dataType)
        {
            switch (dataType)
            {
                case enumDataType.Byte:
                    return new MaxMinValueComputerByte();
                case enumDataType.UInt16:
                    return new MaxMinValueComputerUInt16();
                case enumDataType.Int16:
                    return new MaxMinValueComputerInt16();
                case enumDataType.UInt32:
                    return new MaxMinValueComputerUInt32();
                case enumDataType.Int32:
                    return new MaxMinValueComputerInt32();
                case enumDataType.UInt64:
                    return new MaxMinValueComputerUInt64();
                case enumDataType.Int64:
                    return new MaxMinValueComputerInt64();
                case enumDataType.Float:
                    return new MaxMinValueComputerFloat();
                case enumDataType.Double:
                    return new MaxMinValueComputerDouble();
                default:
                    return null;
            }
        }
    }

    public interface IMaxMinValueComputer
    {
        void Compute(IRasterBand[] srcRasters, int[] aoi, out double[] minValues, out double[] maxValues, out double[] meanValues, Action<int, string> progressTracker);
        void Compute(IRasterBand[] srcRasters, int[] aoi, double[][] invalidValues,out double[] minValues, out double[] maxValues, out double[] meanValues, Action<int, string> progressTracker);
        void DirectCompute(int bandCount, object srcBuffers, double[] minValues, double[] maxValues, double[] meanValues);
        void DirectCompute(int bandCount, object srcBuffers, double[][] invalidValues, double[] minValues, double[] maxValues, double[] meanValues);
    }

    public abstract class MaxMinValueComputer<T> : IMaxMinValueComputer
    {
        private const int MAX_BLOCK_SIZE = 1024 * 1024 * 10;//10MB

        public void DirectCompute(int bandCount, object srcBuffers, double[] minValues, double[] maxValues, double[] meanValues)
        {
        }
        public void DirectCompute(int bandCount, object srcBuffers, double[][] invalidValues, double[] minValues, double[] maxValues, double[] meanValues)
        {
        }


        public void Compute(IRasterBand[] srcRasters, int[] aoi, out double[] minValues, out double[] maxValues, out double[] meanValues, Action<int, string> progressTracker)
        {
            if (aoi == null || aoi.Length == 0)
                FullCompute(srcRasters, out minValues, out maxValues, out meanValues, progressTracker);
            else
                ComputeByAOI(srcRasters, aoi, out minValues, out maxValues, out meanValues, progressTracker);
        }

        private void ComputeByAOI(IRasterBand[] srcRasters, int[] aoi, out double[] minValues, out double[] maxValues, out double[] meanValues, Action<int, string> progressTracker)
        {
            enumDataType dataType = srcRasters[0].DataType;
            int width = srcRasters[0].Width;
            int height = srcRasters[0].Height;
            //
            int bandCount = srcRasters.Length;
            minValues = new double[bandCount];
            maxValues = new double[bandCount];
            meanValues = new double[bandCount];
            for (int i = 0; i < bandCount; i++)
            {
                minValues[i] = double.MaxValue;
                maxValues[i] = double.MinValue;
            }
            //
            Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, new Size(width, height));
            SortedList<int, AOIHelper.ColRangeOfRow> aoiRowRanges = AOIHelper.ComputeColRanges(aoi, new Size(width, height));
            int rowCount = aoiRect.Height;
            int bRow = aoiRect.Top;
            int eRow = aoiRect.Bottom;
            int bCol = 0, eCol = 0;
            T[][] srcBuffers = new T[bandCount][];
            GCHandle[] srcHandles = new GCHandle[bandCount];
            for (int i = 0; i < bandCount; i++)
            {
                srcBuffers[i] = new T[width];
                srcHandles[i] = GCHandle.Alloc(srcBuffers[i], GCHandleType.Pinned);
            }
            //
            try
            {
                int aoiPixelCount = 0;
                AOIHelper.ColRangeOfRow range = null;
                for (int r = bRow; r < eRow; r++)
                {
                    try
                    {
                        //读取一行数据
                        for (int b = 0; b < bandCount; b++)
                            srcRasters[b].Read(0, r, width, 1, srcHandles[b].AddrOfPinnedObject(), dataType, width, 1);
                        //获取该行AOI的起始列
                        if (aoiRowRanges.ContainsKey(r))
                        {
                            range = aoiRowRanges[r];
                            //调用计算委托
                            bCol = range.BeginCol;
                            eCol = range.EndCol + 1;
                            HandleBlock(bandCount, srcBuffers, bCol, eCol, minValues, maxValues, meanValues);
                            aoiPixelCount += (eCol - bCol);
                            if (progressTracker != null)
                                progressTracker(100 * (r - bRow + 1) / rowCount, string.Empty);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                }
                HandleMeanValues(bandCount, meanValues, aoiPixelCount);
            }
            finally
            {
                for (int i = 0; i < bandCount; i++)
                    srcHandles[i].Free();
            }
        }

        private void FullCompute(IRasterBand[] srcRasters, out double[] minValues, out double[] maxValues, out double[] meanValues, Action<int, string> progressTracker)
        {
            int bandCount = srcRasters.Length;
            minValues = new double[bandCount];
            maxValues = new double[bandCount];
            meanValues = new double[bandCount];
            for (int i = 0; i < bandCount; i++)
            {
                minValues[i] = double.MaxValue;
                maxValues[i] = double.MinValue;
            }
            //计算每块包含的行数
            int rowsOfBlock = 0;
            int rowSize = srcRasters[0].Width * DataTypeHelper.SizeOf(srcRasters[0].DataType);
            if (rowSize >= MAX_BLOCK_SIZE)
                rowsOfBlock = 1;
            else
                rowsOfBlock = MAX_BLOCK_SIZE / rowSize;
            //计算总块数
            int countBlocks = (int)Math.Ceiling((float)srcRasters[0].Height / rowsOfBlock); //总块数
            if (countBlocks == 0)
                countBlocks = 1;
            if (countBlocks == 1)
                rowsOfBlock = srcRasters[0].Height;
            //创建参与计算的波段缓存区(缓存区大小==块大小)
            int width = srcRasters[0].Width;
            int height = srcRasters[0].Height;
            int srcCount = srcRasters.Length;
            GCHandle[] srcHandles = new GCHandle[srcCount];
            T[][] srcBuffers = new T[srcCount][];
            for (int i = 0; i < srcCount; i++)
            {
                srcBuffers[i] = new T[rowsOfBlock * width];
                srcHandles[i] = GCHandle.Alloc(srcBuffers[i], GCHandleType.Pinned);
            }
            //
            int bRow = 0;//begin row
            int eRow = 0;//end row
            int actualCount = 0;
            int bufferRowCount = 0;
            float percent = countBlocks / 100f;
            int ipercent = 0;
            for (int i = 0; i < countBlocks; i++, bRow += rowsOfBlock)
            {
                //确保最后一块的结束行正确
                eRow = Math.Min(height, bRow + rowsOfBlock);
                bufferRowCount = eRow - bRow;
                //从参与计算的波段iSrc中读取第i块数据
                for (int iSrc = 0; iSrc < srcCount; iSrc++)
                    srcRasters[iSrc].Read(0, bRow, width, bufferRowCount, srcHandles[iSrc].AddrOfPinnedObject(), srcRasters[iSrc].DataType, width, bufferRowCount);
                //执行波段计算并将计算结果写到目标波段块缓存区
                actualCount = bufferRowCount * width;
                //计算波段的最大值，最小值，
                HandleBlock(bandCount, srcBuffers, 0, actualCount, minValues, maxValues, meanValues);
                //打印进度
                if (progressTracker != null)
                {
                    ipercent = (int)(i / percent);
                    progressTracker(ipercent, string.Empty);
                }
            }
            //计算波段的均值
            HandleMeanValues(bandCount, meanValues, srcRasters[0].Width * srcRasters[0].Height);
            //
            if (ipercent < 100)
                if (progressTracker != null)
                    progressTracker(100, string.Empty);
            //释放缓存区
            for (int i = 0; i < srcCount; i++)
                srcHandles[i].Free();
        }

        public void Compute(IRasterBand[] srcRasters, int[] aoi, double[][] invalidValues, out double[] minValues, out double[] maxValues, out double[] meanValues, Action<int, string> progressTracker)
        {
            if (aoi == null || aoi.Length == 0)
                FullCompute(srcRasters, invalidValues, out minValues, out maxValues, out meanValues, progressTracker);
            else
                ComputeByAOI(srcRasters, aoi,invalidValues ,out minValues, out maxValues, out meanValues, progressTracker);
        }

        private void ComputeByAOI(IRasterBand[] srcRasters, int[] aoi, double[][] invalidValues, out double[] minValues, out double[] maxValues, out double[] meanValues, Action<int, string> progressTracker)
        {
            enumDataType dataType = srcRasters[0].DataType;
            int width = srcRasters[0].Width;
            int height = srcRasters[0].Height;
            //
            int bandCount = srcRasters.Length;
            minValues = new double[bandCount];
            maxValues = new double[bandCount];
            meanValues = new double[bandCount];
            for (int i = 0; i < bandCount; i++)
            {
                minValues[i] = double.MaxValue;
                maxValues[i] = double.MinValue;
            }
            //
            Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, new Size(width, height));
            SortedList<int, AOIHelper.ColRangeOfRow> aoiRowRanges = AOIHelper.ComputeColRanges(aoi, new Size(width, height));
            int rowCount = aoiRect.Height;
            int bRow = aoiRect.Top;
            int eRow = aoiRect.Bottom;
            int bCol = 0, eCol = 0;
            T[][] srcBuffers = new T[bandCount][];
            GCHandle[] srcHandles = new GCHandle[bandCount];
            for (int i = 0; i < bandCount; i++)
            {
                srcBuffers[i] = new T[width];
                srcHandles[i] = GCHandle.Alloc(srcBuffers[i], GCHandleType.Pinned);
            }
            //
            try
            {
                //int aoiPixelCount = 0;
                int[] valueCountOfBlock;
                int[] aoiPixelCount = new int[bandCount];
                AOIHelper.ColRangeOfRow range =null;
                for (int r = bRow; r < eRow; r++)
                {
                    try
                    {
                        //读取一行数据
                        for (int b = 0; b < bandCount; b++)
                            srcRasters[b].Read(0, r, width, 1, srcHandles[b].AddrOfPinnedObject(), dataType, width, 1);
                        //获取该行AOI的起始列
                        if (aoiRowRanges.ContainsKey(r))
                        {
                            range = aoiRowRanges[r];
                            //调用计算委托
                            bCol = range.BeginCol;
                            eCol = range.EndCol + 1;
                            HandleBlock(bandCount, srcBuffers, invalidValues, bCol, eCol, minValues, maxValues, meanValues, out valueCountOfBlock);
                            for (int b = 0; b < bandCount; b++)
                                aoiPixelCount[b] += valueCountOfBlock[b];
                            if (progressTracker != null)
                                progressTracker(100 * (r - bRow + 1) / rowCount, string.Empty);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        throw ex;
                    }
                }
                HandleMeanValues(bandCount, meanValues, aoiPixelCount);
            }
            finally
            {
                for (int i = 0; i < bandCount; i++)
                    srcHandles[i].Free();
            }
        }

        private void FullCompute(IRasterBand[] srcRasters, double[][] invalidValues, out double[] minValues, out double[] maxValues, out double[] meanValues, Action<int, string> progressTracker)
        {
            int bandCount = srcRasters.Length;
            minValues = new double[bandCount];
            maxValues = new double[bandCount];
            meanValues = new double[bandCount];
            for (int i = 0; i < bandCount; i++)
            {
                minValues[i] = double.MaxValue;
                maxValues[i] = double.MinValue;
            }
            //计算每块包含的行数
            int rowsOfBlock = 0;
            int rowSize = srcRasters[0].Width * DataTypeHelper.SizeOf(srcRasters[0].DataType);
            if (rowSize >= MAX_BLOCK_SIZE)
                rowsOfBlock = 1;
            else
                rowsOfBlock = MAX_BLOCK_SIZE / rowSize;
            //计算总块数
            int countBlocks = (int)Math.Ceiling((float)srcRasters[0].Height / rowsOfBlock); //总块数
            if (countBlocks == 0)
                countBlocks = 1;
            if (countBlocks == 1)
                rowsOfBlock = srcRasters[0].Height;
            //创建参与计算的波段缓存区(缓存区大小==块大小)
            int width = srcRasters[0].Width;
            int height = srcRasters[0].Height;
            int srcCount = srcRasters.Length;
            GCHandle[] srcHandles = new GCHandle[srcCount];
            T[][] srcBuffers = new T[srcCount][];
            for (int i = 0; i < srcCount; i++)
            {
                srcBuffers[i] = new T[rowsOfBlock * width];
                srcHandles[i] = GCHandle.Alloc(srcBuffers[i], GCHandleType.Pinned);
            }
            //
            int bRow = 0;//begin row
            int eRow = 0;//end row
            int actualCount = 0;
            int bufferRowCount = 0;
            float percent = countBlocks / 100f;
            int ipercent = 0;
            int[] valueCountOfBlock;
            int[] valueCount = new int[bandCount];
            for (int i = 0; i < countBlocks; i++, bRow += rowsOfBlock)
            {
                //确保最后一块的结束行正确
                eRow = Math.Min(height, bRow + rowsOfBlock);
                bufferRowCount = eRow - bRow;
                //从参与计算的波段iSrc中读取第i块数据
                for (int iSrc = 0; iSrc < srcCount; iSrc++)
                    srcRasters[iSrc].Read(0, bRow, width, bufferRowCount, srcHandles[iSrc].AddrOfPinnedObject(), srcRasters[iSrc].DataType, width, bufferRowCount);
                //执行波段计算并将计算结果写到目标波段块缓存区
                actualCount = bufferRowCount * width;
                //计算波段的最大值，最小值，
                HandleBlock(bandCount, srcBuffers,invalidValues, 0, actualCount, minValues, maxValues, meanValues,out valueCountOfBlock);
                for (int b = 0; b < bandCount; b++)
                    valueCount[b] += valueCountOfBlock[b];
                //打印进度
                if (progressTracker != null)
                {
                    ipercent = (int)(i / percent);
                    progressTracker(ipercent, string.Empty);
                }
            }
            //计算波段的均值
            HandleMeanValues(bandCount, meanValues, valueCount);
            //
            if (ipercent < 100)
                if (progressTracker != null)
                    progressTracker(100, string.Empty);
            //释放缓存区
            for (int i = 0; i < srcCount; i++)
                srcHandles[i].Free();
        }

        private void HandleMeanValues(int bandCount, double[] meanValues, int[] pixelCount)
        {
            for (int b = 0; b < bandCount; b++)
                meanValues[b] = meanValues[b] / pixelCount[b];
        }

        private void HandleMeanValues(int bandCount, double[] meanValues, int pixelCount)
        {
            for (int b = 0; b < bandCount; b++)
                meanValues[b] = meanValues[b] / pixelCount;
        }

        protected abstract void HandleBlock(int bandCount, T[][] srcBuffers, int offset, int acutalCount, double[] minValues, double[] maxValues, double[] meanValues);

        protected abstract void HandleBlock(int bandCount, T[][] srcBuffers, double[][] invalidValues,int offset, int acutalCount, double[] minValues, double[] maxValues, double[] meanValues,out int[] valueCountOfBlock);

        public void DirectCompute(int bandCount, T[][] srcBuffers, double[] minValues, double[] maxValues, double[] meanValues)
        {
            minValues = new double[bandCount];
            maxValues = new double[bandCount];
            meanValues = new double[bandCount];
            for (int i = 0; i < bandCount; i++)
            {
                minValues[i] = double.MaxValue;
                maxValues[i] = double.MinValue;
            }
            int actualCount = srcBuffers[0].Length;
            //计算波段的最大值，最小值，
            HandleBlock(bandCount, srcBuffers, 0, actualCount, minValues, maxValues, meanValues);
            //计算波段的均值
            HandleMeanValues(bandCount, meanValues, actualCount);
        }

        public void DirectCompute(int bandCount, T[][] srcBuffers,double[][] invalidValues, double[] minValues, double[] maxValues, double[] meanValues)
        {
            minValues = new double[bandCount];
            maxValues = new double[bandCount];
            meanValues = new double[bandCount];
            for (int i = 0; i < bandCount; i++)
            {
                minValues[i] = double.MaxValue;
                maxValues[i] = double.MinValue;
            }
            int actualCount = srcBuffers[0].Length;
            int[] valueCount;
            //计算波段的最大值，最小值，
            HandleBlock(bandCount, srcBuffers, invalidValues, 0, actualCount, minValues, maxValues, meanValues, out valueCount);
            //计算波段的均值
            HandleMeanValues(bandCount, meanValues, valueCount);
        }
    }

    public class MaxMinValueComputerByte : MaxMinValueComputer<byte>
    {
        protected override void HandleBlock(int bandCount, byte[][] srcBuffers, int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues)
        {
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                }
            }
        }

        protected override void HandleBlock(int bandCount, byte[][] srcBuffers, double[][] invalidValues,int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues,out int[] valueCountOfBlock)
        {
            valueCountOfBlock = new int[bandCount];
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (IsInvalidValue(srcBuffers[b][i], invalidValues[b]))
                        continue;
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                    valueCountOfBlock[b]++;
                }
            }
        }

        private bool IsInvalidValue(byte value,double[] invalidValues)
        {
            if (invalidValues == null || invalidValues.Length < 1)
                return false;
            for (int i = 0; i < invalidValues.Length; i++)
            {
                if (value == (byte)invalidValues[i])
                    return true;
            }
            return false;
        }
    }

    public class MaxMinValueComputerUInt16 : MaxMinValueComputer<UInt16>
    {
        protected override void HandleBlock(int bandCount, UInt16[][] srcBuffers, int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues)
        {
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                }
            }
        }

        protected override void HandleBlock(int bandCount, UInt16[][] srcBuffers, double[][] invalidValues, int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues, out int[] validValueCount)
        {
            validValueCount = new int[bandCount];
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (IsInvalidValue(srcBuffers[b][i], invalidValues[b]))
                        continue;
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                    validValueCount[b]++;
                }
            }
        }

        private bool IsInvalidValue(UInt16 value, double[] invalidValues)
        {
            if (invalidValues == null || invalidValues.Length < 1)
                return false;
            for (int i = 0; i < invalidValues.Length; i++)
            {
                if (value == (UInt16)invalidValues[i])
                    return true;
            }
            return false;
        }
    }

    public class MaxMinValueComputerInt16 : MaxMinValueComputer<Int16>
    {
        protected override void HandleBlock(int bandCount, Int16[][] srcBuffers, int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues)
        {
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                }
            }
        }

        protected override void HandleBlock(int bandCount, short[][] srcBuffers, double[][] invalidValues, int offset, int acutalCount, double[] minValues, double[] maxValues, double[] meanValues, out int[] validValueCount)
        {
            validValueCount = new int[bandCount];
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < acutalCount; i++)
                {
                    if (IsInvalidValue(srcBuffers[b][i], invalidValues[b]))
                        continue;
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                    validValueCount[b]++;
                }
            }
        }

        private bool IsInvalidValue(Int16 value, double[] invalidValues)
        {
            if (invalidValues == null || invalidValues.Length < 1)
                return false;
            for (int i = 0; i < invalidValues.Length; i++)
            {
                if (value == (Int16)invalidValues[i])
                    return true;
            }
            return false;
        }
    }

    public class MaxMinValueComputerUInt32 : MaxMinValueComputer<UInt32>
    {
        protected override void HandleBlock(int bandCount, UInt32[][] srcBuffers, int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues)
        {
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                }
            }
        }

        protected override void HandleBlock(int bandCount, uint[][] srcBuffers, double[][] invalidValues, int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues, out int[] validValueCount)
        {
            validValueCount = new int[bandCount];
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (IsInvalidValue(srcBuffers[b][i], invalidValues[b]))
                        continue;
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                    validValueCount[b]++;
                }
            }
        }

        private bool IsInvalidValue(UInt32 value, double[] invalidValues)
        {
            if (invalidValues == null || invalidValues.Length < 1)
                return false;
            for (int i = 0; i < invalidValues.Length; i++)
            {
                if (value == (UInt32)invalidValues[i])
                    return true;
            }
            return false;
        }
    }

    public class MaxMinValueComputerInt32 : MaxMinValueComputer<Int32>
    {
        protected override void HandleBlock(int bandCount, Int32[][] srcBuffers, int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues)
        {
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                }
            }
        }

        protected override void HandleBlock(int bandCount, int[][] srcBuffers, double[][] invalidValues, int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues, out int[] validValueCount)
        {
            validValueCount = new int[bandCount];
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (IsInvalidValue(srcBuffers[b][i], invalidValues[b]))
                        continue;
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                    validValueCount[b]++;
                }
            }
        }

        private bool IsInvalidValue(Int32 value, double[] invalidValues)
        {
            if (invalidValues == null || invalidValues.Length < 1)
                return false;
            for (int i = 0; i < invalidValues.Length; i++)
            {
                if (value == (Int32)invalidValues[i])
                    return true;
            }
            return false;
        }
    }

    public class MaxMinValueComputerUInt64 : MaxMinValueComputer<UInt64>
    {
        protected override void HandleBlock(int bandCount, UInt64[][] srcBuffers, int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues)
        {
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                }
            }
        }

        protected override void HandleBlock(int bandCount, ulong[][] srcBuffers, double[][] invalidValues, int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues, out int[] validValueCount)
        {
            validValueCount = new int[bandCount];
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (IsInvalidValue(srcBuffers[b][i], invalidValues[b]))
                        continue;
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                    validValueCount[b]++;
                }
            }
        }

        private bool IsInvalidValue(UInt64 value, double[] invalidValues)
        {
            if (invalidValues == null || invalidValues.Length < 1)
                return false;
            for (int i = 0; i < invalidValues.Length; i++)
            {
                if (value == (UInt64)invalidValues[i])
                    return true;
            }
            return false;
        }
    }

    public class MaxMinValueComputerInt64 : MaxMinValueComputer<Int64>
    {
        protected override void HandleBlock(int bandCount, Int64[][] srcBuffers, int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues)
        {
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                }
            }
        }

        protected override void HandleBlock(int bandCount, long[][] srcBuffers, double[][] invalidValues, int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues, out int[] validValueCount)
        {
            validValueCount = new int[bandCount];
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (IsInvalidValue(srcBuffers[b][i], invalidValues[b]))
                        continue;
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                    validValueCount[b]++;
                }
            }
        }

        private bool IsInvalidValue(Int64 value, double[] invalidValues)
        {
            if (invalidValues == null || invalidValues.Length < 1)
                return false;
            for (int i = 0; i < invalidValues.Length; i++)
            {
                if (value == (Int64)invalidValues[i])
                    return true;
            }
            return false;
        }
    }

    public class MaxMinValueComputerFloat : MaxMinValueComputer<float>
    {
        protected override void HandleBlock(int bandCount, float[][] srcBuffers, int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues)
        {
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                }
            }
        }

        protected override void HandleBlock(int bandCount, float[][] srcBuffers, double[][] invalidValues, int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues, out int[] validValueCount)
        {
            validValueCount = new int[bandCount];
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (IsInvalidValue(srcBuffers[b][i], invalidValues[b]))
                        continue;
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                    validValueCount[b]++;
                }
            }
        }

        private bool IsInvalidValue(float value, double[] invalidValues)
        {
            if (invalidValues == null || invalidValues.Length < 1)
                return false;
            for (int i = 0; i < invalidValues.Length; i++)
            {
                if (value == (float)invalidValues[i])
                    return true;
            }
            return false;
        }
    }

    public class MaxMinValueComputerDouble : MaxMinValueComputer<double>
    {
        protected override void HandleBlock(int bandCount, double[][] srcBuffers, int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues)
        {
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                }
            }
        }

        protected override void HandleBlock(int bandCount, double[][] srcBuffers, double[][] invalidValues, int offset, int actualCount, double[] minValues, double[] maxValues, double[] meanValues, out int[] validValueCount)
        {
            validValueCount = new int[bandCount];
            for (int b = 0; b < bandCount; b++)
            {
                for (int i = offset; i < actualCount; i++)
                {
                    if (IsInvalidValue(srcBuffers[b][i], invalidValues[b]))
                        continue;
                    if (srcBuffers[b][i] < minValues[b])
                        minValues[b] = srcBuffers[b][i];
                    if (srcBuffers[b][i] > maxValues[b])
                        maxValues[b] = srcBuffers[b][i];
                    meanValues[b] += srcBuffers[b][i];
                    validValueCount[b]++;
                }
            }
        }

        private bool IsInvalidValue(double value, double[] invalidValues)
        {
            if (invalidValues == null || invalidValues.Length < 1)
                return false;
            for (int i = 0; i < invalidValues.Length; i++)
            {
                if (value == invalidValues[i])
                    return true;
            }
            return false;
        }
    }
}
