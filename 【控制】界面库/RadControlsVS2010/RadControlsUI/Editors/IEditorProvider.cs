using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Provides functionality for managing editors
	/// </summary>
    public interface IEditorProvider
    {
		/// <summary>
		/// Returns an editor instance of the default type for the editor provider.
		/// </summary>
		/// <returns>An object that implements <see cref="IInputEditor"/> interface.</returns>
        //[Obsolete("This is not used anymore.")]
        IInputEditor GetDefaultEditor();

		/// <summary>
		/// Gets the default editor type for the editor provider.
		/// </summary>
		/// <returns>The default type.</returns>
        Type GetDefaultEditorType();

		/// <summary>
		/// Initializes a specified editor.
		/// </summary>
		/// <param name="editor">An object that implements <see cref="IInputEditor"/> interface.</param>
        void InitializeEditor(IInputEditor editor);
    }
}
