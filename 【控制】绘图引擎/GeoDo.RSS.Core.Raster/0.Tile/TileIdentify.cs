using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public class TileIdentify
    {
        private int _row;        //本级内的行号   
        private int _col;        //本级内的列号   
        private int _offsetX;    //数据块在瓦片中的列偏移
        private int _offsetY;    //数据块在瓦片中的行偏移
        private int _beginRow;   //raster coord,本级全图中的行偏移
        private int _beginCol;   //raster coord,本级全图中的列偏移
        private int _width;      //pixel count
        private int _height;     //pixel count  
        private LevelDef _level;
        private int _tileNo = 1009999991;//1+TileNo{2}+Row{3}+Col{3}

        public TileIdentify(int row, int col, int offsetX, int offsetY, int beginRow, int beginCol, int width, int height)
        {
            _row = row;
            _col = col;
            _offsetX = offsetX;
            _offsetY = offsetY;
            _beginRow = beginRow;
            _beginCol = beginCol;
            _width = width;
            _height = height;
            _level = new LevelDef();
            UpdateTileNo();
        }

        public int TileNo
        {
            get { return _tileNo; }
        }

        public int Row
        {
            get { return _row; }
        }

        public int Col
        {
            get { return _col; }
        }

        private void UpdateTileNo()
        {
            _tileNo = GetTileNo(_level.LevelNo, _row, _col);
        }

        public static int GetTileNo(int levelNo, int row, int col)
        {
            return int.Parse("1" + levelNo.ToString().PadLeft(2, '0') +
                row.ToString().PadLeft(3, '0') + col.ToString().PadLeft(3, '0')+"1");
        }

        public LevelDef Level
        {
            get { return _level; }
            set 
            {
                _level = value;
                UpdateTileNo();
            }
        }

        /// <summary>
        /// 数据块在瓦片中的列偏移
        /// </summary>
        public int OffsetX
        {
            get { return _offsetX; }
        }

        /// <summary>
        /// 数据块在瓦片中的行偏移
        /// </summary>
        public int OffsetY
        {
            get { return _offsetY; }
        }

        /// <summary>
        /// raster coord,本级全图中的行偏移
        /// </summary>
        public int BeginRow
        {
            get { return _beginRow; }
        }

        /// <summary>
        /// raster coord,本级全图中的列偏移
        /// </summary>
        public int BeginCol
        {
            get { return _beginCol; }
        }

        /// <summary>
        /// pixel count,数据块的宽度(非瓦片宽度)
        /// </summary>
        public int Width
        {
            get { return _width; }
        }

        /// <summary>
        /// pixel count,数据块的高度(非瓦片高度)
        /// </summary>
        public int Height
        {
            get { return _height; }
        }

        public override string ToString()
        {
            int hostwidth = 5;
            return string.Format("Row = {0},Col={1},BeginRow = {2} , BeginCol = {3} , Width = {4} , Height = {5} , OffsetX = {6} , OffsetY = {7}",
                _row.ToString().PadRight(3, ' '),
                _col.ToString().PadRight(3, ' '),
                _beginRow.ToString().PadRight(hostwidth, ' '),
                _beginCol.ToString().PadRight(hostwidth, ' '),
                _width.ToString().PadRight(hostwidth, ' '),
                _height.ToString().PadRight(hostwidth, ' '),
                _offsetX.ToString().PadRight(hostwidth, ' '),
                _offsetY.ToString().PadRight(hostwidth, ' ')
                );
        }
    }
}
