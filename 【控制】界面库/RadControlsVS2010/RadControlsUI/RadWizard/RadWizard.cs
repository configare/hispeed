using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Drawing;
using System.Runtime.InteropServices;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// RadWizard is a control which helps you to break a complex process into separate steps.
    /// </summary>
    [Designer(DesignerConsts.RadWizardDesignerString)]
    [ToolboxItem(true)]
    public class RadWizard : RadControl
    {
        #region Fields

        private RadWizardElement wizardElement;
        private Form parentForm;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a RadWizard instance.
        /// </summary>
        public RadWizard()
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            UnWireEvents();
        }

        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);

            this.wizardElement = new RadWizardElement();
            this.wizardElement.OwnerControl = this;
            this.WireEvents();

            parent.Children.Add(wizardElement);
        }

        protected internal virtual void WireEvents()
        {
            if (this.wizardElement == null || this.wizardElement.View == null)
            {
                return;
            }

            this.wizardElement.ModeChanging += new ModeChangingEventHandler(wizardElement_ModeChanging);
            this.wizardElement.ModeChanged += new ModeChangedEventHandler(wizardElement_ModeChanged);
            this.wizardElement.Next += new WizardCancelEventHandler(wizardElement_Next);
            this.wizardElement.Previous += new WizardCancelEventHandler(wizardElement_Previous);
            this.wizardElement.FinishButton.Click += new EventHandler(FinishButton_Click);
            this.wizardElement.CancelButton.Click += new EventHandler(CancelButton_Click);
            this.wizardElement.HelpButton.Click += new EventHandler(HelpButton_Click);
            this.wizardElement.SelectedPageChanging += new SelectedPageChangingEventHandler(wizardElement_SelectedPageChanging);
            this.wizardElement.SelectedPageChanged += new SelectedPageChangedEventHandler(wizardElement_SelectedPageChanged);

            this.ParentChanged += new EventHandler(RadWizard_ParentChanged);
        }

        private void RadWizard_ParentChanged(object sender, EventArgs e)
        {
            if (this.Parent == null)
            {
                return;
            }
            this.parentForm = this.FindForm();
        }

        protected internal virtual void UnWireEvents()
        {
            if (this.wizardElement == null || this.wizardElement.View == null)
            {
                return;
            }

            this.wizardElement.ModeChanging -= new ModeChangingEventHandler(wizardElement_ModeChanging);
            this.wizardElement.ModeChanged -= new ModeChangedEventHandler(wizardElement_ModeChanged);
            this.wizardElement.Next -= new WizardCancelEventHandler(wizardElement_Next);
            this.wizardElement.Previous -= new WizardCancelEventHandler(wizardElement_Previous);
            this.wizardElement.FinishButton.Click -= new EventHandler(FinishButton_Click);
            this.wizardElement.CancelButton.Click -= new EventHandler(CancelButton_Click);
            this.wizardElement.HelpButton.Click -= new EventHandler(HelpButton_Click);
            this.wizardElement.SelectedPageChanging -= new SelectedPageChangingEventHandler(wizardElement_SelectedPageChanging);
            this.wizardElement.SelectedPageChanged -= new SelectedPageChangedEventHandler(wizardElement_SelectedPageChanged);

            this.ParentChanged -= new EventHandler(RadWizard_ParentChanged);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the RadWizardElement which encapsulates the UI representation and functionality of the control.
        /// </summary>
        [Browsable(false)]
        public RadWizardElement WizardElement
        {
            get { return this.wizardElement; }
        }

        /// <summary>
        /// Gets or sets the mode of the control.
        /// </summary>
        [Description("The mode of RadWizard.")]
        [Browsable(true)]
        public WizardMode Mode
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return WizardMode.Wizard97;
                }

                return this.WizardElement.Mode;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.Mode = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indication wether the Aero style should apply when the control is in Wizard Aero mode.
        /// </summary>
        [Description("Indication wether the Aero style should apply when the control is in Wizard Aero mode.")]
        [DefaultValue(true)]
        public bool EnableAeroStyle
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return false;
                }

                return this.WizardElement.EnableAeroStyle;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.EnableAeroStyle = value;
            }
        }

        /// <summary>
        /// Gets the pages collection.
        /// </summary>
        [Description("The pages collection of RadWizad.")]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public WizardPageCollection Pages
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.Pages;
            }
        }

        /// <summary>
        /// Gets or sets the welcome page.
        /// </summary>
        [Description("The Welcome page of RadWizard.")]
        public WizardWelcomePage WelcomePage
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.WelcomePage;
            }
            set
            {

                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.WelcomePage = value;
            }
        }

        /// <summary>
        /// Gets or sets the completion page.
        /// </summary>
        [Description("The Completion page of RadWizad.")]
        public WizardCompletionPage CompletionPage
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.CompletionPage;
            }
            set
            {

                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.CompletionPage = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected page.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WizardPage SelectedPage
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.SelectedPage;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.SelectedPage = value;
            }
        }


        /// <summary>
        /// Gets the command area element.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WizardCommandArea CommandArea
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.CommandArea;
            }
        }

        /// <summary>
        /// Gets or sets the height of the command area. Negative value makes the command area autosize.
        /// </summary>
        [Description("The height of the command area. Negative value makes the command area autosize.")]
        [DefaultValue(-1f)]
        public float CommandAreaHeight
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return -1;
                }

                return this.wizardElement.CommandAreaHeight;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.CommandAreaHeight = value;
            }
        }

        /// <summary>
        /// Gets the page header element.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WizardPageHeaderElement PageHeaderElement
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.PageHeaderElement;
            }
        }

        /// <summary>
        /// Gets or sets the height of the page header. Negative value makes the page header autosize.
        /// </summary>
        [Description("The height of the page header. Negative value makes the page header autosize.")]
        [DefaultValue(-1f)]
        public float PageHeaderHeight
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return -1;
                }

                return this.wizardElement.PageHeaderHeight;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.PageHeaderHeight = value;
            }
        }

        /// <summary>
        /// Gets the element containing the image of the welcome pages.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LightVisualElement WelcomeImageElement
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.WelcomeImageElement;
            }
        }

        /// <summary>
        /// Gets the element containing the image of the completion pages.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LightVisualElement CompletionImageElement
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.CompletionImageElement;
            }
        }


        /// <summary>
        /// Gets or sets the image of the welcome pages.
        /// </summary>
        [DefaultValue(null)]
        [Description("The image of welcome pages.")]
        public Image WelcomeImage
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.WelcomeImage;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.WelcomeImage = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the image of the welcome pages should be visible.
        /// </summary>
        [DefaultValue(false)]
        [Description("Indicates whether the image of the welcome pages should be visible.")]
        public bool HideWelcomeImage
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return false;
                }

                return this.wizardElement.HideWelcomeImage;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.HideWelcomeImage = value;
            }
        }

        /// <summary>
        /// Gets or sets the layout of the welcome pages image.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ImageLayout WelcomeImageLayout
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return ImageLayout.None;
                }

                return this.wizardElement.WelcomeImageLayout;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.WelcomeImageLayout = value;
            }
        }

        /// <summary>
        /// Gets or sets the background image shape of the welcome pages.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadImageShape WelcomeImageBackgroundShape
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.WelcomeImageBackgroundShape;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.WelcomeImageBackgroundShape = value;
            }
        }

        /// <summary>
        /// Gets or sets the image of the completion pages.
        /// </summary>
        [DefaultValue(null)]
        [Description("The image of completion pages.")]
        public Image CompletionImage
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.CompletionImage;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.CompletionImage = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the image of the completion pages should be visible.
        /// </summary>
        [DefaultValue(false)]
        [Description("Indicates whether the image of the completion pages should be visible.")]
        public bool HideCompletionImage
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return false;
                }

                return this.wizardElement.HideCompletionImage;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.HideCompletionImage = value;
            }
        }

        /// <summary>
        /// Gets or sets the layout of the completion pages image.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ImageLayout CompletionImageLayout
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return ImageLayout.None;
                }

                return this.wizardElement.CompletionImageLayout;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.CompletionImageLayout = value;
            }
        }

        /// <summary>
        /// Gets or sets the background image shape of the completion pages.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadImageShape CompletionImageBackgroundShape
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.CompletionImageBackgroundShape;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.CompletionImageBackgroundShape = value;
            }
        }


        /// <summary>
        /// Gets or sets the visibility of the page header's title element.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ElementVisibility PageTitleTextVisibility
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return ElementVisibility.Hidden;
                }

                return this.wizardElement.PageTitleTextVisibility;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.PageTitleTextVisibility = value;
            }
        }

        /// <summary>
        /// Gets or sets the visibility of the page header's header element.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ElementVisibility PageHeaderTextVisibility
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return ElementVisibility.Hidden;
                }

                return this.wizardElement.PageHeaderTextVisibility;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.PageHeaderTextVisibility = value;
            }
        }

        /// <summary>
        /// Gets or sets the icon of the page header.
        /// </summary>
        [Description("The icon of the page header.")]
        public Image PageHeaderIcon
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.PageHeaderIcon;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.PageHeaderIcon = value;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the page header's icon.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ContentAlignment PageHeaderIconAlignment
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return ContentAlignment.MiddleCenter;
                }

                return this.wizardElement.PageHeaderIconAlignment;
            }
            set
            {
                if (this.wizardElement == null)
                {
                    return;
                }

                this.wizardElement.PageHeaderIconAlignment = value;
            }
        }


        /// <summary>
        /// Gets the command area's back button.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadButtonElement BackButton
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.BackButton;
            }
        }

        /// <summary>
        /// Gets the command area's next button.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadButtonElement NextButton
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.NextButton;
            }
        }

        /// <summary>
        /// Gets the command area's cancel button.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadButtonElement CancelButton
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.CancelButton;
            }
        }

        /// <summary>
        /// Gets the command area's finish button.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadButtonElement FinishButton
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.FinishButton;
            }
        }

        /// <summary>
        /// Gets the command area's help button.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public LightVisualElement HelpButton
        {
            get
            {
                if (this.wizardElement == null)
                {
                    return null;
                }

                return this.wizardElement.HelpButton;
            }
        }


        protected override Size DefaultSize
        {
            get
            {
                return new Size(600, 400);
            }
        }

        #endregion

        #region Methods

        internal void ApplyAeroStyle()
        {
            if (!DWMAPI.IsCompositionEnabled || this.IsDesignMode || this.parentForm == null)
            {
                return;
            }

            RadForm radForm = this.parentForm as RadForm;
            if (radForm != null)
            {
                radForm.AllowTheming = false;
                radForm.RootElement.BackColor = Color.Transparent;
            }
            else
            {
                this.parentForm.BackColor = Color.Black;
            }
            this.RootElement.BackColor = Color.Transparent;

            IntPtr hwnd = this.parentForm.Handle;
            NativeMethods.MARGINS margins = new NativeMethods.MARGINS();
            margins.cxLeftWidth = 0;
            margins.cxRightWidth = 0;
            if (this.Dock != DockStyle.Fill || this.Pages.Count == 0)
            {
                margins.cyTopHeight = this.parentForm.Size.Height;
            }
            else
            {
                margins.cyTopHeight = this.PageHeaderElement.Size.Height;
            }
            margins.cyBottomHeight = 0;
            DWMAPI.DwmExtendFrameIntoClientArea(hwnd, ref margins);

            this.parentForm.Refresh();
        }

        internal void UnapplyAeroStyle()
        {
            if (!DWMAPI.IsCompositionEnabled || this.IsDesignMode || this.parentForm == null)
            {
                return;
            }

            RadForm radForm = this.parentForm as RadForm;
            if (radForm != null)
            {
                radForm.AllowTheming = true;
                radForm.RootElement.ResetValue(VisualElement.BackColorProperty, ValueResetFlags.Local);
            }

            IntPtr hwnd = this.parentForm.Handle;
            NativeMethods.MARGINS margins = new NativeMethods.MARGINS();
            margins.cxLeftWidth = 0;
            margins.cxRightWidth = 0;
            margins.cyTopHeight = 0;
            margins.cyBottomHeight = 0;
            DWMAPI.DwmExtendFrameIntoClientArea(hwnd, ref margins);

            this.parentForm.Refresh();
        }

        /// <summary>
        /// Selects next wizard page.
        /// </summary>
        public void SelectNextPage()
        {
            if (this.wizardElement != null)
            {
                this.wizardElement.SelectNextPage();
            }
        }

        /// <summary>
        /// Selects previous wizard page.
        /// </summary>
        public void SelectPreviousPage()
        {
            if (this.wizardElement != null)
            {
                this.wizardElement.SelectPreviousPage();
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Fires before the mode of RadWizard is changed.
        /// </summary>
        public event ModeChangingEventHandler ModeChanging;

        /// <summary>
        /// Raises the <see cref="ModeChanging"/> event.
        /// </summary>
        /// <param name="e">An instance of <see cref="ModeChangingEventArgs"/> containing event data.</param>
        protected virtual void OnModeChanging(ModeChangingEventArgs e)
        {
            if (this.ModeChanging != null)
            {
                this.ModeChanging(this, e);
            }
        }

        private void wizardElement_ModeChanging(object sender, ModeChangingEventArgs e)
        {
            this.OnModeChanging(e);
        }

        /// <summary>
        /// Fires after the mode of RadWizard is changed.
        /// </summary>
        public event ModeChangedEventHandler ModeChanged;

        /// <summary>
        /// Raises the <see cref="ModeChanged"/> event.
        /// </summary>
        /// <param name="e">An instance of <see cref="ModeChangedEventArgs"/> containing event data.</param>
        protected virtual void OnModeChanged(ModeChangedEventArgs e)
        {
            if (this.ModeChanged != null)
            {
                this.ModeChanged(this, e);
            }
        }

        private void wizardElement_ModeChanged(object sender, ModeChangedEventArgs e)
        {
            this.OnModeChanged(e);
        }

        /// <summary>
        /// Fires when the next command button is clicked.
        /// </summary>
        public event WizardCancelEventHandler Next;

        /// <summary>
        /// Raises the <see cref="Next"/> event.
        /// </summary>
        /// <param name="e">An instance of <see cref="WizardCancelEventArgs"/> containing event data.</param>
        protected virtual void OnNext(WizardCancelEventArgs e)
        {
            if (this.Next != null)
            {
                this.Next(this, e);
            }
        }

        private void wizardElement_Next(object sender, WizardCancelEventArgs e)
        {
            this.OnNext(e);
        }

        /// <summary>
        /// Fires when the back command button is clicked.
        /// </summary>
        public event WizardCancelEventHandler Previous;

        /// <summary>
        /// Raises the <see cref="Previous"/> event.
        /// </summary>
        /// <param name="e">An instance of <see cref="WizardCancelEventArgs"/> containing event data.</param>
        protected virtual void OnPrevious(WizardCancelEventArgs e)
        {
            if (this.Previous != null)
            {
                this.Previous(this, e);
            }
        }

        private void wizardElement_Previous(object sender, WizardCancelEventArgs e)
        {
            this.OnPrevious(e);
        }

        /// <summary>
        /// Fires when the finish command button is clicked.
        /// </summary>
        public event EventHandler Finish;

        /// <summary>
        /// Raises the <see cref="Finish"/> event.
        /// </summary>
        /// <param name="e">An instance of <see cref="EventArgs"/>.</param>
        protected virtual void OnFinish(EventArgs e)
        {
            if (this.Finish != null)
            {
                this.Finish(this, e);
            }
        }

        private void FinishButton_Click(object sender, EventArgs e)
        {
            this.OnFinish(e);
        }

        /// <summary>
        /// Fires when the cancel command button is clicked.
        /// </summary>
        public event EventHandler Cancel;

        /// <summary>
        /// Raises the <see cref="Cancel"/> event.
        /// </summary>
        /// <param name="e">An instance of <see cref="EventArgs"/>.</param>
        protected virtual void OnCancel(EventArgs e)
        {
            if (this.Cancel != null)
            {
                this.Cancel(this, e);
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.OnCancel(e);
        }

        /// <summary>
        /// Fires when the help command button is clicked.
        /// </summary>
        public event EventHandler Help;

        /// <summary>
        /// Raises the <see cref="Help"/> event.
        /// </summary>
        /// <param name="e">An instance of <see cref="EventArgs"/>.</param>
        protected virtual void OnHelp(EventArgs e)
        {
            if (this.Help != null)
            {
                this.Help(this, e);
            }
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            this.OnHelp(e);
        }

        /// <summary>
        /// Fires before the selected page of RadWizard is changed.
        /// </summary>
        public event SelectedPageChangingEventHandler SelectedPageChanging;

        /// <summary>
        /// Raises the <see cref="SelectedPageChanging"/> event.
        /// </summary>
        /// <param name="sender">The owner.</param>
        /// <param name="e">An instance of <see cref="SelectedPageChangingEventArgs"/> containing event data.</param>
        protected virtual void OnSelectedPageChanging(object sender, SelectedPageChangingEventArgs e)
        {
            if (this.SelectedPageChanging != null)
            {
                this.SelectedPageChanging(this, e);
            }
        }

        private void wizardElement_SelectedPageChanging(object sender, SelectedPageChangingEventArgs e)
        {
            this.OnSelectedPageChanging(this, e);
        }

        /// <summary>
        /// Fires after the selected page of RadWizard is changed.
        /// </summary>
        public event SelectedPageChangedEventHandler SelectedPageChanged;

        /// <summary>
        /// Raises the <see cref="SelectedPageChanged"/> event.
        /// </summary>
        /// <param name="sender">The owner.</param>
        /// <param name="e">An instance of <see cref="SelectedPageChangedEventArgs"/> containing event data.</param>
        protected virtual void OnSelectedPageChanged(object sender, SelectedPageChangedEventArgs e)
        {
            if (this.SelectedPageChanged != null)
            {
                this.SelectedPageChanged(this, e);
            }
        }

        private void wizardElement_SelectedPageChanged(object sender, SelectedPageChangedEventArgs e)
        {
            this.OnSelectedPageChanged(this, e);
        }

        #endregion
    }
}