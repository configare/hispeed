using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public delegate void TileReadedCallback(TileData tileData);

    public interface ITileDiskCacheManager:IDisposable
    {
        bool IsRunning { get; }
        void Start();
        void Stop();
        void Request(int bandNo, TileId[] tiles);
        void Enqueue(int bandNo,LevelId level);
        void LoadSync(int bandNo, LevelId level);
    }
}
