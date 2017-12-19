using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.Bricks
{
    public interface IVectorObject
    {
        string Text { get; }
        bool IsGeoCoord { get; }
        CoordEnvelope CoordEnvelope { get; }
    }
}
