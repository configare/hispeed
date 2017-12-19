using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a welcome page of RadWizard.
    /// </summary>
    public class WizardWelcomePage : WizardPage
    {
        #region Fields

        private Image welcomeImage;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a WizardWelcomePage instance.
        /// </summary>
        public WizardWelcomePage()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Welcome page image.
        /// </summary>
        [Description("Gets or sets the Welcome page image.")]
        [Category("Header")]
        [Editor(DesignerConsts.RadImageTypeEditorString, typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        [DefaultValue(null)]
        public Image WelcomeImage
        {
            get { return this.welcomeImage; }
            set
            {
                this.welcomeImage = value;
                base.Owner.UpdateImageElements(null);
            }
        }

        #endregion
    }
}