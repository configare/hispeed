using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IValueExtractResult : IExtractResult, IExtractResultBase
    {
        double Value { get; }
        void SetDispaly(bool display);
        void SetOutIdentify(string outIdentify);
    }
}
