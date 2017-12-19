using System;
using System.Windows.Forms;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a DropDownList editor in RadGridView.
    /// </summary>
    [RadToolboxItem(false)]
    public class TreeViewDropDownListEditor : BaseDropDownListEditor
    {   
        #region Overrides

        protected override void OnKeyDown(KeyEventArgs e)
        {
            BaseDropDownListEditorElement editorElement = this.EditorElement as BaseDropDownListEditorElement;
              
            this.selectionStart = editorElement.SelectionStart;

            TreeNodeElement nodeElement = this.OwnerElement as TreeNodeElement;
            if (nodeElement != null)
            {
                if (e.KeyCode == Keys.Enter && e.Modifiers != Keys.Control)
                {
                    nodeElement.TreeViewElement.EndEdit();
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    nodeElement.TreeViewElement.CancelEdit();
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
                TreeNodeElement element = this.OwnerElement as TreeNodeElement;
                if (element != null && element.TreeViewElement != null && element.TreeViewElement.IsEditing)
                {
                    element.TreeViewElement.EndEdit();
                }
            }
        }
         
        protected override void OnLostFocus()
        {
            base.OnLostFocus();

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