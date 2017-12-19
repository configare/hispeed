using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using Telerik.WinControls.Themes.ControlDefault;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class RadPageViewOutlookElement : RadPageViewStackElement
    {
        #region Events

        private static object ItemShownEventKey;
        private static object ItemCheckedEventKey;
        private static object ItemUncheckedEventKey;
        private static object ItemCollapsedEventKey;
        private static object ItemAssociatedButtonClickedEventKey;

        #endregion

        #region Fields

        private OutlookViewOverflowGrip gripElement;
        private OverflowItemsContainer itemsContainer;
        private List<RadPageViewOutlookItem> hiddenItems;
        private List<RadPageViewOutlookItem> uncheckedItems;

        #region Static

        public static Bitmap AssociatedButtonDefaultImage;

        #endregion

        #endregion

        #region Ctor

        static RadPageViewOutlookElement()
        {
            ItemShownEventKey = new object();
            ItemCollapsedEventKey = new object();
            ItemCheckedEventKey = new object();
            ItemUncheckedEventKey = new object();
            ItemAssociatedButtonClickedEventKey = new object();

            new ControlDefault_RadPageView_Telerik_WinControls_UI_RadPageViewOutlookElement().DeserializeTheme();
            AssociatedButtonDefaultImage = ResourceHelper.ImageFromResource(typeof(RadPageViewOutlookAssociatedButton), "Telerik.WinControls.UI.RadPageView.Views.Outlook.Resources.AssociatedButtonDefaultImage.png");
        }

        #endregion

        #region Properties

        #region CLR

        /// <summary>
        /// Gets the element that represents the container which holds
        /// the buttons shown when items in the stack are hidden by using
        /// the overflow grip.
        /// </summary>
        [Browsable(false)]
        public OverflowItemsContainer OverflowItemsContainer
        {
            get
            {
                return this.itemsContainer;
            }
        }

        /// <summary>
        /// Gets the element which represents the grip which can be dragged
        /// to adjust the count of visible items in the stack.
        /// </summary>
        [Browsable(false)]
        public OutlookViewOverflowGrip OverflowGrip
        {
            get
            {
                return this.gripElement;
            }
        }

        /// <summary>
        /// Gets or sets the image that is shown on the
        /// item in the overflow drop-down menu that is used to
        /// show more buttons in the control.
        /// </summary>
        [DefaultValue(null)]
        [Description("Gets or sets the image shown on the item in the overflow drop-down that is used to show more buttons in the control.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        [TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        public Image ShowMoreButtonsImage
        {
            get
            {
                return (Image)this.GetValue(ShowMoreButtonsImageProperty);
            }
            set
            {
                this.SetValue(ShowMoreButtonsImageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the image that is shown on the
        /// item in the overflow drop-down menu that is used to
        /// show fewer buttons in the control.
        /// </summary>
        [DefaultValue(null)]
        [Description("Gets or sets the image shown on the item in the overflow drop-down that is used to show fewer buttons in the control.")]
        [Category(RadDesignCategory.AppearanceCategory)]
        [TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        public Image ShowFewerButtonsImage
        {
            get
            {
                return (Image)this.GetValue(ShowFewerButtonsImageProperty);
            }
            set
            {
                this.SetValue(ShowFewerButtonsImageProperty, value);
            }
        }

        #endregion

        #region Internal

        /// <summary>
        /// Gets the collection containing the unchecked items.
        /// </summary>
        internal List<RadPageViewOutlookItem> UncheckedItems
        {
            get
            {
                return this.uncheckedItems;
            }
        }

        #endregion

        #region RadProperties

        public static RadProperty ShowFewerButtonsImageProperty = RadProperty.Register(
            "ShowFewerButtonsImage",
            typeof(Image),
            typeof(RadPageViewOutlookElement),
            new RadElementPropertyMetadata(null, ElementPropertyOptions.AffectsDisplay)
            );

        public static RadProperty ShowMoreButtonsImageProperty = RadProperty.Register(
            "ShowMoreButtonsImage",
            typeof(Image),
            typeof(RadPageViewOutlookElement),
            new RadElementPropertyMetadata(null, ElementPropertyOptions.AffectsDisplay)
            );

        #endregion

        #endregion

        #region RadObject Overrides

        protected override ValueUpdateResult SetValueCore(RadPropertyValue propVal, object propModifier, object newValue, ValueSource source)
        {
            //Do not allow the update of the following properties unless the value source is DefaultValueOverride.
            if (source != ValueSource.DefaultValueOverride && 
                (propVal.Property == RadPageViewStackElement.StackPositionProperty ||
                propVal.Property == RadPageViewStackElement.ItemSelectionModeProperty ||
                propVal.Property == RadPageViewElement.ItemContentOrientationProperty))
            {
                return ValueUpdateResult.Canceled;
            }
            return base.SetValueCore(propVal, propModifier, newValue, source);
        }

        #endregion

        #region Init

        protected override RadPageViewItem CreateItem()
        {
            return new RadPageViewOutlookItem();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            //this.visibleItemsCount = this.Items.Count;
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.hiddenItems = new List<RadPageViewOutlookItem>();
            this.uncheckedItems = new List<RadPageViewOutlookItem>();
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.ContentArea.ThemeRole = "OutlookViewContentArea";

            this.gripElement = new OutlookViewOverflowGrip();
            this.gripElement.MinSize = new System.Drawing.Size(0, 10);
            this.Children.Add(this.gripElement);

            this.itemsContainer = new OverflowItemsContainer(this);
            this.itemsContainer.MinSize = new Size(0, 35);
            this.Children.Add(this.itemsContainer);
            this.WireEvents();

            this.itemsContainer.ShowMoreButtonsItem.BindProperty(RadMenuItem.ImageProperty, this, ShowMoreButtonsImageProperty, PropertyBindingOptions.OneWay);
            this.itemsContainer.ShowFewerButtonsItem.BindProperty(RadMenuItem.ImageProperty, this, ShowFewerButtonsImageProperty, PropertyBindingOptions.OneWay);
        }

        private void WireEvents()
        {
            this.gripElement.Dragged += this.OverflowGripElement_Dragged;
        }

        private void UnwireEvents()
        {
            this.gripElement.Dragged -= this.OverflowGripElement_Dragged;
        }

        #endregion

        #region Layout

        protected override SizeF MeasureItems(SizeF availableSize)
        {
            SizeF elementAvailableSize = new SizeF(availableSize.Width -= this.gripElement.Margin.Horizontal, availableSize.Height);
            
            this.gripElement.Measure(elementAvailableSize);            
            elementAvailableSize.Height -= this.gripElement.DesiredSize.Height;

            this.itemsContainer.Measure(elementAvailableSize);
            elementAvailableSize.Height -= this.itemsContainer.DesiredSize.Height;

            SizeF desiredSize = base.MeasureItems(elementAvailableSize);
            desiredSize.Height = desiredSize.Height + gripElement.DesiredSize.Height + itemsContainer.DesiredSize.Height;

            return desiredSize;
        }

        protected override RectangleF ArrangeItems(System.Drawing.RectangleF clientRect)
        {
            this.itemsContainer.Arrange(
                new RectangleF(new PointF(
                    clientRect.Left + this.itemsContainer.Margin.Left, 
                    clientRect.Bottom - (this.itemsContainer.DesiredSize.Height + this.itemsContainer.Margin.Bottom)),
                    new SizeF(clientRect.Width - this.itemsContainer.Margin.Horizontal, this.itemsContainer.DesiredSize.Height)));

            clientRect.Height = 
                clientRect.Height - 
                (this.itemsContainer.DesiredSize.Height + this.itemsContainer.Margin.Vertical);

            RectangleF resultRect =  base.ArrangeItems(clientRect);
            System.Windows.Forms.Padding gripElementMargin = this.gripElement.Margin;
            this.gripElement.Arrange(new RectangleF(
                new PointF(
                    clientRect.Left + gripElementMargin.Left,
                    resultRect.Bottom + this.ContentArea.Margin.Bottom + gripElementMargin.Top),
                    new SizeF(clientRect.Width - this.gripElement.Margin.Horizontal, this.gripElement.DesiredSize.Height)));
            
            return resultRect;
        }

        protected override RectangleF GetContentAreaRectangle(RectangleF clientRect)
        {
            RectangleF rectangle =  base.GetContentAreaRectangle(clientRect);
            rectangle.Height -= this.gripElement.DesiredSize.Height + this.gripElement.Margin.Vertical;
            return rectangle;
        }

        #endregion

        #region Overflow logic

        /// <summary>
        /// Gets an array containing the items that are currently hidden by using the
        /// overflow grip.
        /// </summary>
        public RadPageViewOutlookItem[] GetHiddenItems()
        {
            return this.hiddenItems.ToArray();
        }

        /// <summary>
        /// Gets an array containing the items that are currently unchecked by using the
        /// overflow menu.
        /// </summary>
        public RadPageViewOutlookItem[] GetUncheckedItems()
        {
            return this.uncheckedItems.ToArray();
        }

        /// <summary>
        /// This method returns the count of the items which are currently 
        /// visible to the user.
        /// </summary>
        /// <returns></returns>
        protected virtual int GetCurrentlyVisibleItemsCount()
        {
            int count = 0;
            foreach (RadPageViewItem item in this.Items)
            {
                if (item.Visibility != ElementVisibility.Visible)
                {
                    continue;
                }
                count++;
            }

            return count;
        }

        /// <summary>
        /// Makes an item invisible. The item will appear as unchecked in the
        /// overflow menu. 
        /// </summary>
        /// <param name="item">The item to make invisible.</param>
        public virtual void UncheckItem(RadPageViewOutlookItem item)
        {
            if (this.uncheckedItems.Contains(item))
            {
                return;
            }
            this.uncheckedItems.Add(item);
            item.Visibility = ElementVisibility.Collapsed;
            OutlookViewItemEventArgs args = new OutlookViewItemEventArgs(item);
            this.OnItemUnchecked(this, args);

            //Show the first possible hidden item in case we uncheck
            //an item that is among the visible items.
            if (!this.hiddenItems.Contains(item)
                && this.GetCurrentlyVisibleItemsCount() <= this.layoutInfo.itemCount)
            {
                this.ShowFirstPossibleItem();
            }
        }

        /// <summary>
        /// Makes an item visible. The item will appear as checked in the
        /// overflow menu.
        /// </summary>
        /// <param name="item">The item to make visible.</param>
        public virtual void CheckItem(RadPageViewOutlookItem item)
        {
            if (!this.uncheckedItems.Contains(item))
            {
                return;
            }
            this.uncheckedItems.Remove(item);
            OutlookViewItemEventArgs args = new OutlookViewItemEventArgs(item);
            this.OnItemChecked(this, args);

            if (this.hiddenItems.Contains(item))
            {
                return;
            }

            item.Visibility = ElementVisibility.Visible;

            //Hide the first possible visible item in case we check
            //an item that was initially visible.
            if (this.GetCurrentlyVisibleItemsCount() > this.layoutInfo.itemCount)
            {
                this.HideFirstPossibleItem();
            }
        }

        /// <summary>
        /// Drags the overflow grip down to hide the first possible visible item.
        /// </summary>
        /// <returns>True if the drag operation succeeds, otherwise false.</returns>
        public bool DragGripDown()
        {
            if (this.HideFirstPossibleItem())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Drags the overflow grip up to show the first possible hidden item.
        /// </summary>
        /// <returns>True if the drag operation succeeds, otherwise false.</returns>
        public bool DragGripUp()
        {
            if (this.ShowFirstPossibleItem())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Shows a given amount of items from the hidden items
        /// in the <see cref="RadPageViewOutlookElement"/> starting from the
        /// bottom part of the stack.
        /// </summary>
        /// <param name="itemCount">The count of the items to be shown.</param>
        public void ShowItems(int itemCount)
        {
            bool itemShown = true;

            while (itemCount > 0 && itemShown)
            {
                itemCount--;
                itemShown = this.DragGripUp();
            }
        }

      /// <summary>
        /// Hides a given amount of items from the visible items
        /// in the <see cref="RadPageViewOutlookElement"/> starting from the
        /// bottom part of the stack.
        /// </summary>
        /// <param name="itemCount">The count of the items to be hidden.</param>
        public void HideItems(int itemCount)
        {
            bool itemHidden = true;

            while (itemCount > 0 && itemHidden)
            {
                itemCount--;
                itemHidden = this.DragGripDown();
            }
        }

        internal virtual bool ShowFirstPossibleItem()
        {
            if (this.hiddenItems.Count == 0)
            {
                return false;
            }

            RadPageViewOutlookItem item = null;

            for (int i = this.hiddenItems.Count - 1; i > -1; i--)
            {
                item = this.hiddenItems[i];

                if (this.uncheckedItems.Contains(item))
                {
                    item = null;
                    continue;
                }
                else
                {
                    break;
                }
            }

            if (item == null)
            {
                return false;
            }

            this.hiddenItems.Remove(item);

            item.Visibility = ElementVisibility.Visible;
            this.itemsContainer.UnregisterCollapsedItem(item);
            OutlookViewItemEventArgs args = new OutlookViewItemEventArgs(item);
            this.OnItemShown(this, args);
            return true;
        }

        internal virtual bool HideFirstPossibleItem()
        {
            RadPageViewOutlookItem item = this.GetNextVisibleItem();

            if (item == null || this.hiddenItems.Contains(item))
            {
                return false;
            }
           
            item.Visibility = ElementVisibility.Collapsed;
            this.hiddenItems.Add(item);
            this.itemsContainer.RegisterCollapsedItem(item);
            OutlookViewItemEventArgs args = new OutlookViewItemEventArgs(item);
            this.OnItemCollapsed(this, args);
            return true;
        }

        protected virtual RadPageViewOutlookItem GetNextVisibleItem()
        {
            for (int i = this.Items.Count - 1; i > -1; i--)
            {
                RadPageViewOutlookItem item = this.Items[i] as RadPageViewOutlookItem;
                if (item.Visibility == ElementVisibility.Visible)
                    return item;
            }
            return null;
        }

        #endregion

        #region Event handling

        protected override void RemoveItemCore(RadPageViewItem item)
        {
            RadPageViewOutlookItem outlookItem = item as RadPageViewOutlookItem;

            if (outlookItem.AssociatedOverflowButton != null)
            {
                outlookItem.AssociatedOverflowButton.Dispose();
            }

            this.hiddenItems.Remove(outlookItem);
            this.uncheckedItems.Remove(outlookItem);

            base.RemoveItemCore(item);
        }

        private void OverflowGripElement_Dragged(object sender, OverflowEventArgs args)
        {
            if (args.Up)
            {
                this.DragGripUp();
            }
            else
            {
                this.DragGripDown();
            }
        }

        #endregion

        #region Cleanup

        protected override void DisposeManagedResources()
        {
            this.UnwireEvents();
            base.DisposeManagedResources();
        }

        #endregion

        #region Events

        protected internal virtual void OnItemAssociatedButtonClick(object sender, EventArgs args)
        {
            EventHandler handler = this.Events[ItemAssociatedButtonClickedEventKey] as EventHandler;

            if (handler != null)
            {
                handler(sender, args);
            }
        }

        protected virtual void OnItemChecked(object sender, OutlookViewItemEventArgs args)
        {
            OutlookViewItemEventHandler handler = this.Events[ItemCheckedEventKey] as OutlookViewItemEventHandler;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        protected virtual void OnItemUnchecked(object sender, OutlookViewItemEventArgs args)
        {
            OutlookViewItemEventHandler handler = this.Events[ItemUncheckedEventKey] as OutlookViewItemEventHandler;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        protected virtual void OnItemShown(object sender, OutlookViewItemEventArgs args)
        {
            OutlookViewItemEventHandler handler = this.Events[ItemShownEventKey] as OutlookViewItemEventHandler;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        protected virtual void OnItemCollapsed(object sender, OutlookViewItemEventArgs args)
        {
            OutlookViewItemEventHandler handler = this.Events[ItemCollapsedEventKey] as OutlookViewItemEventHandler;
            if (handler != null)
            {
                handler(sender, args);
            }
        }

        /// <summary>
        /// Fires when the user clicks on a button associated with a <see cref="RadPageViewItem"/> instance.
        /// This buttons is shown when the item is collapsed by using the overflow grip.
        /// </summary>
        public event EventHandler ItemAssociatedButtonClicked
        {
            add
            {
                this.Events.AddHandler(ItemAssociatedButtonClickedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(ItemAssociatedButtonClickedEventKey, value);
            }
        }

        /// <summary>
        /// Fires when an item is shown in the <see cref="RadPageViewOutlookElement"/>.
        /// </summary>
        public event OutlookViewItemEventHandler ItemShown
        {
            add
            {
                this.Events.AddHandler(ItemShownEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(ItemShownEventKey, value);
            }
        }

        /// <summary>
        /// Fires when an item is collapsed in the <see cref="RadPageViewOutlookElement"/>.
        /// </summary>
        public event OutlookViewItemEventHandler ItemCollapsed
        {
            add
            {
                this.Events.AddHandler(ItemCollapsedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(ItemCollapsedEventKey, value);
            }
        }

        /// <summary>
        /// Fires when an item is checked in the overflow drop-down menu of the <see cref="RadPageViewOutlookElement"/>.
        /// </summary>
        public event OutlookViewItemEventHandler ItemChecked
        {
            add
            {
                this.Events.AddHandler(ItemCheckedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(ItemCheckedEventKey, value);
            }
        }

        /// <summary>
        /// Fires when an item is unchecked in the overflow drop-down menu of the <see cref="RadPageViewOutlookElement"/>.
        /// </summary>
        public event OutlookViewItemEventHandler ItemUnchecked
        {
            add
            {
                this.Events.AddHandler(ItemUncheckedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(ItemUncheckedEventKey, value);
            }
        }

        #endregion
    }
}
