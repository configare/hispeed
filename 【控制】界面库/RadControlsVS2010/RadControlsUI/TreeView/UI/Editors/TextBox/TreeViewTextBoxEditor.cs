using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class TreeViewTextBoxEditor : BaseTextBoxEditor
    {
        #region Overrides

        protected override void OnKeyDown(KeyEventArgs e)
        { 
            TreeNodeElement nodeElement = this.OwnerElement as TreeNodeElement;
            if (nodeElement != null)
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        nodeElement.TreeViewElement.CancelEdit();
                        break;

                    case Keys.Enter:
                        nodeElement.TreeViewElement.EndEdit();
                        break;

                    case Keys.Up:
                        if (!this.Multiline || (selectionLength == 0 && isAtFirstLine))
                        {
                            nodeElement.TreeViewElement.EndEdit();
                            nodeElement.TreeViewElement.ProcessKeyDown(e);
                        }
                        break;

                    case Keys.Down:
                        if (!this.Multiline || (selectionLength == 0 && isAtLastLine))
                        {
                            nodeElement.TreeViewElement.EndEdit();
                            nodeElement.TreeViewElement.ProcessKeyDown(e);
                        }
                        break;
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