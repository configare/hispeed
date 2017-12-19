using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class PropertyGridDateTimeEditor : BaseDateTimeEditor
    {
        #region Overrides
        
        protected override void OnKeyDown(KeyEventArgs e)
        {
            RadDateTimePickerElement dateTimePickerElement = this.EditorElement as RadDateTimePickerElement;
            if (dateTimePickerElement == null || !dateTimePickerElement.IsInValidState(true))
            {
                return;
            }

            PropertyGridItemElement visualItem = this.OwnerElement as PropertyGridItemElement;
            if (visualItem != null)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    visualItem.PropertyTableElement.EndEdit();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    visualItem.PropertyTableElement.CancelEdit();
                }
                else
                {
                    base.OnKeyDown(e);
                }
            }
        }

        protected override void OnLostFocus()
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
