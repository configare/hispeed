using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a page of RadWizard.
    /// </summary>
    public class WizardPage : LightVisualElement
    {
        #region Dependency properties

        public static RadProperty TitleProperty = RadProperty.Register(
            "Title", typeof(string), typeof(WizardPage), new RadElementPropertyMetadata("Page title"));

        public static RadProperty HeaderProperty = RadProperty.Register(
            "Header", typeof(string), typeof(WizardPage), new RadElementPropertyMetadata("Page header"));

        public static RadProperty CustomizePageHeaderProperty = RadProperty.Register(
            "CustomizePageHeader", typeof(bool), typeof(WizardPage), new RadElementPropertyMetadata(false));

        public static RadProperty TitleVisibilityProperty = RadProperty.Register(
            "TitleVisibility", typeof(ElementVisibility), typeof(WizardPage), new RadElementPropertyMetadata(ElementVisibility.Visible));

        public static RadProperty HeaderVisibilityProperty = RadProperty.Register(
            "HeaderVisibility", typeof(ElementVisibility), typeof(WizardPage), new RadElementPropertyMetadata(ElementVisibility.Visible));

        public static RadProperty IconProperty = RadProperty.Register(
            "Icon", typeof(Image), typeof(WizardPage), new RadElementPropertyMetadata(null));

        #endregion

        #region Fields

        private RadWizardElement owner;
        private Panel contentArea;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a WizardPage instance.
        /// </summary>
        public WizardPage()
        {
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            base.Visibility = ElementVisibility.Collapsed;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the owner RadWizardElement of the page.
        /// </summary>
        public RadWizardElement Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                if (this.owner != value)
                {
                    this.owner = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the panel presenting the content area of the page.
        /// </summary>
        public Panel ContentArea
        {
            get { return this.contentArea; }
            set
            {
                this.contentArea = value;
                if (this.contentArea == null)
                {
                    return;
                }

                if (!this.IsSelected)
                {
                    this.contentArea.Visible = false;
                }
                this.contentArea.BackColor = Color.White;
                if (this.ElementTree != null)
                {
                    this.contentArea.Parent = this.ElementTree.Control;
                }
            }
        }

        /// <summary>
        /// Gets or sets the page title text.
        /// </summary>
        [Description("Gets or sets the page title text.")]
        [Category("Header")]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Gets or sets the page header text.
        /// </summary>
        [Description("Gets or sets the page header text.")]
        [Category("Header")]
        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the page customizes its header.
        /// </summary>
        [Description("Gets or sets a value indicating whether the page customizes its header settings.")]
        [Category("Header Settings")]
        [DefaultValue(false)]
        public bool CustomizePageHeader
        {
            get { return (bool)GetValue(CustomizePageHeaderProperty); }
            set { SetValue(CustomizePageHeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the page's TitleElement visibility. Applies if CustomizePageHeader has value 'true'.
        /// </summary>
        [Description("Gets or sets the page's TitleElement visibility. Applies if CustomizePageHeader has value 'true'.")]
        [Category("Header Settings")]
        [DefaultValue(ElementVisibility.Visible)]
        public ElementVisibility TitleVisibility
        {
            get { return (ElementVisibility)GetValue(TitleVisibilityProperty); }
            set { SetValue(TitleVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the page's HeaderElement visibility. Applies if CustomizePageHeader has value 'true'.
        /// </summary>
        [Description("Gets or sets the page's HeaderElement visibility. Applies if CustomizePageHeader has value 'true'.")]
        [Category("Header Settings")]
        [DefaultValue(ElementVisibility.Visible)]
        public ElementVisibility HeaderVisibility
        {
            get { return (ElementVisibility)GetValue(HeaderVisibilityProperty); }
            set { SetValue(HeaderVisibilityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the page's IconElement image. Applies if CustomizePageHeader has value 'true'.
        /// </summary>
        [Description("Gets or sets the page's IconElement image. Applies if CustomizePageHeader has value 'true'.")]
        [Category("Header Settings")]
        [Editor(DesignerConsts.RadImageTypeEditorString, typeof(System.Drawing.Design.UITypeEditor))]
        [TypeConverter(typeof(Telerik.WinControls.Primitives.ImageTypeConverter))]
        [DefaultValue(null)]
        public Image Icon
        {
            get { return (Image)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Gets a value indicating whether the page is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                if (this.owner == null)
                {
                    return false;
                }
                return this == this.owner.SelectedPage;
            }
        }

        #endregion

        #region Methods

        internal virtual void LocateContentArea()
        {
            if (this.contentArea.Width == this.Size.Width && this.contentArea.Height == this.Size.Height &&
                this.contentArea.Location.X == this.BoundingRectangle.X && this.contentArea.Location.Y == this.BoundingRectangle.Y)
            {
                return;
            }

            this.contentArea.Width = this.Size.Width;
            this.contentArea.Height = this.Size.Height;
            this.contentArea.Location = new Point(this.BoundingRectangle.X, this.BoundingRectangle.Y);
        }

        private void Update()
        {
            if (this.Owner != null && this.Owner.SelectedPage == this)
            {
                this.Owner.Refresh();
            }
        }

        #endregion

        #region Overrides

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == TitleProperty || e.Property == HeaderProperty ||
                e.Property == CustomizePageHeaderProperty || e.Property == TitleVisibilityProperty || e.Property == HeaderVisibilityProperty || e.Property == IconProperty)
            {
                this.Update();
            }

            base.OnPropertyChanged(e);
        }

        /// <summary>
        /// Returns a string representation of the page.
        /// </summary>
        /// <returns>The string representation of the page.</returns>
        public override string ToString()
        {
            return string.Format("{0} ({1})", this.Title, this.GetType().Name);
        }

        #endregion
    }
}