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

namespace CodeCell.Bricks.UIs.Dockables
{
   /// <summary>
   /// Dock preview engine
   /// </summary>
   internal class DockPreviewEngine
   {
      #region Fields.

      private CenterButtonToDockFill   _dockFillGuider         = new CenterButtonToDockFill();
      private CenterButtonToDockRight  _dockRightGuider2       = new CenterButtonToDockRight();
      private CenterButtonToDockBottom _dockBottomGuider2      = new CenterButtonToDockBottom();
      private CenterButtonToDockLeft   _dockLeftGuider2        = new CenterButtonToDockLeft();
      private CenterButtonToDockUp     _dockTopGuider2         = new CenterButtonToDockUp();
      private ButtonToDockRight        _dockRightGuider1       = new ButtonToDockRight();
      private ButtonToDockBottom       _dockBottomGuider1      = new ButtonToDockBottom();
      private ButtonToDockLeft         _dockLeftGuider1        = new ButtonToDockLeft();
      private ButtonToDockUp           _dockTopGuider1         = new ButtonToDockUp();

      private DockPreview              _dockPreview            = new DockPreview();
      private zDockMode                _dockMode               = zDockMode.None;

      private Rectangle                _leftPreviewBounds      = new Rectangle();
      private Rectangle                _rightPreviewBounds     = new Rectangle();
      private Rectangle                _topPreviewBounds       = new Rectangle();
      private Rectangle                _bottomPreviewBounds    = new Rectangle();
      private Rectangle                _fillPreviewBounds      = new Rectangle();

      private zDockMode                _lastAllowedDock        = zDockMode.None;
      private Rectangle                _lastScreenClientBounds = new Rectangle();

      #endregion Fields.

      #region Instance.

      /// <summary>
      /// Default constructor
      /// </summary>
      public DockPreviewEngine ()
      {
      }

      #endregion Instance.

      #region Public section.

      /// <summary>
      /// Notify when the preview is shown or hidden
      /// </summary>
      public event EventHandler VisibleChanged;

      /// <summary>
      /// Bounds of the preview form in screen coordinates, when docking is on left.
      /// </summary>
      public Rectangle LeftPreviewBounds
      {
         get { return _leftPreviewBounds; }
         set { _leftPreviewBounds = value; }
      }

      /// <summary>
      /// Bounds of the preview form in screen coordinates, when docking is on right.
      /// </summary>
      public Rectangle RightPreviewBounds
      {
         get { return _rightPreviewBounds; }
         set { _rightPreviewBounds = value; }
      }

      /// <summary>
      /// Bounds of the preview form in screen coordinates, when docking is on top.
      /// </summary>
      public Rectangle TopPreviewBounds
      {
         get { return _topPreviewBounds; }
         set { _topPreviewBounds = value; }
      }

      /// <summary>
      /// Bounds of the preview form in screen coordinates, when docking is on bottom.
      /// </summary>
      public Rectangle BottomPreviewBounds
      {
         get { return _bottomPreviewBounds; }
         set { _bottomPreviewBounds = value; }
      }

      /// <summary>
      /// Bounds of the preview form in screen coordinates, when docking is on fill.
      /// </summary>
      public Rectangle FillPreviewBounds
      {
         get { return _fillPreviewBounds; }
         set { _fillPreviewBounds = value; }
      }

      /// <summary>
      /// Flag indicating if the dock guiders are visible or not
      /// </summary>
      public bool Visibile
      {
         get { return _dockFillGuider.Visible; }
      }

      /// <summary>
      /// Gets the selected dock mode
      /// </summary>
      public zDockMode DockMode
      {
         get { return _dockMode; }
      }

      /// <summary>
      /// Updates the dock preview depending on current mouse position
      /// </summary>
      /// <param name="mousePosition">mouse position in screen coordinates</param>
      public void UpdateDockPreviewOnMouseMove (Point mousePosition)
      {
         _dockMode = zDockMode.None;

         if (_dockLeftGuider1.Bounds.Contains (mousePosition) || _dockLeftGuider2.Bounds.Contains(mousePosition))
         {
            _dockPreview.Size      = LeftPreviewBounds.Size;
            _dockPreview.Location  = LeftPreviewBounds.Location;
            _dockMode              = zDockMode.Left;
         }

         if (_dockRightGuider1.Bounds.Contains (mousePosition) || _dockRightGuider2.Bounds.Contains (mousePosition))
         {
            _dockPreview.Size      = RightPreviewBounds.Size;
            _dockPreview.Location  = RightPreviewBounds.Location;
            _dockMode              = zDockMode.Right;
         }

         if (_dockTopGuider1.Bounds.Contains (mousePosition) || _dockTopGuider2.Bounds.Contains (mousePosition))
         {
            _dockPreview.Size      = TopPreviewBounds.Size;
            _dockPreview.Location  = TopPreviewBounds.Location;
            _dockMode              = zDockMode.Top;
         }

         if (_dockBottomGuider1.Bounds.Contains (mousePosition) || _dockBottomGuider2.Bounds.Contains (mousePosition))
         {
            _dockPreview.Size      = BottomPreviewBounds.Size;
            _dockPreview.Location  = BottomPreviewBounds.Location;
            _dockMode              = zDockMode.Bottom;
         }

         if (_dockFillGuider.Contains (mousePosition) )
         {
            _dockPreview.Size      = FillPreviewBounds.Size;
            _dockPreview.Location  = FillPreviewBounds.Location;
            _dockMode              = zDockMode.Fill;
         }


         _dockPreview.TopMost = false;
         _dockPreview.Visible = _dockMode != zDockMode.None;
      }

      /// <summary>
      /// Show the dock guiders buttons
      /// </summary>
      /// <param name="allowedDockMode">allowed dock mode. Guiders will be shown depending on this mode.</param>
      /// <param name="screenClientBounds">screen client bounds of the container</param>
      public void ShowDockGuiders (zDockMode allowedDockMode, Rectangle screenClientBounds)
      {
         if (allowedDockMode == zDockMode.None)
         {
            HideDockGuiders();
            return;
         }

         if (_lastAllowedDock == allowedDockMode && _lastScreenClientBounds == screenClientBounds)
         {
            return;
         }

         _lastAllowedDock        = allowedDockMode;
         _lastScreenClientBounds = screenClientBounds;

         int width    = screenClientBounds.Width;
         int height   = screenClientBounds.Height;
         Point offset = screenClientBounds.Location;
         Point center = new Point(width / 2, height / 2);

         Point leftPosition1     = new Point(12,                                      center.Y - _dockLeftGuider1.Height / 2);
         Point topPosition1      = new Point(center.X - _dockTopGuider1.Width / 2,    12);
         Point rightPosition1    = new Point(width - _dockRightGuider1.Width - 12,    center.Y - _dockRightGuider1.Height / 2);
         Point bottomPosition1   = new Point(center.X - _dockBottomGuider1.Width / 2, height - _dockBottomGuider1.Height - 12);

         Point fillPosition      = new Point(center.X - _dockFillGuider.Width / 2,    center.Y - _dockFillGuider.Height / 2);

         Point leftPosition2     = new Point(fillPosition.X - _dockLeftGuider2.Width + 7, center.Y - _dockLeftGuider2.Height / 2);
         Point topPosition2      = new Point(center.X - _dockTopGuider2.Width / 2 - 1,    fillPosition.Y - _dockTopGuider2.Height + 7);
         Point rightPosition2    = new Point(fillPosition.X + _dockFillGuider.Width - 7,  center.Y - _dockRightGuider2.Height / 2);
         Point bottomPosition2   = new Point(center.X - _dockBottomGuider2.Width / 2,     fillPosition.Y + _dockBottomGuider2.Height + 7);

         leftPosition1.Offset   (offset);
         rightPosition1.Offset  (offset);
         topPosition1.Offset    (offset);
         bottomPosition1.Offset (offset);
         fillPosition.Offset    (offset);
         leftPosition2.Offset   (offset);
         rightPosition2.Offset  (offset);
         topPosition2.Offset    (offset);
         bottomPosition2.Offset (offset);

         _dockLeftGuider1.Location    = leftPosition1;
         _dockRightGuider1.Location   = rightPosition1;
         _dockTopGuider1.Location     = topPosition1;
         _dockBottomGuider1.Location  = bottomPosition1;
         _dockFillGuider.Location     = fillPosition;
         _dockLeftGuider2.Location    = leftPosition2;
         _dockRightGuider2.Location   = rightPosition2;
         _dockTopGuider2.Location     = topPosition2;
         _dockBottomGuider2.Location  = bottomPosition2;

         _dockFillGuider.Show ();

         bool showLeft = EnumUtility.Contains (allowedDockMode, zDockMode.Left);
         _dockLeftGuider1.Visible = showLeft;
         _dockLeftGuider2.Visible = showLeft;

         bool showRight = EnumUtility.Contains (allowedDockMode, zDockMode.Right);
         _dockRightGuider1.Visible = showRight;
         _dockRightGuider2.Visible = showRight;

         bool showTop = EnumUtility.Contains (allowedDockMode, zDockMode.Top);
         _dockTopGuider1.Visible = showTop;
         _dockTopGuider2.Visible = showTop;

         bool showBottom = EnumUtility.Contains (allowedDockMode, zDockMode.Bottom);
         _dockBottomGuider1.Visible = showBottom;
         _dockBottomGuider2.Visible = showBottom;

         _dockFillGuider.ShowFillPreview = EnumUtility.Contains (allowedDockMode, zDockMode.Fill);

         if (VisibleChanged != null)
         {
            VisibleChanged (this, EventArgs.Empty);
         }
      }

      /// <summary>
      /// Hide the dock guiders
      /// </summary>
      public void HideDockGuiders ()
      {
         if (Visibile == false)
         {
            return;
         }

         _lastAllowedDock        = zDockMode.None;
         _lastScreenClientBounds = new Rectangle();

         _dockLeftGuider1.Hide ();
         _dockRightGuider1.Hide ();
         _dockTopGuider1.Hide ();
         _dockBottomGuider1.Hide ();
         _dockFillGuider.Hide ();
         _dockLeftGuider2.Hide ();
         _dockRightGuider2.Hide ();
         _dockTopGuider2.Hide ();
         _dockBottomGuider2.Hide ();

         if (_dockPreview.Visible)
         {
            _dockPreview.Hide ();
         }

         if (VisibleChanged != null)
         {
            VisibleChanged(this, EventArgs.Empty);
         }

         _dockMode = zDockMode.None;
      }

      #endregion Public section.
   }
}
