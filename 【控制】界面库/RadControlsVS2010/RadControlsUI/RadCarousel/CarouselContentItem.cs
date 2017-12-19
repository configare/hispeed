using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using Telerik.WinControls.Layouts;
using System.Drawing;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// RadCarouselContentItem with CarouselElement and Reflection primitive
    /// </summary>
    public class CarouselContentItem : VisualElement
    {
        
#region members
        public ReflectionPrimitive reflectionPrimitive;
        protected RadElement hostedItem;
        internal bool paintingReflection = false;
        private RadCarouselElement owner;

        #endregion

#region CStor

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.StretchHorizontally = false;
            this.StretchVertically = false;
        }

        public CarouselContentItem()
        {
        }

        /// <summary>
        /// create element with HostedItem
        /// </summary>
        /// <param name="hostedItem"></param>
        public CarouselContentItem(RadElement hostedItem)
        {
            this.HostedItem = hostedItem;
        }        
#endregion


        /// <summary>
        /// Represent the HostedItem
        /// </summary>
        public RadElement HostedItem
        {
            get
            {
                return this.hostedItem;
            }
            set
            {
                if (value == null)
                    throw new ArgumentException("HostedItem cannot be null!");

                if (this.hostedItem != value)
                {
                    if (this.hostedItem != null && this.hostedItem.Parent == this)
                    {
                        this.Children.Remove(this.hostedItem);
                    }

                    this.hostedItem = value;
                    this.Children.Add(this.hostedItem);
                    this.reflectionPrimitive.OwnerElement = this.hostedItem;
                }
            }
        }

        protected override void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
        {
            if (this.reflectionPrimitive != null 
                && !this.reflectionPrimitive.IsDisposed
                && !this.reflectionPrimitive.IsDisposing
                && changeOperation == ItemsChangeOperation.Removing)
            {
                if (object.ReferenceEquals(child, this.reflectionPrimitive.OwnerElement))
                {
                    this.reflectionPrimitive.OwnerElement = null;
                }
            }

            base.OnChildrenChanged(child, changeOperation);
        }

        /// <summary>
        /// Gets the owner RadCarouselElement.
        /// </summary>
        /// <value>The owner.</value>
        public RadCarouselElement Owner
        {
            get { return this.owner; }
        }

        public Point Center
        {
            get 
            { 
                return this.HostedItem != null ?
                    new Point(this.HostedItem.BoundingRectangle.Width / 2, this.HostedItem.BoundingRectangle.Height / 2) :
                    new Point(this.Size.Width / 2, this.Size.Height / 2);
            }
        }

        internal void SetOwner(RadCarouselElement owner)
        {
            this.owner = owner;
            this.reflectionPrimitive.ItemReflectionPercentage = this.Owner.ItemReflectionPercentage;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.reflectionPrimitive = new ReflectionPrimitive(this.HostedItem);
            this.Children.Add(this.reflectionPrimitive);
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            this.HostedItem.Measure(availableSize);
            this.reflectionPrimitive.Measure(availableSize);
            SizeF hostedItemSize = this.HostedItem.DesiredSize;

            return new SizeF(hostedItemSize.Width, (hostedItemSize.Height * 2 ) + 1);
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            this.HostedItem.Arrange(new RectangleF(0, 0, finalSize.Width, finalSize.Height / 2));
            this.reflectionPrimitive.Arrange(new RectangleF(0, (finalSize.Height / 2 ) + 2, finalSize.Width, finalSize.Height / 3f));

            return finalSize;
        }

        /// <summary>
        /// Custom implementation for RadCarouselItem
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj) || (obj != null && object.Equals(hostedItem, obj));
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        protected override void DisposeManagedResources()
        {
            if (this.ElementTree != null)
            {
                ((RadControl)this.ElementTree.Control).ElementInvalidated -=
                    new EventHandler(this.reflectionPrimitive.CarouselContentItem_ElementInvalidated);
            }
            base.DisposeManagedResources();
        }
    }
}

