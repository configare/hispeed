using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 栅格运算接口
    /// 注：1.不同区域(源栅格在目标外部、相交、内部)、不同分辨率在内部处理
    ///     2.不同数据类型不做处理
    /// </summary>
    public abstract class RasterBandsOperator<T> : IRasterBandsOperator<T>
    {
        private IContextMessage _contextMessage;

        public RasterBandsOperator(IContextMessage contextMessage)
        {
            _contextMessage = contextMessage;
        }

        protected abstract T ValueAdd(T v1, T v2);

        private bool CheckDataTypeIsAccordant(IRasterBand raster1, IRasterBand[] raster2, string operatorType)
        {
            foreach (IRasterBand rst in raster2)
            {
                bool isOK = CheckDataTypeIsAccordant(raster1, rst, operatorType);
                if (!isOK)
                    return false;
            }
            return true;
        }

        private bool CheckDataTypeIsAccordant(IRasterBand raster1, IRasterBand raster2, string operatorType)
        {
            if (raster1.DataType != raster2.DataType)
            {
                string msg = "源栅格波段与目标栅格波段数据类型不一致,无法执行" + operatorType + "。";
                if (_contextMessage != null)
                    _contextMessage.PrintMessage(msg);
                else
                    Console.WriteLine(msg);
                return false;
            }
            return true;
        }

        private void PrintMsgWhileNoOverlap(string operatorType)
        {
            string msg = "两栅格波段没有重叠部分,无法执行" + operatorType+"操作。";
            if (_contextMessage != null)
                _contextMessage.PrintMessage(msg);
            else
                Console.WriteLine(msg);
        }

        private bool ComputeOverlapPart(IRasterBand dstRaster, IRasterBand srcRaster,
            out Rectangle srcRect, out Rectangle dstRect)
        {
            srcRect = Rectangle.Empty;
            dstRect = Rectangle.Empty;
            //不重叠判断
            if (srcRaster.CoordEnvelope.MaxX <= dstRaster.CoordEnvelope.MinX || //left
                srcRaster.CoordEnvelope.MinX >= dstRaster.CoordEnvelope.MaxX || //right
                srcRaster.CoordEnvelope.MinY >= dstRaster.CoordEnvelope.MaxY || //top
                srcRaster.CoordEnvelope.MaxY <= dstRaster.CoordEnvelope.MinY)   //bottom
                return false;
            //计算相交部分
            CoordEnvelope evp = srcRaster.CoordEnvelope.Intersect(dstRaster.CoordEnvelope);
            //计算源行列号范围
            srcRect = GetOverlapBound(srcRaster, evp);
            //计算目标行列号范围
            dstRect = GetOverlapBound(dstRaster, evp);
            return true;
        }

        private Rectangle GetOverlapBound(IRasterBand raster, CoordEnvelope overlapEnvelope)
        {
            double resX = raster.CoordEnvelope.Width / raster.Width;
            double resY = raster.CoordEnvelope.Height / raster.Height;
            int bRow = (int)Math.Round((raster.CoordEnvelope.MaxY - overlapEnvelope.MaxY) / resY);
            int eRow = bRow + (int)Math.Round(overlapEnvelope.Height / resY);
            int bCol = (int)Math.Round((overlapEnvelope.MinX - raster.CoordEnvelope.MinX) / resX);
            int eCol = bCol + (int)Math.Round(overlapEnvelope.Width / resX);
            return Rectangle.FromLTRB(bCol, bRow, eCol, eRow);
        }

        /// <summary>
        /// 覆盖
        /// 操作方法：
        /// dstRaster[iPixel] = srcRasters[0..n][iPixel]
        /// 主要通途：对不同区域的多幅栅格拼接为中国区域后执行面积统计
        /// </summary>
        /// <param name="dstRaster"></param>
        /// <param name="srcRasters"></param>
        public void Cover(IRasterBand dstRaster, IRasterBand[] srcRasters)
        {
            if (!CheckDataTypeIsAccordant(dstRaster, srcRasters, "覆盖操作"))
                return;
            Rectangle dstRect, srcRect;
            bool isOverlaped;
            foreach (IRasterBand rst in srcRasters)
            {
                isOverlaped = ComputeOverlapPart(dstRaster, rst, out srcRect, out dstRect);
                if (!isOverlaped)
                    continue;
                Write(rst, srcRect, dstRaster, dstRect);
            }
        }

        private unsafe void Write(IRasterBand srcRaster, Rectangle srcRect, IRasterBand dstRaster, Rectangle dstRect)
        {
            byte[] srcBuffer = new byte[srcRect.Width * DataTypeHelper.SizeOf(srcRaster.DataType)];
            GCHandle srcHandle = GCHandle.Alloc(srcBuffer, GCHandleType.Pinned);
            IntPtr srcBuffer0 = srcHandle.AddrOfPinnedObject();
            try
            {
                float yScale = srcRect.Height / dstRect.Height;
                int srcRow = 0;
                int preRow = -1;
                for (int dstRow = dstRect.Top; dstRow < dstRect.Height; dstRow++)
                {
                    srcRow = (int)(dstRow * yScale);
                    if (srcRow != preRow)
                        srcRaster.Read(srcRect.Left, srcRow, srcRect.Width, 1, srcBuffer0, srcRaster.DataType, srcRect.Width, 1);
                    dstRaster.Write(dstRect.Left, dstRow, dstRect.Width, 1, srcBuffer0, dstRaster.DataType, srcRect.Width, 1);
                    preRow = srcRow;
                }
            }
            finally
            {
                srcHandle.Free();
            }
        }

        private unsafe void Write(IRasterBand srcRaster, Rectangle srcRect, IRasterBand dstRaster, Rectangle dstRect, Func<T, T, T> action)
        {
            int dataTypeSize = DataTypeHelper.SizeOf(dstRaster.DataType);
            T[] srcBuffer = new T[srcRect.Width];
            T[] dstBuffer = new T[dstRect.Width];
            GCHandle srcHandle = GCHandle.Alloc(srcBuffer, GCHandleType.Pinned);
            GCHandle dstHandle = GCHandle.Alloc(dstBuffer, GCHandleType.Pinned);
            IntPtr srcBuffer0 = srcHandle.AddrOfPinnedObject();
            IntPtr dstBuffer0 = dstHandle.AddrOfPinnedObject();
            try
            {
                float yScale = srcRect.Height / dstRect.Height;
                float xScale = srcRect.Width / dstRect.Width;
                int srcRow = 0;
                int srcCol = 0;
                int preSrcRow = -1;
                int preDstRow = -1;
                for (int dstRow = dstRect.Top; dstRow < dstRect.Height; dstRow++)
                {
                    srcRow = (int)(dstRow * yScale);
                    if (srcRow != preSrcRow)
                        srcRaster.Read(srcRect.Left, srcRow, srcRect.Width, 1, srcBuffer0, srcRaster.DataType, srcRect.Width, 1);
                    if (dstRow != preDstRow)
                        dstRaster.Read(dstRect.Left, dstRow, dstRect.Width, 1, dstBuffer0, dstRaster.DataType, dstRect.Width, 1);
                    //
                    for (int dstCol = 0; dstCol < dstRect.Width; dstCol++)
                    {
                        srcCol = (int)(dstCol * xScale);
                        dstBuffer[dstCol] = action(dstBuffer[dstCol], srcBuffer[srcCol]);
                    }
                    //
                    dstRaster.Write(dstRect.Left, dstRow, dstRect.Width, 1, dstBuffer0, dstRaster.DataType, dstRect.Width, 1);
                    preSrcRow = srcRow;
                    preDstRow = dstRow;
                }
            }
            finally
            {
                srcHandle.Free();
                dstHandle.Free();
            }
        }

        public void Add(IRasterBand dstRaster, IRasterBand[] srcRasters)
        {
            if (!CheckDataTypeIsAccordant(dstRaster, srcRasters, "相加操作"))
                return;
            Rectangle dstRect, srcRect;
            bool isOverlaped;
            foreach (IRasterBand rst in srcRasters)
            {
                if (rst.Equals(dstRaster))
                    continue;
                isOverlaped = ComputeOverlapPart(dstRaster, rst, out srcRect, out dstRect);
                if (!isOverlaped)
                    continue;
                Write(rst, srcRect, dstRaster, dstRect, (v1, v2) => { return ValueAdd(v1, v2); });
            }
        }

        public void Subtract(IRasterBand dstRaster, IRasterBand raster1, IRasterBand raster2)
        {
            if (!CheckDataTypeIsAccordant(dstRaster, new IRasterBand[]{raster1,raster2}, "相减操作"))
                return;
            Rectangle dstRect, srcRect;
            bool isOverlaped;
            isOverlaped = ComputeOverlapPart(raster1, raster2, out srcRect, out dstRect);
            if (!isOverlaped)
            {
                PrintMsgWhileNoOverlap("相减操作");
                return;
            }
        }

        public void Multiply(IRasterBand dstRaster, IRasterBand raster1, IRasterBand raster2)
        {
            throw new NotImplementedException();
        }

        public void Roll(IRasterBand dstRaster, IRasterBand[] srcRasters)
        {
            throw new NotImplementedException();
        }

        public void Compare(IRasterBand dstRaster, IRasterBand raster1, IRasterBand raster2, object comparer)
        {
            throw new NotImplementedException();
        }

        public void Max(IRasterBand dstRaster, IRasterBand[] srcRasters)
        {
            throw new NotImplementedException();
        }

        public void Min(IRasterBand dstRaster, IRasterBand[] srcRasters)
        {
            throw new NotImplementedException();
        }

        public void Avg(IRasterBand dstRaster, IRasterBand[] srcRasters)
        {
            throw new NotImplementedException();
        }

        public void DisAvg(IRasterBand dstRaster, IRasterBand primaryRaster, IRasterBand[] srcRasters)
        {
            throw new NotImplementedException();
        }

        public void PixelArea(IRasterBand dstRaster)
        {
            throw new NotImplementedException();
        }
    }
}
