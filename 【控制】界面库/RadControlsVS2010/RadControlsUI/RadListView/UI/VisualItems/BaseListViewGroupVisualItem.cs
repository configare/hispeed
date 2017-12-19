using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Enumerations;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    public class BaseListViewGroupVisualItem : BaseListViewVisualItem
    {
        public static RadProperty ExpandedProperty = RadProperty.Register(
            "Expanded", typeof(bool), typeof(BaseListViewGroupVisualItem),
                new RadElementPropertyMetadata(true,
                    ElementPropertyOptions.InvalidatesLayout |
                    ElementPropertyOptions.AffectsParentMeasure |
                    ElementPropertyOptions.AffectsParentArrange));
        
        [Browsable(false)]
        public bool IsExpanded
        {
            get
            {
                return (bool)this.GetValue(ExpandedProperty);
            }
            internal set
            {
                this.SetValue(ExpandedProperty, value);
            }
        }

        #region Initialization

        static BaseListViewGroupVisualItem()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ListViewGroupVisualItemStateManagerFactory(), typeof(BaseListViewGroupVisualItem));
        }
        
        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.toggleElement.Class = "ListViewGroupToggleButton";
            this.toggleElement.ThemeRole = "ListViewGroupToggleButton";

            this.ShowHorizontalLine = true;
            this.HorizontalLineColor = Color.FromArgb(255, 207, 214, 225);
        }

        #endregion

        #region Overrides

        public override bool IsCompatible(ListViewDataItem data, object context)
        {
            return data is ListViewDataItemGroup;
        }
         
        protected override void SynchronizeProperties()
        {
            base.SynchronizeProperties();
            this.IsExpanded = (this.Data as ListViewDataItemGroup).Expanded;
            this.toggleElement.ToggleState = this.IsExpanded ? ToggleState.On : ToggleState.Off;
             
        }

        protected override bool OnToggleButtonStateChanging(StateChangingEventArgs args)
        {
            if (this.IsInValidState(true))
            {
                (this.Data as ListViewDataItemGroup).Expanded = args.NewValue == Enumerations.ToggleState.On;
                args.Cancel = ((this.Data as ListViewDataItemGroup).Expanded != (args.NewValue == Enumerations.ToggleState.On));
            }

            return args.Cancel;
        }
         
        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            (this.Data as ListViewDataItemGroup).Expanded ^= true;
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == ExpandedProperty)
            {
                this.Data.Owner.InvalidateMeasure(true);
                this.Data.Owner.ViewElement.Scroller.UpdateScrollRange();
            }

        }

        protected override void DrawHorizontalLine(Paint.IGraphics graphics)
        {
            SizeF size = this.layoutManagerPart.RightPart.DesiredSize;
            int x = (int)this.toggleElement.DesiredSize.Width;
            int y = this.Size.Height / 2;
            int x2 = 0;
            int y2 = y;

            ContentAlignment textAlignment = this.GetTextAlignment();

            if (textAlignment == ContentAlignment.MiddleLeft ||
                textAlignment == ContentAlignment.TopLeft ||
                textAlignment == ContentAlignment.BottomLeft)
            {
                x += (int)size.Width + 10;
                x2 = this.Size.Width - 2;
            }
            else if (textAlignment == ContentAlignment.MiddleRight ||
                     textAlignment == ContentAlignment.TopRight ||
                     textAlignment == ContentAlignment.BottomRight)
            {
                x += 1;
                x2 = this.Size.Width - 2 - (int)size.Width - 10;
            }
            else if (textAlignment == ContentAlignment.MiddleCenter ||
                     textAlignment == ContentAlignment.TopCenter ||
                     textAlignment == ContentAlignment.BottomCenter)
            {
                x += 1;
                x2 = this.Size.Width / 2 - (int)size.Width / 2 - 10;
            }

            if (x < x2)
            {
                Graphics g = (Graphics)graphics.UnderlayGraphics;
                using (Pen pen = new Pen(this.HorizontalLineColor, this.HorizontalLineWidth))
                {
                    g.DrawLine(pen, x, y, x2, y2);
                    if (this.TextAlignment == ContentAlignment.MiddleCenter ||
                        this.TextAlignment == ContentAlignment.TopCenter ||
                        this.TextAlignment == ContentAlignment.BottomCenter)
                    {
                        x = (this.Size.Width / 2) + (int)size.Width / 2 + 10 + (int)this.toggleElement.DesiredSize.Width;
                        x2 = this.Size.Width - 2;
                        g.DrawLine(pen, x, y, x2, y2);
                    }
                }
            }
        }

        protected override void DrawHorizontalLineWithoutText(Paint.IGraphics graphics)
        {
            Graphics g = (Graphics)graphics.UnderlayGraphics;
            using (Pen pen = new Pen(this.HorizontalLineColor, this.HorizontalLineWidth))
            {
                g.DrawLine(pen, this.toggleElement.Size.Width, this.Size.Height / 2, 
                    this.Size.Width, this.Size.Height / 2);
            }
        }

        #endregion
    }
}
