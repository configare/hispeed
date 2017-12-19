using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class PropertyGridTextBoxEditor : BaseTextBoxEditor
    {
        #region Overrides

        protected override void OnKeyDown(KeyEventArgs e)
        {
            PropertyGridItemElement visualItem = this.OwnerElement as PropertyGridItemElement;
            if (visualItem != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        visualItem.PropertyTableElement.CancelEdit();
                        break;
                    case Keys.Enter:
                        visualItem.PropertyTableElement.EndEdit();
                        break;
                    case Keys.Up:
                        if (!this.Multiline || (selectionLength == 0 && isAtFirstLine))
                        {
                            visualItem.PropertyTableElement.EndEdit();
                            visualItem.PropertyTableElement.ProcessKeyDown(e);
                        }
                        break;
                    case Keys.Down:
                        if (!this.Multiline || (selectionLength == 0 && isAtLastLine))
                        {
                            visualItem.PropertyTableElement.EndEdit();
                            visualItem.PropertyTableElement.ProcessKeyDown(e);
                        }
                        break;
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