using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public interface IOpenFileProcessor
    {
        bool IsSupport(string fname,string extName);
        bool Open(string fname,out bool memoryIsNotEnough);
        void SetSession(ISmartSession session);
    }
}
