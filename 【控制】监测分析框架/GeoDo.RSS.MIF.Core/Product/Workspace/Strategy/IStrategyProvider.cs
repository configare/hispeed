using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IStrategyProvider
    {
        bool IsOK(DateTime dt);
        bool IsOK(string filename);
    }
}
