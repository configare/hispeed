using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.WebDataServer
{
    internal class ServerInstanceForFile:ServerInstanceBase
    {
        public ServerInstanceForFile(InstanceIdentify instanceIdentify, ICatalogProvider catalogProvider,string args)
            : base(instanceIdentify, catalogProvider,args)
        { 
        }

        public override IFeaturesReaderService GetFeaturesReaderService(string fetclassId)
        {
            throw new NotImplementedException();
        }
    }
}
