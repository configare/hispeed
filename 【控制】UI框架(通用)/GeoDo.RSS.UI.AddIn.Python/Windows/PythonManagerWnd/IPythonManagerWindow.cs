using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using System.Windows.Forms;

namespace GeoDo.RSS.UI.AddIn.Python
{
    interface IPythonManagerWindow : ISmartToolWindow 
    {
        void SetupWorkspace(string wp);
        void RefreshWorkspace();
        TreeView GetTree();
    }
}
