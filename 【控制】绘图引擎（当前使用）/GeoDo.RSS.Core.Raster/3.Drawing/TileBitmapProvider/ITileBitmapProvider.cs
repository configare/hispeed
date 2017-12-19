using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface ITileBitmapProvider : IDisposable
    {
        IDataProviderReader DataProviderReader { get; }
        ITileBitmapCacheManager TileCacheManager { get; }
        /// <summary>
        /// [GET]视窗范围瓦片定位器对象
        /// </summary>
        INearestTilesLocator TilesLocator { get; }
        /// <summary>
        /// [GET]根据画布(Canvas)中的瓦片大小和缩放级数定义创建的瓦片计算器
        /// </summary>
        ITileComputer TileComputer { get; }
        int LoadTileBitmaps(LevelDef[] levels,bool isSync);
        TileBitmap GetTileBitmap(TileIdentify tile);
        TileIdentify[] GetVisibleTiles(out LevelDef level, out Rectangle rasterRect, out int bRow, out int bCol, out int rows, out int cols);
        /// <summary>
        /// 更新选中合成位图的波段序号数组
        /// </summary>
        /// <param name="selectedBandNos">波段序号(从1开始)数组</param>
        void UpdateSelectedBandNos(int[] selectedBandNos);
        void Reset();
        bool IsActive { get; }
        void Active();
        void Deactive();
        int TileCountOfLoading { get; }       
    }
}
