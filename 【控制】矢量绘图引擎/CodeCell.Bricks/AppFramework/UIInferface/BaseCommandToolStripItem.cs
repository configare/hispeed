using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.Bricks.AppFramework
{
    public class BaseCommandToolStripItem:BaseCommand
    {
        private ToolStripMenuItem _it = null;
        public BaseCommandToolStripItem(ToolStripMenuItem it) 
        {
            _it = it;
            _text = it.Text;
            _name = it.Name;
            _tooltips = _text;
        }

        public BaseCommandToolStripItem(ToolStripMenuItem it,bool isBeginGroup)
            : base(isBeginGroup)
        {
            _it = it;
            _text = it.Text;
            _name = it.Name;
            _tooltips = _text;
        }

        public override void Click()
        {
            _it.PerformClick();
        }
    }
}
