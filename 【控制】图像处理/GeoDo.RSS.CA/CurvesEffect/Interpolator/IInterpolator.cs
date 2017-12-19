using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.CA
{
    /// <summary>
    /// 差值算法
    /// </summary>
    public interface IInterpolator
    {
        int Count { get; }
        void Add(double x, double y);
        void Clear();
        double Interpolate(double x);
    }
}
