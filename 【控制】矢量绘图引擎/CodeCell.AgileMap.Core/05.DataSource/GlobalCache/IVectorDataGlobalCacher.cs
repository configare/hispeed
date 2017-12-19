using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    /// <summary>
    /// 矢量数据全局缓存接口
    /// </summary>
    public interface IVectorDataGlobalCacher:IDisposable
    {
        void SetGllPrjChecker(Func<bool> isGllPrj);
        //活动视窗坐标为等经纬度
        bool IsGllPrjOfActiveViewer { get; }
        bool IsEnabled { get; set; }
        ICachedVectorData GetData(string shpFileName);
        ICachedVectorData Request(string shpFileName);
        void Release(string identify);
        //
        void PutFeatures(ICachedFeatures features);
        ICachedFeatures GetFeatures(string identify);
    }
}
