using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface ITileBitmapCacheManager:IDisposable
    {
        int Count { get; }
        void Clear();
        bool IsEmpty();
        IEnumerable<TileBitmap> TileBitmaps { get; }
        bool IsExist(TileIdentify tile);
        void Put(TileBitmap tileBitmap);
        void Remove(TileBitmap tileBitmap);
        TileBitmap Get(int levelNo, int row, int col);
    }
}
