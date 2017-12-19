using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;

namespace GeoDo.RSS.MIF.Core
{
    public interface IPixelIndexMapper : IExtractResult, IDisposable, IExtractResultBase
    {
        /// <summary>
        /// 记录用户自定义数据（例如：火点特征数组）
        /// </summary>
        object Tag { get; set; }
        Size Size { get; }
        CoordEnvelope CoordEnvelope { get; }
        ISpatialReference SpatialRef { get; }
        /// <summary>
        /// 判断是否为空
        /// </summary>
        /// <returns></returns>
        bool IsEmpty();
        int Count { get; }
        /// <summary>
        /// 获取像元索引
        /// </summary>
        IEnumerable<int> Indexes { get; }
        /// <summary>
        /// 添加像元索引
        /// </summary>
        /// <param name="index"></param>
        void Put(int index);
        /// <summary>
        /// 添加像元索引
        /// </summary>
        /// <param name="indexes"></param>
        void Put(int[] indexes);
        /// <summary>
        /// 移除像元索引
        /// </summary>
        /// <param name="indexes"></param>
        void Remove(int[] indexes);
        /// <summary>
        /// 重新初始化
        /// </summary>
        void Reset();
        void SetDispaly(bool display);
        void SetOutIdentify(string outIdentify);
        bool Get(int index);
    }
}
