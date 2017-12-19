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
using System.Drawing;
using System.Windows.Forms;

namespace CodeCell.Bricks.UIs.Dockables
{
   /// <summary>
   /// Component for computing the layout of the dock panels
   /// </summary>
   internal class DockPanelsLayout : IDisposable
   {
      #region Fields.

      /// <summary>
      /// Dimension of the buttons panel
      /// </summary>
      public const int                 ButtonsPanelDimension            = 24;
      /// <summary>
      /// Minimum allowed dimension of the panels
      /// </summary>
      public const int                 MinPanelDimension                = 24;//by fdc,old = 150
      /// <summary>
      /// Minimum view width
      /// </summary>
      public const int                 MinViewWidth                     = 3 * MinPanelDimension + 2 * ButtonsPanelDimension + 2 * 4;
      /// <summary>
      /// Minimum view height
      /// </summary>
      public const int                 MinViewHeight                    = 3 * MinPanelDimension + 3 * ButtonsPanelDimension + 2 * 4;

      private bool                     _disposed                        = false;

      private SideDockPanel            _leftPanel                       = null;
      private SideDockPanel            _rightPanel                      = null;
      private SideDockPanel            _topPanel                        = null;
      private SideDockPanel            _bottomPanel                     = null;
      private DockPanel                _centerPanel                     = null;

      private Rectangle                _leftBottomButtonsBounds         = new Rectangle();
      private Rectangle                _rightBottomButtonsBounds        = new Rectangle();

      private DockPanel[] _dockPanels = null;

      #endregion Fields.

      #region Instance.

      /// <summary>
      /// Constructor.
      /// </summary>
      public DockPanelsLayout ()
      {
         _leftPanel   = new SideDockPanel (zDockMode.Left,   FormBorderStyle.FixedToolWindow);
         _rightPanel  = new SideDockPanel (zDockMode.Right,  FormBorderStyle.FixedToolWindow);
         _topPanel    = new SideDockPanel (zDockMode.Top,    FormBorderStyle.FixedToolWindow);
         _bottomPanel = new SideDockPanel (zDockMode.Bottom, FormBorderStyle.FixedToolWindow);
         _centerPanel = new DockPanel     (zDockMode.Fill,   FormBorderStyle.None);

         _leftPanel.UpdateLayoutRequested    += OnUpdateLayoutRequested;
         _rightPanel.UpdateLayoutRequested   += OnUpdateLayoutRequested;
         _topPanel.UpdateLayoutRequested     += OnUpdateLayoutRequested;
         _bottomPanel.UpdateLayoutRequested  += OnUpdateLayoutRequested;
         _centerPanel.UpdateLayoutRequested  += OnUpdateLayoutRequested;

         _dockPanels = new DockPanel[] 
         {
             _leftPanel,
             _topPanel,
             _rightPanel,
             _bottomPanel,
             _centerPanel
         };
      }

      #endregion Instance.

      #region IDisposable.

      /// <summary>
      /// Dispose
      /// </summary>
      public void Dispose ()
      {
         if (_disposed)
         {
            return;
         }

         _leftPanel.UpdateLayoutRequested    -= OnUpdateLayoutRequested;
         _rightPanel.UpdateLayoutRequested   -= OnUpdateLayoutRequested;
         _topPanel.UpdateLayoutRequested     -= OnUpdateLayoutRequested;
         _bottomPanel.UpdateLayoutRequested  -= OnUpdateLayoutRequested;
         _centerPanel.UpdateLayoutRequested  -= OnUpdateLayoutRequested;

         _leftPanel   = null;
         _rightPanel  = null;
         _topPanel    = null;
         _bottomPanel = null;
         _centerPanel = null;

         _disposed    = true;

         GC.SuppressFinalize(this);
      }

      #endregion  IDisposable.

      #region Public section.

      //by fdc
      public DockPanel[] DockPanels
      {
          get
          {
              return _dockPanels;
          }
      }

      /// <summary>
      /// Request to update the layout
      /// </summary>
      public event EventHandler UpdateLayoutRequested;

      /// <summary>
      /// Left panel
      /// </summary>
      public SideDockPanel LeftPanel
      {
         get 
         { 
            CheckNotDisposed();
            return _leftPanel; 
         }
      }

      /// <summary>
      /// Right panel
      /// </summary>
      public SideDockPanel RightPanel
      {
         get
         {
            CheckNotDisposed ();
            return _rightPanel;
         }
      }

      /// <summary>
      /// Top panel
      /// </summary>
      public SideDockPanel TopPanel
      {
         get
         {
            CheckNotDisposed ();
            return _topPanel;
         }
      }

      /// <summary>
      /// Bottom panel
      /// </summary>
      public SideDockPanel BottomPanel
      {
         get
         {
            CheckNotDisposed ();
            return _bottomPanel;
         }
      }

      /// <summary>
      /// Center panel
      /// </summary>
      public DockPanel CenterPanel
      {
         get
         {
            CheckNotDisposed ();
            return _centerPanel;
         }
      }

      /// <summary>
      /// Left-Bottom buttons bounds
      /// </summary>
      public Rectangle LeftBottomButtonsBounds
      {
         get
         {
            CheckNotDisposed ();
            return _leftBottomButtonsBounds;
         }
         set
         {
            CheckNotDisposed ();
            _leftBottomButtonsBounds = value;
         }
      }

      /// <summary>
      /// Right-Bottom buttons bounds
      /// </summary>
      public Rectangle RightBottomButtonsBounds
      {
         get
         {
            CheckNotDisposed ();
            return _rightBottomButtonsBounds;
         }
         set
         {
            CheckNotDisposed ();
            _rightBottomButtonsBounds = value;
         }
      }

      /// <summary>
      /// Update the layout of the left panel
      /// </summary>
      /// <param name="containerSize">container size</param>
      /// <returns>true if the layout was updated</returns>
      public bool UpdateLeftPanelLayout (Size containerSize)
      {
         CheckNotDisposed ();

         Rectangle buttonsBounds       = new Rectangle ();
         Rectangle contentBounds       = new Rectangle ();
         Rectangle bottomButtonsBounds = new Rectangle ();
         Rectangle splitterBounds      = new Rectangle ();
         Rectangle previewBounds       = new Rectangle ();

         if (LeftPanel.AutoHide)
         {
            buttonsBounds = new Rectangle (0, 0, ButtonsPanelDimension, containerSize.Height);
         }
         else
         {
            bottomButtonsBounds = new Rectangle (0, containerSize.Height - ButtonsPanelDimension,
               LeftPanel.NotHiddenDimension, ButtonsPanelDimension);
         }

         contentBounds = new Rectangle (buttonsBounds.Right, 0,
            LeftPanel.NotHiddenDimension, containerSize.Height - bottomButtonsBounds.Height);

         splitterBounds = new Rectangle (contentBounds.Right, 0,
            LeftPanel.SplitterDimension, containerSize.Height);

         previewBounds = contentBounds;

         if (LeftPanel.HasVisibleToolWindows == false)
         {
            buttonsBounds.Width  = 0;
            contentBounds.Width  = 0;
            splitterBounds.Width = 0;
            bottomButtonsBounds.Width = 0;

            splitterBounds.X = 0;

            previewBounds.Location = new Point ();
            previewBounds.Size     = new Size(LeftPanel.NotHiddenDimension, containerSize.Height);
         }
         else if (LeftPanel.AutoHidden)
         {
            contentBounds.Width  = 0;
            splitterBounds.Width = 0;
            splitterBounds.X     = buttonsBounds.Right;
         }

         LeftPanel.PreviewBounds = previewBounds;

         LeftPanel.MinSlidePos = buttonsBounds.Width + MinPanelDimension;
         LeftPanel.MaxSlidePos = containerSize.Width - RightPanel.NotHiddenDimension - ButtonsPanelDimension - MinPanelDimension - RightPanel.SplitterDimension;
         LeftPanel.MaxSlidePos = Math.Max (LeftPanel.MinSlidePos, LeftPanel.MaxSlidePos);

         bool changed = UpdatePanelLayout (buttonsBounds, contentBounds, splitterBounds, LeftPanel);

         changed |= _leftBottomButtonsBounds != bottomButtonsBounds;

         if (changed)
         {
            _leftBottomButtonsBounds = bottomButtonsBounds;
         }

         return changed;
      }

      /// <summary>
      /// Update the layout of the right panel
      /// </summary>
      /// <param name="containerSize">container size</param>
      /// <returns>true if the layout was updated</returns>
      public bool UpdateRightPanelLayout (Size containerSize)
      {
         CheckNotDisposed ();

         Rectangle buttonsBounds       = new Rectangle ();
         Rectangle contentBounds       = new Rectangle ();
         Rectangle bottomButtonsBounds = new Rectangle ();
         Rectangle splitterBounds      = new Rectangle ();
         Rectangle previewBounds       = new Rectangle ();

         buttonsBounds = new Rectangle (containerSize.Width - ButtonsPanelDimension, 0,
            ButtonsPanelDimension, containerSize.Height);

         bottomButtonsBounds = new Rectangle (
            containerSize.Width - RightPanel.NotHiddenDimension, containerSize.Height - ButtonsPanelDimension,
            RightPanel.NotHiddenDimension, ButtonsPanelDimension);

         if (RightPanel.AutoHide)
         {
            bottomButtonsBounds.Height = 0;
         }
         else
         {
            buttonsBounds.Width = 0;
            buttonsBounds.X     = containerSize.Width;
         }

         contentBounds  = new Rectangle (
            buttonsBounds.Left - RightPanel.NotHiddenDimension, 0,
            RightPanel.NotHiddenDimension, containerSize.Height - bottomButtonsBounds.Height);

         splitterBounds = new Rectangle (contentBounds.Left - RightPanel.SplitterDimension, 0,
            RightPanel.SplitterDimension, containerSize.Height);

         previewBounds  = contentBounds;

         if (RightPanel.HasVisibleToolWindows == false)
         {
            buttonsBounds.Width  = 0;
            contentBounds.Width  = 0;
            splitterBounds.Width = 0;
            bottomButtonsBounds.Width = 0;

            splitterBounds.X = containerSize.Width;

            previewBounds.Location = new Point(containerSize.Width - RightPanel.NotHiddenDimension, 0);
            previewBounds.Size     = new Size(RightPanel.NotHiddenDimension, containerSize.Height);
         }
         else if (RightPanel.AutoHidden)
         {
            contentBounds.Width  = 0;
            splitterBounds.Width = 0;
            splitterBounds.X     = buttonsBounds.Left;
         }

         RightPanel.PreviewBounds = previewBounds;

         RightPanel.MinSlidePos = LeftPanel.NotHiddenDimension + ButtonsPanelDimension + MinPanelDimension + LeftPanel.SplitterDimension;
         RightPanel.MaxSlidePos = containerSize.Width - MinPanelDimension - buttonsBounds.Width;
         RightPanel.MinSlidePos = Math.Min (RightPanel.MinSlidePos, RightPanel.MaxSlidePos);

         bool changed = UpdatePanelLayout (buttonsBounds, contentBounds, splitterBounds, RightPanel);

         changed |= _rightBottomButtonsBounds != bottomButtonsBounds;

         if (changed)
         {
            _rightBottomButtonsBounds = bottomButtonsBounds;
         }

         return changed;
      }

      /// <summary>
      /// Update the layout of the bottom panel
      /// </summary>
      /// <param name="containerSize">container size</param>
      /// <returns>true if the layout was updated</returns>
      public bool UpdateBottomPanelLayout (Size containerSize)
      {
         CheckNotDisposed ();

         Rectangle buttonsBounds       = new Rectangle ();
         Rectangle contentBounds       = new Rectangle ();
         Rectangle splitterBounds      = new Rectangle ();
         Rectangle previewBounds       = new Rectangle ();

         buttonsBounds = new Rectangle (LeftPanel.SplitterBounds.Right, containerSize.Height - ButtonsPanelDimension,
            RightPanel.SplitterBounds.Left - LeftPanel.SplitterBounds.Right, ButtonsPanelDimension);

         contentBounds  = new Rectangle (
            buttonsBounds.Left, buttonsBounds.Top - BottomPanel.NotHiddenDimension,
            buttonsBounds.Width, BottomPanel.NotHiddenDimension);

         splitterBounds = new Rectangle (contentBounds.Left, contentBounds.Top - BottomPanel.SplitterDimension,
            contentBounds.Width, BottomPanel.SplitterDimension);

         previewBounds  = contentBounds;

         if (BottomPanel.HasVisibleToolWindows == false)
         {
            buttonsBounds.Height  = 0;
            contentBounds.Height  = 0;
            splitterBounds.Height = 0;
            splitterBounds.Y      = containerSize.Height;

            previewBounds.Location = new Point(previewBounds.Left, containerSize.Height - BottomPanel.NotHiddenDimension);
         }
         else if (BottomPanel.AutoHidden)
         {
            contentBounds.Height  = 0;
            splitterBounds.Height = 0;
            splitterBounds.Y      = buttonsBounds.Top;
         }

         BottomPanel.PreviewBounds = previewBounds;

         BottomPanel.MinSlidePos = TopPanel.NotHiddenDimension + ButtonsPanelDimension + MinPanelDimension + TopPanel.SplitterDimension;
         BottomPanel.MaxSlidePos = containerSize.Height - buttonsBounds.Height - MinPanelDimension;
         BottomPanel.MinSlidePos = Math.Min (BottomPanel.MinSlidePos, BottomPanel.MaxSlidePos);

         return UpdatePanelLayout (buttonsBounds, contentBounds, splitterBounds, BottomPanel);
      }

      /// <summary>
      /// Update the layout of the top panel
      /// </summary>
      /// <param name="containerSize">container size</param>
      /// <returns>true if the layout was updated</returns>
      public bool UpdateTopPanelLayout (Size containerSize)
      {
         CheckNotDisposed ();

         Rectangle buttonsBounds       = new Rectangle ();
         Rectangle contentBounds       = new Rectangle ();
         Rectangle splitterBounds      = new Rectangle ();
         Rectangle previewBounds       = new Rectangle ();

         buttonsBounds = new Rectangle (LeftPanel.SplitterBounds.Right, 0,
            RightPanel.SplitterBounds.Left - LeftPanel.SplitterBounds.Right, ButtonsPanelDimension);

         contentBounds  = new Rectangle (
            buttonsBounds.Left, buttonsBounds.Bottom,
            buttonsBounds.Width, TopPanel.NotHiddenDimension);

         splitterBounds = new Rectangle (contentBounds.Left, contentBounds.Bottom,
            contentBounds.Width, TopPanel.SplitterDimension);

         previewBounds  = contentBounds;

         if (TopPanel.HasVisibleToolWindows == false)
         {
            buttonsBounds.Height  = 0;
            contentBounds.Height  = 0;
            splitterBounds.Height = 0;
            splitterBounds.Y      = 0;

            previewBounds.Location = new Point(previewBounds.Left, 0);
         }
         else if (TopPanel.AutoHidden)
         {
            contentBounds.Height  = 0;
            splitterBounds.Y      = buttonsBounds.Bottom;
            splitterBounds.Height = 0;
         }

         TopPanel.PreviewBounds = previewBounds;

         TopPanel.MinSlidePos = buttonsBounds.Height + MinPanelDimension;
         TopPanel.MaxSlidePos = containerSize.Height - BottomPanel.NotHiddenDimension - ButtonsPanelDimension - MinPanelDimension - BottomPanel.SplitterDimension;
         TopPanel.MaxSlidePos = Math.Max(TopPanel.MinSlidePos, TopPanel.MaxSlidePos);

         return UpdatePanelLayout (buttonsBounds, contentBounds, splitterBounds, TopPanel);
      }

      /// <summary>
      /// Update the layout of the center panel
      /// </summary>
      /// <param name="containerSize">container size</param>
      /// <returns>true if the layout was updated</returns>
      public bool UpdateCenterPanelLayout (Size containerSize)
      {
         CheckNotDisposed ();

         Rectangle buttonsBounds       = new Rectangle ();
         Rectangle contentBounds       = new Rectangle ();
         Rectangle previewBounds       = new Rectangle ();

         buttonsBounds  = new Rectangle (LeftPanel.SplitterBounds.Right, TopPanel.SplitterBounds.Bottom,
            RightPanel.SplitterBounds.Left - LeftPanel.SplitterBounds.Right, ButtonsPanelDimension);

         contentBounds  = new Rectangle (
            buttonsBounds.Left,  buttonsBounds.Bottom,
            buttonsBounds.Width, BottomPanel.SplitterBounds.Top - buttonsBounds.Bottom);

         previewBounds  = contentBounds;

         if (CenterPanel.HasVisibleToolWindows == false)
         {
            buttonsBounds.Width  = 0;
            contentBounds.Width  = 0;

            previewBounds.Location = buttonsBounds.Location;
            previewBounds.Size     = new Size(previewBounds.Width, BottomPanel.SplitterBounds.Top - previewBounds.Top);
         }

         CenterPanel.PreviewBounds = previewBounds;

         return UpdatePanelLayout (buttonsBounds, contentBounds, CenterPanel);
      }

      #endregion Public section.

      #region Private section.

      /// <summary>
      /// On request to update the layout
      /// </summary>
      /// <param name="sender">sender</param>
      /// <param name="e">e</param>
      private void OnUpdateLayoutRequested (object sender, EventArgs e)
      {
         EventHandler updateLayoutRequested = UpdateLayoutRequested;
         if (updateLayoutRequested != null)
         {
            updateLayoutRequested(sender, e);
         }
      }

      /// <summary>
      /// Update the panel layout
      /// </summary>
      /// <param name="buttonsBounds">bounds of the buttons panel</param>
      /// <param name="contentBounds">bounds of the content panel</param>
      /// <param name="panel">panel to be updated</param>
      /// <returns>true if there were layout changes updated by this call</returns>
      private bool UpdatePanelLayout (Rectangle buttonsBounds, Rectangle contentBounds, DockPanel panel)
      {
         bool changed = false;

         changed |= panel.ButtonsBounds  != buttonsBounds;
         changed |= panel.ContentBounds  != contentBounds;

         if (changed)
         {
            panel.UpdateBounds (contentBounds, buttonsBounds);
         }

         return changed;
      }

      /// <summary>
      /// Update the panel layout
      /// </summary>
      /// <param name="buttonsBounds">bounds of the buttons panel</param>
      /// <param name="contentBounds">bounds of the content panel</param>
      /// <param name="splitterBounds">bounds of the splitter attached to the panel</param>
      /// <param name="panel">panel to be updated</param>
      /// <returns>true if there were layout changes updated by this call</returns>
      private bool UpdatePanelLayout (Rectangle buttonsBounds, Rectangle contentBounds, Rectangle splitterBounds, SideDockPanel panel)
      {
         bool changed = UpdatePanelLayout (buttonsBounds, contentBounds, panel);

         changed |= panel.SplitterBounds != splitterBounds;

         if (changed)
         {
            panel.SplitterBounds = splitterBounds;
         }

         return changed;
      }

      /// <summary>
      /// Check that object is not disposed
      /// </summary>
      private void CheckNotDisposed ()
      {
         if (_disposed)
         {
            throw new ObjectDisposedException(GetType().Name);
         }
      }

      #endregion Private section.
   }
}
