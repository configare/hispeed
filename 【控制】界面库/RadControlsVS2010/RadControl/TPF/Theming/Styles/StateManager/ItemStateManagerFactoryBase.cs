using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Styles
{
    public abstract class ItemStateManagerFactoryBase
    {
        private ItemStateManagerBase stateManagerInstance;

        public ItemStateManagerBase StateManagerInstance
        {
            get
            {
                if (stateManagerInstance == null)
                {
                    stateManagerInstance = this.CreateStateManager();
                }

                return stateManagerInstance;
            }
        }

        protected abstract ItemStateManagerBase CreateStateManager();
    }
}
