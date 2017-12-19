using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.Components
{
    public class cmdCatalog : BaseCommand
    {
        public cmdCatalog()
        {
            Init();
        }

        public cmdCatalog(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "目录管理";
            _tooltips = _text;
        //    _image = ResourceLoader.GetBitmap("cmdZoomIn.png");
        }

        public override void Click()
        {
            //using (frmCatalog frm = new frmCatalog())
            //{
            //    frm.ShowDialog();
            //}
        }
    }
}
