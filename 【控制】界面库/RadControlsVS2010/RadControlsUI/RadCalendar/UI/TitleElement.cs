using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class TitleElement : CalendarVisualElement
    {
        internal protected TitleElement(): this(string.Empty)
        {
        }

        internal protected TitleElement(string text)
            : this(null, text)
        {
        }

		public override CalendarView View
		{
			get
			{
				return base.View;
			}
			set
			{
				base.View = value;
                if (value != null)
                {
                    this.Text = value.GetTitleContent();
                    this.Invalidate();
                }
			}
		}

        public TitleElement(CalendarVisualElement owner, string text)
            : base(owner, null, null)
        {
            this.Text = text;
        }

        protected override System.Drawing.SizeF MeasureOverride(System.Drawing.SizeF availableSize)
        {
            SizeF size = base.MeasureOverride(availableSize);
            return new SizeF(size.Width, size.Height + this.Padding.Vertical);
        }

        protected override void InitializeFields()
        {
             base.InitializeFields();
             this.Class = "CalendarNavigationElement";
        }
    }
}
