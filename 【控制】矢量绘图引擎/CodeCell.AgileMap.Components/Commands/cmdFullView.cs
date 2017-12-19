using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.Components
{
    public class cmdFullView:BaseCommand
    {
        public cmdFullView()
        {
            Init();
        }

        public cmdFullView(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "全视图";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdFullView.png");
        }

        public override void Click()
        {
            IHookOfAgileMap budgis = _hook as IHookOfAgileMap;
            if (budgis.MapControl == null || budgis.MapControl.Map == null)
                return;
            budgis.MapControl.ExtentPrj = budgis.MapControl.FullExtentPrj;
            budgis.MapControl.ReRender();
        }
    }
}
