using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.ToolBox
{
    public class cmdArrow:BaseCommand
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
        }
    }
}
