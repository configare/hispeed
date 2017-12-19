using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.MEF
{
    public interface IComponentLoader<T>:IDisposable
    {
        enumVersionControlMode LoadMode { get; set; }
        T[] LoadComponents(params string[] dirsOrfiles);
        T[] LoadComponentsByCatalogName(string name);
    }
}
