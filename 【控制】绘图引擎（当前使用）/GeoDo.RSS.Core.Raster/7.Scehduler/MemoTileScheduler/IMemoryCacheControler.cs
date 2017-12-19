using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public enum enumCacheType
    { 
        All,
        Nearest3Level
    }

    public interface IMemoryCacheControler
    {
        enumCacheType CacheType { get; }
        Rectangle GetCacheWindow(int levelNo);
        void GetNearLevels(int levelNo, out int lowLevelNo, out int highLevelNo);
        void ReduceMemory();
    }
}
