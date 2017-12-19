using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Xml.Serialization.GeneratedAssembly;
using Telerik.WinControls.UI.Docking.Serialization;
using Telerik.WinControls.Keyboard;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.UI;
using Telerik.WinControls.UI.Design;
using Telerik.WinControls.XmlSerialization;
using System.Windows.Forms.Design;
using Telerik.WinControls.Design;
using Telerik.WinControls.UI.Localization;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// Represents the major control in the Telerik.WinForms.UI.Docking assembly. Root component for the entire docking framework.
    /// Provides highly extensible and completely transparent transaction-based API for managing a set of tool and document windows registered within the docking framework.
    /// Mimics the user experience found in Microsoft Visual Studio - dockable panes, tabbed documents, document navigation plus much more.
    /// <example>A typical scenario of utilizing a docking framework is as follows:
    /// <list type="1">
    /// <item>Create a new RadDock instance.</item>
    /// <item>Create the desired <see cref="ToolWindow">ToolWindow</see> and/or <see cref="DocumentWindow">DocumentWindow</see> instances.</item>
    /// <item>Register the newly created dock windows with the RadDock using the <see cref="RadDock.DockWindow(DockWindow, DockPosition)">DockWindow</see> or the <see cref="RadDock.AddDocument(DockWindow)">AddDocument</see> methods respectively.</item>
    /// <item>Add the new RadDock instance to a Form.</item>
    /// <item>Display the Form.</item>
    /// </list>
    /// </example>
    /// <remarks>
    /// Although a rare scenario, sometimes the need of nested RadDock instances may emerge. This is fully supported by the RadDock component and unlimited levels of nesting are allowed.
    /// </remarks>
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.ContainersGroup)]
    [ToolboxItem(true)]
    [Designer("Telerik.WinControls.UI.Design.RadDockDesigner, Telerik.WinControls.UI.Design, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e")]
    [RadThemeDesignerData(typeof(RadDockThemeDesignerData))]
    public class RadDock : RadSplitContainer, Telerik.WinControls.Interfaces.ILoadObsoleteDockingManagerXml, IMessageListener
    {
        #region Fields

        internal const string DockSplitContainerThemeClassName = "DockSplitContainer";
        private static ulong DockTabStripCounter;
        private const int LEFT = 0;
        private const int TOP = 1;
        private const int RIGHT = 2;
        private const int BOTTOM = 3;
        private const int GROUP_OFFSET = 10;

        private Dictionary<int, RadDockService> services;
        private QuickNavigatorSettings quickNavigatorSettings;
        private RadDockCommandManager commandManager;
        private Queue<RadDockTransaction> transactions;
        private int transactionBlockCount;
        private int lockUpdateCount;
        private bool isDisposing;
        private DocumentManager documentManager;
        private DockWindow activeWindow;
        private DockWindow prevActiveWindow;
        private bool cleaningUp;
        private bool settingActiveWindow;
        private bool messageFilterAdded;
        private bool enableFloatingWindowTheming = true;
        private bool activeWindowChanged = false;
        private bool treatTabbedDocumentsAsToolWindows;
        private IDockingGuidesTemplate guidesTemplate;
        private Type layoutStrategyType;
        private DockWindowInsertOrder toolWindowInsertOrder;
        private TabStripAlignment documentTabsAlignment;
        private TabStripAlignment toolTabsAlignment;
        private TabStripTextOrientation documentTabsTextOrientation;
        private TabStripTextOrientation toolTabsTextOrientation;
        private bool documentTabsVisible;
        private bool toolTabsVisible;
        private bool showDocumentCloseButton;
        private bool showToolCloseButton;

        private AutoHideTabStripElement[] autoHideTabStrips = new AutoHideTabStripElement[4];
        private DockLayoutPanel tablayout;
        internal AutoHidePopup autoHidePopup;
        private DocumentContainer mainDocumentContainer;
        private bool mainDocumentContainerVisible = true;
        private Form parentForm;
        private MdiController mdiController;
        private DockType mdiChildrenDockType = DockType.Document;
        private SortedList<string, DockWindow> attachedWindows;
        private Timer autoHideTimer = new Timer();
        private FloatingWindowCollection floatingWindows = new FloatingWindowCollection();
        private bool activateFromAutoHide = false;
        private AutoHideAnimateMode autoHideAnimateMode;
        private DockWindowCollection userCollection;
        private Point lastMousePos;
        private bool loadLayout;

        private FloatingWindowList serializableFloatingWindows;
        private ComponentXmlSerializationInfo xmlSerializationInfo = null;
        private GuidToNameMappingCollection guidToNameMappings = null;
        private DockAutoHideSerializationContainer serializableAutoHideContainer;

        private static readonly object SplitContainerNeededEventKey;
        private static readonly object DockTabStripNeededEventKey;
        private static readonly object PageViewInstanceCreatedEventKey;
        private static readonly object DockWindowAddedEventKey;
        private static readonly object DockWindowRemovedEventKey;
        private static readonly object DockWindowClosingEventKey;
        private static readonly object DockWindowClosedEventKey;
        private static readonly object DockStateChangingEventKey;
        private static readonly object DockStateChangedEventKey;
        private static readonly object ActiveWindowChangingEventKey;
        private static readonly object SelectedTabChangingEventKey;
        private static readonly object ActiveWindowChangedEventKey;
        private static readonly object SelectedTabChanedEventKey;
        private static readonly object MdiChildActivateEventKey;
        private static readonly object QuickNavigatorSnapshotNeededEventKey;
        private static readonly object TransactionCommittingEventKey;
        private static readonly object TransactionCommittedEventKey;
        private static readonly object TransactionBlockStartedEventKey;
        private static readonly object TransactionBlockEndedEventKey;
        private static readonly object FloatingWindowCreatedEventKey;
        private static readonly object AutoHideWindowDisplayingEventKey;
        private static readonly object AutoHideWindowDisplayedEventKey;
        private static readonly object AutoHideWindowHiddenEventKey;

        #endregion

        #region Initialize & Dispose

        static RadDock()
        {
            SplitContainerNeededEventKey = new object();
            DockTabStripNeededEventKey = new object();
            PageViewInstanceCreatedEventKey = new object();
            DockWindowAddedEventKey = new object();
            DockWindowRemovedEventKey = new object();
            DockWindowClosingEventKey = new object();
            DockWindowClosedEventKey = new object();
            DockStateChangingEventKey = new object();
            DockStateChangedEventKey = new object();
            ActiveWindowChangingEventKey = new object();
            SelectedTabChangingEventKey = new object();
            SelectedTabChanedEventKey = new object();
            ActiveWindowChangedEventKey = new object();
            MdiChildActivateEventKey = new object();
            QuickNavigatorSnapshotNeededEventKey = new object();
            TransactionCommittingEventKey = new object();
            TransactionCommittedEventKey = new object();
            TransactionBlockStartedEventKey = new object();
            TransactionBlockEndedEventKey = new object();
            FloatingWindowCreatedEventKey = new object();
            AutoHideWindowDisplayingEventKey = new object();
            AutoHideWindowDisplayedEventKey = new object();
            AutoHideWindowHiddenEventKey = new object();

            new Telerik.WinControls.Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_Docking_RadDock().DeserializeTheme();
            new Telerik.WinControls.Themes.ControlDefault.ControlDefault_DockSplitContainer().DeserializeTheme();
            new Telerik.WinControls.Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_Docking_DocumentContainer().DeserializeTheme();
        }

        /// <summary>
        /// Initializes a new <see cref="RadDock">RadDock</see> instance.
        /// </summary>
        public RadDock()
        {
            this.lastMousePos = new Point(-1, -1);
            this.documentTabsVisible = true;
            this.toolTabsVisible = true;
            this.documentTabsAlignment = TabStripAlignment.Top;
            this.toolTabsAlignment = TabStripAlignment.Bottom;
            this.documentTabsTextOrientation = TabStripTextOrientation.Default;
            this.toolTabsTextOrientation = TabStripTextOrientation.Default;

            this.autoHideAnimateMode = AutoHideAnimateMode.AnimateShow;
            this.autoHidePopup = new AutoHidePopup(this);
            this.autoHideTimer = new Timer();
            this.autoHideTimer.Interval = 300;
            this.autoHideTimer.Tick += new EventHandler(autoHideTimer_Tick);

            this.documentManager = new DocumentManager(this);
            this.TabStop = false;

            this.mdiController = new MdiController(this);
            this.commandManager = new RadDockCommandManager(this);

            this.services = new Dictionary<int, RadDockService>();
            this.quickNavigatorSettings = new QuickNavigatorSettings();

            this.RegisterService(ServiceConstants.DragDrop, new DragDropService());
            this.RegisterService(ServiceConstants.Redock, new RedockService());
            this.RegisterService(ServiceConstants.ContextMenu, new ContextMenuService());
            this.transactions = new Queue<RadDockTransaction>();

            this.attachedWindows = new SortedList<string, DockWindow>(4);

            this.CreateMainDocumentContainer();
            this.toolWindowInsertOrder = DockWindowInsertOrder.Default;

            RadDockLocalizationProvider.CurrentProviderChanged += RadDockLocalizationProvider_CurrentProviderChanged;
        }

        private void RadDockLocalizationProvider_CurrentProviderChanged(object sender, EventArgs e)
        {
            foreach (DockTabStrip strip in this.EnumFrameworkControls<DockTabStrip>())
            {
                strip.OnLocalizationProviderChanged();
            }

            this.autoHidePopup.ToolStrip.OnLocalizationProviderChanged();
        }

        private void CreateMainDocumentContainer()
        {
            if (this.mainDocumentContainer != null)
            {
                return;
            }

            this.mainDocumentContainer = new DocumentContainer();
            this.mainDocumentContainer.Disposed += OnMainDocumentContainer_Disposed;
            this.AttachDocumentContainer();

            if (this.IsHandleCreated)
            {
                this.SplitPanels.Add(this.mainDocumentContainer);
            }

            if (this.Site != null)
            {
                IContainer container = this.Site.Container;
                if (container != null)
                {
                    container.Add(this.mainDocumentContainer);
                }
            }
        }

        private void AttachDocumentContainer()
        {
            this.mainDocumentContainer.DockManager = this;
            this.mainDocumentContainer.UpdateCollapsed();
        }

        private void DestroyMainDocumentContainer()
        {
            if (this.mainDocumentContainer == null)
            {
                return;
            }

            this.mainDocumentContainer.Parent = null;
            this.mainDocumentContainer.Disposed -= OnMainDocumentContainer_Disposed;
            this.mainDocumentContainer.DockManager = null;
            this.mainDocumentContainer.Dispose();
        }

        private void OnMainDocumentContainer_Disposed(object sender, EventArgs e)
        {
            this.mainDocumentContainer = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                RadDockLocalizationProvider.CurrentProviderChanged -= RadDockLocalizationProvider_CurrentProviderChanged;

                //we need this internal flag because the base.Disposing is not updated yet and we do not want to handle additional notification during our disposing logic.
                this.isDisposing = true;

                this.activeWindow = null;
                this.prevActiveWindow = null;

                if (this.commandManager != null)
                {
                    this.commandManager.Dispose();
                }

                if (this.documentManager != null)
                {
                    this.documentManager.Dispose();
                }

                if (this.mdiController != null)
                {
                    this.mdiController.Dispose();
                }

                if (this.parentForm != null)
                {
                    this.parentForm.Move -= OnParentForm_Move;
                }

                //dispose all hidden windows, that are currently not parented
                foreach (DockWindow window in this.attachedWindows.Values)
                {
                    if (window.Parent == null)
                    {
                        window.Dispose();
                    }
                }

                //dispose all services.
                foreach (RadDockService service in this.services.Values)
                {
                    service.Dispose();
                }

                //dispose any floating windows
                while (floatingWindows.Count > 0)
                {
                    floatingWindows[0].Dispose();
                }

                //dispose auto-hide tabstrips
                foreach (RadElement element in this.autoHideTabStrips)
                {
                    element.Dispose();
                }

                this.autoHideTimer.Dispose();
                this.autoHidePopup.Dispose();
            }

            base.Dispose(disposing);
        }

        internal void RestoreWindowsStatesAfterLoad()
        {
            this.LoadDeserializedFloatingAndAutoHideWindows();
            this.EnsureHiddenWindows();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        protected override void CreateChildItems(RadElement parent)
        {
            base.CreateChildItems(parent);

            this.tablayout = new AutoHideStripLayout(this);
            this.tablayout.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            this.SplitPanelElement.Children.Add(tablayout);

            for (int i = 0; i < this.autoHideTabStrips.Length; i++)
            {
                AutoHideTabStripElement strip = new AutoHideTabStripElement();
                strip.StripButtons = StripViewButtons.None;
                strip.Visibility = ElementVisibility.Collapsed;
                strip.ContentArea.Visibility = ElementVisibility.Collapsed;
                this.autoHideTabStrips[i] = strip;
            }

            this.tablayout.Children.Add(this.autoHideTabStrips[TOP]);
            this.autoHideTabStrips[TOP].TabsPosition = TabPositions.Bottom;
            this.autoHideTabStrips[TOP].StripAlignment = StripViewAlignment.Top;
            this.autoHideTabStrips[TOP].StretchHorizontally = true;
            this.autoHideTabStrips[TOP].StretchVertically = false;

            this.tablayout.Children.Add(this.autoHideTabStrips[BOTTOM]);
            this.autoHideTabStrips[BOTTOM].TabsPosition = TabPositions.Top;
            this.autoHideTabStrips[BOTTOM].StripAlignment = StripViewAlignment.Bottom;
            this.autoHideTabStrips[BOTTOM].StretchHorizontally = true;
            this.autoHideTabStrips[BOTTOM].StretchVertically = false;

            this.tablayout.Children.Add(this.autoHideTabStrips[LEFT]);
            this.autoHideTabStrips[LEFT].TabsPosition = TabPositions.Right;
            this.autoHideTabStrips[LEFT].StripAlignment = StripViewAlignment.Left;
            this.autoHideTabStrips[LEFT].StretchHorizontally = false;
            this.autoHideTabStrips[LEFT].StretchVertically = true;

            this.tablayout.Children.Add(this.autoHideTabStrips[RIGHT]);
            this.autoHideTabStrips[RIGHT].TabsPosition = TabPositions.Left;
            this.autoHideTabStrips[RIGHT].StripAlignment = StripViewAlignment.Right;
            this.autoHideTabStrips[RIGHT].StretchHorizontally = false;
            this.autoHideTabStrips[RIGHT].StretchVertically = true;
        }

        /// <summary>
        /// Determines whether all floating windows' frames within the framework will be themed using Telerik's TPF or will be let with their system appearance.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether all floating windows' frames within the framework will be themed using Telerik's TPF or will be let with their system appearance.")]
        public bool EnableFloatingWindowTheming
        {
            get
            {
                return this.enableFloatingWindowTheming;
            }
            set
            {
                if (this.enableFloatingWindowTheming != value)
                {
                    this.enableFloatingWindowTheming = value;
                    for (int i = 0; i < this.FloatingWindows.Count; i++)
                    {
                        this.floatingWindows[i].AllowTheming = value;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new bool Collapsed
        {
            get
            {
                return base.Collapsed;
            }
            set
            {
                base.Collapsed = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new RadSplitContainer SplitContainer
        {
            get
            {
                return base.SplitContainer;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new RadSplitContainer RootContainer
        {
            get
            {
                return base.RootContainer;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new SplitPanelSizeInfo SizeInfo
        {
            get
            {
                return base.SizeInfo;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AutoScroll
        {
            get
            {
                return base.AutoScroll;
            }
            set
            {
                base.AutoScroll = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AllowDrop
        {
            get
            {
                return base.AllowDrop;
            }
            set
            {
                base.AllowDrop = value;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Size AutoScrollMargin
        {
            get
            {
                return base.AutoScrollMargin;
            }
            set
            {
                base.AutoScrollMargin = value;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Size AutoScrollMinSize
        {
            get
            {
                return base.AutoScrollMinSize;
            }
            set
            {
                base.AutoScrollMinSize = value;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new SplitPanelCollection SplitPanels
        {
            get
            {
                return base.SplitPanels;
            }
        }

        [Browsable(false)]
        public DockLayoutPanel TabStripsLayout
        {
            get
            {
                return this.tablayout;
            }
        }

        /// <summary>
        /// Called when control's creation is complete.
        /// </summary>
        /// <param name="desiredSize"></param>
        protected override void OnLoad(Size desiredSize)
        {
            base.OnLoad(desiredSize);

            if (this.DesignMode)
            {
                if (!this.IsInitializing)
                {
                    this.EnsureActiveWindow();
                }
                return;
            }

            this.parentForm = this.FindForm();
            this.mdiController.Load();

            if (this.parentForm != null)
            {
                this.parentForm.Move += OnParentForm_Move;
            }

            //assign owner for each floating form
            foreach (FloatingWindow window in this.floatingWindows)
            {
                window.Owner = this.parentForm;
            }

            //assign owner to the auto-hide popup form
            this.autoHidePopup.Owner = this.parentForm;

            //notify the command manager that loading is complete
            this.commandManager.OnDockManagerLoaded();
            this.documentManager.OnDockManagerLoaded();

            if (!this.IsInitializing)
            {
                this.EnsureActiveWindow();
            }

            if (this.parentForm != null && this.parentForm.WindowState == FormWindowState.Maximized)
            {
                //schedule additional layout pass when the Form is maximized
                this.BeginInvoke(new MethodInvoker(PerformLayout));
            }
        }

        private void OnParentForm_Move(object sender, EventArgs e)
        {
            if (this.autoHidePopup != null && this.autoHidePopup.Visible)
            {
                this.UpdateAutoHidePopupBounds();
            }
        }

        internal Form ParentForm
        {
            get
            {
                return this.parentForm;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void EndInit()
        {
            base.EndInit();

            this.EnsureInitialized();

            this.EndTransactionBlock();

            if (!this.IsDesignMode)
            {
                this.RestoreWindowsStatesAfterLoad();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void BeginInit()
        {
            base.BeginInit();

            this.BeginTransactionBlock(false);
        }

        internal void EnsureInitialized()
        {
            this.RegisterControl(this);
            this.RegisterFloatingWindows();
            this.RegisterAutoHideWindows();
            this.RegisterTabbedDocuments();
            this.RegisterDockedWindows();

            DocumentContainer docContainer = this.MainDocumentContainer;
            if (docContainer.DockManager == null)
            {
                docContainer.DockManager = this;
            }
            if (docContainer.Parent == null)
            {
                docContainer.Parent = this;
            }

            if (!this.DesignMode && this.IsLoaded && this.ShouldProcessNotification())
            {
                this.ActivateWindow(this.activeWindow);
            }
        }

        private void RegisterDockedWindows()
        {
            foreach (DockWindow window in DockHelper.GetDockWindows(this, true, this))
            {
                //we may have some windows marked for hiding, do not set their state to Docked
                if (window.DesiredDockState == DockState.Hidden)
                {
                    continue;
                }

                if (window.Parent is DocumentTabStrip)
                {
                    window.InnerDockState = DockState.TabbedDocument;
                }
                else if (window.Parent is ToolTabStrip)
                {
                    window.InnerDockState = DockState.Docked;
                }
            }
        }

        private void RegisterAutoHideWindows()
        {
            if (this.autoHidePopup == null)
            {
                return;
            }

            this.EnsureRegisteredStrip(this.autoHidePopup.ToolStrip);
            foreach (DockWindow window in this.autoHidePopup.ToolStrip.TabPanels)
            {
                this.EnsureRegisteredWindow(window, null);
                window.InnerDockState = DockState.AutoHide;
            }
        }

        private void RegisterTabbedDocuments()
        {
            this.documentManager.BuildDocumentList();
        }

        private void EnsureHiddenWindows()
        {
            foreach (DockWindow window in this.attachedWindows.Values)
            {
                if (window.DesiredDockState == DockState.Hidden)
                {
                    this.CloseWindow(window);
                }
            }
        }

        private void RegisterFloatingWindows()
        {
            foreach (FloatingWindow window in this.floatingWindows)
            {
                this.RegisterControl(window.DockContainer);
                foreach (Control c in ControlHelper.EnumChildControls(window, true))
                {
                    DockWindow dockWindow = c as DockWindow;
                    if (dockWindow != null && dockWindow.DockManager == this)
                    {
                        dockWindow.InnerDockState = DockState.Floating;
                    }
                }
            }
        }

        /// <summary>
        /// Delegates the ThemeChanged event to all owned controls and elements.
        /// </summary>
        protected override void OnThemeChanged()
        {
            base.OnThemeChanged();

            string themeName = this.ThemeName;

            foreach (DockTabStrip strip in ControlHelper.GetChildControls<DockTabStrip>(this, true))
            {
                if (strip.DockManager == this)
                {
                    strip.ThemeName = this.ThemeName;
                }
            }

            foreach (FloatingWindow window in this.floatingWindows)
            {
                window.ThemeName = themeName;
            }

            if (this.autoHidePopup != null)
            {
                this.autoHidePopup.ThemeName = themeName;
            }

            //TODO: Guides should support theming, remove once implemented
            //TODO: Partially, the drag and drop operation is related to the floating form theme
            if (themeName == "VisualStudio 2008")
            {
                // this.DockingGuidesTemplate = PredefinedDockingGuidesTemplate.VS2008;
                this.EnableFloatingWindowTheming = false;
            }
            else if (themeName == "Office2010")
            {
                //this.DockingGuidesTemplate = PredefinedDockingGuidesTemplate.Office2010;
            }
            else
            {
                //this.DockingGuidesTemplate = null;
                this.EnableFloatingWindowTheming = true;
            }
            ThemedDockingGuidesTemplate themedDockingGuides = DockingGuidesTemplate as ThemedDockingGuidesTemplate;
            if (themedDockingGuides != null)
            {
                themedDockingGuides.ThemeName = this.ThemeName;
            }
        }

        protected override void OnParentVisibleChanged(EventArgs e)
        {
            base.OnParentVisibleChanged(e);

            this.UpdateOnVisibleChanged();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            this.UpdateOnVisibleChanged();
        }

        private void UpdateOnVisibleChanged()
        {
            if (!this.Visible && this.IsLoaded)
            {
                foreach (DockWindow window in this.attachedWindows.Values)
                {
                    if (window.DockState == DockState.Floating)
                    {
                        window.AllowedDockState |= AllowedDockState.Hidden;
                        window.DockState = DockState.Hidden;
                    }
                }
            }
        }

        #endregion

        #region MDI

        /// <summary>
        /// Gets or sets a value indicating whether [auto detect MDI child].
        /// </summary>
        /// <value><c>true</c> if [auto detect MDI child]; otherwise, <c>false</c>.</value>
        [DefaultValue(false)]
        public bool AutoDetectMdiChildren
        {
            get
            {
                return this.mdiController.Enabled;
            }
            set
            {
                this.mdiController.Enabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating dock type of MDI child host windows added to RadDock.
        /// </summary>
        [Description("Gets or sets a value indicating dock type of MDI child host windows added to RadDock.")]
        [DefaultValue(DockType.Document)]
        public DockType MdiChildrenDockType
        {
            get
            {
                return this.mdiChildrenDockType;
            }
            set
            {
                this.mdiChildrenDockType = value;
            }
        }

        /// <summary>
        /// Gets an array with all the standard Forms added as TabbedDocument to this RadDock instance.
        /// </summary>
        [Browsable(false)]
        public Form[] MdiChildren
        {
            get
            {
                return this.mdiController.MdiChildren.ToArray();
            }
        }

        /// <summary>
        /// Gets the MdiController instance that listens for added MDI children and wraps them into a HostWindow.
        /// Exposed for the sake of tests.
        /// </summary>
        internal MdiController MdiController
        {
            get
            {
                return this.mdiController;
            }
        }

        #endregion

        #region AutoHide

        /// <summary>
        /// Notifies that an auto-hidden window is about to be displayed. Cancelable.
        /// </summary>
        public event AutoHideWindowDisplayingEventHandler AutoHideWindowDisplaying
        {
            add
            {
                this.Events.AddHandler(AutoHideWindowDisplayingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(AutoHideWindowDisplayingEventKey, value);
            }
        }

        /// <summary>
        /// Notifies that an auto-hidden window is about to be displayed. Cancelable.
        /// </summary>
        public event DockWindowEventHandler AutoHideWindowDisplayed
        {
            add
            {
                this.Events.AddHandler(AutoHideWindowDisplayedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(AutoHideWindowDisplayedEventKey, value);
            }
        }

        /// <summary>
        /// Notifies that a window which DockState is AutoHide has been hidden.
        /// </summary>
        public event DockWindowEventHandler AutoHideWindowHidden
        {
            add
            {
                this.Events.AddHandler(AutoHideWindowHiddenEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(AutoHideWindowHiddenEventKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the duration of the auto hide window animation. The default value is 200 milliseconds.
        /// </summary>
        /// <value>The duration of the auto hide window animation. The default value is 200 milliseconds.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(200)]
        public int AutoHideAnimationDuration
        {
            get
            {
                if (this.autoHidePopup != null)
                {
                    return this.autoHidePopup.AnimationDuration;
                }

                return -1;
            }
            set
            {
                if (this.autoHidePopup != null)
                {
                    this.autoHidePopup.AnimationDuration = value;
                }
            }
        }

        InstalledHook IMessageListener.DesiredHook
        {
            get
            {
                return InstalledHook.GetMessage;
            }
        }

        MessagePreviewResult IMessageListener.PreviewMessage(ref Message msg)
        {
            MessagePreviewResult result = MessagePreviewResult.NotProcessed;
            switch (msg.Msg)
            {
                case NativeMethods.WM_MOUSEMOVE:
                    if (this.autoHidePopup.Visible)
                    {
                        Point client = this.PointToClient(Control.MousePosition);
                        this.HandleMouseMove(client);
                    }
                    result = MessagePreviewResult.Processed;
                    break;
                case NativeMethods.WM_LBUTTONDOWN:
                case NativeMethods.WM_RBUTTONDOWN:
                case NativeMethods.WM_XBUTTONDOWN:
                    Control target = Control.FromChildHandle(msg.HWnd);
                    if (target != null && target != this && !(target is RadPopupControlBase) &&
                        ControlHelper.FindAncestor<AutoHidePopup>(target) != this.autoHidePopup &&
                        ControlHelper.FindAncestor<RadPopupControlBase>(target) == null &&
                        !(target is ContextMenuStrip))
                    {
                        this.CloseAutoHidePopup();
                        result = MessagePreviewResult.Processed;
                    }
                    break;
            }

            return result;
        }

        void IMessageListener.PreviewWndProc(Message msg)
        {
            throw new NotImplementedException();
        }

        void IMessageListener.PreviewSystemMessage(SystemMessage message, Message msg)
        {
            throw new NotImplementedException();
        }

        private int GetAutoHideStripIndex(AutoHidePosition position)
        {
            int index;
            if (position == AutoHidePosition.Auto)
            {
                index = 0;
            }
            else
            {
                index = (int)position;
            }

            return index;
        }

        internal void OnDockWindowAutoHideSizeChanged(DockWindow window)
        {
            if (this.DesignMode || this.autoHidePopup == null || !this.autoHidePopup.Visible)
            {
                return;
            }

            DockWindow active = this.autoHidePopup.ActiveWindow;
            Debug.Assert(active != null, "Must have an active window at this point.");
            if (active == null || active != window)
            {
                return;
            }

            if (this.autoHidePopup.AutoHideDock == DockStyle.Left || this.autoHidePopup.AutoHideDock == DockStyle.Right)
            {
                this.autoHidePopup.Width = window.AutoHideSize.Width;
            }
            else
            {
                this.autoHidePopup.Height = window.AutoHideSize.Height;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            if (!this.IsLoaded)
            {
                return;
            }

            this.mdiController.UnwireEvents();
            this.parentForm = this.FindForm();
            this.mdiController.WireEvents();
        }

        internal void CloseAutoHidePopup()
        {
            this.lastMousePos = new Point(-1, -1);
            DockWindow window = this.autoHidePopup.ActiveWindow;
            bool actualHide = this.autoHidePopup.Visible && window != null;

            this.autoHidePopup.HideWindow();
            this.autoHideTimer.Stop();

            if (this.messageFilterAdded)
            {
                RadMessageFilter.Instance.RemoveListener(this);
                messageFilterAdded = false;
            }

            if (actualHide)
            {
                this.OnAutoHideWindowHidden(new DockWindowEventArgs(window));
            }
        }

        private void autoHideTimer_Tick(object sender, EventArgs e)
        {
            this.autoHideTimer.Stop();

            TabStripItem hitItem = this.HitTestTabStipItem();

            //popup is not yet displayed, check whether we should do so
            if (!this.autoHidePopup.Visible)
            {
                if (hitItem != null)
                {
                    this.ShowAutoHidePopup(hitItem, AutoHideDisplayReason.TabItemHovered);
                }
                return;
            }

            //prevent possible null ref exceptions
            DockWindow active = this.autoHidePopup.ActiveWindow;
            Debug.Assert(active != null, "Must have an active window at this point.");
            if (active == null)
            {
                return;
            }

            Point mousePos = Control.MousePosition;
            if (this.autoHidePopup.Bounds.Contains(mousePos))
            {
                return;
            }

            //check whether we are over a tab item - if yes, display the associated DockWindow
            if (hitItem == null)
            {
                if (!this.autoHidePopup.ContainsFocus)
                {
                    //close the popup
                    this.CloseAutoHidePopup();
                }
            }
            else if (hitItem != active.AutoHideTab)
            {
                this.CloseAutoHidePopup();
                this.ShowAutoHidePopup(hitItem, AutoHideDisplayReason.TabItemHovered);
            }
        }

        private TabStripItem HitTestTabStipItem()
        {
            Point mouse = this.PointToClient(Control.MousePosition);
            return this.elementTree.GetElementAtPoint(mouse) as TabStripItem;
        }

        /// <summary>
        /// Gets the auto hide tab item.
        /// </summary>
        /// <param name="dockWindow">The dock window.</param>
        /// <returns></returns>
        public TabStripItem GetAutoHideTab(DockWindow dockWindow)
        {
            for (int i = 0; i < this.autoHideTabStrips.Length; i++)
            {
                for (int j = 0; j < this.autoHideTabStrips[i].Items.Count; j++)
                {
                    if (((TabStripItem)this.autoHideTabStrips[i].Items[j]).TabPanel == dockWindow)
                    {
                        return (TabStripItem)this.autoHideTabStrips[i].Items[j];
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the auto hide tab strip.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public RadPageViewStripElement GetAutoHideTabStrip(AutoHidePosition position)
        {
            return this.autoHideTabStrips[this.GetAutoHideStripIndex(position)];
        }

        internal void UpdateAutoHideTabStrips()
        {
            this.autoHideTabStrips[LEFT].Visibility = (this.autoHideTabStrips[LEFT].Items.Count > 0) ? ElementVisibility.Visible :
                    ElementVisibility.Collapsed;

            this.autoHideTabStrips[RIGHT].Visibility = (this.autoHideTabStrips[RIGHT].Items.Count > 0) ? ElementVisibility.Visible :
                    ElementVisibility.Collapsed;

            this.autoHideTabStrips[TOP].Visibility = (this.autoHideTabStrips[TOP].Items.Count > 0) ? ElementVisibility.Visible :
                    ElementVisibility.Collapsed;

            this.autoHideTabStrips[BOTTOM].Visibility = (this.autoHideTabStrips[BOTTOM].Items.Count > 0) ? ElementVisibility.Visible :
                   ElementVisibility.Collapsed;

            this.tablayout.InvalidateMeasure();
            this.SplitPanelElement.InvalidateMeasure();
            this.SplitPanelElement.UpdateLayout();

            this.LayoutInternal();
        }

        internal void ShowAutoHidePopup(TabStripItem tabItem, AutoHideDisplayReason reason)
        {
            if (!this.IsHandleCreated)
            {
                return;
            }

            if (tabItem.TabPanel == this.autoHidePopup.ActiveWindow && this.autoHidePopup.Visible)
            {
                return;
            }

            DockWindow window = tabItem.TabPanel as DockWindow;
            AutoHideWindowDisplayingEventArgs args = new AutoHideWindowDisplayingEventArgs(window, reason);
            this.OnAutoHideWindowDisplaying(args);
            if (args.Cancel)
            {
                return;
            }

            this.InitializeAutoHidePopup();
            this.autoHidePopup.ToolStrip.SelectedTab = tabItem.TabPanel;
            tabItem.Owner.SelectedItem = tabItem;

            AutoHideTabStripElement tabStrip = tabItem.Owner as AutoHideTabStripElement;
            this.autoHidePopup.AutoHideDock = GetAutoHideDockStyle(tabStrip.TabsPosition);
            this.UpdateAutoHidePopupBounds();

            if (!this.autoHidePopup.Visible)
            {
                //show but do not activate the auto-hide popup
                this.autoHidePopup.ShowWindow();
            }

            if (this.activeWindow != null && this.activeWindow.FindForm() == this.autoHidePopup)
            {
                this.autoHidePopup.Activate();
            }

            this.autoHidePopup.Focus();

            if (!this.messageFilterAdded)
            {
                RadMessageFilter.Instance.AddListener(this);
                this.messageFilterAdded = true;
            }

            this.OnAutoHideWindowDisplayed(new DockWindowEventArgs(window));
        }

        private void InitializeAutoHidePopup()
        {
            Form owner = this.FindForm();
            if (owner != null)
            {
                // FIX TS_6.21.2011 the following line prevents a crash because of a circular reference 
                this.autoHidePopup.Owner = null;
                this.autoHidePopup.Owner = owner;

                if (this.autoHidePopup.TopMost != owner.TopMost)
                {
                    this.autoHidePopup.TopMost = owner.TopMost;
                }
            }

            if (this.autoHidePopup.IsHandleCreated)
            {
                return;
            }
            this.autoHidePopup.RightToLeft = this.RightToLeft;
            this.autoHidePopup.CreateControl();
            Rectangle virtualScreen = SystemInformation.VirtualScreen;

            //these adjustments are needed for the sake of the AnimateWindow API,
            //which will not take child controls' content when displayed for the first time.
            this.autoHidePopup.MinimumSize = new Size(0, 0);
            this.autoHidePopup.Size = new Size(0, 0);
            this.autoHidePopup.Location = new Point(virtualScreen.X - this.autoHidePopup.Width, virtualScreen.Y - this.autoHidePopup.Height);

            AutoHideAnimateMode currMode = this.autoHideAnimateMode;
            this.autoHideAnimateMode = AutoHideAnimateMode.None;

            //show and hide window. needed for the proper window slide animation
            NativeMethods.ShowWindow(this.autoHidePopup.Handle, NativeMethods.SW_SHOWNOACTIVATE);
            this.autoHidePopup.Hide();

            this.autoHideAnimateMode = currMode;
        }

        internal List<AutoHideGroup> GetAutoHideGroups(AutoHidePosition position)
        {
            List<AutoHideGroup> res = new List<AutoHideGroup>();

            RadPageViewStripElement autoHideTab = this.GetAutoHideTabStrip(position);
            foreach (TabStripItem tabItem in autoHideTab.Items)
            {
                AutoHideGroup group = ((DockWindow)tabItem.TabPanel).AutoHideGroup;
                if (!res.Contains(group))
                {
                    res.Add(group);
                }
            }

            return res;
        }

        private void UpdateAutoHidePopupBounds()
        {
            if (this.autoHidePopup == null)
            {
                return;
            }

            DockWindow active = this.autoHidePopup.ActiveWindow;
            Debug.Assert(active != null, "Showing popup without active window");
            if (active == null)
            {
                return;
            }

            Point clientLocation = Point.Empty;
            Size size = Size.Empty;
            AutoHideTabStripElement stripElement = null;
            Rectangle contentRect = this.ContentRectangle;
            int width;
            int height;

            switch (this.autoHidePopup.AutoHideDock)
            {
                case DockStyle.Left:
                    stripElement = this.autoHideTabStrips[LEFT];

                    width = Math.Min(this.autoHidePopup.ActiveWindow.AutoHideSize.Width, contentRect.Width - stripElement.Size.Width);
                    size = new Size(width, contentRect.Height);
                    clientLocation = new Point(stripElement.Size.Width + stripElement.AutoHidePopupOffset, contentRect.Y);
                    break;
                case DockStyle.Right:
                    stripElement = this.autoHideTabStrips[RIGHT];

                    width = Math.Min(this.autoHidePopup.ActiveWindow.AutoHideSize.Width, contentRect.Width - stripElement.Size.Width);
                    size = new Size(width, contentRect.Height);
                    clientLocation = new Point(contentRect.Right - width - stripElement.AutoHidePopupOffset, contentRect.Y);
                    break;
                case DockStyle.Top:
                    stripElement = this.autoHideTabStrips[TOP];

                    height = Math.Min(this.autoHidePopup.ActiveWindow.AutoHideSize.Height, contentRect.Height - stripElement.Size.Height);
                    size = new Size(contentRect.Width, height);
                    clientLocation = new Point(contentRect.X, contentRect.Y + stripElement.AutoHidePopupOffset);
                    break;
                case DockStyle.Bottom:
                    stripElement = this.autoHideTabStrips[BOTTOM];

                    height = Math.Min(this.autoHidePopup.ActiveWindow.AutoHideSize.Height, contentRect.Height - stripElement.Size.Height);
                    size = new Size(contentRect.Width, height);
                    clientLocation = new Point(contentRect.X, contentRect.Bottom - height - stripElement.AutoHidePopupOffset);
                    break;
            }

            this.autoHidePopup.Bounds = new Rectangle(this.PointToScreen(clientLocation), size);
        }

        private DockStyle GetAutoHideDockStyle(TabPositions tabPosition)
        {
            switch (tabPosition)
            {
                case TabPositions.Left:
                    return DockStyle.Right;
                case TabPositions.Right:
                    return DockStyle.Left;
                case TabPositions.Top:
                    return DockStyle.Bottom;
                case TabPositions.Bottom:
                    return DockStyle.Top;
            }

            return DockStyle.None;
        }

        private bool IsContextMenuShown()
        {
            ContextMenuService service = this.GetService<ContextMenuService>(ServiceConstants.ContextMenu);
            if (service != null)
            {
                return service.IsMenuDisplayed;
            }

            return false;
        }

        /// <summary>
        /// Raises the <see cref="AutoHideWindowDisplaying">AutoHideWindowDisplaying</see> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAutoHideWindowDisplaying(AutoHideWindowDisplayingEventArgs e)
        {
            AutoHideWindowDisplayingEventHandler eh = this.Events[AutoHideWindowDisplayingEventKey] as AutoHideWindowDisplayingEventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="AutoHideWindowDisplayed">AutoHideWindowDisplayed</see> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAutoHideWindowDisplayed(DockWindowEventArgs e)
        {
            DockWindowEventHandler eh = this.Events[AutoHideWindowDisplayedEventKey] as DockWindowEventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="AutoHideWindowHidden">AutoHideWindowHidden</see> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnAutoHideWindowHidden(DockWindowEventArgs e)
        {
            DockWindowEventHandler eh = this.Events[AutoHideWindowHiddenEventKey] as DockWindowEventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        protected override bool ProcessFocusRequested(RadElement element)
        {
            //we do not want to be focused by elements in our element tree
            return false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            TabStripItem hitItem = this.elementTree.GetElementAtPoint(e.Location) as TabStripItem;
            if (hitItem == null)
            {
                return;
            }

            if (this.autoHidePopup.Visible &&
                this.autoHidePopup.ActiveWindow.AutoHideTab != hitItem)
            {
                this.CloseAutoHidePopup();
            }

            this.ShowAutoHidePopup(hitItem, AutoHideDisplayReason.TabItemClicked);
            //user may have canceled the operation
            if (!this.autoHidePopup.Visible)
            {
                return;
            }

            this.autoHidePopup.Activate();
            this.ActiveWindow = this.autoHidePopup.ActiveWindow;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            this.HandleMouseMove(e.Location);
        }

        private void HandleMouseMove(Point client)
        {
            if (this.lastMousePos == client)
            {
                return;
            }

            this.lastMousePos = client;
            this.autoHideTimer.Stop();
            this.autoHideTimer.Start();
        }

        #endregion

        #region Quick Navigator

        /// <summary>
        /// Raised when the QuickNavigator is displayed and a preview snapshot for the currently selected window is needed.
        /// </summary>
        public event DockWindowSnapshotEventHandler QuickNavigatorSnapshotNeeded
        {
            add
            {
                this.Events.AddHandler(QuickNavigatorSnapshotNeededEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(QuickNavigatorSnapshotNeededEventKey, value);
            }
        }

        /// <summary>
        /// Visualizes the QuickNavigator control allowing for active tool windows and documents browsing.
        /// </summary>
        /// <returns>The <see cref="QuickNavigator">QuickNavigator</see> instance that is currently displayed. May return null if operation was unsuccessful.</returns>
        public QuickNavigator DisplayQuickNavigator()
        {
            if (!this.quickNavigatorSettings.Enabled)
            {
                return null;
            }

            QuickNavigatorPopup popup = new QuickNavigatorPopup(this);
            popup.Display();
            return popup.Navigator;
        }

        /// <summary>
        /// Gets the object which controls the appearance of the QuickNavigator control.
        /// </summary>
        [Description("Gets the object which controls the appearance of the QuickNavigator control.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public QuickNavigatorSettings QuickNavigatorSettings
        {
            get
            {
                return this.quickNavigatorSettings;
            }
        }

        /// <summary>
        /// Raises the <see cref="QuickNavigatorSnapshotNeeded"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnQuickNavigatorSnapshotNeeded(DockWindowSnapshotEventArgs e)
        {
            DockWindowSnapshotEventHandler eh = this.Events[QuickNavigatorSnapshotNeededEventKey] as DockWindowSnapshotEventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        #endregion

        #region Public Properties

        public override int SplitterWidth
        {
            get
            {
                return base.SplitterWidth;
            }
            set
            {
                base.SplitterWidth = value;
                SetSplitterWidthRecursive(this);
            }
        }

        private void SetSplitterWidthRecursive(RadSplitContainer container)
        {
            foreach (Control control in container.Controls)
            {
                RadSplitContainer c = control as RadSplitContainer;
                if (c != null)
                {
                    c.SplitterWidth = this.SplitterWidth;
                    SetSplitterWidthRecursive(c);
                }
            }
        }

        /// <summary>
        /// Gets or sets the insert order to be used when adding dock windows to a ToolTabStrip.
        /// </summary>
        [Description("Gets or sets the insert order to be used when adding dock windows to a ToolTabStrip.")]
        public DockWindowInsertOrder ToolWindowInsertOrder
        {
            get
            {
                if (this.toolWindowInsertOrder == DockWindowInsertOrder.Default)
                {
                    return DockWindowInsertOrder.InFront;
                }

                return this.toolWindowInsertOrder;
            }
            set
            {
                this.toolWindowInsertOrder = value;
            }
        }

        bool ShouldSerializeToolWindowInsertOrder()
        {
            return this.toolWindowInsertOrder != DockWindowInsertOrder.Default;
        }

        /// <summary>
        /// Determines what animation will be used when displaying/hiding auto-hidden windows.
        /// </summary>
        [DefaultValue(AutoHideAnimateMode.AnimateShow)]
        [Description("Determines what animation will be used when displaying/hiding auto-hidden windows.")]
        public AutoHideAnimateMode AutoHideAnimation
        {
            get
            {
                return this.autoHideAnimateMode;
            }
            set
            {
                this.autoHideAnimateMode = value;
            }
        }

        /// <summary>
        /// Overrides the content rectangle to add any visible auto-hide tabstrips to the calculations.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override System.Drawing.Rectangle ContentRectangle
        {
            get
            {
                int left = (this.autoHideTabStrips[LEFT] != null && this.autoHideTabStrips[LEFT].Visibility == ElementVisibility.Visible)
                    ? this.autoHideTabStrips[LEFT].Size.Width : 0;

                int top = (this.autoHideTabStrips[TOP] != null && this.autoHideTabStrips[TOP].Visibility == ElementVisibility.Visible)
                    ? this.autoHideTabStrips[TOP].Size.Height : 0;

                int width = (this.autoHideTabStrips[RIGHT] != null && this.autoHideTabStrips[RIGHT].Visibility == ElementVisibility.Visible)
                   ? this.Width - (this.autoHideTabStrips[RIGHT].Size.Width + left) : this.Width - left;

                int height = (this.autoHideTabStrips[BOTTOM] != null && this.autoHideTabStrips[BOTTOM].Visibility == ElementVisibility.Visible)
                   ? this.Height - (this.autoHideTabStrips[BOTTOM].Size.Height + top) : this.Height - top;


                return new System.Drawing.Rectangle(left + this.Padding.Left, top + this.Padding.Top,
                    width - (this.Padding.Left + this.Padding.Right), height - (this.Padding.Top + this.Padding.Bottom));
            }
        }

        /// <summary>
        /// Gets or sets the Type to be used when a new RadSplitContainer instance is internally created and a layout strategy is initialized for it.
        /// Allows plugging of completely custom layout behavior.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Type LayoutStrategyType
        {
            get
            {
                return this.layoutStrategyType;
            }
            set
            {
                if (this.layoutStrategyType == value)
                {
                    return;
                }

                this.layoutStrategyType = value;
                this.ApplyLayoutStrategy();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
            }
        }

        /// <summary>
        /// Gets DockWindow instance with the corresponding Name.
        /// </summary>
        /// <param name="dockWindowName"></param>
        /// <returns>DockWindow instance with matching Name. Null (Nothing in VB.NET) otherwise.</returns>
        public DockWindow this[string dockWindowName]
        {
            get
            {
                if (this.InnerList.ContainsKey(dockWindowName))
                {
                    return this.InnerList[dockWindowName];
                }

                return null;
            }
        }


        /// <summary>
        /// Gets DockWindow instance with the corresponding Name by the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dockWindowName"></param>
        /// <returns>DockWindow instance with matching Name. Null (Nothing in VB.NET) otherwise.</returns>
        public T GetWindow<T>(string dockWindowName) where T : DockWindow
        {
            if (this.InnerList.ContainsKey(dockWindowName))
            {
                return this.InnerList[dockWindowName] as T;
            }

            return default(T);
        }


        /// <summary>
        /// Gets the windows by the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> GetWindows<T>() where T : DockWindow
        {
            foreach (DockWindow w in this.DockWindows)
            {
                if (w is T)
                {
                    yield return w as T;
                }
            }
        }


        /// <summary>
        /// Gets a collection with all the currently attached <see cref="Telerik.WinControls.UI.Docking.DockWindow">DockWindow</see> instances.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DockWindowCollection DockWindows
        {
            get
            {
                if (userCollection == null)
                {
                    this.userCollection = new DockWindowCollection(this);
                }

                return this.userCollection;
            }
        }

        #endregion

        #region TabStrip Settings

        /// <summary>
        /// Determines whether DocumentTabStrip instances will display Close Button next to each item.
        /// </summary>
        [DefaultValue(false)]
        [Description("Determines whether DocumentTabStrip instances will display Close Button next to each item.")]
        public bool ShowDocumentCloseButton
        {
            get
            {
                return this.showDocumentCloseButton;
            }
            set
            {
                if (this.showDocumentCloseButton == value)
                {
                    return;
                }

                this.showDocumentCloseButton = value;
                foreach (DocumentTabStrip strip in this.EnumFrameworkControls<DocumentTabStrip>())
                {
                    strip.ShowItemCloseButton = this.showDocumentCloseButton;
                }
            }
        }

        /// <summary>
        /// Determines whether ToolTabStrip instances will display Close Button next to each item.
        /// </summary>
        [DefaultValue(false)]
        [Description("Determines whether ToolTabStrip instances will display Close Button next to each item.")]
        public bool ShowToolCloseButton
        {
            get
            {
                return this.showToolCloseButton;
            }
            set
            {
                if (this.showToolCloseButton == value)
                {
                    return;
                }

                this.showToolCloseButton = value;
                foreach (ToolTabStrip strip in this.EnumFrameworkControls<ToolTabStrip>())
                {
                    strip.ShowItemCloseButton = this.showToolCloseButton;
                }
            }
        }

        /// <summary>
        /// Gets or sets the text orientation of the TabStripElement in all ToolTabStrip instances.
        /// </summary>
        [Description("Gets or sets the text orientation of the TabStripElement in all ToolTabStrip instances.")]
        public TabStripTextOrientation DocumentTabsTextOrientation
        {
            get
            {
                return this.documentTabsTextOrientation;
            }
            set
            {
                if (value == this.documentTabsTextOrientation)
                {
                    return;
                }

                this.documentTabsTextOrientation = value;
                foreach (DocumentTabStrip strip in this.EnumFrameworkControls<DocumentTabStrip>())
                {
                    strip.TabStripTextOrientation = this.documentTabsTextOrientation;
                }
            }
        }

        bool ShouldSerializeDocumentTabsTextOrientation()
        {
            return this.documentTabsTextOrientation != TabStripTextOrientation.Default;
        }

        /// <summary>
        /// Gets or sets the text orientation of the TabStripElement in all ToolTabStrip instances.
        /// </summary>
        [Description("Gets or sets the text orientation of the TabStripElement in all ToolTabStrip instances.")]
        public TabStripTextOrientation ToolTabsTextOrientation
        {
            get
            {
                return this.toolTabsTextOrientation;
            }
            set
            {
                if (value == this.toolTabsTextOrientation)
                {
                    return;
                }

                this.toolTabsTextOrientation = value;
                foreach (ToolTabStrip strip in this.EnumFrameworkControls<ToolTabStrip>())
                {
                    strip.TabStripTextOrientation = this.toolTabsTextOrientation;
                }
            }
        }

        bool ShouldSerializeToolTabsTextOrientation()
        {
            return this.toolTabsTextOrientation != TabStripTextOrientation.Default;
        }

        /// <summary>
        /// Gets or sets the alignment of the TabStripElement in all ToolTabStrip instances.
        /// </summary>
        [DefaultValue(TabStripAlignment.Bottom)]
        [Description("Gets or sets the alignment of the TabStripElement in all ToolTabStrip instances.")]
        public TabStripAlignment ToolTabsAlignment
        {
            get
            {
                return this.toolTabsAlignment;
            }
            set
            {
                if (value == TabStripAlignment.Default)
                {
                    value = TabStripAlignment.Bottom;
                }

                if (this.toolTabsAlignment == value)
                {
                    return;
                }

                this.toolTabsAlignment = value;
                foreach (ToolTabStrip strip in this.EnumFrameworkControls<ToolTabStrip>())
                {
                    strip.TabStripAlignment = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the alignment of the TabStripElement in all DocumentTabStrip instances.
        /// </summary>
        [DefaultValue(TabStripAlignment.Top)]
        [Description("Gets or sets the alignment of the TabStripElement in all DocumentTabStrip instances.")]
        public TabStripAlignment DocumentTabsAlignment
        {
            get
            {
                return this.documentTabsAlignment;
            }
            set
            {
                if (value == TabStripAlignment.Default)
                {
                    value = TabStripAlignment.Top;
                }

                if (this.documentTabsAlignment == value)
                {
                    return;
                }

                this.documentTabsAlignment = value;
                foreach (DocumentTabStrip strip in this.EnumFrameworkControls<DocumentTabStrip>())
                {
                    strip.TabStripAlignment = value;
                }
            }
        }

        /// <summary>
        /// Determines whether the TabStripElement in DocumentTabStrip instances is visible.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether the TabStripElement in DocumentTabStrip instances is visible.")]
        public bool DocumentTabsVisible
        {
            get
            {
                return this.documentTabsVisible;
            }
            set
            {
                if (this.documentTabsVisible == value)
                {
                    return;
                }

                this.documentTabsVisible = value;
                foreach (DocumentTabStrip strip in this.EnumFrameworkControls<DocumentTabStrip>())
                {
                    strip.TabStripVisible = value;
                }
            }
        }

        /// <summary>
        /// Determines whether the TabStripElement in ToolTabStrip instances is visible.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether the TabStripElement in ToolTabStrip instances is visible.")]
        public bool ToolTabsVisible
        {
            get
            {
                return this.toolTabsVisible;
            }
            set
            {
                if (this.toolTabsVisible == value)
                {
                    return;
                }

                this.toolTabsVisible = value;
                foreach (ToolTabStrip strip in this.EnumFrameworkControls<ToolTabStrip>())
                {
                    strip.TabStripVisible = value;
                }
            }
        }

        protected override void OnPaddingChanged(EventArgs e)
        {
            base.OnPaddingChanged(e);

            this.tablayout.InvalidateMeasure();
        }

        #endregion

        #region Transactions

        #region Begin/End Blocks

        /// <summary>
        /// Begins an update operation. Internally used by the transaction mechanism.
        /// </summary>
        public void BeginUpdate()
        {
            //DockHelper.BeginUpdate(this);
            this.SuspendLayout();

            this.lockUpdateCount++;
        }

        /// <summary>
        /// Ends a BeginUpdate block.
        /// </summary>
        public void EndUpdate()
        {
            this.EndUpdate(true);
        }

        /// <summary>
        /// Ends a BeginUpdate block and optionally performs update.
        /// </summary>
        /// <param name="update"></param>
        public void EndUpdate(bool update)
        {
            if (this.lockUpdateCount == 0)
            {
                return;
            }

            this.lockUpdateCount--;
            if (this.lockUpdateCount == 0)
            {
                this.PerformUpdate();
            }
        }

        /// <summary>
        /// Performs the core update logic after an EndUpdate call.
        /// </summary>
        protected virtual void PerformUpdate()
        {
            this.ResumeLayout(true);
            //DockHelper.EndUpdate(this, true);
        }

        /// <summary>
        /// Performs a clean-up pass which removes all unnecessary internally created split panels and/or collapses or disposes them.
        /// </summary>
        public void CleanUp()
        {
            if (this.transactionBlockCount > 0)
            {
                return;
            }

            if (this.cleaningUp)
            {
                return;
            }

            this.cleaningUp = true;

            RedockService service = this.GetService<RedockService>(ServiceConstants.Redock);
            if (service != null)
            {
                service.ResumeCleanUp(true);
            }

            //force strips and containers clean-up
            int count = this.floatingWindows.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                FloatingWindow window = this.floatingWindows[i];
                DockHelper.CleanupContainer(window.DockContainer, this);
                window.UpdateVisibility();
            }

            DockHelper.CleanupContainer(this, this);

            this.cleaningUp = false;
        }

        /// <summary>
        /// Opens a transaction (batch) operation. 
        /// This is used by the framework to indicate some lengthy operations, during which no updates should be performed.
        /// </summary>
        public void BeginTransactionBlock()
        {
            this.BeginTransactionBlock(false);
        }

        /// <summary>
        /// Opens a transaction (batch) operation. 
        /// This is used by the framework to indicate some lengthy operations, during which no updates should be performed.
        /// </summary>
        public void BeginTransactionBlock(bool saveActiveWindow)
        {
            if (this.lockUpdateCount == 0)
            {
                this.BeginUpdate();
                RedockService service = this.GetService<RedockService>(ServiceConstants.Redock);
                if (service != null)
                {
                    service.SuspendCleanUp();
                }

                if (saveActiveWindow)
                {
                    this.prevActiveWindow = this.ActiveWindow;
                }

                this.OnTransactionBlockStarted(EventArgs.Empty);
            }

            this.transactionBlockCount++;
        }

        /// <summary>
        /// Adds the 
        /// </summary>
        /// <param name="transaction"></param>
        public void EnqueueTransaction(RadDockTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("Transaction");
            }
            if (this.transactionBlockCount == 0)
            {
                throw new ArgumentException("A transaction block should be opened before a transaction may be pushed");
            }

            this.transactions.Enqueue(transaction);
        }

        /// <summary>
        /// Commits all pending transactions without exitting the transaction block.
        /// </summary>
        public void FlushTransactions()
        {
            this.CommitTransactions();
        }

        /// <summary>
        /// Determines whether the RadDock is currently in a transaction (internal operation).
        /// </summary>
        [Browsable(false)]
        public bool IsInTransactionBlock
        {
            get
            {
                return this.transactionBlockCount > 0;
            }
        }

        /// <summary>
        /// Ends previously opened transaction.
        /// Optionally preforms update.
        /// </summary>
        public void EndTransactionBlock()
        {
            //TODO: We need more testing for this logic, update after Q2 is released.
            //we are not yet loaded, do not end and commit transactions.
            //if (!this.loaded)
            //{
            //    return;
            //}

            if (this.transactionBlockCount == 0)
            {
                return;
            }

            if (this.transactionBlockCount > 1)
            {
                this.transactionBlockCount--;
                return;
            }

            try
            {
                this.CommitTransactions();
            }
            finally
            {
                this.transactionBlockCount = 0;
                this.CleanUp();
                this.EnsureActiveWindow();
                this.EndUpdate(true);
                this.OnTransactionBlockEnded(EventArgs.Empty);
            }
        }

        /// <summary>
        /// An internal helper method that will either enqueue the transaction if a block is currently running
        /// or will open block, commit the transaction and close that block.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="saveActiveWindow"></param>
        internal void RegisterTransaction(RadDockTransaction transaction, bool saveActiveWindow)
        {
            if (this.transactionBlockCount > 0)
            {
                this.transactions.Enqueue(transaction);
            }
            else
            {
                this.BeginTransactionBlock(saveActiveWindow);
                this.transactions.Enqueue(transaction);
                this.EndTransactionBlock();
            }
        }

        #endregion

        #region Commit

        /// <summary>
        /// Commits all transactions available on the transaction stack.
        /// </summary>
        private void CommitTransactions()
        {
            while (this.transactions.Count > 0)
            {
                this.CommitTransaction(this.transactions.Dequeue());
            }
        }

        /// <summary>
        /// Commits the specified transaction.
        /// </summary>
        /// <param name="transaction"></param>
        protected virtual void CommitTransaction(RadDockTransaction transaction)
        {
            this.EnsureTransaction(transaction);

            //no associated windows, do nothing
            if (transaction.AssociatedWindows.Count == 0)
            {
                return;
            }

            //allow listeners to cancel the transaction
            RadDockTransactionCancelEventArgs args = new RadDockTransactionCancelEventArgs(transaction);
            this.OnTransactionCommitting(args);
            if (args.Cancel)
            {
                return;
            }

            this.PreCommitTransaction(transaction);

            switch (transaction.TransactionType)
            {
                case DockTransactionType.DockWindow:
                    CommitDockWindowTransaction((DockWindowTransaction)transaction);
                    break;
                case DockTransactionType.DragDrop:
                    CommitDragDropTransaction((DragDropTransaction)transaction);
                    break;
                case DockTransactionType.Redock:
                    CommitRedockTransaction((RedockTransaction)transaction);
                    break;
                case DockTransactionType.AutoHide:
                    CommitAutoHideTransaction((AutoHideTransaction)transaction);
                    break;
                case DockTransactionType.Close:
                    CommitCloseTransaction((CloseTransaction)transaction);
                    break;
                case DockTransactionType.Float:
                    CommitFloatTransaction((FloatTransaction)transaction);
                    break;
            }

            this.PostCommitTransaction(transaction);
            this.OnTransactionCommitted(transaction);
        }

        /// <summary>
        /// Final step of processing a transaction.
        /// Performs clean-up logic, raises DockStateChanged, DockWindowClosed, DockWindowAdded and DockWindowRemoved events as needed.
        /// </summary>
        /// <param name="transaction"></param>
        private void OnTransactionCommitted(RadDockTransaction transaction)
        {
            if (transaction.TargetState != DockState.AutoHide)
            {
                this.CleanAutoHideTabs(transaction.AssociatedWindows, true);
            }

            foreach (DockWindow window in transaction.AssociatedWindows)
            {
                bool changed = window.InnerDockState != transaction.TargetState;
                window.InnerDockState = transaction.TargetState;
                window.UpdateActiveState(false);
                DockTabStrip strip = window.DockTabStrip;
                if (strip != null)
                {
                    strip.UpdateAfterTransaction();
                }

                if (changed)
                {
                    this.OnDockStateChanged(new DockWindowEventArgs(window));
                }

                //raise the DockWindowClosed event
                if (transaction.TransactionType == DockTransactionType.Close)
                {
                    this.OnDockWindowClosed(new DockWindowEventArgs(window));
                }
            }

            //raise the DockWindowAdded event
            foreach (DockWindow newWindow in transaction.AddedWindows)
            {
                this.OnDockWindowAdded(new DockWindowEventArgs(newWindow));
            }

            //raise the DockWindowRemoved event
            foreach (DockWindow oldWindow in transaction.RemovedWindows)
            {
                this.OnDockWindowRemoved(new DockWindowEventArgs(oldWindow));
            }

            this.OnTransactionCommitted(new RadDockTransactionEventArgs(transaction));
        }

        /// <summary>
        /// Allows inheritors to provide additional pre-commit processing of a transaction.
        /// </summary>
        /// <param name="transaction"></param>
        protected virtual void PreCommitTransaction(RadDockTransaction transaction)
        {
        }

        /// <summary>
        /// Allows inheritors to provide additional post-commit processing of a transaction.
        /// </summary>
        /// <param name="transaction"></param>
        protected virtual void PostCommitTransaction(RadDockTransaction transaction)
        {
        }

        private void CommitFloatTransaction(FloatTransaction transaction)
        {
            FloatingWindow floatingWindow = this.CreateFloatingWindow();
            DockTabStrip strip = this.CreateNewTabStrip(DockType.ToolWindow);
            if (transaction.AssociatedPanel != null)
            {
                strip.SizeInfo.Copy(transaction.AssociatedPanel.SizeInfo);
            }
            floatingWindow.DockContainer.SplitPanels.Add(strip);

            foreach (DockWindow window in transaction.AssociatedWindows)
            {
                strip.TabPanels.Add(window);
            }

            if (transaction.Bounds != Rectangle.Empty)
            {
                floatingWindow.Bounds = transaction.Bounds;
            }
            else
            {
                Size floatSize = transaction.AssociatedWindows[0].DefaultFloatingSize;
                floatingWindow.Bounds = this.GetDefaultFloatBounds(floatSize);
            }
        }

        private void CommitCloseTransaction(CloseTransaction transaction)
        {
            for (int i = transaction.AssociatedWindows.Count - 1; i >= 0; i--)
            {
                DockWindow window = transaction.AssociatedWindows[i];
                DockWindowCancelEventArgs args = new DockWindowCancelEventArgs(window);
                this.OnDockWindowClosing(args);

                if (args.Cancel)
                {
                    transaction.AssociatedWindows.RemoveAt(i);
                    continue;
                }

                //remove window from its parent collection
                window.Parent = null;

                switch (window.CloseAction)
                {
                    case DockWindowCloseAction.Close:
                        this.UnregisterWindow(window, transaction);
                        break;
                    case DockWindowCloseAction.CloseAndDispose:
                        this.UnregisterWindow(window, transaction);
                        window.Dispose();
                        break;
                    case DockWindowCloseAction.Hide:
                        //remember the last dock state of each window, so that we may restore it later.
                        if (window.DockState != DockState.Hidden && !loadLayout)
                        {
                            window.PreviousDockState = window.DockState;
                        }
                        break;
                }

                this.ClearActiveWindowReferences(window);
            }
        }

        /// <summary>
        /// Commits an auto-hide transaction and puts associated windows in AutoHide dock state.
        /// </summary>
        /// <param name="transaction"></param>
        private void CommitAutoHideTransaction(AutoHideTransaction transaction)
        {
            //ensure that we have cleaned previously auto-hidden windows
            this.CleanAutoHideTabs(transaction.AssociatedWindows, true);

            int index = this.GetAutoHideStripIndex(transaction.Position);
            RadPageViewStripElement tabStripElement = this.GetAutoHideTabStrip(transaction.Position);
            bool offsetGroup = (tabStripElement.Items.Count > 0);

            //register an auto-hide group
            AutoHideGroup group = new AutoHideGroup(transaction.AssociatedWindows);
            this.autoHidePopup.ToolStrip.AddAutoHideGroup(group);

            foreach (DockWindow window in transaction.AssociatedWindows)
            {
                if (window.AutoHideSize.IsEmpty)
                {
                    if (window.DockTabStrip != null)
                    {
                        window.AutoHideSize = window.DockTabStrip.SizeInfo.AbsoluteSize;
                    }
                    else
                    {
                        window.AutoHideSize = SplitPanelSizeInfo.DefaultAbsoluteSize;
                    }
                }

                TabStripItem tabItem = new TabStripItem(window);
                window.AutoHideTab = tabItem;
                window.AutoHideGroup = group;
                tabStripElement.AddItem(tabItem);
                this.activateFromAutoHide = true;
                this.autoHidePopup.ToolStrip.TabPanels.Add(tabItem.TabPanel);
            }

            this.UpdateAutoHideTabStrips();
        }

        /// <summary>
        /// Commits a transaction associated with DockWindow calls.
        /// </summary>
        /// <param name="transaction"></param>
        private void CommitDockWindowTransaction(DockWindowTransaction transaction)
        {
            DockWindow window = transaction.AssociatedWindow;

            SplitPanel dockAnchor = transaction.DockAnchor;
            if (dockAnchor == null)
            {
                dockAnchor = this.GetDockWindowTarget(window, transaction.TargetWindow, transaction.Position);
                if (dockAnchor != null)
                {
                    transaction.TargetState = this.GetDockState(dockAnchor);
                }
            }

            switch (transaction.Position)
            {
                case DockPosition.Fill:
                    DockTabStrip strip = dockAnchor as DockTabStrip;
                    if (strip == null)
                    {
                        throw new ArgumentException("Invalid DockTabStrip for a DockPosition.Fill operation.");
                    }
                    this.InsertWindow(window, strip);
                    break;
                default:
                    DockType type = transaction.TargetState == DockState.TabbedDocument ? DockType.Document : DockType.ToolWindow;
                    DockTabStrip newStrip = this.CreateNewTabStrip(type);
                    if (window.DockTabStrip != null)
                    {
                        newStrip.SizeInfo.Copy(window.DockTabStrip.SizeInfo);
                    }
                    newStrip.TabPanels.Add(window);

                    if (newStrip.DockType == DockType.ToolWindow)
                    {
                        ((ToolTabStrip)newStrip).CurrentAutoHidePosition = DockHelper.GetAutoHidePosition(transaction.Position);
                    }

                    this.DockSplitPanel(newStrip, dockAnchor, transaction.Position);
                    break;
            }
        }

        /// <summary>
        /// Commits a transaction, created by the DragDrop service.
        /// </summary>
        /// <param name="transaction"></param>
        private void CommitDragDropTransaction(DragDropTransaction transaction)
        {
            SplitPanel toDock = null;
            //check whether we are dragging single window (if in preview mode)
            if (transaction.DraggedWindow != null)
            {
                if (transaction.TargetState == DockState.TabbedDocument)
                {
                    this.AddDocument(transaction.DraggedWindow, (DocumentTabStrip)transaction.DockAnchor, transaction.Position);
                    return;
                }

                toDock = this.CreateTabStripForWindow(transaction.DraggedWindow);
            }
            else
            {
                toDock = this.CloneSplitPanel(transaction.AssociatedPanel);
            }

            this.DockSplitPanel(toDock, transaction.DockAnchor, transaction.Position);
        }

        /// <summary>
        /// Commits a transaction, instanciated by the Redock service.
        /// </summary>
        /// <param name="transaction"></param>
        private void CommitRedockTransaction(RedockTransaction transaction)
        {
            RedockService service = this.GetService<RedockService>(ServiceConstants.Redock);
            if (service == null || !service.CanOperate())
            {
                return;
            }

            DockState newState = transaction.TargetState;
            List<DockWindow> unrestoredWindows = new List<DockWindow>();
            foreach (DockWindow window in transaction.AssociatedWindows)
            {
                RedockState state = service.GetState(window, newState, true);
                if (state == null || !this.RestoreState(window, state))
                {
                    unrestoredWindows.Add(window);
                }
            }

            if (!transaction.PerformDefaultAction || unrestoredWindows.Count == 0)
            {
                return;
            }

            //perform default redock action
            foreach (DockWindow window in unrestoredWindows)
            {
                switch (newState)
                {
                    case DockState.Docked:
                    case DockState.AutoHide:
                        this.DockWindow(window, DockPosition.Left);
                        break;
                    case DockState.Floating:
                        this.FloatWindow(window, Rectangle.Empty);
                        break;
                    case DockState.TabbedDocument:
                        this.AddDocument(window);
                        break;
                }

                transaction.AssociatedWindows.Remove(window);
            }
        }

        /// <summary>
        /// Commits a transaction, initiated by a request for a DockBehavior change.
        /// </summary>
        /// <param name="transaction"></param>
        private void CommitChangeBehaviorTransaction(RadDockTransaction transaction)
        {
        }

        /// <summary>
        /// Commits a transaction, which explicitly changes the state of its associated dock windows.
        /// </summary>
        /// <param name="transaction"></param>
        private void CommitChangeStateTransaction(RadDockTransaction transaction)
        {
        }

        /// <summary>
        /// Sets the DockState of each DockWindow, depending on the provided transaction.
        /// </summary>
        /// <param name="transaction"></param>
        private void UpdateDockState(RadDockTransaction transaction)
        {
            foreach (DockWindow window in transaction.AssociatedWindows)
            {
                bool changed = window.InnerDockState != transaction.TargetState;
                window.InnerDockState = transaction.TargetState;
                DockTabStrip strip = window.DockTabStrip;
                if (strip != null)
                {
                    strip.UpdateButtons();
                }

                if (changed)
                {
                    this.OnDockStateChanged(new DockWindowEventArgs(window));
                }
            }
        }

        /// <summary>
        /// Prepares and validates the transaction.
        /// </summary>
        /// <param name="transaction"></param>
        private void EnsureTransaction(RadDockTransaction transaction)
        {
            RedockService service = this.GetService<RedockService>(ServiceConstants.Redock);
            for (int i = transaction.AssociatedWindows.Count - 1; i >= 0; i--)
            {
                DockWindow window = transaction.AssociatedWindows[i];
                //we may have several Close transactions stacked
                if (window.IsDisposed || window.Disposing)
                {
                    transaction.AssociatedWindows.RemoveAt(i);
                    continue;
                }

                this.EnsureRegisteredWindow(window, transaction);

                if (!this.CanChangeWindowState(window, transaction.TargetState, window.DockState != transaction.TargetState))
                {
                    transaction.AssociatedWindows.RemoveAt(i);
                }
                else if (service != null)
                {
                    //save redock state if the window will be put in another state after the transaction
                    if (window.DockState != transaction.TargetState)
                    {
                        service.SaveState(window);
                    }
                }
            }
        }

        /// <summary>
        /// Ensures that any previously saved active window is re-activated after transaction commitment
        /// </summary>
        private void EnsureActiveWindow()
        {
            if (this.IsInitializing || !this.IsLoaded)
            {
                return;
            }

            DockWindow active = this.prevActiveWindow;
            if (active == null)
            {
                active = this.ActiveWindow;
                //we do not want to display an auto-hidden window after a transaction
                if (active != null && active.DockState == DockState.AutoHide && !this.autoHidePopup.Visible)
                {
                    this.SetDefaultActiveWindow();
                    active = this.activeWindow;
                }
            }

            if (active != null &&
                !(active.Disposing || active.IsDisposed || active.InnerDockState == DockState.Hidden))
            {
                bool notify = this.activeWindowChanged && this.prevActiveWindow == null;
                this.ActivateWindow(active, notify);
            }

            this.prevActiveWindow = null;
            this.activeWindowChanged = false;

            Form f = active == null ? this.FindForm() : active.FindForm();
            if (f != null && f != this.autoHidePopup && f.Visible)
            {
                f.Activate();
            }
        }

        private Rectangle GetDefaultFloatBounds(Size size)
        {
            Control parent = this.Parent;

            Rectangle screenBounds;
            if (parent != null)
            {
                screenBounds = parent.RectangleToScreen(this.Bounds);
            }
            else
            {
                screenBounds = this.RectangleToScreen(this.ClientRectangle);
            }

            return DockHelper.CenterRect(screenBounds, size);
        }

        #endregion

        #region Events

        /// <summary>
        /// Raised whenever a new <see cref="DockTabStrip">DockTabStrip</see> instance is needed internally by the framework.
        /// Allows for providing custom <see cref="ToolTabStrip">ToolTabStrip</see> and <see cref="DocumentTabStrip">DocumentTabStrip</see> implementations.
        /// </summary>
        public event DockTabStripNeededEventHandler DockTabStripNeeded
        {
            add
            {
                this.Events.AddHandler(DockTabStripNeededEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(DockTabStripNeededEventKey, value);
            }
        }

        /// <summary>
        /// Fires after RadPageViewElement is created.
        /// </summary>
        public event PageViewInstanceCreatedEventHandler PageViewInstanceCreated
        {
            add
            {
                this.Events.AddHandler(PageViewInstanceCreatedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PageViewInstanceCreatedEventKey, value);
            }
        }

        /// <summary>
        /// Raised whenever a new <see cref="RadSplitContainer">RadSplitContainer</see> instance is needed internally by the framework.
        /// Allows for providing custom <see cref="RadSplitContainer">RadSplitContainer</see> implementation.
        /// </summary>
        public event SplitContainerNeededEventHandler SplitContainerNeeded
        {
            add
            {
                this.Events.AddHandler(SplitContainerNeededEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(SplitContainerNeededEventKey, value);
            }
        }

        /// <summary>
        /// Notifies for a successful BeginTransactionBlock operation.
        /// </summary>
        public event EventHandler TransactionBlockStarted
        {
            add
            {
                this.Events.AddHandler(TransactionBlockStartedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(TransactionBlockStartedEventKey, value);
            }
        }

        /// <summary>
        /// Notifies for a successful EndTransactionBlock operation, when all transactions are committed, the DockTree is cleaned, and updates are resumed.
        /// </summary>
        public event EventHandler TransactionBlockEnded
        {
            add
            {
                this.Events.AddHandler(TransactionBlockEndedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(TransactionBlockEndedEventKey, value);
            }
        }

        /// <summary>
        /// Raised when a RadDockTransaction is successfully committed.
        /// Allows listeners to perform some additional operations.
        /// </summary>
        public event RadDockTransactionEventHandler TransactionCommitted
        {
            add
            {
                this.Events.AddHandler(TransactionCommittedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(TransactionCommittedEventKey, value);
            }
        }

        /// <summary>
        /// Raised when a RadDockTransaction is about to be committed.
        /// Allows listeners to investigate the transaction, perform some additional actions and/or cancel it.
        /// </summary>
        public event RadDockTransactionCancelEventHandler TransactionCommitting
        {
            add
            {
                this.Events.AddHandler(TransactionCommittingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(TransactionCommittingEventKey, value);
            }
        }

        /// <summary>
        /// Notifies for a new DockWindow registered with this RadDock instance.
        /// </summary>
        public event DockWindowEventHandler DockWindowAdded
        {
            add
            {
                base.Events.AddHandler(RadDock.DockWindowAddedEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(RadDock.DockWindowAddedEventKey, value);
            }
        }

        /// <summary>
        /// Notifies for a DockWindow removed from this RadDock instance. This event will not be raised for hidden windows.
        /// </summary>
        public event DockWindowEventHandler DockWindowRemoved
        {
            add
            {
                base.Events.AddHandler(DockWindowRemovedEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(DockWindowRemovedEventKey, value);
            }
        }

        /// <summary>
        /// Raised before a DockWindow.Close method is called.
        /// </summary>
        public event DockWindowCancelEventHandler DockWindowClosing
        {
            add
            {
                base.Events.AddHandler(DockWindowClosingEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(DockWindowClosingEventKey, value);
            }
        }

        /// <summary>
        /// Raised after a DockWindow has been closed.
        /// </summary>
        public event DockWindowEventHandler DockWindowClosed
        {
            add
            {
                base.Events.AddHandler(DockWindowClosedEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(DockWindowClosedEventKey, value);
            }
        }

        /// <summary>
        /// Notifies for an upcomming change in the DockState of the associated window. Cancelable.
        /// </summary>
        public event DockStateChangingEventHandler DockStateChanging
        {
            add
            {
                base.Events.AddHandler(DockStateChangingEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(DockStateChangingEventKey, value);
            }
        }

        /// <summary>
        /// Notifies for a change in the DockState of the associated window.
        /// </summary>
        public event DockWindowEventHandler DockStateChanged
        {
            add
            {
                base.Events.AddHandler(DockStateChangedEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(DockStateChangedEventKey, value);
            }
        }

        /// <summary>
        /// Notifies for an upcomming change of the ActiveWindow property. Cancelable.
        /// </summary>
        public event DockWindowCancelEventHandler ActiveWindowChanging
        {
            add
            {
                base.Events.AddHandler(ActiveWindowChangingEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(ActiveWindowChangingEventKey, value);
            }
        }


        /// <summary>
        /// Occurs when selected tab changing in currently manipulated DockTabStrip.
        /// </summary>
        public event SelectedTabChangingEventHandler SelectedTabChanging
        {
            add
            {
                base.Events.AddHandler(SelectedTabChangingEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(SelectedTabChangingEventKey, value);
            }
        }

        /// <summary>
        /// Occurs when selected tab changed in currently manipulated DockTabStrip.
        /// </summary>
        public event SelectedTabChangedEventHandler SelectedTabChanged
        {
            add
            {
                base.Events.AddHandler(SelectedTabChanedEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(SelectedTabChanedEventKey, value);
            }
        }

        /// <summary>
        /// Notifies for an actual change of the ActiveWindow property.
        /// </summary>
        public event DockWindowEventHandler ActiveWindowChanged
        {
            add
            {
                base.Events.AddHandler(ActiveWindowChangedEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(ActiveWindowChangedEventKey, value);
            }
        }

        /// <summary>
        /// Notifies that a FloatingWindow instance is internally created by the framework.
        /// Allows listeners to examine and optionally change the window itself.
        /// </summary>
        public event FloatingWindowEventHandler FloatingWindowCreated
        {
            add
            {
                this.Events.AddHandler(FloatingWindowCreatedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(FloatingWindowCreatedEventKey, value);
            }
        }

        /// <summary>
        /// Raises the FloatingWindowCreated event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnFloatingWindowCreated(FloatingWindowEventArgs e)
        {
            FloatingWindowEventHandler eh = this.Events[FloatingWindowCreatedEventKey] as FloatingWindowEventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        /// <summary>
        /// Raises the TransactionCommitting event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTransactionCommitting(RadDockTransactionCancelEventArgs e)
        {
            RadDockTransactionCancelEventHandler eh = this.Events[TransactionCommittingEventKey] as RadDockTransactionCancelEventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        /// <summary>
        /// Raises the TransactionCommitted event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTransactionCommitted(RadDockTransactionEventArgs e)
        {
            RadDockTransactionEventHandler eh = this.Events[TransactionCommittedEventKey] as RadDockTransactionEventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        /// <summary>
        /// Raises the TransactionBlockEnded event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTransactionBlockEnded(EventArgs e)
        {
            EventHandler eh = this.Events[TransactionBlockEndedEventKey] as EventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="TransactionBlockStarted"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnTransactionBlockStarted(EventArgs e)
        {
            EventHandler eh = this.Events[TransactionBlockStartedEventKey] as EventHandler;
            if (eh != null)
            {
                eh(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RadDock.ActiveWindowChanging">ActiveWindowChanging</see> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnActiveWindowChanging(DockWindowCancelEventArgs e)
        {
            DockWindowCancelEventHandler handler1 = (DockWindowCancelEventHandler)base.Events[RadDock.ActiveWindowChangingEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        protected virtual void OnSelectedTabChanging(SelectedTabChangingEventArgs e)
        {
            SelectedTabChangingEventHandler handler1 = (SelectedTabChangingEventHandler)base.Events[RadDock.SelectedTabChangingEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        protected virtual void OnSelectedTabChanged(SelectedTabChangedEventArgs e)
        {
            SelectedTabChangedEventHandler handler1 = (SelectedTabChangedEventHandler)base.Events[RadDock.SelectedTabChanedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RadDock.ActiveWindowChanged">ActiveWindowChanged</see> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnActiveWindowChanged(DockWindowEventArgs e)
        {
            //notify the document manager for the change
            this.documentManager.OnActiveWindowChanged(this.activeWindow);

            DockWindowEventHandler handler1 = (DockWindowEventHandler)base.Events[RadDock.ActiveWindowChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RadDock.DockWindowClosing">DockWindowClosing</see> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDockWindowClosing(DockWindowCancelEventArgs e)
        {
            //allow the DockWindow itself to examine the Close request and optionally cancel it.
            e.NewWindow.OnClosing(e);
            if (e.Cancel)
            {
                return;
            }

            DockWindowCancelEventHandler handler1 = (DockWindowCancelEventHandler)base.Events[RadDock.DockWindowClosingEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RadDock.DockWindowClosed">DockWindowClosed</see> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDockWindowClosed(DockWindowEventArgs e)
        {
            //allow the DockWindow itself to perform additional actions upon close.
            e.DockWindow.OnClosed(e);

            DockWindowEventHandler handler1 = (DockWindowEventHandler)base.Events[RadDock.DockWindowClosedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RadDock.DockStateChanging">DockStateChanging</see> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDockStateChanging(DockStateChangingEventArgs e)
        {
            DockStateChangingEventHandler handler1 = (DockStateChangingEventHandler)base.Events[RadDock.DockStateChangingEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RadDock.DockStateChanged">DockStateChanged</see> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDockStateChanged(DockWindowEventArgs e)
        {
            //allow document manager to update itself
            this.documentManager.OnDockWindowDockStateChanged(e.DockWindow);

            DockWindowEventHandler handler1 = (DockWindowEventHandler)base.Events[RadDock.DockStateChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RadDock.DockWindowAdded">DockWindowAdded</see> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDockWindowAdded(DockWindowEventArgs e)
        {
            this.documentManager.OnDockWindowAdded(e.DockWindow);
            DockWindowEventHandler handler1 = (DockWindowEventHandler)base.Events[RadDock.DockWindowAddedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Raises the <see cref="RadDock.DockWindowRemoved">DockWindowRemoved</see> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnDockWindowRemoved(DockWindowEventArgs e)
        {
            this.documentManager.OnDockWindowRemoved(e.DockWindow);
            DockWindowEventHandler handler1 = (DockWindowEventHandler)base.Events[RadDock.DockWindowRemovedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        #endregion

        #region Notifications

        /// <summary>
        /// Receives a notification from a tooltabstrip that the mouse was clicked inside its area.
        /// The default implementation will try to restore the state of the associated windows.
        /// </summary>
        /// <param name="strip"></param>
        /// <param name="click"></param>
        protected internal virtual void OnToolTabStripDoubleClick(ToolTabStrip strip, Point click)
        {
            if (!this.ShouldProcessNotification())
            {
                return;
            }

            RedockService service = this.GetService<RedockService>(ServiceConstants.Redock);
            if (service == null || !service.CanOperate())
            {
                return;
            }

            DockWindow window = strip.ActiveWindow;
            if (window == null)
            {
                return;
            }

            RedockTransaction transaction = null;
            DockState state = RedockService.GetNewDockState(window.DockState);

            if (strip.CaptionElement.ControlBoundingRectangle.Contains(click))
            {
                transaction = new RedockTransaction(state, ControlHelper.GetChildControls<DockWindow>(strip), true);
                transaction.Reason = RedockTransactionReason.ToolTabStripCaptionDoubleClick;
            }
            else
            {
                TabItem item = strip.ElementTree.GetElementAtPoint(click) as TabItem;
                if (item != null)
                {
                    transaction = new RedockTransaction(state, new DockWindow[] { window }, true);
                    transaction.Reason = RedockTransactionReason.TabItemDoubleClick;
                }
            }

            if (transaction != null)
            {
                this.RegisterTransaction(transaction, true);
            }
        }

        /// <summary>
        /// An Auto-hide button click notification, received from a registered ToolTabStrip.
        /// </summary>
        /// <param name="strip"></param>
        protected internal virtual void OnAutoHideButtonClicked(ToolTabStrip strip)
        {
            DockWindow activeWindow = strip.ActiveWindow;
            Debug.Assert(activeWindow != null, "Must have active window at this point.");

            if (activeWindow.DockState == DockState.Docked)
            {
                this.SetWindowState(activeWindow, DockState.AutoHide);
            }
            else if (activeWindow.DockState == DockState.AutoHide)
            {
                this.SetWindowState(activeWindow, DockState.Docked);
            }
        }

        /// <summary>
        /// Recieves a notification for a change state request, made from the window's associated context menu.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="newState"></param>
        protected internal virtual void OnDockStateContextMenuClicked(DockWindow window, DockState newState)
        {
            this.SetWindowState(window, newState);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        protected internal override void OnControlTreeChanged(ControlTreeChangedEventArgs args)
        {
            base.OnControlTreeChanged(args);

            if (!this.ShouldProcessNotification())
            {
                return;
            }

            switch (args.Action)
            {
                case ControlTreeChangeAction.Add:
                    this.RegisterControl(args.Child);
                    break;
                case ControlTreeChangeAction.Remove:
                    this.UnregisterControl(args.Child);
                    break;
            }
        }

        /// <summary>
        /// The manager gets notified for a change in the specified <see cref="Telerik.WinControls.UI.Docking.DockWindow">DockWindow</see> instance Name property.
        /// </summary>
        /// <param name="dockWindow">The DockWindow which name has changed.</param>
        /// <param name="oldName">The window's previous name.</param>
        protected internal virtual void OnDockWindowNameChanged(DockWindow dockWindow, string oldName)
        {
            Debug.Assert(this.attachedWindows.ContainsKey(oldName), "Dock window is not attached!");
            //remove and re-insert the specified window to update its key in the list.
            this.attachedWindows.Remove(oldName);
            this.attachedWindows.Add(dockWindow.Name, dockWindow);
        }

        /// <summary>
        /// Registers all DockWindow or DockTabStrip instances, residing in the provided control tree.
        /// </summary>
        /// <param name="parent"></param>
        private void RegisterControl(Control parent)
        {
            if (parent is DockWindow)
            {
                DockWindow window = (DockWindow)parent;
                this.EnsureRegisteredWindow(window, null);
            }
            else if (parent is DockTabStrip)
            {
                this.EnsureRegisteredStrip((DockTabStrip)parent);
            }
            else if (parent is DocumentContainer)
            {
                ((DocumentContainer)parent).DockManager = this;
            }
            else if (parent is RadSplitContainer)
            {
                this.EnsureRegisteredContainer(parent as RadSplitContainer);
            }

            //do not go into another nested RadDock's control tree.
            if (parent is RadDock && parent != this)
            {
                return;
            }

            //recursively register entire control tree
            foreach (Control child in parent.Controls)
            {
                RegisterControl(child);
            }
        }

        private void EnsureRegisteredStrip(DockTabStrip strip)
        {
            if (strip == null)
            {
                return;
            }

            strip.DockManager = this;
            strip.ThemeName = this.ThemeName;
            if (string.IsNullOrEmpty(strip.Name))
            {
                //specify unique name for the strip, since hidden windows serialization will need it
                strip.Name = "DockTabStrip" + ++DockTabStripCounter;
            }
        }

        private void EnsureRegisteredContainer(RadSplitContainer container)
        {
            if (container == null)
            {
                return;
            }

            container.splitContainerElement.SetDefaultValueOverride(SplitContainerElement.SplitterWidthProperty, this.SplitterWidth);
            container.IsCleanUpTarget = true;
            container.LayoutStrategy = this.CreateLayoutStrategy();
            container.ThemeClassName = DockSplitContainerThemeClassName;
            container.ThemeName = this.ThemeName;
        }

        /// <summary>
        /// Unregisters all DockWindow or DockTabStrip instances, residing in the provided control tree.
        /// </summary>
        /// <param name="parent"></param>
        private void UnregisterControl(Control parent)
        {
            if (parent is DockWindow)
            {
                DockWindow window = (DockWindow)parent;
                if (this.attachedWindows.ContainsValue(window))
                {
                    this.UnregisterWindow(window, null);
                }
            }
            else if (parent is DockTabStrip)
            {
                ((DockTabStrip)parent).DockManager = null;
            }

            foreach (Control child in parent.Controls)
            {
                this.UnregisterControl(child);
            }
        }

        internal bool ShouldProcessNotification()
        {
            if (this.isDisposing || this.IsInitializing || this.transactionBlockCount > 0 || this.cleaningUp)
            {
                return false;
            }

            return !(this.Disposing || this.IsDisposed);
        }

        #endregion

        #region Dock Operations

        /// <summary>
        /// Adds the specified DockWindow instance to the main document tab strip.
        /// </summary>
        /// <param name="dockWindow">The dock window.</param>
        public void AddDocument(DockWindow dockWindow)
        {
            this.AddDocument(dockWindow, DockPosition.Fill);
        }

        /// <summary>
        /// Adds the the specified DockWindow instance at the specified dock position, aligned with the main document tabstrip.
        /// </summary>
        /// <param name="dockWindow">The dock window.</param>
        /// <param name="position">The desired dock position.</param>
        public void AddDocument(DockWindow dockWindow, DockPosition position)
        {
            this.AddDocument(dockWindow, this.GetDefaultDocumentTabStrip(true), position);
        }

        /// <summary>
        /// Adds the the specified DockWindow instance to the specified DocumentTabStrip instance.
        /// </summary>
        /// <param name="dockWindow">The dock window.</param>
        /// <param name="target">The target of the dock opeation.</param>
        /// <param name="position">The desired dock position.</param>
        public void AddDocument(DockWindow dockWindow, DockWindow target, DockPosition position)
        {
            DocumentTabStrip strip = target == null ? null : target.DockTabStrip as DocumentTabStrip;
            if (strip == null)
            {
                strip = this.GetDefaultDocumentTabStrip(true);
            }

            this.AddDocument(dockWindow, strip, position);
        }

        /// <summary>
        /// Adds the specified DockWindow instance to a new DocumentTabStrip instance.
        /// </summary>
        /// <param name="dockWindow">The dock window.</param>
        /// <param name="tabStrip">The DocumentTabStrip, which is the target of the operation.</param>
        /// <param name="position">The position.</param>
        /// <returns></returns>
        public void AddDocument(DockWindow dockWindow, DocumentTabStrip tabStrip, DockPosition position)
        {
            if (dockWindow == null)
            {
                throw new ArgumentNullException("DockWindow");
            }

            if (tabStrip == null)
            {
                tabStrip = this.GetDefaultDocumentTabStrip(true);
            }

            if (dockWindow.DockType == DockType.ToolWindow)
            {
                //store the previous state for the window if we are adding a ToolWindow as a document
                dockWindow.PreviousDockState = dockWindow.DockState;
            }

            this.RegisterTransaction(new DockWindowTransaction(DockState.TabbedDocument, dockWindow, null, tabStrip, position), false);
        }

        /// <summary>
        /// Move DockWindow to previuos document tab strip if exist
        /// when DockWindow is in document mode
        /// </summary>
        public bool MoveToPreviousDocumentTabStrip(DockWindow dockWindow)
        {
            DocumentTabStrip prev = this.GetPreviousDocumentStrip(dockWindow);
            if (prev == null)
            {
                return false;
            }

            this.AddDocument(dockWindow, prev, DockPosition.Fill);
            return true;
        }

        /// <summary>
        /// Move DockWindow to next document tab strip if exist
        /// when DockWindow is in document mode
        /// </summary>
        public bool MoveToNextDocumentTabStrip(DockWindow dockWindow)
        {
            DocumentTabStrip next = this.GetNextDocumentStrip(dockWindow);
            if (next == null)
            {
                return false;
            }

            this.AddDocument(dockWindow, next, DockPosition.Fill);
            return true;
        }

        /// <summary>
        /// Gets the previous document tab strip, regarding the specified DockWindow.
        /// The window should be in TabbedDocument state for the method to work.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public DocumentTabStrip GetPreviousDocumentStrip(DockWindow window)
        {
            DocumentTabStrip currStrip = window.DockTabStrip as DocumentTabStrip;
            if (currStrip == null)
            {
                return null;
            }

            currStrip = ControlHelper.GetNextControl<DocumentTabStrip>(this.MainDocumentContainer, currStrip, true, false, false);
            if (currStrip != null && currStrip.DockManager == this)
            {
                return currStrip;
            }

            return null;
        }

        /// <summary>
        /// Gets the next document tab strip, regarding the specified DockWindow.
        /// The window should be in TabbedDocument state for the method to work.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public DocumentTabStrip GetNextDocumentStrip(DockWindow window)
        {
            DocumentTabStrip currStrip = window.DockTabStrip as DocumentTabStrip;
            if (currStrip == null)
            {
                return null;
            }

            currStrip = ControlHelper.GetNextControl<DocumentTabStrip>(this.MainDocumentContainer, currStrip, true, true, false);
            if (currStrip != null && currStrip.DockManager == this)
            {
                return currStrip;
            }

            return null;
        }

        /// <summary>
        /// Determines whether the specified DockWindow is registered with this RadDock instances.
        /// </summary>
        /// <param name="dockWindow"></param>
        /// <returns></returns>
        public bool Contains(DockWindow dockWindow)
        {
            return this.attachedWindows.ContainsValue(dockWindow);
        }

        /// <summary>
        /// Determines whether a DockWindow instance with the specified name is registered with this RadDock instance.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Contains(string name)
        {
            return this.attachedWindows.ContainsKey(name);
        }

        /// <summary>
        /// Get the host window for Control instance docked with DockControl method or for MDI children Form object
        /// </summary>
        /// <param name="control"></param>
        /// <returns>found HostWindow instance or null</returns>
        public HostWindow GetHostWindow(Control control)
        {
            if (control == null)
            {
                return null;
            }

            HostWindow window = control.Parent as HostWindow;
            if (window != null && window.DockManager == this)
            {
                return window;
            }

            return null;
        }

        /// <summary>Activates the MDI child of a form.</summary>
        /// <param name="form">The child form to activate.</param>
        public void ActivateMdiChild(Form form)
        {
            HostWindow hostWindow = this.GetHostWindow(form);
            if (hostWindow != null)
            {
                this.ActivateWindow(hostWindow);
            }
        }

        /// <summary>
        /// Registers the specified control as part of the docking framework.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="position"></param>
        public HostWindow DockControl(Control control, DockPosition position)
        {
            HostWindow hostDockWindow = new HostWindow(control);
            DockWindow(hostDockWindow, position);

            return hostDockWindow;
        }

        /// <summary>
        /// Registers the specified control as part of the docking framework.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="position"></param>
        /// <param name="dockType"></param>
        public HostWindow DockControl(Control control, DockPosition position, DockType dockType)
        {
            HostWindow hostDockWindow = new HostWindow(control, dockType);
            DockWindow(hostDockWindow, position);

            return hostDockWindow;
        }

        /// <summary>
        /// Registers the specified control as part of the docking framework.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="target"></param>
        /// <param name="position"></param>
        public HostWindow DockControl(Control control, DockWindow target, DockPosition position)
        {
            DockTabStrip anchor = target == null ? null : target.DockTabStrip;
            return DockControl(control, anchor, position);
        }

        /// <summary>
        /// Registers the specified control as part of the docking framework.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="target"></param>
        /// <param name="position"></param>
        /// <param name="dockType"></param>
        public HostWindow DockControl(Control control, DockWindow target, DockPosition position, DockType dockType)
        {
            DockTabStrip anchor = target == null ? null : target.DockTabStrip;
            return DockControl(control, anchor, position, dockType);
        }

        /// <summary>
        /// Registers the specified control as part of the docking framework.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="dockAnchor"></param>
        /// <param name="position"></param>
        public HostWindow DockControl(Control control, DockTabStrip dockAnchor, DockPosition position)
        {
            HostWindow hostDockWindow = new HostWindow(control);
            DockWindow(hostDockWindow, dockAnchor, position);

            return hostDockWindow;
        }

        /// <summary>
        /// Registers the specified control as part of the docking framework.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="dockAnchor"></param>
        /// <param name="position"></param>
        /// <param name="dockType"></param>
        public HostWindow DockControl(Control control, DockTabStrip dockAnchor, DockPosition position, DockType dockType)
        {
            HostWindow hostDockWindow = new HostWindow(control, dockType);
            DockWindow(hostDockWindow, dockAnchor, position);

            return hostDockWindow;
        }

        /// <summary>
        /// Registers the specified DockWindow instance and docks it at the desired psotion.
        /// </summary>
        /// <param name="dockWindow"></param>
        /// <param name="position"></param>
        public void DockWindow(DockWindow dockWindow, DockPosition position)
        {
            DockWindow target = null;
            DockWindow(dockWindow, target, position);
        }

        /// <summary>
        /// Registers the specified DockWindow instance and docks it at the desired psotion, using the provided target window as an anchor.
        /// </summary>
        /// <param name="dockWindow"></param>
        /// <param name="target"></param>
        /// <param name="position"></param>
        public void DockWindow(DockWindow dockWindow, DockWindow target, DockPosition position)
        {
            DockTabStrip targetStrip = this.GetDockWindowTarget(dockWindow, target, position);
            DockState targetState = this.GetDockState(targetStrip);
            this.RegisterTransaction(new DockWindowTransaction(targetState, dockWindow, target, targetStrip, position), false);
        }

        /// <summary>
        /// Registers the specified DockWindow instance and docks it at the desired position, using the provided SplitPanel as an anchor.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="dockAnchor">Any split panel instance that resides </param>
        /// <param name="position"></param>
        public void DockWindow(DockWindow window, SplitPanel dockAnchor, DockPosition position)
        {
            DockState targetState = this.GetDockState(dockAnchor);
            this.RegisterTransaction(new DockWindowTransaction(targetState, window, null, dockAnchor, position), false);
        }

        /// <summary>
        /// Removes the specified window, without disposing it, from the dock manager.
        /// </summary>
        /// <param name="window"></param>
        public void RemoveWindow(DockWindow window)
        {
            RemoveWindow(window, DockWindowCloseAction.Close);
        }

        /// <summary>
        /// Removes the specified window, using the specified DockWindowCloseAction.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="closeAction">The desired action to be taken.</param>
        public void RemoveWindow(DockWindow window, DockWindowCloseAction closeAction)
        {
            if (!this.attachedWindows.ContainsValue(window))
            {
                return;
            }

            window.CloseAction = closeAction;
            this.CloseWindow(window);
        }

        /// <summary>
        /// Removes all DockWindows, without disposing them.
        /// </summary>
        public void RemoveAllWindows()
        {
            this.RemoveAllWindows(DockWindowCloseAction.Close);
        }

        /// <summary>
        /// Removes all DockWindows, using the specified close action.
        /// </summary>
        /// <param name="closeAction">The action to be taken when closing a window.</param>
        public void RemoveAllWindows(DockWindowCloseAction closeAction)
        {
            this.BeginTransactionBlock(false);

            foreach (DockWindow window in this.attachedWindows.Values)
            {
                this.RemoveWindow(window, closeAction);
            }

            this.EndTransactionBlock();
        }

        /// <summary>
        /// Removes all DocumentWindows, without disposing it.
        /// </summary>
        public void RemoveAllDocumentWindows()
        {
            this.RemoveAllDocumentWindows(DockWindowCloseAction.Close);
        }

        /// <summary>
        /// Removes all DocumentWindows, using the specified close action.
        /// </summary>
        /// <param name="closeAction">The action to be taken when closing a window.</param>
        public void RemoveAllDocumentWindows(DockWindowCloseAction closeAction)
        {
            this.BeginTransactionBlock(false);

            foreach (DockWindow window in DockHelper.GetDockWindows(this.MainDocumentContainer, true, this))
            {
                this.RemoveWindow(window, closeAction);
            }

            this.EndTransactionBlock();
        }

        /// <summary>
        /// Calls the Close method for all currently registered windows, using each window's CloseAction.
        /// </summary>
        public void CloseAllWindows()
        {
            this.BeginTransactionBlock(false);

            foreach (DockWindow window in this.attachedWindows.Values)
            {
                this.RemoveWindow(window, window.CloseAction);
            }

            this.EndTransactionBlock();
        }

        /// <summary>
        /// Gets the internal sorted list, which stores 
        /// </summary>
        internal SortedList<string, DockWindow> InnerList
        {
            get
            {
                return this.attachedWindows;
            }
        }

        /// <summary>
        /// Gets the default DocumentTabStrip instance, used to add documents without explicitly specified dock target.
        /// </summary>
        /// <param name="createNew">True to create a new DocumentTabStrip instance and add it to the document container.</param>
        /// <returns></returns>
        public DocumentTabStrip GetDefaultDocumentTabStrip(bool createNew)
        {
            //get the parent of the currently active window
            DockWindow activeDocument = this.documentManager.ActiveDocument;
            if (activeDocument != null)
            {
                DocumentTabStrip currActive = activeDocument.DockTabStrip as DocumentTabStrip;
                if (currActive != null)
                {
                    return currActive;
                }
            }

            DocumentTabStrip firstStrip = null;
            foreach (Control child in ControlHelper.EnumChildControls(this, true))
            {
                firstStrip = child as DocumentTabStrip;
                if (firstStrip != null && firstStrip.DockManager == this)
                {
                    return firstStrip;
                }
            }

            if (createNew)
            {
                firstStrip = this.CreateNewTabStrip(DockType.Document) as DocumentTabStrip;
                this.MainDocumentContainer.SplitPanels.Add(firstStrip);
            }

            return firstStrip;
        }

        /// <summary>
        /// Enumerates all currently alive T instances available within the framework.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> EnumFrameworkControls<T>() where T : Control
        {
            if (this.isDisposing || this.IsDisposed)
            {
                yield break;
            }

            foreach (Control child in ControlHelper.EnumChildControls(this, true))
            {
                if (child is T)
                {
                    yield return (T)child;
                }
            }

            foreach (FloatingWindow window in this.floatingWindows)
            {
                foreach (Control child in ControlHelper.EnumChildControls(window, true))
                {
                    if (child is T)
                    {
                        yield return (T)child;
                    }
                }
            }
        }

        /// <summary>
        /// Applies the LayoutStrategy as defined by the layoutStrategyType field.
        /// </summary>
        private void ApplyLayoutStrategy()
        {
            //apply the new strategy
            this.BeginUpdate();

            foreach (RadSplitContainer container in DockHelper.GetSplitContainers(this, true, this))
            {
                container.LayoutStrategy = this.CreateLayoutStrategy();
            }

            foreach (FloatingWindow window in this.floatingWindows)
            {
                foreach (RadSplitContainer container in DockHelper.GetSplitContainers(window, true, this))
                {
                    container.LayoutStrategy = this.CreateLayoutStrategy();
                }
            }

            this.EndUpdate();
        }

        /// <summary>
        /// Creates new SplitContainerLayoutStrategy instance. Allows inheritors to provide custom type.
        /// </summary>
        /// <returns></returns>
        protected virtual SplitContainerLayoutStrategy CreateLayoutStrategy()
        {
            SplitContainerLayoutStrategy strategy = null;
            if (this.layoutStrategyType != null)
            {
                try
                {
                    strategy = Activator.CreateInstance(this.layoutStrategyType) as SplitContainerLayoutStrategy;
                }
                catch
                {
                    strategy = null;
                }
            }

            if (strategy == null)
            {
                strategy = new SplitContainerLayoutStrategy();
            }

            strategy.RootContainerType = typeof(RadDock);
            return strategy;
        }

        private DockTabStrip GetDockWindowTarget(DockWindow forWindow, DockWindow target, DockPosition position)
        {
            DockTabStrip targetStrip = null;
            if (target != null)
            {
                targetStrip = target.DockTabStrip;
            }
            if (targetStrip == null && forWindow.DockType == DockType.Document)
            {
                targetStrip = this.GetDefaultDocumentTabStrip(true);
            }

            return targetStrip;
        }

        private void SaveAutoHidePosition(SplitPanel panel, DockPosition position)
        {
            foreach (ToolTabStrip toolTabStrip in DockHelper.GetDockTabStrips<ToolTabStrip>(panel, true, this))
            {
                toolTabStrip.CurrentAutoHidePosition = DockHelper.GetAutoHidePosition(position);
            }
        }

        internal FloatingWindow CreateFloatingWindow()
        {
            FloatingWindow window = new FloatingWindow(this);
            window.Owner = this.FindForm();
            window.ThemeName = this.ThemeName;
            window.DockContainer.splitContainerElement.SetDefaultValueOverride(SplitContainerElement.SplitterWidthProperty, this.SplitterWidth);
            if (window.Owner != null)
            {
                window.TopMost = window.Owner.TopMost;
            }

            FloatingWindowEventArgs args = new FloatingWindowEventArgs(window);
            this.OnFloatingWindowCreated(args);
            //get the instance from the event arguments as the user may have changed it
            window = args.Window;

            this.floatingWindows.InnerList.Add(window);
            window.Disposed += delegate(object sender, EventArgs e)
            {
                this.floatingWindows.InnerList.Remove((FloatingWindow)sender);
            };

            return window;
        }

        protected internal override void ApplySplitterWidth(int splitterWidth)
        {
            base.ApplySplitterWidth(splitterWidth);

            foreach (FloatingWindow window in this.floatingWindows)
            {
                window.DockContainer.splitContainerElement.SetDefaultValueOverride(SplitContainerElement.SplitterWidthProperty, splitterWidth);
            }
        }

        private Orientation GetDockOrientation(DockPosition pos)
        {
            Orientation orientation = Orientation.Vertical;
            if (pos == DockPosition.Top || pos == DockPosition.Bottom)
            {
                orientation = Orientation.Horizontal;
            }

            return orientation;
        }

        private void RegisterWindow(DockWindow dockWindow)
        {
            if (this.attachedWindows.ContainsValue(dockWindow))
            {
                return;
            }

            if (attachedWindows.ContainsKey(dockWindow.Name))
            {
                throw new InvalidOperationException("DockWindow with same name already exist in RadDock.");
            }

            //check whether the window currently resides on another dock manager
            if (dockWindow.DockManager != null)
            {
                throw new ArgumentException("The specified window is already registered with another dock manager");
            }

            if (string.IsNullOrEmpty(dockWindow.Name))
            {
                int windowCounter = 0;
                int toolWindowCounter = 0;
                int documentWindowCounter = 0;
                do
                {
                    if (dockWindow is HostWindow)
                    {
                        dockWindow.Name = ((HostWindow)dockWindow).Content.GetType().Name + (++windowCounter).ToString();
                    }
                    else if (dockWindow.DockType == DockType.Document)
                    {
                        int nextNumber = ++documentWindowCounter;
                        dockWindow.Name = "documentWindow" + nextNumber;
                    }
                    else
                    {
                        int nextNumber = ++toolWindowCounter;
                        dockWindow.Name = "toolWindow" + nextNumber;
                    }
                } while (this.attachedWindows.ContainsKey(dockWindow.Name));
            }

            this.attachedWindows.Add(dockWindow.Name, dockWindow);
            dockWindow.DockManager = this;
            dockWindow.Disposed += OnDockWindowDisposed;
        }

        //private void OnDockWindowDisposed(object sender, EventArgs e)
        //{
        //    if (!this.ShouldProcessNotification())
        //    {
        //        return;
        //    }

        //    DockWindow window = (DockWindow)sender;
        //    this.ClearActiveWindowReferences(window);

        //    this.UnregisterWindow(window, null);
        //    this.CleanAutoHideTabs(new DockWindow[] { window }, true);
        //    this.CleanUp();
        //}

        /*
         * 将以上注释的函数修改为该函数
         * 目的是解决停靠窗口关闭时不能释放资源的问题
         */
        private void OnDockWindowDisposed(object sender, EventArgs e)
        {
            //if (!this.ShouldProcessNotification())
            //{
            //    return;
            //}

            DockWindow window = (DockWindow)sender;
            this.ClearActiveWindowReferences(window);

            this.UnregisterWindow(window, null);
            this.CleanAutoHideTabs(new DockWindow[] { window }, true);
            this.CleanUp();
        }

        private void ClearActiveWindowReferences(DockWindow window)
        {
            if (window == this.activeWindow)
            {
                this.activeWindow = null;
            }
            else if (window == this.prevActiveWindow)
            {
                this.prevActiveWindow = null;
            }
        }

        private void UnregisterWindow(DockWindow dockWindow, RadDockTransaction transaction)
        {
            this.attachedWindows.Remove(dockWindow.Name);
            dockWindow.Disposed -= OnDockWindowDisposed;

            RedockService service = this.GetService<RedockService>(ServiceConstants.Redock);
            if (service != null)
            {
                service.ClearAllStates(dockWindow);
            }
            dockWindow.DockManager = null;

            if (transaction != null)
            {
                transaction.RemovedWindows.Add(dockWindow);
            }
            else
            {
                OnDockWindowRemoved(new DockWindowEventArgs(dockWindow));
            }
        }

        private void EnsureRegisteredWindow(DockWindow window, RadDockTransaction transaction)
        {
            if (!this.attachedWindows.ContainsValue(window))
            {
                this.RegisterWindow(window);
                if (transaction != null)
                {
                    transaction.AddedWindows.Add(window);
                }
                else
                {
                    OnDockWindowAdded(new DockWindowEventArgs(window));
                }
            }

            this.EnsureRegisteredStrip(window.DockTabStrip);
            window.DockManager = this;

            if (string.IsNullOrEmpty(window.Text))
            {
                window.Text = window.Name;
            }
        }

        private SplitPanel CloneSplitPanel(SplitPanel panel)
        {
            SplitPanel newPanel;
            if (panel is RadSplitContainer)
            {
                newPanel = this.CloneContainer((RadSplitContainer)panel);
            }
            else
            {
                DockTabStrip strip = (DockTabStrip)panel;
                newPanel = this.CloneTabStrip(strip);
            }

            return newPanel;
        }

        private DockTabStrip CloneTabStrip(DockTabStrip strip)
        {
            DockTabStrip newStrip = this.CreateNewTabStrip(strip.DockType);

            //get sizing information from the current strip
            strip.CopyTo(newStrip);

            //add all panels into the new strip
            while (strip.TabPanels.Count > 0)
            {
                newStrip.TabPanels.Add(strip.TabPanels[0]);
            }

            return newStrip;
        }

        private RadSplitContainer CloneContainer(RadSplitContainer source)
        {
            RadSplitContainer container = this.CreateNewSplitContainer();
            container.Orientation = source.Orientation;
            container.SizeInfo.Copy(source.SizeInfo);

            for (int i = 0; i < source.SplitPanels.Count; i++)
            {
                SplitPanel panel = source.SplitPanels[i];
                SplitPanel newPanel = null;
                if (panel is DockTabStrip)
                {
                    DockTabStrip strip = (DockTabStrip)panel;
                    if (strip.TabPanels.Count > 0)
                    {
                        newPanel = this.CloneTabStrip(strip);
                    }
                }
                else if (panel is RadSplitContainer)
                {
                    newPanel = this.CloneContainer((RadSplitContainer)panel);
                }

                if (newPanel != null)
                {
                    container.SplitPanels.Add(newPanel);
                }
            }

            return container;
        }

        private DockTabStrip CreateNewTabStrip(DockType type)
        {
            DockTabStrip strip = null;
            DockTabStripNeededEventArgs args = null;

            if (this.DesignMode)
            {
                IDesignerHost host = this.DesignerHost;
                if (host != null)
                {
                    if (type == DockType.Document)
                    {
                        strip = host.CreateComponent(typeof(DocumentTabStrip)) as DockTabStrip;
                    }
                    else
                    {
                        strip = host.CreateComponent(typeof(ToolTabStrip)) as DockTabStrip;
                    }
                }
            }
            else
            {
                //allow for custom DocumentTabStrip and ToolTabStrip implementations by raising the DockTabStripNeeded event
                DockTabStripNeededEventHandler eh = this.Events[DockTabStripNeededEventKey] as DockTabStripNeededEventHandler;
                if (eh != null)
                {
                    args = new DockTabStripNeededEventArgs(type);
                    eh(this, args);
                    strip = args.Strip;
                }
            }

            if (strip == null)
            {
                strip = this.CreateDefaultTabStrip(type);

                PageViewInstanceCreatedEventHandler eh = this.Events[PageViewInstanceCreatedEventKey] as PageViewInstanceCreatedEventHandler;
                if (eh != null)
                {
                    PageViewInstanceCreatedEventArgs eventArgs = new PageViewInstanceCreatedEventArgs(strip.TabStripElement);
                    eh(this, eventArgs);
                }
            }

            this.UpdateNewStripProperties(strip, args);
            this.EnsureRegisteredStrip(strip);

            return strip;
        }


        /// <summary>
        /// Creates and returns the default DockTabStrip for a given DockType.
        /// </summary>
        [Description("Creates and returns the default DockTabStrip for a given DockType")]
        public DockTabStrip CreateDefaultTabStrip(DockType type)
        {
            DockTabStrip strip;

            if (type == DockType.Document)
            {
                strip = new DocumentTabStrip(this);
            }
            else
            {
                strip = new ToolTabStrip(this);
            }

            return strip;
        }

        private void UpdateNewStripProperties(DockTabStrip strip, DockTabStripNeededEventArgs args)
        {
            //apply text orientation
            if (args != null && args.TabStripTextOrientation != TabStripTextOrientation.Default)
            {
                strip.TabStripTextOrientation = args.TabStripTextOrientation;
            }
            else
            {
                strip.TabStripTextOrientation = strip.DockType == DockType.Document ? this.documentTabsTextOrientation : this.toolTabsTextOrientation;
            }

            //apply tab alignment
            if (args != null && args.TabStripAlignment != TabStripAlignment.Default)
            {
                strip.TabStripAlignment = args.TabStripAlignment;
            }
            else
            {
                strip.TabStripAlignment = strip.DockType == DockType.Document ? this.documentTabsAlignment : this.toolTabsAlignment;
            }

            //apply tabstrip visibility
            if (args != null && args.TabStripVisible != null)
            {
                strip.TabStripVisible = args.TabStripVisible.Value;
            }
            else
            {
                strip.TabStripVisible = strip.DockType == DockType.Document ? this.documentTabsVisible : this.toolTabsVisible;
            }

            //apply tabstrip CloseButton
            if (args != null && args.ShowCloseButton != null)
            {
                strip.ShowItemCloseButton = args.ShowCloseButton.Value;
            }
            else
            {
                strip.ShowItemCloseButton = strip.DockType == DockType.Document ? this.showDocumentCloseButton : this.showToolCloseButton;
            }
        }

        private RadSplitContainer CreateNewSplitContainer()
        {
            RadSplitContainer container = null;

            if (this.DesignMode)
            {
                IDesignerHost host = this.DesignerHost;
                if (host != null)
                {
                    container = host.CreateComponent(typeof(RadSplitContainer)) as RadSplitContainer;
                }
            }
            else
            {
                //allow for custom RadSplitContainer implementations by raising the SplitContainerNeeded event
                SplitContainerNeededEventHandler eh = this.Events[SplitContainerNeededEventKey] as SplitContainerNeededEventHandler;
                if (eh != null)
                {
                    SplitContainerNeededEventArgs args = new SplitContainerNeededEventArgs();
                    eh(this, args);
                    container = args.Container;
                }
            }

            //no custom container, create default instance
            if (container == null)
            {
                container = new RadSplitContainer();
            }

            this.EnsureRegisteredContainer(container);

            return container;
        }

        private DockTabStrip CreateTabStripForWindow(DockWindow window)
        {
            DockTabStrip strip = this.CreateNewTabStrip(window.DockType);

            if (window.TabStrip != null)
            {
                strip.SizeInfo.Copy(window.TabStrip.SizeInfo);
            }
            strip.TabPanels.Add(window);

            return strip;
        }

        internal void RestoreWindows(IEnumerable<DockWindow> windows, DockState state)
        {
        }

        private bool RestoreState(DockWindow window, RedockState state)
        {
            DockTabStrip targetStrip = state.TargetStrip;
            //for some reason the redock target strip is either disposed or not accessible
            if (targetStrip == null)
            {
                return false;
            }

            //int index = Math.Min(targetStrip.TabPanels.Count, state.TabIndex);
            //targetStrip.TabPanels.Insert(index, window);
            targetStrip.TabPanels.Add(window);
            targetStrip.IsRedockTarget = false;
            targetStrip.UpdateTabSelection(false);

            return true;
        }

        internal DockState GetDockState(Control child)
        {
            if (child == null)
            {
                return DockState.Docked;
            }

            Form f = child.FindForm();
            if (f is FloatingWindow && (f as FloatingWindow).DockManager == this)
            {
                return DockState.Floating;
            }

            if (f is AutoHidePopup && (f as AutoHidePopup).DockManager == this)
            {
                return DockState.AutoHide;
            }

            if (child is DocumentWindow && (child as DocumentWindow).DockManager == this)
            {
                return DockState.TabbedDocument;
            }

            if (child is DocumentTabStrip && (child as DocumentTabStrip).DockManager == this)
            {
                return DockState.TabbedDocument;
            }

            return DockState.Docked;
        }

        /// <summary>
        /// Inserts all DockWindow instances that reside on the specified split panel in the provided strip's TabPanels collection.
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="strip"></param>
        private void AddAsTabItems(SplitPanel panel, DockTabStrip strip)
        {
            if (strip == null)
            {
                throw new ArgumentNullException("Null DockPosition.Fill target");
            }

            List<DockWindow> windows = DockHelper.GetDockWindows(panel, true, this);
            foreach (DockWindow window in windows)
            {
                InsertWindow(window, strip);
            }
        }

        private void CleanAutoHideTabs(IEnumerable<DockWindow> windows, bool updatePopup)
        {
            foreach (DockWindow window in windows)
            {
                TabStripItem tab = window.AutoHideTab;
                if (tab == null)
                {
                    continue;
                }

                tab.Owner.RemoveItem(tab);
                tab.Dispose();
                window.AutoHideTab = null;
                if (window.AutoHideGroup != null)
                {
                    window.AutoHideGroup.Windows.Remove(window);
                }
                window.AutoHideGroup = null;
            }

            this.autoHidePopup.ToolStrip.CleanUpGroups();

            if (updatePopup)
            {
                this.CloseAutoHidePopup();
                this.UpdateAutoHideTabStrips();
            }
        }

        /// <summary>
        /// Docks the provided SplitPanel at the specified position.
        /// Used internally by the framework to dock DockTabStrips and entire containers.
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="dockAnchor"></param>
        /// <param name="position"></param>
        private void DockSplitPanel(SplitPanel panel, SplitPanel dockAnchor, DockPosition position)
        {
            switch (position)
            {
                case DockPosition.Fill:
                    this.AddAsTabItems(panel, dockAnchor as DockTabStrip);
                    break;
                default:
                    this.SaveAutoHidePosition(panel, position);

                    RadSplitContainer container = panel as RadSplitContainer;
                    if (container != null && container.SplitPanels.Count == 1)
                    {
                        //we do not want to add whole container with one strip only
                        panel = container.SplitPanels[0];
                    }
                    RadSplitContainer targetContainer = (dockAnchor != null) ? dockAnchor.SplitContainer : this;
                    if (targetContainer == null)
                    {
                        targetContainer = this;
                    }

                    this.PerformDock(targetContainer, panel, dockAnchor, position);
                    break;
            }
        }

        /// <summary>
        /// Performs the core dock operation and splits the layout in different orientations if needed.
        /// </summary>
        /// <param name="targetContainer"></param>
        /// <param name="panel"></param>
        /// <param name="dockAnchor"></param>
        /// <param name="position"></param>
        private void PerformDock(RadSplitContainer targetContainer, SplitPanel panel, SplitPanel dockAnchor, DockPosition position)
        {
            targetContainer.SuspendLayout();

            Orientation orientation = this.GetDockOrientation(position);
            bool lead = (position == DockPosition.Left || position == DockPosition.Top);

            //check whether orientation is the same
            if (targetContainer.Orientation == orientation)
            {
                int insertIndex;
                if (dockAnchor != null)
                {
                    insertIndex = targetContainer.SplitPanels.IndexOf(dockAnchor);
                    insertIndex = lead ? insertIndex : insertIndex + 1;
                }
                else
                {
                    insertIndex = lead ? 0 : targetContainer.SplitPanels.Count;
                }
                panel.Parent = null;
                targetContainer.SplitPanels.Insert(insertIndex, panel);
            }
            else
            {
                //orientation is different, we need to reflect this
                RadSplitContainer newContainer = this.CreateNewSplitContainer();
                newContainer.SuspendLayout();

                if (dockAnchor != null)
                {
                    newContainer.Orientation = orientation;
                    //copy the strip's size info to the new container
                    newContainer.SizeInfo.Copy(dockAnchor.SizeInfo);

                    int index = targetContainer.SplitPanels.IndexOf(dockAnchor);
                    //dockAnchor.Parent = null;
                    newContainer.SplitPanels.Add(dockAnchor);
                    newContainer.SplitPanels.Insert((lead) ? 0 : 1, panel);
                    targetContainer.SplitPanels.Insert(index, newContainer);
                }
                else
                {
                    newContainer.Orientation = targetContainer.Orientation;
                    //copy the strip's size info to the new container
                    newContainer.SizeInfo.Copy(panel.SizeInfo);

                    while (targetContainer.SplitPanels.Count > 0)
                    {
                        newContainer.SplitPanels.Add(targetContainer.SplitPanels[0]);
                    }

                    targetContainer.Orientation = orientation;
                    targetContainer.SplitPanels.Add(newContainer);
                    targetContainer.SplitPanels.Insert((lead) ? 0 : targetContainer.SplitPanels.Count, panel);
                }

                newContainer.ResumeLayout(true);
            }

            targetContainer.ResumeLayout(true);
        }

        /// <summary>
        /// 
        /// </summary>
        protected internal override void UpdateCollapsed()
        {
            this.Collapsed = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="specified"></param>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);

            if (this.autoHidePopup != null && this.autoHidePopup.Visible)
            {
                this.UpdateAutoHidePopupBounds();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientPoint"></param>
        /// <returns></returns>
        public override SplitterElement GetSplitterElementAtPoint(Point clientPoint)
        {
            if (!this.ShouldProcessNotification())
            {
                return null;
            }

            return base.GetSplitterElementAtPoint(clientPoint);
        }

        /// <summary>
        /// Determines whether the provided dock state may be applied to the specified DockWindow.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool CanChangeWindowState(DockWindow window, DockState state)
        {
            return this.CanChangeWindowState(window, state, false);
        }

        /// <summary>
        /// Determines whether the specified window's DockState value can be changed to the specified new one.
        /// </summary>
        /// <param name="window">The window, which should be examined.</param>
        /// <param name="state">The new target state.</param>
        /// <param name="raiseChanging">True to raise DockStateChanging notification, false otherwise.</param>
        /// <returns>True if the state can be changed, false otherwise.</returns>
        protected internal virtual bool CanChangeWindowState(DockWindow window, DockState state, bool raiseChanging)
        {
            if (this.IsInitializing)
            {
                return false;
            }

            AllowedDockState allowedState = DockHelper.GetAllowedDockState(state);
            if ((window.AllowedDockState & allowedState) != allowedState)
            {
                return false;
            }

            if (!this.IsDockStateTransitionAllowed(window, state))
            {
                return false;
            }

            if (raiseChanging)
            {
                DockStateChangingEventArgs args = new DockStateChangingEventArgs(window, state);
                this.OnDockStateChanging(args);
                if (args.Cancel)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsDockStateTransitionAllowed(DockWindow window, DockState newState)
        {
            if (newState == DockState.TabbedDocument && !this.mainDocumentContainerVisible)
            {
                return false;
            }

            DockState currState = window.DockState;

            //eliminate invalid state transitions
            switch (currState)
            {
                case DockState.TabbedDocument:
                    if (newState == DockState.AutoHide)
                    {
                        return false;
                    }
                    if (newState == DockState.Floating || newState == DockState.Docked)
                    {
                        return this.treatTabbedDocumentsAsToolWindows || window.DockType == DockType.ToolWindow;
                    }
                    break;
                case DockState.Floating:
                    if (newState == DockState.AutoHide)
                    {
                        return false;
                    }
                    break;
                case DockState.AutoHide:
                    if (newState == DockState.Floating || newState == DockState.TabbedDocument)
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }

        private IDesignerHost DesignerHost
        {
            get
            {
                return this.GetService(typeof(IDesignerHost)) as IDesignerHost;
            }
        }

        private void InsertWindow(DockWindow window, DockTabStrip parent)
        {
            DockWindowInsertOrder order;
            if (parent.DockType == DockType.ToolWindow)
            {
                order = this.ToolWindowInsertOrder;
            }
            else
            {
                order = this.documentManager.DocumentInsertOrder;
            }

            if (order == DockWindowInsertOrder.InFront)
            {
                parent.TabPanels.Insert(0, window);
            }
            else
            {
                parent.TabPanels.Add(window);
            }
        }

        #endregion

        #region State Transitions

        /// <summary>
        /// Makes the specified window floating.
        /// </summary>
        /// <param name="window"></param>
        public void FloatWindow(DockWindow window)
        {
            this.FloatWindows(new DockWindow[] { window }, Rectangle.Empty);
        }

        /// <summary>
        /// Makes the specified window floating, using the provided bounds.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="bounds">The desired floating bounds. Pass Rectangle.Empty to set the default bounds.</param>
        public void FloatWindow(DockWindow window, Rectangle bounds)
        {
            this.FloatWindows(new DockWindow[] { window }, bounds);
        }

        /// <summary>
        /// Makes the specified windows floating, using the provided bounds.
        /// </summary>
        /// <param name="windows"></param>
        /// <param name="bounds">The desired floating bounds. Pass Rectangle.Empty to set the default bounds.</param>
        public void FloatWindows(IEnumerable<DockWindow> windows, Rectangle bounds)
        {
            FloatTransaction transaction = new FloatTransaction(windows, null, bounds);
            this.RegisterTransaction(transaction, false);
        }

        /// <summary>
        /// Makes the specified tool tab strip floating.
        /// </summary>
        /// <param name="strip"></param>
        /// <param name="bounds">The desired floating bounds. Pass Rectangle.Empty to set the default bounds.</param>
        public void FloatToolTabStrip(ToolTabStrip strip, Rectangle bounds)
        {
            IEnumerable<DockWindow> windows = ControlHelper.GetChildControls<DockWindow>(strip);
            FloatTransaction transaction = new FloatTransaction(windows, strip, bounds);
            this.RegisterTransaction(transaction, false);
        }

        /// <summary>
        /// Makes the specified DockWindow auto-hidden.
        /// </summary>
        /// <param name="window"></param>
        public void AutoHideWindow(DockWindow window)
        {
            IEnumerable<DockWindow> windows;

            AutoHidePosition position = AutoHidePosition.Left;
            //An auto-hide operation will hide entire tabstrip
            ToolTabStrip strip = window.DockTabStrip as ToolTabStrip;
            if (strip != null)
            {
                windows = DockHelper.GetDockWindows(strip, true, this);
                position = this.GetAutoHidePosition(strip);
            }
            else
            {
                windows = new DockWindow[] { window };
            }

            this.AutoHideWindows(windows, position);
        }

        /// <summary>
        /// Makes the specified DockWindow collection auto-hidden.
        /// </summary>
        /// <param name="windows"></param>
        /// <param name="position">The edge at which the windows should be auto-hidden against.</param>
        public void AutoHideWindows(IEnumerable<DockWindow> windows, AutoHidePosition position)
        {
            AutoHideTransaction transaction = new AutoHideTransaction(windows, position);
            this.RegisterTransaction(transaction, false);
        }

        /// <summary>
        /// Removes or hides (depending on the CloseAction) the specified window.
        /// </summary>
        /// <param name="window"></param>
        public void CloseWindow(DockWindow window)
        {
            if (window == null || window.IsDisposed)
            {
                return;
            }
            if (window != null && window.CloseAction == DockWindowCloseAction.Hide && window.DockState == DockState.Hidden)
            {
                return;
            }
            this.CloseWindows(new DockWindow[] { window });
        }

        /// <summary>
        /// Removes or hides (depending on the CloseAction) the specified windows.
        /// </summary>
        /// <param name="windows"></param>
        public void CloseWindows(IEnumerable<DockWindow> windows)
        {
            CloseTransaction transaction = new CloseTransaction(windows);
            this.RegisterTransaction(transaction, false);
        }

        /// <summary>
        /// Displays the specified window if was previously hidden.
        /// </summary>
        /// <param name="window"></param>
        public void DisplayWindow(DockWindow window)
        {
            this.DisplayWindows(new DockWindow[] { window });
        }

        /// <summary>
        /// Displays the provided dock windows if they were previously hidden and are registered with this RadDock instance.
        /// </summary>
        /// <param name="windows"></param>
        public void DisplayWindows(IEnumerable<DockWindow> windows)
        {
            List<DockWindow> allowed = new List<DockWindow>();
            foreach (DockWindow window in windows)
            {
                if (window.DockState == DockState.Hidden && this.attachedWindows.ContainsValue(window))
                {
                    allowed.Add(window);
                }
            }

            if (allowed.Count == 0)
            {
                return;
            }

            RedockService service = this.GetService<RedockService>(ServiceConstants.Redock);
            if (service != null)
            {
                service.Redock(allowed, true);
            }
            else
            {
                this.BeginTransactionBlock();
                foreach (DockWindow window in windows)
                {
                    this.DockWindow(window, DockPosition.Left);
                }
                this.EndTransactionBlock();
            }
        }

        /// <summary>
        /// Applies the desired DockState to the specified DockWindow.
        /// If a previously RedockState is saved for the desired DockState, this state is restored, else the default action is performed.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="state"></param>
        public void SetWindowState(DockWindow window, DockState state)
        {
            if (window.DockState == state)
            {
                return;
            }

            if (this.IsInitializing || this.DesignMode)
            {
                window.DesiredDockState = state;
                return;
            }

            IEnumerable<DockWindow> windows = this.GetWindowsForChangeState(window, state);
            RedockService service = this.GetService<RedockService>(ServiceConstants.Redock);

            switch (state)
            {
                case DockState.AutoHide:
                    ToolTabStrip toolStrip = window.DockTabStrip as ToolTabStrip;
                    AutoHidePosition position = AutoHidePosition.Left;
                    if (toolStrip != null)
                    {
                        position = this.GetAutoHidePosition(toolStrip);
                    }
                    this.AutoHideWindows(windows, position);
                    break;
                case DockState.Docked:
                    if (service != null)
                    {
                        service.RestoreState(windows, DockState.Docked, true);
                        break;
                    }
                    foreach (DockWindow toDock in windows)
                    {
                        this.DockWindow(toDock, DockPosition.Left);
                    }
                    break;
                case DockState.Floating:
                    if (service != null)
                    {
                        service.RestoreState(windows, DockState.Floating, true);
                    }
                    else
                    {
                        this.FloatWindows(windows, Rectangle.Empty);
                    }
                    break;
                case DockState.Hidden:
                    this.CloseWindows(windows);
                    break;
                case DockState.TabbedDocument:
                    foreach (DockWindow document in windows)
                    {
                        this.AddDocument(document);
                    }
                    break;
            }
        }

        private AutoHidePosition GetAutoHidePosition(ToolTabStrip toolStrip)
        {
            if (toolStrip.AutoHidePosition != AutoHidePosition.Auto)
            {
                return toolStrip.AutoHidePosition;
            }

            if (!this.mainDocumentContainerVisible)
            {
                return toolStrip.CurrentAutoHidePosition;
            }

            SplitPanel docContainer = this.MainDocumentContainer;
            //consider strip's alignment against the main document strip
            RadSplitContainer container = DockHelper.FindCommonAncestor(toolStrip, docContainer);
            if (container == null)
            {
                return toolStrip.CurrentAutoHidePosition;
            }

            SplitPanel containingToolStrip = DockHelper.GetDirectChildContainingPanel(container, toolStrip);
            if (containingToolStrip == null)
            {
                return toolStrip.CurrentAutoHidePosition;
            }

            SplitPanel containingDocumentStrip = DockHelper.GetDirectChildContainingPanel(container, docContainer);
            if (containingDocumentStrip == null)
            {
                return toolStrip.CurrentAutoHidePosition;
            }

            int index1 = container.SplitPanels.IndexOf(containingToolStrip);
            int index2 = container.SplitPanels.IndexOf(containingDocumentStrip);

            AutoHidePosition position;
            if (container.Orientation == Orientation.Horizontal)
            {
                position = index1 > index2 ? AutoHidePosition.Bottom : AutoHidePosition.Top;
            }
            else
            {
                position = index1 > index2 ? AutoHidePosition.Right : AutoHidePosition.Left;
            }

            return position;
        }

        private IEnumerable<DockWindow> GetWindowsForChangeState(DockWindow window, DockState state)
        {
            IEnumerable<DockWindow> windows = new DockWindow[] { window };
            //transition from auto-hidden to another state is associated with a group of windows.
            if (window.DockState == DockState.AutoHide)
            {
                AutoHideGroup group = this.autoHidePopup.ToolStrip.GetAutoHideGroup(window);
                Debug.Assert(group != null, "An auto-hidden dock window must be associated with an auto-hide group");

                //a hide operation will affect only one window (active one), while a restore operation will affect all grouped windows.
                if (state != DockState.Hidden)
                {
                    windows = new List<DockWindow>(group.Windows);
                }
            }
            else if (state == DockState.AutoHide)
            {
                DockTabStrip strip = window.DockTabStrip;
                if (strip != null)
                {
                    windows = ControlHelper.GetChildControls<DockWindow>(strip);
                }
            }

            return windows;
        }

        #endregion

        #endregion

        #region Commands

        /// <summary>
        /// Gets the <see cref="RadDockCommandManager">CommandManager</see> registered with this RadDock instance.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadDockCommandManager CommandManager
        {
            get
            {
                return this.commandManager;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (this.MainDocumentContainer.Parent == null)
            {
                this.MainDocumentContainer.Parent = this;
            }

            if (this.commandManager != null)
            {
                this.commandManager.Enabled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            if (!this.Disposing && !this.IsDisposed && this.commandManager != null)
            {
                this.commandManager.Enabled = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            if (this.commandManager != null)
            {
                this.commandManager.Enabled = this.Enabled;
            }
        }

        #endregion

        #region Document Management

        /// <summary>
        /// Gets the <see cref="DocumentManager">DocumentManager</see> instance that manages all the DockWindow instances which are parented <see cref="RadDock.MainDocumentContainer">MainDocumentContainer</see>.
        /// For a Document is considered either a DocumentWindow instance or a ToolWindow, which current <see cref="Telerik.WinControls.UI.Docking.DockWindow.DockState">DockState</see> is <see cref="Telerik.WinControls.UI.Docking.DockState.TabbedDocument">TabbedDocument</see>.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public DocumentManager DocumentManager
        {
            get
            {
                return this.documentManager;
            }
        }

        /// <summary>
        /// Gets or sets the document container for this RadDock instance.
        /// This property is used primarily for serialization purposes and is not intended to be used directly by code.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public DocumentContainer MainDocumentContainer
        {
            get
            {
                if (this.mainDocumentContainer == null)
                {
                    this.CreateMainDocumentContainer();
                }

                return this.mainDocumentContainer;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("MainDocumentContainer");
                }

                if (this.mainDocumentContainer == value)
                {
                    return;
                }

                this.DestroyMainDocumentContainer();

                this.mainDocumentContainer = value;
                this.AttachDocumentContainer();
            }
        }

        /// <summary>
        /// Determines whether the main document container is visible.
        /// </summary>
        [DefaultValue(true)]
        [Description("Determines whether the main document container is visible.")]
        public bool MainDocumentContainerVisible
        {
            get
            {
                return this.mainDocumentContainerVisible;
            }
            set
            {
                if (this.mainDocumentContainerVisible == value)
                {
                    return;
                }

                this.mainDocumentContainerVisible = value;
                if (this.mainDocumentContainer != null)
                {
                    this.mainDocumentContainer.UpdateCollapsed();
                }
            }
        }

        #endregion

        #region Service Support

        /// <summary>
        /// Registers the specified service with ourselves.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="service"></param>
        public void RegisterService(int key, RadDockService service)
        {
            RadDockService oldService;
            this.services.TryGetValue(key, out oldService);

            if (oldService != null && oldService != service)
            {
                oldService.DockManager = null;
            }

            if (service != null)
            {
                service.DockManager = this;
            }

            this.services[key] = service;
        }

        /// <summary>
        /// Retrieves currently registered <see cref="RadDockService">Service</see> by the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetService<T>() where T : RadDockService
        {
            foreach (RadDockService service in this.services.Values)
            {
                if (service is T)
                {
                    return (T)service;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves currently registered Service by the provided key.
        /// All predefined service keys may be found in ServiceConstants class.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        protected internal T GetService<T>(int key) where T : RadDockService
        {
            RadDockService service;
            this.services.TryGetValue(key, out service);

            return service as T;
        }

        #endregion

        #region Drag & Drop

        /// <summary>
        /// Gets or sets the template, which defines the appearance of the guides displayed upon drag-and-drop operation.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IDockingGuidesTemplate DockingGuidesTemplate
        {
            get
            {
                if (this.guidesTemplate == null)
                {
                    ThemedDockingGuidesTemplate themedTemplate = new ThemedDockingGuidesTemplate();
                    themedTemplate.ThemeName = this.ThemeName;
                    this.guidesTemplate = themedTemplate;
                }
                return this.guidesTemplate;
            }
            set
            {
                if (this.guidesTemplate != value)
                {
                    IDisposable disposable = this.guidesTemplate as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                    this.guidesTemplate = value;
                    DragDropService service = this.GetService<DragDropService>(ServiceConstants.DragDrop);
                    if (service != null)
                    {
                        service.OnDockingGuidesTemplateChanged();
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the DragDropService is currently working (a drag-and-drop operation is running).
        /// </summary>
        [Browsable(false)]
        public bool IsDragging
        {
            get
            {
                DragDropService service = this.GetService<DragDropService>(ServiceConstants.DragDrop);
                if (service != null)
                {
                    return service.State == ServiceState.Working;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets or sets the mode used by the DragDropService when a drag-and-drop request is made.
        /// </summary>
        [DefaultValue(DragDropMode.Auto)]
        [Description("Gets or sets the mode used by the DragDropService when a drag-and-drop request is made.")]
        public DragDropMode DragDropMode
        {
            get
            {
                DragDropService service = this.GetService<DragDropService>(ServiceConstants.DragDrop);
                if (service != null)
                {
                    return service.DragDropMode;
                }

                return DragDropMode.Immediate;
            }
            set
            {
                DragDropService service = this.GetService<DragDropService>(ServiceConstants.DragDrop);
                if (service != null)
                {
                    service.DragDropMode = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the allowed dock states for a committing drag-and-drop operation.
        /// </summary>
        [DefaultValue(AllowedDockState.All)]
        [Description("Gets or sets the allowed dock states for a committing drag-and-drop operation.")]
        public AllowedDockState DragDropAllowedDockStates
        {
            get
            {
                DragDropService service = this.GetService<DragDropService>(ServiceConstants.DragDrop);
                if (service != null)
                {
                    return service.AllowedStates;
                }

                return AllowedDockState.All;
            }
            set
            {
                DragDropService service = this.GetService<DragDropService>(ServiceConstants.DragDrop);
                if (service != null)
                {
                    service.AllowedStates = value;
                }
            }
        }

        //TODO: This functionality is not completely tested yet, that's why it is internal
        /// <summary>
        /// Determines whether tabbed documents will have special behavior that differs from a ToolWindow's one.
        /// For example if this is true, a tabbed document will be allowed to become floating by a drag-and-drop operation.
        /// </summary>
        [DefaultValue(false)]
        [Description("Determines whether tabbed documents will have special behavior that differs from a ToolWindow's one. For example if this is true, a tabbed document will be allowed to become floating by a drag-and-drop operation.")]
        internal bool TreatTabbedDocumentsAsToolWindows
        {
            get
            {
                return this.treatTabbedDocumentsAsToolWindows;
            }
            set
            {
                this.treatTabbedDocumentsAsToolWindows = value;
            }
        }

        internal void BeginDrag(object data)
        {
            if (!this.ShouldProcessNotification())
            {
                return;
            }

            DragDropService service = this.GetService<DragDropService>(ServiceConstants.DragDrop);
            if (service == null || service.State == ServiceState.Working || service.DragDropBehavior == DragDropBehavior.Manual)
            {
                return;
            }

            //hide AutoHidePopup (if displayed)
            if (this.autoHidePopup.Visible)
            {
                this.CloseAutoHidePopup();
            }

            //start the drag-and-drop operation
            service.Start(data);
        }

        internal void EndDrag(bool commit)
        {
            DragDropService service = this.GetService<DragDropService>(ServiceConstants.DragDrop);
            if (service == null || service.State != ServiceState.Working)
            {
                return;
            }

            service.Stop(commit);
        }

        #endregion

        #region Serialization

        /// <summary>
        /// 
        /// </summary>
        public override ISite Site
        {
            get
            {
                return base.Site;
            }
            set
            {
                base.Site = value;
                if (value != null)
                {
                    IContainer container = value.Container;
                    if (container != null)
                    {
                        container.Add(this.MainDocumentContainer);
                    }
                }
            }
        }

        /// <summary>
        /// Notifies that manager's state has been loaded from an external XML source.
        /// </summary>
        public event EventHandler LoadedFromXml;

        /// <summary>
        /// Notifies that manager's state has been saved to an external XML source.
        /// </summary>
        public event EventHandler SavedToXml;

        /// <summary>
        /// Raises the <see cref="RadDock.LoadedFromXml">LoadedFromXml</see> event.
        /// </summary>
        protected virtual void OnLoadedFromXml()
        {
            foreach (DockTabStrip strip in this.EnumFrameworkControls<DockTabStrip>())
            {
                strip.UpdateAfterTransaction();
            }

            EventHandler handler = this.LoadedFromXml;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// Raises the <see cref="RadDock.SavedToXml">SavedToXml</see> event.
        /// </summary>
        protected virtual void OnSavedToXml()
        {
            EventHandler handler = this.SavedToXml;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// Loads the docking layout configuration from a file. 
        /// This method will try to guess if the xml in the file was written with 
        /// the DockingManager or with the new RadDock in order to determine how to 
        /// read it successfully.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void LoadFromXml(string fileName)
        {
            bool oldXmlFormat = false;
            using (XmlReader reader = XmlReader.Create(fileName))
            {
                oldXmlFormat = RadDock.IsOldXmlFormat(reader);
            }

            using (XmlReader reader = XmlReader.Create(fileName))
            {
                this.LoadFromXmlCore(reader, oldXmlFormat);
            }
        }

        /// <summary>
        /// Loads the docking layout configuration from a stream.
        /// This method will try to guess if the xml in the stream was written with 
        /// the DockingManager or with the new RadDock in order to determine how to 
        /// read it successfully.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void LoadFromXml(Stream stream)
        {
            if (stream == null)
            {
                return;
            }

            bool oldXmlFormat = false;
            if (stream.CanSeek)
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    oldXmlFormat = RadDock.IsOldXmlFormat(reader);
                }

                //seek to 
                stream.Seek(0, SeekOrigin.Begin);
            }
            using (XmlReader reader = XmlReader.Create(stream))
            {
                this.LoadFromXmlCore(reader, oldXmlFormat);
            }
        }


        /// <summary>
        /// Loads the docking layout configuration from a TextReader.
        /// Note that this is a new method and does not support loading xml 
        /// that was written using DockingManager.
        /// </summary>
        /// <param name="textReader">The text reader.</param>
        public void LoadFromXml(TextReader textReader)
        {
            bool oldXmlFormat = false;
            using (XmlReader reader = XmlReader.Create(textReader))
            {
                this.LoadFromXmlCore(reader, oldXmlFormat);
            }
        }

        /// <summary>
        /// Overrwrite this method to change the loading of docking layout configuration.
        /// </summary>
        /// <param name="reader">The XmlReader instance that will be used to load the layoiut configuration.</param>
        /// <param name="oldXmlFormat">if set to <c>true</c> the layout configuration is in the old DockingManager format.</param>
        protected virtual void LoadFromXmlCore(XmlReader reader, bool oldXmlFormat)
        {
            if (reader == null)
            {
                return;
            }

            this.activeWindow = null;
            this.prevActiveWindow = null;
            this.CleanAutoHideTabs(this.attachedWindows.Values, true);
            this.ResetDesiredDockState();

            this.documentManager.OnLoadingFromXML();

            if (oldXmlFormat)
            {
                this.BeginTransactionBlock(false);
                this.LoadFromOldXml(reader);
                this.EndTransactionBlock();
            }
            else
            {
                this.LoadFromNewXml(reader);
            }

            this.EnsureInitialized();
            this.OnLoadedFromXml();
        }

        private void ResetDesiredDockState()
        {
            foreach (DockWindow window in this.attachedWindows.Values)
            {
                window.ResetDesiredDockState();
            }
        }

        private void ClearControls(Control.ControlCollection collection)
        {
            foreach (Control control in collection)
            {
                control.Dispose();
            }
            collection.Clear();
        }

        private void ClearDocking()
        {
            List<DockWindow> dockWindows1 = new List<DockWindow>();
            foreach (DockWindow dockWindow in this.attachedWindows.Values)
            {
                if (dockWindow.DockState == DockState.Floating)
                {
                    dockWindows1.Add(dockWindow);
                }
            }

            foreach (DockWindow floatingWindow in dockWindows1)
            {
                if (!floatingWindow.IsDisposed)
                {
                    FloatingWindow floatingForm = floatingWindow.FindForm() as FloatingWindow;
                    floatingForm.Dispose();
                }
            }

            this.ClearControls(this.autoHidePopup.Controls);
            this.ClearControls(this.Controls);

            this.attachedWindows.Clear();
        }

        private void LoadFromOldXml(XmlReader reader)
        {
            XmlDockingManagerSerializer serializer = new XmlDockingManagerSerializer();
            XmlDockingManager xmlManager = serializer.Deserialize(reader) as XmlDockingManager;

            this.SuspendLayout();

            //this.ClearDocking();

            RadDockComponentFactory componentFactory = null;
            if (DesignMode)
            {
                IDesignerHost host = null;
                host = (IDesignerHost)this.Site.GetService(typeof(IDesignerHost));
                componentFactory = new DesignTimeRadDockComponentFactory(host);
            }
            else
            {
                componentFactory = new RunTimeRadDockComponentFactory();
            }
            List<SplitPanel> list = xmlManager.Deserialize(this, componentFactory);

            this.RegisterControl(this);
            foreach (DockWindow window in this.attachedWindows.Values)
            {
                this.SetWindowState(window, window.DesiredDockState);
            }

            this.ResumeLayout();
        }

        private void LoadFromNewXml(XmlReader reader)
        {
            loadLayout = true;

            this.BeginTransactionBlock(false);
            DockXmlSerializer serializer = new DockXmlSerializer(this, this.XmlSerializationInfo);
            string radDockName = typeof(RadDock).Name;
            while (reader.Name != radDockName &&
                reader.ReadState != ReadState.EndOfFile &&
                reader.ReadState != ReadState.Error)
            {
                reader.Read();
            }

            TabStripPanel.DisableSelection = true;
            serializer.ReadObjectElement(reader, this);
            TabStripPanel.DisableSelection = false;
            this.EnsureInitialized();
            this.EndTransactionBlock();
            this.RestoreWindowsStatesAfterLoad();

            foreach (DockWindow window in DockWindows)
            {
                window.EnsureVisible();
            }

            if (FloatingWindows.Count > 1)
            {
                SortedList<int, FloatingWindow> sortedFloatingWindows = new SortedList<int, FloatingWindow>();
                foreach (FloatingWindow window in FloatingWindows)
                {
                    if (!sortedFloatingWindows.ContainsKey(window.ZIndex))
                    {
                        sortedFloatingWindows.Add(window.ZIndex, window);
                    }
                }

                for (int i = 1; i < sortedFloatingWindows.Count; i++)
                {
                    FloatingWindow top = sortedFloatingWindows.Values[i];
                    FloatingWindow previos = sortedFloatingWindows.Values[i - 1];
                    if (top.ZIndex != 0)
                    {
                        NativeMethods.SetWindowPos(new HandleRef(top, top.Handle), new HandleRef(previos, previos.Handle),
                            0, 0, 0, 0, NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
                    }
                }
            }

            foreach (ToolWindow window in this.GetWindows<ToolWindow>())
            {
                if (window.PreviousDockState == DockState.Floating && window.DockState == DockState.Hidden)
                {
                    window.DockState = DockState.Floating;
                    window.DockState = DockState.Hidden;
                }
            }
            loadLayout = false;
        }

        private static bool IsOldXmlFormat(XmlReader reader)
        {
            while (reader.Read())
            {
                string name = reader.Name;
                if (!string.IsNullOrEmpty(name) && name.ToLower() != "xml")
                {
                    break;
                }
            }

            return reader.Name == "DockingTree";
        }

        /// <summary>
        /// Creates the core XmlWritterSettings to be used by the serialization manager.
        /// </summary>
        /// <returns></returns>
        protected virtual XmlWriterSettings CreateXmlWriterSettings()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            settings.CloseOutput = false;
            settings.Indent = true;

            return settings;
        }

        /// <summary>
        /// Saves the docking layout configuration in an XML format in the specified file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        public void SaveToXml(string fileName)
        {
            using (XmlWriter writer = XmlWriter.Create(fileName, this.CreateXmlWriterSettings()))
            {
                this.SaveToXmlCore(writer);
            }
        }

        /// <summary>
        /// Saves the docking layout configuration in an XML format in the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void SaveToXml(Stream stream)
        {
            using (XmlWriter writer = XmlWriter.Create(stream, this.CreateXmlWriterSettings()))
            {
                this.SaveToXmlCore(writer);
            }
        }

        /// <summary>
        /// Saves the docking layout configuration in an XML format in the specified TextWriter.
        /// </summary>
        /// <param name="textWriter">The text writer.</param>
        public void SaveToXml(TextWriter textWriter)
        {
            using (XmlWriter writer = XmlWriter.Create(textWriter, this.CreateXmlWriterSettings()))
            {
                this.SaveToXmlCore(writer);
            }
        }

        /// <summary>
        /// Gets the default serialization info for RadDock used by Save/Load loyout methods to persist grid settings to/from XML.
        /// </summary>
        ///<remarks>
        /// You can use the serialization info to include/exclude properties of RadDock and related objects from XML serialization.
        ///</remarks>
        /// <returns></returns>
        public virtual ComponentXmlSerializationInfo GetDefaultXmlSerializationInfo()
        {
            PropertySerializationMetadataCollection metadata = new PropertySerializationMetadataCollection();
            metadata.Add(typeof(Control), "Tag", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));

            metadata.Add(typeof(RadDock), "Location", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            metadata.Add(typeof(RadDock), "Size", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            metadata.Add(typeof(RadDock), "Dock", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            metadata.Add(typeof(RadDock), "Anchor", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            metadata.Add(typeof(RadDock), "Name", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            metadata.Add(typeof(RadDock), "ThemeName", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));

            metadata.Add(typeof(RadControl), "RootElement", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            metadata.Add(typeof(Control), "DataBindings", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            metadata.Add(typeof(Control), "Cursor", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            metadata.Add(typeof(DockTabStrip), "ActiveWindow", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            metadata.Add(typeof(TabStripPanel), "ActiveControl", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            //metadata.Add(typeof(TabStripPanel), "TabPanels", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Content));
            metadata.Add(typeof(RadDock), "ActiveWindow", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            metadata.Add(typeof(RadDock), "GuidToNameMappings", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            metadata.Add(typeof(RadDock), "MainDocumentContainer", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            metadata.Add(typeof(DockWindow), "Controls", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            metadata.Add(typeof(FloatingWindow), "Controls", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            metadata.Add(typeof(Control), "Visible", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));

            metadata.Add(typeof(DocumentContainer), "IsMainDocumentContainer", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible));

            metadata.Add(typeof(DockWindowPlaceholder), "Name", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));
            metadata.Add(typeof(DockWindowPlaceholder), "Text", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Hidden));

            metadata.Add(typeof(DocumentContainer), "Name", new DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible));

            metadata.Add(new PropertySerializationMetadata(typeof(RadSplitContainer), "Orientation", true));
            metadata.Add(new PropertySerializationMetadata(typeof(SplitPanelSizeInfo), "SplitterCorrection", true));
            metadata.Add(new PropertySerializationMetadata(typeof(SplitPanelSizeInfo), "AutoSizeScale", true));
            //metadata.Add(new PropertySerializationMetadata(typeof(DockWindow), "DockState", true));

            return new ComponentXmlSerializationInfo(metadata);
        }

        /// <summary>
        /// Gets the serialization info for RadDock used by Save/Load loyout methods to persist grid settings to/from XML.
        /// By default, or when set to null the ComponentXmlSerializationInfo provided by GetDefaultXmlSerializationInfo() will be used.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ComponentXmlSerializationInfo XmlSerializationInfo
        {
            get
            {
                if (xmlSerializationInfo == null)
                {
                    xmlSerializationInfo = this.GetDefaultXmlSerializationInfo();
                }

                return this.xmlSerializationInfo;
            }
            set
            {
                this.xmlSerializationInfo = value;
            }
        }

        /// <summary>
        /// Performs the core save logic.
        /// </summary>
        /// <param name="writer"></param>
        protected virtual void SaveToXmlCore(XmlWriter writer)
        {
            UpdateFloatingWindowsZOrderInfo();

            DockXmlSerializer serializer = new DockXmlSerializer(this, this.XmlSerializationInfo, true);

            writer.WriteStartElement(typeof(RadDock).Name);
            serializer.WriteObjectElement(writer, this);

            this.OnSavedToXml();
        }

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public GuidToNameMappingCollection GuidToNameMappings
        {
            get
            {
                if (this.guidToNameMappings == null)
                {
                    this.guidToNameMappings = new GuidToNameMappingCollection();
                }
                return this.guidToNameMappings;
            }
        }

        /// <summary>
        /// Gets an instance of DockAutoHideSerializationContainer that contains information about auto-hidden windows which have been 
        /// set up through designer or xml file. This nethod is no intended for use directly from your code.
        /// </summary>
#if !DEBUG
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Inheritance(InheritanceLevel.NotInherited)]
        public DockAutoHideSerializationContainer SerializableAutoHideContainer
        {
            get
            {
                if (this.serializableAutoHideContainer == null)
                {
                    this.serializableAutoHideContainer = new DockAutoHideSerializationContainer(this);
                }

                return this.serializableAutoHideContainer;
            }
        }

        /// <summary>
        /// Gets an instance of FloatingWindowList that contains information about floating windows which have been 
        /// set up through designer or xml file. This nethod is no intended for use directly from your code.
        /// </summary>
#if !DEBUG
        [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Inheritance(InheritanceLevel.NotInherited)]
        public FloatingWindowList SerializableFloatingWindows
        {
            get
            {
                if (serializableFloatingWindows == null)
                {
                    serializableFloatingWindows = new FloatingWindowList();
                }

                return serializableFloatingWindows;
            }
        }

        internal void SynchSerializableFloatingWindows()
        {
            this.DisposeSerializableFloatingWindows();

            this.serializableFloatingWindows = new FloatingWindowList();

            if (this.FloatingWindows.Count == 0)
            {
                return;
            }

            ComponentXmlSerializationInfo serializationInfo = this.GetDefaultXmlSerializationInfo();
            FloatingWindowsXmlResolvingSerializer ser = new FloatingWindowsXmlResolvingSerializer(this, serializationInfo);

            StringBuilder sb = new StringBuilder();
            using (XmlTextWriter writer = new XmlTextWriter(new StringWriter(sb)))
            {
                ser.WriteCollectionElement(writer, this.FloatingWindows, "FloatingWindows");
            }

            FloatingWindowsXmlNonResolvingSerializer deSer = new FloatingWindowsXmlNonResolvingSerializer(serializationInfo);

            using (XmlTextReader reader = new XmlTextReader(new StringReader(sb.ToString())))
            {
                deSer.ReadCollectionElement(reader, this.serializableFloatingWindows);
            }
        }

        internal void DisposeSerializableFloatingWindows()
        {
            if (this.serializableFloatingWindows != null)
            {
                foreach (SerializableFloatingWindow window in this.serializableFloatingWindows)
                {
                    window.DockContainer.Dispose();
                }
            }

            this.serializableFloatingWindows = null;
        }

        /// <summary>
        /// The method is intended fot use within RadDcok and Advanced Layout Designer, to float windows after loading radDock layout from designer
        /// </summary>
        private void LoadDeserializedFloatingAndAutoHideWindows()
        {
            ComponentXmlSerializationInfo serializationInfo = this.GetDefaultXmlSerializationInfo();

            this.BeginTransactionBlock();

            //Since we do not use transactions in this case, we should take care of redock state manually
            RedockService service = this.GetService<RedockService>(ServiceConstants.Redock);
            if (service != null)
            {
                foreach (DockWindow window in DockHelper.GetDockWindows(this, true, this))
                {
                    service.ClearState(window, window.DockState);
                    service.SaveState(window);
                }
            }

            //deserialize auto-hidden windows
            this.SerializableAutoHideContainer.LoadDeserializedWindows();

            foreach (SerializableFloatingWindow serWindow in this.SerializableFloatingWindows)
            {
                FloatingWindowsXmlNonResolvingSerializer ser = new FloatingWindowsXmlNonResolvingSerializer(serializationInfo);

                StringBuilder sb = new StringBuilder();
                using (XmlTextWriter writer = new XmlTextWriter(new StringWriter(sb)))
                {
                    writer.WriteStartElement("FloatingWindow");
                    ser.WriteObjectElement(writer, serWindow);
                    writer.WriteEndElement();
                }

                FloatingWindow newFw = this.CreateFloatingWindow();
                newFw.ZIndex = serWindow.ZIndex;
                FloatingWindowsXmlResolvingSerializer deserializer = new FloatingWindowsXmlResolvingSerializer(this, serializationInfo);

                using (XmlTextReader textReader = new XmlTextReader(new StringReader(sb.ToString())))
                {
                    textReader.Read();
                    deserializer.ReadObjectElement(textReader, newFw);
                }

                newFw.RemovePlaceHolders();
                newFw.UpdateVisibility();
            }

            //Clear floating windows cache, as we no longer need it
            this.DisposeSerializableFloatingWindows();
            if (this.serializableAutoHideContainer != null)
            {
                this.serializableAutoHideContainer.ClearAutoHideGroups();
            }

            this.EndTransactionBlock();
            this.EnsureInitialized();
        }

        private void UpdateFloatingWindowsZOrderInfo()
        {
            Dictionary<IntPtr, FloatingWindow> toolWindows = new Dictionary<IntPtr, FloatingWindow>();
            foreach (FloatingWindow window in this.FloatingWindows)
            {
                toolWindows.Add(window.Handle, window);
            }

            Process p = Process.GetCurrentProcess();
            IntPtr handle = p.MainWindowHandle;
            int z = 0;
            do
            {
                if (toolWindows.ContainsKey(handle))
                {
                    toolWindows[handle].ZIndex = ++z;
                }
                handle = Telerik.WinControls.NativeMethods.GetWindow(handle, NativeMethods.GetWindow_Cmd.GW_HWNDPREV);
            } while (handle != IntPtr.Zero);
        }

        #endregion

        #region Theming

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string ThemeClassName
        {
            get
            {
                return typeof(RadDock).FullName;
            }
            set
            {
                base.ThemeClassName = value;
            }
        }

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            if (element is RadTabStripElement)
            {
                return true;
            }

            return base.ControlDefinesThemeForElement(element);
        }

        #endregion

        #region ILoadObsoleteDockingManagerXml Members

        void Telerik.WinControls.Interfaces.ILoadObsoleteDockingManagerXml.LoadDockingManagerXml(TextReader input)
        {
            using (XmlReader reader = XmlReader.Create(input))
            {
                this.LoadFromOldXml(reader);
            }
        }

        private EventHandler<Telerik.WinControls.Interfaces.DockableDeserializedEventArgs> dockableDeserializedHandler = null;

        event EventHandler<Telerik.WinControls.Interfaces.DockableDeserializedEventArgs> Telerik.WinControls.Interfaces.ILoadObsoleteDockingManagerXml.DockableDeserialized
        {
            add
            {
                this.dockableDeserializedHandler += value;
            }
            remove
            {
                this.dockableDeserializedHandler -= value;
            }
        }

        internal void OnDockableDeserialized(Telerik.WinControls.Interfaces.DockableDeserializedEventArgs e)
        {
            EventHandler<Telerik.WinControls.Interfaces.DockableDeserializedEventArgs> handler = this.dockableDeserializedHandler;
            if (handler != null)
            {
                handler(this, e);
            }
        }
        #endregion

        #region ActiveWindow Management

        /// <summary>
        /// Gets or sets the DockWindow instance, which is currently active (meaning it contains the Keyboard focus).
        /// </summary>
        [Browsable(false)]
        [DefaultValue(null)]
        public DockWindow ActiveWindow
        {
            get
            {
                if (this.activeWindow != null)
                {
                    return this.activeWindow;
                }

                this.SetDefaultActiveWindow();
                return this.activeWindow;
            }
            set
            {
                if (this.activeWindow == value)
                {
                    return;
                }

                if (!this.ShouldProcessNotification() || !this.IsLoaded)
                {
                    this.activeWindowChanged = this.activeWindow != value;
                    if (this.activeWindow != null)
                    {
                        this.activeWindow.UpdateActiveState(false);
                    }
                    this.activeWindow = value;
                }
                else
                {
                    this.ActivateWindow(value);
                }
            }
        }

        /// <summary>
        /// Sets the first available DockWindow as currently active.
        /// </summary>
        private void SetDefaultActiveWindow()
        {
            //first search for a dock window that resides on the RadDock
            this.SetDefaultActiveWindow(this);
            if (this.activeWindow != null)
            {
                return;
            }

            //search for a floating dock window
            foreach (FloatingWindow window in this.floatingWindows)
            {
                this.SetDefaultActiveWindow(window);
            }
        }

        private void SetDefaultActiveWindow(Control parent)
        {
            foreach (Control child in ControlHelper.EnumChildControls(parent, true))
            {
                DockTabStrip strip = child as DockTabStrip;
                if (strip == null || strip.TabPanels.Count == 0 || strip.DockManager != this)
                {
                    continue;
                }

                this.activeWindow = strip.ActiveWindow;
                this.activeWindowChanged = true;
                Debug.Assert(this.activeWindow != null, "Must have an active window at this point.");

                if (this.activeWindow != null)
                {
                    this.activeWindow.UpdateActiveState(true);
                }
                break;
            }
        }

        /// <summary>
        /// Activates the specified DockWindow and gives it the keyboard focus.
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public bool ActivateWindow(DockWindow window)
        {
            return this.ActivateWindow(window, false);
        }

        /// <summary>
        /// Activates the specified window and gives it the keyboard focus.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="forceNotify">
        /// True to force ActiveWindowChanging and ActiveWindowChanged notifications, false otherwise.
        /// Sometimes a transaction may change the current active window several times and the method needs to know when to raise the notifications.
        /// </param>
        /// <returns></returns>
        protected virtual bool ActivateWindow(DockWindow window, bool forceNotify)
        {
            if (!this.IsHandleCreated)
            {
                return false;
            }

            if (this.settingActiveWindow)
            {
                return false;
            }

            if (window == null || window.DockState == DockState.Hidden || !this.attachedWindows.ContainsValue(window))
            {
                this.activeWindow = null;
                return false;
            }

            this.settingActiveWindow = true;
            this.UpdateActiveWindow(window, forceNotify);
            this.settingActiveWindow = false;

            return window.ContainsFocus;
        }

        private void UpdateActiveWindow(DockWindow window, bool forceNotify)
        {
            DockWindow tabWindow = null;
            DockWindow currActive = this.activeWindow;
            bool windowChanged = window != currActive;

            if (forceNotify || window != this.activeWindow)
            {
                DockWindowCancelEventArgs args = new DockWindowCancelEventArgs(this.activeWindow, window);
                OnActiveWindowChanging(args);
                if (args.Cancel)
                {
                    return;
                }

                if (window != null && window.TabStrip != null && window.TabStrip.SelectedTab is DockWindow && window.TabStrip.SelectedTab != window)
                {
                    SelectedTabChangingEventArgs e = new SelectedTabChangingEventArgs((DockWindow)window.TabStrip.SelectedTab, window);
                    OnSelectedTabChanging(e);
                    if (e.Cancel)
                    {
                        return;
                    }

                    tabWindow = (DockWindow)window.TabStrip.SelectedTab;
                }
            }

            if (window.AutoHideTab != null && !this.activateFromAutoHide)
            {
                this.ShowAutoHidePopup(window.AutoHideTab, AutoHideDisplayReason.Activate);
                //auto-hide display is canceled
                if (!this.autoHidePopup.Visible)
                {
                    return;
                }
            }

            if (currActive != null && currActive.DockState != DockState.TabbedDocument)
            {
                currActive.UpdateActiveState(false);
            }

            this.activeWindow = window;
            this.activeWindow.EnsureVisible();
            this.activeWindow.UpdateActiveState(true);

            if (window.TabStrip != null && window.DockState != DockState.AutoHide)
            {
                window.TabStrip.SelectTab(window);
            }

            this.activateFromAutoHide = false;
            this.UpdateActiveControl(window);

            if (forceNotify || windowChanged)
            {
                OnActiveWindowChanged(new DockWindowEventArgs(window));
            }

            if (tabWindow != null)
            {
                SelectedTabChangedEventArgs e = new SelectedTabChangedEventArgs(tabWindow, window);
                OnSelectedTabChanged(e);
            }
        }


        private void UpdateActiveControl(DockWindow window)
        {
            ContainerControl container = window;
            while (true)
            {
                ContainerControl activeContainer = container.ActiveControl as ContainerControl;
                if (activeContainer == null)
                {
                    break;
                }

                container = activeContainer;
            }

            Control focused = ControlHelper.GetFocusedControl();
            if (focused == null || !ControlHelper.IsDescendant(container, focused))
            {
                if (!container.SelectNextControl(container, true, true, true, false))
                {
                    ContainerControl parentContainer = ControlHelper.FindAncestor<ContainerControl>(window);
                    if (parentContainer != null)
                    {
                        parentContainer.ActiveControl = window;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a list of the floating windows 
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FloatingWindowCollection FloatingWindows
        {
            get
            {
                return this.floatingWindows;
            }
        }

        #endregion
    }
}
