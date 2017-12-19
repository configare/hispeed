using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;

namespace GeoDo.RSS.MIF.Core
{
    public interface IPixelFeatureMapper<T> : IExtractResult, IDisposable, IExtractResultBase
    {
        Size Size { get; }
        CoordEnvelope CoordEnvelope { get; }
        ISpatialReference SpatialRef { get; }
        int Count { get; }
           /// <summary>
        /// 获取像元索引
        /// </summary>
        IEnumerable<int> Indexes { get; }
        /// <summary>
        /// 根据自然索引获取像元特征值
        /// </summary>
        /// <param name="index">0..Indexes.Length</param>
        /// <returns>像元特征值</returns>
        T GetValueByIndex(int index);
        /// <summary>
        /// 添加像元
        /// </summary>
        /// <param name="index">像元在全图中的索引</param>
        /// <param name="feature">像元特征</param>
        void Put(int index, T feature);
        void SetDispaly(bool display);
        void SetOutIdentify(string outIdentify);
    }
}
