using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{    
    /// <summary>
    /// This interface supports the editor infrastructure of the RadGridView.
    /// </summary>
    public interface IValueEditor 
    {
        void Initialize(object owner, object value);

        void BeginEdit();

        bool EndEdit();

        bool Validate();

        object Value { get; set; }
        
        /// <summary>
        /// Occurs when the editor is validating the value.
        /// </summary>
        event CancelEventHandler Validating;

        /// <summary>
        /// Occurs when the editor is finished validating the value.
        /// </summary>
        event EventHandler Validated;

        /// <summary>
        /// Occurs when the editor value is being changed. Cancelable event.
        /// </summary>
        event ValueChangingEventHandler ValueChanging;

        /// <summary>
        /// Occurs when the value of the editor changes.
        /// </summary>
        event EventHandler ValueChanged;

        /// <summary>
        /// Occurs when internally the editor detects an error or when the Validating event fails.
        /// </summary>
        event ValidationErrorEventHandler ValidationError;
    }
}