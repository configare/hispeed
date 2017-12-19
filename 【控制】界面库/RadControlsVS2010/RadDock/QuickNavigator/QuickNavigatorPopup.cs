using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Keyboard;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents a pop-up form which displays all currently opened tool windows and documents.
    /// </summary>
    internal class QuickNavigatorPopup : Form
    {
        #region Constructor

        public QuickNavigatorPopup(RadDock dockManager)
        {
            this.dockManager = dockManager;
            this.RightToLeft = dockManager.RightToLeft;
            this.FormBorderStyle = FormBorderStyle.None;
            this.ShowInTaskbar = false;
            this.KeyPreview = true;
            this.StartPosition = FormStartPosition.Manual;

            this.navigator = new QuickNavigator(dockManager);
            this.navigator.Dock = DockStyle.Fill;
            this.navigator.Parent = this;

            this.navigator.RegionChanged += new EventHandler(OnQuickNavigator_RegionChanged);
        }

        

        #endregion

        #region Overrides

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                if (this.dockManager != null)
                {
                    if (this.dockManager.QuickNavigatorSettings.DropShadow &&
                        OSFeature.IsPresent(SystemParameter.DropShadow))
                    {
                        cp.ClassStyle |= NativeMethods.CS_DROPSHADOW;
                    }
                }

                return cp;
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.navigator.SelectedItem == null || this.Disposing || this.IsDisposed)
            {
                return;
            }

            Keys key = e.KeyCode;
            e.Handled = true;

            switch (key)
            {
                case Keys.Left:
                    this.navigator.NavigateLeft();
                    return;
                case Keys.Right:
                    this.navigator.NavigateRight();
                    return;
                case Keys.Up:
                    this.navigator.NavigateUp();
                    return;
                case Keys.Down:
                case Keys.Tab:
                    this.navigator.NavigateDown();
                    return;
            }

            //close the popup upon any other key
            this.Close();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Control || e.KeyCode == Keys.ControlKey) && AutoHide)
            {
                this.Close();
            }
            else
            {
                base.OnKeyUp(e);
            }
        }

        protected override void OnDeactivate(EventArgs e)
        {
            base.OnDeactivate(e);

            if (this.IsDisposed || this.Disposing)
            {
                return;
            }

            if (AutoHide)
            {
                //we need to hide ourselves
                this.Close();
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            DockWindow pane = this.SelectedPane;
            if (pane != null)
            {
                this.dockManager.ActiveWindow = pane;
            }
        }

        private void OnQuickNavigator_RegionChanged(object sender, EventArgs e)
        {
            this.Region = this.navigator.Region;
        }

        #endregion

        #region Implementation

        public void Display()
        {
            Form owner = this.dockManager.FindForm();
            if (owner == null)
            {
                return;
            }

            this.navigator.ThemeName = this.dockManager.ThemeName;
            this.navigator.Initialize();
            Size preferredSize = this.navigator.GetPreferredSize();
            //check whether we have something to display
            if (preferredSize == Size.Empty)
            {
                return;
            }

            //calculate screen bounds for the popup
            this.Bounds = GetDisplayBounds(preferredSize);
            //display the form
            
            this.Show();

            //initially move to the next document in the z-order (mimic Visual Studio)
            this.navigator.NavigateDown();
        }

        private Rectangle GetDisplayBounds(Size preferredSize)
        {
            Rectangle alignBounds = Rectangle.Empty;
            Rectangle screen = Screen.FromControl(this.dockManager).WorkingArea;

            switch (this.dockManager.QuickNavigatorSettings.DisplayPosition)
            {
                case QuickNavigatorDisplayPosition.CenterDockManager:
                    alignBounds = this.dockManager.Parent.RectangleToScreen(dockManager.Bounds);
                    break;
                case QuickNavigatorDisplayPosition.CenterMainForm:
                    alignBounds = this.dockManager.TopLevelControl.Bounds;
                    break;
                case QuickNavigatorDisplayPosition.CenterScreen:
                    alignBounds = screen;
                    break;
            }
            
            int edgeOffset = 5;
            int x = alignBounds.X + (alignBounds.Width - preferredSize.Width) / 2;
            int y = alignBounds.Y + (alignBounds.Height - preferredSize.Height) / 2;

            x = Math.Max(screen.X + edgeOffset, x);
            y = Math.Max(screen.Y + edgeOffset, y);

            if (x + preferredSize.Width + edgeOffset > screen.Right)
            {
                x = screen.Right - preferredSize.Width - edgeOffset;
            }
            if (y + preferredSize.Height + edgeOffset > screen.Bottom)
            {
                y = screen.Bottom - preferredSize.Height - edgeOffset;
            }

            return new Rectangle(x, y, preferredSize.Width, preferredSize.Height);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the currently DockWindow that is currently selected in the navigator.
        /// </summary>
        public DockWindow SelectedPane
        {
            get
            {
                QuickNavigatorListItem item = this.navigator.SelectedItem;
                if (item != null)
                {
                    return item.Tag as DockWindow;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the QuickNavigator instance displayed on this popup.
        /// </summary>
        public QuickNavigator Navigator
        {
            get
            {
                return this.navigator;
            }
        }

        #endregion

        #region Fields

        //a flag for testing purposes
        internal static bool AutoHide = true;
        private QuickNavigator navigator;
        private RadDock dockManager;

        #endregion
    }
}
