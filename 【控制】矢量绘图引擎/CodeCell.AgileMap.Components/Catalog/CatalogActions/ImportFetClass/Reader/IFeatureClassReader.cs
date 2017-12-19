using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Components
{
    public interface IFeatureClassReader:IDisposable
    {
        string AnnoTable { get; }
        ISpatialReference SpatialReference { get; }
        string[] FieldNames { get; }
        Feature[] Read(IProgressTracker tracker);
        Envelope Envelope { get; }
        enumShapeType ShapeType { get; }
    }
}
