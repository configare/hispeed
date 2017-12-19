using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using CodeCell.AgileMap.Components;

namespace CodeCell.AgileMap.Express
{
    public class ToolMenus:BaseMenu
    {
        public ToolMenus()
        {
            _items = new IItem[]
                           {
                              new cmdCatalog(),
                              new cmdToolBox(true)
                           };
            _text = "工具(&T)";
        }
    }
}
