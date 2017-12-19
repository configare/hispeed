using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IFileDataSource : IGridReader, IPersistable,IDataSource
    {
        string FileUrl { get; set; }
    }
}
