using System.Windows.Forms;
using System;

namespace Telerik.WinControls.UI
{
    public class PropertyGridSpinEditor : BaseSpinEditor
    {
        #region Overrides

        public override void Initialize(object owner, object value)
        {
            PropertyGridItemElement element = owner as PropertyGridItemElement;
            Type editedType = ((PropertyGridItem)element.Data).PropertyType;

            if (editedType == typeof(decimal) || editedType == typeof(double) || editedType == typeof(float))
            {
                ((BaseSpinEditorElement)this.editorElement).DecimalPlaces = 2;
            }
            else
            {
                ((BaseSpinEditorElement)this.editorElement).DecimalPlaces = 0;
            }

            base.Initialize(owner, value);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            BaseSpinEditorElement editorElement = this.EditorElement as BaseSpinEditorElement;

            PropertyGridItemElement visualItem = this.OwnerElement as PropertyGridItemElement;
            if (visualItem != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Return:
                        if (e.Modifiers != Keys.Control)
                        {
                            editorElement.Validate();
                            visualItem.PropertyTableElement.EndEdit();
                        }
                        break;
                    case Keys.Escape:
                        visualItem.Data.PropertyGridTableElement.CancelEdit();
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

        protected override void OnKeyUp(KeyEventArgs e)
        {
            BaseSpinEditorElement editorElement = this.EditorElement as BaseSpinEditorElement;

            PropertyGridItemElement visualItem = this.OwnerElement as PropertyGridItemElement;
            if (visualItem != null)
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
            PropertyGridItemElement visualItem = this.OwnerElement as PropertyGridItemElement;
            if (visualItem != null && visualItem.IsInValidState(true) && !visualItem.ElementTree.Control.Focused)
            {
                visualItem.PropertyTableElement.EndEdit();
            }
        }

        #endregion
    }
}
