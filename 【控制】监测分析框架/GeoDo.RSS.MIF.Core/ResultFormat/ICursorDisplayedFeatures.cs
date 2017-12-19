using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    /// <summary>
    /// 可支持光标位置信息显示的特征集合
    /// </summary>
    public interface ICursorDisplayedFeatures
    {
        string Name { get; }
        string GetCursorInfo(int pixelIndex);
    }
}
