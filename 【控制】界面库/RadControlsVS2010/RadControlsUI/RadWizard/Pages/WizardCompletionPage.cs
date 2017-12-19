using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a completion page of RadWizard.
    /// </summary>
    public class WizardCompletionPage : WizardPage
    {
        #region Fields

        private Image completionImage;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a WizardCompletionPage instance.
        /// </summary>
        public WizardCompletionPage()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Completion page image.
        /// </summary>
        [Description("Gets or sets the Completion page image.")]
        [Category("Header")]
        [Editor(DesignerConsts.RadImageTypeEditorString, typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        [DefaultValue(null)]
        public Image CompletionImage
        {
            get { return this.completionImage; }
            set
            {
                this.completionImage = value;
                base.Owner.UpdateImageElements(null);
            }
        }

        #endregion
    }
}