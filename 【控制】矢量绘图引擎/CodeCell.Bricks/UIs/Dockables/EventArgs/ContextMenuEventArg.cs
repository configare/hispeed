using System;
using System.Drawing;
using System.Windows.Forms;

namespace CodeCell.Bricks.UIs.Dockables
{
   /// <summary>
   /// Context menu event args
   /// </summary>
   public class ContextMenuEventArg : EventArgs
   {
      #region Fields.

      private DockableToolWindow          _selection        = null;
      private Point                       _mouseLocation    = new Point();
      private MouseButtons                _mouseButtons     = MouseButtons.None;

      #endregion Fields.

      #region Instance.

      /// <summary>
      /// Create a new instance of <see cref="ContextMenuEventArg"/>
      /// </summary>
      /// <param name="selection">selected tool window</param>
      /// <param name="mouseLocation">mouse location in screen coordinates, when context menu is requested.</param>
      /// <param name="mouseButtons">mouse buttons pressed when context menu is requested.</param>
      public ContextMenuEventArg (DockableToolWindow selection, Point mouseLocation, MouseButtons mouseButtons)
      {
         _selection     = selection;
         _mouseLocation = mouseLocation;
         _mouseButtons  = mouseButtons;
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

      /// <summary>
      /// Mouse location in screen coordinates, when context menu is requested. 
      /// </summary>
      public Point MouseLocation
      {
         get { return _mouseLocation; }
      }

      /// <summary>
      /// Mouse buttons pressed when context menu is requested.
      /// </summary>
      public MouseButtons MouseButtons
      {
         get { return _mouseButtons; }
      }

      #endregion Public section.
   }
}
