using System;
using System.Collections.Generic;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IVectorFeatureDataReader : IUniversalVectorDataReader,IFeatureFetcher
    {
        Envelope Envelope { get; }
        int FeatureCount { get; }
        Feature[] Features { get; }
        string[] Fields { get; }
        enumShapeType ShapeType { get; }
        ISpatialReference SpatialReference { get; }
        Feature[] GetFeatures(Envelope envelope/*,RepeatFeatureRecorder recorder*/);
        void SetArgsOfLeveling(ArgOfLeveling arg);
    }
}
