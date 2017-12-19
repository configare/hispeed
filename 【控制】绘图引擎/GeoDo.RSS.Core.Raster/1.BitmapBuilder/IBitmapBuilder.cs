using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface IBitmapBuilder<T>:IDisposable
    {
        void Build(int width, int height,T[] redBuffer, T[] greenBuffer, T[] blueBuffer,ref Bitmap bitmap);
        void Build(int width, int height,int offsetX,int offsetY, T[] redBuffer, T[] greenBuffer, T[] blueBuffer, ref Bitmap bitmap);
        void Build(int width,int height,T[] redBuffer, T[] greenBuffer, T[] blueBuffer,
                         Func<T,byte> redStretcher,Func<T,byte> greenStretcher,Func<T,byte> blueStretcher,
                         ref Bitmap bitmap);
        void Build(int width, int height,int offsetX,int offsetY, T[] redBuffer, T[] greenBuffer, T[] blueBuffer,
                         Func<T, byte> redStretcher, Func<T, byte> greenStretcher, Func<T, byte> blueStretcher,
                         ref Bitmap bitmap);
        void Build(int width, int height, T[] grayBuffer, ref Bitmap bitmap);
        void Build(int width, int height,int offsetX,int offsetY, T[] grayBuffer, ref Bitmap bitmap);
        void Build(int width, int height, T[] grayBuffer, Func<T, byte> grayStretcher, ref Bitmap bitmap);
        void Build(int width, int height, int offsetX, int offsetY, T[] grayBuffer, Func<T, byte> grayStretcher, ref Bitmap bitmap);
    }
}
