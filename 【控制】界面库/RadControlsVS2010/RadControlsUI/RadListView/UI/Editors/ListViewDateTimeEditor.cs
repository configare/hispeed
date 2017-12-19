using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class ListViewDateTimeEditor : BaseDateTimeEditor
    {
        #region Overrides

        protected override void OnKeyDown(KeyEventArgs e)
        {
            RadDateTimePickerElement dateTimePickerElement = this.EditorElement as RadDateTimePickerElement;
            if (dateTimePickerElement == null || !dateTimePickerElement.IsInValidState(true))
            {
                return;
            }

            BaseListViewVisualItem itemElement = this.OwnerElement as BaseListViewVisualItem;
            if (itemElement != null)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    itemElement.Data.Owner.EndEdit();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    itemElement.Data.Owner.CancelEdit();
                }
                else
                {
                    base.OnKeyDown(e);
                }
            }
        }

        protected override void OnLostFocus()
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
