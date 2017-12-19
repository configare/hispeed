using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface IFileDiskCacheManager:IDisposable
    {
        string CacheDir { get; }
        void Start();
        void Stop();
        bool IsExist(TileIdentify tile, int[] selectedBandNos);
        T[] GetTile<T>(TileIdentify tile, int bandNo);
        void PutTile<T>(TileIdentify tile,int bandNo, T[] buffer);
    }
}
