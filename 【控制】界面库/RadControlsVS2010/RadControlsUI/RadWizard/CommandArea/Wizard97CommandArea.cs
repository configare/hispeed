using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a command area of RadWizard in Wizard97 mode.
    /// </summary>
    public class Wizard97CommandArea : WizardCommandArea
    {
        #region Fields

        private WizardCommandAreaButtonElement backButton;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a Wizard97CommandArea instance.
        /// </summary>
        public Wizard97CommandArea()
        {
            if (base.CommandElements.Count > 1)
            {
                base.CommandElements.Insert(base.CommandElements.Count - 1, this.backButton);
            }
            else
            {
                base.CommandElements.Add(this.backButton);
            }
        }

        /// <summary>
        /// Creates a Wizard97CommandArea element.
        /// </summary>
        /// <param name="wizardElement">>Owner of the element.</param>
        public Wizard97CommandArea(RadWizardElement wizardElement)
        {
            base.Owner = wizardElement;
        }

        protected override void DisposeManagedResources()
        {
            this.backButton.Click -= new EventHandler(BackButton_Click);
            base.DisposeManagedResources();
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.backButton = new WizardCommandAreaButtonElement();
            this.backButton.Class = "BackButton";
            this.backButton.UseDefaultDisabledPaint = false;
            this.backButton.Text = RadWizardLocalizationProvider.CurrentProvider.GetLocalizedString(RadWizardStringId.BackButtonText);
            this.backButton.Alignment = ContentAlignment.MiddleRight;
            this.backButton.MinSize = new Size(100, 24);
            this.backButton.Click += new EventHandler(BackButton_Click);
            this.Children.Add(this.backButton);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the CommandArea Back button element.
        /// </summary>
        public RadButtonElement BackButton
        {
            get { return this.backButton; }
        }

        #endregion

        #region Event handlers

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Owner.SelectPreviousPage();
        }

        #endregion
    }
}