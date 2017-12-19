using System.Drawing;
using System.Windows.Forms;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Styles;
using System;
using System.ComponentModel;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Layout;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represent the overflow button at the end of each strip
    /// </summary>
    public class RadCommandBarOverflowButton : RadCommandBarVisualElement
    {
        public static RadProperty HasOverflowedItemsProperty = RadProperty.Register("HasOverflowedItems", typeof(bool), typeof(RadCommandBarOverflowButton),
                                                                                    new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsDisplay));

        #region Fields

        protected RadDropDownMenu dropDownMenuElement;
        protected RadMenuItem addRemoveButtonsMenuItem;
        protected RadMenuItem customizeButtonMenuItem;
        protected RadCommandBarOverflowPanelElement panel;


        protected CommandBarStripElement owner;
        protected ArrowPrimitive arrowPrimitive;
        protected LayoutPanel layout;
        protected CommandBarCustomizeDialogProvider dialogProvider;

        private bool cachedHasOverflowedItems = false;

        #endregion

        #region Overrides

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.DrawBorder = true;
            this.DrawFill = true;
            this.MinSize = new Size(11, 25);
        }

        protected override void OnMouseDown(System.Windows.Forms.MouseEventArgs e)
        {
            if (!this.OnOverflowMenuOpening(new CancelEventArgs()))
            {
                base.OnMouseDown(e);
                this.ShowOverflowMenu();
                this.OnOverflowMenuOpened(new EventArgs());
            }
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            return new SizeF(11f, 25f);
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e);
            this.SetValue(IsMouseDownProperty, false);
            this.Invalidate();
        }

        #endregion

        #region Properties

        public CommandBarCustomizeDialogProvider DialogProvider
        {
            get
            {
                return this.dialogProvider;
            }
            set
            {
                this.dialogProvider = value;
            }
        }

        /// <summary>
        /// Gets the "Add or Remove Items" menu item from overflow menu
        /// </summary>
        public RadMenuItem AddRemoveButtonsMenuItem
        {
            get
            {
                return addRemoveButtonsMenuItem;
            }
        }

        /// <summary>
        /// Gets the menu item from overflow menu which opens the Customize Dialog
        /// </summary>
        public RadMenuItem CustomizeButtonMenuItem
        {
            get
            {
                return customizeButtonMenuItem;
            }
        }

        /// <summary>
        /// Gets the overflow panel which contains the overflowed items
        /// </summary>
        public RadCommandBarOverflowPanelElement OverflowPanel
        {
            get
            {
                return panel;
            }
        }


        /// <summary>
        /// Gets the RadDropDownMenu that is shown on click.
        /// </summary>
        public RadDropDownMenu DropDownMenu
        {
            get
            {
                return dropDownMenuElement;
            }
        }

        /// <summary>
        /// Gets whether there are items in the overflow panel.
        /// </summary>
        public bool HasOverflowedItems
        {
            get
            {
                return this.cachedHasOverflowedItems;
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the overflow button.
        /// </summary>
        public override Orientation Orientation
        {
            get
            {
                return this.orientation;
            }
            set
            {
                this.orientation = value;
                this.AngleTransform = (value == Orientation.Horizontal) ? 0f : 90f;
            }
        }

        /// <summary>
        /// Gets or sets the dropdown menu element theme name.
        /// </summary>
        public string HostControlThemeName
        {
            get
            {
                return this.dropDownMenuElement.ThemeName;
            }
            set
            {
                //this.hostControl.ThemeName = value;
                this.dropDownMenuElement.ThemeName = value;
            }
        }

        /// <summary>
        /// Gets or sets the panel in which overflowed items are arranged.
        /// </summary>
        public LayoutPanel ItemsLayout
        {
            get
            {
                return layout;
            }
        }

        /// <summary>
        /// Gets or sets the ArrowPrimitive element of the button.
        /// </summary>
        public ArrowPrimitive ArrowPrimitive
        {
            get
            {
                return arrowPrimitive;
            }
            set
            {
                arrowPrimitive = value;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// This event fires before oferflow menu is opened.
        /// </summary>
        public event CancelEventHandler OverflowMenuOpening;

        /// <summary>
        /// This event fires when overflow menu is opened.
        /// </summary>
        public event EventHandler OverflowMenuOpened;

        /// <summary>
        /// This event fires before oferflow menu is opened.
        /// </summary>
        public event CancelEventHandler OverflowMenuClosing;

        /// <summary>
        /// This event fires when overflow menu is opened.
        /// </summary>
        public event EventHandler OverflowMenuClosed;

        #endregion

        #region Events Management

        /// <summary>
        /// Raises the <see cref="E:RadCommandBarOverflowButton.OverflowMenuOpened"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected void OnOverflowMenuOpened(EventArgs e)
        {
            this.SetValue(RadDropDownButtonElement.IsDropDownShownProperty, true);

            if (this.OverflowMenuOpened != null)
            {
                this.OverflowMenuOpened(owner, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:RadCommandBarOverflowButton.OverflowMenuOpening"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the
        /// event data.</param>
        protected bool OnOverflowMenuOpening(CancelEventArgs e)
        {
            if (this.OverflowMenuOpening != null)
            {
                this.OverflowMenuOpening(owner, e);
                return e.Cancel;
            }
            return false;
        }

        /// <summary>
        /// Raises the  <see cref="E:RadCommandBarOverflowButton.OverflowMenuClosed"/> event.
        /// </summary>
        /// <param name="sender">The element that is reponsible for firing the event.</param>
        /// <param name="args">A <see cref="RadPopupClosedEventArgs"/> that contains the
        /// event data.</param> 
        protected void OnOverflowMenuClosed(object sender, RadPopupClosedEventArgs args)
        {
            this.SetValue(RadDropDownButtonElement.IsDropDownShownProperty, false);

            if (this.OverflowMenuClosed != null)
            {
                this.OverflowMenuClosed(sender, args);
            }
        }

        /// <summary>
        /// Raises the  <see cref="E:RadCommandBarOverflowButton.OverflowMenuClosing"/> event.
        /// </summary>
        /// <param name="sender">The element that is reponsible for firing the event.</param>
        /// <param name="args">A <see cref="RadPopupClosingEventArgs"/> that contains the
        /// event data.</param> 
        protected virtual void OnOverflowMenuClosing(object sender, RadPopupClosingEventArgs args)
        {
            if (args.Cancel)
            {
                return;
            }

            if (this.OverflowMenuClosing != null)
            {
                CancelEventArgs e = new CancelEventArgs();
                this.OverflowMenuClosing(this, e);
                args.Cancel = e.Cancel;
            }
        }

        #endregion

        #region Ctors

        static RadCommandBarOverflowButton()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new RadCommandBarOverflowButtonStateManagerFactory(), typeof(RadCommandBarOverflowButton));
        }

        public RadCommandBarOverflowButton(CommandBarStripElement owner)
        {
            this.dialogProvider = new CommandBarCustomizeDialogProvider();

            this.owner = owner;
            this.dropDownMenuElement = new RadDropDownMenu();
            this.dropDownMenuElement.MinimumSize = this.owner.OverflowMenuMinSize;
            this.dropDownMenuElement.MaximumSize = this.owner.OverflowMenuMaxSize;
            this.dropDownMenuElement.RightToLeft = this.RightToLeft ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;

            panel = new RadCommandBarOverflowPanelElement();
            RadMenuSeparatorItem separator = new RadMenuSeparatorItem();
            panel.Visibility = ElementVisibility.Collapsed;
            separator.Visibility = ElementVisibility.Collapsed;
            this.dropDownMenuElement.Items.Add(panel);
            this.dropDownMenuElement.Items.Add(separator);

            addRemoveButtonsMenuItem = new RadMenuItem(CommandBarLocalizationProvider.CurrentProvider.GetLocalizedString(CommandBarStringId.OverflowMenuAddOrRemoveButtonsText));

            this.dropDownMenuElement.Items.Add(addRemoveButtonsMenuItem);

            this.dropDownMenuElement.Items.Add(new RadMenuSeparatorItem());

            this.customizeButtonMenuItem = new RadMenuItem(CommandBarLocalizationProvider.CurrentProvider.GetLocalizedString(CommandBarStringId.OverflowMenuCustomizeText));


            this.dropDownMenuElement.Items.Add(customizeButtonMenuItem);

            this.layout = panel.Layout;
            this.layout.MaxSize = this.owner.OverflowMenuMaxSize;

            WireEvents();
        }

        protected void customizeButton_Click(object sender, EventArgs e)
        {
            RadControl senderControl = null;
            if(this.ElementTree != null )
            {
                senderControl = this.ElementTree.Control as RadControl;
            }

            if (this.owner.FloatingForm != null && !this.owner.FloatingForm.IsDisposed)
            {
                senderControl = this.owner.FloatingForm.ItemsHostControl;
                CommandBarCustomizeDialogProvider.CurrentProvider.ShowCustomizeDialog(this.owner, this.owner.FloatingForm.StripInfoHolder);
            }
            else
            {
                RadCommandBar commandBar = null;
                if (senderControl != null)
                {
                    commandBar = senderControl as RadCommandBar;
                }

                if (commandBar != null)
                {
                    CommandBarCustomizeDialogProvider.CurrentProvider.ShowCustomizeDialog(this.owner, commandBar.CommandBarElement.StripInfoHolder);
                }
            }
        }

        void wrapLayout_ChildrenChanged(object sender, ChildrenChangedEventArgs e)
        {
            if (e.ChangeOperation == ItemsChangeOperation.Inserted || e.ChangeOperation == ItemsChangeOperation.Removed
                || e.ChangeOperation == ItemsChangeOperation.Cleared)
            {
                this.cachedHasOverflowedItems = (layout.Children.Count > 0);
                this.SetValue(HasOverflowedItemsProperty, layout.Children.Count > 0);
            }
        }

        private void WireEvents()
        {
            this.dropDownMenuElement.PopupClosed += new RadPopupClosedEventHandler(OnOverflowMenuClosed);
            this.dropDownMenuElement.PopupClosing += new RadPopupClosingEventHandler(OnOverflowMenuClosing);
            this.customizeButtonMenuItem.Click += customizeButton_Click;
            this.layout.ChildrenChanged += new ChildrenChangedEventHandler(wrapLayout_ChildrenChanged);
            this.addRemoveButtonsMenuItem.DropDownClosing += dropDownMenuElement_DropDownClosing;
            CommandBarLocalizationProvider.CurrentProviderChanged += new EventHandler(CommandBarLocalizationProvider_CurrentProviderChanged);
        }

        private void UnwireEvents()
        {
            this.dropDownMenuElement.PopupClosed -= new RadPopupClosedEventHandler(OnOverflowMenuClosed);
            this.dropDownMenuElement.PopupClosing -= new RadPopupClosingEventHandler(OnOverflowMenuClosing);
            this.customizeButtonMenuItem.Click -= customizeButton_Click;
            this.layout.ChildrenChanged -= new ChildrenChangedEventHandler(wrapLayout_ChildrenChanged);
            this.addRemoveButtonsMenuItem.DropDownClosing -= dropDownMenuElement_DropDownClosing;
            CommandBarLocalizationProvider.CurrentProviderChanged -= new EventHandler(CommandBarLocalizationProvider_CurrentProviderChanged);
        }

        protected override void DisposeManagedResources()
        {
            this.UnwireEvents();
            base.DisposeManagedResources();
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == RadCommandBarOverflowButton.HasOverflowedItemsProperty)
            {
                if (this.HasOverflowedItems)
                {
                    this.dropDownMenuElement.Items[0].Visibility = ElementVisibility.Visible;
                    this.dropDownMenuElement.Items[1].Visibility = ElementVisibility.Visible;
                }
                else
                {
                    this.dropDownMenuElement.Items[0].Visibility = ElementVisibility.Collapsed;
                    this.dropDownMenuElement.Items[1].Visibility = ElementVisibility.Collapsed;
                }
            }
            if (e.Property == RadElement.RightToLeftProperty)
            {
                this.dropDownMenuElement.RightToLeft = this.RightToLeft ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
            }
        }
        #endregion

        #region Methods

        public void PopulateDropDownMenu()
        {
            addRemoveButtonsMenuItem.Items.Clear();
            int itemCount = this.owner.Items.Count;

            for (int i = 0; i < itemCount; ++i)
            {
                RadCommandBarBaseItem itemBase = this.owner.Items[i];
                if (!itemBase.VisibleInOverflowMenu)
                {
                    continue;
                }

                RadCommandBarOverflowMenuItem subMenuItem = new RadCommandBarOverflowMenuItem(itemBase, this.dropDownMenuElement);
                addRemoveButtonsMenuItem.Items.Add(subMenuItem);
            }

            int overflowedItemsCount = this.layout.Children.Count;
            for (int i = 0; i < overflowedItemsCount; ++i)
            {
                RadCommandBarBaseItem item = (this.layout.Children[i] as RadCommandBarBaseItem);
                if (item != null)
                {
                    if (!item.VisibleInOverflowMenu)
                    {
                        continue;
                    }

                    RadCommandBarOverflowMenuItem subMenuItem = new RadCommandBarOverflowMenuItem(item, this.dropDownMenuElement);
                    addRemoveButtonsMenuItem.Items.Add(subMenuItem);
                }
            }

            addRemoveButtonsMenuItem.MinSize = this.owner.OverflowMenuMinSize;
            addRemoveButtonsMenuItem.MaxSize = this.owner.OverflowMenuMaxSize;
            LightVisualElement lightVisualItem = (LightVisualElement)this.dropDownMenuElement.Items[0];
            this.SetVisualStyles(lightVisualItem);
            this.dropDownMenuElement.Items[0].InvalidateMeasure(true);
            this.dropDownMenuElement.Items[0].PerformLayout();
            this.dropDownMenuElement.LoadElementTree();
        }


        private void SetVisualStyles(LightVisualElement item)
        {
            item.BackColor = owner.BackColor;
            item.BackColor2 = owner.BackColor2;
            item.BackColor3 = owner.BackColor3;
            item.BackColor4 = owner.BackColor4;
            item.NumberOfColors = owner.NumberOfColors;
            item.BorderColor = owner.BorderColor;
            item.BorderColor2 = owner.BorderColor2;
            item.BorderColor3 = owner.BorderColor3;
            item.BorderColor4 = owner.BorderColor4;
            item.DrawFill = owner.DrawFill;
            item.DrawBorder = owner.DrawBorder;
        }

        private void ShowOverflowMenu()
        {
            this.PopulateDropDownMenu();

            Point p = new Point(0, this.Size.Height);
            if (this.orientation == System.Windows.Forms.Orientation.Vertical)
            {
                p = new Point(this.Size.Width, this.Size.Height);
            }
            else if (this.RightToLeft)
            {
                p = new Point(this.Size.Width - this.dropDownMenuElement.PreferredSize.Width, this.Size.Height);
            }
            this.dropDownMenuElement.Show(this, p);
        }

        #endregion

        #region Event Handlers

        void CommandBarLocalizationProvider_CurrentProviderChanged(object sender, EventArgs e)
        {
            this.addRemoveButtonsMenuItem.Text = CommandBarLocalizationProvider.CurrentProvider.GetLocalizedString(CommandBarStringId.OverflowMenuAddOrRemoveButtonsText);
            this.customizeButtonMenuItem.Text = CommandBarLocalizationProvider.CurrentProvider.GetLocalizedString(CommandBarStringId.OverflowMenuCustomizeText);
        }

        void dropDownMenuElement_DropDownClosing(object sender, RadPopupClosingEventArgs args)
        {
            RadItemCollection items = ((RadMenuItem)this.dropDownMenuElement.Items.Last).Items;
            for (int i = 0; i < items.Count; ++i)
            {
                if (items[i].ContainsMouse)
                {
                    args.Cancel = true;
                }
            }
        }
        #endregion

        #region Nested classes

        class RadCommandBarOverflowButtonStateManagerFactory : ItemStateManagerFactory
        {
            protected override StateNodeBase CreateSpecificStates()
            {
                StateNodeWithCondition overflowedState = new StateNodeWithCondition("HasOverflowedItems", new SimpleCondition(RadCommandBarOverflowButton.HasOverflowedItemsProperty, true));
                StateNodeWithCondition dropDownOpenedState = new StateNodeWithCondition("IsDropDownShown", new SimpleCondition(RadDropDownButtonElement.IsDropDownShownProperty, true));
                CompositeStateNode all = new CompositeStateNode("command bar overflow button states");
                all.AddState(overflowedState);
                all.AddState(dropDownOpenedState);

                return all;
            }

            protected override ItemStateManagerBase CreateStateManager()
            {
                ItemStateManagerBase sm = base.CreateStateManager();

                sm.AddDefaultVisibleState("HasOverflowedItems");
                sm.AddDefaultVisibleState("IsDropDownShown");

                return sm;
            }
        }

        #endregion
    }
}
