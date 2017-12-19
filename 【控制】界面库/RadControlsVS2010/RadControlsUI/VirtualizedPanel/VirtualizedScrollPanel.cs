using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using Telerik.WinControls.Data;
using System.Drawing;
using System.Collections;
using System.Reflection;

namespace Telerik.WinControls.UI
{
    public class VirtualizedScrollPanel<Item, Element> :
        ScrollViewElement<VirtualizedStackContainer<Item>>
            where Element: IVirtualizedElement<Item>, new()
            where Item: class
    {
        IList<Item> items;
        ItemScroller<Item> scroller;
        IVirtualizedElementProvider<Item> elementProvider = null;
        bool autoSizeItems;

        #region Initialization & disposal

        protected override void CreateChildElements()
        {
            this.elementProvider = this.CreateElementProvider();
            this.scroller = this.CreateItemScroller();

            base.CreateChildElements();

            this.InitializeItemScroller(this.scroller);

            this.HScrollBar.Maximum = 0;

            this.WireEvents();
        }

        /// <summary>
        /// Performs events subscription to internal objects.
        /// The base implementation must always be called.
        /// </summary>
        protected virtual void WireEvents()
        {
            this.scroller.ScrollerUpdated += new EventHandler(scroller_ScrollerUpdated);
            this.HScrollBar.ValueChanged += new EventHandler(HScrollBar_ValueChanged);
        }

        /// <summary>
        /// Performs events unsubscription from internal objects.
        /// The base implementation must always be called.
        /// </summary>
        protected virtual void UnwireEvents()
        {
            this.scroller.ScrollerUpdated -= scroller_ScrollerUpdated;
            this.HScrollBar.ValueChanged -= HScrollBar_ValueChanged;
            if (this.items is Telerik.WinControls.Data.ObservableCollection<Item>)
            {
                (this.items as Telerik.WinControls.Data.ObservableCollection<Item>).CollectionChanged -= new NotifyCollectionChangedEventHandler(VirtualizedScrollPanel_CollectionChanged);
            }
        }

        /// <summary>
        /// This method creates an object that implements IVirtualizedElementProvider. Child elements are not yet created
        /// in this method.
        /// </summary>
        /// <returns>A new instance of an implementation of IVirtualizedElementProvider.</returns>
        protected virtual IVirtualizedElementProvider<Item> CreateElementProvider()
        {
            return new VirtualizedPanelElementProvider<Item, Element>();
        }


        /// <summary>
        /// Creates an instance of ITraverser which traverses the child elements.
        /// </summary>
        /// <returns></returns>
        protected virtual ITraverser<Item> CreateItemTraverser(IList<Item> items)
        {
            return new ItemsTraverser<Item>(items);
        }

        /// <summary>
        /// Creates an instance of ItemScroller. Child elements are not yet created in this method.
        /// </summary>
        /// <returns></returns>
        protected virtual ItemScroller<Item> CreateItemScroller()
        {
            return new ItemScroller<Item>();
        }

        /// <summary>
        /// This method provides a chance to setup the ItemScroller.
        /// </summary>
        /// <param name="scroller">The item scroller on which properties will be set.</param>
        protected virtual void InitializeItemScroller(ItemScroller<Item> scroller)
        {
            this.scroller.Scrollbar = this.VScrollBar;
            this.scroller.ElementProvider = this.elementProvider;
        }

        /// <summary>
        /// This method provides a chance to setup the the VirtualizedStackContainer.
        /// </summary>
        /// <param name="viewElement">The view element on which properties will be set.</param>
        protected override void InitializeViewElement(VirtualizedStackContainer<Item> viewElement)
        {
 	        base.InitializeViewElement(viewElement);

            this.ViewElement.ElementProvider = elementProvider;
            this.ViewElement.DataProvider = this.scroller;
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();

            this.UnwireEvents();

            this.scroller.Dispose();
        }

        #endregion

        #region Properties

        public virtual IList<Item> Items
        {
            get 
            { 
                return this.items;
            }
            set 
            {
                if (this.items != value)
                {
                    if(this.items is Telerik.WinControls.Data.ObservableCollection<Item>)
                    {
                        (this.items as Telerik.WinControls.Data.ObservableCollection<Item>).CollectionChanged -= new NotifyCollectionChangedEventHandler(VirtualizedScrollPanel_CollectionChanged);
                    }
                    this.items = value;
                    this.scroller.Traverser = this.CreateItemTraverser(this.items);
                    //this.scroller.ItemCount = this.items.Count;
                    if (this.items is Telerik.WinControls.Data.ObservableCollection<Item>)
                    {
                        (this.items as Telerik.WinControls.Data.ObservableCollection<Item>).CollectionChanged += new NotifyCollectionChangedEventHandler(VirtualizedScrollPanel_CollectionChanged);
                    }
                    this.scroller.UpdateScrollRange();
                    this.ViewElement.UpdateItems();
                }
            }
        }

        public ItemScroller<Item> Scroller
        {
            get 
            {
                return this.scroller; 
            }
        }

        public bool FitItemsToSize
        {
            get 
            { 
                return this.ViewElement.FitElementsToSize;
            }
            set 
            {
                if (this.ViewElement.FitElementsToSize != value)
                {
                    this.ViewElement.FitElementsToSize = value;
                    UpdateFitToSizeMode();          
                }
            }
        }

        public Orientation Orientation
        {
            get
            {
                return this.ViewElement.Orientation;
            }
            set
            {
                if (this.ViewElement.Orientation != value)
                {
                    this.ViewElement.Orientation = value;                  
                }
            }
        }

        public bool AutoSizeItems
        {
            get
            {
                return this.autoSizeItems;
            }
            set
            {
                if (this.autoSizeItems != value)
                {
                    this.autoSizeItems = value;
                    OnAutoSizeChanged();
                }
            }
        }

        public virtual int ItemSpacing
        {
            get { return this.Scroller.ItemSpacing; }
            set
            {
                this.Scroller.ItemSpacing = value;
                this.ViewElement.ItemSpacing = value;
                this.Scroller.UpdateScrollRange();
                this.ViewElement.UpdateItems();
            }
        }

        #endregion

        #region Layout

        public virtual SizeF MeasureItem(Item item, SizeF availableSize)
        {
            IVirtualizedElement<Item> element = ViewElement.ElementProvider.GetElement(item, null);
            RadElement radElement = (RadElement)element;

            this.SuspendLayout();

            this.Children.Add(radElement);
            element.Attach(item, null);

            radElement.ResetLayout(true);
            radElement.Measure(availableSize);//new SizeF(float.PositiveInfinity, float.PositiveInfinity));
            SizeF desiredSize = radElement.GetDesiredSize(false);

            this.Children.Remove(radElement);
            this.ViewElement.ElementProvider.CacheElement(element);
            element.Detach();

            this.ResumeLayout(false);

            return desiredSize;
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF desiredSize = base.MeasureOverride(availableSize);

            if (UpdateOnMeasure(availableSize))
            {
                base.MeasureOverride(availableSize);
            }           

            return desiredSize;
        }
        
        protected virtual SizeF GetItemDesiredSize(Item item)
        {
            return MeasureItem(item, new SizeF(float.PositiveInfinity, float.PositiveInfinity));
        }

        #endregion

        #region Event handlers

        private void VirtualizedScrollPanel_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.scroller.UpdateScrollRange();
        }

        protected virtual void scroller_ScrollerUpdated(object sender, EventArgs e)
        {
            this.ViewElement.ScrollOffset = new SizeF(this.ViewElement.ScrollOffset.Width, -this.Scroller.ScrollOffset);
            this.ViewElement.InvalidateMeasure();
        }

        protected virtual void HScrollBar_ValueChanged(object sender, EventArgs e)
        {
            this.ViewElement.ScrollOffset = new SizeF(-this.HScrollBar.Value, this.ViewElement.ScrollOffset.Height);
            this.ViewElement.InvalidateMeasure();
        }

        protected virtual void OnAutoSizeChanged()
        {
            this.Scroller.UpdateScrollRange();
            this.ViewElement.UpdateItems();
            this.ViewElement.PerformLayout();
        }

        #endregion

        /// <summary>
        /// Gets the <see cref="Element"/> with the specified item.
        /// </summary>
        /// <value></value>
        public Element GetElement(Item item)
        {
            foreach (RadElement element in this.ViewElement.Children)
            {
                IVirtualizedElement<Item> virtualElement = element as IVirtualizedElement<Item>;
                if (virtualElement != null && virtualElement.Data == item)
                {
                    return (Element)virtualElement;
                }
            }

            return default(Element);
        }

        protected virtual bool UpdateOnMeasure(SizeF availableSize)
        {
            RectangleF clientRect = GetClientRectangle(availableSize);

            RadScrollBarElement hscrollbar = this.HScrollBar;
            RadScrollBarElement vscrollbar = this.VScrollBar;

            if (this.Orientation == Orientation.Horizontal)
            {
                hscrollbar = this.VScrollBar;
                vscrollbar = this.HScrollBar;
            } 
            ElementVisibility visibility = hscrollbar.Visibility;
            if (FitItemsToSize)
            {
                HScrollBar.Visibility = ElementVisibility.Collapsed;
            }
            else
            {
   
                hscrollbar.LargeChange = (int)(clientRect.Width - vscrollbar.DesiredSize.Width - this.ViewElement.Margin.Horizontal);
                hscrollbar.SmallChange = hscrollbar.LargeChange / 10;
                hscrollbar.Visibility = hscrollbar.LargeChange < hscrollbar.Maximum ? ElementVisibility.Visible : ElementVisibility.Collapsed;
            }

            SizeF clientSize = clientRect.Size;
            if (HScrollBar.Visibility == ElementVisibility.Visible)
            {
                clientSize.Height -= HScrollBar.DesiredSize.Height;
            }
            this.scroller.ClientSize = clientSize;
            
            return visibility != hscrollbar.Visibility;
        }
        
        protected virtual void UpdateFitToSizeMode()
        {
            if (FitItemsToSize)
            {
                this.HScrollBar.Maximum = 0;
            }
            else
            {
                int maximumWidth = 0;
                if (this.Items != null)
                {
                    foreach (Item item in this.Items)
                    {
                        int itemWidth = (int)GetItemDesiredSize(item).Width;
                        maximumWidth = Math.Max(itemWidth, maximumWidth);
                    }
                }
                this.HScrollBar.Maximum = maximumWidth;            
            }
        }
    }
}
