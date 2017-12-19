using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.MIF.Core
{
    public class RasterPixelsVisitor<T> : IRasterPixelsVisitor<T>, IDisposable
    {
        protected IArgumentProvider _argProvider;
        protected PixelWndAccessor<T> _wndAccessor;

        public RasterPixelsVisitor(IArgumentProvider argProvider)
        {
            _argProvider = argProvider;
            _wndAccessor = new PixelWndAccessor<T>(argProvider.DataProvider);
        }

        public void VisitPixel(Rectangle aoiRect, int[] aoi, int[] bandNos, Action<int, T[]> extractAction)
        {
            IRasterBand[] bands = GetRasterBands(bandNos);
            T[][] buffers = BuildBuffers(aoiRect, bandNos.Length);
            enumDataType dataType = _argProvider.DataProvider.DataType;
            int width = _argProvider.DataProvider.Width;
            T[] pixelBuffer = new T[bandNos.Length];
            int oIdx = 0, idx = 0;
            GCHandle[] handles = GetHandles(buffers);
            try
            {
                if (aoi == null || aoi.Length == 0)
                {
                    for (int r = aoiRect.Top; r < aoiRect.Bottom; r++, oIdx = r * width, idx = 0)
                    {
                        for (int bIdx = 0; bIdx < bands.Length; bIdx++)
                            bands[bIdx].Read(aoiRect.Left, r, aoiRect.Width, 1, handles[bIdx].AddrOfPinnedObject(), dataType, aoiRect.Width, 1);
                        oIdx += aoiRect.Left;
                        for (int c = aoiRect.Left; c < aoiRect.Width; c++, oIdx++, idx++)
                        {
                            for (int bIdx = 0; bIdx < bands.Length; bIdx++)
                                pixelBuffer[bIdx] = buffers[bIdx][idx];
                            extractAction(oIdx, pixelBuffer);
                        }
                    }
                }
                else
                {
                    Dictionary<int, AOIHelper.AOIRowRange> aoirows = AOIHelper.ComputeRowIndexRange(aoi, new Size(_argProvider.DataProvider.Width, _argProvider.DataProvider.Height));
                    foreach (int r in aoirows.Keys)
                    {
                        for (int bIdx = 0; bIdx < bands.Length; bIdx++)
                            bands[bIdx].Read(aoiRect.Left, r, aoiRect.Width, 1, handles[bIdx].AddrOfPinnedObject(), dataType, aoiRect.Width, 1);
                        idx = 0;
                        AOIHelper.AOIRowRange range = aoirows[r];
                        for (int aoiidx = range.BeginIndex; aoiidx < range.EndIndex; aoiidx++)
                        {
                            oIdx = aoi[aoiidx];
                            int orow = oIdx / width;
                            int ocol = oIdx - orow * width;
                            for (int bIdx = 0; bIdx < bands.Length; bIdx++)
                                pixelBuffer[bIdx] = buffers[bIdx][ocol - aoiRect.Left];
                            extractAction(oIdx, pixelBuffer);
                        }
                    }
                }
            }
            finally
            {
                FreeHandles(handles);
            }
        }

        public void VisitPixel(int[] bandNos, Action<int, T[]> extractAction)
        {
            if (extractAction == null)
                return;
            if (bandNos == null || bandNos.Length == 0)
                bandNos = GetAllBandNo();
            Rectangle rect = AOIHelper.ComputeAOIRect(_argProvider.AOI, new Size(_argProvider.DataProvider.Width, _argProvider.DataProvider.Height));
            int[] aoi = _argProvider.AOI;
            VisitPixel(rect, aoi, bandNos, extractAction);
        }

        public void VisitPixelWnd(Rectangle aoiRect, int[] aoi, int[] bandNos, int[] wndBandNos, int minWndSize, int maxWndSize, Func<int, int, T[], T[][], bool> isNeedIncWndSize, Action<int, int, T[], T[][]> extractAction)
        {
            _wndAccessor.Reset(wndBandNos, aoiRect);
            int crtWndSize = minWndSize;
            IRasterBand[] bands = GetRasterBands(bandNos);
            T[][] buffers = BuildBuffers(aoiRect, bandNos.Length);
            enumDataType dataType = _argProvider.DataProvider.DataType;
            int width = _argProvider.DataProvider.Width;
            T[] pixelBuffer = new T[bandNos.Length];
            T[][] wndBuffers = new T[wndBandNos.Length][];
            BuildWndBuffers(crtWndSize, wndBuffers);
            int oIdx = 0, idx = 0;
            GCHandle[] handles = GetHandles(buffers);
            try
            {
                if (aoi == null || aoi.Length == 0)
                {
                    for (int r = aoiRect.Top; r < aoiRect.Bottom; r++, oIdx = r * width, idx = 0)
                    {
                        for (int bandIdx = 0; bandIdx < bands.Length; bandIdx++)
                            bands[bandIdx].Read(aoiRect.Left, r, aoiRect.Width, 1, handles[bandIdx].AddrOfPinnedObject(), dataType, aoiRect.Width, 1);
                        oIdx += aoiRect.Left;
                        for (int c = aoiRect.Left; c < aoiRect.Width; c++, oIdx++, idx++)
                        {
                            for (int bIdx = 0; bIdx < bands.Length; bIdx++)
                                pixelBuffer[bIdx] = buffers[bIdx][idx];
                            //
                            if (!_wndAccessor.ReadWndPixels(oIdx, crtWndSize, wndBuffers))
                                continue;
                            while (isNeedIncWndSize(oIdx, crtWndSize, pixelBuffer, wndBuffers) && crtWndSize < maxWndSize)
                            {
                                crtWndSize += 2;//1,3,5,7,9,...
                                BuildWndBuffers(crtWndSize, wndBuffers);
                                //目前这里有个bug，扩大窗口后，没有为wndBuffers赋值，但是直接在这里读取的话，程序速度会很慢。
                            }
                            //by chennan 20121226修改火点判识与一期不一致问题
                            crtWndSize = minWndSize;
                            BuildWndBuffers(crtWndSize, wndBuffers);
                            _wndAccessor.ReadWndPixels(oIdx, crtWndSize, wndBuffers);
                            //
                            extractAction(oIdx, crtWndSize, pixelBuffer, wndBuffers);

                        }
                    }
                }
                else
                {
                    Dictionary<int, AOIHelper.AOIRowRange> aoirows = AOIHelper.ComputeRowIndexRange(aoi, new Size(_argProvider.DataProvider.Width, _argProvider.DataProvider.Height));
                    foreach (int r in aoirows.Keys)
                    {
                        for (int bIdx = 0; bIdx < bands.Length; bIdx++)
                            bands[bIdx].Read(aoiRect.Left, r, aoiRect.Width, 1, handles[bIdx].AddrOfPinnedObject(), dataType, aoiRect.Width, 1);
                        idx = 0;
                        AOIHelper.AOIRowRange range = aoirows[r];
                        for (int aoiidx = range.BeginIndex; aoiidx < range.EndIndex; aoiidx++)
                        {
                            oIdx = aoi[aoiidx];
                            int orow = oIdx / width;
                            int ocol = oIdx - orow * width;
                            for (int bIdx = 0; bIdx < bands.Length; bIdx++)
                                pixelBuffer[bIdx] = buffers[bIdx][ocol - aoiRect.Left];
                            //
                            if (!_wndAccessor.ReadWndPixels(oIdx, crtWndSize, wndBuffers))
                                continue;
                            while (isNeedIncWndSize(oIdx, crtWndSize, pixelBuffer, wndBuffers) && crtWndSize < maxWndSize)
                            {
                                crtWndSize += 2;
                                BuildWndBuffers(crtWndSize, wndBuffers);
                                ////by chennan 高温火点窗口数据读取
                                //_wndAccessor.ReadWndPixels(oIdx, crtWndSize-2, wndBuffers);
                                //目前这里有个bug，扩大窗口后，没有为wndBuffers赋值，但是直接在这里读取的话，程序速度会很慢。
                            }
                            //by chennan 20121226修改火点判识与一期不一致问题
                            crtWndSize = minWndSize;
                            BuildWndBuffers(crtWndSize, wndBuffers);
                            _wndAccessor.ReadWndPixels(oIdx, crtWndSize, wndBuffers);
                            //
                            extractAction(oIdx, crtWndSize, pixelBuffer, wndBuffers);
                        }
                    }
                }
            }
            finally
            {
                FreeHandles(handles);
            }
        }

        public void VisitPixelWnd(int[] bandNos, int[] wndBandNos, int minWndSize, int maxWndSize, Func<int, int, T[], T[][], bool> isNeedIncWndSize, Action<int, int, T[], T[][]> extractAction)
        {
            if (extractAction == null || wndBandNos == null || wndBandNos.Length == 0)
                return;
            Rectangle rect = AOIHelper.ComputeAOIRect(_argProvider.AOI, new Size(_argProvider.DataProvider.Width, _argProvider.DataProvider.Height));
            int[] aoi = _argProvider.AOI;
            VisitPixelWnd(rect, aoi, bandNos, wndBandNos, minWndSize, maxWndSize, isNeedIncWndSize, extractAction);
        }

        private void BuildWndBuffers(int wndSize, T[][] wndBuffers)
        {
            for (int i = 0; i < wndBuffers.Length; i++)
                wndBuffers[i] = new T[wndSize * wndSize];
        }

        private void FreeHandles(GCHandle[] handles)
        {
            if (handles == null || handles.Length == 0)
                return;
            foreach (GCHandle h in handles)
                h.Free();
        }

        private GCHandle[] GetHandles(T[][] buffers)
        {
            GCHandle[] handles = new GCHandle[buffers.Length];
            for (int i = 0; i < buffers.Length; i++)
                handles[i] = GCHandle.Alloc(buffers[i], GCHandleType.Pinned);
            return handles;
        }

        private T[][] BuildBuffers(Rectangle rect, int bandCount)
        {
            T[][] buffers = new T[bandCount][];
            for (int i = 0; i < bandCount; i++)
                buffers[i] = new T[rect.Width];
            return buffers;
        }

        private IRasterBand[] GetRasterBands(int[] bandNos)
        {
            IRasterBand[] bands = new IRasterBand[bandNos.Length];
            for (int i = 0; i < bandNos.Length; i++)
                bands[i] = _argProvider.DataProvider.GetRasterBand(bandNos[i]);
            return bands;
        }

        private int[] GetAllBandNo()
        {
            int[] bNos = new int[_argProvider.DataProvider.BandCount];
            for (int b = 1; b <= _argProvider.DataProvider.BandCount; b++)
                bNos[b - 1] = b;
            return bNos;
        }

        public virtual void Dispose()
        {
            _argProvider = null;
        }
    }
}
