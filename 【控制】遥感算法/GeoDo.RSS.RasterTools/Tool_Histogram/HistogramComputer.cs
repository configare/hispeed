using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.Drawing;

namespace GeoDo.RSS.RasterTools
{
    public static class HistogramComputerFactory
    {
        public static IHistogramComputer GetHistogramComputer(enumDataType dataType)
        {
            switch (dataType)
            {
                case enumDataType.Byte:
                    return new HistogramComputerByte();
                case enumDataType.UInt16:
                    return new HistogramComputerUInt16();
                case enumDataType.Int16:
                    return new HistogramComputerInt16();
                case enumDataType.UInt32:
                    return new HistogramComputerUInt32();
                case enumDataType.Int32:
                    return new HistogramComputerInt32();
                case enumDataType.UInt64:
                    return new HistogramComputerUInt64();
                case enumDataType.Int64:
                    return new HistogramComputerInt64();
                case enumDataType.Float:
                    return new HistogramComputerFloat();
                case enumDataType.Double:
                    return new HistogramComputerDouble();
                default:
                    return null;
            }
        }
    }

    public interface IHistogramComputer
    {
        void Compute(IRasterBand[] srcRasters, RasterQuickStatResult[] results, Action<int, string> progressTracker);
        void Compute(IRasterBand[] srcRasters, int[] aoi, RasterQuickStatResult[] results, Action<int, string> progressTracker);
    }

    public abstract class HistogramComputer<T> : IHistogramComputer
    {
        private const int MAX_BLOCK_SIZE = 1024 * 1024 * 10;//10MB

        public void Compute(IRasterBand[] srcRasters, int[] aoi, RasterQuickStatResult[] results, Action<int, string> progressTracker)
        {
            int width = srcRasters[0].Width;
            int height = srcRasters[0].Height;
            Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, new Size(width, height));
            SortedList<int, AOIHelper.ColRangeOfRow> aoiRowRanges = AOIHelper.ComputeColRanges(aoi, new Size(width, height));
            T[] buffer = GetRowBuffer(aoiRowRanges);
            GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            IntPtr bufferPtr = handle.AddrOfPinnedObject();
            try
            {
                int cols = 0;
                for (int i = 0; i < srcRasters.Length; i++)
                {
                    foreach (int r in aoiRowRanges.Keys)
                    {
                        AOIHelper.ColRangeOfRow range = aoiRowRanges[r];
                        cols = range.EndCol - range.BeginCol + 1;
                        srcRasters[i].Read(range.BeginCol, range.Row, cols, 1, bufferPtr, srcRasters[i].DataType, cols, 1);
                        HandleHistogram(buffer, results[i], cols);
                    }
                }
                HandleMeanValues(srcRasters.Length, results, aoi.Length);
            }
            finally
            {
                handle.Free();
            }
        }

        private T[] GetRowBuffer(SortedList<int, AOIHelper.ColRangeOfRow> aoiRowRanges)
        {
            int cols = 0;
            foreach (AOIHelper.ColRangeOfRow range in aoiRowRanges.Values)
            {
                cols = Math.Max(cols, range.EndCol - range.BeginCol + 1);
            }
            return new T[cols];
        }

        protected abstract void HandleHistogram(T[] buffer, RasterQuickStatResult rasterQuickStatResult, int cols);

        public void Compute(IRasterBand[] srcRasters, RasterQuickStatResult[] results, Action<int, string> progressTracker)
        {
            int bandCount = srcRasters.Length;
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
            int bRow = 0;
            int eRow = 0;
            int actualCount = 0;
            int bufferRowCount = 0;
            float percent = countBlocks / 100f;
            int ipercent = 0;
            for (int i = 0; i < countBlocks; i++, bRow += rowsOfBlock)
            {
                //确保最后一块的结束行正确
                eRow = Math.Min(height, bRow + rowsOfBlock);
                bufferRowCount = eRow - bRow;
                //从参与计算的波段中读取第i块数据
                for (int iSrc = 0; iSrc < srcCount; iSrc++)
                    srcRasters[iSrc].Read(0, bRow, width, bufferRowCount, srcHandles[iSrc].AddrOfPinnedObject(), srcRasters[iSrc].DataType, width, bufferRowCount);
                //执行波段计算并将计算结果写到目标波段块缓存区
                actualCount = bufferRowCount * width;
                //
                HandleHistogram(bandCount, srcBuffers, actualCount, results);
                //打印进度
                if (progressTracker != null)
                {
                    ipercent = (int)(i / percent);
                    progressTracker(ipercent, string.Empty);
                }
            }
            //
            HandleMeanValues(bandCount, results, srcRasters[0].Width * srcRasters[0].Height);
            //
            if (ipercent < 100)
                if (progressTracker != null)
                    progressTracker(100, string.Empty);
            //释放缓存区
            for (int i = 0; i < srcCount; i++)
                srcHandles[i].Free();
        }

        private void HandleMeanValues(int bandCount, RasterQuickStatResult[] results, int pixelCount)
        {
            for (int b = 0; b < bandCount; b++)
            {
                results[b].Stddev = results[b].Stddev / pixelCount;
                results[b].HistogramResult.PixelCount = pixelCount;
            }
        }

        protected abstract void HandleHistogram(int bandCount, T[][] srcBuffers, int acutalCount, RasterQuickStatResult[] results);
    }


    public class HistogramComputerByte : HistogramComputer<byte>
    {
        protected override void HandleHistogram(int bandCount, byte[][] srcBuffers, int acutalCount, RasterQuickStatResult[] results)
        {
            int minValue = 0, maxValue = 0;
            int idx = 0;
            int bin = 0;
            double stddevValue, meanValue;
            double std = 0;
            HistogramResult histogram;
            for (int b = 0; b < bandCount; b++)
            {
                histogram = results[b].HistogramResult;
                minValue = (int)results[b].MinValue;
                maxValue = (int)results[b].MaxValue;
                bin = (int)histogram.Bin;
                stddevValue = results[b].Stddev;
                meanValue = results[b].MeanValue;
                int itemCount = histogram.Items.Length;
                if (bin != 0)
                {
                    for (int i = 0; i < acutalCount; i++)
                    {//idx最大256
                        idx = (srcBuffers[b][i] - minValue) / bin;
                        if (idx < 0 || idx > itemCount)
                        continue;
                        histogram.Items[idx]++;
                        std = srcBuffers[b][i] - meanValue;
                        stddevValue += (std * std);
                    }
                    results[b].Stddev = stddevValue;
                }  
                else
                {
                    histogram.Items[0] = acutalCount;
                }
            }
        }

        protected override void HandleHistogram(byte[] buffer, RasterQuickStatResult result, int cols)
        {
            int idx = 0;
            double std = 0;
            HistogramResult histogram = result.HistogramResult;
            int minValue = (int)result.MinValue;
            int maxValue = (int)result.MaxValue;
            int bin = (int)histogram.Bin;
            double stddevValue = result.Stddev;
            double meanValue = result.MeanValue;
            int itemCount = histogram.Items.Length;
            if (bin != 0)
            {
                for (int i = 0; i < cols; i++)
                {
                    idx = (buffer[i] - minValue) / bin;
                    if (idx < 0 || idx > itemCount)
                        continue;
                    histogram.Items[idx]++;
                    std = buffer[i] - meanValue;
                    stddevValue += (std * std);
                }
                result.Stddev = stddevValue;
            }
            else
            {
                histogram.Items[0] = cols;
            }
        }
    }


    public class HistogramComputerInt16 : HistogramComputer<Int16>
    {
        protected override void HandleHistogram(int bandCount, Int16[][] srcBuffers, int acutalCount, RasterQuickStatResult[] results)
        {
            int minValue = 0, maxValue = 0;
            int idx = 0;
            int bin = 0;
            double stddevValue, meanValue;
            double std = 0;
            HistogramResult histogram;
            for (int b = 0; b < bandCount; b++)
            {
                histogram = results[b].HistogramResult;
                minValue = (int)results[b].MinValue;
                maxValue = (int)results[b].MaxValue;
                bin = (int)histogram.Bin;
                stddevValue = results[b].Stddev;
                meanValue = results[b].MeanValue;
                int itemCount = histogram.Items.Length;
                if (bin != 0)
                {
                    for (int i = 0; i < acutalCount; i++)
                    {
                        idx = (srcBuffers[b][i] - minValue) / bin;
                        if (idx < 0 || idx > itemCount)
                            continue;
                        histogram.Items[idx]++;
                        std = srcBuffers[b][i] - meanValue;
                        stddevValue += (std * std);
                    }
                    results[b].Stddev = stddevValue;
                }
                else
                {
                    histogram.Items[0] = acutalCount;
                }
            }
        }

        protected override void HandleHistogram(Int16[] buffer, RasterQuickStatResult result, int cols)
        {
            int idx = 0;
            double std = 0;
            HistogramResult histogram = result.HistogramResult;
            int minValue = (int)result.MinValue;
            int maxValue = (int)result.MaxValue;
            int bin = (int)histogram.Bin;
            double stddevValue = result.Stddev;
            double meanValue = result.MeanValue;
            int itemCount = histogram.Items.Length;
            if (bin != 0)
            {
                for (int i = 0; i < cols; i++)
                {
                    idx = (buffer[i] - minValue) / bin;
                    if (idx < 0 || idx > itemCount)
                        continue;
                    histogram.Items[idx]++;
                    std = buffer[i] - meanValue;
                    stddevValue += (std * std);
                }
                result.Stddev = stddevValue;
            }
            else
            {
                histogram.Items[0] = cols;
            }
        }
    }

    public class HistogramComputerUInt16 : HistogramComputer<UInt16>
    {
        protected override void HandleHistogram(int bandCount, UInt16[][] srcBuffers, int acutalCount, RasterQuickStatResult[] results)
        {
            int minValue = 0, maxValue = 0;
            int idx = 0;
            int bin = 0;
            double stddevValue, meanValue;
            double std = 0;
            HistogramResult histogram;
            for (int b = 0; b < bandCount; b++)
            {
                histogram = results[b].HistogramResult;
                minValue = (int)results[b].MinValue;
                maxValue = (int)results[b].MaxValue;
                bin = (int)histogram.Bin;
                stddevValue = results[b].Stddev;
                meanValue = results[b].MeanValue;
                int itemCount = histogram.Items.Length;
                if (bin != 0)
                {
                    for (int i = 0; i < acutalCount; i++)
                    {
                        idx = (srcBuffers[b][i] - minValue) / bin;
                        if (idx < 0 || idx > itemCount)
                            continue;
                        histogram.Items[idx]++;
                        std = srcBuffers[b][i] - meanValue;
                        stddevValue += (std * std);
                    }
                    results[b].Stddev = stddevValue;
                }
                else
                {
                    histogram.Items[0] = acutalCount;
                }
            }
        }

        protected override void HandleHistogram(UInt16[] buffer, RasterQuickStatResult result, int cols)
        {
            int idx = 0;
            double std = 0;
            HistogramResult histogram = result.HistogramResult;
            int minValue = (int)result.MinValue;
            int maxValue = (int)result.MaxValue;
            int bin = (int)histogram.Bin;
            double stddevValue = result.Stddev;
            double meanValue = result.MeanValue;
            int itemCount = histogram.Items.Length;
            if (bin != 0)
            {
                for (int i = 0; i < cols; i++)
                {
                    idx = (buffer[i] - minValue) / bin;
                    if (idx < 0 || idx > itemCount)
                        continue;
                    histogram.Items[idx]++;
                    std = buffer[i] - meanValue;
                    stddevValue += (std * std);
                }
                result.Stddev = stddevValue;
            }
            else
            {
                histogram.Items[0] = cols;
            }
        }
    }

    public class HistogramComputerInt32 : HistogramComputer<Int32>
    {
        protected override void HandleHistogram(int bandCount, Int32[][] srcBuffers, int acutalCount, RasterQuickStatResult[] results)
        {
            int minValue = 0, maxValue = 0;
            int idx = 0;
            int bin = 0;
            double stddevValue, meanValue;
            double std = 0;
            HistogramResult histogram;
            for (int b = 0; b < bandCount; b++)
            {
                histogram = results[b].HistogramResult;
                minValue = (int)results[b].MinValue;
                maxValue = (int)results[b].MaxValue;
                bin = (int)histogram.Bin;
                stddevValue = results[b].Stddev;
                meanValue = results[b].MeanValue;
                int itemCount = histogram.Items.Length;
                if (bin != 0)
                {
                    for (int i = 0; i < acutalCount; i++)
                    {//idx最大256
                        idx = (srcBuffers[b][i] - minValue) / bin;
                        if (idx < 0 || idx > itemCount)
                            continue;
                        histogram.Items[idx]++;
                        std = srcBuffers[b][i] - meanValue;
                        stddevValue += (std * std);
                    }
                    results[b].Stddev = stddevValue;
                }
                else
                {
                    histogram.Items[0] = acutalCount;
                }
            }
        }

        protected override void HandleHistogram(Int32[] buffer, RasterQuickStatResult result, int cols)
        {
            int idx = 0;
            double std = 0;
            HistogramResult histogram = result.HistogramResult;
            int minValue = (int)result.MinValue;
            int maxValue = (int)result.MaxValue;
            int bin = (int)histogram.Bin;
            double stddevValue = result.Stddev;
            double meanValue = result.MeanValue;
            int itemCount = histogram.Items.Length;
            if (bin != 0)
            {
                for (int i = 0; i < cols; i++)
                {
                    idx = (buffer[i] - minValue) / bin;
                    if (idx < 0 || idx > itemCount)
                        continue;
                    histogram.Items[idx]++;
                    std = buffer[i] - meanValue;
                    stddevValue += (std * std);
                }
                result.Stddev = stddevValue;
            }
            else
            {
                histogram.Items[0] = cols;
            }
        }
    }

    public class HistogramComputerUInt32 : HistogramComputer<UInt32>
    {
        protected override void HandleHistogram(int bandCount, UInt32[][] srcBuffers, int acutalCount, RasterQuickStatResult[] results)
        {
            double minValue = 0, maxValue = 0;
            int idx = 0;
            double bin = 0;
            double stddevValue, meanValue;
            double std = 0;
            HistogramResult histogram;
            for (int b = 0; b < bandCount; b++)
            {
                histogram = results[b].HistogramResult;
                minValue = results[b].MinValue;
                maxValue = results[b].MaxValue;
                bin = histogram.Bin;
                stddevValue = results[b].Stddev;
                meanValue = results[b].MeanValue;
                int itemCount = histogram.Items.Length;
                if (bin != 0d)
                {
                    for (int i = 0; i < acutalCount; i++)
                    {//idx最大256
                        idx = (int)((srcBuffers[b][i] - minValue) / bin);
                        if (idx < 0 || idx > itemCount)
                            continue;
                        histogram.Items[idx]++;
                        std = srcBuffers[b][i] - meanValue;
                        stddevValue += (std * std);
                    }
                    results[b].Stddev = stddevValue;
                }
                else
                {
                    histogram.Items[0] = acutalCount;
                }
            }
        }

        protected override void HandleHistogram(UInt32[] buffer, RasterQuickStatResult result, int cols)
        {
            int idx = 0;
            double std = 0;
            HistogramResult histogram = result.HistogramResult;
            double minValue = result.MinValue;
            double maxValue = result.MaxValue;
            double bin = histogram.Bin;
            double stddevValue = result.Stddev;
            double meanValue = result.MeanValue;
            int itemCount = histogram.Items.Length;
            if (bin != 0d)
            {
                for (int i = 0; i < cols; i++)
                {//idx最大256
                    idx = (int)((buffer[i] - minValue) / bin);
                    if (idx < 0 || idx > itemCount)
                        continue;
                    histogram.Items[idx]++;
                    std = buffer[i] - meanValue;
                    stddevValue += (std * std);
                }
                result.Stddev = stddevValue;
            }
            else
            {
                histogram.Items[0] = cols;
            }
        }
    }

    public class HistogramComputerInt64 : HistogramComputer<Int64>
    {
        protected override void HandleHistogram(int bandCount, Int64[][] srcBuffers, int acutalCount, RasterQuickStatResult[] results)
        {
            double minValue = 0, maxValue = 0;
            long idx = 0;
            double bin = 0;
            double stddevValue, meanValue;
            double std = 0;
            HistogramResult histogram;
            for (int b = 0; b < bandCount; b++)
            {
                histogram = results[b].HistogramResult;
                minValue = results[b].MinValue;
                maxValue = results[b].MaxValue;
                bin = histogram.Bin;
                stddevValue = results[b].Stddev;
                meanValue = results[b].MeanValue;
                int itemCount = histogram.Items.Length;
                if (bin != 0d)
                {
                    for (int i = 0; i < acutalCount; i++)
                    {
                        idx = (int)((srcBuffers[b][i] - minValue) / bin);
                        if (idx < 0 || idx > itemCount)
                            continue;
                        histogram.Items[idx]++;
                        std = srcBuffers[b][i] - meanValue;
                        stddevValue += (std * std);
                    }
                    results[b].Stddev = stddevValue;
                } 
                else
                {
                    histogram.Items[0] = acutalCount;
                }
            }
        }

        protected override void HandleHistogram(Int64[] buffer, RasterQuickStatResult result, int cols)
        {
            int idx = 0;
            double std = 0;
            HistogramResult histogram = result.HistogramResult;
            double minValue = result.MinValue;
            double maxValue = result.MaxValue;
            double bin = histogram.Bin;
            double stddevValue = result.Stddev;
            double meanValue = result.MeanValue;
            int itemCount = histogram.Items.Length;
            if (bin != 0d)
            {
                for (int i = 0; i < cols; i++)
                {
                    idx = (int)((buffer[i] - minValue) / bin);
                    if (idx < 0 || idx > itemCount)
                        continue;
                    histogram.Items[idx]++;
                    std = buffer[i] - meanValue;
                    stddevValue += (std * std);
                }
                result.Stddev = stddevValue;
            } 
            else
            {
                histogram.Items[0] = cols;
            }
        }
    }

    public class HistogramComputerUInt64 : HistogramComputer<UInt64>
    {
        protected override void HandleHistogram(int bandCount, UInt64[][] srcBuffers, int acutalCount, RasterQuickStatResult[] results)
        {
            double minValue = 0, maxValue = 0;
            long idx = 0;
            double bin = 0;
            double stddevValue, meanValue;
            double std = 0;
            HistogramResult histogram;
            for (int b = 0; b < bandCount; b++)
            {
                histogram = results[b].HistogramResult;
                minValue = results[b].MinValue;
                maxValue = results[b].MaxValue;
                bin = histogram.Bin;
                stddevValue = results[b].Stddev;
                meanValue = results[b].MeanValue;
                int itemCount = histogram.Items.Length;
                if (bin != 0d)
                {
                    for (int i = 0; i < acutalCount; i++)
                    {
                        idx = (int)((srcBuffers[b][i] - minValue) / bin);
                        if (idx < 0 || idx > itemCount)
                            continue;
                        histogram.Items[idx]++;
                        std = srcBuffers[b][i] - meanValue;
                        stddevValue += (std * std);
                    }
                    results[b].Stddev = stddevValue;
                } 
                else
                {
                    histogram.Items[0] = acutalCount;
                }
            }
        }

        protected override void HandleHistogram(UInt64[] buffer, RasterQuickStatResult result, int cols)
        {
            int idx = 0;
            double std = 0;
            HistogramResult histogram = result.HistogramResult;
            double minValue = result.MinValue;
            double maxValue = result.MaxValue;
            double bin = histogram.Bin;
            double stddevValue = result.Stddev;
            double meanValue = result.MeanValue;
            int itemCount = histogram.Items.Length;
            if (bin != 0d)
            {
                for (int i = 0; i < cols; i++)
                {
                    idx = (int)((buffer[i] - minValue) / bin);
                    if (idx < 0 || idx > itemCount)
                        continue;
                    histogram.Items[idx]++;
                    std = buffer[i] - meanValue;
                    stddevValue += (std * std);
                }
                result.Stddev = stddevValue;
            } 
            else
            {
                histogram.Items[0] = cols;
            }
        }
    }

    public class HistogramComputerFloat : HistogramComputer<float>
    {
        protected override void HandleHistogram(int bandCount, float[][] srcBuffers, int acutalCount, RasterQuickStatResult[] results)
        {
            float minValue = 0, maxValue = 0;
            int idx = 0;
            float bin = 0;
            double stddevValue, meanValue;
            double std = 0;
            HistogramResult histogram;
            for (int b = 0; b < bandCount; b++)
            {
                histogram = results[b].HistogramResult;
                minValue = (float)results[b].MinValue;
                maxValue = (float)results[b].MaxValue;
                bin = (float)histogram.Bin;
                stddevValue = results[b].Stddev;
                meanValue = results[b].MeanValue;
                int itemCount = histogram.Items.Length;
                if (bin != 0f)
                {
                    for (int i = 0; i < acutalCount; i++)
                    {
                        idx = (int)((srcBuffers[b][i] - minValue) / bin);
                        if (idx < 0 || idx > itemCount)
                            continue;
                        histogram.Items[idx]++;
                        std = srcBuffers[b][i] - meanValue;
                        stddevValue += (std * std);
                    }
                    results[b].Stddev = stddevValue;
                } 
                else
                {
                    histogram.Items[0] = acutalCount;
                }
            }
        }

        protected override void HandleHistogram(float[] buffer, RasterQuickStatResult result, int cols)
        {
            int idx = 0;
            double std = 0;
            HistogramResult histogram = result.HistogramResult;
            double minValue = (float)result.MinValue;
            double maxValue = (float)result.MaxValue;
            float bin = (float)histogram.Bin;
            double stddevValue = result.Stddev;
            double meanValue = result.MeanValue;
            int itemCount = histogram.Items.Length;
            if (bin !=0f)
            {
                for (int i = 0; i < cols; i++)
                {
                    idx = (int)((buffer[i] - minValue) / bin);
                    if (idx < 0 || idx > itemCount)
                        continue;
                    histogram.Items[idx]++;
                    std = buffer[i] - meanValue;
                    stddevValue += (std * std);
                }
                result.Stddev = stddevValue;
            } 
            else
            {
                histogram.Items[0] = cols;
            }
        }
    }

    public class HistogramComputerDouble : HistogramComputer<double>
    {
        protected override void HandleHistogram(int bandCount, double[][] srcBuffers, int acutalCount, RasterQuickStatResult[] results)
        {
            double minValue = 0, maxValue = 0;
            int idx = 0;
            double bin = 0;
            double stddevValue, meanValue;
            double std = 0;
            HistogramResult histogram;
            for (int b = 0; b < bandCount; b++)
            {
                histogram = results[b].HistogramResult;
                minValue = results[b].MinValue;
                maxValue = results[b].MaxValue;
                bin = histogram.Bin;
                stddevValue = results[b].Stddev;
                meanValue = results[b].MeanValue;
                int itemCount = histogram.Items.Length;
                if (bin !=0d)
                {
                    for (int i = 0; i < acutalCount; i++)
                    {
                        idx = (int)((srcBuffers[b][i] - minValue) / bin);
                        if (idx < 0 || idx > itemCount)
                            continue;
                        histogram.Items[idx]++;
                        std = srcBuffers[b][i] - meanValue;
                        stddevValue += (std * std);
                    }
                    results[b].Stddev = stddevValue;
                } 
                else
                {
                    histogram.Items[0] = acutalCount;
                }
            }
        }

        protected override void HandleHistogram(double[] buffer, RasterQuickStatResult result, int cols)
        {
            int idx = 0;
            double std = 0;
            HistogramResult histogram = result.HistogramResult;
            double minValue = result.MinValue;
            double maxValue = result.MaxValue;
            double bin = histogram.Bin;
            double stddevValue = result.Stddev;
            double meanValue = result.MeanValue;
            int itemCount = histogram.Items.Length;
            if (bin !=0d)
            {
                for (int i = 0; i < cols; i++)
                {
                    idx = (int)((buffer[i] - minValue) / bin);
                    if (idx < 0 || idx > itemCount)
                        continue;
                    histogram.Items[idx]++;
                    std = buffer[i] - meanValue;
                    stddevValue += (std * std);
                }
                result.Stddev = stddevValue;
            } 
            else
            {
                histogram.Items[0] = cols;
            }
        }
    }
}
