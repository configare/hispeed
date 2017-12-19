using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a top element of RadWizard in Aero mode.
    /// </summary>
    public class WizardAeroTopElement : BaseWizardElement
    {
        #region Fields

        private WizardAeroButtonElement backButton;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a WizardAeroTopElement instance.
        /// </summary>
        public WizardAeroTopElement()
        {
        }

        /// <summary>
        /// Creates a WizardAeroTopElement.
        /// </summary>
        /// <param name="wizardElement">Owner of the element.</param>
        public WizardAeroTopElement(RadWizardElement wizardElement)
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

            this.backButton = new WizardAeroButtonElement();
            this.backButton.UseDefaultDisabledPaint = false;
            this.backButton.MinSize = new Size(28, 28);
            this.backButton.Click += new EventHandler(BackButton_Click);
            this.Children.Add(this.backButton);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the AeroTopElement Back button element.
        /// </summary>
        public WizardAeroButtonElement BackButton
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

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.Owner.PageHeaderHeight > -1)
            {
                return base.MeasureOverride(new SizeF(availableSize.Width, this.Owner.PageHeaderHeight));
            }

            this.backButton.Measure(availableSize);
            float width = this.backButton.DesiredSize.Width + this.Padding.Left + this.Padding.Right;
            float height = this.backButton.DesiredSize.Height + this.Padding.Top + this.Padding.Bottom;

            SizeF desiredSize = new SizeF(width, height);
            return desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF backButtonRect = new RectangleF(0, 0, this.backButton.DesiredSize.Width, this.backButton.DesiredSize.Height);
            this.backButton.Arrange(backButtonRect);

            return finalSize;
        }

        #endregion
    }
}