using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSGeo.GDAL;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GeoDo.RSS.DF.GDAL
{
    internal unsafe class BandPixelsVisitor<T>:IDisposable
    {
        public unsafe void Visit(Band band, int interleave, Action<T> action, Action<int, string> progressCallback)
        {
            interleave = Math.Max(1, interleave);
            int rowsOfBlock = (int)Math.Floor((float)GeoDo.RSS.Core.DF.Constants.MAX_PIXELS_BLOCK / band.XSize);//每块的行数
            if (rowsOfBlock > band.YSize)
                rowsOfBlock = band.YSize;
            int countBlocks = (int)Math.Floor((float)band.YSize / rowsOfBlock); //总块数
            T[] buffer = new T[rowsOfBlock * band.XSize];
            IntPtr bufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
            int bRow = 0;
            int eRow = 0;
            int actualCount = 0;
            int bufferRowCount = 0;
            float percent = countBlocks / 100f;
            int ipercent = 0;
            for (int i = 0; i <= countBlocks; i++,bRow+= rowsOfBlock)
            {
                eRow = Math.Min(band.YSize, bRow + rowsOfBlock);
                bufferRowCount = eRow - bRow;
                band.ReadRaster(0, bRow, band.XSize, bufferRowCount, bufferPtr, band.XSize, bufferRowCount, band.DataType, 0, 0);
                //
                actualCount = bufferRowCount * band.XSize;
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
