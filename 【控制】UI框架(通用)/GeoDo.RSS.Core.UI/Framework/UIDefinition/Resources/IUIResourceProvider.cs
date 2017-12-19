using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GeoDo.RSS.Core.UI
{
    public interface IUIResourceProvider:IDisposable
    {
        Image GetImage(string resIdentify);
    }
}
