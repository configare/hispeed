using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Core
{
    public interface IPersistContextEnvironment
    {
        void Reset();
        void Put(string varName, object varValue);
        object Get(string varName);
    }
}
