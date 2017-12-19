using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;
using System.Drawing;
using GeoDo.RSS.Core.VectorDrawing;

namespace GeoDo.RSS.UI.Bricks
{
    public interface ISimpleMapControl
    {
        bool IsAllowPanMap { get; set; }
        void ApplyMap(string mcdfile);
        CoordEnvelope CurrentEnvelope { get; set; }
        void ToViewport(CoordEnvelope geoCoordEnvelope);
        void ToChinaViewport();
        void ToWorldViewport();
        void Render();
        void Reset();
        void AddImageLayer(string name, Bitmap bitmap, CoordEnvelope coordEnvelope, bool isGeoCoord);
        void RemoveImageLayer(string name);
        void RemoveAllImageLayers();
        ISimpleVectorObjectHost CreateObjectHost(string name);
        CoordEnvelope DrawedAOI { get; set; }
        bool IsAllowAOI { get; set; }
        bool IsAllowSelect { get; set; }
    }
}
