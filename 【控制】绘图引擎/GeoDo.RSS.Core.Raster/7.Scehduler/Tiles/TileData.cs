using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class TileData:IDisposable
    {
        public int BandNo;
        public TileId Tile;
        /// <summary>
        /// T[]
        /// </summary>
        public object Data;

        public void Dispose()
        {
            Data = null;
        }
    }
}
