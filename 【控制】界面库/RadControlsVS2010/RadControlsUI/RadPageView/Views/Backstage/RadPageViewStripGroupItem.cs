using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class RadPageViewStripGroupItem : RadPageViewStripItem
    {
        #region Fields

        private LightVisualElement underline;

        #endregion

        #region Constructors

        public RadPageViewStripGroupItem()
        {
        }

        public RadPageViewStripGroupItem(string text) : base(text)
        {
        }

        public RadPageViewStripGroupItem(string text, Image image) :base(text,image)
        {
        }

        #endregion

        #region Overrides

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.DrawFill = true;
            this.BackColor = Color.Green;

            this.underline = new LightVisualElement();
            underline.StretchHorizontally = true;
            underline.Class = "PageViewGroupItemUnderline";
            underline.ThemeRole = "PageViewGroupItemUnderline";
            this.Children.Add(underline);
        }

        protected override void ArrangeChildren(SizeF available)
        {
            base.ArrangeChildren(available);

            RectangleF clientRect = GetClientRectangle(available);
            this.underline.Arrange(new RectangleF(clientRect.Left, clientRect.Bottom - this.underline.DesiredSize.Height,
                clientRect.Width, this.underline.DesiredSize.Height));
        }

        #endregion
    }
}
