using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    public interface IFeatureRenderer : IDisposable,IPersistable
    {
        ISymbol CurrentSymbol { get; }
        void Render(bool enableDisplayLevel, QuickTransformArgs quickTransformArgs, IGrid gd,Graphics g, Envelope rect,RepeatFeatureRecorder recorder);
        RotateFieldDef RotateFieldDef { get; set; }
    }
}
