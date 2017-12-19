using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public interface IAVILayerDisplay
    {
        bool IsCustom { set; }
        void DisplayAvi(ISmartSession session, string wndName, string[] fnames, 
                                 string templateName, string subIdentify, out string outputFname);
    }
}
