using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    internal class FileHelper
    {
        public static IRasterDataProvider Open(string filename)
        {
            return GeoDataDriver.Open(filename) as IRasterDataProvider;
        }

        public static IRasterDataProvider OpenUpdate(string filename)
        {
            return GeoDataDriver.Open(filename, enumDataProviderAccess.Update,null) as IRasterDataProvider;
        }

    }
}
