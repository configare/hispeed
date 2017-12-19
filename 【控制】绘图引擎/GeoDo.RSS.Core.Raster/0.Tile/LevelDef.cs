using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public struct LevelDef
    {
        public int LevelNo;//从0开始
        public Size Size;
        public float Scale;//本级缩放比
        public TileIdentify[] TileIdentities;
        public int Rows;  //本级的瓦片行数
        public int Cols;   //本级的瓦片列数

        public bool IsEmpty()
        {
            return LevelNo == 0 && Size.IsEmpty && TileIdentities == null;
        }

        /// <summary>
        /// 根据视窗的坐标范围，计算本级中可视的瓦片
        /// </summary>
        /// <param name="beginRow"></param>
        /// <param name="beginCol"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public TileIdentify[] GetTileIdentifiesByOriginal(int beginRow, int beginCol, int width, int height)
        {
            List<TileIdentify> tiles = new List<TileIdentify>();
            foreach (TileIdentify tile in TileIdentities)
            {
                if ((tile.BeginCol + tile.Width) / Scale < beginCol ||
                    (tile.BeginRow + tile.Height) / Scale < beginRow ||
                    (tile.BeginCol / Scale > beginCol + width) ||
                    (tile.BeginRow / Scale > beginRow + height))
                    continue;
                tiles.Add(tile);
            }
            return tiles.Count > 0 ? tiles.ToArray() : null;
        }

        /// <summary>
        /// 获取瓦片数据块对应在原始图像(1:1)中的像素坐标位置
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="beginRow"></param>
        /// <param name="beginCol"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void GetOriginalRowColByDataBlock(TileIdentify tile, ref int beginRow, ref int beginCol, ref int width, ref int height)
        {
            beginRow = (int)(tile.BeginRow / Scale);
            beginCol = (int)(tile.BeginCol / Scale);
            width = (int)(tile.Width / Scale);
            height = (int)(tile.Height / Scale);
        }

        /// <summary>
        /// 获取瓦片原始图像(1:1)中的像素坐标位置
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="beginRow"></param>
        /// <param name="beginCol"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void GetOriginalRowCol(int tileSize, TileIdentify tile, ref int beginRow, ref int beginCol, ref int width, ref int height)
        {
            beginRow = (int)(tile.BeginRow / Scale - tile.OffsetY / Scale);
            beginCol = (int)(tile.BeginCol / Scale - tile.OffsetX / Scale);
            width = (int)(tileSize / Scale);
            height = (int)(tileSize / Scale);
        }

        /// <summary>
        /// 获取视窗范围内指定行列号位置周围的瓦片
        /// </summary>
        /// <param name="tileSize">瓦片大小</param>
        /// <param name="centerRow">指定行号</param>
        /// <param name="centerCol">指定列号</param>
        /// <param name="wndSize">视窗大小</param>
        /// <returns></returns>
        public TileIdentify[] GetAroundTiles(int tileSize, int centerRow, int centerCol, Size wndSize)
        {
            int bRow = centerRow, eRow = centerRow;
            int bCol = centerCol, eCol = centerCol;
            if (wndSize.Height > tileSize)
            {
                while ((eRow - bRow + 1) * tileSize < wndSize.Height)
                {
                    eRow++;
                    bRow--;
                }
            }
            if (wndSize.Width > tileSize)
            {
                while ((eCol - bCol + 1) * tileSize < wndSize.Width)
                {
                    eCol++;
                    bCol--;
                }
            }
            List<TileIdentify> tiles = new List<TileIdentify>();
            for (int r = bRow; r <= eRow; r++)
            {
                for (int c = bCol; c < eCol; c++)
                {
                    TileIdentify t = GetTileByRowCol(r, c);
                    if (t != null)
                        tiles.Add(t);
                }
            }
            return tiles.Count > 0 ? tiles.ToArray() : null;
        }

        public TileIdentify GetTileByRowCol(int row, int col)
        {
            foreach (TileIdentify tile in TileIdentities)
                if (tile.Row == row && tile.Col == col)
                    return tile;
            return null;
        }

        public override string ToString()
        {
            return "{" + string.Format(
                "LevelNo:{0},Size:{1},Scale:{2},TileCount:{3}", LevelNo, Size, Scale.ToString("0.####"), (TileIdentities != null ? TileIdentities.Length : 0)
                ) + "}";
        }
    }
}
