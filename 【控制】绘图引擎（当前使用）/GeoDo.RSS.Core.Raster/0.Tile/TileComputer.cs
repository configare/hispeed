using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class TileComputer : ITileComputer
    {
        private int _tileSize;
        private int _sampleRatio;

        public TileComputer(int tileSize, int sampleRatio)//eg:2倍
        {
            _tileSize = tileSize;
            _sampleRatio = sampleRatio;
        }

        public int TileSize
        {
            get { return _tileSize; }
        }

        public int SampleRatio
        {
            get { return _sampleRatio; }
        }

        public TileIdentify[] GetTileIdentifies(int width, int height, out int rowCount, out int colCount)
        {
            int hPlaceholders = GetPlaceholder(width);
            int hPlaceholderLeft = hPlaceholders/2;
            int hPlaceholderRight = hPlaceholderLeft;
            if (hPlaceholders % 2 != 0)
                hPlaceholderRight++;
            //
            int vPlaceholders = GetPlaceholder(height);
            int vPlaceholderTop = vPlaceholders / 2;
            int vPlaceholderBottom = vPlaceholderTop;
            if (vPlaceholders % 2 != 0)
                vPlaceholderBottom++;
            //
            int sizeX = width + hPlaceholderLeft + hPlaceholderRight;
            int sizeY = height + vPlaceholderTop + vPlaceholderBottom;
            rowCount = sizeY / _tileSize;
            colCount = sizeX / _tileSize;
            int beginRow = 0, beginCol = 0, dataWidth = 0, dataHeight = 0;
            int offsetX = 0, offsetY = 0;
            List<TileIdentify> tiles = new List<TileIdentify>();
            beginRow = 0;
            beginCol = 0;
            for (int r = 0; r < rowCount; r++, beginRow = r * _tileSize - vPlaceholderTop, dataHeight = _tileSize, beginCol = 0)
            {
                offsetY = 0;
                dataHeight = _tileSize;
                if (r == 0)
                {
                    offsetY = vPlaceholderTop;
                    dataHeight -= vPlaceholderTop;
                }
                if (r == rowCount - 1)
                {
                    dataHeight -= vPlaceholderBottom;
                }

                for (int c = 0; c < colCount; c++, beginCol = c * _tileSize - hPlaceholderLeft, dataWidth = _tileSize)
                {
                    offsetX = 0;
                    dataWidth = _tileSize;
                    if (c == 0)
                    {
                        offsetX = hPlaceholderLeft;
                        dataWidth -= hPlaceholderLeft;
                    }
                    if (c == colCount - 1)
                    {
                        dataWidth -= hPlaceholderRight;
                    }
                    //
                    TileIdentify tile = new TileIdentify(r, c, offsetX, offsetY, beginRow, beginCol, dataWidth, dataHeight);
                    tiles.Add(tile);
                }
            }
            return tiles.Count > 0 ? tiles.ToArray() : null;
        }

        public int GetPlaceholder(int size)
        {
            return _tileSize - size % _tileSize;
        }

        public Size GetSizeByLevel(int level, int width, int height)
        {
            Size size = new Size(width, height);
            for (int i = 0; i < level; i++)
            {
                size.Width /= _sampleRatio;
                size.Height /= _sampleRatio;
            }
            return size;
        }

        public int GetLevelCount(int width, int height)
        {
            int size = Math.Max(width, height);
            int level = 1;
            size = (int)Math.Ceiling(size / (float)_sampleRatio);
            while (size > _tileSize)
            {
                level++;
                size = (int)Math.Ceiling(size / (float)_sampleRatio);
            }
            if (size > _tileSize / _sampleRatio)
                level++;
            return level;
        }


        public LevelDef[] GetLevelDefs(int width, int height)
        {
            int rowCount = 0, colCount = 0;//not use
            int levelCount = GetLevelCount(width, height);
            List<LevelDef> levels = new List<LevelDef>(levelCount);
            for (int i = 0; i < levelCount; i++)
            {
                Size size = GetSizeByLevel(i, width, height);
                TileIdentify[] tiles = GetTileIdentifies(size.Width, size.Height, out rowCount, out colCount);
                //
                LevelDef level = new LevelDef();
                level.Rows = rowCount;
                level.Cols = colCount;
                level.LevelNo = i;
                level.Scale = Math.Max(size.Width / (float)width, size.Height / (float)height);
                level.Size = size;
                level.TileIdentities = tiles;
                if (tiles != null)
                    foreach (TileIdentify t in tiles)
                        t.Level = level;
                levels.Add(level);
            }
            return levels.Count > 0 ? levels.ToArray() : null;
        }

        public LevelDef GetSuitableLevel(LevelDef[] levels, int wndWidth, int wndHeight, int dataWidth, int dataHeight)
        {
            float scaleX = wndWidth / (float)dataWidth;
            float scaleY = wndHeight / (float)dataHeight;
            float scale = Math.Min(scaleX, scaleY);
            float dlt = 0;
            float minDlt = float.MaxValue;
            LevelDef retLevel = new LevelDef();
            foreach (LevelDef level in levels)
            {
                dlt = Math.Abs(level.Scale - scale);
                if (dlt < minDlt)
                {
                    retLevel = level;
                    minDlt = dlt;
                }
            }
            return retLevel;
        }
    }
}
