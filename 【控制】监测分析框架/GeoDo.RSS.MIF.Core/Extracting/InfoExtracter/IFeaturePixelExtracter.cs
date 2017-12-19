using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 定量产品生成
    /// </summary>
    /// <typeparam name="TDataType"></typeparam>
    /// <typeparam name="TFeature"></typeparam>
    public interface IFeaturePixelExtracter<TDataType,TFeature>:IExtracter
    {
        void Extract(IPixelFeatureMapper<TFeature> extractedPixels);
    }
}
