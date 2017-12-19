using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public interface ITileComputer
    {
        int TileSize { get;}
        int SampleRatio { get; }
        int GetLevelCount(int width, int height);
        Size GetSizeByLevel(int level, int width, int height);
        TileIdentify[] GetTileIdentifies(int width, int height, out int rowCount, out int colCount);
        LevelDef[] GetLevelDefs(int width, int height);
        LevelDef GetSuitableLevel(LevelDef[] levels,int wndWidth, int wndHeight, int dataWidth, int dataHeight);
    }
}
