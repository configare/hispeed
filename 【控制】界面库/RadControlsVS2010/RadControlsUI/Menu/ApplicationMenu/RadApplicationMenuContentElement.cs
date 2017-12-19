using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    public class RadApplicationMenuContentElement : RadElement
    {
        private FillPrimitive fill;
        private BorderPrimitive border;
        private StackLayoutPanel layout;

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.UseNewLayoutSystem = true;
        }

        #region Properties

        public FillPrimitive Fill
        {
            get
            {
                return this.fill;
            }
        }

        public BorderPrimitive Border
        {
            get
            {
                return this.border;
            }
        }

        public StackLayoutPanel Layout
        {
            get
            {
                return this.layout;
            }
            set
            {
                if (value != null && value != this.layout)
                {
                    if (this.layout != null)
                    {
                        this.Children.Remove(this.layout);
                    }
                    this.layout = value;
                    this.Children.Add(this.layout);
                }
            }
        }

        #endregion

        protected override void CreateChildElements()
        {
            fill = new FillPrimitive();
            this.Children.Add(fill);

            border = new BorderPrimitive();
            this.Children.Add(border);

            layout = new StackLayoutPanel();
            this.Children.Add(layout);
        }
    }
}
