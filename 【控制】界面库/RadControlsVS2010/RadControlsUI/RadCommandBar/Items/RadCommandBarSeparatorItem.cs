using System;
using System.Windows.Forms;
using System.Text;
using Telerik.WinControls.Styles;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a separator for the items in <see cref="CommandBarStripElement"/>.
    /// </summary>
    public class CommandBarSeparator : RadCommandBarBaseItem
    {
        static CommandBarSeparator()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ItemStateManagerFactory(), typeof(CommandBarSeparator));
        }

        public CommandBarSeparator()
        {
            this.visibleInOverflowMenu = false;
        }

        public static RadProperty ThiknessProperty =
        RadProperty.Register("Thickness", typeof(int), typeof(CommandBarSeparator),
                             new RadElementPropertyMetadata(2,ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.AffectsMeasure
                                                            | ElementPropertyOptions.AffectsArrange | ElementPropertyOptions.AffectsParentMeasure | ElementPropertyOptions.AffectsParentArrange
                                                            | ElementPropertyOptions.AffectsTheme | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        protected int cachedThikness = 2;

        /// <summary>
        ///		Gets or sets the thickness of the separator item.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory), DefaultValue(2)]
        [Description("Gets or sets the thickness of the separator item.")]        
        public int Thickness
        {
            get
            {
                return this.cachedThikness;
            }
            set
            {
                this.cachedThikness = value;
                this.MinSize = new Size(this.cachedThikness, 0);
                this.SetValue(CommandBarSeparator.ThiknessProperty, this.cachedThikness);
            }
        }

        [Browsable(false)]
        public new string Text
        {
            get
            {
                return "";
            }
            set
            {
            }
        }

        #region Overrides

        protected override void OnOrientationChanged(EventArgs e)
        {
            this.AngleTransform = (this.Orientation == System.Windows.Forms.Orientation.Vertical) ? 90 : 0;
            this.StretchVertically = (this.orientation == Orientation.Horizontal);
            this.StretchHorizontally = (this.orientation == Orientation.Vertical);
            base.OnOrientationChanged(e);
        }

        protected override void CreateChildElements()
        {
            this.StretchVertically = (this.orientation == Orientation.Horizontal);
            this.StretchHorizontally = (this.orientation == Orientation.Vertical);
            this.SetValue(CommandBarSeparator.ThiknessProperty, cachedThikness);
            this.DrawFill = true;
            this.DrawBorder = false;
            this.DrawText = false;
            this.MinSize = new Size(this.cachedThikness, 0);
            this.BackColor = Color.Silver;
            this.BackColor2 = Color.Silver;        
        }
        
        #endregion
    }
}
