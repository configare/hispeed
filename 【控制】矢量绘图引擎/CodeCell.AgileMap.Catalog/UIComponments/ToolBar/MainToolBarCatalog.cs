using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using CodeCell.AgileMap.Components;

namespace CodeCell.AgileMap.Catalog
{
    public class MainToolBarCatalog : BaseToolbar
    {
        public MainToolBarCatalog()
        {
            _items = new IItem[] 
                         {
                             //new cmdOpenFile(),
                             //new cmdSave(true),
                             //new cmdSetProjection(true),
                             new cmdSave(true),
                             new cmdArrow(true),
                             new cmdPan(),
                             new cmdZoomIn(),
                             new cmdZoomOut(),
                             new cmdFullView(),
                             new cmdPreviousOpr(true),
                             new cmdNextOpr(),
                             new cmdText("比例尺 1 : ",true),
                             new cmdScale(),
                             new cmdAddLayer(),
                             new cmdLayerManager(),
                             new cmdText("查询定位:",true),
                             new cmdQueryBox(),
                             new cmdQueryResultContainer()
                         };
        }
    }
}
