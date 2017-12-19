using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface IWindowBitmapBuilder:IDisposable
    {
        void UpdataWndSize(Size wndSize);
        void Build(TileData[][] tiles, Rectangle rasterWindowEnvelope);
        Bitmap Bitmap { get; }
    }
}
