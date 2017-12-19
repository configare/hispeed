using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Encapsulates a docking guide image. Includes additional settings allowing for proper Docking Guides visualization.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DockingGuideImage : RadDockObject, IDockingGuideImage
    {
        #region Implementation

        internal void SetReadOnly(bool readOnly)
        {
            this.predefined = readOnly;
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Determines which properties should be serialized.
        /// </summary>
        /// <param name="propName"></param>
        /// <returns></returns>
        protected override bool ShouldSerializeProperty(string propName)
        {
            if (this.predefined)
            {
                return false;
            }

            switch (propName)
            {
                case "Image":
                    return this.image != null;
                case "HotImage":
                    return this.hotImage != null;
                case "PreferredSize":
                    return this.preferredSize.Width > 0 && this.preferredSize.Height > 0;
                case "LocationOnCenterGuide":
                    return this.location != Point.Empty;
            }

            return base.ShouldSerializeProperty(propName);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Determines whether the image is internally created by the framework.
        /// Such images may not be modified.
        /// </summary>
        [Description("Determines whether the image is internally created by the framework. Such images may not be modified.")]
        public bool Predefined
        {
            get
            {
                return this.predefined;
            }
        }

        /// <summary>
        /// Gets or sets the desired sized to be used when rendering image.
        /// By default the size of the Image itself is used.
        /// </summary>
        [Description("Gets or sets the desired sized to be used when rendering image. By default the size of the Image itself is used.")]
        public Size PreferredSize
        {
            get
            {
                if (this.preferredSize.Width > 0 && this.preferredSize.Height > 0)
                {
                    return this.preferredSize;
                }

                if (this.image != null)
                {
                    return this.image.Size;
                }

                return Size.Empty;
            }
            set
            {
                value.Width = Math.Max(0, value.Width);
                value.Height = Math.Max(0, value.Height);

                if (this.preferredSize == value)
                {
                    return;
                }

                if (!this.OnPropertyChanging("PreferresSize"))
                {
                    return;
                }

                this.preferredSize = value;
                this.OnPropertyChanged("PreferredSize");
            }
        }

        bool ShouldSerializePreferredSize()
        {
            return this.ShouldSerializeProperty("PreferredSize");
        }

        /// <summary>
        /// Gets or sets the location of the image when displayed on the "Center" docking guide.
        /// </summary>
        [Description("Gets or sets the location of the image when displayed on the \"Center\" docking guide.")]
        public Point LocationOnCenterGuide
        {
            get
            {
                return this.location;
            }
            set
            {
                if (this.location == value)
                {
                    return;
                }

                if (!this.OnPropertyChanging("LocationOnCenterGuide"))
                {
                    return;
                }

                this.location = value;
                this.OnPropertyChanged("LocationOnCenterGuide");
            }
        }

        bool ShouldSerializeLocationOnCenterGuide()
        {
            return this.ShouldSerializeProperty("LocationOnCenterGuide");
        }

        /// <summary>
        /// Gets or sets the default image.
        /// </summary>
        [Description("Gets or sets the default image.")]
        public Image Image
        {
            get
            {
                return this.image;
            }
            set
            {
                if (this.image == value)
                {
                    return;
                }

                if (!this.OnPropertyChanging("Image"))
                {
                    return;
                }

                this.image = value;
                this.OnPropertyChanged("Image");
            }
        }

        bool ShouldSerializeImage()
        {
            return this.ShouldSerializeProperty("Image");
        }

        /// <summary>
        /// Gets or sets the hot image (the image to be displayed when the mouse hovers the guide displaying this image).
        /// </summary>
        [Description("Gets or sets the hot image (the image to be displayed when the mouse hovers the guide displaying this image).")]
        public Image HotImage
        {
            get
            {
                return this.hotImage;
            }
            set
            {
                if (this.hotImage == value)
                {
                    return;
                }

                if (!this.OnPropertyChanging("HotImage"))
                {
                    return;
                }

                this.hotImage = value;
                this.OnPropertyChanged("HotImage");
            }
        }

        bool ShouldSerializeHotImage()
        {
            return this.ShouldSerializeProperty("HotImage");
        }

        #endregion

        #region Fields

        private Image image;
        private Image hotImage;
        private Size preferredSize;
        private Point location;
        private bool predefined;

        #endregion
    }
}
