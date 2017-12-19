using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.UI
{
    public interface IPlugIn:IDisposable
    {
        string Name { get; }
        Image Icon { get; }
        Dictionary<string, object> Arguments { get; }
        IUIGroupProvider UI { get; }
        ICommand[] Commands { get; }
        void Init(ISmartSession session);
    }
}
