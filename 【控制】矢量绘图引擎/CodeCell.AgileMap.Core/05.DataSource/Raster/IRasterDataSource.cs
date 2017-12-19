using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IRasterDataSource:IDataSource
    {
        IRasterReader Reader { get; }
        string Url { get; }
    }
}
