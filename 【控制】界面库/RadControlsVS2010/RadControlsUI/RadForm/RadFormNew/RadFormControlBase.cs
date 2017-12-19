using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using System.Diagnostics;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls;
using System.Runtime.InteropServices;
using Telerik.WinControls.Keyboard;
using System.Reflection;
using System.Collections.Specialized;
using System.ComponentModel.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// A Delegate which is used for invoking the base implementation of WndProc of this form.
    /// </summary>
    /// <param name="m"></param>
    public delegate void WndProcInvoker(ref Message m);

    /// <summary>
    /// Represents a RadFormControl. RadFormControlBase is an abstract class and is base class for
    /// all telerik windows forms.
    /// </summary>
    public abstract class RadFormControlBase : Form, IComponentTreeHandler, ISupportInitializeNotification, INotifyPropertyChanged
    {
        #region Fields

        private static readonly object ToolTipTextNeededEventKey = null;
        private static readonly object ScreenTipNeededEventKey = new object();

        private FormControlBehavior formBehavior;
        protected bool isBehaviorPrepared = false;
        private string oldThemeName = null;
        private FormBorderStyle formBorderStyle;
        private bool loaded;
        private bool disposing;
        public bool controlIsInitializingRootComponent = false;
        protected bool isPainting = false;
        private bool isInitializing = false;
        private RadControlDesignTimeData designTimeData;
        private ComponentThemableElementTree elementTree;
        private ComponentInputBehavior inputBehavior;
        private ImageList imageList;
        private ImageList smallImageList;
        private int suspendUpdateCounter = 0;
        private Size smallImageScalingSize = Size.Empty;

        #region Reflected

        private FieldInfo clientWidthField;
        private FieldInfo clientHeightField;
        private FieldInfo formStateSetClientSizeField;
        private FieldInfo formStateField;

        #endregion

        #endregion

        #region Constructor

        static RadFormControlBase()
        {
            ToolTipTextNeededEventKey = new object();
            PropertyChangedEventKey = new object();
        }

        public RadFormControlBase()
        {
            this.Construct();
            
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer, true);

            this.formBorderStyle = base.FormBorderStyle;
            base.FormBorderStyle = FormBorderStyle.None;
            this.LoadElementTree(this.Size);
        }

        protected virtual void Construct()
        {
            this.contextLayoutManager = new ContextLayoutManager(this);
            this.inputBehavior = new ComponentInputBehavior(this);
            this.elementTree = new ComponentOverrideElementTree(this);
            this.elementTree.EnsureRootElement();
            this.elementTree.RootElement.UseNewLayoutSystem = this.GetUseNewLayout();
            this.elementTree.CreateChildItems(this.elementTree.RootElement);
            this.InitializeReflectedFields();
        }

        private void InitializeReflectedFields()
        {
            this.clientWidthField = typeof(Control).GetField("clientWidth", BindingFlags.NonPublic | BindingFlags.Instance);
            this.clientHeightField = typeof(Control).GetField("clientHeight", BindingFlags.NonPublic | BindingFlags.Instance);
            this.formStateSetClientSizeField = typeof(Form).GetField("FormStateSetClientSize", BindingFlags.NonPublic | BindingFlags.Static);
            this.formStateField = typeof(Form).GetField("formState", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public override ISite Site
        {
            get
            {
                return base.Site;
            }
            set
            {
                base.Site = value;

                ISite site = this.Site;
                if (site != null && site.DesignMode)
                {
                    this.RootElement.SetIsDesignMode(true, true);
                }
                else
                {
                    this.RootElement.SetIsDesignMode(false, true);
                }
            }
        }

        /// <summary>
        /// Determines whether the control and all its child elements should use the new layout system.
        /// </summary>
        /// <returns></returns>
        protected virtual bool GetUseNewLayout()
        {
            return true;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            this.LoadElementTree(this.Size);
        }

        /// <summary>
        /// Loads the element tree. While not loaded, no layout operations are allowed upon the tree.
        /// By default, the tree will be loaded when the control is displayed for the first time.
        /// </summary>
        public virtual void LoadElementTree()
        {
            this.LoadElementTree(this.Size);
        }

        /// <summary>
        /// Loads the element tree using the specified desired size.
        /// </summary>
        /// <param name="desiredSize"></param>
        public virtual void LoadElementTree(Size desiredSize)
        {
            if (this.loaded)
            {
                return;
            }

            this.OnLoad(desiredSize);
        }

        /// <summary>
        /// Notifies that the control is about to be visualized.
        /// </summary>
        /// <param name="desiredSize"></param>
        protected virtual void OnLoad(Size desiredSize)
        {
            if (this.isInitializing)
            {
                return;
            }

            this.elementTree.EnsureThemeAppliedInitially(false);
            //notify all elements, listening for the ImageList of the control
            this.elementTree.RootElement.NotifyControlImageListChanged();
            //notify the root element for the loading event
            this.elementTree.RootElement.OnLoad(true);

            //we are already loaded
            this.loaded = true;

            //set the current control bounds as initial to the element tree
            desiredSize = this.elementTree.PerformInnerLayout(true, Left, Top, desiredSize.Width, desiredSize.Height);
            if (this.AutoSize)
            {
                base.SetBoundsCore(Left, Top, desiredSize.Width, desiredSize.Height, BoundsSpecified.All);
            }

            
        }

        /// <summary>
        /// In this override we reset the RootElement's BackColor property
        /// since the DocumentDesigner class sets the BackColor of the
        /// Form to Control when initializing and thus overrides the theme.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (this.IsDesignMode)
            {
                this.BackColor = Color.Empty;

                this.initialFormLocation = this.Location;
            }
        }

        /// <summary>
        /// Calls the base OnPaint implementation. This method
        /// can be used by the form behavior to call the base
        /// implementation in case it is needed.
        /// </summary>
        internal void CallBaseOnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        /// <summary>
        /// Calls the base OnPaintBackground implementation. This method
        /// can be used by the form behavior to call the base
        /// implementation in case it is needed.
        /// </summary>
        internal void CallBaseOnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.formBehavior == null || !this.isBehaviorPrepared)
            {
                base.OnPaint(e);
                return;
            }

            if (this.formBehavior.OnAssociatedFormPaint(e))
            {
                return;
            }

            base.OnPaint(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.formBehavior == null || !this.isBehaviorPrepared)
            {
                base.OnPaintBackground(e);
                return;
            }

            if (this.formBehavior.OnAssociatedFormPaintBackground(e))
            {
                return;
            }

            base.OnPaintBackground(e);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a boolean value which determines
        /// whether the control is loaded.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return this.loaded;
            }
        }

        public override Size MaximumSize
        {
            get
            {
                return base.MaximumSize;
            }
            set
            {
                base.MaximumSize = this.RootElement.MaxSize = value;
            }
        }

        public override Size MinimumSize
        {
            get
            {
                return base.MinimumSize;
            }
            set
            {
                base.MinimumSize = value;

              //  Size minimumWindowSize = SystemInformation.MinimumWindowSize;
              //  int actualWidth = value.Width;
              //  int actualHeight = value.Height;

              //  if (actualWidth < minimumWindowSize.Width
              //      && actualWidth > 0)
              //  {
              //      actualWidth = minimumWindowSize.Width;
              //  }

              //  if (actualHeight < minimumWindowSize.Height && actualHeight > 0)
              //  {
              //      actualHeight = minimumWindowSize.Height;
              //  }

              //this.RootElement.MinSize = new Size(actualWidth, actualHeight);
            }
        }

        public new bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        /// <summary>
        /// Gets or sets the FormBorderStyle of the Form.
        /// </summary>
        public new FormBorderStyle FormBorderStyle
        {
            get
            {
                return (this.formBehavior != null && this.isBehaviorPrepared)
                    ? this.formBorderStyle : base.FormBorderStyle;
            }
            set
            {
                if (this.formBehavior != null && this.isBehaviorPrepared)
                {
                    //Padding clientMargin = this.formBehavior.ClientMargin;
                    this.formBorderStyle = value;
                    this.UpdateStyles();

                    //if (value == FormBorderStyle.None)
                    {
                        this.SetClientSizeCore(this.ClientSize.Width, this.ClientSize.Height);
                    }
                }
                else
                {
                    base.FormBorderStyle = value;
                }
            }
        }

        /// <summary>
        /// Gets the behavior associated with this form if any.
        /// </summary>
        public FormControlBehavior FormBehavior
        {
            get
            {
                return this.formBehavior;
            }
            set
            {
                this.ResetFormBehavior(false);

                if (value != null)
                {
                    this.formBehavior = value;
                    this.PrepareBehavior();
                    this.RecreateHandle();
                }
            }
        }


        [Browsable(false)]
        [Description("Gets the ComponentInputBehavior instance that handles all logic and user interaction in RadControl.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual ComponentInputBehavior Behavior
        {
            get
            {
                return this.inputBehavior;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Category(RadDesignCategory.StyleSheetCategory)]
        public virtual string ThemeClassName
        {
            get
            {
                return this.elementTree.ThemeClassName;
            }
            set
            {
                if (this.elementTree.ThemeClassName != value)
                {
                    this.elementTree.ThemeClassName = value;
                    this.OnNotifyPropertyChanged("ThemeClassName");
                }
            }
        }
#if DEBUG
        [Browsable(true)]
#else
                [Browsable(false)]
#endif
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool UseNewLayoutSystem
        {
            get
            {
                if (this.elementTree != null)
                {
                    return this.elementTree.RootElement.UseNewLayoutSystem;
                }
                return false;
            }
            set
            {
                if (this.elementTree == null)
                {
                    return;
                }

                if (this.elementTree.RootElement.UseNewLayoutSystem != value)
                {
                    this.elementTree.RootElement.UseNewLayoutSystem = value;
                    this.OnNotifyPropertyChanged("UseNewLayoutSystem");
                }
            }
        }


        [Browsable(true),
         DefaultValue(null),
         Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the ImageList that contains the images displayed by this control.")]
        public virtual ImageList SmallImageList
        {
            get
            {
                return this.smallImageList;
            }
            set
            {
                if (this.smallImageList != value)
                {
                    EventHandler recreateHandle = new EventHandler(this.ImageListRecreateHandle);
                    EventHandler detachHandle = new EventHandler(this.DetachSmallImageList);
                    if (this.smallImageList != null)
                    {
                        this.smallImageList.RecreateHandle -= recreateHandle;
                        this.smallImageList.Disposed -= detachHandle;
                    }
                    this.smallImageList = value;
                    if (this.smallImageList != null)
                    {
                        this.smallImageList.RecreateHandle += recreateHandle;
                        this.smallImageList.Disposed += detachHandle;
                    }
                    this.ElementTree.RootElement.NotifyControlImageListChanged();
                    this.InvalidateIfNotSuspended();
                }
            }
        }


        #region Compatibility properties
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadElement FocusedElement
        {
            get
            {
                return this.Behavior.FocusedElement;
            }
            set
            {
                this.Behavior.FocusedElement = value;
            }
        }

        /// <summary>
        /// Indicates focus cues display, when available, based on the corresponding control type and the current UI state.
        /// </summary>
        [DefaultValue(false), Category("Accessibility")]
        public virtual bool AllowShowFocusCues
        {
            get
            {
                return this.Behavior.AllowShowFocusCues;
            }
            set
            {
                this.Behavior.AllowShowFocusCues = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether ToolTips are shown for the RadItem objects contained in 
        /// the RadControl.
        /// </summary>
        [Description("Gets or sets a value indicating whether ToolTips are shown for the RadItem objects contained in the RadControl."),
        DefaultValue(true), Category("Behavior")]
        public virtual bool ShowItemToolTips
        {
            get
            {
                return this.Behavior.ShowItemToolTips;
            }
            set
            {
                this.Behavior.ShowItemToolTips = value;
            }
        }

        [Category(RadDesignCategory.BehaviorCategory),
        TypeConverter(typeof(ExpandableObjectConverter)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public InputBindingsCollection CommandBindings
        {
            get
            {
                return this.Behavior.Shortcuts.InputBindings;
            }
        }


        /// <summary>
        /// Gets or sets whether Key Map (Office 2007 like accelerator keys map)
        /// is used for this speciffic control. Currently this option is implemented for 
        /// the RadRibbonBar control only.
        /// </summary>
        [Browsable(true), DefaultValue(false)]
        public bool EnableKeyMap
        {
            get
            {
                return this.Behavior.EnableKeyTips;
            }
            set
            {
                this.Behavior.EnableKeyTips = value;
            }
        }


        /// <summary>
        /// Gets a value indicating whether the ThemeClassName property was set to value different from null (Nothing in VB.NET).
        /// </summary>
        public bool IsThemeClassNameSet
        {
            get
            {
                return this.ElementTree.IsThemeClassNameSet;
            }
        }


        #endregion

        #endregion

        #region Ambient Properties

        #region BackColor

        /// <summary>
        /// Gets or sets the BackColor of the control.
        /// This is actually the BackColor property of the root element.
        /// </summary>
        [Description("Gets or sets the BackColor of the control. This is actually the BackColor property of the root element.")]
        public override Color BackColor
        {
            get
            {
                return this.elementTree.RootElement.BackColor;
            }
            set
            {
                if (value == Color.Empty)
                {
                    this.elementTree.RootElement.ResetValue(VisualElement.BackColorProperty, ValueResetFlags.Local);
                }
                else
                {
                    this.elementTree.RootElement.BackColor = value;
                }
            }
        }

        /// <summary>
        /// Determines whether the BackColor property should be serialized.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeBackColor()
        {
            return this.ShouldSerializeProperty(VisualElement.BackColorProperty);
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            base.OnBackColorChanged(e);

            if (!this.RootElement.GetBitState(RootRadElement.RootElementInitiatedPropertyChangeStateKey))
            {
                this.RootElement.ResetValue(VisualElement.BackColorProperty, ValueResetFlags.Inherited);
            }
        }

        #endregion

        #region ForeColor

        /// <summary>
        /// Gets or sets the ForeColor of the control.
        /// This is actually the ForeColor property of the root element.
        /// </summary>
        [Description("Gets or sets the ForeColor of the control. This is actually the ForeColor property of the root element.")]
        public override Color ForeColor
        {
            get
            {
                return this.elementTree.RootElement.ForeColor;
            }
            set
            {
                if (value == Color.Empty)
                {
                    this.elementTree.RootElement.ResetValue(VisualElement.ForeColorProperty, ValueResetFlags.Local);
                }
                else
                {
                    this.elementTree.RootElement.ForeColor = value;
                }
            }
        }

        /// <summary>
        /// Determines whether the ForeColor property should be serialized.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeForeColor()
        {
            return this.ShouldSerializeProperty(VisualElement.ForeColorProperty);
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);

            if (!this.RootElement.GetBitState(RootRadElement.RootElementInitiatedPropertyChangeStateKey))
            {
                this.RootElement.ResetValue(VisualElement.ForeColorProperty, ValueResetFlags.Inherited);
            }
        }

        #endregion

        #region Font

        /// <summary>
        /// Gets or sets the Font of the control. This is actually the Font property of the root element.
        /// </summary>
        [Description("Gets or sets the Font of the control. This is actually the Font property of the root element.")]
        public override Font Font
        {
            get
            {
                return this.elementTree.RootElement.Font;
            }
            set
            {
                if (value == null)
                {
                    this.elementTree.RootElement.ResetValue(VisualElement.FontProperty, ValueResetFlags.Local);
                }
                else
                {
                    this.elementTree.RootElement.Font = value;
                }
            }
        }

        /// <summary>
        /// Determines whether the ForeColor property should be serialized.
        /// </summary>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeFont()
        {
            return this.ShouldSerializeProperty(VisualElement.FontProperty);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);

            if (!this.RootElement.GetBitState(RootRadElement.RootElementInitiatedPropertyChangeStateKey))
            {
                this.RootElement.ResetValue(VisualElement.FontProperty, ValueResetFlags.Inherited);
            }
        }

        #endregion

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            //invalidate the inherited property modifier for all ambient properties that may be inherited from the parent
            this.elementTree.ResetAmbientProperties();
        }

        /// <summary>
        /// Determines whether the specified RadProperty should be serialized.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        protected virtual bool ShouldSerializeProperty(RadProperty property)
        {
            return (int)this.elementTree.RootElement.GetValueSource(property) > (int)ValueSource.Style;
        }

        #endregion

        #region Methods

        #region Form behavior specific

        internal void SetFormStyle(ControlStyles style, bool value)
        {
            this.SetStyle(style, value);
        }

        internal void CallUpdateStyles()
        {
            this.UpdateStyles();
        }

        internal void CallRecreateHandle()
        {
            this.RecreateHandle();
        }
        internal void CallInitializeFormBehavior()
        {
            if (this.formBehavior == null)
            {
                this.formBehavior = this.InitializeFormBehavior();
                if (this.formBehavior != null)
                {
                    this.PrepareBehavior();
                }
            }
        }

        private void PrepareBehavior()
        {
            this.formBehavior.SetBaseWndProcCallback(CallBaseWndProc);
            this.formBehavior.SetDefWndProcCallback(CallDefWndProc);
            if (this.formBehavior.HandlesCreateChildItems)
            {
                this.formBehavior.CreateChildItems(this.RootElement);
            }

            this.isBehaviorPrepared = true;
            this.RootElement.SetValue(RootRadElement.ApplyShapeToControlProperty, true);

            this.UpdateStyles();
        }

        /// <summary>
        /// Called to initialize the behavior of the form.
        /// </summary>
        /// <returns></returns>
        protected abstract FormControlBehavior InitializeFormBehavior();

        /// <summary>
        /// Resets the behavior associated with the Form.
        /// </summary>
        /// <param name="callInitialize">Determines whether the InitializeFormBehavior method
        /// will be called after the p</param>
        internal void ResetFormBehavior(bool callInitialize)
        {
            this.isBehaviorPrepared = false;
            
            if (this.formBehavior != null
                && this.formBehavior.HandlesCreateChildItems)
            {
                this.formBehavior.Dispose();
                this.formBehavior = null;

                this.RootElement.DisposeChildren();
                this.RootElement.ResetValue(RootRadElement.ApplyShapeToControlProperty, ValueResetFlags.Local);
            }

            if (!callInitialize)
            {
                base.FormBorderStyle = FormBorderStyle.Sizable;
                this.UpdateStyles();
                this.Region = null;
            }
            else
            {
                this.CallInitializeFormBehavior();
            }

            this.RecreateHandle();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;

                if (this.formBehavior != null)
                {
                    return this.formBehavior.CreateParams(createParams);
                }

                return createParams;
            }
        }

        #endregion

        #region Overriden

        protected override void Dispose(bool disposing)
        {
#if (EVALUATION || OEM)
			        RadControl.licenseCount = -1;
#endif

            if (disposing)
            {
                this.disposing = true;

                this.SuspendLayout();

                // The element tree will remove all elements from the layout queues
                if (this.elementTree != null)
                {
                    this.elementTree.Dispose(true);
                }

                if (this.contextLayoutManager != null)
                {
                    this.contextLayoutManager.Dispose();
                    this.contextLayoutManager = null;
                }

                if (this.inputBehavior != null)
                {
                    this.inputBehavior.Dispose();
                }

                if (this.formBehavior != null)
                {
                    this.formBehavior.Dispose();
                }

                if (this.Region != null)
                {
                    this.Region.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        private bool wmWindowPosChangedProcessing = false;

        protected override void WndProc(ref Message m)
        {
            //TODO: Examine more carefully
            if (this.IsDisposed || this.disposing)
            {
                base.WndProc(ref m);
                return;
            }

            if (m.Msg == NativeMethods.WM_WINDOWPOSCHANGING)
            {
                if (this.IsDesignMode)
                {
                    this.PerformDesignModeLocationCorrection(ref m);
                }
            }

            if (this.formBehavior == null || !this.isBehaviorPrepared)
            {
                base.WndProc(ref m);
            }
            else if (!this.formBehavior.HandleWndProc(ref m))
            {
                if (m.Msg == NativeMethods.WM_WINDOWPOSCHANGED)
                {
                    this.wmWindowPosChangedProcessing = true;
                }

                base.WndProc(ref m);

                if (m.Msg == NativeMethods.WM_WINDOWPOSCHANGED)
                {
                    this.wmWindowPosChangedProcessing = false;
                }

            }
        }
        private Point? initialFormLocation = null;
        protected virtual void PerformDesignModeLocationCorrection(ref Message msg)
        {
            NativeMethods.WINDOWPOS windowPosStruct = (NativeMethods.WINDOWPOS)
              Marshal.PtrToStructure(msg.LParam, typeof(NativeMethods.WINDOWPOS));

                ScrollableControl parent = this.Parent as ScrollableControl;

                if (parent != null)
                {
                    if (!parent.HorizontalScroll.Visible && this.initialFormLocation.HasValue)
                    {
                        windowPosStruct.x = this.initialFormLocation.Value.X;
                    }

                    if (!parent.VerticalScroll.Visible && this.initialFormLocation.HasValue)
                    {
                        windowPosStruct.y = this.initialFormLocation.Value.Y;
                    }
                }

            Marshal.StructureToPtr(windowPosStruct, msg.LParam, true);
            msg.Result = IntPtr.Zero;
        }


        protected virtual void CallBaseWndProc(ref Message m)
        {
            base.WndProc(ref m);
        }

        private void CallDefWndProc(ref Message m)
        {
            this.DefWndProc(ref m);
        }

        #endregion

        #endregion

        #region IComponentTreeHandler Members

        object IComponentTreeHandler.GetAmbientPropertyValue(RadProperty property)
        {
            if (property == VisualElement.BackColorProperty)
            {
                return base.BackColor;
            }

            if (property == VisualElement.ForeColorProperty)
            {
                return base.ForeColor;
            }

            if (property == VisualElement.FontProperty)
            {
                return base.Font;
            }

            return RadProperty.UnsetValue;
        }

        void IComponentTreeHandler.OnAmbientPropertyChanged(RadProperty property)
        {
            if (property == VisualElement.BackColorProperty)
            {
                base.OnBackColorChanged(EventArgs.Empty);
            }
            else if (property == VisualElement.ForeColorProperty)
            {
                base.OnForeColorChanged(EventArgs.Empty);
            }
            else if (property == VisualElement.FontProperty)
            {
                base.OnFontChanged(EventArgs.Empty);
            }
        }

        bool IComponentTreeHandler.OnFocusRequested(RadElement element)
        {
            return this.ProcessFocusRequested(element);
        }

        bool IComponentTreeHandler.OnCaptureChangeRequested(RadElement element, bool capture)
        {
            return this.ProcessCaptureChangeRequested(element, capture);
        }

        /// <summary>
        /// Processes a focus request from the specified element.
        /// </summary>
        /// <param name="element">The element that requested the focus.</param>
        /// <returns>True if focus is approved, false otherwise.</returns>
        protected virtual bool ProcessFocusRequested(RadElement element)
        {
            if (this.Focused)
            {
                return false;
            }

            return this.Focus();
        }

        /// <summary>
        /// Processes a capture request from the specified element.
        /// </summary>
        /// <param name="element">The element which requested the capture.</param>
        /// <param name="capture"></param>
        /// <returns>True if the capture request is approved, otherwise false.</returns>
        protected virtual bool ProcessCaptureChangeRequested(RadElement element, bool capture)
        {
            return this.Capture = capture;
        }

        protected virtual RadControlDesignTimeData CreateDesignTimeData()
        {
            return null;
        }

        protected virtual void OnNotifyPropertyChanged(string propertyName)
        {
            this.OnNotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler1 =
                (PropertyChangedEventHandler)this.Events[PropertyChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        protected virtual void CreateChildItems(RadElement parent)
        {
            parent.Name = this.Name;
            this.CallInitializeFormBehavior();
        }

        protected virtual RootRadElement CreateRootElement()
        {
            return new FormRootElement(this);
        }

        internal protected virtual void OnThemeNameChanged(ThemeNameChangedEventArgs e)
        {
            if (this.ThemeNameChanged != null)
            {
                this.ThemeNameChanged(this, e);
            }
        }

        protected virtual void OnThemeChanged()
        {
            if (!this.loaded
                || this.RootElement.ElementState != ElementState.Loaded
                || !this.isBehaviorPrepared)
            {
                return;
            }

            this.RootElement.InvalidateMeasure(true);
            this.RootElement.UpdateLayout();

            FormControlBehavior formBehavior = this.FormBehavior;
            if (this.oldThemeName != this.ThemeName && formBehavior != null)
            {
                if (this.WindowState != FormWindowState.Maximized)
                {
                    this.SetClientSizeCore(this.ClientSize.Width, this.ClientSize.Height);
                }

                this.oldThemeName = this.ThemeName;
                this.PerformLayout(this, "ClientSize");
            }
        }

        protected internal virtual void OnDisplayPropertyChanged(RadPropertyChangedEventArgs e)
        {
        }

        protected virtual void OnToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            ToolTipTextNeededEventHandler handler =
                base.Events[ToolTipTextNeededEventKey] as ToolTipTextNeededEventHandler;

            if ((handler != null) && !(base.IsDisposed || base.Disposing))
            {
                handler(sender, e);
            }
        }

        private void ImageListRecreateHandle(object sender, EventArgs e)
        {
            if (base.IsHandleCreated)
            {
                this.InvalidateIfNotSuspended();
            }
        }

        private void DetachImageList(object sender, EventArgs e)
        {
            this.ImageList = null;
        }

        private void DetachSmallImageList(object sender, EventArgs e)
        {
            this.SmallImageList = null;
        }

        void IComponentTreeHandler.InitializeRootElement(RootRadElement rootElement)
        {
        }

        RootRadElement IComponentTreeHandler.CreateRootElement()
        {
            return this.CreateRootElement();
        }

        void IComponentTreeHandler.CreateChildItems(RadElement parent)
        {
            this.CreateChildItems(parent);
        }

        public event ThemeNameChangedEventHandler ThemeNameChanged;

        /// <summary>
        /// Occurs when a RadItem instance iside the RadControl requires ToolTip text. 
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced),
        Category("Behavior"),
        Description("Occurs when a RadItem instance inside the RadControl requires ToolTip text. ")]
        public event ToolTipTextNeededEventHandler ToolTipTextNeeded
        {
            add
            {
                base.Events.AddHandler(ToolTipTextNeededEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(ToolTipTextNeededEventKey, value);
            }
        }

        /// <summary>
        /// Occurs prior the ScreenTip of a RadItem instance inside the RadControl is displayed.
        /// </summary>
        [Category("Behavior"),
         Description("Occurs prior the ScreenTip of a RadItem instance inside the RadControl is displayed.")]
        public event ScreenTipNeededEventHandler ScreenTipNeeded
        {
            add
            {
                base.Events.AddHandler(ScreenTipNeededEventKey, value);
            }
            remove
            {
                base.Events.RemoveHandler(ScreenTipNeededEventKey, value);
            }
        }

        void IComponentTreeHandler.CallOnThemeNameChanged(ThemeNameChangedEventArgs e)
        {
            this.CallOnThemeNameChanged(e);
        }

        internal void CallOnThemeNameChanged(ThemeNameChangedEventArgs e)
        {
            this.OnThemeNameChanged(e);
        }

        protected override void OnAutoSizeChanged(EventArgs e)
        {
            base.OnAutoSizeChanged(e);
            this.ElementTree.OnAutoSizeChanged(e);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            if (this.loaded)
            {
                this.elementTree.PerformInnerLayout(true, this.Location.X, this.Location.Y, this.Width, this.Height);
            }
        }

        protected override bool ScaleChildren
        {
            get
            {
                return this.ElementTree.ScaleChildren;
            }
        }

        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            BoundsSpecified bs = specified;
            if (!this.isInitialized)
            {
                bs = specified & (~BoundsSpecified.Size);
            }
            base.ScaleControl(factor, bs);
        }

        void IComponentTreeHandler.CallSetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            this.CallSetBoundsCore(x, y, width, height, specified);
        }

        internal void CallSetClientSizeCore(int x, int y)
        {
            this.SetClientSizeCore(x, y);
        }

        internal void CallSetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);
        }

        protected override void SetClientSizeCore(int x, int y)
        {
            if (!this.loaded)
            {
                base.SetClientSizeCore(x, y);
                return;
            }

            if (((this.FormBehavior != null && (clientWidthField != null))
                && ((clientHeightField != null)
                && (formStateField != null)))
                && (formStateSetClientSizeField != null))
            {
                Size sizeToSet = new Size(x + this.FormBehavior.ClientMargin.Horizontal, y + this.FormBehavior.ClientMargin.Vertical);
                base.Size = sizeToSet;
                clientWidthField.SetValue(this, x);
                clientHeightField.SetValue(this, y);
                BitVector32.Section section = (BitVector32.Section)formStateSetClientSizeField.GetValue(this);
                BitVector32 vector = (BitVector32)formStateField.GetValue(this);
                vector[section] = 1;
                formStateField.SetValue(this, vector);
                this.OnClientSizeChanged(EventArgs.Empty);
                vector[section] = 0;
                formStateField.SetValue(this, vector);
            }
            else
            {
                base.SetClientSizeCore(x, y);
            }

        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (this.loaded && this.wmWindowPosChangedProcessing && this.FormBehavior != null)
            {

                try
                {
                    FieldInfo formStateExWindowBoundsFieldInfo = typeof(Form).GetField("FormStateExWindowBoundsWidthIsClientSize", BindingFlags.NonPublic | BindingFlags.Static);
                    FieldInfo formStateExFieldInfo = typeof(Form).GetField("formStateEx", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo restoredBoundsFieldInfo = typeof(Form).GetField("restoredWindowBounds", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (((formStateExWindowBoundsFieldInfo != null) && (formStateExFieldInfo != null)) && (restoredBoundsFieldInfo != null))
                    {
                        Rectangle rectangle = (Rectangle)restoredBoundsFieldInfo.GetValue(this);
                        BitVector32.Section section = (BitVector32.Section)formStateExWindowBoundsFieldInfo.GetValue(this);
                        BitVector32 vector = (BitVector32)formStateExFieldInfo.GetValue(this);

                        if (vector[section] == 1)
                        {
                            width = rectangle.Width + this.FormBehavior.ClientMargin.Horizontal;
                            height = rectangle.Height + this.FormBehavior.ClientMargin.Vertical;

                        }
                    }
                }
                catch
                {
                }
            }

            base.SetBoundsCore(x, y, width, height, specified);

            if (this.loaded)
            {
                this.elementTree.SetBoundsCore(x, y, width, height, specified);
            }

        }
        public override Size GetPreferredSize(Size proposedSize)
        {
            Size preferredSize = base.GetPreferredSize(proposedSize);

            if (!this.IsLoaded || !this.isBehaviorPrepared)
            {
                return preferredSize;
            }

            if (!DWMAPI.IsCompositionEnabled)
            {
                if (this.FormBorderStyle != FormBorderStyle.None)
                {
                    preferredSize = new Size(preferredSize.Width + this.formBehavior.ClientMargin.Horizontal,
                        preferredSize.Height + this.formBehavior.ClientMargin.Top);
                }
            }

            return preferredSize;
        }

        Size IComponentTreeHandler.CallGetPreferredSize(Size proposedSize)
        {
            return this.CallGetPrefferedSize(proposedSize);
        }

        internal Size CallGetPrefferedSize(Size proposedSize)
        {
            return base.GetPreferredSize(proposedSize);
        }

        void IComponentTreeHandler.CallOnLayout(LayoutEventArgs e)
        {
            this.CallOnLayout(e);
        }

        internal void CallOnLayout(LayoutEventArgs e)
        {
            base.OnLayout(e);
        }

        public void InvalidateIfNotSuspended()
        {
            this.InvalidateIfNotSuspended(false);
        }


        internal void InvalidateIfNotSuspended(bool invalidateChildren)
        {
            if (this.IsUpdateSuspended)
            {
                return;
            }

            this.Invalidate(invalidateChildren);
        }

        public bool GetShowFocusCues()
        {
            return this.ShowFocusCues;
        }

        void IComponentTreeHandler.OnDisplayPropertyChanged(RadPropertyChangedEventArgs e)
        {
            this.OnDisplayPropertyChanged(e);
        }

        public bool IsDesignMode
        {
            get
            {
                return this.DesignMode;
            }
        }

        void IComponentTreeHandler.CallOnMouseCaptureChanged(EventArgs e)
        {
            this.CallOnMouseCaptureChanged(e);
        }

        internal void CallOnMouseCaptureChanged(EventArgs e)
        {
            base.OnMouseCaptureChanged(e);
        }

        void IComponentTreeHandler.CallOnToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            this.OnToolTipTextNeeded(sender, e);
        }

        public ComponentThemableElementTree ElementTree
        {
            get
            {
                if (this.elementTree == null)
                {
                    this.elementTree = new ComponentOverrideElementTree(this);
                }
                return this.elementTree;
            }
        }

        ComponentInputBehavior IComponentTreeHandler.Behavior
        {
            get
            {
                return this.Behavior;
            }
        }

        string IComponentTreeHandler.ThemeClassName
        {
            get
            {
                return this.ThemeClassName;
            }
            set
            {
                this.ThemeClassName = value;
            }
        }

        [Browsable(true), Category(RadDesignCategory.StyleSheetCategory)]
        [Description("Gets or sets theme name.")]
        [DefaultValue((string)"")]
        [Editor(DesignerConsts.ThemeNameEditorString, typeof(UITypeEditor))]
        public string ThemeName
        {
            get
            {
                return this.ElementTree.ThemeName;
            }
            set
            {
                if (this.ElementTree.ThemeName != value)
                {
                    this.ElementTree.ThemeName = value;
                    this.OnNotifyPropertyChanged("ThemeName");
                }
            }
        }

        [Browsable(true), Category(RadDesignCategory.DataCategory)]
        [Description("Gets the RootElement of a Control.")]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public RootRadElement RootElement
        {
            get
            {
                return this.ElementTree.RootElement;
            }
        }

        public void InvalidateElement(RadElement element)
        {
            if (this.IsUpdateSuspended)
                return;

            if (!this.Visible)
                return;

            Rectangle bounds = element.ControlBoundingRectangle;

            Point offset = element.GetScrollingOffset();
            bounds.Offset(-offset.X, -offset.Y);

            if (this.isBehaviorPrepared && this.IsLoaded)
            {
                this.formBehavior.InvalidateElement(element, bounds);
            }
            else
            {

                this.AddInvalidatedRect(bounds);
            }

            if (RadElement.TraceInvalidation)
            {
                Debug.WriteLine(String.Format("InvalidateElement(1): element = {0}; ElementBounds = {1}",
                    element.GetType().Name, element.ControlBoundingRectangle.ToString()));
            }
        }

        public void InvalidateElement(RadElement element, Rectangle bounds)
        {
            if (this.IsUpdateSuspended)
                return;

            if (this.isPainting)
                return;

            if (!this.Visible)
                return;

            if (this.isBehaviorPrepared && this.IsLoaded)
            {
                this.formBehavior.InvalidateElement(element, bounds);
            }
            else
            {
                this.AddInvalidatedRect(bounds);
            }
            if (RadElement.TraceInvalidation)
            {
                Debug.WriteLine(String.Format("InvalidateElement(2): element = {0}; bounds = {1}; ",
                    element.GetType().Name, bounds.ToString()));
            }
        }

        protected void AddInvalidatedRect(Rectangle rect)
        {
            if (rect.IsEmpty)
                return;

            rect.Inflate(1, 1);

            this.Invalidate(rect, false);
        }


        public void SuspendUpdate()
        {
            this.suspendUpdateCounter++;
        }

        public void ResumeUpdate()
        {
            this.ResumeUpdate(true);
        }

        protected bool IsUpdateSuspended
        {
            get
            {
                return this.suspendUpdateCounter > 0;
            }
        }

        public void ResumeUpdate(bool invalidate)
        {
            if (this.suspendUpdateCounter <= 0)
                return;

            this.suspendUpdateCounter--;

            if (invalidate && this.suspendUpdateCounter == 0)
            {
                this.Invalidate(true);
            }
        }

        ImageList IComponentTreeHandler.SmallImageList
        {
            get
            {
                return this.SmallImageList;
            }
            set
            {
                this.SmallImageList = value;
            }
        }

        /// <summary>
        ///		Gets or sets the ImageList that contains the images displayed by this control.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory), DefaultValue(null)]
        [Description("Gets or sets the ImageList that contains the images displayed by this control.")]
        public ImageList ImageList
        {
            get
            {
                return this.imageList;
            }
            set
            {
                if (this.imageList != value)
                {
                    EventHandler recreateHandle = new EventHandler(this.ImageListRecreateHandle);
                    EventHandler detachHandle = new EventHandler(this.DetachImageList);
                    if (this.imageList != null)
                    {
                        this.imageList.RecreateHandle -= recreateHandle;
                        this.imageList.Disposed -= detachHandle;
                    }
                    this.imageList = value;
                    this.OnNotifyPropertyChanged("ImageList");
                    if (this.imageList != null)
                    {
                        this.imageList.RecreateHandle += recreateHandle;
                        this.imageList.Disposed += detachHandle;
                    }
                    this.ElementTree.RootElement.NotifyControlImageListChanged();
                    this.InvalidateIfNotSuspended();
                }
            }
        }

        public Size SmallImageScalingSize
        {
            get
            {
                return this.smallImageScalingSize;
            }
            set
            {
                this.smallImageScalingSize = value;
            }
        }

        public Size ImageScalingSize
        {
            get
            {
                return this.smallImageScalingSize;
            }
            set
            {
                this.smallImageScalingSize = value;
            }
        }

        Telerik.WinControls.Themes.Design.RadControlDesignTimeData IComponentTreeHandler.DesignTimeData
        {
            get
            {
                if (designTimeData == null)
                {
                    designTimeData = this.CreateDesignTimeData();
                }

                if (designTimeData == null)
                {
                    RadThemeDesignerDataAttribute res = (RadThemeDesignerDataAttribute)TypeDescriptor.GetAttributes(this)[typeof(RadThemeDesignerDataAttribute)];
                    if (res != null)
                    {
                        designTimeData = Activator.CreateInstance(res.DesignTimeDataType) as RadControlDesignTimeData;
                        designTimeData.ControlName = this.Name;
                    }
                }

                if (designTimeData == null)
                {
                    designTimeData = new RadControlDefaultDesignTimeData(this);
                }

                return designTimeData;
            }
        }

        bool IComponentTreeHandler.Initializing
        {
            get
            {
                if (!this.loaded)
                {
                    return true;
                }

                return this.isInitializing;
            }
        }

        public void RegisterHostedControl(RadHostItem hostElement)
        {
            if (hostElement == null)
                return;

            if (!this.Controls.Contains(hostElement.HostedControl))
            {
                this.Controls.Add(hostElement.HostedControl);
                // Keep synchronized the layout state of hosted controls - see also RadHostItem.OnTunnelEvent()
                if (this.UseNewLayoutSystem && this.ElementTree.IsLayoutSuspended)
                    hostElement.HostedControl.SuspendLayout();
            }
        }

        public void UnregisterHostedControl(RadHostItem hostElement, bool removeControl)
        {
            if (hostElement == null)
                return;

            if (this.Controls.Contains(hostElement.HostedControl))
            {
                // Keep synchronized the layout state of hosted controls - see also RadHostItem.OnTunnelEvent()
                if (this.UseNewLayoutSystem && this.ElementTree.IsLayoutSuspended)
                    hostElement.HostedControl.ResumeLayout(false);
                if (removeControl)
                    this.Controls.Remove(hostElement.HostedControl);
            }
        }

        /// <summary>
        /// Gets a value indicating if control themes by default define PropertySettings for the specified element. 
        /// If true is returned the ThemeResolutionService would not not set any theme to the element to avoid duplicatingthe style
        /// settings of the element.
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool ControlDefinesThemeForElement(RadElement element)
        {
            return false;
        }

        protected virtual void OnScreenTipNeeded(object sender, ScreenTipNeededEventArgs e)
        {
            ScreenTipNeededEventHandler handler =
                base.Events[ScreenTipNeededEventKey] as ScreenTipNeededEventHandler;

            if ((handler != null) && !(base.IsDisposed || base.Disposing))
            {
                handler(sender, e);
            }
        }


        internal void CallOnToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
        {
            this.OnToolTipTextNeeded(sender, e);
        }

        internal void CallOnScreenTipNeeded(object sender, ScreenTipNeededEventArgs e)
        {
            this.OnScreenTipNeeded(sender, e);
        }

        void IComponentTreeHandler.CallOnScreenTipNeeded(object sender, ScreenTipNeededEventArgs e)
        {
            this.OnScreenTipNeeded(sender, e);
        }

        public void ControlThemeChangedCallback()
        {
            this.OnThemeChanged();
        }

        #endregion

        #region ILayoutHandler Members

        private ContextLayoutManager contextLayoutManager;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ILayoutManager LayoutManager
        {
            get
            {
                return this.contextLayoutManager;
            }
        }

        public void InvokeLayoutCallback(LayoutCallback callback)
        {
            if (!this.IsDisposed && !this.Disposing && this.IsHandleCreated)
            {
                this.BeginInvoke(callback, this.LayoutManager);
            }
        }

        #endregion

        #region ISupportInitializeNotification Members

        public event EventHandler Initialized;
        private bool isInitialized = false;

        public bool IsInitialized
        {
            get
            {
                return this.isInitialized;
            }
        }

        #endregion

        #region ISupportInitialize Members

        public virtual void BeginInit()
        {
            this.isInitializing = true;
        }

        public virtual void EndInit()
        {
            isInitializing = false;

            if (this.inputBehavior.CommandBindings.Count > 0)
            {
                this.inputBehavior.Shortcuts.AddShortcutsSupport();
            }

            this.isInitialized = true;

            //check whether a load request occurred while in initialization block
            if (this.IsHandleCreated && !this.loaded)
            {
                this.OnLoad(EventArgs.Empty);
            }
            else if (this.loaded)
            {
                this.elementTree.EnsureThemeAppliedInitially(true);
            }

            if (this.Initialized != null)
            {
                this.Initialized(this, EventArgs.Empty);
            }
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Occurs when when a property of an object changes change. 
        /// Calling the event is developer's responsibility.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                this.Events.AddHandler(PropertyChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(PropertyChangedEventKey, value);
            }
        }
        private static readonly object PropertyChangedEventKey;
        #endregion

        #region Nested Types

        private class ComponentOverrideElementTree : ComponentThemableElementTree
        {
            public ComponentOverrideElementTree(RadFormControlBase owner)
                : base(owner)
            {
            }

            protected internal override void CreateChildItems(RadElement parent)
            {
                this.ComponentTreeHandler.CreateChildItems(parent);
            }

            protected override RootRadElement CreateRootElement()
            {
                return (this.ComponentTreeHandler).CreateRootElement();
            }

            protected override void InitializeRootElement(RootRadElement rootElement)
            {
                if (!(this.Control as RadFormControlBase).controlIsInitializingRootComponent)
                {
                    this.ComponentTreeHandler.InitializeRootElement(rootElement);
                }
                else
                {
                    (this.Control as RadFormControlBase).controlIsInitializingRootComponent = false;
                }
            }

            public override bool ControlDefinesThemeForElement(RadElement element)
            {
                return this.ComponentTreeHandler.ControlDefinesThemeForElement(element);
            }
        }

        #endregion
    }
}




