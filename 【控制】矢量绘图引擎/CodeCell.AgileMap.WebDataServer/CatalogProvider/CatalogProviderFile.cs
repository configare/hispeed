using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.WebDataServer
{
    internal class CatalogProviderFile : ICatalogProvider
    {
        public CatalogProviderFile(string filepath)
        {
        }

        #region ICatalogProvider Members

        public FetDatasetIdentify[] GetFetDatasetIdentify()
        {
            throw new NotImplementedException();
        }

        public FetClassIdentify[] GetFetClassIdentify()
        {
            throw new NotImplementedException();
        }

        public FetClassProperty GetFetClassProperty(string fetclassId)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
