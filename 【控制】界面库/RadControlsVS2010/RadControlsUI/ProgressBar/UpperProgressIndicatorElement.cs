using System.ComponentModel;
namespace Telerik.WinControls.UI
{
    public class UpperProgressIndicatorElement : ProgressIndicatorElement
    {
        public static RadProperty AutoOpacityProperty = RadProperty.Register(
            "AutoOpacity", typeof(bool), typeof(UpperProgressIndicatorElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty AutoOpacityMinimumProperty = RadProperty.Register(
            "AutoOpacityMinimum", typeof(double), typeof(UpperProgressIndicatorElement), new RadElementPropertyMetadata(
                0.75d, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// Gets or sets whether this progress indicatior will automatically control its 
        /// opacity when close to or over the second progress indicator.
        /// </summary>
        [Description("Gets or sets whether this progress indicatior will automatically control its " + 
            "opacity when close to or over the second progress indicator.")]
        [Category("Behavior")]
        public bool AutoOpacity
        {
            get
            {
                return (bool)this.GetValue(AutoOpacityProperty);
            }
            set
            {
                this.SetValue(AutoOpacityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum opacity level this progress indicator will go to
        /// when over the second progress indicator when AutoOpacity property is set
        /// to true.
        /// </summary>
        [Description("Gets or sets the minimum opacity level this progress indicator will go to when " +
            "over the second progress indicator when AutoOpacity property is set to true.")]
        [Category("Behavior")]
        public double AutoOpacityMinimum
        {
            get
            {
                return (double)this.GetValue(AutoOpacityMinimumProperty);
            }
            set
            {
                this.SetValue(AutoOpacityMinimumProperty, value);
            }
        }
    }
}
