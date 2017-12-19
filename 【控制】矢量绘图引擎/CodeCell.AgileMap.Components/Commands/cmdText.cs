using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using System.Windows.Forms;

namespace CodeCell.AgileMap.Components
{
    public class cmdText:BaseControlItem
    {
        public cmdText(string text)
        {
            _text = text;
            Init();
        }

        public cmdText(string text,bool beginGroup)
            : base(beginGroup)
        {
            _text = text;
            Init();
        }

        private void Init()
        {
            ToolStripLabel lb = new ToolStripLabel(_text);
            lb.Width = 100;
            _control = lb;
        }
    }
}
