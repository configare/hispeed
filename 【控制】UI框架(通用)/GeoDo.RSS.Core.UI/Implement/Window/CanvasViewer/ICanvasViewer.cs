using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.Core.UI
{
    public delegate void CanvasViewerContextMenuHandler(object sender,string action,object data);

    public interface ICanvasViewer:ISmartViewer,
        IViewerWithCoordinate,IDisposable,
        IIsLayerable,ICanvasViewerMenuHandlerManager
    {
        ICanvas Canvas { get; }
        object RgbProcessorArgEditorEnvironment { get; }
        Bitmap ActiveDrawing { get; }
        IAOIProvider AOIProvider { get; }
        IAOIContainerLayer AOIContainerLayer { get; }
        ISelectedAOILayer SelectedAOILayer { get; }
    }
}
