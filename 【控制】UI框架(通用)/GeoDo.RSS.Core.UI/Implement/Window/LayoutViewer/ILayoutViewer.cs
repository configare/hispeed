using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Layout;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.Core.UI
{
    public interface ILayoutViewer : ISmartViewer, IViewerWithCoordinate,IIsLayerable
    {
        ILayoutHost LayoutHost { get; }
        object RgbProcessorArgEditorEnvironment { get; }
    }
}
