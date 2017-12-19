using System;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class PropertyGridColorEditor : BaseColorEditor
    {
        public override void BeginEdit()
        {
            base.BeginEdit();

            ((BaseColorEditorElement)this.editorElement).DialogClosed += new DialogClosedEventHandler(PropertyGridColorEditor_DialogClosed);
        }

        public override bool EndEdit()
        {
            ((BaseColorEditorElement)this.editorElement).DialogClosed -= PropertyGridColorEditor_DialogClosed;

            return base.EndEdit();
        }

        private void PropertyGridColorEditor_DialogClosed(object sender, DialogClosedEventArgs e)
        {
            PropertyGridItemElement visualItem = this.OwnerElement as PropertyGridItemElement;
            if (visualItem != null && visualItem.IsInValidState(true))
            {
                visualItem.PropertyTableElement.EndEdit();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            BaseColorEditorElement editorElement = this.EditorElement as BaseColorEditorElement;

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
                        if (editorElement.ColorTextBox.TextBoxItem.SelectionLength == editorElement.ColorTextBox.TextBoxItem.TextLength)
                        {
                            editorElement.Text = String.Empty;
                        }
                        break;
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            BaseColorEditorElement editorElement = this.EditorElement as BaseColorEditorElement;

            PropertyGridItemElement visualItem = this.OwnerElement as PropertyGridItemElement;
            if (visualItem != null)
            {
                int selectionStart = editorElement.ColorTextBox.TextBoxItem.SelectionStart;
                int selectionLength = editorElement.ColorTextBox.TextBoxItem.SelectionLength;

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
