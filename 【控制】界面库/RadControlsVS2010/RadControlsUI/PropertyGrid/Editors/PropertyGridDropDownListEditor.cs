using System.Windows.Forms;
using System;

namespace Telerik.WinControls.UI
{
    public class PropertyGridDropDownListEditor : BaseDropDownListEditor
    {
        #region Overrides
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            BaseDropDownListEditorElement editorElement = this.EditorElement as BaseDropDownListEditorElement;

            this.selectionStart = editorElement.SelectionStart;

            PropertyGridItemElement visualItem = this.OwnerElement as PropertyGridItemElement;
            if (visualItem != null)
            {
                if (e.KeyCode == Keys.Enter && e.Modifiers != Keys.Control)
                {
                    visualItem.PropertyTableElement.EndEdit();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    visualItem.PropertyTableElement.CancelEdit();
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

        protected override void OnLostFocus()
        {
            base.OnLostFocus();

            PropertyGridItemElement visualItem = this.OwnerElement as PropertyGridItemElement;
            if (visualItem != null && visualItem.IsInValidState(true) && !visualItem.ElementTree.Control.Focused)
            {
                visualItem.PropertyTableElement.EndEdit();
            }
        }

        public override void Initialize(object owner, object value)
        {
            base.Initialize(owner, value);

            PropertyGridItemElement element = owner as PropertyGridItemElement;
            PropertyGridItem item = element.Data as PropertyGridItem;

            if (item == null)
            {
                return;
            }

            if (item.PropertyType.IsEnum)
            {
                BaseDropDownListEditorElement editorElement = this.editorElement as BaseDropDownListEditorElement;
                editorElement.ListElement.BindingContext = item.PropertyGridTableElement.BindingContext;
                editorElement.DataSource = Enum.GetValues(item.PropertyType);
                editorElement.SelectedValue = item.Value;
            }
            else if(item.PropertyType == typeof(bool))
            {
                BaseDropDownListEditorElement editorElement = this.editorElement as BaseDropDownListEditorElement;
                editorElement.ListElement.BindingContext = item.PropertyGridTableElement.BindingContext;
                
                RadListDataItem trueItem = new RadListDataItem(bool.TrueString, true);
                RadListDataItem falseItem = new RadListDataItem(bool.FalseString, false);
                editorElement.Items.Add(trueItem);
                editorElement.Items.Add(falseItem);

                editorElement.SelectedValue = item.Value;
            }
        }
        
        #endregion
    }
}
