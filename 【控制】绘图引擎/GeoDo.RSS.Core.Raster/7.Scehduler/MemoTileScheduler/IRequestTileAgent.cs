using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface IRequestTileAgent
    {
        void DoRequest(int bandNo, TileId tile);
        void LoadBySync(int bandNo, LevelId level);
    }
}
