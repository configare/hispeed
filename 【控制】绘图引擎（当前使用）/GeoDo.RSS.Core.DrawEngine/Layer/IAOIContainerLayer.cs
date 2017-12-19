using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine
{
    public delegate void AOIGeometryIsUpdatedHandler(object sender,object geometry);

    public interface IAOIContainerLayer
    {
        bool IsOnlyOneAOI { get; set; }
        Color Color { get; set; }
        float LineWidth { get; set; }
        void Reset();
        void AddAOI(object geometry);
        void RemoveSelectedAOI(object[] geometry);
        object FirstAOI { get; }
        IEnumerable<object> AOIs { get; }
        bool IsAllowEdit { get; set; }
        AOIGeometryIsUpdatedHandler AOIGeometryIsUpdated { get; set; }
    }
}
