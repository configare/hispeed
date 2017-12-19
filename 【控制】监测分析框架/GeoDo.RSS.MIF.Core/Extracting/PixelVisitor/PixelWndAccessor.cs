using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.MIF.Core
{
    public class PixelWndAccessor<T> : IDisposable
    {
        protected IRasterDataProvider _dataProvider;
        protected int[] _bandNos;
        protected Rectangle _aoiRect;
        protected int _width, _height;
        protected T[][] _buffers;
        protected int _crtCenterRow = -1;
        protected int _crtWndSize = 0;

        public PixelWndAccessor(IRasterDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            _width = _dataProvider.Width;
            _height = _dataProvider.Height;
        }

        public void Reset(int[] bandNos, Rectangle aoiRect)
        {
            _bandNos = bandNos;
            _aoiRect = aoiRect;
            CorrectAOIRect();
        }

        private void CorrectAOIRect()
        {
            int left, right, top, bottom;
            left = Math.Max(0, _aoiRect.Left);
            top = Math.Max(0, _aoiRect.Top);
            right = Math.Min(_aoiRect.Right, _dataProvider.Width);
            bottom = Math.Min(_aoiRect.Bottom, _dataProvider.Height);
            _aoiRect = Rectangle.FromLTRB(left, top, right, bottom);
        }

        public int[] bandNos
        {
            get { return _bandNos; }
        }

        public Rectangle AOIRect
        {
            get { return _aoiRect; }
        }

        public bool ReadWndPixels(int pixelIdx, int wndSize, T[][] wndBuffers)
        {
            int row = pixelIdx / _width;
            int col = pixelIdx - row * _width;
            int halfSize = wndSize / 2;
            if (row < halfSize || row + halfSize >= _height)
                return false;
            if (col < halfSize || col + halfSize >= _width)
                return false;
            if (row != _crtCenterRow || _crtCenterRow == -1 || _crtWndSize != wndSize)
                ReBuildBuffers(row, wndSize);
            int wndPixelIdx = 0;
            if (col < halfSize)
                return false;
            for (int i = 0; i < _bandNos.Length; i++, wndPixelIdx = 0)
            {
                int idx = 0;
                int endrow = 2 * halfSize + 1;
                int endcol = col + halfSize + 1;
                for (int r = 0; r < endrow; r++, idx += _width)
                {
                    for (int c = col - halfSize; c < endcol; c++)
                    {
                        wndBuffers[i][wndPixelIdx++] = _buffers[i][idx + c];
                    }
                }
            }
            return true;
        }

        private void ReBuildBuffers(int row, int wndSize)
        {
            _crtCenterRow = row;
            _crtWndSize = wndSize;
            _buffers = new T[_bandNos.Length][];
            for (int i = 0; i < _bandNos.Length; i++)
            {
                _buffers[i] = new T[_dataProvider.Width * wndSize];
                GCHandle handle = GCHandle.Alloc(_buffers[i], GCHandleType.Pinned);
                _dataProvider.GetRasterBand(_bandNos[i]).Read(
                    0,_crtCenterRow - wndSize / 2,
                    _dataProvider.Width, wndSize, 
                    handle.AddrOfPinnedObject(), _dataProvider.DataType,
                    _dataProvider.Width, wndSize);
                handle.Free();
            }
        }

        public void Dispose()
        {
            _dataProvider = null;
        }
    }
}
