using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents the base for all editor elements. Provides the default visual states such as IsFocused and Disabled.
    /// </summary>
    public abstract class RadEditorElement : RadItem
    {
        static RadEditorElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new EditorElementStateManager(), typeof(RadEditorElement));
        }
    }
}
