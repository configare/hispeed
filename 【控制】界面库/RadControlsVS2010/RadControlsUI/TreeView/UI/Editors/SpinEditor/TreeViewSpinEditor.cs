using System;
using System.Collections.Generic;

using System.Text;
using System.Drawing.Design;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a spin editor in RadGridView.
    /// </summary>
    [ToolboxItem(false)]
    public class TreeViewSpinEditor : BaseSpinEditor
    {
        #region Overrides

        protected override void OnKeyDown(KeyEventArgs e)
        {
            BaseSpinEditorElement editorElement = this.EditorElement as BaseSpinEditorElement;
             
            TreeNodeElement treeNodeElement = this.OwnerElement as TreeNodeElement;
            if (treeNodeElement != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Return:
                        if (e.Modifiers != Keys.Control)
                        {
                            editorElement.Validate();
                            treeNodeElement.TreeViewElement.EndEdit();
                        }
                        break;

                    case Keys.Escape:
                        treeNodeElement.TreeViewElement.CancelEdit();
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
              
            TreeNodeElement treeNodeElement = this.OwnerElement as TreeNodeElement;
            if (treeNodeElement != null)
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
