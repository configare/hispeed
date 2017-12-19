using System.ComponentModel;
using System.Drawing;
using Telerik.WinControls.Design;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.UI;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a drop-down button element with support for an additional overflow image.
    /// </summary>
    [RadToolboxItemAttribute(false)]
    public class OverflowDropDownButtonElement : RadDropDownButtonElement
    {
        #region Fields

        private Image cachedImage = null;

        #endregion

        #region Properties

        /// <summary>
        /// A property to specify an additional image to be displayed when <see cref="OverflowDropDownButtonElement.OverflowMode">OverflowMode</see> is true.
        /// </summary>
        public static RadProperty OverflowImageProperty = RadProperty.Register(
           "OverflowImage",
           typeof(Image),
           typeof(OverflowDropDownButtonElement),
           new RadElementPropertyMetadata(null,
               ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        /// A property to determine whether a <see cref="OverflowDropDownButtonElement">OverflowDropDownButtonElement</see> instance is currently in overflow mode.
        /// </summary>
        public static RadProperty OverflowModeProperty = RadProperty.Register(
            "OverflowMode", typeof(bool), typeof(OverflowDropDownButtonElement), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.None));

        ///<summary>
        /// Gets or sets the overflow image that is displayed on a button element.
        /// </summary>		
        [RadPropertyDefaultValue("OverflowImage", typeof(OverflowDropDownButtonElement)),
        Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets the overflow image that is displayed on a button element."),
        RefreshProperties(RefreshProperties.All),
        TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        public virtual Image OverflowImage
        {
            get
            {
                return (Image)this.GetValue(OverflowDropDownButtonElement.OverflowImageProperty);
            }
            set
            {
                this.SetValue(OverflowDropDownButtonElement.OverflowImageProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the drop down button is in overflow mode
        /// </summary>
        [RadPropertyDefaultValue("OverflowMode", typeof(OverflowDropDownButtonElement))]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the drop down button is in overflow mode.")]
        [RefreshProperties(RefreshProperties.All)]
        public bool OverflowMode
        {
            get
            {
                return (bool)this.GetValue(OverflowDropDownButtonElement.OverflowModeProperty);
            }
            set
            {
                this.SetValue(OverflowDropDownButtonElement.OverflowModeProperty, value);
            }
        }

        #endregion

        /// <summary>
        /// Provides additional logic for synchronizing the overflow image.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if(e.Property == OverflowDropDownButtonElement.OverflowModeProperty)
            {
                if ((bool)e.NewValue)
                {
                    this.cachedImage = this.Image;
                    if (this.OverflowImage != null)
                    {
                        this.Image = this.OverflowImage;
                    }
                    return;
                }

                if (this.cachedImage != null)
                {
                    this.Image = this.cachedImage;
                }
            }

            if (e.Property == RadButtonItem.ImageProperty)
            {
                if (this.Image == null)
                {
                    this.cachedImage = null;
                }
            }
        }
    }
}
