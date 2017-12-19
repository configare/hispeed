using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IServerDataSource:IGridReader,IDataSource,IPersistable
    {
        string Uri { get; set; }
        InstanceIdentify InstanceIdentify { get; set; }
        FetClassIdentify FetClassIdentify { get; set; }
    }
}
