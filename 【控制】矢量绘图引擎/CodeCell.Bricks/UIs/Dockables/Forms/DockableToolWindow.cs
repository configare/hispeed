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
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CodeCell.Bricks.UIs.Dockables
{
   internal delegate void OnDockableToolWindowSelectedChanged(object sender,bool isSelected);
   /// <summary>
   /// Dockable tool window implements basic functionalities required to allow tool windows to be docked using
   /// <see cref="DockContainer">DockContainer</see>
   /// </summary>
   /// <remarks>Use this object as base class for your auto-dockable tool windows.</remarks>
   public class DockableToolWindow : Form, ITitleData
   {
      #region Fields.

      private const int       NCPAINT                    = 133;
      private const int       NCACTIVATE                 = 134;
      private const int       PAINT                      = 15;
      private const int       SETTEXT                    = 12;
      private const int       NCLBUTTONDOWN              = 0x00A1;
      private const int       NCLBUTTONUP                = 0x00A2;
      private const int       NCRBUTTONDOWN              = 0x00A4;
      private const int       NCRBUTTONUP                = 0x00A5;
      private const int       SYSCOMMAND                 = 0x0112;
      private const int       SIZE                       = 0x0005;
      private const int       SIZING                     = 0x0214;
      private const int       ENTERSIZEMOVE              = 0x0231;
      private const int       EXITSIZEMOVE               = 0x0232;
      private const int       NCMOUSEMOVE                = 0x00A0;
      private const int       MOVING                     = 0x0216;
      private const int       LBUTTONDOWN                = 0x0201;
      private const int       NCLBUTTONDBLCLK            = 0x00A3;
      private const int       SETCURSOR                  = 0x0020;

      private bool            _disposed                  = false;

      private bool            _captureSuspended          = false;
      private bool            _captureReplaceStarted     = false;
      private Point           _lastNCMouseDownPos        = new Point();
      private Point           _lastPosOnNcMouseDown      = new Point();

      private bool            _resizeIsLocked            = false;
      private zDockMode       _dockMode                  = zDockMode.None;
      private bool            _autoHide                  = false;
      private Timer           _timer                     = new Timer();

      private FormBorderStyle _borderBeforeLock          = FormBorderStyle.FixedToolWindow;
      private Rectangle       _boundsBeforeLock          = new Rectangle();
      private Rectangle       _lockedBounds              = new Rectangle();

      private bool            _mouseIsOverAutoButton     = false;
      private bool            _mouseIsOverMenuButton     = false;

      private bool            _hideInsteadClosing        = false;

      private TabButton       _tabButton                 = null;
      internal OnDockableToolWindowSelectedChanged _onSelectedChanged = null ;

      /// <summary>
      /// this field should be string but it seems that on title change the variable is changed using
      /// non .NET calls so use a list of characters and create the string all the time from this list
      /// </summary>
      private List<char>      _title                     = new List<char>();



      #endregion Fields.

      #region Instance.

      /// <summary>
      /// Default constructor which creates a new instance of <see cref="DockableToolWindow"/>
      /// </summary>
      public DockableToolWindow ()
      {
         FormBorderStyle   = FormBorderStyle.FixedToolWindow;
         ShowInTaskbar     = false;
         TopLevel          = false;

         _timer.Interval   = 10;
         _timer.Enabled    = true;
         _timer.Tick      += OnTimer;

         MaximizeBox = false;

         _boundsBeforeLock   = Bounds;
         SizeChanged += OnSizeChanged;
         FormClosing += OnFormClosing;
      }

      #endregion Instance.

      #region Public section.

      /// <summary>
      /// Returns the allowed dock values for the tool window which specialize this class
      /// </summary>
      /// <remarks>Override this method to return the allowed dock for the current tool window</remarks>
      public virtual zDockMode AllowedDock
      {
         get 
         {
            CheckNotDisposed();

            return zDockMode.All; 
         }
      }

      /// <summary>
      /// Hide the tool window instead of closing it when close event is received. Default false
      /// </summary>
      /// <remarks>
      /// This option is checked only when the close event was generated after a user action.
      /// If the application or parent form is closed, then this flag is ignored and close event is not canceled.
      /// </remarks>
      public bool HideInsteadClosing
      {
         get { return _hideInsteadClosing; }
         set { _hideInsteadClosing = value; }
      }

      /// <summary>
      /// Current dock mode of the tool window
      /// </summary>
      public zDockMode DockMode
      {
         get { return _dockMode; }
      }

      #endregion Public section.

      #region Protected section.

      /// <summary>
      /// Event raised when <see cref="ContextMenuStrip">ContextMenuStrip</see> for tool window will be shown.
      /// </summary>
      internal event EventHandler ContextMenuForToolWindow;

      /// <summary>
      /// Event raised when AutoHide button was clicked.
      /// </summary>
      internal event EventHandler AutoHideButtonClick;

      /// <summary>
      /// Get the internal title of the tool window
      /// </summary>
      /// <returns>internal title of the tool window</returns>
      internal string InternalTitle ()
      {
         char[] vector = _title.ToArray ();

         return new string (vector);
      }

      /// <summary>
      /// Get the full title of the form
      /// </summary>
      string ITitleData.Title()
      {
         return InternalTitle();
      }

      /// <summary>
      /// Read-only flag which is set when the form is docked and not set when is floating
      /// </summary>
      internal bool IsDocked
      {
         get { return _dockMode != zDockMode.None; }
      }

      /// <summary>
      /// Flag set when the form is on auto-hide mode
      /// </summary>
      internal bool AutoHide
      {
         get { return _autoHide; }
         set 
         {
            if (_autoHide != value)
            {
               _autoHide = value;
               InvalidateForm();
            }
         }
      }

      //by fdc

       public bool IsSelected
       {
           get { return _tabButton.Selected; }
       }

      /// <summary>
      /// Bounds before size-lock
      /// </summary>
      internal Rectangle BoundsBeforeLock
      {
         get { return _boundsBeforeLock; }
      }

      /// <summary>
      /// Get the restore size of the tool window
      /// </summary>
      /// <param name="mousePosition">mouse position in screen coordinates</param>
      /// <returns>restore size</returns>
      internal Size GetRestoreSize (Point mousePosition)
      {
         if (DockMode == zDockMode.Left || DockMode == zDockMode.Right)
         {
            return new Size (Width, BoundsBeforeLock.Height);
         }

         if (DockMode == zDockMode.Top || DockMode == zDockMode.Bottom)
         {
            int preferredWidth = mousePosition.X - PointToScreen (Location).X + 50;
            return new Size (Math.Max (BoundsBeforeLock.Width, preferredWidth), Height);
         }

         return BoundsBeforeLock.Size;
      }

      /// <summary>
      /// Lock the form. This lock should be called when dock begins
      /// </summary>
      /// <param name="lockedBounds">locked bounds</param>
      /// <param name="lockedBorder">locked border</param>
      /// <param name="dockMode">dock mode of the tool window</param>
      internal void LockFormSizeAndDock (Rectangle lockedBounds, FormBorderStyle lockedBorder, zDockMode dockMode)
      {
         if (_dockMode == zDockMode.None)
         {
            _dockMode  = dockMode;
            _boundsBeforeLock = Bounds;
            _borderBeforeLock = FormBorderStyle;
         }

         _resizeIsLocked   = true;
         _lockedBounds     = lockedBounds;
         FormBorderStyle   = lockedBorder;
         Bounds            = lockedBounds;
      }

      /// <summary>
      /// Lock the form. This lock should be called when dock ends
      /// </summary>
      /// <param name="lockedSize">locked size</param>
      internal void LockFormSizeAndUndock (Size lockedSize)
      {
         _resizeIsLocked      = true;
         _lockedBounds.Size   = lockedSize;
         FormBorderStyle      = _borderBeforeLock;
         Size                 = lockedSize;
         _dockMode            = zDockMode.None;
      
         Application.DoEvents ();
      }

      /// <summary>
      /// Unlock the form size
      /// </summary>
      internal void UnlockFormSize ()
      {
         if (_resizeIsLocked)
         {
            _lockedBounds     = _boundsBeforeLock;
            FormBorderStyle   = _borderBeforeLock;
            Bounds            = _lockedBounds;
         }

         _resizeIsLocked  = false;
      }

      /// <summary>
      /// Tab button associated whith the form
      /// </summary>
      internal TabButton TabButton
      {
         get { return _tabButton; }
         set 
         {
             _tabButton = value;
             if (_tabButton != null)
                 _tabButton._onTabButtonSelectedChanged += new OnTabButtonSelectedChanged(TabButtonSelectedChanged);
         }
      }

      private void TabButtonSelectedChanged(object sender, bool isSelected)
      {
          if (_onSelectedChanged != null)
              _onSelectedChanged(this, isSelected);
      }


      /// <summary>
      /// Bounds for auto-hide button in screen coordinates
      /// </summary>
      internal Rectangle AutoButtonScreenBounds
      {
         get
         {
            Rectangle rect = AutoButtonBounds;
            if (Parent != null)
            {
               Point delta = Parent.PointToScreen (Location);
               rect.Offset (delta);
            }

            return rect;
         }
      }

      /// <summary>
      /// Menu button bounds in screen coordinates
      /// </summary>
      internal Rectangle MenuButtonScreenBounds
      {
         get
         {
            Rectangle rect = MenuButtonBounds;
            if (Parent != null)
            {
               Point delta = Parent.PointToScreen (Location);
               rect.Offset (delta);
            }

            return rect;
         }
      }

      /// <summary>
      /// Title bar bounds in screen coordinates
      /// </summary>
      internal Rectangle TitleBarScreenBounds
      {
         get
         {
            Rectangle rect = Bounds;
            if (Parent != null)
            {
               rect = Parent.RectangleToScreen (rect);
            }

            Rectangle rectClient = RectangleToScreen (ClientRectangle);

            rect.Height = rectClient.Top - rect.Top;

            return rect;
         }
      }

      /// <summary>
      /// Check that instance is not disposed
      /// </summary>
      protected void CheckNotDisposed ()
      {
         if (_disposed)
         {
            throw new ObjectDisposedException (this.GetType ().Name);
         }
      }

      /// <summary>
      /// Force the sizes of the window
      /// </summary>
      /// <param name="x">x position of the window</param>
      /// <param name="y">y position of the window</param>
      /// <param name="width">length of the window</param>
      /// <param name="height">height of the window</param>
      /// <param name="specified">size mode</param>
      protected override void SetBoundsCore (int x, int y, int width, int height, BoundsSpecified specified)
      {
         if (_resizeIsLocked)
         {
            if (EnumUtility.Contains (specified, BoundsSpecified.Width))
            {
               width = _lockedBounds.Width;
            }

            if (EnumUtility.Contains (specified, BoundsSpecified.Height))
            {
               height = _lockedBounds.Height;
            }
         }

         base.SetBoundsCore (x, y, width, height, specified);
      }

      /// <summary>
      /// Clean up any resources being used. 
      /// </summary>
      /// <param name="disposing">disposing is performed on user request</param>
      protected override void Dispose (bool disposing)
      {
         if (disposing && _disposed == false)
         {
            _disposed = true;

            SizeChanged -= OnSizeChanged;
            FormClosing -= OnFormClosing;

            if (_timer != null)
            {
               _timer.Enabled = false;
               _timer.Tick -= OnTimer;
               _timer.Dispose();
               _timer = null;
            }
         }
         base.Dispose (disposing);
      }

      /// <summary>
      /// Method for handling the form messages loop
      /// </summary>
      /// <param name="m">message to be handled</param>
      protected override void WndProc (ref Message m)
      {
         base.WndProc (ref m);

         if (_disposed)
         {
            return;
         }

         int message          = (int)m.Msg;
         Point cursorPos      = MousePosition;
         MouseButtons button  = MouseButtons;

         switch (message)
         {
            case SYSCOMMAND:
               {
                  if (AutoButtonScreenBounds.Contains (cursorPos))
                  {
                     if (AutoHideButtonClick != null && _mouseIsOverAutoButton)
                     {
                        AutoHideButtonClick (this, EventArgs.Empty);
                     }
                  }

                  if (MenuButtonScreenBounds.Contains (cursorPos))
                  {
                     if (ContextMenuForToolWindow != null && _mouseIsOverMenuButton)
                     {
                        ContextMenuForToolWindow (this, EventArgs.Empty);
                     }
                  }
               }
               break;

            case NCMOUSEMOVE:
               _timer.Interval = 10;
               break;

            case MOVING:
            case NCLBUTTONDOWN:
            case LBUTTONDOWN:
            case SETCURSOR:
            case NCLBUTTONDBLCLK:
               if (IsDocked && Parent != null && Capture && TitleBarScreenBounds.Contains(MousePosition))
               {
                  // Call get text allows releasing the capture. Let this line here
                  string text = Text;

                  _lastNCMouseDownPos   = cursorPos;
                  _lastPosOnNcMouseDown = Parent.PointToScreen(Location);
                  _captureSuspended     = true;
                  Capture  = false;
                  _timer.Interval = 10;
               }
               break;

            case EXITSIZEMOVE:
               InvalidateForm();
               break;

            case NCLBUTTONUP:
               _captureSuspended      = false;
               _captureReplaceStarted = false;
               break;

            case NCACTIVATE:
            case NCPAINT:
            case PAINT:
            case SETTEXT:
               DrawExtraButtons (m.HWnd);
               break;
         }
      }

      #endregion Protected section.

      #region Private section.

      /// <summary>
      /// InitializeComponent
      /// </summary>
      private void InitializeComponent ()
      {
         this.SuspendLayout ();
         // 
         // DockableToolWindow
         // 
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
         this.ClientSize = new System.Drawing.Size (241, 404);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
         this.Name = "DockableToolWindow";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
         this.Text = "DockableToolWindow";
         this.ResumeLayout (false);

      }

      /// <summary>
      /// Method called when size changed
      /// </summary>
      /// <param name="sender">sender</param>
      /// <param name="e">e</param>
      private void OnSizeChanged (object sender, EventArgs e)
      {
         if (_resizeIsLocked)
         {
            Size = _lockedBounds.Size;
         }
         else
         {
            _boundsBeforeLock = Bounds;
         }

         if (InternalTitle() == null)
         {
            SaveTitleText(Text);
         }

         string titleText   = string.Empty + InternalTitle();
         string currentText = string.Empty + Text;
         if (currentText.EndsWith (TextUtility.EndEllipsis))
         {
            currentText = currentText.Substring(0, currentText.Length - 3);
            if (titleText.StartsWith (currentText) == false)
            {
               SaveTitleText(currentText);
               titleText   = currentText;
            }
         }
         else if (currentText != titleText && currentText != string.Empty)
         {
            SaveTitleText(currentText);
            titleText   = currentText;
         }

         Text = TextUtility.WrapText (titleText, MenuButtonBounds.Left - 16, Font);
         InvalidateForm();
      }

      /// <summary>
      /// Method called when close event occurs
      /// </summary>
      /// <param name="sender">sender</param>
      /// <param name="e">e</param>
      private void OnFormClosing (object sender, FormClosingEventArgs e)
      {
         if (e.CloseReason == CloseReason.UserClosing)
         {
            if (HideInsteadClosing)
            {
               e.Cancel = true;
               Hide ();
            }
         }
      }

      /// <summary>
      /// Method called when timer event occurs
      /// </summary>
      /// <param name="sender">sender</param>
      /// <param name="e">e</param>
      private void OnTimer (object sender, EventArgs e)
      {
         if (Visible == false || Parent == null)
         {
            return;
         }

         Point mousePosition  = MousePosition;
         MouseButtons button  = MouseButtons;
         bool invalidate      = false;

         if (PerformCaptureReplace (mousePosition, button))
         {
            return;
         }

         if (AutoButtonScreenBounds.Contains (mousePosition))
         {
            if (_mouseIsOverAutoButton == false)
            {
               _mouseIsOverAutoButton = true;
               invalidate = true;
            }
         }
         else if (_mouseIsOverAutoButton)
         {
            _mouseIsOverAutoButton = false;
            invalidate = true;
         }

         if (MenuButtonScreenBounds.Contains (mousePosition))
         {
            if (_mouseIsOverMenuButton == false)
            {
               _mouseIsOverMenuButton   = true;
               invalidate = true;
            }
         }
         else if (_mouseIsOverMenuButton)
         {
            _mouseIsOverMenuButton = false;
            invalidate = true;
         }

         if (invalidate)
         {
            InvalidateForm ();
         }
         else
         {
            _timer.Interval = 10000;
         }
      }

      /// <summary>
      /// Save title text
      /// </summary>
      /// <param name="titleText"></param>
      private void SaveTitleText (string titleText)
      {
         _title.Clear();

         if (titleText != null)
         {
            _title.AddRange (titleText.ToCharArray ());
         }
      }

      /// <summary>
      /// Draw extra buttons on form title bar
      /// </summary>
      /// <param name="m">m</param>
      private void DrawExtraButtons (IntPtr hWnd)
      {
         if (_dockMode == zDockMode.None || FormBorderStyle == FormBorderStyle.None || Visible == false)
         {
            return;
         }

         //IntPtr dc = GetDCEx (hWnd, new IntPtr (1), DCX.WINDOW | DCX.INTERSECTRGN | DCX.CACHE | DCX.CLIPSIBLINGS);
         IntPtr dc = GetDCEx (hWnd, IntPtr.Zero, (DCX.WINDOW | DCX.CACHE | DCX.CLIPSIBLINGS));
         if (dc == IntPtr.Zero)
         {
            return;
         }

         Rectangle rectButonAuto = AutoButtonBounds;
         Rectangle rectAutoEcran = AutoButtonScreenBounds;
         Rectangle rectButonMenu = MenuButtonBounds;
         Rectangle rectMenuEcran = MenuButtonScreenBounds;
         using (Graphics g = Graphics.FromHdc (dc))
         {
            DrawUtility.DrawAutoHideButton (rectButonAuto, AutoHide, rectAutoEcran.Contains (MousePosition), g);
            DrawUtility.DrawContextMenuButton (rectButonMenu, rectMenuEcran.Contains (MousePosition), g);
         }

         ReleaseDC (hWnd, dc);
      }

      /// <summary>
      /// Perform capture replace
      /// </summary>
      /// <param name="mousePosition">mouse position</param>
      /// <param name="mouseButton">mouse button</param>
      /// <returns>true if capture replace was performed</returns>
      private bool PerformCaptureReplace (Point mousePosition, MouseButtons mouseButton)
      {
         if (_captureSuspended && mouseButton == MouseButtons.Left && Parent != null)
         {
            if (_captureReplaceStarted == false)
            {
               _captureReplaceStarted = true;
            }

            int deltaX = _lastNCMouseDownPos.X - _lastPosOnNcMouseDown.X;
            int deltaY = _lastNCMouseDownPos.Y - _lastPosOnNcMouseDown.Y;

            Location   = Parent.PointToClient(new Point (mousePosition.X - deltaX, mousePosition.Y - deltaY));

            return true;
         }
         else if (mouseButton != MouseButtons.Left && _captureReplaceStarted)
         {
            _captureSuspended       = false;
            _captureReplaceStarted  = false;
         }

         return false;
      }

      /// <summary>
      /// Invalidate form
      /// </summary>
      private void InvalidateForm ()
      {
         Invalidate ();
         // Force title redraw
         string text = Text;
         Text = Text + " ";
         Text = text;
      }

      /// <summary>
      /// Auto-button bounds
      /// </summary>
      private Rectangle AutoButtonBounds
      {
         get 
         {
            int captionHeight = SystemInformation.ToolWindowCaptionHeight;
            int posOffset     = captionHeight;

            int buttonHeight  = 2 * captionHeight / 3;
            int buttonWidth   = buttonHeight;

            Rectangle titleBounds = TitleBarScreenBounds;
            int top = (titleBounds.Height - buttonHeight) / 2;
            if (2 * top + buttonHeight < titleBounds.Height)
            {
               top++;
            }

            // Last pos is for close button, previous for auto button, so extract 2 * posOffset from width
            return new Rectangle (Width - 2 * posOffset, top, buttonWidth, buttonHeight); 
         }
      }

      /// <summary>
      /// Menu button bounds
      /// </summary>
      private Rectangle MenuButtonBounds
      {
         get
         {
            int captionHeight = SystemInformation.ToolWindowCaptionHeight;
            int posOffset     = captionHeight;

            Rectangle rect = AutoButtonBounds;
            rect.Offset (-posOffset, 0);

            return rect;
         }
      }

      /// <summary>
      /// DCX enum
      /// </summary>
      private enum DCX : int
      {
         WINDOW = 0x00000001,
         CACHE = 0x00000002,
         NORESETATTRS = 0x00000004,
         CLIPCHILDREN = 0x00000008,
         CLIPSIBLINGS = 0x00000010,
         PARENTCLIP = 0x00000020,
         EXCLUDERGN = 0x00000040,
         INTERSECTRGN = 0x00000080,
         EXCLUDEUPDATE = 0x00000100,
         INTERSECTUPDATE = 0x00000200,
         LOCKWINDOWUPDATE = 0x00000400,
         VALIDATE = 0x00200000
      }

      [DllImport ("User32.dll")]
      private static extern IntPtr GetDCEx (IntPtr hWnd, IntPtr hrgn, DCX flags);

      [DllImport ("User32.dll")]
      private static extern int ReleaseDC (IntPtr hWnd, IntPtr hDC);

      [DllImport ("User32.dll")]
      private static extern bool PostMessage (IntPtr hWnd, int mesaj, IntPtr wParam, IntPtr lParam);

      [DllImport ("User32.dll")]
      private static extern IntPtr SetCapture (IntPtr hWnd);

      [DllImport ("User32.dll")]
      private static extern bool ReleaseCapture();

      [DllImport ("User32.dll")]
      private static extern IntPtr GetCapture();


      #endregion Private section.
   }
}
