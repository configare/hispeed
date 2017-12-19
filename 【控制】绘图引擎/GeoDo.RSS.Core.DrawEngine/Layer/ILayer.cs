using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface ILayer:IDisposable
    {
        string Name { get; }
        string Alias { get; }
        bool Enabled { get; set; }
    }
}
