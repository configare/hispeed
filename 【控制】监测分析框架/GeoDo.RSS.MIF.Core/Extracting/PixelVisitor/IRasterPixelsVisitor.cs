using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.MIF.Core
{
    public interface IRasterPixelsVisitor<T>:IDisposable
    {
        void VisitPixel(int[] bandNos, Action<int, T[]> extractAction);
        void VisitPixelWnd(int[] bandNos, int[] wndBandNos, int minWndSize, int maxWndSize, Func<int, int, T[], T[][], bool> isNeedIncWndSize, Action<int, int, T[], T[][]> extractAction);
        void VisitPixel(Rectangle aoiRect,int[] aoi,int[] bandNos,Action<int,T[]> extractAction);
        void VisitPixelWnd(Rectangle aoiRect,int[] aoi,int[] bandNos, int[] wndBandNos, int minWndSize, int maxWndSize, Func<int, int, T[], T[][], bool> isNeedIncWndSize, Action<int, int, T[], T[][]> extractAction);
    }
}
