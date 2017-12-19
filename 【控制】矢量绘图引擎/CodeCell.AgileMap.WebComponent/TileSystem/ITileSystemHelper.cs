using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CodeCell.AgileMap.WebComponent
{
    public interface ITileSystemHelper
    {
        /// <summary>
        /// Determines the ground resolution (in meters per pixel) at a specified
        /// latitude and level of detail.
        /// </summary>
        /// <param name="latitude">Latitude (in degrees) at which to measure the
        /// ground resolution.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>The ground resolution, in meters per pixel.</returns>
        double GetGroundResolution(double latitude, int levelOfDetail);

        /// <summary>
        /// Determines the map width and height (in pixels) at a specified level
        /// of detail.
        /// </summary>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>The map width and height in pixels.</returns>
        uint GetMapSize(int levelOfDetail);
        
        /// <summary>
        /// 计算请求矩形范围最接近的显示级别
        /// </summary>
        /// <param name="requestSize">请求坐标范围(例如：鼠标框选)</param>
        /// <returns>合适的显示级别</returns>
        int GetLevelOfDetail(Size requestSize);

        /// <summary>
        /// 计算在指定显示级别下请求坐标范围内的Tiles
        /// </summary>
        /// <param name="levelOfDetail">显示级别</param>
        /// <param name="requestRect">请求坐标范围</param>
        /// <returns>合适的Tile序号</returns>
        TileDef[] GetTiles(int levelOfDetail, Rect requestRect,out int width,out int height);

        /// <summary>
        /// Converts tile XY coordinates into a QuadKey at a specified level of detail.
        /// </summary>
        /// <param name="tileX">Tile X coordinate.</param>
        /// <param name="tileY">Tile Y coordinate.</param>
        /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
        /// to 23 (highest detail).</param>
        /// <returns>A string containing the QuadKey.</returns>
        string TileXYToQuadKey(uint tileX, uint tileY, int levelOfDetail);
        
        /// <summary>
        /// Converts a QuadKey into tile XY coordinates.
        /// </summary>
        /// <param name="quadKey">QuadKey of the tile.</param>
        /// <param name="tileX">Output parameter receiving the tile X coordinate.</param>
        /// <param name="tileY">Output parameter receiving the tile Y coordinate.</param>
        /// <param name="levelOfDetail">Output parameter receiving the level of detail.</param>
        void QuadKeyToTileXY(string quadKey, out int tileX, out int tileY, out int levelOfDetail);
    }
}
