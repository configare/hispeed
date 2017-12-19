using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.Components
{
    public class cmdToolBox : BaseCommand
    {
        public cmdToolBox()
        {
            Init();
        }

        public cmdToolBox(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "工具箱";
            _tooltips = _text;
         //   _image = ResourceLoader.GetBitmap("cmdZoomIn.png");
         }

        public override void Click()
        {
            //using (frmModelEditor frm = new frmModelEditor())
            //{
            //    frm.RegAssemblies(new string[] { "AgileMap.Bricks.dll" });
            //    frm.ShowDialog();
            //}
        }
    }
}
