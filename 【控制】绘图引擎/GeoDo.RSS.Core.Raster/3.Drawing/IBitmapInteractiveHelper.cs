using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    /// <summary>
    /// 影像当前窗口显示位图交互帮助对象
    /// 注：因分块的原因，位图大小可能大于窗口大小。
    /// </summary>
    public interface IBitmapInteractiveHelper
    {
        Bitmap Bitmap { get; }
        GeoDo.RSS.Core.DrawEngine.CoordEnvelope Envelope { get; }
        Color GetColorAt(int screenX, int screenY);
        Color GetColorAtPrj(double prjX, double prjY);
    }
}
