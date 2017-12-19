using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    /// <summary>
    /// 虚拟图像开始点(用于虚拟数据提供者的访问)
    /// </summary>
    public interface IVirtualScan0
    {
        int OffsetX { get; }
        int OffsetY { get; }
        bool IsVirtualScan0 { get; }
        void SetScan0(int offsetX, int offsetY);
        void Reset();
    }
}
