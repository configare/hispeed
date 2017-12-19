using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class ListViewDropDownListEditor : BaseDropDownListEditor
    {
        #region Overrides

        protected override void OnKeyDown(KeyEventArgs e)
        {
            BaseDropDownListEditorElement editorElement = this.EditorElement as BaseDropDownListEditorElement;

            this.selectionStart = editorElement.SelectionStart;

            BaseListViewVisualItem itemElement = this.OwnerElement as BaseListViewVisualItem;
            if (itemElement != null)
            {
                if (e.KeyCode == Keys.Enter && e.Modifiers != Keys.Control)
                {
                    itemElement.Data.Owner.EndEdit();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    itemElement.Data.Owner.CancelEdit();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                {
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Down && e.Modifiers == Keys.Alt)
                {
                    ((RadDropDownListElement)this.EditorElement).ShowPopup();
                    e.Handled = true;
                }
            }
        }

        public override void OnValueChanged()
        {
            base.OnValueChanged();

            if (!((BaseDropDownListEditorElement)this.EditorElement).IsPopupOpen)
            {
                BaseListViewVisualItem itemElement = this.OwnerElement as BaseListViewVisualItem;
                if (itemElement != null && itemElement.Data.Owner != null && itemElement.Data.Owner.IsEditing)
                {
                    itemElement.Data.Owner.EndEdit();
                }
            }
        }

        protected override void OnLostFocus()
        {
            base.OnLostFocus();

            BaseListViewVisualItem itemElement = this.OwnerElement as BaseListViewVisualItem;
            if (itemElement != null &&
                itemElement.IsInValidState(true) &&
                !itemElement.ElementTree.Control.Focused &&
                !itemElement.ElementTree.Control.ContainsFocus)
            {
                itemElement.Data.Owner.EndEdit();
            }
        }

        #endregion
    }
}
