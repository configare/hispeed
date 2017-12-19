using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.Core.RasterDrawing
{
    //[StructLayout(LayoutKind.Sequential)]
    public struct LevelId
    {
        public int No;
        public int Width;
        public int Height;
        public int TileCount;
        public float Scale;
        public TileId[] Tiles;
        public int Rows;
        public int Cols;

        public bool IsEmpty()
        {
            return Width == 0 && Height == 0;
        }

        public override string ToString()
        {
            return string.Format("No={0},Width={1},Height={2},TileCount={3},Scale={4}",
                No.ToString(), Width.ToString().PadRight(6, ' '),
                Height.ToString().PadRight(6, ' '),
                TileCount.ToString().PadRight(4, ' '),
                Scale.ToString());
        }
    }

    //[StructLayout(LayoutKind.Sequential)]
    public struct TileId
    {
        public int LevelNo;
        public int Row;
        public int Col;
        public int Width;
        public int Height;
        public int Index;

        public override string ToString()
        {
            return string.Format("LevelNo={0},Row={1},Col={2},Width={3},Height={4}",
                LevelNo.ToString(),
                Row.ToString().PadRight(6, ' '),
                Col.ToString().PadRight(6, ' '),
                Width.ToString().PadRight(4, ' '),
                Height.ToString());
        }
    }

    public unsafe class LevelTileComputer
    {
        private int _tileSize;
        private int _width;
        private int _height;
        private LevelId[] _levels = null;

        public LevelTileComputer(int tileSize, int width, int height)
        {
            _tileSize = tileSize;
            _width = width;
            _height = height;
            _levels = ComputerLevels();
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }

        public int TileSize
        {
            get { return _tileSize; }
        }

        public LevelId[] Levels
        {
            get { return _levels; }
        }

        public LevelId GetNearestLevel(float scale)
        {
            float dlt;
            float mindlt = float.MaxValue;
            LevelId lv = new LevelId();
            for(int i=0;i<_levels.Length ;i++)
            {
                dlt = Math.Abs(_levels[i].Scale - scale);
                if (dlt < mindlt)
                {
                    mindlt = dlt;
                    lv = _levels[i];
                }
            }
            return lv;
        }

        private LevelId[] ComputerLevels()
        {
            int levelNo = 0;
            int w = _width, h = _height;
            List<LevelId> levels = new List<LevelId>();
            while (w > _tileSize || h > _tileSize)
            {
                LevelId lv = new LevelId();
                lv.No = levelNo++;
                lv.Width = w;
                lv.Height = h;
                lv.Scale = Math.Max(w / (float)_width, h / (float)_height);
                lv.TileCount = GetTileCount(ref lv, out lv.Tiles);
                w /= 2;
                h /= 2;
                levels.Add(lv);
            }
            if (w <= _tileSize && h <= _tileSize)
            {
                LevelId lv = new LevelId();
                lv.No = levelNo++;
                lv.Width = w;
                lv.Height = h;
                lv.Scale = Math.Max(w / (float)_width, h / (float)_height);
                lv.TileCount = GetTileCount(ref lv, out lv.Tiles);
                levels.Add(lv);
            }
            return levels.Count > 0 ? levels.ToArray() : null;
        }

        private int GetTileCount(ref LevelId lv, out TileId[] tiles)
        {
            int rows = (int)Math.Ceiling(lv.Height / (float)_tileSize);
            int cols = (int)Math.Ceiling(lv.Width / (float)_tileSize);
            tiles = new TileId[rows * cols];
            lv.Rows = rows;
            lv.Cols = cols;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    TileId tile = new TileId();
                    tile.LevelNo = lv.No;
                    if (c == cols - 1)
                        tile.Width = Math.Min(_tileSize, lv.Width - (cols - 1) * _tileSize);
                    else
                        tile.Width = _tileSize;
                    if (r == rows - 1)
                        tile.Height = Math.Min(_tileSize, lv.Height - (rows - 1) * _tileSize);
                    else
                        tile.Height = _tileSize;
                    tile.Row = r;
                    tile.Col = c;
                    tile.Index = r * cols + c;
                    tiles[tile.Index] = tile;
                }
            }
            return rows * cols;
        }
    }
}
