using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.Core.RasterDrawing
{
    /// <summary>
    /// RasterDrawing需要内存估算器
    /// </summary>
    public class MemEstimatorOfRasterDrawing
    {
        public float Estimate(string fname,out int memoryOfTile,out int tileCount)
        {
            int bandCount;
            int width;
            int height;
            tileCount = 0;
            memoryOfTile = 0;
            using (IRasterDataProvider prd = GeoDataDriver.Open(fname) as IRasterDataProvider)
            {
                bandCount = prd.BandCount;
                width = prd.Width;
                height = prd.Height;
            }
            int tileSize = 512;
            TileComputer computer = new TileComputer(tileSize, 2);
            LevelDef[] lvs = computer.GetLevelDefs(width, height);
            if (lvs == null || lvs.Length == 0)
                return 0;
            tileCount = 0;
            foreach (LevelDef lv in lvs)
            {
                if (lv.TileIdentities != null && lv.TileIdentities.Length > 0)
                    tileCount += lv.TileIdentities.Length;
            }
            //
            float memorySize = 0;
            if (bandCount == 1)
                memoryOfTile = tileSize * tileSize * 1;
            else
                memoryOfTile = tileSize * tileSize * 3;
            memorySize = tileCount * memoryOfTile / 1024 / 1024;//MB
            //
            return memorySize;
        }
    }
}
