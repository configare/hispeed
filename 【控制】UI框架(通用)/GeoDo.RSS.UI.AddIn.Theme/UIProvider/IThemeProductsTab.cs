using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.UI.AddIn.Theme
{
    public interface IThemeProductsTab
    {
        bool IsOpenWorkspace { get; set; }
        void ActiveProduct(string identify,bool isOpenWorkspace);
    }
}
