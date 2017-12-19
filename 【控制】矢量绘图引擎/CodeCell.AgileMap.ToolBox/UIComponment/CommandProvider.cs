using System;
using System.Collections.Generic;
using System.Text;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.ToolBox
{
    public class CommandProvider:ICommandProvider
    {
        protected IToolbar[] _toolbars = null;
        protected IMenu[] _menus = null;

        public CommandProvider()
        {
            _toolbars = new IToolbar[] 
                             {
                                 new MainToolBar()
                             };
            _menus = new IMenu[] 
                             {
                                 new FileMenus(),
                                 new EditMenus(),
                                 new ViewMenus(),
                                 new ToolMenus()
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
