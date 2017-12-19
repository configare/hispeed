using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    [Serializable]
    public class FetClassProperty
    {
        public string Name = null;
        public Envelope FullEnvelope = null;
        public enumCoordinateType CoordinateType = enumCoordinateType.Geographic;
        public enumShapeType ShapeType = enumShapeType.NullShape;
        public string[] Fields = null;
        public ISpatialReference SpatialReference = null;
        public int FeatureCount = 0;
    }
}
