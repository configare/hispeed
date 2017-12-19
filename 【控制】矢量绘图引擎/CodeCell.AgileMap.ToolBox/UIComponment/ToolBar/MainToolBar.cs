using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.ToolBox
{
    public class MainToolBar:BaseToolbar
    {
        public MainToolBar()
        {
            _items = new IItem[] 
                         {
                             new cmdOpenFile(),
                             new cmdSave(true),
                             new cmdArrow(true),
                             //new cmdCopy(true),
                             //new cmdCut(),
                             //new cmdPaste(),
                             new cmdDelete(true),
                             new cmdText("显示比例:",true),
                             new cmdScale(),
                             new cmdFullView(true)
                         };
        }
    }
}
