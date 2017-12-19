using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// A base class for all of the items contained in <see cref="CommandBarStripElement"/>.
    /// </summary>
    [DefaultEvent("Click")]
    public class RadCommandBarBaseItem : RadCommandBarVisualElement
    {
        #region RoutedEvents
        public static RoutedEvent ClickEvent =
        RegisterRoutedEvent("ClickEvent", typeof(RadCommandBarBaseItem));
        public static RoutedEvent VisibleInStripChangedEvent =
        RegisterRoutedEvent("VisibleInStripChangedEvent", typeof(RadCommandBarBaseItem));
        public static RoutedEvent VisibleInStripChangingEvent =
        RegisterRoutedEvent("VisibleInStripChangingEvent", typeof(RadCommandBarBaseItem));

        #endregion

        public static RadProperty VisibleInStripProperty = RadProperty.Register("VisibleInStrip", typeof(bool), typeof(RadCommandBarBaseItem),
                                                                                new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsParentMeasure | ElementPropertyOptions.AffectsParentArrange | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsArrange | ElementPropertyOptions.InvalidatesLayout));

        #region Fields
        protected bool visibleInStrip = true;
        protected bool inheritsParentOrientation = true;
        protected bool visibleInOverflowMenu = true;
        private Size localMinSize; 
        #endregion

        #region Events 

        /// <summary>
        /// Occurs when the orientation is changed
        /// </summary>
        public event EventHandler OrientationChanged;

        /// <summary>
        /// Occurs before the orientation is changed
        /// </summary>
        public event CancelEventHandler OrientationChanging; 

        /// <summary>
        /// Occurs when the <see cref="P:RadCommandBarBaseItem.VisibleInStrip"/> property is changed.
        /// </summary>
        public event EventHandler VisibleInStripChanged;
        
        #endregion

        #region Properties

        /// <summary>
        ///		Show or hide item from the strip overflow menu
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Indicates whether the item should be drawn in the overflow menu.")]     
        public bool VisibleInOverflowMenu
        {
            get
            {
                return visibleInOverflowMenu;
            }
            set
            {
                visibleInOverflowMenu = value;
            }
        }

        /// <summary>
        ///		Gets or sets the Orientation of the item.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory), DefaultValue(System.Windows.Forms.Orientation.Horizontal)]
        [Description("Gets or sets the orientation of the item.")]
        public override Orientation Orientation
        {
            get
            {
                return base.Orientation;
            }
            set
            {
                if (this.orientation != value && !this.OnOrientationChanging(new CancelEventArgs()))
                {
                    this.SetOrientationCore(value);
                    this.OnOrientationChanged(new EventArgs());
                }
            }
        }

        private void SetOrientationCore(Orientation value)
        {
            this.orientation = value;
            bool isStretchedHorizontally = this.StretchHorizontally;
            this.StretchHorizontally = this.StretchVertically;
            this.StretchVertically = isStretchedHorizontally;
        }

        /// <summary>
        ///		Show or hide item from the strip
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory), DefaultValue(true)]
        [Description("Indicates whether the item should be drawn in the strip.")]
        public virtual bool VisibleInStrip
        {
            get
            {
                return this.visibleInStrip;
            }
            set
            {
                if (this.visibleInStrip != value)
                {
                    CancelEventArgs e = new CancelEventArgs();
                    this.RaiseBubbleEvent(this, new RoutedEventArgs(e, VisibleInStripChangingEvent));

                    if (!e.Cancel)
                    {
                        this.visibleInStrip = value;
                        this.SetValue(RadElement.MinSizeProperty, (value) ? this.localMinSize : Size.Empty);
                        this.SetValue(RadCommandBarBaseItem.VisibleInStripProperty, value);
                        this.OnVisibleInStripChanged(new EventArgs());
                    }
                }
            }
        }

        /// <summary>
        ///		Gets or sets that the orientation will be inherit from parent
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory), DefaultValue(true)]
        [Description("Indicates whether the item's orientation will be affected by parent's orientation.")]
        public virtual bool InheritsParentOrientation
        {
            get
            {
                return inheritsParentOrientation;
            }
            set
            {
                inheritsParentOrientation = value;
            }
        }

        #endregion

        #region Event Management

        /// <summary>
        /// Raises the <see cref="E:RadCommandBarBaseItem.OrientationChanged"/> event.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected virtual void OnOrientationChanged(EventArgs e)
        {
            if (this.OrientationChanged != null)
            {
                this.OrientationChanged(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:RadCommandBarBaseItem.OrientationChanging"/> event.
        /// </summary>
        /// <param name="e">Event data.</param>
        /// <returns>true if the event should be canceled, false otherwise.</returns>
        protected virtual bool OnOrientationChanging(CancelEventArgs e)
        {
            if (this.OrientationChanging != null)
            {
                this.OrientationChanging(this, e);
                return e.Cancel;
            }
            return false;
        }

        /// <summary>
        /// Raises the <see cref="E:RadCommandBarBaseItem.VisibleInStripChanged"/> event.
        /// </summary>
        /// <param name="e">Event data.</param>
        protected virtual void OnVisibleInStripChanged(EventArgs e)
        {
            if (this.VisibleInStripChanged != null)
            {
                this.VisibleInStripChanged(this, e);
            }

            this.RaiseBubbleEvent(this, new RoutedEventArgs(new EventArgs(), VisibleInStripChangedEvent));
        }

        #endregion

        #region Overrides
         
        public override Size MinSize
        {
            get
            {
                return base.MinSize;
            }
            set
            {
                this.localMinSize = value;
                if (this.VisibleInStrip)
                {
                    base.MinSize = value;
                }
            }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.Alignment = ContentAlignment.MiddleCenter;
            this.SetDefaultValueOverride(LightVisualElement.StretchHorizontallyProperty, false);
            this.SetDefaultValueOverride(LightVisualElement.StretchVerticallyProperty, true);
            this.localMinSize = this.MinSize;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.RaiseBubbleEvent(this, new RoutedEventArgs(new EventArgs(), ClickEvent));
        }
         
        protected virtual SizeF ExcludeBorders(SizeF elementSize)
        { 
            elementSize.Height -= this.BorderBottomWidth + this.BorderTopWidth;
            elementSize.Width -= this.BorderRightWidth + this.BorderLeftWidth;
            return elementSize;
        }

        protected virtual SizeF IncludeBorders(SizeF elementSize)
        {
            elementSize.Height += this.BorderBottomWidth + this.BorderTopWidth;
            elementSize.Width += this.BorderRightWidth + this.BorderLeftWidth;
            return elementSize;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.SetValue(RadItem.IsMouseDownProperty, false);
        }

        #endregion
    }
}
