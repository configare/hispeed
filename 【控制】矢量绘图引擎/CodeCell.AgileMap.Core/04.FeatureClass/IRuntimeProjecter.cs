using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IRuntimeProjecter
    {
        void Project(ShapePoint point);
        void Project(ShapePoint[] points);
        void Project(Envelope envelope);
        string CanvasSpatialRef { get; }
    }
}
