using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.ToolBox
{
    public class EditMenus:BaseMenu
    {
        public EditMenus()
        {
            _items = new IItem[] 
                          {
                              //new cmdCopy(),
                              //new cmdCut(),
                              //new cmdPaste(),
                              new cmdDelete(true)
                          };
            _text = "编辑(&E)";
        }
    }
}
