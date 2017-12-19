using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface IDataProviderReader : IDisposable
    {
        object GrayStretcher { get; }
        void UpdateSelectedBandNos(int[] selectedBandNos);
        TileBitmap CreateBitmapByTile(LevelDef level, int beginRow, int beginCol, int width, int height, TileIdentify tile);
        Bitmap GetOverview(IRasterDataProvider dataProvider, int width, int height, int[] bands, Action<int> progress);
        void SetColorMapTable(IColorMapTableGetter colorTableGetter);
        IFileDiskCacheManager FileDiskCacheManager { get; }
    }
}
