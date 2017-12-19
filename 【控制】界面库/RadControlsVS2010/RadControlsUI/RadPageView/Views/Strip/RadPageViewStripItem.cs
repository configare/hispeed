using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Paint;
using System.ComponentModel;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    public class RadPageViewStripItem : RadPageViewItem
    {
        public static RadProperty AutoCorrectOrientationProperty = RadProperty.Register(
           "AutoCorrectOrientation",
           typeof(bool),
           typeof(RadPageViewStripItem),
           new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets or sets a property which determines whether to consider the ItemBorderAndFillOrientation of RadPageViewElement.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets a property which determines whether to consider the ItemBorderAndFillOrientation of RadPageViewElement.")]
        public bool AutoCorrectOrientation
        {
            get { return (bool)GetValue(AutoCorrectOrientationProperty); }
            set { SetValue(AutoCorrectOrientationProperty, value); }
        }

        #region Constructors/Initialization

        public RadPageViewStripItem()
        {
        }

        public RadPageViewStripItem(string text)
        {
            this.Text = text;
        }

        public RadPageViewStripItem(string text, Image image)
        {
            this.Text = text;
            this.Image = image;
        }

        #endregion


        protected override object CorrectFillAndBorderOrientation(IGraphics g)
        {
            if (AutoCorrectOrientation)
            {
                return this.ApplyOrientationTransform(g, this.BorderAndFillOrientation);
            }
            return null;
        }

        protected override RectangleF ModifyBorderAndFillPaintRect(RectangleF preferred, Padding padding)
        {
            if (!AutoCorrectOrientation)
            {
                return preferred;
            }
            return base.ModifyBorderAndFillPaintRect(preferred, padding);
        }
    }
}
