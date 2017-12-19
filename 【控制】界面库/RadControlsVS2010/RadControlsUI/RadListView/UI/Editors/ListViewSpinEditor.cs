using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class ListViewSpinEditor : BaseSpinEditor
    {
        #region Overrides

        protected override void OnKeyDown(KeyEventArgs e)
        {
            BaseSpinEditorElement editorElement = this.EditorElement as BaseSpinEditorElement;

            BaseListViewVisualItem itemElement = this.OwnerElement as BaseListViewVisualItem;
            if (itemElement != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Return:
                        if (e.Modifiers != Keys.Control)
                        {
                            editorElement.Validate();
                            itemElement.Data.Owner.EndEdit();
                        }
                        break;

                    case Keys.Escape:
                        itemElement.Data.Owner.CancelEdit();
                        break;

                    case Keys.Delete:
                        if (selectionLength == editorElement.TextBoxItem.TextLength)
                        {
                            editorElement.Text = null;
                        }
                        break;
                }
            }
        }

        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            BaseSpinEditorElement editorElement = this.EditorElement as BaseSpinEditorElement;

            BaseListViewVisualItem itemElement = this.OwnerElement as BaseListViewVisualItem;
            if (itemElement != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Right:
                        if ((RightToLeft && selectionStart == 0) || (!RightToLeft && selectionStart == editorElement.Text.Length))
                        {
                            editorElement.Validate();
                        }
                        break;

                    case Keys.Left:
                        if ((RightToLeft && selectionStart == editorElement.Text.Length) || (!RightToLeft && selectionStart == 0 && selectionLength == 0))
                        {
                            editorElement.Validate();
                        }
                        break;
                }
            }
        }

        public override void OnLostFocus()
        {
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
