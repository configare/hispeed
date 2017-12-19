using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Paint;
using System.ComponentModel;
using System.Diagnostics;
using System.ComponentModel.Design.Serialization;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Themes.Design;
using Telerik.WinControls.Layouts;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using System.Drawing.Design;
using Telerik.WinControls.Design;
using System.Security.Permissions;
using System.Globalization;
using Telerik.WinControls.Keyboard;
using System.Drawing.Drawing2D;
using System.Reflection;
using Telerik.WinControls.Assistance;
using System.ComponentModel.Design;


namespace Telerik.WinControls
{
    

	/// <summary>
	/// Represents a RadControl. RadControl is an abstract class and is base class for
	/// all Telerik controls.
	/// </summary>
	[Designer(DesignerConsts.RadControlDesignerString)]
	[DesignerSerializer(DesignerConsts.RadControlCodeDomSerializerString, DesignerConsts.CodeDomSerializerString)]
    [TypeDescriptionProvider(typeof(ReplaceRadControlProvider))]
    [ToolboxItem(false)]
	public class RadControl : ScrollableControl, INotifyPropertyChanged, ISupportInitializeNotification, IComponentTreeHandler
	{
#if OEM
		//internal static readonly string OemAssemblyName = "OEM ASSEMBLY NAME HERE";

		internal static bool? designTime = null;
		internal static bool isOem = false;
#endif

		// Fields
		protected ComponentThemableElementTree elementTree;
		protected  ComponentInputBehavior behavior;
		private bool loaded;
		private static readonly object ToolTipTextNeededEventKey = null;

		private static readonly object ScreenTipNeededEventKey = new object();
		private bool controlIsInitializingRootComponent = false;
		private bool isResizing;
		private bool isDisposing;
		private bool isDisplayed;
		private bool isFocusable = true;
		private Rectangle invalidResizeRect;

#if (EVALUATION || OEM)
		internal static int licenseCount = -1;
#endif

		private ImageList imageList = null;
		private Size imageScalingSize = Size.Empty;

		private ContextLayoutManager layoutManager = null;

		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ILayoutManager LayoutManager
		{
			get
			{
				return this.layoutManager;
			}
		}

		public void InvokeLayoutCallback(LayoutCallback callback)
		{
			if (this.IsHandleCreated && !this.Disposing && !this.IsDisposed)
				this.BeginInvoke(callback, this.layoutManager);
		}

		/// <summary>
		/// Determines whether the control is properly loaded.
		/// </summary>
		[Browsable(false)]
		public bool IsLoaded
		{
			get
			{
				return this.loaded;
			}
		}

		#region Constructors/Initializers

		static RadControl()
		{
			ToolTipTextNeededEventKey = new object();
			PropertyChangedEventKey = new object();
			
#if OEM
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				string assemblyFullName = assembly.FullName;
				string assemblyName = assemblyFullName.Substring(0, assemblyFullName.IndexOf(','));
				if (assemblyName == RadControl.OemAssemblyName)
				{
					isOem = true;
					break;
				}
			}
#endif
		}

		public RadControl()
		{
            MeasurementGraphics.IncreaseControlCount();

			this.Construct();

			base.SetAutoSizeMode(AutoSizeMode.GrowAndShrink);

			base.SetStyle(ControlStyles.UserPaint |
						  ControlStyles.AllPaintingInWmPaint |
						  ControlStyles.OptimizedDoubleBuffer |
						  ControlStyles.SupportsTransparentBackColor, true);

			this.smallImageScalingSize = new Size(16, 16);
			this.imageScalingSize = new Size(16, 16);
		}

		protected virtual void Construct()
		{
			this.layoutManager = new ContextLayoutManager(this);
			this.behavior = this.CreateBehavior();
			this.elementTree = new ComponentOverrideElementTree(this);
			this.elementTree.EnsureRootElement();
			this.elementTree.RootElement.UseNewLayoutSystem = this.GetUseNewLayout();
			this.elementTree.CreateChildItems(this.elementTree.RootElement);
			this.elementTree.RootElement.SetChildrenLocalValuesAsDefault(true);
		}

		/// <summary>
		/// Creates the input behavior instance. Allows inheritors to provide custom input implementations.
		/// </summary>
		/// <returns></returns>
		protected virtual ComponentInputBehavior CreateBehavior()
		{
			return new ComponentInputBehavior(this);
		}

		/// <summary>
		/// Determines whether the control and all its child elements should use the new layout system.
		/// </summary>
		/// <returns></returns>
		protected virtual bool GetUseNewLayout()
		{
			return true;
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
			if (!this.loaded)
			{
				this.OnLoad(desiredSize);
			}
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

			bool newLayout = this.GetUseNewLayout();
			//adjust the AutoSize mode for old layout system
			if (!newLayout)
			{
				if (this.AutoSize)
				{
					this.elementTree.RootElement.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
				}
				else
				{
					this.elementTree.RootElement.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
				}
			}

			this.elementTree.EnsureThemeAppliedInitially(false);
			//notify all elements, listening for the ImageList of the control
			this.elementTree.RootElement.NotifyControlImageListChanged();
			//notify the root element for the loading event
			this.elementTree.RootElement.OnLoad(true);

			//we are already loaded
			this.loaded = true;

			desiredSize = this.GetInitialDesiredSize(desiredSize, newLayout);

			if (this.AutoSize)
			{
				base.SetBoundsCore(Left, Top, desiredSize.Width, desiredSize.Height, BoundsSpecified.All);
			}
		}

		protected internal virtual Size GetInitialDesiredSize(Size availableSize, bool useNewLayoutSystem)
		{
			//set the current control bounds as initial to the element tree
			if (useNewLayoutSystem)
			{
				return this.elementTree.PerformInnerLayout(true, Left, Top, availableSize.Width, availableSize.Height);
			}
			else
			{
				this.elementTree.PerformLayoutInternal(this.RootElement);
				return this.elementTree.GetPreferredSize(availableSize);
			}
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();

			this.LoadElementTree(this.Size);
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			base.OnHandleDestroyed(e);

			//special case, when Control's handle is destroyed but the Control is not disposed, we need to unload the element tree
			//Dido: we do not need to unload the element tree when the handle is being recreated.
			if (!this.isDisposing && !this.RecreatingHandle)
			{
				this.elementTree.RootElement.OnUnload(this.elementTree, true);
				this.elementTree.RootElement.SetThemeApplied(false);
				this.loaded = false;
			}
		}

		#endregion

		#region Public Properties

#if DEBUG
		[Browsable(true)]
#else
		[Browsable(false)]
#endif
		public virtual ComponentThemableElementTree ElementTree
		{
			get
			{
				return this.elementTree;
			}
		}

		/// <summary>Gets the input behavior for the control.</summary>
		[Browsable(false)]
		public ComponentInputBehavior Behavior
		{
			get
			{
				return this.behavior;
			}
		}

		/// <summary>Gets the RootElement of the control.</summary>
		[Browsable(true), Category(RadDesignCategory.DataCategory)]
		[Description("Gets the RootElement of a Control.")]
		[TypeConverter(typeof(ExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public RootRadElement RootElement
		{
			get
			{
				if (this.elementTree == null)
				{
					return null;
				}
				return this.elementTree.RootElement;
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
				if (this.elementTree == null)
				{
					return false;
				}
				return this.elementTree.RootElement.UseNewLayoutSystem;
			}
			set
			{
				if (this.elementTree == null || this.UseNewLayoutSystem == value)
				{
					return;
				}

				this.elementTree.RootElement.UseNewLayoutSystem = value;
				this.OnNotifyPropertyChanged("UseNewLayoutSystem");
			}
		}

		/// <summary>
		///     Gets or sets control's preffered theme name. Themes are stored and retrieved using
		///     APIs of <see cref="Telerik.WinControls.ThemeResolutionService"/>.
		/// </summary>
		/// <remarks>
		/// If <strong>ThemeResolutionService.ApplicatonThemeName</strong> refers to a
		/// non-empty string, the theme of a RadControl can differ from the one set using
		/// <strong>RadControls.ThemeName</strong> property. If the themes differ, the
		/// <strong>RadControls.ThemeName</strong> property will be overridden by
		/// <strong>ThemeResolutionService.ApplicatonThemeName</strong>. If no theme is registered
		/// with a name as <strong>ThemeResolutionService.ApplicatonThemeName</strong>, then
		/// control will revert to the theme specified by its <strong>ThemeName</strong> property.
		/// If <strong>ThemeName</strong> is assigned to a non-existing theme name, the control may
		/// have no visual properties assigned, which will cause it look and behave in unexpected
		/// manner. If <strong>ThemeName</strong> equals empty string, control's theme is set to a
		/// theme that is registered within <strong>ThemeResolutionService</strong> with the name
		/// "ControlDefault".
		/// </remarks>
		[Browsable(true), Category(RadDesignCategory.StyleSheetCategory)]
		[Description("Gets or sets theme name.")]
		[DefaultValue((string)"")]
		[Editor(DesignerConsts.ThemeNameEditorString, typeof(UITypeEditor))]
		public string ThemeName
		{
			get 
			{ 
				return this.elementTree.ThemeName; 
			}
			set
			{
				if (this.elementTree.ThemeName != value)
				{
					this.elementTree.ThemeName = value;
					this.OnNotifyPropertyChanged("ThemeName");
				}
			}
		}

		[Editor("Telerik.WinControls.UI.TextOrHtmlSelector, Telerik.WinControls.RadMarkupEditor, Version=" + VersionNumber.Number +", Culture=neutral, PublicKeyToken=5bb2a467cbec794e",typeof(UITypeEditor)),
		Localizable(true),
		Browsable(true), Category(RadDesignCategory.BehaviorCategory),
		Description("Gets or sets the text associated with this control."),
		Bindable(true),
		SettingsBindable(true), 
		DefaultValue("")]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text = value;
			}
		}

		/// <summary>
		/// Gets or sets value indicating whether the control is styled through theme
		/// </summary>
		[DefaultValue(true)]
		[Category(RadDesignCategory.StyleSheetCategory)]
		public bool EnableTheming
		{
			get
			{
				return this.ElementTree.EnableTheming;
			}
			set
			{
				this.ElementTree.EnableTheming = value;
			}
		}

		/// <summary>
		/// Gets or sets the class name string that ThemeResolutionService will use to find the themes registered for the control.
		/// </summary>
		/// <remarks>
		/// By default the return value is RadControl's type FullName; Some controls like drop down menu has different ThemeClassName
		/// depending on the runtime usaage of the control.
		/// </remarks>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Category(RadDesignCategory.StyleSheetCategory)]
		public virtual string ThemeClassName
		{
			get
			{
				if (this.elementTree == null)
				{
					throw new ObjectDisposedException("Attempting to accept already disposed control");
				}
				return this.elementTree.ThemeClassName;
			}
			set
			{
				if (this.elementTree.ThemeClassName == value)
				{
					return;
				}

				this.elementTree.ThemeClassName = value;
				this.OnNotifyPropertyChanged("ThemeClassName");
			}
		}

		/// <summary>Gets or sets StyleSheet for the control. Generally the stylesheet is assigned automatically when control's ThemeName is assigned.</summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[DefaultValue(null)]
		[Browsable(true), Category(RadDesignCategory.StyleSheetCategory)]
		[Description("Gets or sets StyleSheet for the control.")]
		public StyleSheet Style
		{
			get
			{
				return this.RootElement.Style;
			}
			set
			{
				if (this.RootElement.Style != value)
				{
					this.RootElement.Style = value;
					this.OnNotifyPropertyChanged("Style");
				}
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
					this.elementTree.RootElement.NotifyControlImageListChanged();
					this.InvalidateIfNotSuspended();
				}
			}
		}

		/// <summary>Gets or sets the image scaling size.</summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[DefaultValue(typeof(Size), "16,16")]
		public Size ImageScalingSize
		{
			get
			{
				return this.imageScalingSize;
			}
			set
			{
				if (this.imageScalingSize != value)
				{
					this.imageScalingSize = value;
					this.OnNotifyPropertyChanged("ImageScalingSize");
				}
			}
		}

		/// <summary>
		/// Determines whether to use compatible text rendering engine (GDI+) or not (GDI). 
		/// </summary>
		[DefaultValue(true)]
		[Category(RadDesignCategory.BehaviorCategory)]
		[Description("Determines whether to use compatible text rendering engine (GDI+) or not (GDI).")]
		public virtual bool UseCompatibleTextRendering
		{
			get
			{
				return this.RootElement.UseCompatibleTextRendering;
			}
			set
			{
				this.RootElement.UseCompatibleTextRendering = value;
			}
		}

		/// <summary>
		/// 	<para>Gets or sets a value indicating whether the control is automatically resized
		///     to display its entire contents.</para>
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		Description("Gets or sets a value indicating whether the control is automatically resized to display its entire contents."),
		Browsable(true), Category(RadDesignCategory.LayoutCategory), DefaultValue(false),
		EditorBrowsable(EditorBrowsableState.Always)]
		public override bool AutoSize
		{
			get
			{
				return base.AutoSize;
			}
			set
			{
				base.AutoSize = value;
				this.OnNotifyPropertyChanged("AutoSize");
			}
		}

		/// <summary>
		/// Gets or sets the size that is the upper limit that GetPreferredSize can
		/// specify.
		/// </summary>
		public override Size MaximumSize
		{
			get
			{
				Size maxSize = this.RootElement.MaxSize;
				return maxSize;
			}
			set
			{
				base.MaximumSize = value;
				this.RootElement.MaxSize = value;
			}
		}

		/// <summary>
		/// Gets or sets the size that is the lower limit that GetPreferredSize can
		/// specify
		/// </summary>
		public override Size MinimumSize
		{
			get
			{
				Size minSize = this.RootElement.MinSize;
				return minSize;
			}
			set
			{
				base.MinimumSize = value;
				this.RootElement.MinSize = value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool Focusable
		{
			get
			{
				return this.isFocusable;
			}
			set
			{
				this.isFocusable = value;
			}
		}

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

		#region ISupportInitializeNotification Members

		/// <summary>Fires when the control is initialized.</summary>
		public event EventHandler Initialized;

		private bool isInitialized = false;

		bool ISupportInitializeNotification.IsInitialized
		{
			get 
			{ 
				return this.isInitialized; 
			}
		}

		#endregion

		#region ISupportInitialize Members

		internal bool isInitializing = false;

		/// <summary>Suspends layout during initialization.</summary>
		public virtual void BeginInit()
		{
			this.isInitializing = true;
		}

		protected bool IsInitializing
		{
			get
			{
				return this.isInitializing;
			}
		}

		/// <summary>Resumes layout.</summary>
		public virtual void EndInit()
		{
			isInitializing = false;

			if (this.behavior.CommandBindings.Count > 0)
			{
				this.behavior.Shortcuts.AddShortcutsSupport();
			}

			//explicitly apply the BindingContext after initialization is complete
			this.RootElement.BindingContext = this.BindingContext;

			this.isInitialized = true;

			//check whether a load request occurred while in initialization block
			if (this.IsHandleCreated && !this.loaded)
			{
				this.LoadElementTree(this.Size);
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

		#region Property Changed
		/// <summary>
		/// Must be overriddne when control should perform special logic prior to
		/// repainting.
		/// </summary>
		protected internal virtual void OnDisplayPropertyChanged(RadPropertyChangedEventArgs e)
		{
		}

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);
			this.RootElement.Enabled = this.Enabled;
		}

		protected override void OnBindingContextChanged(EventArgs e)
		{
			base.OnBindingContextChanged(e);

			//assign the binding context to the root element
			//TODO: Check thoroughly
			this.RootElement.BindingContext = this.BindingContext;
		}

		protected override void OnLocationChanged(EventArgs e)
		{
			base.OnLocationChanged(e);
			if (!this.loaded)
			{
				return;
			}

			if (this.elementTree.UseNewLayoutSystem)
			{
				this.RootElement.ForceLocationTo(this.Location);
			}
			else
			{
				if (!this.RootElement.GetBitState(RootRadElement.RootElementInitiatedPropertyChangeStateKey))
				{
					this.RootElement.BitState[RootRadElement.ControlInitiatedPropertyChangeStateKey] = true;
					this.RootElement.Location = this.Location;
				}
			}
		}

		protected override void OnPaddingChanged(EventArgs e)
		{
			if (!this.RootElement.GetBitState(RootRadElement.RootElementInitiatedPropertyChangeStateKey))
			{
				this.RootElement.Padding = this.Padding;
			}

			if (this.elementTree.RootElement.Children.Count > 0)
			{
				this.elementTree.RootElement.Children[0].Padding = this.Padding;
			}

			base.OnPaddingChanged(e);
		}

		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged(e);
			this.RootElement.RightToLeft = this.GetRightToLeft(this, this.RightToLeft);
		}

		private bool GetRightToLeft(Control control, RightToLeft value)
		{
			switch (value)
			{
				case RightToLeft.Inherit:
					if (control.Parent != null)
					{
						return GetRightToLeft(control.Parent, control.Parent.RightToLeft);
					}
					else
					{
						return false;
					}
				case RightToLeft.No:
					return false;
				case RightToLeft.Yes:
					return true;
				default:
					return false;
			}
		}
		
		#endregion

		#region Invalidate

		private bool isPainting = false;

		public override void Refresh()
		{
			if (this.IsUpdateSuspended)
				return;

			base.Refresh();
		}

		public virtual void InvalidateElement(RadElement element)
		{
			if (this.IsUpdateSuspended || !this.Visible)
			{
				return;
			}
			
			//prepare the invalid rect, depending on the visual element
			Rectangle bounds = element.ControlBoundingRectangle;
			Point offset = element.GetScrollingOffset();
			bounds.Offset(-offset.X, -offset.Y);

			this.AddInvalidatedRect(bounds);
			this.OnInvalidated(element);
			if (RadElement.TraceInvalidation)
			{
				Debug.WriteLine(String.Format("InvalidateElement(1): element = {0}; ElementBounds = {1}",
					element.GetType().Name, bounds.ToString()));
			}
		}

		/// <summary>
		/// Forces repaint of the elements specified and waits untinl this operation completes.
		/// </summary>
		/// <param name="elements"></param>
		public void RepaintElements(params RadElement[] elements)
		{
			if (this.IsUpdateSuspended)
				return;

			for (int i = 0; i < elements.Length; i++)
			{
				RadElement element = elements[i];
				Rectangle bounds = element.ControlBoundingRectangle;

				Point offset = element.GetScrollingOffset();
				bounds.Offset(-offset.X, -offset.Y);

				NativeMethods.RECT rcUpdate =
					NativeMethods.RECT.FromXYWH(bounds.X, bounds.Y, bounds.Width, bounds.Height);
				NativeMethods.RedrawWindow(new HandleRef(this, this.Handle), ref rcUpdate, NativeMethods.NullHandleRef, 0x85);

				if (RadElement.TraceInvalidation)
				{
					Debug.WriteLine(String.Format("InvalidateElement(1): element = {0}; ElementBounds = {1}",
						element.GetType().Name, bounds.ToString()));
				}
			}
			this.Update();
		}

		#region InvalidatedEvent
		private static object InvalidatedEventKey = new object();

		public event EventHandler ElementInvalidated
		{
			add
			{
				this.Events.AddHandler(RadControl.InvalidatedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadControl.InvalidatedEventKey, value);
			}
		}

		protected virtual void OnInvalidated(RadElement element)
		{
			EventHandler handler1 =
			(EventHandler)this.Events[RadControl.InvalidatedEventKey];
			if (handler1 != null)
			{
				handler1(element, null);
			}
		}
		#endregion

		public void InvalidateElement(RadElement element, Rectangle bounds)
		{
			if (this.IsUpdateSuspended)
				return;

			if (this.isPainting)
				return;

			if (!this.Visible)
				return;
			this.OnInvalidated(element);
			//InitPaintTimer();

			//if (this.UseNewLayoutSystem)
			{
				//    InvalidateElement(element);
				//invalidRect = invalidRect.IsEmpty ? bounds : Rectangle.Union(invalidRect, bounds);
			}
			// else
			{
				//if (element.OldTotalTransformationMatrix == null)
				//    bounds = this.ClientRectangle;
				//else
				//    bounds = TelerikHelper.GetBoundingRect(bounds, element.OldTotalTransformationMatrix);

				this.AddInvalidatedRect(bounds);
				//invalidRect = invalidRect.IsEmpty ? bounds : Rectangle.Union(invalidRect, bounds);
			}

			if (RadElement.TraceInvalidation)
			{
				Debug.WriteLine(String.Format("InvalidateElement(2): element = {0}; bounds = {1}; ",
					element.GetType().Name, bounds.ToString()));
			}

			//this.paintTimer_Tick(this, System.EventArgs.Empty);
		}

		internal virtual void AddInvalidatedRect(Rectangle rect)
		{
			if (rect.IsEmpty)
				return;

			// PATCH
			rect.Inflate(1, 1);

			//for (int i = 0; i < this.invalidRectList.Count; ++i)
			//{
			//    Rectangle alredyInvalidated = this.invalidRectList[i];
			//    if (alredyInvalidated.Contains(rect))
			//        return;
			//}

			//invalidRectList.Add(rect);
			if (!this.isResizing)
			{
				this.Invalidate(rect, false);
			}
			else
			{
				if (this.invalidResizeRect == Rectangle.Empty)
				{
					this.invalidResizeRect = rect;
				}
				else
				{
					this.invalidResizeRect = Rectangle.Union(rect, this.invalidResizeRect);
				}
			}
		}

		//void InitPaintTimer()
		//{
		//    if (this.paintTimer == null)
		//    {
		//        this.paintTimer = new System.Timers.Timer();
		//        this.paintTimer.Interval = 1000 / RadElement.RenderingMaxFramerate;
		//        this.paintTimer.Elapsed += new System.Timers.ElapsedEventHandler(paintTimer_Elapsed);
		//        this.paintTimer.Start();
		//    }
		//    else if (this.paintTimer.Interval != (1000 / RadElement.RenderingMaxFramerate))
		//    {
		//        this.paintTimer.Stop();
		//        this.paintTimer.Interval = 1000 / RadElement.RenderingMaxFramerate;
		//        this.paintTimer.Start();
		//    }
		//}

		//void paintTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		//{
		//    if (this.IsHandleCreated)
		//    {
		//        BeginInvoke(new MethodInvoker(DoInvalidate));
		//    }
		//}

		//private int idleTicks = 0;

		//void paintTimer_Tick(object sender, EventArgs e)
		//{
		//    /*if (!invalidRect.IsEmpty)
		//    {
		//        this.Invalidate(invalidRect, true);
		//        invalidRect = Rectangle.Empty;
		//    }*/
		//}

		//private void DoInvalidate()
		//{
		//    if (invalidRectList.Count > 0)
		//    {
		//        for (int i = 0; i < invalidRectList.Count; i++)
		//        {
		//            this.Invalidate(invalidRectList[i], false);
		//        }

		//        invalidRectList.Clear();
		//    }
		//    else
		//    {
		//        idleTicks++;
		//        if (idleTicks > 30)
		//        {
		//            if (this.paintTimer != null)
		//            {
		//                this.paintTimer.Stop();
		//                this.paintTimer.Dispose();
		//                this.paintTimer = null;
		//            }
		//            idleTicks = 0;
		//        }
		//    }
		//}

		private int suspendUpdateCounter = 0;

		public void SuspendUpdate()
		{
			this.suspendUpdateCounter++;
		}

		public void ResumeUpdate()
		{
			this.ResumeUpdate(true);
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

		private bool IsUpdateSuspended
		{
			get
			{
				return this.suspendUpdateCounter > 0;
			}
		}

		internal void InvalidateIfNotSuspended(bool invalidateChildren)
		{
			if (this.IsUpdateSuspended)
			{
				return;
			}

			this.Invalidate(invalidateChildren);
		}

		public void InvalidateIfNotSuspended()
		{
			this.InvalidateIfNotSuspended(false);
		}
		#endregion

		#region INotifyPropertyChanged Members
		
		private static readonly object PropertyChangedEventKey;

		/// <summary>
		/// Occurs when when a property of an object changes change. 
		/// Calling the event is developer's responsibility.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				this.Events.AddHandler(RadControl.PropertyChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadControl.PropertyChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the PropertyChanged event
		/// </summary>
		/// <param name="propertyName">The name of the property</param>
		protected virtual void OnNotifyPropertyChanged(string propertyName)
		{			
			this.OnNotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void OnNotifyPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChangedEventHandler handler1 =
				(PropertyChangedEventHandler)this.Events[RadControl.PropertyChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		#endregion

		internal CreateParams GetCreateParams()
		{
			return this.CreateParams;
		}

		IDesignerHost GetDesigner()
		{
			if (this.DesignMode)
			{
				return (IDesignerHost)Site.GetService(typeof(IDesignerHost));                        
			}
			return null;
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void ResetBackColor() 
		{ 
			BackColor = Color.Empty; 
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void ResetForeColor() 
		{ 
			ForeColor = Color.Empty; 
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

		#region System

#if DEBUG__
		internal List<RadElement> _ElementPaintList = new List<RadElement>();
		int paintCount = 0;
#endif

		protected override void OnPaint(PaintEventArgs e)
		{

			// BM_18_02_2008
			// Measure timespan
			//before = DateTime.Now.Ticks;

			//base.OnPaint(e);


			// EP_16_11_2007: 
			// Do not remove the following line of code! Needed because of some MS code,
			// related to showing the FocusCues for the first time!
			bool whatever = this.ShowFocusCues;


			this.isPainting = true;
			e.Graphics.PixelOffsetMode = PixelOffsetMode.None;
			this.RootElement.Paint(new RadGdiGraphics(e.Graphics), e.ClipRectangle);
			//Debug.WriteLine("Paint: " + e.ClipRectangle);
			//this.RootElement.Paint(new RadGdiGraphics(e.Graphics), this.ClientRectangle);
			/*if (this.GetType().Name == "MainControl")
				Console.WriteLine("RadControl.OnPaint(): ClipRectangle = {0}, ClientRectangle = {1}",
					e.ClipRectangle, this.ClientRectangle);*/
			base.OnPaint(e);

			// BM_18_02_2008
			// Measure timespan
			//counter++;

			//if (counter == 1000)
			//{
			//    Debug.WriteLine(((ticks / TimeSpan.TicksPerMillisecond)));

			//    counter = 0;
			//    ticks = 0;
			//}
			//ticks += (DateTime.Now.Ticks - before);

			this.isPainting = false;
#if DEBUG__

			Debug.WriteLine("");
			Debug.Write(paintCount++ + " " + e.ClipRectangle + " - " + this._ElementPaintList.Count);
			foreach(RadElement element in this._ElementPaintList)
			{
				Debug.Write("; " + element.GetType().Name);
			}
			this._ElementPaintList.Clear();
#endif
		}

		/// <summary>
		/// Gets or sets a value indicating whether the control causes validation to be performed on any controls that require validation when it receives focus.
		/// </summary>
		//[DefaultValue(false)]
		[Description("Gets or sets a value indicating whether the control causes validation to be performed on any controls that require validation when it receives focus.")]
		new public bool CausesValidation
		{
			get
			{
				return GetCausesValidation();
			}
			set
			{
				base.CausesValidation = value;
			}
		}

		protected virtual bool GetCausesValidation()
		{
			if (this.ContainsFocus || this.Focused)
			{
				return false;
			}
			return base.CausesValidation;
		}

		protected override void WndProc(ref Message m)
		{
			int msg = m.Msg;
			if (msg == NativeMethods.WM_LBUTTONDOWN || msg == NativeMethods.WM_LBUTTONUP ||
				msg == NativeMethods.WM_RBUTTONDOWN || msg == NativeMethods.WM_RBUTTONUP ||
				msg == NativeMethods.WM_MBUTTONDOWN || msg == NativeMethods.WM_MBUTTONUP)
			{
				if (this.CausesValidation)
				{
					this.ProcessFocusRequested(null);
					this.Behavior.ValidationCanceledAction();
				}
			}
			else if (msg == NativeMethods.WM_CAPTURECHANGED)
			{
				this.OnCaptureLosing();
			}

			base.WndProc(ref m);

			if (msg == NativeMethods.WM_SHOWWINDOW)
			{
				this.isDisplayed = m.WParam != IntPtr.Zero;
			}

			if (this.Behavior.ValidationCancelled && this.Capture)
			{
				this.ProcessCaptureChangeRequested(null, false);
			}
		}

		protected virtual void OnCaptureLosing()
		{
		}

		protected IntPtr SendMessage(int msg, bool wparam, int lparam)
		{
			return NativeMethods.SendMessage(new HandleRef(this, this.Handle), msg, wparam, lparam);
		}

		public virtual void RegisterHostedControl(RadHostItem hostElement)
		{
			if (hostElement == null || hostElement.HostedControl.Parent == this)
				return;

			//suspend layout, the host item will take care of its hosted control's bounds
			this.SuspendLayout();
			this.Controls.Add(hostElement.HostedControl);
			this.ResumeLayout(false);
		}

		public virtual void UnregisterHostedControl(RadHostItem hostElement, bool removeControl)
		{
			if (hostElement == null)
				return;

			//suspend layout, the host item will take care of the layout
			this.SuspendLayout();

			if (hostElement.HostedControl != null && hostElement.HostedControl.Parent == this)
			{
				// Keep synchronized the layout state of hosted controls - see also RadHostItem.OnTunnelEvent()
				if (this.UseNewLayoutSystem && this.elementTree.IsLayoutSuspended)
					hostElement.HostedControl.ResumeLayout(false);
				if (removeControl)
					this.Controls.Remove(hostElement.HostedControl);
			}

			this.ResumeLayout(false);
		}


		protected override void Dispose(bool disposing)
		{
#if (EVALUATION || OEM)
			RadControl.licenseCount = -1;
#endif
			this.isDisposing = true;
			if (disposing)
			{
				this.SuspendLayout();

				// The element tree will remove all elements from the layout queues
				if (this.elementTree != null)
				{
					this.elementTree.Dispose(disposing);
				}
				if (this.layoutManager != null)
				{
					this.layoutManager.Dispose();
					this.layoutManager = null;
				}

				if (this.Region != null)
				{
					this.Region.Dispose();
				}

                MeasurementGraphics.DecreaseControlCount();

				this.Behavior.Dispose();                
			}

			base.Dispose(disposing);
		}


		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void RadAccessibilityNotifyClients(AccessibleEvents accEvent, int childID)
		{
			this.AccessibilityNotifyClients(accEvent, -4, childID);
		}
		#endregion

		internal void CallOnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged(e);
		}

		public bool GetShowFocusCues()
		{
			return this.ShowFocusCues;
		}

		internal virtual int ShowParams
		{
			get
			{
				return 5;
			}
		}

		/// <summary>
		/// Determines whether an element from this element tree may be displayed in the EditUIElements dialog.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool CanEditUIElement(RadElement element)
		{
			//no restrictions for run-time
			if (!this.DesignMode)
			{
				return true;
			}

			return this.CanEditElementAtDesignTime(element);
		}

		/// <summary>
		/// Determines whether an element may be edited via the EditUIElements dialog at design-time.
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected virtual bool CanEditElementAtDesignTime(RadElement element)
		{
			return true;
		}

		[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
		protected override bool ProcessMnemonic(char charCode)
		{
			if (TelerikHelper.CanProcessMnemonic(this) &&
				(this.Focused || base.ContainsFocus) &&
				this.Behavior.ProcessMnemonic(charCode))
			{
				return true;
			}
			return base.ProcessMnemonic(charCode);
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

		#region Small ImageList
		private ImageList smallImageList = null;
		private Size smallImageScalingSize = Size.Empty;

		private void DetachSmallImageList(object sender, EventArgs e)
		{
			this.SmallImageList = null;
		}

		/// <summary>
		///		Gets or sets the SmallImageList that contains the small images which are displayed when there's not enough space.
		/// </summary>
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
					this.elementTree.RootElement.NotifyControlImageListChanged();
					this.InvalidateIfNotSuspended();
				}
			}
		}

		/// <summary>Gets or sets the small image scaling size.</summary>
		[Browsable(false), Category(RadDesignCategory.AppearanceCategory)]
		[DefaultValue(typeof(Size), "16,16")]
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
		#endregion

		#region Layout Management

		protected override bool ScaleChildren
		{
			get
			{
				return this.elementTree.ScaleChildren;
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

#if DEBUG
		private bool IsControlSuspended(Control c)
		{
			if (c != null)
			{
				PropertyInfo pi = typeof(Control).GetProperty("IsLayoutSuspended", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.ExactBinding);
				return (bool)pi.GetValue(c, null);
			}
			return false;
		}

		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool MSParentLayoutSuspended
		{
			get
			{
				return IsControlSuspended(this.Parent);
			}
			set
			{
				Control parent = this.Parent;
				if (parent != null)
				{
					if (IsControlSuspended(parent))
						parent.ResumeLayout(true);
					else
						parent.SuspendLayout();
				}
			}
		}

		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool MSParentParentLayoutSuspended
		{
			get
			{
				return IsControlSuspended(this.ParentParent);
			}
			set
			{
				Control parent = this.ParentParent;
				if (parent != null)
				{
					if (IsControlSuspended(parent))
						parent.ResumeLayout(true);
					else
						parent.SuspendLayout();
				}
			}
		}

		[TypeConverter(typeof(ExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		public new Control Parent
		{
			get { return base.Parent; }
			set { base.Parent = value; }
		}

		[TypeConverter(typeof(ExpandableObjectConverter))]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		public Control ParentParent
		{
			get
			{
				if (base.Parent == null)
					return null;
				return base.Parent.Parent;
			}
		}
#endif

		protected override void OnLayout(LayoutEventArgs e)
		{
			if (!this.loaded || this.isDisposing)
			{
				base.OnLayout(e);
				return;
			}
			//a layout event may occur even if control's size is not changed.
			//in this case we need to invalidate the layout and perform a new pass.
			this.elementTree.OnLayout(e, this.Bounds, this.AutoSize, this.Parent);
			if (!this.isResizing && this.elementTree.UseNewLayoutSystem && !this.layoutManager.IsUpdating)
			{
				if (!this.elementTree.RootElement.IsLayoutSuspended)
				{
					this.elementTree.RootElement.InvalidateMeasure();
					this.elementTree.RootElement.UpdateLayout();
				}
			}
		}

		internal void CallOnLayout(LayoutEventArgs e)
		{
			base.OnLayout(e);
		}

		internal Size CallGetPreferredSize(Size proposedSize)
		{
			return base.GetPreferredSize(proposedSize);
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			Size sz;
			if (this.loaded)
			{
				sz = this.elementTree.GetPreferredSize(proposedSize);
			}
			else
			{
				Size baseSize = base.GetPreferredSize(proposedSize);
				int width;
				int height;

				if (proposedSize.Width == 0 || proposedSize.Width == Int32.MaxValue)
				{
					width = baseSize.Width;
				}
				else
				{
					width = proposedSize.Width;
				}

				if (proposedSize.Height == 0 || proposedSize.Height == Int32.MaxValue)
				{
					height = baseSize.Height;
				}
				else
				{
					height = proposedSize.Height;
				}

				sz = new Size(width, height);
			}
			return sz;
		}

		internal void CallSetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			base.SetBoundsCore(x, y, width, height, specified);
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			//control is not loaded yet or bounds are the same, visual tree should do nothing
			if (!this.loaded || this.isDisposing)
			{
				base.SetBoundsCore(x, y, width, height, specified);
				return;
			}

			//in the new layout system we do not need updates when setting same bounds
			if(this.UseNewLayoutSystem && this.Bounds == new Rectangle(x, y, width, height))
			{
				base.SetBoundsCore(x, y, width, height, specified);
				return;
			}

			this.isResizing = true;
			this.elementTree.SetBoundsCore(x, y, width, height, specified);
			this.isResizing = false;

			if (this.invalidResizeRect != Rectangle.Empty)
			{
				this.Invalidate(this.invalidResizeRect, false);
				this.invalidResizeRect = Rectangle.Empty;
			}
		}

		protected override void OnAutoSizeChanged(EventArgs e)
		{
			base.OnAutoSizeChanged(e);
			this.elementTree.OnAutoSizeChanged(e);
		} 
		#endregion

		#region Events Management
		
		protected override void OnMouseCaptureChanged(EventArgs e)
		{
			this.Behavior.OnMouseCaptureChanged(e);
		}

		internal void CallOnMouseCaptureChanged(EventArgs e)
		{
			base.OnMouseCaptureChanged(e);
		}

		void IComponentTreeHandler.CallOnMouseCaptureChanged(EventArgs e)
		{
			CallOnMouseCaptureChanged(e);
		}
		

		protected override void OnGotFocus(EventArgs e)
		{
			if (this.Behavior.OnGotFocus(e))
			{
				return;
			}
			base.OnGotFocus(e);
		}

		internal void CallBaseOnGotFocus(EventArgs e)
		{
			this.OnGotFocus(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			if (this.Behavior.OnLostFocus(e))
			{
				return;
			}
			base.OnLostFocus(e);
		}

		internal void CallBaseOnLostFocus(EventArgs e)
		{
			this.OnLostFocus(e);
		}

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

		protected virtual void OnToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
		{
			ToolTipTextNeededEventHandler handler =
				base.Events[ToolTipTextNeededEventKey] as ToolTipTextNeededEventHandler;

			if ((handler != null) && !(base.IsDisposed || base.Disposing))
			{
				handler(sender, e);
			}
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

		void IComponentTreeHandler.CallOnToolTipTextNeeded(object sender, ToolTipTextNeededEventArgs e)
		{
			this.OnToolTipTextNeeded(sender, e);
		}

		void IComponentTreeHandler.CallOnScreenTipNeeded(object sender, ScreenTipNeededEventArgs e)
		{
			this.OnScreenTipNeeded(sender, e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if (this.Behavior.OnMouseUp(e))
			{
				return;
			}
			base.OnMouseUp(e);
		}

		internal void CallOnMouseUp(MouseEventArgs e)
		{       
			this.OnMouseUp(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			if (this.Behavior.OnMouseDown(e))
			{
				return;
			}
			base.OnMouseDown(e);
		}

		internal void CallOnMouseDown(MouseEventArgs e)
		{
			this.OnMouseDown(e);
		}

		protected override void OnClick(EventArgs e)
		{
			if (this.Behavior.OnClick(e))
			{
				return;
			}
			base.OnClick(e);
		}

		internal void CallOnClick(EventArgs e)
		{
			this.OnClick(e);
		}

		protected override void OnDoubleClick(EventArgs e)
		{
			if (this.Behavior.OnDoubleClick(e))
			{
				return;
			}
			base.OnDoubleClick(e);
		}

		internal void CallOnDoubleClick(EventArgs e)
		{
			this.OnDoubleClick(e);
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			if (this.Behavior.OnMouseEnter(e))
			{
				return;
			}
			base.OnMouseEnter(e);
		}

		internal void CallOnMouseEnter(EventArgs e)
		{
			this.OnMouseEnter(e);
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (this.Behavior.OnMouseWheel(e))
			{
				return;
			}
			base.OnMouseWheel(e);
		}

		internal void CallOnMouseWheel(MouseEventArgs e)
		{
			this.OnMouseWheel(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			if (this.Behavior.OnMouseLeave(e))
			{
				return;
			}
			base.OnMouseLeave(e);
		}

		internal void CallOnMouseLeave(EventArgs e)
		{
			this.OnMouseLeave(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if (this.Behavior.OnMouseMove(e))
			{
				return;
			}
			base.OnMouseMove(e);
		}

		internal void CallOnMouseMove(MouseEventArgs e)
		{
			this.OnMouseMove(e);
		}

		protected override void OnMouseHover(EventArgs e)
		{
			if (this.Behavior.OnMouseHover(e))
			{
				return;
			}
			base.OnMouseHover(e);
		}

		internal void CallOnMouseHover(EventArgs e)
		{
			this.OnMouseHover(e);
		}

		protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			if (this.Behavior.OnPreviewKeyDown(e))
			{
				return;
			}
			base.OnPreviewKeyDown(e);
		}

		internal void CallOnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			this.OnPreviewKeyDown(e);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if (this.Behavior.OnKeyDown(e))
			{
				return;
			}
			base.OnKeyDown(e);
		}

		internal void CallBaseOnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
		} 

		internal void CallOnKeyDown(KeyEventArgs e)
		{
			this.OnKeyDown(e);
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (this.Behavior.OnKeyPress(e))
			{
				return;
			}
			base.OnKeyPress(e);
		}

		internal void CallBaseOnKeyPress(KeyPressEventArgs e)
		{
			base.OnKeyPress(e);
		}

		internal void CallOnKeyPress(KeyPressEventArgs e)
		{
			this.OnKeyPress(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			if (this.Behavior.OnKeyUp(e))
			{
				return;
			}
			base.OnKeyUp(e);
		}

		internal void CallBaseOnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
		}

		internal void CallOnKeyUp(KeyEventArgs e)
		{
			this.OnKeyUp(e);
		}

		/// <summary>Fires when the theme name is changed.</summary>
		public event ThemeNameChangedEventHandler ThemeNameChanged;

		internal protected virtual void OnThemeNameChanged(ThemeNameChangedEventArgs e)
		{
			if (ThemeNameChanged != null)
			{
				ThemeNameChanged(this, e);
			}
		}

		internal void CallOnThemeNameChanged(ThemeNameChangedEventArgs e)
		{
			this.OnThemeNameChanged(e);
		}
		#endregion

		#region Design Time
		
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public bool IsDesignMode
		{
			get
			{
				return this.DesignMode;
			}
		}

		/// <summary>
		/// Determines whether the control is currently displayed on the screen.
		/// </summary>
		[Browsable(false)]
		public bool IsDisplayed
		{
			get
			{
				return this.isDisplayed;
			}
		}

		private RadControlDesignTimeData elementData;

		RadControlDesignTimeData IComponentTreeHandler.DesignTimeData
		{
			get
			{
				if (elementData == null)
				{
					elementData = this.CreateDesignTimeData();
				}

				if (elementData == null)
				{
					RadThemeDesignerDataAttribute res = (RadThemeDesignerDataAttribute)TypeDescriptor.GetAttributes(this)[typeof(RadThemeDesignerDataAttribute)];
					if (res != null)
					{
						elementData = Activator.CreateInstance(res.DesignTimeDataType) as RadControlDesignTimeData;
						elementData.ControlName = this.Name;
					}
				}

				if (elementData == null)
				{
					elementData = new RadControlDefaultDesignTimeData(this);
				}

				return elementData;
			}
		}

		protected virtual RadControlDesignTimeData CreateDesignTimeData()
		{
			return null;
		}

		/// <summary>
		/// Method used by control Code Dom serializer to access element in the collection of RootElement.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public RadElement GetChildAt(int index)
		{
			return this.elementTree.RootElement.GetChildAt(index);
		}

		#endregion

		#region Compatibility Code
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
		[Browsable(true), Category("Accessibility")]
		[DefaultValue(false)]
		[Description("Indicates focus cues display, when available, based on the corresponding control type and the current UI state.")]
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
		[Browsable(false)]
		public bool IsThemeClassNameSet
		{
			get 
			{ 
				return this.elementTree.IsThemeClassNameSet; 
			}
		} 
		#endregion

		protected virtual void OnThemeChanged()
		{
		}

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
			else if (property == RadElement.PaddingProperty)
			{
				this.Padding = this.RootElement.Padding;
			}
		}

		void IComponentTreeHandler.ControlThemeChangedCallback()
		{
			this.OnThemeChanged();
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
			if(this.Focused || !this.isFocusable)
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

		protected virtual void InitializeRootElement(RootRadElement rootElement)
		{
			this.controlIsInitializingRootComponent = true;
		}

		protected virtual RootRadElement CreateRootElement()
		{
			return new RootRadElement();
		}

		protected virtual void CreateChildItems(RadElement parent)
		{
			this.RootElement.Name = this.Name;
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

		void IComponentTreeHandler.InitializeRootElement(RootRadElement rootElement)
		{
			this.InitializeRootElement(rootElement);
		}

		RootRadElement IComponentTreeHandler.CreateRootElement()
		{
			return this.CreateRootElement();
		}

		void IComponentTreeHandler.CreateChildItems(RadElement parent)
		{
			this.CreateChildItems(parent);
		}

		void IComponentTreeHandler.CallOnThemeNameChanged(ThemeNameChangedEventArgs e)
		{
			this.CallOnThemeNameChanged(e);
		}

		void IComponentTreeHandler.CallSetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			this.CallSetBoundsCore(x, y, width, height, specified);
		}

		Size IComponentTreeHandler.CallGetPreferredSize(Size proposedSize)
		{
			return this.CallGetPreferredSize(proposedSize);
		}

		void IComponentTreeHandler.CallOnLayout(LayoutEventArgs e) 
		{
			this.CallOnLayout(e);
		}

		bool IComponentTreeHandler.IsDesignMode
		{
			get 
			{
				return this.IsDesignMode; 
			}
		}

		ComponentThemableElementTree IComponentTreeHandler.ElementTree
		{
			get 
			{
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

		void IComponentTreeHandler.OnDisplayPropertyChanged(RadPropertyChangedEventArgs e)
		{
			this.OnDisplayPropertyChanged(e);
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

		/// <summary>
		/// Checks whether the <paramref name="element"/>'s theme is defined by the control.
		/// </summary>
		/// <remarks>
		/// If true is returned the ThemeResolutionService would not not set any theme to the element 
		/// to avoid duplicating the style settings of the element.
		/// </remarks>
		/// <param name="element">The element to should be checked.</param>
		/// <returns>true if the control defines theme for this element, false otherwise.</returns>
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool ControlDefinesThemeForElement(RadElement element)
		{
			return false;
		}

		 #endregion

		private class ComponentOverrideElementTree : ComponentThemableElementTree
		{
			public ComponentOverrideElementTree(RadControl owner)
				: base(owner)
			{
			}

			protected internal override void CreateChildItems(RadElement parent)
			{
				this.ComponentTreeHandler.CreateChildItems(parent);
			}

			protected override RootRadElement CreateRootElement()
			{
				return this.ComponentTreeHandler.CreateRootElement();
			}

			protected override void InitializeRootElement(RootRadElement rootElement)
			{
				if (!(this.Control as RadControl).controlIsInitializingRootComponent)
				{
					this.ComponentTreeHandler.InitializeRootElement(rootElement);
				}
				else
				{
					(this.Control as RadControl).controlIsInitializingRootComponent = false;
				}
			}

			public override bool ControlDefinesThemeForElement(RadElement element)
			{
				return (this.Control as RadControl).ControlDefinesThemeForElement(element); 
			}
		}
	}
}
