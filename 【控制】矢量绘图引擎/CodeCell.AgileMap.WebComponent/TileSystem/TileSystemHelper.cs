using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CodeCell.AgileMap.WebComponent
{
    public class TileSystemHelper:ITileSystemHelper
    {
        private const double EarthRadius = 6378137;
        private const double MinLatitude = -85.05112878;
        private const double MaxLatitude = 85.05112878;
        private const double MinLongitude = -180;
        private const double MaxLongitude = 180;
        private Rect _mapFullRect = Rect.Empty;
        private int _maxLevelOfDetail = 20;
        private Size _tileSize = Size.Empty;

        public TileSystemHelper(Rect mapFullRect, Size tileSize, int maxLevelOfDetail)
        {
            _mapFullRect = mapFullRect;
            _maxLevelOfDetail = maxLevelOfDetail;
            _tileSize = tileSize;
        }

        /// <summary>
        /// Clips a number to the specified minimum and maximum values.
        /// </summary>
        /// <param name="n">The number to clip.</param>
        /// <param name="minValue">Minimum allowable value.</param>
        /// <param name="maxValue">Maximum allowable value.</param>
        /// <returns>The clipped value.</returns>
        private double Clip(double n, double minValue, double maxValue)
        {
            return Math.Min(Math.Max(n, minValue), maxValue);
        }

        /// <summary>
        /// Determines the ground resolution (in meters per pixel) at a specified
        /// latitude and level of detail.
        /// </summary>
        /// <param name="latitude">Latitude (in degrees) at which to measure the
        /// ground resolution.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>The ground resolution, in meters per pixel.</returns>
        public double GetGroundResolution(double latitude, int levelOfDetail)
        {
            latitude = Clip(latitude, MinLatitude, MaxLatitude);
            return Math.Cos(latitude * Math.PI / 180) * 2 * Math.PI * EarthRadius / GetMapSize(levelOfDetail);
        }

        /// <summary>
        /// Determines the map width and height (in pixels) at a specified level
        /// of detail.
        /// </summary>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>The map width and height in pixels.</returns>
        public uint GetMapSize(int levelOfDetail)
        {
            return (uint)256 << levelOfDetail;
        }

        /// <summary>
        /// 计算请求矩形范围最接近的显示级别
        /// </summary>
        /// <param name="requestSize">请求坐标范围(例如：鼠标框选)</param>
        /// <param name="fullSize">地图全图的坐标范围</param>
        /// <param name="maxLevelOfDetail">最大显示级别</param>
        /// <returns>合适的显示级别</returns>
        public int GetLevelOfDetail(Size requestSize)
        {
            float w = (float)Math.Max(requestSize.Width, requestSize.Height);
            float factor = (float)(w / _mapFullRect.Width);
            /*
             * factor    LevelOfDetail
             *   1            1
             *  1/2           2
             *  1/4           3
             *  1/8           4
             */
            double minDlt = double.MaxValue;
            int retLevelOfDetail = 1;
            for (int i = 1, basefactor = 1; i < _maxLevelOfDetail; i++, basefactor *= 2)
            {
                double dlt = Math.Abs(factor - 1d / basefactor);
                if (dlt < minDlt)
                {
                    minDlt = dlt;
                    retLevelOfDetail = i;
                }
            }
            return retLevelOfDetail;
        }

        public TileDef[] GetTiles(int levelOfDetail, Rect requestRect,out int width,out int height)
        {
            uint tileCountOfWidth = GetMapSize(levelOfDetail) / (uint)_tileSize.Width;
            float resTileX = (float)(_mapFullRect.Width / tileCountOfWidth);
            float resTileY = (float)(_mapFullRect.Height / tileCountOfWidth);
            uint bx = 0;
            if (requestRect.X > _mapFullRect.X)
                bx = (uint)((requestRect.X - _mapFullRect.X) / resTileX);
            uint by = 0;
            if (requestRect.Y > _mapFullRect.Y)
                by = (uint)((requestRect.Y - _mapFullRect.Y) / resTileY);
            uint offset = 1;
            if (levelOfDetail == 1)
                offset = 0;
            uint ex = bx + offset + (uint)Math.Ceiling(Math.Min(requestRect.Width, _mapFullRect.Width) / resTileX);
            uint ey = by + offset + (uint)Math.Ceiling(Math.Min(requestRect.Height, _mapFullRect.Height) / resTileY);
            width = (int)((ex - bx) * _tileSize.Width);
            height = (int)((ey - by) * _tileSize.Height);
            List<TileDef> tiles = new List<TileDef>();
            for (uint c = bx; c < ex; c++)
            {
                for (uint r = by; r < ey; r++)
                {
                    string quadkey = TileXYToQuadKey(c, r, levelOfDetail);
                    Rect rect = new Rect(_mapFullRect.X + c * resTileX, _mapFullRect.Y + r * resTileY, resTileX, resTileY);
                    TileDef tile = new TileDef(r, c, rect, quadkey);
                    tiles.Add(tile);
                }
            }
            return tiles.Count > 0 ? tiles.ToArray() : null;
        }

        /// <summary>
        /// Converts tile XY coordinates into a QuadKey at a specified level of detail.
        /// </summary>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>A string containing the QuadKey.</returns>
        public string TileXYToQuadKey(uint tileX, uint tileY, int levelOfDetail)
        {
            StringBuilder quadKey = new StringBuilder();
            for (int i = levelOfDetail; i > 0; i--)
            {
                char digit = '0';
                int mask = 1 << (i - 1);
                if ((tileX & mask) != 0)
                {
                    digit++;
                }
                if ((tileY & mask) != 0)
                {
                    digit++;
                    digit++;
                }
                quadKey.Append(digit);
            }
            return quadKey.ToString();
        }

        /// <summary>
        /// Converts a QuadKey into tile XY coordinates.
        /// </summary>
        /// <param name="quadKey">QuadKey of the tile.</param>
        /// <param name="tileX">Output parameter receiving the tile X coordinate.</param>
        /// <param name="tileY">Output parameter receiving the tile Y coordinate.</param>
        /// <param name="levelOfDetail">Output parameter receiving the level of detail.</param>
        public void QuadKeyToTileXY(string quadKey, out int tileX, out int tileY, out int levelOfDetail)
        {
            tileX = tileY = 0;
            levelOfDetail = quadKey.Length;
            for (int i = levelOfDetail; i > 0; i--)
            {
                int mask = 1 << (i - 1);
                switch (quadKey[levelOfDetail - i])
                {
                    case '0':
                        break;
                    case '1':
                        tileX |= mask;
                        break;
                    case '2':
                        tileY |= mask;
                        break;
                    case '3':
                        tileX |= mask;
                        tileY |= mask;
                        break;
                    default:
                        throw new ArgumentException("Invalid QuadKey digit sequence.");
                }
            }
        }
    }
}
