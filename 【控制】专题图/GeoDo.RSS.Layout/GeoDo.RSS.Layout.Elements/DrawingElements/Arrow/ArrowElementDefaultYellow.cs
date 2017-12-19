using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace GeoDo.RSS.Layout.Elements
{
    [Export(typeof(IElement)), Category("箭头")]
    public class ArrowElementDefaultYellow : ArrowElement
    {
        public ArrowElementDefaultYellow()
            : base()
        {
            Init();
        }

        public ArrowElementDefaultYellow(PointF location)
            : base(location)
        {
            Init();
        }

        private void Init()
        {
            _name = "箭头(默认黄色)";
            this.Color = Color.Yellow;
        }
    }
}
