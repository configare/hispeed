using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using FastColoredTextBoxNS;

namespace GeoDo.RSS.UI.AddIn.Python
{
    public interface IPythonEditorWindow : ISmartToolWindow
    {
        FastColoredTextBox getTB();
        void CreateTB(string fileName);
    }
}
