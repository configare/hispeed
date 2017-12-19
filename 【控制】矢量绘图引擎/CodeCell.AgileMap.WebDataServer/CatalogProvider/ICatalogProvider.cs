using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.WebDataServer
{
    internal interface ICatalogProvider:IDisposable
    {
        FetDatasetIdentify[] GetFetDatasetIdentify();
        FetClassIdentify[] GetFetClassIdentify();
        FetClassProperty GetFetClassProperty(string fetclassId);
    }
}
