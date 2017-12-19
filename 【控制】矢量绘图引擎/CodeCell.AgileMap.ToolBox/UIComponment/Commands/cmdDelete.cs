using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.ToolBox
{
    public class cmdDelete : BaseCommand
    {
        public cmdDelete()
        {
            Init();
        }

        public cmdDelete(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "删除";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdDelete.png");
        }
    }
}
