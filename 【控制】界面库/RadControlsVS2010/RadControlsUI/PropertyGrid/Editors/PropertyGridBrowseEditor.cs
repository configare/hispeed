using System;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class PropertyGridBrowseEditor : BaseBrowseEditor
    {
        public override void BeginEdit()
        {
            ((BaseBrowseEditorElement)this.editorElement).DialogClosed += new DialogClosedEventHandler(PropertyGridBrowseEditor_DialogClosed);

            base.BeginEdit();
        }

        public override bool EndEdit()
        {
            ((BaseBrowseEditorElement)this.editorElement).DialogClosed -= PropertyGridBrowseEditor_DialogClosed;

            return base.EndEdit();
        }

        void PropertyGridBrowseEditor_DialogClosed(object sender, DialogClosedEventArgs e)
        {
            PropertyGridItemElement visualItem = this.OwnerElement as PropertyGridItemElement;
            if (visualItem != null && visualItem.IsInValidState(true))
            {
                visualItem.PropertyTableElement.EndEdit();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            BaseBrowseEditorElement editorElement = this.EditorElement as BaseBrowseEditorElement;

            PropertyGridItemElement visualItem = this.OwnerElement as PropertyGridItemElement;
            if (visualItem != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        if (this.Validate())
                        {
                            visualItem.PropertyTableElement.EndEdit();
                        }
                        break;
                    case Keys.Escape:
                        visualItem.Data.PropertyGridTableElement.CancelEdit();
                        break;
                    case Keys.Delete:
                        if (editorElement.FilePathTextBox.TextBoxItem.SelectionLength == editorElement.FilePathTextBox.TextBoxItem.TextLength)
                        {
                            editorElement.Text = String.Empty;
                        }
                        break;
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            BaseBrowseEditorElement editorElement = this.EditorElement as BaseBrowseEditorElement;

            PropertyGridItemElement visualItem = this.OwnerElement as PropertyGridItemElement;
            if (visualItem != null)
            {
                int selectionStart = editorElement.FilePathTextBox.TextBoxItem.SelectionStart;
                int selectionLength = editorElement.FilePathTextBox.TextBoxItem.SelectionLength;

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

        protected override void OnLostFocus()
        {
            base.OnLostFocus();

            PropertyGridItemElement visualItem = this.OwnerElement as PropertyGridItemElement;
            if (visualItem != null && visualItem.IsInValidState(true) && !visualItem.ElementTree.Control.Focused)
            {
                visualItem.PropertyTableElement.EndEdit();
            }
        }
    }
}
