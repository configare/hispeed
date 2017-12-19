using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 定性产品生成
    /// </summary>
    public interface IInterestedPixelExtracter : IExtracter, IDisposable
    {
        void Reset(IArgumentProvider argProvider, int[] visitBandNos, string express);
        void Extract(IPixelIndexMapper extractedPixels);
    }
}
