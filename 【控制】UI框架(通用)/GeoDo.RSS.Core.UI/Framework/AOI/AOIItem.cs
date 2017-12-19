using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public class AOIItem
    {
        public string Name;
        public GeoDo.RSS.Core.DrawEngine.CoordEnvelope GeoEnvelope;

        public AOIItem()
        { 
        }

        public AOIItem(string name,GeoDo.RSS.Core.DrawEngine.CoordEnvelope envelope)
        {
            Name = name;
            GeoEnvelope = envelope;
        }
    }
}
