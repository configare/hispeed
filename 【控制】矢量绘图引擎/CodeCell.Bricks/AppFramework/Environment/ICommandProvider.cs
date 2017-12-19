using System;
using System.Collections.Generic;
using System.Text;

namespace CodeCell.Bricks.AppFramework
{
    public interface ICommandProvider
    {
        IToolbar[] Toolbars { get;}
        IMenu[] Menus { get;}
    }
}
