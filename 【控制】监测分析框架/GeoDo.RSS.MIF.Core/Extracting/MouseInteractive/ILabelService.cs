using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface ILabelService
    {
        void Reset();
        ILabelLayer GetLabelLayer(string name, string[] fieldNames);
        ILabelLayer FindLabelLayer(string name);
    }
}
