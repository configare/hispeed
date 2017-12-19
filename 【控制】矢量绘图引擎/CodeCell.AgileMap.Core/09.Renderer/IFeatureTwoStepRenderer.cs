using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    public enum enumTwoStepType
    { 
        Outline,
        Fill
    }

    public interface IFeatureTwoStepRenderer:IFeatureRenderer
    {
        enumTwoStepType StepType { get; set; }
    }
}
