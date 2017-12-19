using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Base class for RadWizard view elements.
    /// </summary>
    public abstract class WizardView : LightVisualElement
    {
        #region Dependency properties

        public static RadProperty CommandAreaHeightProperty = RadProperty.Register(
            "CommandAreaHeight", typeof(float), typeof(WizardView), new RadElementPropertyMetadata(-1f));

        public static RadProperty PageHeaderHeightProperty = RadProperty.Register(
            "PageHeaderHeight", typeof(float), typeof(WizardView), new RadElementPropertyMetadata(-1f));

        public static RadProperty HideWelcomeImageProperty = RadProperty.Register(
            "HideWelcomeImage", typeof(bool), typeof(WizardView), new RadElementPropertyMetadata(false));

        public static RadProperty HideCompletionImageProperty = RadProperty.Register(
            "HideCompletionImage", typeof(bool), typeof(WizardView), new RadElementPropertyMetadata(false));

        #endregion

        #region Fields

        private RadWizardElement owner;

        private WizardCommandArea commandArea;
        private WizardPageHeaderElement pageHeaderElement;

        private LightVisualElement welcomeImageElement;
        private Image welcomeImage;
        private LightVisualElement completionImageElement;
        private Image completionImage;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a WizardView instance.
        /// </summary>
        public WizardView()
        {
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.pageHeaderElement = new WizardPageHeaderElement();
            this.Children.Add(this.pageHeaderElement);

            this.welcomeImageElement = new LightVisualElement();
            this.welcomeImageElement.Class = "WelcomeImage";
            this.welcomeImageElement.ImageAlignment = ContentAlignment.TopCenter;
            this.welcomeImageElement.ImageLayout = ImageLayout.None;
            this.Children.Add(this.welcomeImageElement);

            this.completionImageElement = new LightVisualElement();
            this.completionImageElement.Class = "CompletionImage";
            this.completionImageElement.ImageAlignment = ContentAlignment.TopCenter;
            this.completionImageElement.ImageLayout = ImageLayout.None;
            this.Children.Add(this.completionImageElement);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the owner RadWizardElement of the view.
        /// </summary>
        public RadWizardElement Owner
        {
            get { return this.owner; }
            internal set { this.owner = value; }
        }


        /// <summary>
        /// Gets the pages collection of the Owner RadWizardElement.
        /// </summary>
        public WizardPageCollection Pages
        {
            get { return this.Owner.Pages; }
        }

        /// <summary>
        /// Gets the welcome page of the Owner RadWizardElement.
        /// </summary>
        public WizardWelcomePage WelcomePage
        {
            get { return this.Owner.WelcomePage; }
        }

        /// <summary>
        /// Gets the completion page of the Owner RadWizardElement.
        /// </summary>
        public WizardCompletionPage CompletionPage
        {
            get { return this.Owner.CompletionPage; }
        }

        /// <summary>
        /// Gets the selected page of the Owner RadWizardElement.
        /// </summary>
        public WizardPage SelectedPage
        {
            get { return this.Owner.SelectedPage; }
        }


        /// <summary>
        /// Gets the command area of the view.
        /// </summary>
        public WizardCommandArea CommandArea
        {
            get { return this.commandArea; }
            internal set { this.commandArea = value; }
        }

        /// <summary>
        /// Gets or sets the height of the command area. Negative value makes the command area autosize.
        /// </summary>
        public float CommandAreaHeight
        {
            get { return (float)GetValue(CommandAreaHeightProperty); }
            set { SetValue(CommandAreaHeightProperty, value); }
        }

        /// <summary>
        /// Gets the page header of the view.
        /// </summary>
        public WizardPageHeaderElement PageHeaderElement
        {
            get { return this.pageHeaderElement; }
        }

        /// <summary>
        /// Gets or sets the height of the page header. Negative value makes the page header autosize.
        /// </summary>
        public float PageHeaderHeight
        {
            get { return (float)GetValue(PageHeaderHeightProperty); }
            set { SetValue(PageHeaderHeightProperty, value); }
        }


        /// <summary>
        /// Gets the element containing the image of the welcome pages.
        /// </summary>
        public LightVisualElement WelcomeImageElement
        {
            get { return this.welcomeImageElement; }
        }

        /// <summary>
        /// Gets or sets the image of the welcome pages.
        /// </summary>
        public Image WelcomeImage
        {
            get { return this.welcomeImage; }
            set { this.welcomeImage = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the image of the welcome pages should be visible.
        /// </summary>
        public bool HideWelcomeImage
        {
            get { return (bool)GetValue(HideWelcomeImageProperty); }
            set { SetValue(HideWelcomeImageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the layout of the welcome pages image.
        /// </summary>
        public ImageLayout WelcomeImageLayout
        {
            get { return this.welcomeImageElement.ImageLayout; }
            set { this.welcomeImageElement.ImageLayout = value; }
        }

        /// <summary>
        /// Gets or sets the background image shape of the welcome pages.
        /// </summary>
        public RadImageShape WelcomeImageBackgroundShape
        {
            get { return this.welcomeImageElement.BackgroundShape; }
            set { this.welcomeImageElement.BackgroundShape = value; }
        }

        /// <summary>
        /// Gets the element containing the image of the welcome pages.
        /// </summary>
        public LightVisualElement CompletionImageElement
        {
            get { return this.completionImageElement; }
        }

        /// <summary>
        /// Gets or sets the image of the completion pages.
        /// </summary>
        public Image CompletionImage
        {
            get { return this.completionImage; }
            set { this.completionImage = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the image of the completion pages should be visible.
        /// </summary>
        public bool HideCompletionImage
        {
            get { return (bool)GetValue(HideCompletionImageProperty); }
            set { SetValue(HideCompletionImageProperty, value); }
        }

        /// <summary>
        /// Gets or sets the layout of the completion pages image.
        /// </summary>
        public ImageLayout CompletionImageLayout
        {
            get { return this.completionImageElement.ImageLayout; }
            set { this.completionImageElement.ImageLayout = value; }
        }

        /// <summary>
        /// Gets or sets the background image shape of the completion pages.
        /// </summary>
        public RadImageShape CompletionImageBackgroundShape
        {
            get { return this.completionImageElement.BackgroundShape; }
            set { this.completionImageElement.BackgroundShape = value; }
        }


        /// <summary>
        /// Gets or sets the visibility of the page header's title element.
        /// </summary>
        public ElementVisibility PageTitleTextVisibility
        {
            get { return this.pageHeaderElement.TitleVisibility; }
            set { this.pageHeaderElement.TitleVisibility = value; }
        }

        /// <summary>
        /// Gets or sets the visibility of the page header's header element.
        /// </summary>
        public ElementVisibility PageHeaderTextVisibility
        {
            get { return this.pageHeaderElement.HeaderVisibility; }
            set { this.pageHeaderElement.HeaderVisibility = value; }
        }

        /// <summary>
        /// Gets or sets the icon of the page header.
        /// </summary>
        public Image PageHeaderIcon
        {
            get { return this.pageHeaderElement.Icon; }
            set { this.pageHeaderElement.Icon = value; }
        }

        /// <summary>
        /// Gets or sets the alignment of the page header's icon.
        /// </summary>
        public ContentAlignment PageHeaderIconAlignment
        {
            get { return this.pageHeaderElement.IconAlignment; }
            set { this.pageHeaderElement.IconAlignment = value; }
        }


        /// <summary>
        /// Gets the command area's back button.
        /// </summary>
        public virtual RadButtonElement BackButton
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the command area's next button.
        /// </summary>
        public virtual RadButtonElement NextButton
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the command area's cancel button.
        /// </summary>
        public virtual RadButtonElement CancelButton
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the command area's finish button.
        /// </summary>
        public virtual RadButtonElement FinishButton
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the command area's help button.
        /// </summary>
        public virtual LightVisualElement HelpButton
        {
            get { return null; }
        }

        #endregion

        #region Methods

        protected internal virtual void AddPages()
        {
            foreach (WizardPage page in this.Owner.Pages)
            {
                this.Children.Add(page);
            } 
        }

        internal float ArrangeImageElement(SizeF finalSize, LightVisualElement imageElement, float pageHeaderHeight)
        {
            float backgroundShapeWidth = imageElement.BackgroundShape != null ? imageElement.BackgroundShape.Image.Size.Width : 0;
            float width = imageElement.Image != null ? imageElement.DesiredSize.Width : backgroundShapeWidth;
            float x;
            if (this.RightToLeft)
            {
                x = finalSize.Width - width;
            }
            else
            {
                x = 0;
            }
            float commandAreaHeight = this.CommandAreaHeight > -1 ? this.CommandAreaHeight : this.commandArea.DesiredSize.Height;
            RectangleF imageRect = new RectangleF(x, pageHeaderHeight, width, finalSize.Height - pageHeaderHeight - commandAreaHeight);
            imageElement.Visibility = ElementVisibility.Visible;
            imageElement.Arrange(imageRect);

            return width;
        }

        #endregion

        #region Overrides

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == CommandAreaHeightProperty || e.Property == PageHeaderHeightProperty || e.Property == HideWelcomeImageProperty || e.Property == HideCompletionImageProperty)
            {
                this.Owner.InvalidateMeasure(true);
            }

            base.OnPropertyChanged(e);
        }

        #endregion
    }
}