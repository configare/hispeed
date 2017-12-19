using Telerik.WinControls;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Telerik.WinControls.Design;
using Telerik.WinControls.Themes.Design;
using System;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// <para>Represents a title bar. This control helps in creation of borderless forms by
    /// substituting the system title bar. Subscribe for radTitleBar events to implement 
    /// the actual action for the the corresponding event. For example, on Close event 
    /// close the form of your application.</para>
    /// <para>Use the Visual Style Builder to change the default appearance and the visible
    /// elements. For example the system menu is not visible by default.</para>
    /// </summary>
    [RadThemeDesignerData(typeof(RadTitleBarDesignTimeData))]
    [ToolboxItem(true)]
    [Docking(DockingBehavior.Ask)]
    [Description("Used to add a titlebar to a shaped form")]
    public class RadTitleBar : RadControl
    {
        private RadTitleBarElement titleBarElement;
        private ContextMenu menu;
        private Form associatedForm = null;

        static RadTitleBar()
        {			
        }
        
        /// <summary>
        /// Initializes a new instance of the RadTitleBar class.
        /// </summary>
        public RadTitleBar()
        {
            menu = new ContextMenu();
            menu.Popup += new EventHandler(menu_Popup);

            MenuItem item = new MenuItem("Restore");
            item.Click += new EventHandler(item_RestoreClick);
            menu.MenuItems.Add(item);

            item = new MenuItem("Move");
            menu.MenuItems.Add(item);

            item = new MenuItem("Size");
            menu.MenuItems.Add(item);

            item = new MenuItem("Minimize");
            item.Click += new EventHandler(item_MinimizeClick);
            menu.MenuItems.Add(item);

            item = new MenuItem("Maximize");
            item.Click += new EventHandler(item_MaximizeClick);
            menu.MenuItems.Add(item);

            menu.MenuItems.Add(new MenuItem("-"));

            item = new MenuItem("Close    Alt+F4");
            item.Click += new EventHandler(item_CloseClick);
            menu.MenuItems.Add(item);

            this.ContextMenu = menu;
            this.TabStop = false;
            
            // PATCH - for double click in design-time
            Size sz = this.DefaultSize;
            this.ElementTree.PerformInnerLayout(true, 0, 0, sz.Width, sz.Height);
        }

        #region Properties

        /// <summary>
        /// Gets or sets the text associated with this item. 
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the text associated with this item.")]
        [Bindable(true)]
        [SettingsBindable(true)]
        public override string Text
        {
            get
            {
                return this.titleBarElement.Text;
            }
            set
            {
                this.titleBarElement.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that determines whether the title bar
        /// can manage the owner form.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Determines whether the parent form can be managed by the title bar.")]
        [DefaultValue(true)]
        public bool CanManageOwnerForm
        {
            get
            {
                return this.titleBarElement.CanManageOwnerForm;
            }
            set
            {
                this.titleBarElement.CanManageOwnerForm = value;
            }
        }

        /// <summary>
        /// Allow form's resize
        /// </summary>
        [Browsable(true), Category("Appearance")]
        [Description("Get or sets a value indicating whether the form can be resized")]
        [DefaultValue(true)]
        public bool AllowResize
        {
            get
            {
                return this.titleBarElement.AllowResize;
            }
            set
            {
                this.titleBarElement.AllowResize = value;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(220, 23);
            }
        }

        [Obsolete("Please, use the Text property instead.")]
        [Description("Get or sets a the caption of the titlebar")]
        [DefaultValue("")]
        [Localizable(true)]
        [Browsable(false)]
        public string Caption
        {
            get
            {
                return this.titleBarElement.Text;
            }
            set
            {
                this.titleBarElement.Text = value;
            }
        }

        /// <summary>
        /// An Icon that represents the icon for the form.
        /// </summary>
        [DefaultValue(null)]
        [Category("Window Style")]
        public Icon ImageIcon
        {
            get
            {
                return this.titleBarElement.ImageIcon;
            }
            set
            {
                this.titleBarElement.ImageIcon = value;
            }
        }

        /// <summary>
        /// Gets the instance of RadTitleBarElement wrapped by this control. RadTitleBarElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadTitleBar.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadTitleBarElement TitleBarElement
        {
            get
            {
                return this.titleBarElement;
            }
        }

        [DefaultValue((string)null), 
        Localizable(true), 
        Description("Background right image"), 
        Category("Appearance")]
        public Image RightImage
        {
            get
            {
                return this.titleBarElement.RightImage;
            }
            set
            {
                this.titleBarElement.RightImage = value;
            }
        }

        [DefaultValue((string)null), 
        Localizable(true), 
        Description("Background left image"), 
        Category("Appearance")]
        public Image LeftImage
        {
            get
            {
                return this.titleBarElement.LeftImage;
            }
            set
            {
                this.titleBarElement.LeftImage = value;
            }
        }

        #endregion

        #region Events

        public event TitleBarSystemEventHandler Close;
        
        /// <summary>
        /// Fires when a minimize action is performed by the user (the minimize button is
        /// pressed).
        /// </summary>
        public event TitleBarSystemEventHandler Minimize;
        
        /// <summary>
        /// Fires when a maximize/restore action is performed by the user (maximizes button
        /// is pressed or the title bar is double clicked).
        /// </summary>
        public event TitleBarSystemEventHandler MaximizeRestore;
        
        /// <summary>
        /// Fires when the minimize in the tray button is pressed. It is hidden by default.
        /// Use the Visual Style Builder to set which elements are visible and design their visual
        /// appearance.
        /// </summary>
        public event TitleBarSystemEventHandler MinimizeInTheTray;

        #endregion

        #region Event handlers

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            this.titleBarElement.Text = this.Text;
        }

        protected virtual void OnClose(object sender, EventArgs args)
        {
            if (Close != null)
            {
                Close(sender, args);
            }
            Form form = FindParentForm(this);
            if (form != null)
            {
                form.Close();	
            }
        }

        protected virtual void OnMinimize(object sender, EventArgs args)
        {
            Form form = FindParentForm(this);

            if (form != null && !form.MinimizeBox)
            {
                return;
            }

            if (Minimize != null)
            {
                Minimize(sender, args);
            }
            
            if (form != null)
            {
                form.WindowState = FormWindowState.Minimized;
            }
        }

        protected virtual void OnMaximizeRestore(object sender, EventArgs args)
        {
            Form form = FindParentForm(this);

            if (form != null && !form.MaximizeBox)
            {
                return;
            }

            if (MaximizeRestore != null)
            {
                MaximizeRestore(sender, args);
            }
            
            if (form != null)
            {
                if (form.WindowState == FormWindowState.Maximized)
                {
                    form.WindowState = FormWindowState.Normal;
                }
                else
                {
                    form.WindowState = FormWindowState.Maximized;
                }
            }
        }

        protected virtual void OnMinimizeInTheTray(object sender, EventArgs args)
        {
            if (MinimizeInTheTray != null)
            {
                MinimizeInTheTray(sender, args);
            }
        }

        protected override void OnBackgroundImageChanged(EventArgs e)
        {
            base.OnBackgroundImageChanged(e);
            this.TitleBarElement.MiddleImage = this.BackgroundImage;
        }
        protected override void OnParentChanged(EventArgs e)
        {
            if (this.associatedForm != null)
            {
                this.associatedForm.TextChanged -= OnParentForm_TextChanged;
                this.associatedForm.StyleChanged -= OnAssociatedForm_StyleChanged;
            }

            base.OnParentChanged(e);
            this.associatedForm = FindParentForm(this);
            if (this.associatedForm is Form)
            {
                this.associatedForm.TextChanged += OnParentForm_TextChanged;
                this.associatedForm.StyleChanged += OnAssociatedForm_StyleChanged;
                this.titleBarElement.Text = this.associatedForm.Text;
            }
        }

        private void OnAssociatedForm_StyleChanged(object sender, EventArgs e)
        {
            Form form = FindParentForm(this);
            if (form != null)
            {
                if (form.MaximizeBox)
                {
                    this.titleBarElement.MaximizeButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Visible);
                }
                else
                {
                    this.titleBarElement.MaximizeButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Collapsed);
                }

                if (form.MinimizeBox)
                {
                    this.titleBarElement.MinimizeButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Visible);
                }
                else
                {
                    this.titleBarElement.MinimizeButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Collapsed);
                }
            }
        }

        private void OnParentForm_TextChanged(object sender, EventArgs e)
        {
            if (this.associatedForm != null)
                this.titleBarElement.Text = this.associatedForm.Text;
        }

        private void item_RestoreClick(object sender, EventArgs e)
        {
            this.OnMaximizeRestore(sender, e);
        }

        private void item_MinimizeClick(object sender, EventArgs e)
        {
            this.OnMinimize(sender, e);
        }

        private void item_MaximizeClick(object sender, EventArgs e)
        {
            this.OnMaximizeRestore(sender, e);
        }

        private void item_CloseClick(object sender, EventArgs e)
        {
            this.OnClose(sender, e);
        }

        private void menu_Popup(object sender, EventArgs e)
        {
            Form form = FindParentForm(this);

            if (form != null)
            {
                bool maximized = form.WindowState == FormWindowState.Maximized;
                this.menu.MenuItems[0].Enabled = maximized;
                this.menu.MenuItems[1].Enabled = !maximized;
                this.menu.MenuItems[2].Enabled = !maximized;
                this.menu.MenuItems[4].Enabled = !maximized;
            }
        }

        #endregion

        #region Internals

        internal void CallOnClose(object sender, EventArgs args)
        {
            this.OnClose(sender, args);
        }

        internal void CallOnMinimize(object sender, EventArgs args)
        {
            this.OnMinimize(sender, args);
        }

        internal void CallOnMaximizeRestore(object sender, EventArgs args)
        {
            this.OnMaximizeRestore(sender, args);
        }

        internal void CallOnMinimizeInTheTray(object sender, EventArgs args)
        {
            this.OnMinimizeInTheTray(sender, args);
        }

        internal static Form FindParentForm(Control ctl)
        {
            if (ctl == null)
                return null;

            if (ctl.Parent != null && ctl.Parent is Form && !((Form)ctl.Parent).TopLevel)
                return (Form)ctl.Parent;

            Form form = ctl.FindForm();
			
            if (form != null && form.IsMdiChild)
                return form;

            if (ctl is IComponentTreeHandler)
                return ((IComponentTreeHandler)ctl).Behavior.FindFormInternal(ctl);
			
            return null;
        }

        #endregion

        protected override void CreateChildItems(RadElement parent)
        {
            this.titleBarElement = new RadTitleBarElement();
            this.titleBarElement.Class = "TitleBar";
            this.RootElement.Children.Add(this.titleBarElement);
            base.CreateChildItems(parent);
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadTitleBarAccessibleObject(this);
        }
    }
}