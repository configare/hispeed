using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    internal class RadListDataGroupItem : RadListDataItem
    {
        public static readonly RadProperty CollapsibleProperty =
            RadProperty.Register("Collapsible", typeof(bool), typeof(RadListDataGroupItem),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsParentMeasure | ElementPropertyOptions.AffectsParentArrange));

        public static readonly RadProperty CollapsedProperty =
            RadProperty.Register("Collapsed", typeof(bool), typeof(RadListDataGroupItem),
            new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsParentMeasure | ElementPropertyOptions.AffectsParentArrange));

        private bool cachedCollapsible = false;

        public RadListDataGroupItem(ListGroup group)
            : base(group.Header)
        {
            this.group = group;
            this.Collapsed = group.Collapsed;
            this.Collapsible = group.Collapsible;
        }

        public bool Collapsible
        {
            get { return cachedCollapsible; }
            set 
            {
                if (this.cachedCollapsible != value)
                {
                    this.cachedCollapsible = value;
                    this.group.Collapsible = value;
                    this.SetValue(RadListDataGroupItem.CollapsibleProperty, value);
                }
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == RadListDataGroupItem.CollapsibleProperty)
            {
                if (!this.Collapsible)
                {
                    this.Collapsed = false;
                }
            }
        }

        public bool Collapsed
        {
            get
            {
                return this.group.Collapsed;
            }
            set
            {
                if (
                    (this.Collapsible || (!this.Collapsible && !value))
                    &&  value != this.group.Collapsed)
                {
                    this.group.Collapsed = value;
                    this.SetValue(RadListDataGroupItem.CollapsedProperty, value);
                    this.Owner.Scroller.UpdateScrollRange();
                    this.Owner.InvalidateMeasure(true);
                }
            }
        }

        internal override ListGroup Group
        {
            get
            {
                return group;
            }
        }

    }
}
