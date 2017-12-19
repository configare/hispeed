using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Themes.ControlDefault;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a view element of RadWizard in Wizard97 mode.
    /// </summary>
    public class Wizard97View : WizardView
    {
        #region Initialization

        static Wizard97View()
        {
            new ControlDefault_RadWizard_Telerik_WinControls_UI_Wizard97View().DeserializeTheme();
        }

        /// <summary>
        /// Creates a Wizard97View instance.
        /// </summary>
        public Wizard97View()
        { }

        /// <summary>
        /// Creates a Wizard97View instance.
        /// </summary>
        /// <param name="wizardElement">Owner of the element.</param>
        public Wizard97View(RadWizardElement wizardElement)
        {
            base.Owner = wizardElement;
            base.CommandArea.Owner = wizardElement;
            base.PageHeaderElement.Owner = wizardElement;
            base.PageHeaderElement.IconElement.Alignment = ContentAlignment.MiddleRight;

            base.AddPages();
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            base.CommandArea = new Wizard97CommandArea();
            this.Children.Add(base.CommandArea);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the command area's back button.
        /// </summary>
        public override RadButtonElement BackButton
        {
            get
            {
                Wizard97CommandArea commandArea = base.CommandArea as Wizard97CommandArea;
                return commandArea.BackButton;
            }
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

            float pageHeaderHeight = base.PageHeaderHeight > -1 ? base.PageHeaderHeight : base.PageHeaderElement.DesiredSize.Height;
            RectangleF pageHeaderElementRect = new RectangleF(0, 0, finalSize.Width, pageHeaderHeight);
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