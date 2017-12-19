using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface ILabelDef:IPersistable
    {
        enumLabelSource LabelSource { get; set; }
        bool EnableLabeling { get; set; }
        ScaleRange DisplayScaleRange { get; }
        bool VisibleAtScale(int scale);
        IContainerSymbol ContainerSymbol { get; }
        int Angle { get; set; }
    }
}
