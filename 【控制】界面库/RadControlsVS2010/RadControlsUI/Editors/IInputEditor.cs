using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public interface IInputEditor : IValueEditor
    {
        IEditorManager EditorManager
        {
            get;
            set;
        }

        bool IsModified
        { 
            get; 
        }
    }
}
