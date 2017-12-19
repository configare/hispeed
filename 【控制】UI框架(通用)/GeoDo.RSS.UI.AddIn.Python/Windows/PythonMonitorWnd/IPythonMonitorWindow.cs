using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using GeoDo.PythonEngine;

namespace GeoDo.RSS.UI.AddIn.Python
{
    public interface IPythonMonitorWindow : ISmartToolWindow
    {
        void NotityFetchLog();
    }
}
