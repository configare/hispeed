using System;

namespace Telerik.WinControls.UI
{
    public delegate void PropertyGridEditorRequiredEventHandler(object sender, PropertyGridEditorRequiredEventArgs e);

    public class PropertyGridEditorRequiredEventArgs : EditorRequiredEventArgs
    {
        PropertyGridItemBase item;

        public PropertyGridEditorRequiredEventArgs(PropertyGridItemBase item, Type editorType)
        : base(editorType)
        {
            this.item = item;
        }

        public PropertyGridItemBase Item
        {
            get
            {
                return this.item;
            }
        }
    }
}