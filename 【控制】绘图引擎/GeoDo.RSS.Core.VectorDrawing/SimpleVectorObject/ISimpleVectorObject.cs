using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.Core.VectorDrawing
{
    public interface ISimpleVectorObject
    {
        int OID { get; set; }
        string Name { get; }
        string[] AttValues { get; }
        void SetGeometry(CoordEnvelope geometry);
        void SetGeometry(Shape geoShape);
        Shape Geometry { get; }
        bool Visible { get; set; }
        bool IsSelected { get; set; }
        bool IsWaitingProcess { get; set; }
    }
}
