using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;

namespace GeoDo.RSS.Core.DrawEngine
{
    public interface ISelectedAOILayer
    {
        Color Color { get; set; }
        float LineWidth { get; set; }
        void Reset();
        void AddSelectedAOI(object geometry);
        void RemoveSelectedAOI(object[] geometry);
        IAOIContainerLayer AOIContaingerLayer { set; }
        object FirstAOI { get; }
        bool Edit { get; set; }
        IEnumerable<object> AOIs { get; }
    }
}
