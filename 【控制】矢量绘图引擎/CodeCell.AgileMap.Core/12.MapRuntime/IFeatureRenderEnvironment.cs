using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace CodeCell.AgileMap.Core
{
    public interface IFeatureRenderEnvironment
    {
        Envelope ExtentOfProjectionCoord { get; }
        OnTransformChangedHandler OnTransformChanged { get; set; }
        int CurrentScale { get; }
        int CurrentLevel { get; }
        ICoordinateTransform CoordinateTransform { get; }
        bool IsMouseBusy { get; }
        IConflictor ConflictorForSymbol { get; }
        IConflictor ConflictorForLabel { get; }
    }
}
