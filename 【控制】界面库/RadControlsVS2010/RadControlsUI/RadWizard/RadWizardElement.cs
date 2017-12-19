using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using Telerik.WinControls.Data;
using System.Windows.Forms;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Encapsulates the UI representation and functionality of RadWizard.
    /// </summary>
    public class RadWizardElement : LightVisualElement
    {
        #region Dependency properties

        public static RadProperty EnableAeroStyleProperty = RadProperty.Register(
            "EnableAeroStyle", typeof(bool), typeof(RadWizardElement), new RadElementPropertyMetadata(true));

        #endregion

        #region Fields

        private WizardMode mode;
        private WizardView view;
        private RadWizard ownerControl;

        private WizardPageCollection pages;
        private WizardWelcomePage welcomePage;
        private WizardCompletionPage completionPage;
        private WizardPage selectedPage;

        private bool pageHeaderCustomized;

        #endregion

        #region Initialization

        /// <summary>
        /// Creates a RadWizardElement instance.
        /// </summary>
        public RadWizardElement()
        {
            this.pages.CollectionChanging += new NotifyCollectionChangingEventHandler(Pages_CollectionChanging);
            this.pages.CollectionChanged += new NotifyCollectionChangedEventHandler(Pages_CollectionChanged);
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.pages = new WizardPageCollection(this);

            this.mode = WizardMode.Wizard97;
            this.UpdateView(this.mode);

            this.pageHeaderCustomized = false;
        }

        protected override void DisposeManagedResources()
        {
            this.pages.CollectionChanging -= new NotifyCollectionChangingEventHandler(Pages_CollectionChanging);
            this.pages.CollectionChanged -= new NotifyCollectionChangedEventHandler(Pages_CollectionChanged);
            base.DisposeManagedResources();
        }

        protected internal override void OnParentPropertyChanged(RadPropertyChangedEventArgs args)
        {
            base.OnParentPropertyChanged(args);

            if (this.ElementTree != null && this.selectedPage != null && this.selectedPage.ContentArea != null && this.selectedPage.ContentArea.Parent != this.ElementTree.Control)
            {
                this.selectedPage.ContentArea.Parent = this.ElementTree.Control;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the mode of RadWizard.
        /// </summary>
        public WizardMode Mode
        {
            get { return this.mode; }
            set
            {
                if (this.mode == value)
                {
                    return;
                }

                ModeChangingEventArgs changingEventArgs = new ModeChangingEventArgs(this.mode, value);
                this.OnModeChanging(changingEventArgs);
                if (changingEventArgs.Cancel)
                {
                    return;
                }

                WizardMode previousMode = this.mode;
                this.mode = value;
                this.UpdateView(this.mode);

                ModeChangedEventArgs changedEventArgs = new ModeChangedEventArgs(previousMode, this.mode);
                this.OnModeChanged(changedEventArgs);
            }
        }

        /// <summary>
        /// Gets the view of RadWizard.
        /// </summary>
        public WizardView View
        {
            get { return this.view; }
        }

        /// <summary>
        /// Gets the Owner RadWizard control.
        /// </summary>
        public RadWizard OwnerControl
        {
            get { return this.ownerControl; }
            internal set { this.ownerControl = value; }
        }

        /// <summary>
        /// Gets or sets a value indication wether the Aero style should apply when RadWizard is in Wizard Aero mode.
        /// </summary>
        public bool EnableAeroStyle
        {
            get { return (bool)GetValue(EnableAeroStyleProperty); }
            set { SetValue(EnableAeroStyleProperty, value); }
        }


        /// <summary>
        /// Gets the pages collection.
        /// </summary>
        public WizardPageCollection Pages
        {
            get { return this.pages; }
        }

        /// <summary>
        /// Gets or sets the welcome page.
        /// </summary>
        public WizardWelcomePage WelcomePage
        {
            get { return this.welcomePage; }
            set { this.welcomePage = value; }
        }

        /// <summary>
        /// Gets or sets the completion page.
        /// </summary>
        public WizardCompletionPage CompletionPage
        {
            get { return this.completionPage; }
            set { this.completionPage = value; }
        }

        /// <summary>
        /// Gets or sets the selected page.
        /// </summary>
        public WizardPage SelectedPage
        {
            get { return this.selectedPage; }
            set
            {
                SelectedPageChangingEventArgs changingEventArgs = new SelectedPageChangingEventArgs(this.selectedPage, value);
                this.OnSelectedPageChanging(this, changingEventArgs);
                if (changingEventArgs.Cancel)
                {
                    return;
                }

                if (this.selectedPage != null)
                {
                    this.selectedPage.Visibility = ElementVisibility.Collapsed;
                    if (this.selectedPage.ContentArea != null)
                    {
                        this.selectedPage.ContentArea.Visible = false;
                    }
                }
                if (value == null || value.Owner != this)
                {
                    this.selectedPage = null;
                    this.UpdateView(this.selectedPage);
                    this.InvalidateMeasure(true);
                    return;
                }

                WizardPage previousPage = this.selectedPage;
                this.selectedPage = value;
                if (this.selectedPage.ContentArea != null)
                {
                    this.selectedPage.ContentArea.Visible = true;
                    if (this.ElementTree != null && this.selectedPage.ContentArea.Parent != this.ElementTree.Control)
                    {
                        this.selectedPage.ContentArea.Parent = this.ElementTree.Control;
                    }
                }

                this.UpdateView(this.selectedPage);
                this.selectedPage.Visibility = ElementVisibility.Visible;
                this.InvalidateMeasure(true);

                SelectedPageChangedEventArgs changedEventArgs = new SelectedPageChangedEventArgs(previousPage, this.selectedPage);
                this.OnSelectedPageChanged(this, changedEventArgs);
            }
        }


        /// <summary>
        /// Gets the command area element.
        /// </summary>
        public WizardCommandArea CommandArea
        {
            get { return this.view.CommandArea; }
        }

        /// <summary>
        /// Gets or sets the height of the command area. Negative value makes the command area autosize.
        /// </summary>
        public float CommandAreaHeight
        {
            get { return this.view.CommandAreaHeight; }
            set { this.view.CommandAreaHeight = value; }
        }

        /// <summary>
        /// Gets the page header element.
        /// </summary>
        public WizardPageHeaderElement PageHeaderElement
        {
            get { return this.view.PageHeaderElement; }
        }

        /// <summary>
        /// Gets or sets the height of the page header. Negative value makes the page header autosize.
        /// </summary>
        public float PageHeaderHeight
        {
            get { return this.view.PageHeaderHeight; }
            set { this.view.PageHeaderHeight = value; }
        }

        /// <summary>
        /// Gets the element containing the image of the welcome pages.
        /// </summary>
        public LightVisualElement WelcomeImageElement
        {
            get { return this.view.WelcomeImageElement; }
        }

        /// <summary>
        /// Gets the element containing the image of the completion pages.
        /// </summary>
        public LightVisualElement CompletionImageElement
        {
            get { return this.view.CompletionImageElement; }
        }


        /// <summary>
        /// Gets or sets the image of the welcome pages.
        /// </summary>
        public Image WelcomeImage
        {
            get { return this.view.WelcomeImage; }
            set
            {
                this.view.WelcomeImage = value;
                this.UpdateImageElements(this.selectedPage);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the image of the welcome pages should be visible.
        /// </summary>
        public bool HideWelcomeImage
        {
            get { return this.view.HideWelcomeImage; }
            set { this.view.HideWelcomeImage = value; }
        }

        /// <summary>
        /// Gets or sets the layout of the welcome pages image.
        /// </summary>
        public ImageLayout WelcomeImageLayout
        {
            get { return this.view.WelcomeImageLayout; }
            set { this.view.WelcomeImageLayout = value; }
        }

        /// <summary>
        /// Gets or sets the background image shape of the welcome pages.
        /// </summary>
        public RadImageShape WelcomeImageBackgroundShape
        {
            get { return this.view.WelcomeImageBackgroundShape; }
            set { this.view.WelcomeImageBackgroundShape = value; }
        }

        /// <summary>
        /// Gets or sets the image of the completion pages.
        /// </summary>
        public Image CompletionImage
        {
            get { return this.view.CompletionImage; }
            set
            {
                this.view.CompletionImage = value;
                this.UpdateImageElements(this.selectedPage);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the image of the completion pages should be visible.
        /// </summary>
        public bool HideCompletionImage
        {
            get { return this.view.HideCompletionImage; }
            set { this.view.HideCompletionImage = value; }
        }

        /// <summary>
        /// Gets or sets the layout of the completion pages image.
        /// </summary>
        public ImageLayout CompletionImageLayout
        {
            get { return this.view.CompletionImageLayout; }
            set { this.view.CompletionImageLayout = value; }
        }

        /// <summary>
        /// Gets or sets the background image shape of the completion pages.
        /// </summary>
        public RadImageShape CompletionImageBackgroundShape
        {
            get { return this.view.CompletionImageBackgroundShape; }
            set { this.view.CompletionImageBackgroundShape = value; }
        }


        /// <summary>
        /// Gets or sets the visibility of the page header's title element.
        /// </summary>
        public ElementVisibility PageTitleTextVisibility
        {
            get { return this.view.PageTitleTextVisibility; }
            set { this.view.PageTitleTextVisibility = value; }
        }

        /// <summary>
        /// Gets or sets the visibility of the page header's header element.
        /// </summary>
        public ElementVisibility PageHeaderTextVisibility
        {
            get { return this.view.PageHeaderTextVisibility; }
            set { this.view.PageHeaderTextVisibility = value; }
        }

        /// <summary>
        /// Gets or sets the icon of the page header.
        /// </summary>
        public Image PageHeaderIcon
        {
            get { return this.view.PageHeaderIcon; }
            set
            {
                this.view.PageHeaderIcon = value;
                this.UpdatePageHeaderIconElement(this.selectedPage);
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the page header's icon.
        /// </summary>
        public ContentAlignment PageHeaderIconAlignment
        {
            get { return this.view.PageHeaderIconAlignment; }
            set { this.view.PageHeaderIconAlignment = value; }
        }


        /// <summary>
        /// Gets the command area's back button.
        /// </summary>
        public RadButtonElement BackButton
        {
            get { return this.view.BackButton; }
        }

        /// <summary>
        /// Gets the command area's next button.
        /// </summary>
        public RadButtonElement NextButton
        {
            get { return this.view.NextButton; }
        }

        /// <summary>
        /// Gets the command area's cancel button.
        /// </summary>
        public RadButtonElement CancelButton
        {
            get { return this.view.CancelButton; }
        }

        /// <summary>
        /// Gets the command area's finish button.
        /// </summary>
        public RadButtonElement FinishButton
        {
            get { return this.view.FinishButton; }
        }

        /// <summary>
        /// Gets the command area's help button.
        /// </summary>
        public LightVisualElement HelpButton
        {
            get { return this.view.HelpButton; }
        }

        #endregion

        #region Methods

        private void UpdateView(WizardMode mode)
        {
            RadWizard control = null;
            if (this.ElementTree != null)
            {
                control = this.ElementTree.Control as RadWizard;
            }

            if (this.view != null)
            {
                if (control != null)
                {
                    control.UnWireEvents();
                }

                this.Children.Remove(view);
            }
            WizardView previousView = this.view;
            switch (this.mode)
            {
                case WizardMode.Wizard97:
                    this.view = new Wizard97View(this);
                    if (control != null)
                    {
                        control.UnapplyAeroStyle();
                    }
                    break;
                case WizardMode.Aero:
                    this.view = new WizardAeroView(this);
                    break;
                default:
                    break;
            }
            if (this.view != null)
            {
                this.Children.Add(view);
                this.UpdateView(this.selectedPage);

                if (control != null)
                {
                    control.WireEvents();
                }
            }
            if (previousView != null)
            {
                this.SetViewProperties(previousView);
            }
        }

        private void UpdateView(WizardPage page)
        {
            this.UpdateImageElements(page);
            this.UpdatePageHeaderElement(page);
            this.UpdateCommandButtonsStatus(page);
            this.CommandArea.UpdateInfo(page);
            this.PageHeaderElement.UpdateInfo(page);
        }

        /// <summary>
        /// Refreshes the element's view.
        /// </summary>
        public void Refresh()
        {
            this.UpdateView(this.selectedPage);
            this.InvalidateMeasure(true);
        }

        internal void ApplyAeroStyle()
        {
            if (this.mode == WizardMode.Aero && this.ownerControl != null)
            {
                this.ownerControl.ApplyAeroStyle();
            }
        }


        /// <summary>
        /// Selects next wizard page.
        /// </summary>
        public void SelectNextPage()
        {
            WizardCancelEventArgs e = new WizardCancelEventArgs();
            this.OnNext(e);
            if (e.Cancel)
            {
                return;
            }

            int selectedPageIndex = this.Pages.IndexOf(this.SelectedPage);
            if (selectedPageIndex < this.Pages.Count - 1)
            {
                this.SelectedPage = this.Pages[++selectedPageIndex];
            }
        }

        /// <summary>
        /// Selects previous wizard page.
        /// </summary>
        public void SelectPreviousPage()
        {
            WizardCancelEventArgs e = new WizardCancelEventArgs();
            this.OnPrevious(e);
            if (e.Cancel)
            {
                return;
            }

            int selectedPageIndex = this.Pages.IndexOf(this.SelectedPage);
            if (selectedPageIndex > 0)
            {
                this.SelectedPage = this.Pages[--selectedPageIndex];
            }
        }


        internal void UpdateImageElements(WizardPage page)
        {
            if (page == null)
            {
                if (selectedPage == null)
                {
                    return;
                }
                page = this.selectedPage;
            }

            WizardWelcomePage welcomePage = page as WizardWelcomePage;
            if (welcomePage != null)
            {
                if (welcomePage.WelcomeImage != null)
                {
                    this.WelcomeImageElement.Image = welcomePage.WelcomeImage;
                }
                else
                {
                    this.WelcomeImageElement.Image = this.WelcomeImage;
                }

                return;
            }

            WizardCompletionPage completionPage = this.SelectedPage as WizardCompletionPage;
            if (completionPage != null)
            {
                if (completionPage.CompletionImage != null)
                {
                    this.CompletionImageElement.Image = completionPage.CompletionImage;
                }
                else
                {
                    this.CompletionImageElement.Image = this.CompletionImage;
                }
            }
        }

        private void UpdatePageHeaderElement(WizardPage page)
        {
            if (page == null || this.view == null || this.PageHeaderElement == null)
            {
                this.PageHeaderElement.Title = String.Empty;
                this.PageHeaderElement.Header = String.Empty;
                this.PageHeaderElement.IconElement.Visibility = ElementVisibility.Collapsed;
                return;
            }

            this.PageHeaderElement.Title = page.Title;
            this.PageHeaderElement.Header = page.Header;
            this.UpdatePageHeaderTextsVisibility(page);
            this.UpdatePageHeaderIconElement(page);

            this.PageHeaderElement.IconElement.Visibility = ElementVisibility.Visible;
        }

        private void UpdatePageHeaderTextsVisibility(WizardPage page)
        {
            if (page == null)
            {
                return;
            }

            if (page.CustomizePageHeader)
            {
                this.PageHeaderElement.TitleElement.Visibility = page.TitleVisibility;
                this.PageHeaderElement.HeaderElement.Visibility = page.HeaderVisibility;

                this.pageHeaderCustomized = true;
            }
            else if (this.pageHeaderCustomized)
            {
                if (this.PageHeaderElement.SetDefaultTitleVisibility)
                {
                    this.PageHeaderElement.TitleElement.Visibility = this.PageHeaderElement.DefaultTitleVisibility;
                }
                else
                {
                    this.PageHeaderElement.TitleElement.ResetValue(VisibilityProperty, ValueResetFlags.Local);
                }
                if (this.PageHeaderElement.SetDefaultHeaderVisibility)
                {
                    this.PageHeaderElement.HeaderElement.Visibility = this.PageHeaderElement.DefaultHeaderVisibility;
                }
                else
                {
                    this.PageHeaderElement.HeaderElement.ResetValue(VisibilityProperty, ValueResetFlags.Local);
                }

                this.pageHeaderCustomized = false;
            }
        }

        private void UpdatePageHeaderIconElement(WizardPage page)
        {
            if (page == null)
            {
                return;
            }

            if (page.CustomizePageHeader)
            {
                this.PageHeaderElement.IconElement.Image = page.Icon;
            }
            else
            {
                this.PageHeaderElement.IconElement.Image = this.PageHeaderIcon;
            }
        }

        private void UpdateCommandButtonsStatus(WizardPage wizardPage)
        {
            if (wizardPage == null)
            {
                this.CollapseButtons();
                return;
            }

            this.BackButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Visible);
            this.CancelButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Visible);
            this.HelpButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Visible);
            int pageIndex = this.Pages.IndexOf(wizardPage);
            if (wizardPage is WizardWelcomePage)
            {
                if (this.BackButton != null)
                {
                    this.BackButton.Enabled = false;
                }
                if (this.NextButton != null)
                {
                    this.NextButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Visible);
                    this.NextButton.Enabled = pageIndex < this.Pages.Count - 1;
                }
                if (this.FinishButton != null)
                {
                    this.FinishButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Collapsed);
                }
            }
            else if (wizardPage is WizardCompletionPage)
            {
                if (this.BackButton != null)
                {
                    this.BackButton.Enabled = pageIndex > 0;
                }
                if (this.NextButton != null)
                {
                    this.NextButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Collapsed);
                }
                if (this.FinishButton != null)
                {
                    this.FinishButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Visible);
                }
            }
            else
            {
                if (this.BackButton != null)
                {
                    this.BackButton.Enabled = pageIndex > 0;
                }
                if (this.NextButton != null)
                {
                    this.NextButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Visible);
                    this.NextButton.Enabled = pageIndex < this.Pages.Count - 1;
                }
                if (this.FinishButton != null)
                {
                    this.FinishButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Collapsed);
                }
            }
        }

        private void CollapseButtons()
        {
            this.BackButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Collapsed);
            this.NextButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Collapsed);
            this.FinishButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Collapsed);
            this.CancelButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Collapsed);
            this.HelpButton.SetDefaultValueOverride(RadElement.VisibilityProperty, ElementVisibility.Collapsed);
        }

        private void SetViewProperties(WizardView previousView)
        {
            this.WelcomeImage = previousView.WelcomeImage;
            this.HideWelcomeImage = previousView.HideWelcomeImage;
            this.CompletionImage = previousView.CompletionImage;
            this.HideCompletionImage = previousView.HideCompletionImage;
        }


        public bool HitTestButtons(Point controlClient)
        {
            if (this.NextButton != null && this.NextButton.ControlBoundingRectangle.Contains(controlClient))
            {
                return true;
            }
            if (this.BackButton != null && this.BackButton.ControlBoundingRectangle.Contains(controlClient))
            {
                return true;
            }
            if (this.view != null && this.view.ControlBoundingRectangle.Contains(controlClient))
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Event handlers

        private void Pages_CollectionChanging(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null && e.OldItems.Count > 0 && e.OldItems[0] == this.selectedPage)
            {
                WizardPage pageToBeRemoved = this.selectedPage;
                if (this.Pages.Count > 1)
                {
                    if (pageToBeRemoved == this.Pages[this.Pages.Count - 1])
                    {
                        this.SelectPreviousPage();
                    }
                    else
                    {
                        this.SelectNextPage();
                    }
                }
                else
                {
                    this.SelectedPage = null;
                }
            }
        }

        private void Pages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems != null && e.NewItems.Count > 0)
            {
                WizardPage page = e.NewItems[0] as WizardPage;

                if (this.selectedPage == null)
                {
                    this.SelectedPage = page;
                }
                if (page is WizardWelcomePage && this.welcomePage == null)
                {
                    this.welcomePage = page as WizardWelcomePage;
                }
                if (page is WizardCompletionPage && this.completionPage == null)
                {
                    this.completionPage = page as WizardCompletionPage;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.NewItems != null && e.NewItems.Count > 0)
            {
                WizardPage page = e.NewItems[0] as WizardPage;

                if (this.welcomePage == page)
                {
                    this.welcomePage = null;
                }
                else if (this.completionPage == page)
                {
                    this.completionPage = null;
                }
            }

            this.UpdateView(this.selectedPage);
        }

        #endregion

        #region Overrides

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == EnableAeroStyleProperty && this.Mode == WizardMode.Aero)
            {
                if ((bool)e.NewValue)
                {
                    this.PageHeaderElement.UnapplyThemeStyles();
                    this.PageHeaderElement.Owner.ApplyAeroStyle();
                }
                else
                {
                    this.PageHeaderElement.ApplyThemeStyles();
                }
            }

            base.OnPropertyChanged(e);
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

        #endregion
    }
}