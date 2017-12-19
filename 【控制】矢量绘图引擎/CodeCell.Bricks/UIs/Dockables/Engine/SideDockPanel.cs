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
   /// Side dock panel
   /// </summary>
   internal class SideDockPanel : DockPanel
   {
      #region Fields.

      private int                      _minSlidePos                     = 0;
      private int                      _maxSlidePos                     = 0;

      private bool                     _autoHide                        = false;
      private bool                     _autoHidden                      = false;
      private Rectangle                _splitterBounds                  = new Rectangle();
      private int                      _notHiddenDimension              = 150;
      private int                      _splitterDimension               = 3;

      #endregion Fields.

      #region Instance.

      /// <summary>
      /// Constructor
      /// </summary>
      /// <param name="dockPanels">dock panels interface</param>
      /// <param name="dockMode">dock mode of this panel</param>
      /// <param name="toolWindowsBorder">border of the tool windows from this panel</param>
      public SideDockPanel (zDockMode dockMode, FormBorderStyle toolWindowsBorder) : base(dockMode, toolWindowsBorder)
      {
      }

      #endregion Instance.

      #region Public section.

      /// <summary>
      /// Flag indicating that panel is auto-hidden
      /// </summary>
      public bool AutoHidden
      {
         get { return _autoHidden; }
         set { _autoHidden = value; }
      }

      /// <summary>
      /// Flag indicating that panel is in auto-hide mode
      /// </summary>
      public bool AutoHide
      {
         get { return _autoHide; }
         set { SetAutoHideMode(value); }
      }

      /// <summary>
      /// Splitter dimension
      /// </summary>
      public int SplitterDimension
      {
         get { return _splitterDimension; }
      }

      /// <summary>
      /// Not hidden dimension
      /// </summary>
      public int NotHiddenDimension
      {
         get { return _notHiddenDimension; }
         set { _notHiddenDimension = value; }
      }

      /// <summary>
      /// Min slide pos for the panel.
      /// </summary>
      /// <remarks>
      /// For Left panel the right side is moved.
      /// For Top panel the bottom side is moved.
      /// For Right panel the left side is moved.
      /// For Bottom panel the top side is moved.
      /// </remarks>
      public int MinSlidePos
      {
         get { return _minSlidePos; }
         set { _minSlidePos = value; }
      }

      /// <summary>
      /// Max slide pos for the panel.
      /// </summary>
      /// <remarks>
      /// For Left panel the right side is moved.
      /// For Top panel the bottom side is moved.
      /// For Right panel the left side is moved.
      /// For Bottom panel the top side is moved.
      /// </remarks>
      public int MaxSlidePos
      {
         get { return _maxSlidePos; }
         set { _maxSlidePos = value; }
      }

      /// <summary>
      /// Splitter bounds
      /// </summary>
      public Rectangle SplitterBounds
      {
         get { return _splitterBounds; }
         set { _splitterBounds = value; }
      }

      #endregion Public section.

      #region Protected section.

      /// <summary>
      /// Toggle auto-hide mode
      /// </summary>
      protected override void ToggleAutoHideMode ()
      {
         SetAutoHideMode(AutoHide == false);
      }

      #endregion Protected section.

      #region Private section.

      /// <summary>
      /// Set auto-hide mode on tool-windows
      /// </summary>
      /// <param name="toolWindows">tool-Windows</param>
      /// <param name="autoHide">new autoHide value</param>
      protected void SetAutoHideMode (bool autoHide)
      {
         _autoHide = autoHide;

         foreach (DockableToolWindow toolWindow in _toolWindows)
         {
            toolWindow.AutoHide = autoHide;
         }

         RaiseUpdateLayout ();
      }

      #endregion Private section.
   }
}
