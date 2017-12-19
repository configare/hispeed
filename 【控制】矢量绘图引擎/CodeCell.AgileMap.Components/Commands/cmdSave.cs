using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public class cmdSave:BaseCommand
    {
        public cmdSave()
        {
            Init();
        }

        public cmdSave(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "保存";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdSave.png");
        }

        public override void Click()
        {
             IMapControl mapControl = (_hook as IHookOfAgileMap).MapControl;
             if (mapControl != null && mapControl.Map != null)
             {
                 SaveMapDialog.Save(mapControl.Map, true);
             }
        }
    }
}
