using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using CodeCell.AgileMap.Components;

namespace CodeCell.AgileMap.Express
{
    public class FileMenus : BaseMenu
    {
        public FileMenus()
        {
            _items = new IItem[] 
                         {
                             new cmdOpenFile(),
                             new cmdSave(true)
                         };
            _text = "文件(&F)";
        }
    }
}
