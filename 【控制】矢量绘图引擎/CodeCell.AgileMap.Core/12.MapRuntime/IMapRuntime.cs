using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace CodeCell.AgileMap.Core
{
    public interface IMapRuntime : IMapRuntimeRenderable,IDisposable
    {
        IMapRuntimeHost Host { get; set; }
        RectangleF ActualViewport { get; }
        int Scale { get; set; }
        ScaleBarArgs ScaleBarArgs { get; }
        IMap Map { get; }
        void Apply(IMap map);
        string CanvasSpatialRef { get; set; }
        IRuntimeExchanger RuntimeExchanger { get; }
        int DPI { get; set; }
        OnMapScaleChangedHandler OnMapScaleChanged { get; set; }
        OnViewExtentChangedHandler OnViewExtentChanged { get; set; }
        ILightLayerContainer LightLayerContainer { get; }
        IMapRefresh MapRefresh { get; }
        ILocatingFocusLayer LocatingFocusLayer { get; set; }
        ILocationIconLayer LocationIconLayer { get; set; }
        IQueryResultContainer QueryResultContainer { get; set; }
        ILocationService LocationService { get; }
        bool IsExceedMinResolution(RectangleF evpPrj);
        bool IsExceedMaxResolution(RectangleF evpPrj);
        void HitTest(PointF pixelPt, out Feature feature, out RectangleF rect);
        Feature[] HitTest(RectangleF rect);
        Feature HitTestByPrj(ShapePoint prjPoint);
        Feature[] HitTestByPrj(Envelope prjRect);
        IAsyncDataArrivedNotify AsyncDataArrivedNotify { get; set; }
    }
}
