using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public interface ILinkableViewer
    {
        bool IsPrimaryLinkWnd { get; set; }
        void To(GeoDo.RSS.Core.DrawEngine.CoordEnvelope viewport);
        EventHandler OnCoordEnvelopeChanged { get; set; }
    }
}
