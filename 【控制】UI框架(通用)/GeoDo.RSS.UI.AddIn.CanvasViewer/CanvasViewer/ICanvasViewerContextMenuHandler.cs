using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DrawEngine;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    internal interface ICanvasViewerContextMenuHandler
    {
        string GetArgProviderUI(enumCanvasViewerMenu menuItem);
        bool HandleImportAOI();
        bool HandleSelectAOIFromFeatures(GeometryOfDrawed result);
        void HandleErase(GeometryOfDrawed result);
        void HandleAdsorb(GeometryOfDrawed result);
        void HandleMagicWandExtracting(GeometryOfDrawed result,Dictionary<string,object> args);
        void HandleClearAOI();
        void HandleFlash();
        void HandleUnDo();
        void HandleReDo();
        void HandleRemoveAll();
    }
}
