using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;
using System.Drawing.Drawing2D;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// <para>
    ///     Represents a BackstageView control - the Office 2010 replacement of ApplicationMenu.
    /// </para>
    /// <para>
    ///     It can contain tabs, pages, buttons and all other RadItems as well.
    /// </para>
    /// </summary>
    [Designer(DesignerConsts.RadRibbonBarBackstageViewDesignerString)]
    [ToolboxItem(false)]
    public class RadRibbonBarBackstageView : RadControl
    {
        #region Fields

        protected BackstageViewElement backstageElement;
        protected RadRibbonBarElement owner;
        private bool isShown = false;

        #endregion

        #region Event keys

        public static readonly object BackstageViewClosedEventKey = new object();
        public static readonly object BackstageViewClosingEventKey = new object();
        public static readonly object BackstageViewOpenedEventKey = new object();
        public static readonly object BackstageViewOpeningEventKey = new object();

        #endregion

        #region Events

        /// <summary>
        /// Fires when the backstage view is closed.
        /// </summary>
        public event EventHandler BackstageViewClosed
        {
            add
            {
                this.Events.AddHandler(BackstageViewClosedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(BackstageViewClosedEventKey, value);
            }
        }

        /// <summary>
        /// Fires when the backstage view is about to close.
        /// </summary>
        public event CancelEventHandler BackstageViewClosing
        {
            add
            {
                this.Events.AddHandler(BackstageViewClosingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(BackstageViewClosingEventKey, value);
            }
        }

        /// <summary>
        /// Fires when the backstage view is opened.
        /// </summary>
        public event EventHandler BackstageViewOpened
        {
            add
            {
                this.Events.AddHandler(BackstageViewOpenedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(BackstageViewOpenedEventKey, value);
            }
        }

        /// <summary>
        /// Fires when the backstage view is about to open.
        /// </summary>
        public event CancelEventHandler BackstageViewOpening
        {
            add
            {
                this.Events.AddHandler(BackstageViewOpeningEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(BackstageViewOpeningEventKey, value);
            }
        }

        /// <summary>
        /// Fires when an item from the items panel is clicked.
        /// </summary>
        public event EventHandler<BackstageItemEventArgs> ItemClicked
        {
            add { this.backstageElement.ItemClicked += value; }
            remove { this.backstageElement.ItemClicked -= value; }
        }

        /// <summary>
        /// Fires when the selected tab is about to change.
        /// </summary>
        public event EventHandler<BackstageItemChangingEventArgs> SelectedItemChanging
        {
            add { this.backstageElement.SelectedItemChanging += value; }
            remove { this.backstageElement.SelectedItemChanging -= value; }
        }

        /// <summary>
        /// Fires when the selected tab is changed.
        /// </summary>
        public event EventHandler<BackstageItemChangeEventArgs> SelectedItemChanged
        {
            add { this.backstageElement.SelectedItemChanged += value; }
            remove { this.backstageElement.SelectedItemChanged -= value; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the selected tab.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Description("Gets or sets the selected tab.")]
        public BackstageTabItem SelectedItem
        {
            get
            {
                return backstageElement.SelectedItem;
            }
            set
            {
                backstageElement.SelectedItem = value;
            }
        }

        [DefaultValue(false)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
            }
        }

        /// <summary>
        /// Indicates whether the backstage view is opened.
        /// </summary>
        [Browsable(false)]
        [Description("Indicates whether the backstage view is opened.")]
        public bool IsShown
        {
            get
            {
                return isShown;
            }
        }

        /// <summary>
        /// Gets the backstage element.
        /// </summary>
        [Browsable(false)]
        [Description("Gets the backstage element")]
        public BackstageViewElement BackstageElement
        {
            get
            {
                return backstageElement;
            }
        }
        
        [Editor(DesignerConsts.RadRibbonBarBackstageItemsCollectionEditorString, typeof(UITypeEditor))]
        [Category(RadDesignCategory.DataCategory)]
        [Description("Gets a collection representing the items contained in this backstage's items panel.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RadItemOwnerCollection Items
        {
            get
            {
                return this.backstageElement.Items;
            }
        }

        /// <summary>
        /// Gets the RadRibbonBar element that the backstage view is attached to.
        /// </summary>
        [Browsable(false)]
        public RadRibbonBarElement Owner
        {
            get
            {
                return owner;
            }
            internal set
            {
                owner = value;
            }
        }

        #endregion

        #region Initialization

        protected override void CreateChildItems(RadElement parent)
        {
            backstageElement = new BackstageViewElement();
            this.RootElement.Children.Add(backstageElement);

            base.CreateChildItems(parent);
            this.Visible = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the backstage view mimicking popup.
        /// </summary>
        /// <param name="location">The location on which the backstage will be shown.</param>
        /// <param name="owner">The RadRibbonBarElement that the backstage view is attached to.</param>
        public void ShowPopup(Point location, RadRibbonBarElement owner)
        {
            this.owner = owner;
             
            Control newParent = owner.ElementTree.Control.Parent;

            if (this.OnBackstageViewOpening())
            {
                return;
            }

            if (owner != null)
            {
                owner.ExpandButton.Enabled = false;
                if (owner.Popup != null)
                {
                    owner.Popup.ClosePopup(RadPopupCloseReason.AppFocusChange);
                }
            }

            if (this.Parent != newParent)
            {
                if (this.Parent != null)
                {
                    this.Parent.Controls.Remove(this);
                }

                if (newParent != null)
                {
                    newParent.Controls.Add(this);
                }
            }

            this.Location = location;

            if (this.Parent != null)
            {
                parentControl_SizeChanged(this.Parent, EventArgs.Empty);

                this.Parent.SizeChanged += new EventHandler(parentControl_SizeChanged);
            }

            this.Visible = true;
            this.BringToFront();
            this.isShown = true;
            this.Focus();
            this.OnBackstageViewOpened();
        }

        /// <summary>
        /// Hides the backstage view.
        /// </summary>
        public void HidePopup()
        {
            if (this.OnBackstageViewClosing())
            {
                return;
            }

            if (this.owner != null)
            {
                owner.ExpandButton.Enabled = true;
            }

            if (this.Parent != null)
            {
                this.Parent.SizeChanged -= new EventHandler(parentControl_SizeChanged);
                this.Visible = false;
                this.SendToBack();
                this.isShown = false;
            }

            this.OnBackstageViewClosed();
        }

        #endregion

        #region Events Management

        /// <summary>
        /// Raises the BackstageViewClosed event.
        /// </summary>
        protected virtual void OnBackstageViewClosed()
        {
            if (this.owner != null)
            {
                this.owner.ApplicationButtonElement.SetValue(RadDropDownButtonElement.IsDropDownShownProperty, false);
            }
            
            EventHandler handler = (EventHandler)this.Events[BackstageViewClosedEventKey];

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the BackstageViewClosing event.
        /// </summary>
        protected virtual bool OnBackstageViewClosing()
        {
            CancelEventHandler handler = (CancelEventHandler)this.Events[BackstageViewClosingEventKey];

            if (handler != null)
            {
                CancelEventArgs args = new CancelEventArgs();
                handler(this, args);
                return args.Cancel;
            }

            return false;
        }

        /// <summary>
        /// Raises the BackstageViewOpened event.
        /// </summary>
        protected virtual void OnBackstageViewOpened()
        {
            if (this.owner != null)
            {
                this.owner.ApplicationButtonElement.SetValue(RadDropDownButtonElement.IsDropDownShownProperty, true);
            }

            if (this.SelectedItem == null || !this.Items.Contains(this.SelectedItem))
            {
                foreach (RadItem item in this.Items)
                {
                    BackstageTabItem tabItem = item as BackstageTabItem;
                    if(tabItem!=null)
                    {
                        this.SelectedItem = tabItem;
                        break;
                    }
                }
            }

            EventHandler handler = (EventHandler)this.Events[BackstageViewOpenedEventKey];

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the BackstageViewOpening event.
        /// </summary>
        protected virtual bool OnBackstageViewOpening()
        {
            CancelEventHandler handler = (CancelEventHandler)this.Events[BackstageViewOpeningEventKey];

            if (handler != null)
            {
                CancelEventArgs args = new CancelEventArgs();
                handler(this, args);
                return args.Cancel;
            }

            return false;
        }

        #endregion

        #region Event Handlers

        protected virtual void parentControl_SizeChanged(object sender, EventArgs e)
        {
            Size newSize = (sender as Control).ClientRectangle.Size;
            newSize.Width -= this.Location.X;
            newSize.Height -= this.Location.Y;

            this.Size = newSize;
        }

        #endregion

        #region Overrides

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            this.BackstageElement.ProcessKeyboardSelection(e.KeyCode);

            if (e.KeyData == Keys.Escape)
            {
                this.HidePopup();
            }
        }
         
        protected override bool IsInputKey(Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;
            if (keyCode == Keys.Left || keyCode == Keys.Right || keyCode == Keys.Up || keyCode == Keys.Down)
            {
                return true;
            }

            return base.IsInputKey(keyData);
        }

        #endregion
    }
}
