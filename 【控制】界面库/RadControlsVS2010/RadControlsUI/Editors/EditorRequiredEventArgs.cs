using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Provides information about the type of the editor required by the 
    /// GridTableViewManager when instantiating the requested type of column.
    /// </summary>
    public class EditorRequiredEventArgs : EventArgs
    {
        // Fields 
        private Type editorType = null;
        private IValueEditor editor = null;

        /// <summary>
        /// Initializes <see cref="EditorRequiredEventArgs"/> with editor type defined.
        /// </summary>
        public EditorRequiredEventArgs() : this(null)
        {
        }

        /// <summary>
        /// Initializes <see cref="EditorRequiredEventArgs"/> setting the required editor type.
        /// </summary>
        /// <param name="editorType">The type of the editor required.</param>
        public EditorRequiredEventArgs(Type editorType)
        {
            this.EditorType = editorType;
        }

        /// <summary>
        /// Initializes <see cref="EditorRequiredEventArgs"/> setting the required editor type.
        /// The IInputEditor property is initialized by GridViewEditManager prior to event call, 
        /// if default implementation is available.
        /// </summary>
        /// <param name="editorType">The type of the editor required.</param>
        /// <param name="defaultEditor">IInputEditor instance if available.</param>
        internal EditorRequiredEventArgs(Type editorType, IInputEditor defaultEditor)
        {
            this.EditorType = editorType;
            this.Editor = defaultEditor;
        }

        /// <summary>
        /// Gets the type of the editor required by the edited control if no default editor is available.
        /// </summary>
        public Type EditorType
        {
            get { return this.editorType; }
            set { this.editorType = value; }
        }

        /// <summary>
        /// Gets the ICellEditor instance if created outside the GridViewEditorManager.
        /// Also if a default editor is provided by the RadGridView, it is available for post-initialization 
        /// or substitution.
        /// </summary>
        public IValueEditor Editor
        {
            get { return editor; }
            set { editor = value; }
        }
    }
}
