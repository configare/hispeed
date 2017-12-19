using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.WebDataServer
{
    internal interface IServerInstance:IDisposable
    {
        InstanceIdentify InstanceIdentify { get; }
        ICatalogProvider CatalogProvider { get; }
        IFeaturesReaderService GetFeaturesReaderService(string fetclassId);
    }
}
