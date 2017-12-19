using System;
using System.Collections.Generic;
using System.Text;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.Catalog
{
    public class CommandProviderCatalog:ICommandProvider
    {
        protected IToolbar[] _toolbars = null;
        protected IMenu[] _menus = null;

        public CommandProviderCatalog()
        {
            _toolbars = new IToolbar[] 
                             {
                                 new MainToolBarCatalog()
                             };
            _menus = new IMenu[] 
                             {
                                 new FileMenusCatalog(),
                                 new EditMenusCatalog(),
                                 new ViewMenusCatalog(),
                                 new ToolMenusCatalog()
                             };
        }

        #region ICommandProvider ≥…‘±

        public IToolbar[] Toolbars
        {
            get 
            {
                return _toolbars;
            }
        }

        public IMenu[] Menus
        {
            get 
            {
                return _menus;
            }
        }

        #endregion
    }
}
