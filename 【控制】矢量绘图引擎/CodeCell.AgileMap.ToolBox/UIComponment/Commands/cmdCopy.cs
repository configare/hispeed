using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.ToolBox
{
    public class cmdCopy : BaseCommand
    {
        public cmdCopy()
        {
            Init();
        }

        public cmdCopy(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "复制";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdCopy.png");
        }
    }
}
