using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IFeatureSplitterByProjection
    {
        bool IsNeedSplit { get; }
        void Split(Feature feature, out Feature[] splittedFeatures);
    }
}
