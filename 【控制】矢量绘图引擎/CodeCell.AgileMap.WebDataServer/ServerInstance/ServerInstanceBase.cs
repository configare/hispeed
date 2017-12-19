using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.WebDataServer
{
    internal  abstract class ServerInstanceBase:IServerInstance
    {
        protected InstanceIdentify _id = null;
        protected ICatalogProvider _catalogProvider = null;
        protected string _args = null;

        public ServerInstanceBase(InstanceIdentify id,ICatalogProvider catalogProvider,string args)
        {
            _id = id;
            _catalogProvider = catalogProvider;
            _args = args;
        }

        [DisplayName("实例标识")]
        public InstanceIdentify Id 
        {
            get { return _id; }
        }

        #region IServerInstance Members

        public InstanceIdentify InstanceIdentify
        {
            get { return _id; }
        }

        public ICatalogProvider CatalogProvider
        {
            get { return _catalogProvider; }
        }

         public abstract IFeaturesReaderService GetFeaturesReaderService(string fetclassId);

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_catalogProvider != null)
            {
                _catalogProvider.Dispose();
                _catalogProvider = null;
            }
        }

        #endregion
    }
}
