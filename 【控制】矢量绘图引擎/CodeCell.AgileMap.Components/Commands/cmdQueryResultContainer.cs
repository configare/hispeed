using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.Components
{
    public class cmdQueryResultContainer : BaseCommand
    {
        public cmdQueryResultContainer()
        {
            Init();
        }

        public cmdQueryResultContainer(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "查询结果容器";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdProperty.png");
        }

        public override void Click()
        {
            IHookOfAgileMap budgis = _hook as IHookOfAgileMap;
            if (budgis.MapControl == null)
                return;
            if (budgis.MapControl.MapRuntime.QueryResultContainer != null)
                budgis.MapControl.MapRuntime.QueryResultContainer.ResultContainerVisible = !budgis.MapControl.MapRuntime.QueryResultContainer.ResultContainerVisible;
        }
    }
}
