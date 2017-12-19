using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace CodeCell.AgileMap.Core
{
    public interface ILabelRender
    {
        void Begin(IConflictor conflictor,object environment);
        void Draw(Matrix emptyRotateMatrix, Graphics g, LabelDef labelDef, ISymbol currentSymbol, Feature fet, QuickTransformArgs tranArgs);
    }
}
