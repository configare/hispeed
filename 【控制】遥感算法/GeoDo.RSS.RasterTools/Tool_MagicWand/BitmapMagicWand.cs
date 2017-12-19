using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing;

namespace GeoDo.RSS.RasterTools
{
    public class BitmapMagicWand : IBitmapMagicWand, IDisposable
    {
        private StatusRecorder _visitedPixels;
        private Queue<int> _seeds = new Queue<int>();
        private int _pixelWidth = -1;
        private int _width, _height;
        private int _stride;
        private IntPtr _scan0;
        private byte[] _seedColors;
        private int _tolerance;
        private byte[] _targetColor;
        private bool _isNeedFillColor = false;

        public void FillColor(BitmapData pData, Point startPoint, byte tolerance, bool isContinued, byte[] targetColor)
        {
            _targetColor = targetColor;
            _isNeedFillColor = true;
            DoMagicWand(pData, startPoint, tolerance, isContinued, null, null);
        }

        public ScanLineSegment[] ExtractSnappedPixels(BitmapData pData, Point startPoint, byte tolerance, bool isContinued)
        {
            _isNeedFillColor = false;
            List<ScanLineSegment> segs = new List<ScanLineSegment>();
            DoMagicWand(pData, startPoint, tolerance, isContinued,
                (row, bcol, ecol) => { segs.Add(new ScanLineSegment(row, bcol, ecol)); }, null);
            return segs.Count > 0 ? segs.ToArray() : null;
        }

        public ScanLinePolygon[] ExtractSnappedPolygons(BitmapData pData, Point startPoint, byte tolerance, bool isContinued)
        {
            _isNeedFillColor = false;
            List<ScanLinePolygon> plys = new List<ScanLinePolygon>();
            IScanLinePolygonBuilder _plyBuilder = new ScanLinePolygonBuilder();
            DoMagicWand(pData, startPoint, tolerance, isContinued,
                (row, bco, ecol) => { _plyBuilder.AddScanLine(row, bco, ecol); },
                () =>
                {
                    ScanLinePolygon ply = _plyBuilder.Build();
                    if (ply != null)
                        plys.Add(ply);
                });
            return plys.Count > 0 ? plys.ToArray() : null;
        }

        private unsafe void DoMagicWand(BitmapData pdata, Point startPoint, byte tolerance, bool isContinued,
            Action<int, int, int> segmentAction, Action regionAction)
        {
            CheckArgBandFillFields(pdata, tolerance, startPoint);
            _visitedPixels = new StatusRecorder(_width * pdata.Height);
            if (isContinued)
            {
                _seeds.Enqueue(startPoint.Y * _width + startPoint.X);
                FloodFill(segmentAction);
                if (regionAction != null)
                    regionAction();
            }
            else
            {
                int count = _width * _height;
                byte* ptr;
                for (int i = 0; i < count; i++)
                {
                    if (_visitedPixels.IsTrue(i))
                        continue;
                    ptr = (byte*)_scan0 + (i / _width) * _stride + (i % _width) * _pixelWidth;
                    if (ColorIsMatched(ptr))
                    {
                        _seeds.Enqueue(i);
                        FloodFill(segmentAction);
                        if (regionAction != null)
                            regionAction();
                    }
                    else
                        _visitedPixels.SetStatus(i, true);
                }
            }
        }

        private unsafe void CheckArgBandFillFields(BitmapData pdata, byte tolerance, Point startPoint)
        {
            CheckArgsAndGetSeedColor(pdata, startPoint, out _pixelWidth);
            _tolerance = tolerance * tolerance * _pixelWidth;//tolerance * tolerance * 4;
        }

        private unsafe void FloodFill(Action<int, int, int> segmentAction)
        {
            int oRow = 0;
            int oCol = 0;
            byte* ptr0 = (byte*)_scan0;
            byte* oPtr = ptr0;
            byte* ptr;
            int bCol = 0, eCol = 0;
            int seedIdx = 0;
            while (_seeds.Count > 0)
            {
                int oSeedIdx = _seeds.Dequeue();
                if (_visitedPixels.IsTrue(oSeedIdx))
                    continue;
                oRow = oSeedIdx / _width;
                oCol = oSeedIdx % _width;
                oPtr = ptr0 + oRow * _stride + oCol * _pixelWidth;
                //临界像元标记
                bool isCriticalPixel = true;
                bool isEnd = false;
                //—> left
                bCol = oCol;
                if (oCol > 0)
                {
                    ptr = oPtr;
                    seedIdx = oSeedIdx;
                    Dec(ref bCol, ref ptr, ref seedIdx);
                    isEnd = IsToLeftBorder(ptr, bCol);
                    while (!isEnd)
                    {
                        HitPixel(seedIdx, ptr);
                        Dec(ref bCol, ref ptr, ref seedIdx);
                        isEnd = IsToLeftBorder(ptr, bCol);
                        isCriticalPixel = false;
                    }
                    Inc(ref bCol, ref ptr, ref seedIdx);
                    if (isCriticalPixel)
                        HitPixel(seedIdx, ptr);
                    //for debug
                    //if (bCol > 0)
                    //    HitColorGreen(ptr - _pixelWidth);
                }
                //—> right      
                eCol = oCol;
                if (oCol < _width - 1)
                {
                    isCriticalPixel = true;
                    ptr = oPtr;
                    seedIdx = oSeedIdx;
                    Inc(ref eCol, ref ptr, ref seedIdx);
                    isEnd = IsToRightBorder(ptr, eCol);
                    while (!isEnd)
                    {
                        HitPixel(seedIdx, ptr);
                        Inc(ref eCol, ref ptr, ref seedIdx);
                        isEnd = IsToRightBorder(ptr, eCol);
                        isCriticalPixel = false;
                    }
                    Dec(ref eCol, ref ptr, ref seedIdx);
                    if (isCriticalPixel)
                        HitPixel(seedIdx, ptr);
                    //for debug
                    //if (eCol < _width - 1)
                    //    HitColorGreen(ptr + _pixelWidth);
                }
                //
                HitPixel(oSeedIdx, oPtr);
                //
                if (segmentAction != null)
                    segmentAction(oRow, bCol, eCol);
                //—> up    
                if (oRow > 0)
                    UpDownMoveRow(bCol, eCol, oRow, -1, ptr0);
                //—> down    
                if (oRow < _height - 1)
                    UpDownMoveRow(bCol, eCol, oRow, 1, ptr0);
                //
                if (!_visitedPixels.IsTrue(oSeedIdx))
                    _visitedPixels.SetStatus(oSeedIdx, true);
            }
        }

        private unsafe void UpDownMoveRow(int bCol, int eCol, int oRow, int rowStep, byte* ptr0)
        {
            byte* ptr = ptr0 + (oRow + rowStep) * _stride + bCol * _pixelWidth;
            int seedIdx = (oRow + rowStep) * _width + bCol;
            for (int c = bCol; c <= eCol; c++, seedIdx++, ptr += _pixelWidth)
            {
                if (_visitedPixels.IsTrue(seedIdx) || !ColorIsMatched(ptr))
                    continue;
                _seeds.Enqueue(seedIdx);
            }
        }

        private unsafe void Inc(ref int col, ref byte* ptr, ref int seedIdx)
        {
            col += 1;
            ptr += _pixelWidth;
            seedIdx += 1;
        }

        private unsafe void Dec(ref int col, ref byte* ptr, ref int seedIdx)
        {
            col -= 1;
            ptr -= _pixelWidth;
            seedIdx -= 1;
        }

        /*
        private unsafe void HitColorGreen(byte* ptr)
        {
            *ptr = 0;
            *(ptr + 1) = 255;
            *(ptr + 2) = 0;
        }

        private unsafe void HitColorRed(byte* ptr)
        {
            *ptr = 0;
            *(ptr + 1) = 0;
            *(ptr + 2) = 255;
        }
         */

        private unsafe void HitPixel(int seedIdx, byte* ptr)
        {
            if (_isNeedFillColor)
                MarkHitedPixel(ptr);
            _visitedPixels.SetStatus(seedIdx, true);
        }

        private unsafe void MarkHitedPixel(byte* ptr)
        {
            for (int b = 0; b < _pixelWidth; b++)
            {
                *(ptr + _pixelWidth - b - 1) = _targetColor[b];
            }
        }

        private unsafe bool IsToRightBorder(byte* ptr, int col)
        {
            return col >= _width || !ColorIsMatched(ptr);
        }

        private unsafe bool IsToLeftBorder(byte* ptr, int col)
        {
            return col < 0 || !ColorIsMatched(ptr);
        }

        private unsafe bool ColorIsMatched(byte* ptr)
        {
            int sum = 0;
            int diff = 0;
            for (int b = 0; b < _pixelWidth; b++)
            {
                diff = _seedColors[b] - *(ptr + _pixelWidth - b - 1);
                sum += diff * diff;
            }
            return sum <= _tolerance;
        }

        private unsafe void CheckArgsAndGetSeedColor(BitmapData pdata, Point startPoint, out int pixelWidth)
        {
            pixelWidth = -1;
            if (pdata == null)
                throw new ArgumentNullException("pdata");
            _width = pdata.Width;
            _height = pdata.Height;
            _stride = pdata.Stride;
            _scan0 = pdata.Scan0;
            byte* ptr;
            if (pdata.PixelFormat == PixelFormat.Format8bppIndexed)
            {
                pixelWidth = 1;
                ptr = (byte*)_scan0 + startPoint.Y * _stride + startPoint.X * _pixelWidth;
                _seedColors = new byte[] { *ptr };
            }
            else if (pdata.PixelFormat == PixelFormat.Format24bppRgb)
            {
                pixelWidth = 3;
                ptr = (byte*)_scan0 + startPoint.Y * _stride + startPoint.X * _pixelWidth;
                _seedColors = new byte[] { *(ptr + 2), *(ptr + 1), *ptr };
            }
            else if (pdata.PixelFormat == PixelFormat.Format32bppArgb)
            {
                pixelWidth = 4;
                ptr = (byte*)_scan0 + startPoint.Y * _stride + startPoint.X * _pixelWidth;
                _seedColors = new byte[] { *(ptr + 3), *(ptr + 2), *(ptr + 1), *ptr };
            }
            else
                throw new NotSupportedException("pdata.PixelFormat");
        }

        public void Dispose()
        {
            _visitedPixels.Dispose();
            _visitedPixels = null;
            _seeds.Clear();
            _seeds = null;
        }
    }
}
