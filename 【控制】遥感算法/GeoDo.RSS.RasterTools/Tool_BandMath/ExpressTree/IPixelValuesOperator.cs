using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.RasterTools
{
    public interface IPixelValuesOperator<T>
    {
        Func<T[], float> GetOperatorFunc();
    }
}
