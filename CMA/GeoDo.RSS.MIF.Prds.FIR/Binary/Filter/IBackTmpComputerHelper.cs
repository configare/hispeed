using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public interface IBackTmpComputerHelper
    {
        void SetBandNo(int visBandIdx, int nearIfrBandIdx, int midIfrBandIdx, int farIfrBandIndex);
        bool VertifyIsCloudPixel(int pixelIdx, int iWndPixel, UInt16[][] wndValues);
        bool VertifyIsWaterPixel(int pixelIdx, int iWndPixel, UInt16[][] wndValues);
        void ResetBandNo();
    }
}
