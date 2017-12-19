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
    public class RadCommandBarOverflowPanelElement : LightVisualElement
    {
        protected LayoutPanel layout;

        /// <summary>
        /// Represent Layout that holds elements over the menu
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        [Description("Represent Layout that holds elements over the menu")]        
        public LayoutPanel Layout
        {
            get
            {
                return layout;
            }
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.layout = this.CreateLayout();
            this.StretchHorizontally = true;
            this.StretchVertically = true;
            this.Children.Add(this.layout);
        }

        public virtual int GetChildMaxWidth()
        {
            int maxWidth = 0;
            foreach (RadElement child in this.Layout.Children)
            {
                RadCommandBarBaseItem item = child as RadCommandBarBaseItem;
                if (item != null && item.VisibleInStrip)
                {
                    maxWidth = Math.Max(maxWidth, (int)Math.Floor(item.DesiredSize.Width));
                }
            }
            return maxWidth;
        }

        public virtual int GetChildrenTotalWidth()
        {
            int totalWidth = 0;
            foreach (RadElement child in this.Layout.Children)
            {
                RadCommandBarBaseItem item = child as RadCommandBarBaseItem;
                if (item != null && item.VisibleInStrip)
                {
                    totalWidth += (int)Math.Floor(item.DesiredSize.Width);
                }
            }
            return totalWidth;
        }

        /// <summary>
        /// This create the default layout 
        /// </summary>
        /// <returns></returns>
        protected virtual LayoutPanel CreateLayout()
        {
            return new RadCommandBarOverflowPanel();
        }

        protected override Type ThemeEffectiveType
        {
            get
            {
                return typeof(CommandBarStripElement);
            }
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF size = base.MeasureOverride(availableSize); 
            return size;
        }
    }
}
