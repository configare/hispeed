using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class cmdArrow : BaseTool
    {
        public cmdArrow()
        {
            Init();
        }

        public cmdArrow(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "选取";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdArrow.png");
            _cursor = ResourceLoader.GetCursor("cmdArrow.cur");
        }

        public override void Click()
        {
            IHookOfAgileMap budgis = _hook as IHookOfAgileMap;
            if (budgis.MapControl == null || budgis.MapControl.Map == null)
                return;
            budgis.MapControl.SetCurrentMapTool(enumMapToolType.Identify);
        }
    }
}
