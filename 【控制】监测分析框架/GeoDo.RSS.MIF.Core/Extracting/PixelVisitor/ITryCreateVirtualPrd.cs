using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface ITryCreateVirtualPrd
    {
       IVirtualRasterDataProvider CreateVirtualRasterPRD(ref Dictionary<string, FilePrdMap> filePrdMap);
    }
}
