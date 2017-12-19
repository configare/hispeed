using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.CA
{
    public interface IRgbArgEditorEnvironmentSupport
    {
        void StartPickColor(IPickColorIsFinished onPickColorIsFinished);
        Bitmap ActiveDrawing { get; }
        List<EventHandler> CanvasRefreshSubscribers { get; }
    }
}
