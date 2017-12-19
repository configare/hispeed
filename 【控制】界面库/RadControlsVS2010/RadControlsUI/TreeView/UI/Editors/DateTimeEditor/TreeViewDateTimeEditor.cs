using System;
using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a date time editor in RadGridView.
    /// </summary>
    [RadToolboxItem(false)]
    public class TreeViewDateTimeEditor : BaseDateTimeEditor
    {
        #region Event handlers

        protected override void OnKeyDown(KeyEventArgs e)
        { 
            RadDateTimePickerElement dateTimePickerElement = this.EditorElement as RadDateTimePickerElement;
            if (dateTimePickerElement == null || !dateTimePickerElement.IsInValidState(true))
            {
                return;
            }

            TreeNodeElement treeNode = this.OwnerElement as TreeNodeElement;
            if (treeNode != null)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    treeNode.TreeViewElement.EndEdit();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    treeNode.TreeViewElement.CancelEdit();
                }
                else
                {
                    base.OnKeyDown(e);
                }
            }
        }
        
        protected override void OnLostFocus()
        {
            TreeNodeElement treeNode = this.OwnerElement as TreeNodeElement;
            if (treeNode != null &&
                treeNode.IsInValidState(true) &&
                !treeNode.ElementTree.Control.Focused &&
                !treeNode.ElementTree.Control.ContainsFocus)
            {
                treeNode.TreeViewElement.EndEdit();
            }
        }

        #endregion
    }
}
