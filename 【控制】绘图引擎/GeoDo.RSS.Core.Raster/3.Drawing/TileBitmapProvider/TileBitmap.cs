using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class TileBitmap:IDisposable
    {
        public TileIdentify Tile;
        public LevelDef Level;
        public Bitmap Bitmap;
        public bool IsOK = false;
     
        public bool IsEmpty()
        {
            return Tile == null;
        }

        public string GetFileName()
        {
            return string.Format("L{0}_R{1}_C{2}.bmp", Level.LevelNo, Tile.Row,Tile.Col);
        }

        public void Dispose()
        {
            if (Bitmap != null)
            {
                Bitmap.Dispose();
                Bitmap = null;
            }
            Tile = null;
        }
    }
}
