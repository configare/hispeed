using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    internal class StatInfoFetClass
    {
        public enumShapeType ShapeType = enumShapeType.NullShape;
        public int FeatureCount = 0;
        public Envelope Envelope = null;

        public StatInfoFetClass()
        { 
        }

        public StatInfoFetClass(enumShapeType shapeType, int featureCount, Envelope envelope)
        {
            ShapeType = shapeType;
            FeatureCount = featureCount;
            Envelope = envelope;
        }
    }
}
