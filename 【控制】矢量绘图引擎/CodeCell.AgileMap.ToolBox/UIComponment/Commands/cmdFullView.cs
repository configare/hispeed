using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.ToolBox
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
            (_hook as IHookOfModelEditor).ModelEditor.Extent = (_hook as IHookOfModelEditor).ModelEditor.FullExtent;
            (_hook as IHookOfModelEditor).ModelEditor.Render();
        }
    }
}
