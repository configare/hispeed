using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Layout.DataFrm
{
    public class GridLineObject
    {
        public string Text;
        public PointF pointF = PointF.Empty;
        public Font font = new Font("微软雅黑", 9f);
        public Brush fontBrush = new SolidBrush(Color.Yellow);
        public Brush maskBrush = new SolidBrush(Color.Blue);

        public GridLineObject()
        { }
    }
}
