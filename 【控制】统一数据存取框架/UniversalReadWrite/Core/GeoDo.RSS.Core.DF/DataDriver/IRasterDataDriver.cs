using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public interface IRasterDataDriver:IGeoDataDriver
    {
        IRasterDataProvider Create(string fileName, int xSize, int ySize, int bandCount, enumDataType dataType, params object[] options);
        IRasterDataProvider CreateCopy(string fileName, IRasterDataProvider dataProvider, params object[] options);
    }
}
