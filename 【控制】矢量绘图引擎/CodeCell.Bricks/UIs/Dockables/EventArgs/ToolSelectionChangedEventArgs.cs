using System;
using System.Drawing;
using System.Windows.Forms;

namespace CodeCell.Bricks.UIs.Dockables
{
   /// <summary>
   /// Tool selection changed event args
   /// </summary>
   public class ToolSelectionChangedEventArgs : EventArgs
   {
      #region Fields.

      private DockableToolWindow          _selection        = null;

      #endregion Fields.

      #region Instance.

      /// <summary>
      /// Create a new instance of <see cref="ContextMenuEventArg"/>
      /// </summary>
      /// <param name="selection">selected tool window</param>
      public ToolSelectionChangedEventArgs (DockableToolWindow selection)
      {
         _selection     = selection;
      }

      #endregion Instance.

      #region Public section.

      /// <summary>
      /// Getter for the tool window which was selected when context menu
      /// is requested.
      /// </summary>
      public DockableToolWindow Selection
      {
         get { return _selection; }
      }

      #endregion Public section.
   }
}
