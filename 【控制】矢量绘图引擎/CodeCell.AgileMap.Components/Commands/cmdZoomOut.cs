using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class cmdZoomOut:BaseTool
    {
        public cmdZoomOut()
        {
            Init();
        }

        public cmdZoomOut(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "缩小";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdZoomOut.png");
            _cursor = ResourceLoader.GetCursor("cmdZoomOut.cur");
        }

        public override void Click()
        {
            IHookOfAgileMap budgis = _hook as IHookOfAgileMap;
            if (budgis.MapControl == null || budgis.MapControl.Map == null)
                return;
            budgis.MapControl.SetCurrentMapTool(enumMapToolType.ZoomOut);
        }
    }
}
