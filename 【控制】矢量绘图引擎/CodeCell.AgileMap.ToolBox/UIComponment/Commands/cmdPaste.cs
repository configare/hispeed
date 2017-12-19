using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.ToolBox
{
    public class cmdPaste : BaseCommand
    {
        public cmdPaste()
        {
            Init();
        }

        public cmdPaste(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _text = "粘贴";
            _tooltips = _text;
            _image = ResourceLoader.GetBitmap("cmdPaste.png");
        }

        public override bool Enabled
        {
            get
            {
                return base.Enabled && _hook.Application.ClipboardEnvironment.Count != 0;
            }
        }

        public override void Click()
        {
            object[] objs =  _hook.Application.ClipboardEnvironment.GetDatas();
            if (objs == null || objs.Length == 0)
                return;
        }
    }
}
