using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Telerik.WinControls.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Design;
using System.ComponentModel.Design;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Represents a context menu 
	/// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.MenuAndToolbarsGroup)]
    [ToolboxItem(true)]
	[RadToolboxItem(false)]
	[DefaultProperty("Items")]
    [Designer(DesignerConsts.RadContextMenuDesignerString)]
    [ToolboxBitmap(typeof(Telerik.WinControls.UI.RadContextMenu), "RadDropDownMenu.bmp")]
    public class RadContextMenu : Component
    {
        private RadContextMenuDropDown menu;

        public RadContextMenu(): this(null)
        {
        }

        public RadContextMenu(IContainer owner)
        {
            if (owner != null)
            {
                owner.Add(this);
            }

            this.menu = new RadContextMenuDropDown();
            this.menu.DropDownOpening += new CancelEventHandler(menu_DropDownOpening);
            this.menu.DropDownOpened += new EventHandler(menu_DropDownOpened);
            this.menu.DropDownClosing += new RadPopupClosingEventHandler(menu_DropDownClosing);
            this.menu.DropDownClosed += new RadPopupClosedEventHandler(menu_DropDownClosed);
            this.menu.Items.ItemsChanged += new ItemChangedDelegate(Items_ItemsChanged);
        }

        #region Events

        /// <summary>
        /// Occurs when the drop down is opening.
        /// </summary>
        public event CancelEventHandler DropDownOpening;

        /// <summary>
        /// Occurs when the drop down is closing.
        /// </summary>
        public event CancelEventHandler DropDownClosing;

        /// <summary>
        /// Occurs when the drop down is opened.
        /// </summary>
        public event EventHandler DropDownOpened;

        /// <summary>
        /// Occurs when the drop down is closed.
        /// </summary>
        public event EventHandler DropDownClosed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets menu items collection
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [Description("Gets menu items collection")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemOwnerCollection Items
        {
            get { return this.DropDown.Items; }
        }

        /// <summary>
        ///     Gets or sets control's preffered theme name. Themes are stored and retrieved using
        ///     APIs of <see cref="Telerik.WinControls.ThemeResolutionService"/>.
        /// </summary>
        /// <remarks>
        /// If <strong>ThemeResolutionService.ApplicatonThemeName</strong> refers to a
        /// non-empty string, the theme of a RadControl can differ from the one set using
        /// <strong>RadControls.ThemeName</strong> property. If the themes differ, the
        /// <strong>RadControls.ThemeName</strong> property will be overridden by
        /// <strong>ThemeResolutionService.ApplicatonThemeName</strong>. If no theme is registered
        /// with a name as <strong>ThemeResolutionService.ApplicatonThemeName</strong>, then
        /// control will revert to the theme specified by its <strong>ThemeName</strong> property.
        /// If <strong>ThemeName</strong> is assigned to a non-existing theme name, the control may
        /// have no visual properties assigned, which will cause it look and behave in unexpected
        /// manner. If <strong>ThemeName</strong> equals empty string, control's theme is set to a
        /// theme that is registered within <strong>ThemeResolutionService</strong> with the name
        /// "ControlDefault".
        /// </remarks>
        [Browsable(true), Category(RadDesignCategory.StyleSheetCategory)]
        [Description("Gets or sets theme name.")]
        [DefaultValue((string)"")]        
        [Editor(DesignerConsts.ThemeNameEditorString, typeof(UITypeEditor))]
        public string ThemeName
        {
            get { return this.DropDown.ThemeName; }
            set { this.DropDown.ThemeName = value; }
        }

        /// <summary>
        /// Gets or sets the ImageList that contains the images displayed by this control.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the ImageList that contains the images displayed by this control.")]
        [DefaultValue(null)]
        public ImageList ImageList
        {
            get { return this.DropDown.ImageList; }
            set { this.DropDown.ImageList = value; }
        }
        
        /// <summary>
        /// Gets menu drop down panel
        /// </summary>       
        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [Description("Gets menu drop down panel")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadDropDownMenu DropDown
        {
            get { return this.menu; }
        }

        #endregion

        #region Public methods
		
        /// <summary>
		/// Displays the context menu in its default position.
		/// </summary>
		public void Show()
		{
			this.DropDown.Show();
		}

		/// <summary>
        /// Displays the context menu relative to the specified screen location.
		/// </summary>
		/// <param name="x">The horizontal screen coordinate, in pixels.</param>
		/// <param name="y">The vertical screen coordinate, in pixels.</param>
		public void Show(int x, int y)
		{
			this.DropDown.Show(x, y);
		}

		/// <summary>
        /// Displays the context menu relative to the specified screen location.
		/// </summary>
		/// <param name="point">The horizontal and vertical location of the screen's upper-left corner, in pixels.</param>
		public void Show(Point point)
		{
			this.DropDown.Show(point);
		}

		/// <summary>
        /// Positions the context menu relative to the specified screen location and with the specified direction.
		/// </summary>
		/// <param name="point">The horizontal and vertical location of the screen's upper-left corner, in pixels.</param>
		/// <param name="popupDirection">One of the RadDirection values.</param>
		public void Show(Point point, RadDirection popupDirection)
		{
			this.DropDown.Show(point, popupDirection);
		}

		/// <summary>
        /// Positions the context menu relative to the specified control location.
		/// </summary>
		/// <param name="control">The control that is the reference point for the RadDropDownMenu position.</param>
		/// <param name="x">The horizontal coordinate relative to the control, in pixels.</param>
		/// <param name="y">The vertical coordinate relative to the control, in pixels.</param>
		public void Show(Control control, int x, int y)
		{
			this.DropDown.Show(control, x, y);
		}

		/// <summary>
        /// Positions the context menu relative to the specified control location.
		/// </summary>
		/// <param name="control">The control that is the reference point for the RadDropDownMenu position.</param>
		/// <param name="point">The horizontal and vertical location of the reference control's upper-left corner, in pixels.</param>
		public void Show(Control control, Point point)
		{
			this.DropDown.Show(control, point);
		}

		/// <summary>
        /// Positions the context menu relative to the specified control location and with the specified direction.
		/// </summary>
		/// <param name="control">The control that is the reference point for the RadDropDownMenu position.</param>
		/// <param name="point">The horizontal and vertical location of the reference control's upper-left corner, in pixels.</param>
		/// <param name="popupDirection">One of the RadDirection values.</param>
		public void Show(Control control, Point point, RadDirection popupDirection)
		{
			this.DropDown.Show(control, point, popupDirection);
		}

		/// <summary>
        /// Positions the context menu relative to the specified RadItem location.
		/// </summary>
		/// <param name="item">The RadItem that is the reference point for the RadDropDownMenu position.</param>
		/// <param name="x">The horizontal coordinate relative to the control, in pixels.</param>
		/// <param name="y">The vertical coordinate relative to the control, in pixels.</param>
		public void Show(RadItem item, int x, int y)
		{
			this.DropDown.Show(item, x, y);
		}

		/// <summary>
        /// Positions the context menu relative to the specified RadItem location.
		/// </summary>
		/// <param name="item">The RadItem that is the reference point for the RadDropDownMenu position.</param>
		/// <param name="point">The horizontal and vertical location of the RadItem's upper-left corner, in pixels.</param>
		public void Show(RadItem item, Point point)
		{
			this.DropDown.Show(item, point);
		}

		/// <summary>
        /// Positions the context menu relative to the specified RadItem location and with the specified direction.
		/// </summary>
		/// <param name="item">The RadItem that is the reference point for the RadDropDownMenu position.</param>
		/// <param name="point">The horizontal and vertical location of the RadItem's upper-left corner, in pixels.</param>
		/// <param name="popupDirection">One of the RadDirection values.</param>
		public void Show(RadItem item, Point point, RadDirection popupDirection)
		{
			this.DropDown.Show(item, point, popupDirection);
		}

		/// <summary>
        /// Positions the context menu relative to the specified RadItem location and 
		/// with specified direction and offset according to the owner.
		/// </summary>
		/// <param name="item">The RadItem that is the reference point for the RadDropDownMenu position.</param>
		/// <param name="ownerOffset">Specifies the offset from the owner in pixels.</param>
		/// <param name="popupDirection">One of the RadDirection values.</param>
		public void Show(RadItem item, int ownerOffset, RadDirection popupDirection)
		{
			this.DropDown.Show(item, ownerOffset, popupDirection);
		}

        #endregion

        #region Event handlers

        /// <summary>
        /// Raises the DropDownOpening event.
        /// </summary>
        /// <param name="args">The event arguments</param>
        protected virtual void OnDropDownOpening(CancelEventArgs args)
        {
            if (DropDownOpening != null)
            {
                DropDownOpening(this, args);
            }
        }

        /// <summary>
        /// Raises the DropDownClosing event.
        /// </summary>
        /// <param name="args">The event arguments</param>
        protected virtual void OnDropDownClosing(CancelEventArgs args)
        {
            if (DropDownClosing != null)
            {
                DropDownClosing(this, args);
            }
        }

        /// <summary>
        /// Raises the DropDownOpened event.
        /// </summary>
        protected virtual void OnDropDownOpened()
        {
            if (DropDownOpened != null)
            {
                DropDownOpened(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the DropDownClosed event.
        /// </summary>
        protected virtual void OnDropDownClosed()
        {
            if (DropDownClosed != null)
            {
                DropDownClosed(this, EventArgs.Empty);
            }
        }

        private void menu_DropDownClosed(object sender, EventArgs e)
        {
            OnDropDownClosed();
        }

        private void menu_DropDownClosing(object sender, CancelEventArgs e)
        {
            OnDropDownClosing(e);
        }

        private void menu_DropDownOpened(object sender, EventArgs e)
        {
            OnDropDownOpened();
        }

        private void menu_DropDownOpening(object sender, CancelEventArgs e)
        {
            OnDropDownOpening(e);
        }

        private void Items_ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            if (operation == ItemsChangeOperation.Inserted)
            {
                RadMenuItemBase menuItem = target as RadMenuItemBase;
                if (menuItem != null)
                {
                    menuItem.OwnerControl = this.menu;
                }
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.DropDown.DropDownOpening -= new CancelEventHandler(menu_DropDownOpening);
                this.DropDown.DropDownOpened -= new EventHandler(menu_DropDownOpened);
                this.DropDown.DropDownClosing -= new RadPopupClosingEventHandler(menu_DropDownClosing);
                this.DropDown.DropDownClosed -= new RadPopupClosedEventHandler(menu_DropDownClosed);
                this.menu.Items.ItemsChanged -= new ItemChangedDelegate(Items_ItemsChanged);
                this.DropDown.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
