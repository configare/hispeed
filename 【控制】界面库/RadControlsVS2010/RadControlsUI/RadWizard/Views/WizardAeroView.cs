using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Themes.ControlDefault;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a view element of RadWizard in Wizard Aero mode.
    /// </summary>
    public class WizardAeroView : WizardView
    {
        #region Fields

        private WizardAeroTopElement topElement;

        #endregion

        #region Initialization

        static WizardAeroView()
        {
            new ControlDefault_RadWizard_Telerik_WinControls_UI_WizardAeroView().DeserializeTheme();
        }

        /// <summary>
        /// Creates a WizardAeroView instance.
        /// </summary>
        public WizardAeroView()
        { }

        /// <summary>
        /// Creates a WizardAeroView instance.
        /// </summary>
        /// <param name="wizardElement">Owner of the element.</param>
        public WizardAeroView(RadWizardElement wizardElement)
        {
            base.Owner = wizardElement;
            base.CommandArea.Owner = wizardElement;
            this.topElement.Owner = wizardElement;
            base.PageHeaderElement.Owner = wizardElement;
            base.PageHeaderElement.IconElement.Alignment = ContentAlignment.MiddleLeft;

            base.AddPages();
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            base.CommandArea = new WizardCommandArea();
            this.Children.Add(base.CommandArea);

            this.topElement = new WizardAeroTopElement();
            this.Children.Add(this.topElement);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the top element of RadWizard in Wizard Aero mode.
        /// </summary>
        public WizardAeroTopElement TopElement
        {
            get { return this.topElement; }
        }

        /// <summary>
        /// Gets the top element's back button.
        /// </summary>
        public override RadButtonElement BackButton
        {
            get { return topElement.BackButton; }
        }

        /// <summary>
        /// Gets the command area's next button.
        /// </summary>
        public override RadButtonElement NextButton
        {
            get { return base.CommandArea.NextButton; }
        }

        /// <summary>
        /// Gets the command area's cancel button.
        /// </summary>
        public override RadButtonElement CancelButton
        {
            get { return base.CommandArea.CancelButton; }
        }

        /// <summary>
        /// Gets the command area's finish button.
        /// </summary>
        public override RadButtonElement FinishButton
        {
            get { return base.CommandArea.FinishButton; }
        }

        /// <summary>
        /// Gets the command area's help button.
        /// </summary>
        public override LightVisualElement HelpButton
        {
            get { return base.CommandArea.HelpButton; }
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            return base.MeasureOverride(availableSize);
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            if (finalSize.Width < 1 || finalSize.Height < 1)
            {
                return finalSize;
            }

            float topElementX;
            float pageHeaderElementX;
            if (this.RightToLeft)
            {
                topElementX = finalSize.Width - this.topElement.DesiredSize.Width;
                pageHeaderElementX = 0;
            }
            else
            {
                topElementX = 0;
                pageHeaderElementX = this.topElement.DesiredSize.Width;
            }

            float pageHeaderHeight = base.PageHeaderHeight > -1 ? base.PageHeaderHeight : base.PageHeaderElement.DesiredSize.Height;
            if (pageHeaderHeight < this.topElement.DesiredSize.Height)
            {
                pageHeaderHeight = this.topElement.DesiredSize.Height;
            }
            RectangleF topElementRect = new RectangleF(topElementX, 0, this.topElement.DesiredSize.Width, pageHeaderHeight);
            RectangleF pageHeaderElementRect = new RectangleF(pageHeaderElementX, 0, finalSize.Width - this.topElement.DesiredSize.Width, pageHeaderHeight);
            float commandAreaHeight = base.CommandAreaHeight > -1 ? base.CommandAreaHeight : base.CommandArea.DesiredSize.Height;
            RectangleF commandAreaRect = new RectangleF(0, finalSize.Height - commandAreaHeight, finalSize.Width, commandAreaHeight);

            float x = 0;
            if (this.SelectedPage is WizardWelcomePage && !base.HideWelcomeImage)
            {
                x = base.ArrangeImageElement(finalSize, base.WelcomeImageElement, pageHeaderHeight);
            }
            else
            {
                base.WelcomeImageElement.Visibility = ElementVisibility.Collapsed;
            }
            if (this.SelectedPage is WizardCompletionPage && !base.HideCompletionImage)
            {
                x = base.ArrangeImageElement(finalSize, base.CompletionImageElement, pageHeaderHeight);
            }
            else
            {
                base.CompletionImageElement.Visibility = ElementVisibility.Collapsed;
            }

            float pageX;
            if (this.RightToLeft)
            {
                pageX = 0;
            }
            else
            {
                pageX = x;
            }
            RectangleF selectedPageRect = new RectangleF(pageX, pageHeaderHeight, finalSize.Width - x, finalSize.Height - pageHeaderHeight - commandAreaHeight);

            this.topElement.Arrange(topElementRect);
            base.PageHeaderElement.Arrange(pageHeaderElementRect);
            if (this.SelectedPage != null)
            {
                this.SelectedPage.Arrange(selectedPageRect);
                if (this.SelectedPage.ContentArea != null)
                {
                    this.SelectedPage.LocateContentArea();
                }
            }
            base.CommandArea.Arrange(commandAreaRect);

            return finalSize;
        }

        #endregion
    }
}