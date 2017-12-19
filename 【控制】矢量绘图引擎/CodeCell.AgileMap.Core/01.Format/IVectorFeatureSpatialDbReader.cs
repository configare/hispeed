using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IVectorFeatureSpatialDbReader:IDisposable,IReaderInsideSession
    {
        string Name { get; }
        string Id { get; }
        string[] Fields { get; }
        bool AllowReadAllAtFirst { get; set; }
        Envelope Envelope { get; }
        ISpatialReference SpatialReference { get; }
        int FeaureCount { get; }
        enumShapeType ShapeType { get; }
        Feature[] Read(Envelope envelope/*, RepeatFeatureRecorder recorder*/);
        void SetArgsOfLeveling(ArgOfLeveling arg);
    }
}
