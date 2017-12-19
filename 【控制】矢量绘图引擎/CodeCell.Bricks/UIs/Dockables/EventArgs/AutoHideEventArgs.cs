using System;
using System.Drawing;

namespace CodeCell.Bricks.UIs.Dockables
{
   /// <summary>
   /// Auto-hide event args
   /// </summary>
   public class AutoHideEventArgs : EventArgs
   {
      #region Fields.

      private zDockMode _selection = zDockMode.None;

      #endregion Fields.

      #region Instance.

      /// <summary>
      /// Create a new instance of <see cref="ContextMenuEventArg"/>
      /// </summary>
      /// <param name="selection">dock mode of the selected panel</param>
      public AutoHideEventArgs (zDockMode selection)
      {
         _selection = selection;
      }

      #endregion Instance.

      #region Public section.

      /// <summary>
      /// Getter for the dock mode of the panel for which the auto-hide state was toggled.
      /// </summary>
      public zDockMode Selection
      {
         get { return _selection; }
      }

      #endregion Public section.
   }
}
