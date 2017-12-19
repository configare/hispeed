using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.Bricks.AppFramework;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class HookAgileMap:HookDefault,IHookOfAgileMap
    {
        protected IMapControl _mapControl = null;

        public HookAgileMap(IMapControl mapControl, IApplication application, ToolStripItem control, ICommandHelper commandHelper)
            : base(application, control, commandHelper)
        {
            _mapControl = mapControl;
            _containerControl = mapControl as Control;
        }

        public IMapControl MapControl
        {
            get { return _mapControl; }
        }
    }
}
