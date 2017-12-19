/***************************************************************************
 *   CopyRight (C) 2008 by SC Crom-Osec SRL                                *
 *   Contact:  contact@osec.ro                                             *
 *                                                                         *
 *   This program is free software; you can redistribute it and/or modify  *
 *   it under the terms of the Crom Free License as published by           *
 *   the SC Crom-Osec SRL; version 1 of the License                        *
 *                                                                         *
 *   This program is distributed in the hope that it will be useful,       *
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of        *
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the         *
 *   Crom Free License for more details.                                   *
 *                                                                         *
 *   You should have received a copy of the Crom Free License along with   *
 *   this program; if not, write to the contact@osec.ro                    *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace CodeCell.Bricks.UIs.Dockables
{
   /// <summary>
   /// Implements resize of the DockPanels using the splitter.
   /// </summary>
   internal class DockPanelsResizer : IDisposable
   {
      #region Fields.

      private const int                SplitterDimension                = 3;

      private Control                  _container                       = null;
      private Form                     _splitter                        = new Form ();
      private zDockMode                _resizedPanel                    = zDockMode.None;
      private int                      _minViewWidth                    = DockPanelsLayout.MinViewHeight;
      private int                      _minViewHeight                   = DockPanelsLayout.MinViewHeight;
      //private bool                     _mouseButtonDown                 = false;
      private bool                     _isCursorChanged                 = false;

      private DockPanelsLayout         _layout                          = null;

      #endregion Fields.

      #region Instance.

      /// <summary>
      /// Consturctor
      /// </summary>
      /// <param name="container">container</param>
      public DockPanelsResizer (Control container)
      {
         _layout = new DockPanelsLayout();
         _layout.UpdateLayoutRequested += new EventHandler (OnUpdateLayoutRequested);

         _container  = container;

         _container.Resize    += OnContainerResize;
         _container.MouseDown += OnMouseDownInContainer;
         _container.MouseMove += OnMouseMovedInContainer;
         _container.MouseUp   += OnMouseUpInContainer;

         _splitter.FormBorderStyle = FormBorderStyle.None;
         _splitter.MinimumSize     = new Size (1, 1);
         _splitter.Width           = 1;
         _splitter.ShowInTaskbar   = false;
         _splitter.TopMost         = true;
         _splitter.BackColor       = Color.DarkGray;
         _splitter.Opacity         = 0.90;
         _splitter.StartPosition   = FormStartPosition.Manual;
         _splitter.AutoScaleMode   = AutoScaleMode.None;
         _splitter.Margin          = new Padding();
         _splitter.AutoScaleDimensions = new SizeF(1, 1);
         _splitter.Bounds          = new Rectangle(0, 0, 2, 2);

         UpdatePanelsLayout();
      }

      #endregion Instance.

      #region IDisposable.

      /// <summary>
      /// Distrugere
      /// </summary>
      public void Dispose ()
      {
         if (_container != null)
         {
            _container.Resize    -= OnContainerResize;
            _container.MouseDown -= OnMouseDownInContainer;
            _container.MouseMove -= OnMouseMovedInContainer;
            _container.MouseUp   -= OnMouseUpInContainer;
         }

         if (_splitter != null)
         {
            _splitter.Dispose();
            _splitter = null;
         }

         if (_container != null)
         {
            _container = null;
         }

         if (_layout != null)
         {
            _layout.Dispose();
            _layout = null;
         }

         GC.SuppressFinalize (this);
      }

      #endregion IDisposable.

      #region Public section.
      
       //by fdc
      public DockPanelsLayout DockPanelsLayout
      {
          get { return _layout; }
      }


      /// <summary>
      /// This event occurs when the minimum allowed size for the container was changed.
      /// The form on which this container is placed should display the entire container
      /// </summary>
      public event EventHandler MinimumSizeChanged;

      /// <summary>
      /// Get the panel with the given dock mode
      /// </summary>
      /// <param name="dockMode">dock mode which can be Left, Right, Top, Bottom or Fill</param>
      /// <returns>panel with given dock mode</returns>
      public DockPanel GetPanel (zDockMode dockMode)
      {
         switch (dockMode)
         {
            case zDockMode.Left:
               return _layout.LeftPanel;

            case zDockMode.Right:
               return _layout.RightPanel;

            case zDockMode.Top:
               return _layout.TopPanel;

            case zDockMode.Bottom:
               return _layout.BottomPanel;

            case zDockMode.Fill:
               return _layout.CenterPanel;

            default:
               return null;
         }
      }

      /// <summary>
      /// Dock the tool window to the specified panel.
      /// </summary>
      /// <param name="toolWindow">tool window to be docked</param>
      /// <param name="dockMode">dock mode can be Left, Right, Top, Bottom and Fill</param>
      public void DockToolWindow (DockableToolWindow toolWindow, zDockMode dockMode)
      {
         DockPanel panel = GetPanel(dockMode);
         if (panel != null)
         {
            panel.DockToolWindow(toolWindow);
         }
      }

      /// <summary>
      /// Undock the specified tool window
      /// </summary>
      /// <param name="toolWindow">tool window to be undocked</param>
      public void UndockToolWindow (DockableToolWindow toolWindow)
      {
         DockPanel panel = GetPanel (toolWindow.DockMode);
         if (panel != null)
         {
            panel.UndockToolWindow(toolWindow);
         }
      }

      /// <summary>
      /// Checks if the given tool window is docked in a side panel which is hidden.
      /// </summary>
      /// <param name="toolWindow">tool window to be checked</param>
      /// <returns>setat daca toolWindow nu e in nici un panou sau daca e intr-un panou vizibil</returns>
      public bool IsVisible (DockableToolWindow toolWindow)
      {
         SideDockPanel panel = GetPanel (toolWindow.DockMode) as SideDockPanel;
         if (panel != null)
         {
            return panel.AutoHidden == false;
         }

         return toolWindow.Visible;
      }

      /// <summary>
      /// Checks if the panel with given dock mode has the AutoHide flag set.
      /// </summary>
      /// <param name="dockMode">dock mode to identify the panel to check. 
      /// Allowed values are Left, Right, Top, Bottom. Other values will be ignored and false will be returned.</param>
      /// <returns>the value of auto-hide flag for identified panel or false if no valid panel was specified.</returns>
      public bool IsAutoHide (zDockMode dockMode)
      {
         SideDockPanel sideDock = GetPanel(dockMode) as SideDockPanel;
         if (sideDock == null)
         {
            return false;
         }

         return sideDock.AutoHide;
      }

      /// <summary>
      /// Set auto-hide flag for the panel with given dock mode
      /// </summary>
      /// <param name="dockMode">dock mode to identify the panel to change. 
      /// Allowed values are Left, Right, Top, Bottom. Other values will be ignored.</param>
      /// <param name="autoHidden">new auto-hide value</param>
      public void SetAutoHide (zDockMode dockMode, bool autoHide)
      {
         SideDockPanel sideDock = GetPanel (dockMode) as SideDockPanel;
         if (sideDock == null)
         {
            return;
         }

         sideDock.AutoHide = autoHide;
      }

      /// <summary>
      /// Set auto-hidden flag for the panel with given dock mode
      /// </summary>
      /// <param name="dockMode">dock mode to identify the panel to change. 
      /// Allowed values are Left, Right, Top, Bottom. Other values will be ignored.</param>
      /// <param name="autoHidden">new auto-hidden value</param>
      public void SetAutoHidden (zDockMode dockMode, bool autoHidden)
      {
         SideDockPanel sidePanel = GetPanel (dockMode) as SideDockPanel;
         if (sidePanel == null)
         {
            return;
         }

         if (sidePanel.AutoHide == false)
         {
            return;
         }

         sidePanel.AutoHidden = autoHidden;

         UpdatePanelsLayout ();
      }

      /// <summary>
      /// Get the fixed buttons bounds in screen coordinates
      /// </summary>
      /// <remarks>
      /// throws a NotSupportedException if the panel is not found using the dockMode criteria
      /// </remarks>
      /// <param name="dockMode">dock mode of the panel for which bounds are requested.
      /// Valid values are Left and Right.</param>
      /// <returns>bounds of the region in which buttons can be drawn for the panel identified by dockMode</returns>
      public Rectangle GetFixedButtonsBounds (zDockMode panou)
      {
         if (panou == zDockMode.Left)
         {
            return _layout.LeftBottomButtonsBounds;
         }

         if (panou == zDockMode.Right)
         {
            return _layout.RightBottomButtonsBounds;
         }

         throw new NotSupportedException ();
      }

      /// <summary>
      /// Get the panel buttons bounds in screen coordinates
      /// </summary>
      /// <remarks>
      /// throws a NotSupportedException if the panel is not found using the dockMode criteria
      /// </remarks>
      /// <param name="dockMode">dock mode of the panel for which bounds are requested.
      /// Valid values are Left, Right, Top, Bottom and Fill.</param>
      /// <returns>bounds of the region in which buttons can be drawn for the panel identified by dockMode</returns>
      public Rectangle GetPanelButtonsBounds (zDockMode dockMode)
      {
         DockPanel panel = GetPanel (dockMode);
         if (panel == null)
         {
            throw new NotSupportedException ();
         }

         return panel.ButtonsBounds;
      }

      /// <summary>
      /// Get the panel bounds in screen coordinates, when the panel is not hidden
      /// </summary>
      /// <param name="dockMode">dock mode of the panel for which bounds are requested</param>
      /// <returns>panel bounds in screen coordinates (computed when the panel is not hidden)</returns>
      public Rectangle GetPanelNonHiddenBounds (zDockMode dockMode)
      {
         if (dockMode == zDockMode.Left)
         {
            return _container.RectangleToScreen (_layout.LeftPanel.PreviewBounds);
         }

         if (dockMode == zDockMode.Right)
         {
            return _container.RectangleToScreen (_layout.RightPanel.PreviewBounds);
         }

         if (dockMode == zDockMode.Top)
         {
            return _container.RectangleToScreen (_layout.TopPanel.PreviewBounds);
         }

         if (dockMode == zDockMode.Bottom)
         {
            return _container.RectangleToScreen (_layout.BottomPanel.PreviewBounds);
         }

         if (dockMode == zDockMode.Fill)
         {
            return _container.RectangleToScreen (_layout.CenterPanel.PreviewBounds);
         }

         throw new NotSupportedException();
      }

      /// <summary>
      /// Get the bounds (in screen coordinates) of the splitter attached to 
      /// the panel identified by the dockMode
      /// </summary>
      /// <remarks>
      /// throws a NotSupportedException if the panel is not found using the dockMode criteria
      /// </remarks>
      /// <param name="dockMode">dock mode of the panel for which bounds are requested.
      /// Valid values are Left, Right, Top, Bottom.</param>
      /// <returns>bounds of the splitter</returns>
      public Rectangle GetPanelSplitterBounds (zDockMode dockMode)
      {
         SideDockPanel panel = GetPanel (dockMode) as SideDockPanel;
         if (panel == null)
         {
            throw new NotSupportedException ();
         }

         return panel.SplitterBounds;
      }

      /// <summary>
      /// Get all the tool windows docked on the panel identified by given dock mode parameter
      /// </summary>
      /// <remarks>
      /// throws a NotSupportedException if the panel is not found using the dockMode criteria
      /// </remarks>
      /// <param name="dockMode">dock mode of the panel for which bounds are requested.
      /// Valid values are Left, Right, Top, Bottom and Fill.</param>
      /// <returns>vector of tool windows from the panel</returns>
      public DockableToolWindow[] GetPanelToolWindows (zDockMode dockMode)
      {
         DockPanel panel = GetPanel (dockMode);
         if (panel == null)
         {
            throw new NotSupportedException ();
         }

         return panel.ToolWindows;
      }

      /// <summary>
      /// Get all the visible tool windows docked on the panel identified by given dock mode parameter
      /// </summary>
      /// <remarks>
      /// throws a NotSupportedException if the panel is not found using the dockMode criteria
      /// </remarks>
      /// <param name="dockMode">dock mode of the panel for which bounds are requested.
      /// Valid values are Left, Right, Top, Bottom and Fill.</param>
      /// <returns>vector of tool windows from the panel</returns>
      public DockableToolWindow[] GetPanelVisibleToolWindows (zDockMode dockMode)
      {
         DockPanel panel = GetPanel(dockMode);
         if (panel == null)
         {
            throw new NotSupportedException();
         }

         return panel.VisibleToolWindows;
      }

      /// <summary>
      /// Update the mouse cursor depending on the mouse position
      /// </summary>
      /// <param name="mousePosition">mouse position in screen coordinates</param>
      public void UpdateMouseCursor (Point mousePosition)
      {
         if (_layout.LeftPanel.SplitterBounds.Contains (mousePosition))
         {
            _container.Cursor = Cursors.VSplit;
            _isCursorChanged  = true;
         }
         else if (_layout.RightPanel.SplitterBounds.Contains (mousePosition))
         {
            _container.Cursor = Cursors.VSplit;
            _isCursorChanged  = true;
         }
         else if (_layout.TopPanel.SplitterBounds.Contains (mousePosition))
         {
            _container.Cursor = Cursors.HSplit;
            _isCursorChanged  = true;
         }
         else if (_layout.BottomPanel.SplitterBounds.Contains (mousePosition))
         {
            _container.Cursor = Cursors.HSplit;
            _isCursorChanged  = true;
         }
         else
         {
            _container.Cursor = Cursors.Default;
            _isCursorChanged  = false;
         }
      }

      /// <summary>
      /// Flag indicating if the cursor is changed due to <see cref="UpdateMouseCursor">UpdateMouseCursor</see> call.
      /// </summary>
      public bool IsCursorChanged
      {
         get { return _isCursorChanged; }
      }

      /// <summary>
      /// Gets the top most tool window from the panel identified by given dock mode
      /// </summary>
      /// <param name="dockMode">dock mode of the panel for which bounds are requested.
      /// Valid values are Left, Right, Top, Bottom and Fill.</param>
      /// <returns>the tool window which is in the top of z-order on the panel identified by dock mode</returns>
      public DockableToolWindow GetTopMostToolWindow (zDockMode dockMode)
      {
         DockableToolWindow topMost = null; // Is the window with smallest index in the container collection

         int smallestIndex = Int32.MaxValue;

         DockableToolWindow[] toolWindows = GetPanelVisibleToolWindows(dockMode);
         foreach (DockableToolWindow toolWindow in toolWindows)
         {
            int zOrderIndex = _container.Controls.GetChildIndex(toolWindow);
            if (zOrderIndex < smallestIndex)
            {
               topMost       = toolWindow;
               smallestIndex = zOrderIndex;
            }
         }

         return topMost;
      }

      /// <summary>
      /// Left panel width
      /// </summary>
      public int LeftPanelWidth
      {
         get { return _layout.LeftPanel.NotHiddenDimension; }
         set
         {
            int newRight = value + _layout.LeftPanel.ContentBounds.Left;
            newRight     = Math.Min (_layout.LeftPanel.MaxSlidePos, Math.Max (_layout.LeftPanel.MinSlidePos, newRight));
            _layout.LeftPanel.NotHiddenDimension = Math.Max (DockPanelsLayout.MinPanelDimension, newRight - _layout.LeftPanel.ContentBounds.Left);
         }
      }

      /// <summary>
      /// Right panel width
      /// </summary>
      public int RightPanelWidth
      {
         get { return _layout.RightPanel.NotHiddenDimension; }
         set
         {
            int newLeft = _layout.RightPanel.ContentBounds.Right - value;
            newLeft     = Math.Min (_layout.RightPanel.MaxSlidePos, Math.Max (_layout.RightPanel.MinSlidePos, newLeft));
            _layout.RightPanel.NotHiddenDimension = Math.Max (DockPanelsLayout.MinPanelDimension, _layout.RightPanel.ContentBounds.Right - newLeft);
         }
      }

      /// <summary>
      /// Top panel height
      /// </summary>
      public int TopPanelHeight
      {
         get { return _layout.TopPanel.NotHiddenDimension; }
         set
         {
            int newBottom = value + _layout.TopPanel.ContentBounds.Top;
            newBottom     = Math.Min (_layout.TopPanel.MaxSlidePos, Math.Max (_layout.TopPanel.MinSlidePos, newBottom));
            _layout.TopPanel.NotHiddenDimension = Math.Max (DockPanelsLayout.MinPanelDimension, newBottom - _layout.TopPanel.ContentBounds.Top);
         }
      }

      /// <summary>
      /// Bottom panel height
      /// </summary>
      public int BottomPanelHeight
      {
         get { return _layout.BottomPanel.NotHiddenDimension; }
         set
         {
            int newTop = _layout.BottomPanel.ContentBounds.Bottom - value;
            newTop     = Math.Min (_layout.BottomPanel.MaxSlidePos, Math.Max (_layout.BottomPanel.MinSlidePos, newTop));
            _layout.BottomPanel.NotHiddenDimension = Math.Max (DockPanelsLayout.MinPanelDimension, _layout.BottomPanel.ContentBounds.Bottom - newTop);
         }
      }

      #endregion Public section.

      #region Private section.
      #region Received events.

      /// <summary>
      /// La apasare button cursor in container
      /// </summary>
      /// <param name="sender">sender</param>
      /// <param name="e">e</param>
      private void OnMouseDownInContainer (object sender, MouseEventArgs e)
      {
         if (e.Button != MouseButtons.Left)
         {
            return;
         }

         //_mouseButtonDown         = true;
         Rectangle splitterBounds = new Rectangle ();
         Cursor cursor            = null;
         _resizedPanel            = zDockMode.None;

         if (_layout.RightPanel.SplitterBounds.Contains (e.Location))
         {
            cursor         = Cursors.VSplit;
            splitterBounds = _container.RectangleToScreen(_layout.RightPanel.SplitterBounds);
            _resizedPanel  = zDockMode.Right;
         }
         else if (_layout.LeftPanel.SplitterBounds.Contains (e.Location))
         {
            cursor         = Cursors.VSplit;
            splitterBounds = _container.RectangleToScreen(_layout.LeftPanel.SplitterBounds);
            _resizedPanel  = zDockMode.Left;
         }
         else if (_layout.TopPanel.SplitterBounds.Contains (e.Location))
         {
            cursor         = Cursors.HSplit;
            splitterBounds = _container.RectangleToScreen(_layout.TopPanel.SplitterBounds);
            _resizedPanel  = zDockMode.Top;
         }
         else if (_layout.BottomPanel.SplitterBounds.Contains (e.Location))
         {
            cursor         = Cursors.HSplit;
            splitterBounds = _container.RectangleToScreen(_layout.BottomPanel.SplitterBounds);
            _resizedPanel  = zDockMode.Bottom;
         }

         if (splitterBounds.Width != 0 && splitterBounds.Height != 0 && cursor != null)
         {
            // First reset splitter to prevent flickering - this is not normal but is happening in ms windows
            _splitter.Bounds    = new Rectangle(-100, -100, 1, 1);
            _splitter.Visible   = true;

            // Then set the splitter bounds
            _splitter.Bounds    = splitterBounds;
            _splitter.Cursor    = cursor;
            _splitter.Visible   = true;
            _container.Cursor    = cursor;
         }
      }

      /// <summary>
      /// La mutare cursor in container
      /// </summary>
      /// <param name="sender">sender</param>
      /// <param name="e">e</param>
      private void OnMouseMovedInContainer (object sender, MouseEventArgs e)
      {
         Point cursorPos  = Control.MousePosition;

         if (_splitter.Visible && _resizedPanel != zDockMode.None)
         {
            _container.Cursor = _splitter.Cursor;

            if (_resizedPanel == zDockMode.Left)
            {
               _splitter.Left = Math.Min (Math.Max (ScreenX(_layout.LeftPanel.MinSlidePos), cursorPos.X), ScreenX(_layout.LeftPanel.MaxSlidePos));
            }
            else if (_resizedPanel == zDockMode.Right)
            {
               _splitter.Left = Math.Min (Math.Max (ScreenX(_layout.RightPanel.MinSlidePos), cursorPos.X), ScreenX(_layout.RightPanel.MaxSlidePos));
            }
            else if (_resizedPanel == zDockMode.Top)
            {
               _splitter.Top = Math.Min (Math.Max (ScreenY(_layout.TopPanel.MinSlidePos), cursorPos.Y), ScreenY(_layout.TopPanel.MaxSlidePos));
            }
            else if (_resizedPanel == zDockMode.Bottom)
            {
               _splitter.Top = Math.Min (Math.Max (ScreenY(_layout.BottomPanel.MinSlidePos), cursorPos.Y), ScreenY(_layout.BottomPanel.MaxSlidePos));
            }

            _isCursorChanged = true;
         }
         else
         {
            UpdateMouseCursor(e.Location);
         }
      }

      /// <summary>
      /// La ridicare button cursor din container
      /// </summary>
      /// <param name="sender">sender</param>
      /// <param name="e">e</param>
      private void OnMouseUpInContainer (object sender, MouseEventArgs e)
      {
         Point cursorPos = Control.MousePosition;

         if (_resizedPanel == zDockMode.Left)
         {
            int screenRight   = Math.Min (Math.Max (ScreenX(_layout.LeftPanel.MinSlidePos), cursorPos.X), ScreenX(_layout.LeftPanel.MaxSlidePos));
            LeftPanelWidth    = ClientX (screenRight) - _layout.LeftPanel.ContentBounds.X;
         }
         else if (_resizedPanel == zDockMode.Right)
         {
            int screenLeft    = Math.Min (Math.Max (ScreenX(_layout.RightPanel.MinSlidePos), cursorPos.X), ScreenX(_layout.RightPanel.MaxSlidePos));
            RightPanelWidth   = _layout.RightPanel.ContentBounds.Right - ClientX (screenLeft);
         }
         else if (_resizedPanel == zDockMode.Top)
         {
            int screenBottom  = Math.Min (Math.Max (ScreenY(_layout.TopPanel.MinSlidePos), cursorPos.Y), ScreenY(_layout.TopPanel.MaxSlidePos));
            TopPanelHeight    = ClientY (screenBottom) - _layout.TopPanel.ContentBounds.Top;
         }
         else if (_resizedPanel == zDockMode.Bottom)
         {
            int screenTop     = Math.Min (Math.Max (ScreenY(_layout.BottomPanel.MinSlidePos), cursorPos.Y), ScreenY(_layout.BottomPanel.MaxSlidePos));
            BottomPanelHeight = _layout.BottomPanel.ContentBounds.Bottom - ClientY (screenTop);
         }

         //_mouseButtonDown        = false;
         _splitter.Visible   = false;
         _container.Cursor    = Cursors.Default;

         if (_resizedPanel != zDockMode.None)
         {
            _resizedPanel = zDockMode.None;
            UpdatePanelsLayout();
         }
      }

      /// <summary>
      /// La redimensionare container
      /// </summary>
      /// <param name="sender">sender</param>
      /// <param name="e">e</param>
      private void OnContainerResize (object sender, EventArgs e)
      {
         UpdatePanelsLayout();
      }

      /// <summary>
      /// On request to update the layout
      /// </summary>
      /// <param name="sender">sender</param>
      /// <param name="e">e</param>
      private void OnUpdateLayoutRequested (object sender, EventArgs e)
      {
         UpdatePanelsLayout();
      }

      #endregion Received events.

      /// <summary>
      /// Actualizeaza dimensiunile panourilor
      /// </summary>
      /// <returns>modurile actualizate</returns>
      public zDockMode UpdatePanelsLayout ()
      {
         zDockMode modActualizat = zDockMode.None;
         if (_layout.UpdateLeftPanelLayout (_container.ClientSize))
         {
            modActualizat |= zDockMode.Left;
         }

         if (_layout.UpdateRightPanelLayout (_container.ClientSize))
         {
            modActualizat |= zDockMode.Right;
         }

         if (_layout.UpdateTopPanelLayout (_container.ClientSize))
         {
            modActualizat |= zDockMode.Top;
         }

         if (_layout.UpdateBottomPanelLayout (_container.ClientSize))
         {
            modActualizat |= zDockMode.Bottom;
         }

         if (_layout.UpdateCenterPanelLayout (_container.ClientSize))
         {
            modActualizat |= zDockMode.Fill;
         }


         int minimumWidth =
            // Fit left panel
            DockPanelsLayout.ButtonsPanelDimension + _layout.LeftPanel.NotHiddenDimension  + SplitterDimension +
            // Fit right panel
            DockPanelsLayout.ButtonsPanelDimension + _layout.RightPanel.NotHiddenDimension + SplitterDimension +
            // Fit center panel
            DockPanelsLayout.MinPanelDimension;

         int minimumHeight  =
            // Fit top panel
            DockPanelsLayout.ButtonsPanelDimension + _layout.TopPanel.NotHiddenDimension    + SplitterDimension +
            // Fit bottom panel
            DockPanelsLayout.ButtonsPanelDimension + _layout.BottomPanel.NotHiddenDimension + SplitterDimension +
            // Fit center panel
            DockPanelsLayout.ButtonsPanelDimension + DockPanelsLayout.MinPanelDimension;

         minimumWidth  = Math.Min (_container.Width,  minimumWidth);
         minimumHeight = Math.Min (_container.Height, minimumHeight);

         if (minimumWidth != _minViewWidth || minimumHeight != _minViewHeight)
         {
            _minViewWidth   = minimumWidth;
            _minViewHeight  = minimumHeight;
            _container.MinimumSize = new Size(_minViewWidth, _minViewHeight);

            if (MinimumSizeChanged != null)
            {
               MinimumSizeChanged (this, EventArgs.Empty);
            }
         }

         _container.Invalidate();

         return modActualizat;
      }

      /// <summary>
      /// Converts x axis coordinate from screen to container client area.
      /// </summary>
      /// <param name="screenX">screen x coordinate</param>
      /// <returns>container client x coordinate</returns>
      private int ClientX (int screenX)
      {
         Point point = new Point (screenX, 0);
         point = _container.PointToClient (point);
         return point.X;
      }

      /// <summary>
      /// Converts y axis coordinate from screen to container client area.
      /// </summary>
      /// <param name="screenY">screen y coordinate</param>
      /// <returns>container client y coordinate</returns>
      private int ClientY (int screenY)
      {
         Point point = new Point (0, screenY);
         point = _container.PointToClient (point);
         return point.Y;
      }

      /// <summary>
      /// Converts x axis coordinate from container client area to screen.
      /// </summary>
      /// <param name="clientX">container client x coordinate</param>
      /// <returns>screen x coordinate</returns>
      private int ScreenX (int clientX)
      {
         Point point = new Point (clientX, 0);
         point = _container.PointToScreen (point);
         return point.X;
      }

      /// <summary>
      /// Converts y axis coordinate from container client area to screen.
      /// </summary>
      /// <param name="clientY">container client y coordinate</param>
      /// <returns>screen y coordinate</returns>
      private int ScreenY (int clientY)
      {
         Point point = new Point (0, clientY);
         point = _container.PointToScreen (point);
         return point.Y;
      }

      #endregion Private section.
   }
}
