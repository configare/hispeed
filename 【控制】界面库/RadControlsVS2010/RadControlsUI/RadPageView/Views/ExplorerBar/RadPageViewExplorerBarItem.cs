using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents a single item in the <see cref="RadPageView"/>'s explorer bar view mode.
    /// </summary>
    public class RadPageViewExplorerBarItem : RadPageViewStackItem
    {
        #region RadProperties

        public static RadProperty IsExpandedProperty = RadProperty.Register(
            "IsExpanded",
            typeof(bool),
            typeof(RadPageViewExplorerBarItem),
            new RadElementPropertyMetadata(false, 
                ElementPropertyOptions.AffectsParentMeasure | 
                ElementPropertyOptions.AffectsMeasure | 
                ElementPropertyOptions.AffectsDisplay | 
                ElementPropertyOptions.CanInheritValue | 
                ElementPropertyOptions.Cancelable)
            );

        #endregion

        #region Fields

        internal const int DEFAULT_PAGE_LENGTH = 300;
        private int itemPageLength = DEFAULT_PAGE_LENGTH;
        private RadPageViewContentAreaElement associatedContentArea;

        #endregion

        #region Ctor/Initialization

        /// <summary>
        /// Creates an instance of the <see cref="RadPageViewExplorerBarItem"/>.
        /// </summary>
        public RadPageViewExplorerBarItem()
            : base()
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="RadPageViewExplorerBarItem"/>.
        /// </summary>
        public RadPageViewExplorerBarItem(string text)
            : base(text)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="RadPageViewExplorerBarItem"/>.
        /// </summary>
        public RadPageViewExplorerBarItem(string text, Image image)
            : base(text, image)
        {
        }

        #endregion

        #region Methods

        private void DoExpandCollapse(bool isExpanded)
        {
            if (this.Owner == null)
                return;
            if (isExpanded)
            {
                (this.Owner as RadPageViewExplorerBarElement).ExpandItem(this);
            }
            else
            {
                (this.Owner as RadPageViewExplorerBarElement).CollapseItem(this);
            }
        }

        private bool TryExpandCollapse(bool isExpanding)
        {
            if (this.Owner == null)
                return false;

            if (isExpanding)
            {
                return (this.Owner as RadPageViewExplorerBarElement).OnItemExpanding(this);
            }
            else
            {
                return (this.Owner as RadPageViewExplorerBarElement).OnItemCollapsing(this);
            }
        }

        #endregion

        #region CLR Properties

        internal override int PageLength
        {
            get
            {
                return this.itemPageLength;
            }
            set
            {
                if (value != this.itemPageLength)
                {
                    this.itemPageLength = value;
                    if (this.Owner != null)
                    {
                        this.Owner.InvalidateMeasure();
                    }
                }
            }
        }

        internal override bool IsContentVisible
        {
            get
            {
                return this.IsExpanded;
            }
            set
            {
                this.IsExpanded = value;
                if (this.Page != null)
                {
                    this.Page.IsContentVisible = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a boolean value that determines whether the content of the <see cref="RadPageViewExplorerBarItem"/>
        /// is expanded.
        /// </summary>
        [DefaultValue(false)]
        [Description("Gets or sets a boolean value that determines whether the content of the item is expanded.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool IsExpanded
        {
            get
            {
                return (bool)this.GetValue(IsExpandedProperty);
            }
            set
            {
                this.SetValue(IsExpandedProperty, value);
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="RadPageViewContentAreaElement"/> that
        /// represents the content holder of this <see cref="RadPageViewExplorerBarItem"/>.
        /// </summary>
        public RadPageViewContentAreaElement AssociatedContentAreaElement
        {
            get
            {
                return this.associatedContentArea;
            }
            internal set
            {
                if (this.associatedContentArea != value)
                {
                    this.associatedContentArea = value;
                }

                if (this.associatedContentArea != null)
                {
                    this.associatedContentArea.Visibility = this.IsExpanded ? ElementVisibility.Visible : ElementVisibility.Collapsed;
                }
            }
        }

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(RadPageViewStackItem);
            }
        }

        #endregion

        #region Event handling

        protected override void OnPropertyChanging(RadPropertyChangingEventArgs args)
        {
            if (args.Property == IsExpandedProperty)
            {
                args.Cancel = this.TryExpandCollapse((bool)args.NewValue);
            }

            base.OnPropertyChanging(args);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == IsExpandedProperty)
            {
                this.DoExpandCollapse((bool)e.NewValue);
                if (this.Page != null)
                {
                    this.Page.IsContentVisible = (bool)e.NewValue;

                    this.AssociatedContentAreaElement.Visibility = this.Page.IsContentVisible ? ElementVisibility.Visible : ElementVisibility.Collapsed;
                }
            }
        }

        public override void Attach(RadPageViewPage page)
        {
            base.Attach(page);
            if (page.Owner == null)
                return;

            if (page.Owner.IsHandleCreated)
            {
                bool isExpanded = this.IsExpanded;
                this.Page.Visible = isExpanded;
            }
        }

        public override void Detach()
        {
            if (this.Page.Owner != null)
            {
                if (this.Page.Owner.IsHandleCreated)
                {
                    this.Page.Visible = false;
                }
            }

            base.Detach();
        }

        #endregion
    }
}
