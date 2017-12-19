using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a command area of RadWizard.
    /// </summary>
    public class WizardCommandArea : BaseWizardElement
    {
        #region Fields

        private List<RadElement> commandElements;

        private WizardCommandAreaButtonElement nextButton;
        private WizardCommandAreaButtonElement cancelButton;
        private WizardCommandAreaButtonElement finishButton;
        private LightVisualElement helpButton;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a WizardCommandArea instance.
        /// </summary>
        public WizardCommandArea()
        {
            this.commandElements = new List<RadElement>();
            this.commandElements.Add(cancelButton);
            this.commandElements.Add(finishButton);
            this.commandElements.Add(nextButton);
            this.commandElements.Add(helpButton);
        }

        /// <summary>
        /// Creates a WizardCommandArea element.
        /// </summary>
        /// <param name="wizardElement">Owner of the element.</param>
        public WizardCommandArea(RadWizardElement wizardElement)
        {
            base.Owner = wizardElement;
        }

        protected override void DisposeManagedResources()
        {
            this.nextButton.Click -= new EventHandler(NextButton_Click);
            base.DisposeManagedResources();
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.nextButton = new WizardCommandAreaButtonElement();
            this.nextButton.Class = "NextButton";
            this.nextButton.UseDefaultDisabledPaint = false;
            this.nextButton.Text = RadWizardLocalizationProvider.CurrentProvider.GetLocalizedString(RadWizardStringId.NextButtonText);
            this.nextButton.Alignment = ContentAlignment.MiddleRight;
            this.nextButton.MinSize = new Size(100, 24);
            this.nextButton.Click += new EventHandler(NextButton_Click);
            this.Children.Add(this.nextButton);

            this.cancelButton = new WizardCommandAreaButtonElement();
            this.cancelButton.Class = "CancelButtont";
            this.cancelButton.UseDefaultDisabledPaint = false;
            this.cancelButton.Text = RadWizardLocalizationProvider.CurrentProvider.GetLocalizedString(RadWizardStringId.CancelButtonText);
            this.cancelButton.Alignment = ContentAlignment.MiddleRight;
            this.cancelButton.MinSize = new Size(100, 24);
            this.Children.Add(this.cancelButton);

            this.finishButton = new WizardCommandAreaButtonElement();
            this.finishButton.Class = "FinishButton";
            this.finishButton.UseDefaultDisabledPaint = false;
            this.finishButton.Text = RadWizardLocalizationProvider.CurrentProvider.GetLocalizedString(RadWizardStringId.FinishButtonText);
            this.finishButton.Alignment = ContentAlignment.MiddleRight;
            this.finishButton.MinSize = new Size(100, 24);
            this.Children.Add(this.finishButton);

            this.helpButton = new BaseWizardElement();
            this.helpButton.Class = "HelpButton";
            this.helpButton.Text = RadWizardLocalizationProvider.CurrentProvider.GetLocalizedString(RadWizardStringId.HelpButtonText);
            this.helpButton.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.helpButton.Alignment = ContentAlignment.MiddleLeft;
            this.helpButton.MinSize = new Size(60, 24);
            this.Children.Add(this.helpButton);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the CommandArea elements.
        /// </summary>
        public List<RadElement> CommandElements
        {
            get { return this.commandElements; }
            internal set { this.commandElements = value; }
        }

        /// <summary>
        /// Gets the CommandArea Next button element.
        /// </summary>
        public RadButtonElement NextButton
        {
            get { return this.nextButton; }
        }

        /// <summary>
        /// Gets the CommandArea Cancel button element.
        /// </summary>
        public RadButtonElement CancelButton
        {
            get { return this.cancelButton; }
        }

        /// <summary>
        /// Gets the CommandArea Finish button element.
        /// </summary>
        public RadButtonElement FinishButton
        {
            get { return this.finishButton; }
        }

        /// <summary>
        /// Gets the CommandArea Help button element.
        /// </summary>
        public LightVisualElement HelpButton
        {
            get { return this.helpButton; }
        }

        #endregion

        #region Event handlers

        private void NextButton_Click(object sender, EventArgs e)
        {
            this.Owner.SelectNextPage();
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            bool heightDefined = this.Owner.CommandAreaHeight > -1;

            float height = heightDefined ? this.Owner.CommandAreaHeight : 0;
            float availableWidth = availableSize.Width - this.Padding.Left - this.Padding.Right;
            foreach (RadElement element in this.commandElements)
            {
                element.Measure(availableSize);
                availableWidth -= element.DesiredSize.Width;
                if (availableWidth < 0)
                {
                    if (this.Children.Contains(element))
                    {
                        this.Children.Remove(element);
                    }
                    continue;
                }
                else if (!this.Children.Contains(element))
                {
                    this.Children.Add(element);
                }

                if (!heightDefined)
                {
                    if (height < element.DesiredSize.Height)
                    {
                        height = element.DesiredSize.Height;
                    }
                }
            }
            if (!heightDefined)
            {
                height += this.Padding.Top + this.Padding.Bottom;
            }

            SizeF desiredSize = new SizeF(availableSize.Width, height);
            return desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            if (this.commandElements.Count < 1)
            {
                return finalSize;
            }

            bool allignRight;

            float x;
            float rightX = finalSize.Width;
            float leftX = 0f;
            bool firstLeftElement = true;
            bool firstRightElement = true;
            float y = 0f;
            foreach (RadElement element in this.commandElements)
            {
                switch (element.Alignment)
                {
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.TopLeft:
                        allignRight = false;
                        break;
                    default:
                        allignRight = true;
                        break;
                }
                if (this.RightToLeft)
                {
                    allignRight = !allignRight;
                }

                if (allignRight)
                {
                    rightX -= element.DesiredSize.Width;
                    if (firstRightElement)
                    {
                        rightX -= this.Padding.Right;
                        firstRightElement = false;
                    }
                    x = rightX;
                }
                else
                {
                    if (firstLeftElement)
                    {
                        leftX = this.Padding.Left + element.Margin.Left;
                        firstLeftElement = false;
                    }
                    else
                    {
                        leftX += element.DesiredSize.Width;
                    }
                    x = leftX;
                }
                y = (finalSize.Height - element.DesiredSize.Height) / 2;
                RectangleF rect = new RectangleF(x, y, element.DesiredSize.Width, element.DesiredSize.Height);
                element.Arrange(rect);
            }

            return finalSize;
        }

        #endregion
    }
}