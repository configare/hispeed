using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public interface ILinkableViewerManager
    {
        enumViewerLayoutStyle Layout { get; set; }
        IEnumerable<ILinkableViewer> LinkViewers { get; }
        bool Link(ILinkableViewer viewer);
        bool IsLinking(ILinkableViewer viewer);
        void Unlink(ILinkableViewer viewer);
        void ChangePrimaryLinkViewer(ILinkableViewer viewer);
        void To(ILinkableViewer fromViewer, GeoDo.RSS.Core.DrawEngine.CoordEnvelope viewport);
        void Reset();
    }
}
