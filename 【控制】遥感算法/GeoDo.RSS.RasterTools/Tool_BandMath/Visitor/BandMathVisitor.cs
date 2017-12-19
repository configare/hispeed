using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.RasterTools
{
    public class BandMathVisitor:IBandMathVisitor
    {
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl)]
        public static extern void MemoryCopy(IntPtr pdest, IntPtr psrc, int length);
        private const int MAX_BLOCK_SIZE = 1024 * 1024 * 10;//10MB

        public void Visit<TSrc, TDst>(IRasterBand[] srcRasters, IRasterBand dstRaster, Func<TSrc[], TDst> mathOperator, Action<int, string> progressTracker)
        {
            //计算每块包含的行数
            int rowsOfBlock = 0;
            int rowSize = dstRaster.Width * DataTypeHelper.SizeOf(dstRaster.DataType);
            if (rowSize >= MAX_BLOCK_SIZE)
                rowsOfBlock = 1;
            else
                rowsOfBlock = MAX_BLOCK_SIZE / rowSize;
            //计算总块数
            int countBlocks = (int)Math.Ceiling((float)dstRaster.Height / rowsOfBlock); //总块数
            if (countBlocks == 0)
                countBlocks = 1;
            //创建参与计算的波段缓存区(缓存区大小==块大小)
            int width = srcRasters[0].Width ;
            int height = srcRasters[0].Height;
            int srcCount = srcRasters.Length ;
            GCHandle[] srcHandles = new GCHandle[srcCount];
            TSrc[][] srcBuffers = new TSrc[srcCount][];
            for (int i = 0; i < srcCount; i++)
            {
                srcBuffers[i] = new TSrc[rowsOfBlock * width];
                srcHandles[i] = GCHandle.Alloc(srcBuffers[i], GCHandleType.Pinned);
            }
            //创建目标波段（波段计算结果）缓存区（缓存区大小==块大小）
            TDst[] dstBuffer = new TDst[rowsOfBlock * width];
            GCHandle dstHandle = GCHandle.Alloc(dstBuffer, GCHandleType.Pinned);
            //创建像元值缓存区（缓存区大小==参与计算的波段个数）
            TSrc[] pixelBuffer = new TSrc[srcCount];
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
                for (int pixel = 0; pixel < actualCount; pixel++)
                {
                    for(int b = 0;b<srcCount;b++)
                        pixelBuffer[b] = srcBuffers[b][pixel];
                    //执行波段计算
                    dstBuffer[pixel] = mathOperator(pixelBuffer);
                }
                //将第i块数据写入目标波段
                if (bufferRowCount != rowsOfBlock)
                {
                    TDst[] actualBuffer = new TDst[actualCount];
                    GCHandle actualHandle = GCHandle.Alloc(actualBuffer,GCHandleType.Pinned);
                    MemoryCopy(actualHandle.AddrOfPinnedObject(), dstHandle.AddrOfPinnedObject(), actualCount * DataTypeHelper.SizeOf(dstRaster.DataType));
                    dstRaster.Write(0, bRow, width, bufferRowCount, actualHandle.AddrOfPinnedObject(), dstRaster.DataType, width, bufferRowCount);
                    actualHandle.Free();
                }
                else
                {
                    dstRaster.Write(0, bRow, width, bufferRowCount, dstHandle.AddrOfPinnedObject(), dstRaster.DataType, width, bufferRowCount);
                }
                //打印进度
                if (progressTracker != null)
                {
                    ipercent = (int)(i / percent);
                    progressTracker(ipercent, string.Empty);
                }
            }
            if (ipercent < 100)
                if (progressTracker != null)
                    progressTracker(100, string.Empty);
            //释放缓存区
            for (int i = 0; i < srcCount; i++)
                srcHandles[i].Free();
            dstHandle.Free();
        }
    }
}
