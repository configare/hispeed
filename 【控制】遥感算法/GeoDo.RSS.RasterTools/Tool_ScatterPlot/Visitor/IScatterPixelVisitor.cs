using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using GeoDo.MathAlg;
using System.Drawing;

namespace GeoDo.RSS.RasterTools
{
    public static class ScatterPixelVisitorFactory
    {
        public static IScatterPixelVisitor GetVisitor(enumDataType dataType)
        {
            switch (dataType)
            {
                case enumDataType.Byte:
                    return new ScatterPixelVisitorByte();
                case enumDataType.UInt16:
                    return new ScatterPixelVisitorUInt16();
                case enumDataType.Int16:
                    return new ScatterPixelVisitorInt16();
                case enumDataType.UInt32:
                    return new ScatterPixelVisitorUInt32();
                case enumDataType.Int32:
                    return new ScatterPixelVisitorInt32();
                case enumDataType.UInt64:
                    return new ScatterPixelVisitorUInt64();
                case enumDataType.Int64:
                    return new ScatterPixelVisitorInt64();
                case enumDataType.Float:
                    return new ScatterPixelVisitorFloat();
                case enumDataType.Double:
                    return new ScatterPixelVisitorDouble();
                default:
                    return null;
            }
        }
    }

    public interface IScatterPixelVisitor
    {
        void Visit(int[] aoi,Action<float, float> action, Action<int, string> progressTracker);
        void Init(IRasterBand xBand, IRasterBand yBand);
        void Abort();
        void Visit(Action<float, float> action, Action<int, string> progressTracker);
        void InitObj(Object xdata, Object ydata);
    }

    public abstract class ScatterPixelVisitor<T> : IScatterPixelVisitor
    {
        private const int MAX_BLOCK_SIZE = 1024 * 1024 * 10;//10MB
        private IRasterBand _xBand, _yBand;
        private bool _isAborting = false;
        private bool _runing = false;
        private T [] _xdata, _ydata;

        public void Init(IRasterBand xBand, IRasterBand yBand)
        {
            _xBand = xBand;
            _yBand = yBand;
        }
        public void InitObj(Object xBand, Object yBand)
        {
            _xdata = xBand as T[];
            _ydata = yBand as T[];
        }

        public void Abort()
        {
            if (_runing)
            {
                _isAborting = true;
            }
        }

        public void Visit(int[] aoi,Action<float, float> action, Action<int, string> progressTracker)
        {
            if (aoi == null || aoi.Length == 0)
                FullVisit(action, progressTracker);
            else
                VisitByAOI(aoi, action, progressTracker);
        }

        private void VisitByAOI(int[] aoi, Action<float, float> action, Action<int, string> progressTracker)
        {
            IRasterBand[] srcRasters = new IRasterBand[] { _xBand, _yBand };
            int width = _xBand.Width;
            int height = _xBand.Height;
            int bandCount = 2;
            Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, new Size(width, height));
            //Dictionary<int, AOIHelper.AOIRowRange> aoiRowRanges = AOIHelper.ComputeRowIndexRange(aoi, new Size(width, height));
            SortedList<int,AOIHelper.ColRangeOfRow> aoiRowRanges = AOIHelper.ComputeColRanges(aoi,new Size(width, height));
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
                AOIHelper.ColRangeOfRow range  =null;
                for (int r = bRow; r < eRow; r++)
                {
                    //读取一行数据
                    for (int b = 0; b < bandCount; b++)
                        srcRasters[b].Read(0, r, width, 1, srcHandles[b].AddrOfPinnedObject(), _xBand.DataType, width, 1);
                    //获取该行AOI的起始列
                    if(aoiRowRanges.ContainsKey(r))
                         range = aoiRowRanges[r];
                    else
                        continue;
                    //调用计算委托
                    bCol = range.BeginCol;
                    eCol = range.EndCol;
                    for (int c = bCol; c < eCol; c++)
                        action(ToFloat(srcBuffers[0][c]), ToFloat(srcBuffers[1][c]));
                    //
                    if (progressTracker != null)
                        progressTracker(100 * (r - bRow + 1) / rowCount, string.Empty);
                }
            }
            finally 
            {
                for (int i = 0; i < bandCount; i++)
                    srcHandles[i].Free();
            }
        }

        private void FullVisit(Action<float, float> action, Action<int, string> progressTracker)
        {
            _runing = true;
            IRasterBand[] srcRasters = new IRasterBand[] { _xBand, _yBand };
            int bandCount = 2;
            //计算每块包含的行数
            int rowsOfBlock = 0;
            int rowSize = _xBand.Width * DataTypeHelper.SizeOf(_xBand.DataType);
            if (rowSize >= MAX_BLOCK_SIZE)
                rowsOfBlock = 1;
            else
                rowsOfBlock = MAX_BLOCK_SIZE / rowSize;
            //计算总块数
            int countBlocks = (int)Math.Ceiling((float)_xBand.Height / rowsOfBlock); //总块数
            if (countBlocks == 0)
                countBlocks = 1;
            if (countBlocks == 1)
                rowsOfBlock = srcRasters[0].Height;
            //创建参与计算的波段缓存区(缓存区大小==块大小)
            int width = _xBand.Width;
            int height = _xBand.Height;
            GCHandle[] srcHandles = new GCHandle[bandCount];
            T[][] srcBuffers = new T[bandCount][];
            for (int i = 0; i < bandCount; i++)
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
                if (_isAborting)
                    goto endLine;
                //确保最后一块的结束行正确
                eRow = Math.Min(height, bRow + rowsOfBlock);
                bufferRowCount = eRow - bRow;
                //从参与计算的波段中读取第i块数据
                for (int iSrc = 0; iSrc < bandCount; iSrc++)
                    srcRasters[iSrc].Read(0, bRow, width, bufferRowCount, srcHandles[iSrc].AddrOfPinnedObject(), srcRasters[iSrc].DataType, width, bufferRowCount);
                //执行波段计算并将计算结果写到目标波段块缓存区
                actualCount = bufferRowCount * width;
                //
                for (int k = 0; k < actualCount; k++)
                {
                    if (_isAborting)
                        goto endLine;
                    action(ToFloat(srcBuffers[0][k]), ToFloat(srcBuffers[1][k]));
                }
                //打印进度
                if (progressTracker != null)
                {
                    ipercent = (int)((i + 1) / percent);
                    progressTracker(ipercent, string.Empty);
                }
            }
            //
            if (ipercent < 100)
                if (progressTracker != null)
                    progressTracker(100, string.Empty);
        endLine:
            //释放缓存区
            for (int i = 0; i < bandCount; i++)
                srcHandles[i].Free();
            _isAborting = false;
            _runing = false;
        }

        protected abstract float ToFloat(T t);

        public void Visit(Action<float, float> action, Action<int, string> progressTracker)
        {
            _runing = true;
            int actualCount = _xdata.Length ;
            if (_isAborting)
                goto endLine;
            for (int k = 0; k < actualCount; k++)
            {
                if (_isAborting)
                    goto endLine;
                action(ToFloat(_xdata[k]), ToFloat(_ydata[k]));
            }
            int ipercent = 0;
            if (ipercent < 100)
                if (progressTracker != null)
                    progressTracker(100, string.Empty);
        endLine:
            //释放缓存区
            _isAborting = false;
            _runing = false;
        }
    }

    public class ScatterPixelVisitorByte : ScatterPixelVisitor<byte>
    {
        protected override float ToFloat(byte t)
        {
            return t;
        }
    }

    public class ScatterPixelVisitorInt16 : ScatterPixelVisitor<Int16>
    {
        protected override float ToFloat(Int16 t)
        {
            return t;
        }
    }

    public class ScatterPixelVisitorUInt16 : ScatterPixelVisitor<UInt16>
    {
        protected override float ToFloat(UInt16 t)
        {
            return t;
        }
    }

    public class ScatterPixelVisitorInt32 : ScatterPixelVisitor<Int32>
    {
        protected override float ToFloat(Int32 t)
        {
            return t;
        }
    }

    public class ScatterPixelVisitorUInt32 : ScatterPixelVisitor<UInt32>
    {
        protected override float ToFloat(UInt32 t)
        {
            return t;
        }
    }

    public class ScatterPixelVisitorInt64 : ScatterPixelVisitor<Int64>
    {
        protected override float ToFloat(Int64 t)
        {
            return t;
        }
    }

    public class ScatterPixelVisitorUInt64 : ScatterPixelVisitor<UInt64>
    {
        protected override float ToFloat(UInt64 t)
        {
            return t;
        }
    }

    public class ScatterPixelVisitorFloat : ScatterPixelVisitor<float>
    {
        protected override float ToFloat(float t)
        {
            return t;
        }
    }

    public class ScatterPixelVisitorDouble : ScatterPixelVisitor<double>
    {
        protected override float ToFloat(double t)
        {
            return (float)t;
        }
    }
}
