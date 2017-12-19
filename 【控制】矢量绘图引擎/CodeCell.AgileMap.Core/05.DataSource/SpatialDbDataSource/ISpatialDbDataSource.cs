using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface ISpatialDbDataSource:IGridReader,IDataSource,IPersistable
    {
        string ConnectionString { get; set; }
        bool AllowReadAllAtFirst { get; set; }
    }
}
