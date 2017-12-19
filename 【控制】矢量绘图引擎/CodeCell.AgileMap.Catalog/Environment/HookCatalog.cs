using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.Bricks.AppFramework;
using CodeCell.AgileMap.Components;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Catalog
{
    public class HookCatalog:HookAgileMap,IHookOfCatalog
    {
        public HookCatalog(IMapControl mapControl, IApplication application, ToolStripItem control, ICommandHelper commandHelper)
            : base(mapControl, application, control, commandHelper)
        {
            _mapControl = mapControl;
        }
    }
}
