using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class cmdOpenFile:BaseCommand
    {
        public cmdOpenFile()
        {
            Init();
        }

        public cmdOpenFile(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "打开文件";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdOpen.png");
        }

        public override void Click()
        {
            IMap map = OpenMapDialog.Open();
            if (map != null)
            {
                IMapControl m = (_hook as IHookOfAgileMap).MapControl;
                m.Apply(map);
                m.ReRender();
            }
        }
    }
}
