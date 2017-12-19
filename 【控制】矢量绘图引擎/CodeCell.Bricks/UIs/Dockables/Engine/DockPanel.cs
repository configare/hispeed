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

namespace CodeCell.Bricks.UIs.Dockables
{
   /// <summary>
   /// Dock panel
   /// </summary>
   internal class DockPanel
   {
      #region Fields.

      protected zDockMode                 _dockMode                  = zDockMode.None;
      protected FormBorderStyle           _toolWindowsBorderStyle    = FormBorderStyle.None;

      protected List<DockableToolWindow>  _toolWindows               = new List<DockableToolWindow>();
      protected List<DockableToolWindow>  _visibleToolWindows        = new List<DockableToolWindow> ();
      protected Rectangle                 _contentBounds             = new Rectangle ();
      protected Rectangle                 _buttonsBounds             = new Rectangle ();
      protected Rectangle                 _previewBounds             = new Rectangle ();

      #endregion Fields.

      #region Instance.

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="dockMode">dock mode of this panel</param>
      /// <param name="toolWindowsBorder">border of the tool windows from this panel</param>
      public DockPanel (zDockMode dockMode, FormBorderStyle toolWindowsBorder)
      {
         _dockMode               = dockMode;
         _toolWindowsBorderStyle = toolWindowsBorder;
      }

      #endregion Instance.

      #region Public section.

      /// <summary>
      /// Request to update the layout
      /// </summary>
      public event EventHandler UpdateLayoutRequested;

      /// <summary>
      /// Flag indicating if there are visible tool windows in this panel
      /// </summary>
      public bool HasVisibleToolWindows
      {
         get { return _visibleToolWindows.Count != 0; }
      }

      /// <summary>
      /// Content panel bounds
      /// </summary>
      public Rectangle ContentBounds
      {
         get { return _contentBounds; }
      }

      /// <summary>
      /// Panel buttons bounds
      /// </summary>
      public Rectangle ButtonsBounds
      {
         get { return _buttonsBounds; }
      }

      /// <summary>
      /// Panel preview bounds
      /// </summary>
      public Rectangle PreviewBounds
      {
         get { return _previewBounds; }
         set { _previewBounds = value; }
      }

      /// <summary>
      /// Tool windows
      /// </summary>
      public DockableToolWindow[] ToolWindows
      {
         get { return _toolWindows.ToArray (); }
      }

      /// <summary>
      /// Visible tool windows
      /// </summary>
      public DockableToolWindow[] VisibleToolWindows
      {
         get { return _visibleToolWindows.ToArray (); }
      }

      /// <summary>
      /// Check if the tool window is contained in the panel
      /// </summary>
      /// <param name="toolWindow">tool window to be checked</param>
      /// <returns>true if the tool window is contained in the panel</returns>
      public bool Contains (DockableToolWindow toolWindow)
      {
         return _toolWindows.Contains (toolWindow);
      }

      /// <summary>
      /// Dock the given tool window
      /// </summary>
      /// <param name="toolWindow">tool window to be docked</param>
      public void DockToolWindow (DockableToolWindow toolWindow)
      {
         if (toolWindow.IsDocked)
         {
            return;
         }

         _toolWindows.Add (toolWindow);

         if (toolWindow.Visible && _visibleToolWindows.Contains (toolWindow) == false)
         {
            _visibleToolWindows.Add (toolWindow);
         }

         toolWindow.LockFormSizeAndDock (ContentBounds, _toolWindowsBorderStyle, _dockMode);

         ConnectToolWindow (toolWindow);

         RaiseUpdateLayout();
      }

      /// <summary>
      /// Undock the specified tool window removing it from the windows collection, disconnecting the events 
      /// and restoring the size before dock. The dock panels layout is updated after undock
      /// </summary>
      /// <param name="toolWindow">tool window</param>
      /// <returns>true is the tool-window was in the tool windows collection</returns>
      public void UndockToolWindow (DockableToolWindow toolWindow)
      {
         int index = _toolWindows.IndexOf (toolWindow);
         if (index < 0)
         {
            return;
         }

         Size restoreSize = toolWindow.GetRestoreSize (Control.MousePosition);

         _toolWindows.Remove (toolWindow);
         _visibleToolWindows.Remove (toolWindow);

         DisconnectToolWindow (toolWindow);

         toolWindow.LockFormSizeAndUndock (restoreSize);

         RaiseUpdateLayout();
      }

      /// <summary>
      /// Update content and buttons bounds
      /// </summary>
      /// <param name="contentBounds">new content bounds</param>
      /// <param name="buttonsBounds">new buttons bounds</param>
      /// <returns>true if the content panel bounds where updated</returns>
      public bool UpdateBounds (Rectangle contentBounds, Rectangle buttonsBounds)
      {
         if (_contentBounds == contentBounds)
         {
            return false;
         }

         _contentBounds = contentBounds;
         _buttonsBounds = buttonsBounds;

         UpdateToolWindowsBounds ();

         return true;
      }

      #endregion Public section.

      #region Protected section.

      /// <summary>
      /// Set auto-hide mode on tool-windows
      /// </summary>
      protected virtual void ToggleAutoHideMode ()
      {
      }

      /// <summary>
      /// Raise UpdateLayout event
      /// </summary>
      protected void RaiseUpdateLayout ()
      {
         EventHandler updateLayout = UpdateLayoutRequested;
         if (updateLayout != null)
         {
            updateLayout (this, EventArgs.Empty);
         }
      }

      #endregion Protected section.

      #region Private section.
      #region Received events.

      /// <summary>
      /// Event handler on tool window auto hide changed
      /// </summary>
      /// <param name="sender">sender</param>
      /// <param name="e">e</param>
      private void OnToolWindowAutoHideChanged (object sender, EventArgs e)
      {
         DockableToolWindow toolWindow = sender as DockableToolWindow;
         if (toolWindow == null)
         {
            return;
         }

         if (Contains (toolWindow) == false)
         {
            return;
         }

         ToggleAutoHideMode();
      }

      /// <summary>
      /// Event handler on tool window context menu request
      /// </summary>
      /// <param name="sender">sender</param>
      /// <param name="e">e</param>
      private void OnToolWindowContextMenu (object sender, EventArgs e)
      {
         return;
      }

      /// <summary>
      /// Event handler for tool window visible changed 
      /// </summary>
      /// <param name="sender">sender</param>
      /// <param name="e">e</param>
      private void OnToolWindowVisibleChanged (object sender, EventArgs e)
      {
         DockableToolWindow toolWindow = sender as DockableToolWindow;
         if (toolWindow != null)
         {
            SyncVisibleForms (toolWindow);
         }

         RaiseUpdateLayout ();
      }

      #endregion Received events.

      /// <summary>
      /// Connect the tool window events.
      /// </summary>
      /// <param name="toolWindow">tool window</param>
      private void ConnectToolWindow (DockableToolWindow toolWindow)
      {
         toolWindow.AutoHideButtonClick        += OnToolWindowAutoHideChanged;
         toolWindow.ContextMenuForToolWindow   += OnToolWindowContextMenu;
         toolWindow.VisibleChanged             += OnToolWindowVisibleChanged;
      }

      /// <summary>
      /// Disconnect tool window events
      /// </summary>
      /// <param name="toolWindow">tool window</param>
      private void DisconnectToolWindow (DockableToolWindow toolWindow)
      {
         toolWindow.AutoHideButtonClick        -= OnToolWindowAutoHideChanged;
         toolWindow.ContextMenuForToolWindow   -= OnToolWindowContextMenu;
         toolWindow.VisibleChanged             -= OnToolWindowVisibleChanged;
      }

      /// <summary>
      /// Sync visible forms
      /// </summary>
      /// <param name="toolWindow">tool window</param>
      private void SyncVisibleForms (DockableToolWindow toolWindow)
      {
         if (_toolWindows.Contains (toolWindow))
         {
            if (toolWindow.Visible)
            {
               if (_visibleToolWindows.Contains (toolWindow) == false)
               {
                  _visibleToolWindows.Add (toolWindow);
               }
            }
            else
            {
               _visibleToolWindows.Remove (toolWindow);
            }
         }
      }

      /// <summary>
      /// Update the bounds of the tool windows from this panel
      /// </summary>
      private void UpdateToolWindowsBounds ()
      {
         DockableToolWindow[] toolWindows = _toolWindows.ToArray();
         foreach (DockableToolWindow toolWindow in toolWindows)
         {
            toolWindow.LockFormSizeAndDock (_contentBounds, _toolWindowsBorderStyle, _dockMode);
         }
      }

      #endregion Private section.
   }
}
