using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public unsafe class WindowBitmapBuilder : IWindowBitmapBuilder
    {
        private Bitmap _bitmap;
        private Bitmap _bitmapBuffer;
        private Size _size;
        private LevelTileComputer _tileComputer;
        //private byte[] _tileBuffer;

        public WindowBitmapBuilder(Size wndSize, LevelTileComputer tileComputer)
        {
            _size = wndSize;
            _tileComputer = tileComputer;
            //_tileBuffer = new byte[_tileComputer.TileSize * _tileComputer.TileSize * 3];
            BuildBitmap();
        }

        public void UpdataWndSize(Size wndSize)
        {
            _size = wndSize;
            ReBuildBitmap();
        }

        private void ReBuildBitmap()
        {
            _bitmap.Dispose();
            _bitmapBuffer.Dispose();
            BuildBitmap();
        }

        private void BuildBitmap()
        {
            _bitmap = new Bitmap(_size.Width, _size.Height, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0, 0, _bitmap.Width, _bitmap.Height);
            BitmapData pData = _bitmap.LockBits(rect, ImageLockMode.ReadWrite, _bitmap.PixelFormat);
            _bitmapBuffer = new Bitmap(rect.Width, rect.Height, pData.Stride, pData.PixelFormat, pData.Scan0);
        }

        public void Build(TileData[][] tiles, Rectangle rasterWindowEnvelope)
        {
            if (tiles == null || tiles.Length == 0 || rasterWindowEnvelope.IsEmpty)
                return;
            int bandCount = tiles.Length;
            int tCount = tiles[0].Length;
            for (int bIdx = 1; bIdx < bandCount; bIdx++)
                if (tCount != tiles[bIdx].Length)
                    throw new ArgumentException("每个波段的瓦片数不同,无法生成位图!");
            int bRow, bCol,eRow,eCol;
            GetBeginRowCol(tiles[0], out bRow, out bCol,out eRow,out eCol);
            _bitmapBuffer.Dispose();
            _bitmapBuffer = new Bitmap((eCol - bCol) * _tileComputer.TileSize,
                (eRow - bRow) * _tileComputer.TileSize, PixelFormat.Format24bppRgb);
            _bitmap = _bitmapBuffer;
            using (Graphics g = Graphics.FromImage(_bitmapBuffer))
            {
                g.Clear(Color.Black);
                TileData[] datas = new TileData[bandCount];
                for (int t = 0; t < tCount; t++)
                {
                    for (int bIdx = 0; bIdx < bandCount; bIdx++)
                        datas[bIdx] = tiles[bIdx][t];
                    BuildOneTile(datas, rasterWindowEnvelope, g,bRow ,bCol);
                }
            }
        }

        private void GetBeginRowCol(TileData[] tileData, out int bRow, out int bCol,out int eRow,out int eCol)
        {
            bRow = int.MaxValue;
            bCol = int.MaxValue;
            eRow = int.MinValue;
            eCol = int.MinValue;
            foreach (TileData td in tileData)
            {
                bRow = Math.Min(td.Tile.Row, bRow);
                bCol = Math.Min(td.Tile.Col, bCol);
                eRow = Math.Max(td.Tile.Row, eRow);
                eCol = Math.Max(td.Tile.Col, eCol);
            }
        }

        private void BuildOneTile(TileData[] datas, Rectangle rasterWindowEnvelope, Graphics g,int bRow,int bCol)
        {
            LevelId lv = _tileComputer.Levels[datas[0].Tile.LevelNo];
            int tileSize = _tileComputer.TileSize;
            int y = (int)(tileSize * (datas[0].Tile.Row - bRow));
            int x = (int)(tileSize * (datas[0].Tile.Col - bCol));
            if (datas.Length == 1)
                BuildOneTile(datas[0], x, y, rasterWindowEnvelope, g);
            else if (datas.Length == 3)
                BuildOneTile(datas[0], datas[1], datas[2], x, y, rasterWindowEnvelope, g);
        }

        private void BuildOneTile(TileData redData, TileData greenData, TileData blueData, int x, int y, Rectangle rasterWindowEnvelope, Graphics g)
        {
            LevelId lv = _tileComputer.Levels[redData.Tile.LevelNo];
            int w = redData.Tile.Width;
            int h = redData.Tile.Height;
            Bitmap tBitmap = new Bitmap(w, h, PixelFormat.Format24bppRgb);
            using (IBitmapBuilder<byte> builder = BitmapBuilderFactory.CreateBitmapBuilderByte())
            {
                builder.Build(w, h, redData.Data as byte[],
                    greenData.Data as byte[],
                    blueData.Data as byte[], ref tBitmap);
            }
            g.DrawImage(tBitmap, x, y);
            tBitmap.Dispose();
        }

        private void BuildOneTile(TileData grayData, int x, int y, Rectangle rasterWindowEnvelope, Graphics g)
        {
            throw new NotImplementedException();
        }

        public Bitmap Bitmap
        {
            get { return _bitmap; }
        }

        public void Dispose()
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
        }
    }
}
