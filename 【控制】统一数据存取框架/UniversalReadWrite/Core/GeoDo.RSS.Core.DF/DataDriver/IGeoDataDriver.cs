using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public interface IGeoDataDriver:IDisposable
    {
        string Name { get; }
        string FullName { get; }
        IGeoDataProvider Open(string fileName,byte[] header1024,enumDataProviderAccess access, params object[] args);
        IGeoDataProvider Open(string fileName,enumDataProviderAccess access, params object[] args);
        void Delete(string fileName);
    }
}
