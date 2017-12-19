using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class ListViewTextBoxEditor : BaseTextBoxEditor
    { 
        #region Overrides

        protected override RadElement CreateEditorElement()
        {
            BaseTextBoxEditorElement editor = new BaseTextBoxEditorElement();
            editor.TextBoxItem.Alignment = ContentAlignment.MiddleLeft;
            editor.StretchVertically = true; 
            return editor;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            BaseListViewVisualItem visualItem = this.OwnerElement as BaseListViewVisualItem;
            if (visualItem != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        visualItem.Data.Owner.CancelEdit();
                        break;

                    case Keys.Enter:
                        visualItem.Data.Owner.EndEdit();
                        break;

                    case Keys.Up:
                        if (!this.Multiline || (selectionLength == 0 && isAtFirstLine))
                        {
                            visualItem.Data.Owner.EndEdit();
                            visualItem.Data.Owner.ProcessKeyDown(e);
                        }
                        break;

                    case Keys.Down:
                        if (!this.Multiline || (selectionLength == 0 && isAtLastLine))
                        {
                            visualItem.Data.Owner.EndEdit();
                            visualItem.Data.Owner.ProcessKeyDown(e);
                        }
                        break;
                }
            }
        }

        protected override void OnLostFocus()
        {
            BaseListViewVisualItem visualItem = this.OwnerElement as BaseListViewVisualItem;
            if (visualItem != null &&
                visualItem.IsInValidState(true) &&
                !visualItem.ElementTree.Control.Focused &&
                !visualItem.ElementTree.Control.ContainsFocus)
            {
                visualItem.Data.Owner.EndEdit();
            }
        }
    
        #endregion
    }
}
