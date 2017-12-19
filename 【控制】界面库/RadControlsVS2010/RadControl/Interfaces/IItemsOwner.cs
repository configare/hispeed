using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;

namespace Telerik.WinControls.Elements
{
    /// <summary>
    /// Exposes the Items property for accessing a collection of the items in a
    /// combobox.
    /// </summary>
    public interface IItemsOwner
    {
        /// <summary>
        /// Gets a collection representing the collection of the items contained 
        /// in this ComboBox. 
        /// </summary>
        [RadEditItemsAction]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor)),
        Category(RadDesignCategory.DataCategory)]
        RadItemOwnerCollection Items {get;}
    }
}
