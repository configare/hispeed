using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class cmdZoomIn : BaseTool
    {
        public cmdZoomIn()
        {
            Init();
        }

        public cmdZoomIn(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "放大";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdZoomIn.png");
            _cursor = ResourceLoader.GetCursor("cmdZoomIn.cur");
        }

        public override void Click()
        {
            IHookOfAgileMap budgis = _hook as IHookOfAgileMap;
            if (budgis.MapControl == null || budgis.MapControl.Map == null)
                return;
            budgis.MapControl.SetCurrentMapTool(enumMapToolType.ZoomIn);
        }
    }
}
