using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface IBandMemoryCache
    {
        int BandNo { get; }
        TileData[] GetTiles(int levelNo, Rectangle rasterEnvelope);
        void LoadBySync(int levelNo);
        void Update(int levelNo, Rectangle rasterEnvelopeOfWnd, Rectangle rasterEnvelopeOfVirtualWnd);
        void Cache(TileData tileData);
        void ReduceMemory(int levelNo, int rows, int cols, Rectangle cacheWindow); 
    }
}
