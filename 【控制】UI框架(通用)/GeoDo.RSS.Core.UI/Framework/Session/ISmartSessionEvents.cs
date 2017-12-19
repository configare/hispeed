using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public delegate void SmartSessionLoadedHandler(object  sender);
    public delegate void FileOpenedHandler(object sender,string fileName);

    public interface ISmartSessionEvents
    {
        SmartSessionLoadedHandler OnSmartSessionLoaded { get; set; }
        FileOpenedHandler OnFileOpended { get; set; }
    }
}
