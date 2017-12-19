using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Layout
{
    public interface IGxdVectorHost:IGxdItem
    {
        object McdFileContent { get; }
    }
}
