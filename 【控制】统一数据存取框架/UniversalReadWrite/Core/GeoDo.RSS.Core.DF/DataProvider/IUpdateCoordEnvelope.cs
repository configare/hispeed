using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public interface IUpdateCoordEnvelope
    {
        void Update(CoordEnvelope coordEnvelope);
        bool IsStoreHeaderChanged { get; set; }
    }
}
