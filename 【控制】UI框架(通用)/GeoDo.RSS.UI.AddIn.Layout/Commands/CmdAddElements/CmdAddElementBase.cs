using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Layout
{
    public class CmdAddElementBase:Command
    {
        public void TryRefreshLayerManager()
        {
            ISmartWindow layerManager = _smartSession.SmartWindowManager.GetSmartWindow((wnd) => { return wnd is ILayerManager; });
            if (layerManager != null)
                (layerManager as ILayerManager).Update();
        }
    }
}
