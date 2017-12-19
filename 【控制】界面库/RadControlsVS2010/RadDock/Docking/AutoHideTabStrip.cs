using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.UI;
using Telerik.WinControls.Design;
using System.Diagnostics;
using Telerik.WinControls.UI.Localization;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A predefined <see cref="ToolTabStrip">ToolTabStrip</see> instance that resides within an <see cref="AutoHidePopup">AutoHidePopup</see> and is used to display auto-hidden dock windows.
    /// </summary>
    public class AutoHideTabStrip : ToolTabStrip
    {
        #region Fields

        private DockStyle autoHideDock = DockStyle.Left;
        private int lastSplitterDistance = -1;
        private int splitterDistance = -1;
        private int beginSplitterDistance;
        private bool fixedSplitter = false;
        private bool resizing = false;
        private List<AutoHideGroup> autoHideGroups = new List<AutoHideGroup>();

        private DockLayoutPanel panel;
        internal SplitterElement splitterElement;

        #endregion

        #region Constructors/Initializers

        static AutoHideTabStrip()
        {
            //ThemeResolutionService.RegisterThemeFromStorage(ThemeStorageType.Resource, "Telerik.WinControls.UI.Docking.Resources.ControlDefault.AutoHideTabStrip.xml");
            new Telerik.WinControls.Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_Docking_AutoHideTabStrip().DeserializeTheme();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AutoHideTabStrip()
			: base(null)
		{
		}

        /// <summary>
        /// Initializes a new <see cref="AutoHideTabStrip">AutoHideTabStrip</see> instance and associates it with the provided RadDock instance.
        /// </summary>
        /// <param name="dockManager"></param>
        public AutoHideTabStrip(RadDock dockManager)
            :base(dockManager)
        {
            this.RightToLeft = dockManager.RightToLeft;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public override string ThemeClassName
        {
            get
            {
                return typeof(AutoHideTabStrip).FullName;
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        /// <summary>
        /// Gets or sets the width of the splitter.
        /// </summary>
        public int SplitterWidth
        {
            get 
            {
                return this.splitterElement.SplitterWidth; 
            }
            set
            {
                if (this.splitterElement != null)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException("Invalid value");
                    }

                    this.splitterElement.SplitterWidth = value;
                }
            }
        }

        /// <summary>
        /// Determines whether the splitter is fixed (may be used to resize the owning popup).
        /// </summary>
        public bool FixedSplitter
        {
            get { return this.fixedSplitter; }
            set { this.fixedSplitter = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);

            if (this.window.Parent != null)
            {
                this.window.Parent.Children.Remove(this.window);
            }

            this.panel = new DockLayoutPanel();
            this.splitPanelElement.Children.Add(this.panel);
            this.splitterElement = new SplitterElement();
            DockLayoutPanel.SetDock(this.splitterElement, Telerik.WinControls.Layouts.Dock.Right);
            panel.Children.Add(this.splitterElement);
            panel.Children.Add(this.window);

            this.splitterElement.PrevNavigationButton.Visibility = ElementVisibility.Hidden;
            this.splitterElement.NextNavigationButton.Visibility = ElementVisibility.Hidden;

            this.CloseButton.ToolTipText = RadDockLocalizationProvider.CurrentProvider.GetLocalizedString(RadDockStringId.ToolTabStripCloseButton);
            this.AutoHideButton.ToolTipText = RadDockLocalizationProvider.CurrentProvider.GetLocalizedString(RadDockStringId.ToolTabStripPinButton);
        }

        protected internal override void OnLocalizationProviderChanged()
        {
            base.OnLocalizationProviderChanged();

            this.CloseButton.ToolTipText = RadDockLocalizationProvider.CurrentProvider.GetLocalizedString(RadDockStringId.ToolTabStripCloseButton);
            this.AutoHideButton.ToolTipText = RadDockLocalizationProvider.CurrentProvider.GetLocalizedString(RadDockStringId.ToolTabStripPinButton);
        }

        /// <summary>
        /// Do not handle the mouse double-click event.
        /// </summary>
        /// <returns></returns>
        protected override bool ShouldHandleDoubleClick()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public override System.Drawing.Rectangle TabPanelBounds
        {
            get
            {
                Rectangle rect = base.TabPanelBounds;
                if (this.autoHideDock == DockStyle.Bottom)
                {
                    rect.Y += this.SplitterWidth;
                    rect.Height -= this.SplitterWidth;
                }
                else if (this.autoHideDock == DockStyle.Right)
                {
                    rect.X += this.SplitterWidth;
                    rect.Width -= this.SplitterWidth;
                }
                else if (this.autoHideDock == DockStyle.Top)
                {
                    rect.Height -= this.SplitterWidth;
                }
                else if (this.autoHideDock == DockStyle.Left)
                {
                    rect.Width -= this.SplitterWidth;
                }

                return rect;
            }
        }

        /// <summary>
        /// Gets the DockStyle which determines the edge at which the owning auto-hide popup is displayed.
        /// </summary>
        [DefaultValue(DockStyle.Left)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockStyle AutoHideDock
        {
            get { return autoHideDock; }
            internal set
            {
                if (((value != DockStyle.Top) && (value != DockStyle.Bottom)) && ((value != DockStyle.Left) && (value != DockStyle.Right)))
                {
                    throw new ArgumentException(SR.GetString("AutoHideInvalidDockEnum"));
                }

                autoHideDock = value;
                switch (value)
                {
                    case DockStyle.Bottom:
                        DockLayoutPanel.SetDock(this.splitterElement, Telerik.WinControls.Layouts.Dock.Top);
                        break;
                    case DockStyle.Left:
                        DockLayoutPanel.SetDock(this.splitterElement, Telerik.WinControls.Layouts.Dock.Right);
                        break;
                    case DockStyle.Right:
                        DockLayoutPanel.SetDock(this.splitterElement, Telerik.WinControls.Layouts.Dock.Left);
                        break;
                    case DockStyle.Top:
                        DockLayoutPanel.SetDock(this.splitterElement, Telerik.WinControls.Layouts.Dock.Bottom);
                        break;
                }

                this.splitterElement.Dock = autoHideDock;
                if (this.ActiveWindow != null)
                {
                    this.ActiveWindow.Bounds = this.TabPanelBounds;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (this.fixedSplitter || e.Button != MouseButtons.Left)
            {
                return;
            }

            switch (this.AutoHideDock)
            {
                case DockStyle.Left:
                    if (e.X < this.Width - this.SplitterWidth)
                    {
                        return;
                    }
                    break;
                case DockStyle.Right:
                    if (e.X > this.SplitterWidth)
                    {
                        return;
                    }
                    break;
                case DockStyle.Top:
                    if (e.Y < this.Height - this.SplitterWidth)
                    {
                        return;
                    }
                    break;
                case DockStyle.Bottom:
                    if (e.Y > this.SplitterWidth)
                    {
                        return;
                    }
                    break;
            }

            if (this.AutoHideDock == DockStyle.Left || this.AutoHideDock == DockStyle.Right)
            {
                beginSplitterDistance = Control.MousePosition.X - this.SplitterCenter;
            }
            else if (this.AutoHideDock == DockStyle.Top || this.AutoHideDock == DockStyle.Bottom)
            {
                beginSplitterDistance = Control.MousePosition.Y - this.SplitterCenter;
            }

            resizing = true;
        }

        private int SplitterCenter
        {
            get
            {
                return this.SplitterWidth / 2;
            }
        }

        private Rectangle SplitterBounds
        {
            get
            {
                Rectangle rect = this.DockManager.RectangleToScreen(this.DockManager.ContentRectangle);
                rect.Inflate(-25, -25);

                return rect;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (this.fixedSplitter)
            {
                return;
            }

            if (e.Button == MouseButtons.Left && resizing)
            {
                int pos = splitterDistance;

                if (this.AutoHideDock == DockStyle.Left || this.AutoHideDock == DockStyle.Right)
                {
                    int location = Control.MousePosition.X - this.SplitterCenter;
                    if (location > this.SplitterBounds.Left && location < this.SplitterBounds.Right)
                    {
                        pos = location;
                    }
                }
                else
                {
                    int location = Control.MousePosition.Y - this.SplitterCenter;
                    if (location > this.SplitterBounds.Top && location < this.SplitterBounds.Bottom)
                    {
                        pos = location;
                    }
                }

                if (splitterDistance != pos)
                {
                    splitterDistance = pos;
                    DrawSplitBar();
                }
            }

            switch (this.AutoHideDock)
            {
                case DockStyle.Left:
                    if (e.X <= this.Width && e.X >= this.Width - this.SplitterWidth)
                    {
                        Cursor = Cursors.VSplit;
                        return;
                    }
                    break;

                case DockStyle.Right:
                    if (e.X <= this.SplitterWidth && e.X >= 0)
                    {
                        Cursor = Cursors.VSplit;
                        return;
                    }
                    break;

                case DockStyle.Top:
                    if (e.Y <= this.Height && e.Y >= this.Height - this.SplitterWidth)
                    {
                        Cursor = Cursors.HSplit;
                        return;
                    }
                    break;

                case DockStyle.Bottom:
                    if (e.Y >= 0 && e.Y <= this.SplitterWidth)
                    {
                        Cursor = Cursors.HSplit;
                        return;
                    }
                    break;
            }

            this.Cursor = Cursors.Default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (!resizing)
            {
                this.Cursor = Cursors.Arrow;
            }
        }

        /// <summary>
        /// The auto-hide button is not checked.
        /// </summary>
        /// <returns></returns>
        protected override bool GetAutoHideButtonChecked()
        {
            return false;
        }

        /// <summary>
        /// The tabstrip element is never displayed.
        /// </summary>
        /// <returns></returns>
        protected override bool GetTabStripVisible()
        {
            return false;
        }

        /// <summary>
        /// Collapsed state is not valid for this type.
        /// </summary>
        /// <returns></returns>
        protected override bool GetCollapsed()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (this.fixedSplitter || !this.resizing)
            {
                return;
            }

            Cursor = Cursors.Arrow;
            resizing = false;
            DrawSplitBar();
            UpdateBoundsFromSplitter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateFocus"></param>
        protected internal override void UpdateTabSelection(bool updateFocus)
        {
            //call the base method with FALSE parameter to pevent unwanted window activation
            base.UpdateTabSelection(false);
        }

        private void DrawSplitBar()
        {
            if (this.lastSplitterDistance != -1)    //erase the drawed splitter
            {
                TelerikPaintHelper.DrawHalftoneLine(this.CalcSplitRectangle(this.lastSplitterDistance));
            }
            if (resizing)   //draw current splitter
            {
                TelerikPaintHelper.DrawHalftoneLine(this.CalcSplitRectangle(this.splitterDistance));
                this.lastSplitterDistance = this.splitterDistance;
            }
            else
            {
                this.lastSplitterDistance = -1;
            }
        }

        private Rectangle CalcSplitRectangle(int splitterDistance)
        {
            if (this.AutoHideDock == DockStyle.Left || this.AutoHideDock == DockStyle.Right)
            {
                return new Rectangle(splitterDistance, this.Parent.Top, this.SplitterWidth, this.Parent.Height);
            }
            else if (this.AutoHideDock == DockStyle.Top || this.AutoHideDock == DockStyle.Bottom)
            {
                return new Rectangle(this.Parent.Left, splitterDistance, this.Parent.Width, this.SplitterWidth);
            }

            return Rectangle.Empty;
        }

        private void UpdateBoundsFromSplitter()
        {
            Size autoHideSize = (this.ActiveWindow != null) ? this.ActiveWindow.AutoHideSize : Size.Empty;

            switch (this.AutoHideDock)
            {
                case DockStyle.Bottom:
                    this.Parent.Bounds = new Rectangle(this.Parent.Left, this.Parent.Top - (beginSplitterDistance - splitterDistance),
                        this.Parent.Width, this.Parent.Height + (beginSplitterDistance - splitterDistance));
                    autoHideSize = new Size(autoHideSize.Width, this.Bounds.Height);
                    break;
                case DockStyle.Left:
                    this.Parent.Width += splitterDistance - beginSplitterDistance;
                    autoHideSize = new Size(this.Bounds.Width, autoHideSize.Width);

                    break;
                case DockStyle.Right:
                    this.Parent.Bounds = new Rectangle(this.Parent.Left - (beginSplitterDistance - splitterDistance),
                        this.Parent.Top, this.Parent.Width + (beginSplitterDistance - splitterDistance), this.Parent.Height);
                    autoHideSize = new Size(this.Bounds.Width, autoHideSize.Width);
                    break;
                case DockStyle.Top:
                    this.Parent.Height += splitterDistance - beginSplitterDistance;
                    autoHideSize = new Size(autoHideSize.Width, this.Bounds.Height);
                    break;
            }

            if (this.ActiveWindow != null)
            {
                this.ActiveWindow.AutoHideSize = autoHideSize;
            }
        }

        internal void AddAutoHideGroup(AutoHideGroup group)
        {
            this.autoHideGroups.Add(group);
        }

        internal void RemoveAutoHideGroup(AutoHideGroup group)
        {
            int index = this.autoHideGroups.IndexOf(group);
            if (index != -1)
            {
                group.Windows.Clear();
                this.autoHideGroups.RemoveAt(index);
            }
        }

        internal void RemoveAutoHideGroup(DockWindow window)
        {
            int index = this.FindGroupIndex(window);
            if (index == -1)
            {
                return;
            }

            AutoHideGroup group = this.autoHideGroups[index];
            group.Windows.Clear();
            this.autoHideGroups.RemoveAt(index);
        }

        internal AutoHideGroup GetAutoHideGroup(DockWindow window)
        {
            int index = this.FindGroupIndex(window);
            if (index != -1)
            {
                return this.autoHideGroups[index];
            }

            return null;
        }

        private int FindGroupIndex(DockWindow window)
        {
            for (int i = 0; i < this.autoHideGroups.Count; i++)
            {
                AutoHideGroup group = this.autoHideGroups[i];
                for (int j = 0; j < group.Windows.Count; j++)
                {
                    if (group.Windows[j] == window)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        internal void CleanUpGroups()
        {
            for (int i = 0; i < this.autoHideGroups.Count; i++)
            {
                AutoHideGroup group = this.autoHideGroups[i];
                if (group.Windows.Count == 0)
                {
                    this.autoHideGroups.RemoveAt(i--);
                }
            }
        }
    }
}
