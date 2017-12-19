using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.UI.AddIn.CanvasViewer
{
    public interface IContextMenuArgProvider
    {
        void SetArg(string name, object value);
        object GetArg(string name);
    }
}
