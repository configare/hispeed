using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GeoDo.RSS.Core.DF
{
    public unsafe class BandPixelsVisitor<T>:IDisposable
    {
        //分块读的示例代码
        public unsafe void Visit(IBandOperator band, Action<int,T> action, Action<int, string> progressCallback)
        {
            int rowsOfBlock = (int)Math.Floor((float)GeoDo.RSS.Core.DF.Constants.MAX_PIXELS_BLOCK / band.Width);//每块的行数
            if (rowsOfBlock > band.Height)
                rowsOfBlock = band.Height;
            int countBlocks = (int)Math.Floor((float)band.Height / rowsOfBlock); //总块数
            T[] buffer = new T[rowsOfBlock * band.Width];
            IntPtr bufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            int bRow = 0;
            int eRow = 0;
            int actualCount = 0;
            int bufferRowCount = 0;
            float percent = countBlocks / 100f;
            int ipercent = 0;
            int idx = 0;
            for (int i = 0; i < countBlocks; i++, bRow += rowsOfBlock)
            {
                eRow = Math.Min(band.Height, bRow + rowsOfBlock);
                bufferRowCount = eRow - bRow;
                band.Read(0, bRow, band.Width, bufferRowCount, bufferPtr, band.DataType, band.Width, bufferRowCount);
                //
                actualCount = bufferRowCount * band.Width;
                for (int pixel = 0; pixel < actualCount; pixel ++)
                {
                    action(idx++,buffer[pixel]);
                }
                //
                if (progressCallback != null)
                {
                    ipercent = (int)(i / percent);
                    progressCallback(ipercent, string.Empty);
                }
            }
            if (ipercent < 100)
                if (progressCallback != null)
                    progressCallback(100, string.Empty);
        }

        public unsafe void Visit(IBandOperator band, int interleave, Action<T> action, Action<int, string> progressCallback)
        {
            interleave = Math.Max(1, interleave);
            int rowsOfBlock = (int)Math.Floor((float)GeoDo.RSS.Core.DF.Constants.MAX_PIXELS_BLOCK / band.Width);//每块的行数
            if (rowsOfBlock > band.Height)
                rowsOfBlock = band.Height;
            int countBlocks = (int)Math.Floor((float)band.Height / rowsOfBlock); //总块数
            T[] buffer = new T[rowsOfBlock * band.Width];
            IntPtr bufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            int bRow = 0;
            int eRow = 0;
            int actualCount = 0;
            int bufferRowCount = 0;
            float percent = countBlocks / 100f;
            int ipercent = 0;
            for (int i = 0; i < countBlocks; i++,bRow+= rowsOfBlock)
            {
                eRow = Math.Min(band.Height, bRow + rowsOfBlock);
                bufferRowCount = eRow - bRow;
                band.Read(0, bRow, band.Width, bufferRowCount, bufferPtr, band.DataType, band.Width, bufferRowCount);
                //
                actualCount = bufferRowCount * band.Width;
                for (int pixel = 0; pixel < actualCount; pixel += interleave)
                {
                    action(buffer[pixel]);
                }
                //
                if (progressCallback != null)
                {
                    ipercent = (int)(i / percent);
                    progressCallback(ipercent , string.Empty);
                }
            }
            if(ipercent <100)
                if (progressCallback != null)
                    progressCallback(100, string.Empty);
        }

        public void Dispose()
        {
        }
    }
}
