using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CodeCell.Bricks.RedoUndo;


namespace CodeCell.AgileMap.Core
{
    public interface IMapControl:IMouseLocationInfoPrinterManager
    {
        int ScaleDenominator { get; set; }
        ISpatialReference SpatialReference { get; set; }
        IMap Map { get; }
        void Apply(IMap map);
        RectangleF FullExtentPrj { get; }
        RectangleF ExtentPrj { get; set; }
        Envelope ExtentGeo { get; }
        void PanTo(Envelope geoCoord);
        void PanTo(ShapePoint geoCenterPoint);
        IRuntimeExchanger Exchanger { get; }
        IMapControlEvents MapControlEvents { get; }
        ICoordinateTransform CoordinateTransfrom { get; }
        IOperationStack OperationStack { get; }
        void Render();
        void ReRender();
        void Render(OnRenderIsFinishedHandler finishNotify);
        void ReRender(OnRenderIsFinishedHandler finishNotify);
        void SetCurrentMapTool(enumMapToolType standardToolType);
        void SetCurrentMapTool(IMapTool mapTool);
        IMapTool GetCurrentMapTool();
        IMapRuntime MapRuntime { get; }
        void SetViewport(Envelope geoEnvelope);
        void HitTest(PointF pixelPoint, out Feature feature, out RectangleF rect);
        IBookmarkManager BookmarkManager { get; set; }
    }
}
