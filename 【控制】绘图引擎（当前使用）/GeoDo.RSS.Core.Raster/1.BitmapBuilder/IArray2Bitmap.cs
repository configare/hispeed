using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface IArray2Bitmap<T>:IDisposable
    {
        void ToBitmap(T[] data,Size size, Dictionary<T, Color> colorMap,ref Bitmap bitmap);
    }
}
