using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    public interface IContextMenuToolbarManager
    {
        IContextMenuArgProvider Show(string argProviderUI);
        void Close();
    }
}
