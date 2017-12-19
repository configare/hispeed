using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public interface ICanvasViewerMenuHandlerManager
    {
        IEnumerable<object[]> Handlers { get; }
        void Register(string identify,string menuItem, Action<object,Dictionary<string,object>> action,string argProviderUI);
        void UnRegister(string identify);
    }
}
