using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public interface IEditorManager
    {
        IInputEditor GetDefaultEditor(IEditorProvider provider);

        void RegisterPermanentEditorType(Type editorType);

        bool IsPermanentEditor(Type editorType);
    }
}
