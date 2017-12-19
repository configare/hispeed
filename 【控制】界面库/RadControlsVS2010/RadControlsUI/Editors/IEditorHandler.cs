using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public interface IEditorHandler
    {
        void HandleEditorKeyDown(KeyEventArgs e);
        
        void HandleEditorKeyPress(KeyPressEventArgs e);
        
        void HandleEditorKeyUp(KeyEventArgs e);
        
        void HandleEditorValidating(CancelEventArgs e);
        
        void HandleEditorValidated(EventArgs e);
        
        void HandleEditorValueChanging(ValueChangingEventArgs e);
        
        void HandleEditorValueChanged(EventArgs e);
        
        void HandleEditorValidationError(ValidationErrorEventArgs e);

        void HideEditor(IInputEditor editor);

        void ShowEditor(IInputEditor editor);

        void WriteValue();
        
        void ReadValue();
    }
}
//void OnRequireHideEditor();
//void HandleEditorKeyDown(KeyEventArgs e);
//void HandleEditorKeyPress(KeyPressEventArgs e);
//void HandleEditorKeyUp(KeyEventArgs e);