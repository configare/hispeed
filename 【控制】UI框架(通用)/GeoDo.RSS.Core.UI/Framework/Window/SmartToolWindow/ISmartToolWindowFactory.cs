using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.UI
{
    public interface ISmartToolWindowFactory
    {
        ISmartToolWindow GetSmartToolWindow(int identify);
        bool IsDisplayed(int identify);
        bool IsDisplayed(Type type);
    }
}
