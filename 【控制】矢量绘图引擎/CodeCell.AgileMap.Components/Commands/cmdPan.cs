using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class cmdPan : BaseTool
    {
        public cmdPan()
        {
            Init();
        }

        public cmdPan(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "漫游";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdPan.png");
            _cursor = ResourceLoader.GetCursor("cmdPan.cur");
        }

        public override void Click()
        {
            IHookOfAgileMap budgis = _hook as IHookOfAgileMap;
            if (budgis.MapControl == null || budgis.MapControl.Map == null)
                return;
            budgis.MapControl.SetCurrentMapTool(enumMapToolType.Pan);
        }
    }
}
