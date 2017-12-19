using System;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
//using Telerik.WinControls.Design;
using System.Windows.Forms;
using Telerik.WinControls.Assistance;
using Telerik.WinControls.Design;
using Telerik.WinControls.Elements;
using Telerik.WinControls.Keyboard;
using Telerik.WinControls.Paint;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents the method that will handle the TextChanging event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TextChangingEventHandler(object sender, TextChangingEventArgs e);

    /// <summary>
    ///		Represents the item which could be added to an ItemsCollection and can be selected, deleted or moved during VS design time.
    /// </summary>
    [Designer(DesignerConsts.RadItemDesignerString)]
    [ToolboxItem(false)]
    [DesignerSerializer(DesignerConsts.RadItemCodeDomSerializerString, DesignerConsts.CodeDomSerializerString)]
    [DefaultBindingProperty("Text")]
    [DefaultProperty("Text")]
    [ComVisible(true)]
    public class RadItem : RadComponentElement, ISupportDrag, ISupportDrop, IShortcutProvider
    {
        #region Static Members

        private static readonly object MouseClickEventKey;
        private static readonly object MouseDoubleClickEventKey;
        private static readonly object LostMouseCaptureKey;
        private static readonly object EventQueryAccessibilityHelp;

        private static readonly object EventKeyDown;
        private static readonly object EventKeyPress;
        private static readonly object EventKeyUp;
        private static readonly object EventMouseWheel;

        public static RoutedEvent KeyDownEvent = RadItem.RegisterRoutedEvent("KeyDownEvent", typeof(RadItem));
        public static RoutedEvent KeyPressEvent = RadItem.RegisterRoutedEvent("KeyPressEvent", typeof(RadItem));
        public static RoutedEvent KeyUpEvent = RadItem.RegisterRoutedEvent("KeyUpEvent", typeof(RadItem));
        public static RoutedEvent MouseWheelEvent = RadItem.RegisterRoutedEvent("MouseWheelEvent", typeof(RadItem));

        public readonly static FocusCommand FocusCommand;
        public readonly static ClickCommand ActionCommand;

        #endregion

        #region BitState Keys

        internal const ulong DesignTimeAllowDropStateKey = ComponentElementLastStateKey << 1;
        internal const ulong DesignTimeAllowDragStateKey = DesignTimeAllowDropStateKey << 1;
        internal const ulong IsFocusableStateKey = DesignTimeAllowDragStateKey << 1;
        internal const ulong AutoToolTipStateKey = IsFocusableStateKey << 1;
        internal const ulong ContainsMnemonicStateKey = AutoToolTipStateKey << 1;
        internal const ulong DoubleClickInitiatedStateKey = ContainsMnemonicStateKey << 1;
        internal const ulong AllowDragStateKey = DoubleClickInitiatedStateKey << 1;
        internal const ulong AllowDropStateKey = AllowDragStateKey << 1;

        internal const ulong RadItemLastStateKey = AllowDropStateKey;

        #endregion

        #region Fields

        private RadItemOwnerCollection ownerCollection;
        private RadShortcutCollection shortcuts;
        private bool shortcutFilterAdded;
        private bool captureOnMouseDown;
        private string toolTipText = string.Empty;

        #endregion

        #region Constructors/Initializers

        static RadItem()
        {
            RadItem.MouseClickEventKey = new object();
            RadItem.MouseDoubleClickEventKey = new object();
            RadItem.LostMouseCaptureKey = new object();
            RadItem.EventQueryAccessibilityHelp = new object();

            RadItem.EventKeyDown = new object();
            RadItem.EventKeyPress = new object();
            RadItem.EventKeyUp = new object();
            RadItem.EventMouseWheel = new object();

            FocusCommand = new FocusCommand();
            FocusCommand.Name = "FocusCommand";
            FocusCommand.Text = "This command gives the focus to a selected RadItem instance.";
            FocusCommand.OwnerType = typeof(RadItem);

            ActionCommand = new ClickCommand();
            ActionCommand.Name = "ActionCommand";
            ActionCommand.Text = "This command rises the Click event of a selected RadItem instance.";
            ActionCommand.OwnerType = typeof(RadItem);
        }

        public RadItem()
        {
            this.shortcuts = new RadShortcutCollection(this);
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.ShouldHandleMouseInput = true;
            this.AutoToolTip = this.DefaultAutoToolTip;

            this.InitializeStateManager();
            this.BitState[DesignTimeAllowDropStateKey] = true;
            this.BitState[DesignTimeAllowDragStateKey] = true;
            this.BitState[IsFocusableStateKey] = true;
        }

        #endregion

        #region Events

        /// <summary>
        ///		Occurs when the Text property value is about to be changed.
        /// </summary>
        public event TextChangingEventHandler TextChanging;

        /// <summary>
        ///		Occurs when the Text property value changes.
        /// </summary>
        public event EventHandler TextChanged;

        /// <summary>
        ///		Occurs when the TextOrientation property value changes.
        /// </summary>
        public event EventHandler TextOrientationChanged;

        /// <summary>
        ///		Occurs when the FlipText property value changes.
        /// </summary>
        public event EventHandler FlipTextChanged;

        #endregion

        #region Properties

        public static RadProperty UseDefaultDisabledPaintProperty = RadProperty.Register(
            "UseDefaultDisabledPaint", typeof(bool), typeof(RadItem), new RadElementPropertyMetadata(
                true, ElementPropertyOptions.AffectsDisplay));

        /// <summary>
        ///		Gets or sets whether the item should use the default way for painting the item when disabled (making it gray) or whether
        ///		the disabled appearance should be controlled by the theme.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets whether the item should use the default way for painting the item when disabled (making it gray) or whether the disabled appearance should be controlled by the theme.")]
        [RadPropertyDefaultValue("UseDefaultDisabledPaint", typeof(RadItem))]
        public bool UseDefaultDisabledPaint
        {
            get
            {
                return (bool)GetValue(UseDefaultDisabledPaintProperty);
            }
            set
            {
                SetValue(UseDefaultDisabledPaintProperty, value);
            }
        }

        public static RadProperty ClickModeProperty = RadProperty.Register(
            "ClickMode", typeof(ClickMode), typeof(RadItem), new RadElementPropertyMetadata(
                ClickMode.Release, ElementPropertyOptions.None));

        public static RadProperty TextProperty = RadProperty.Register(
            "Text", typeof(string), typeof(RadItem), new RadElementPropertyMetadata(
                string.Empty,
                ElementPropertyOptions.AffectsMeasure |
                ElementPropertyOptions.AffectsParentMeasure |
                ElementPropertyOptions.AffectsDisplay |
                ElementPropertyOptions.Cancelable));

        public static RadProperty TextOrientationProperty = RadProperty.Register(
            "TextOrientation", typeof(Orientation), typeof(RadItem), new RadElementPropertyMetadata(
                Orientation.Horizontal, ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.CanInheritValue));

        public static RadProperty FlipTextProperty = RadProperty.Register(
            "FlipText", typeof(bool), typeof(RadItem), new RadElementPropertyMetadata(
                false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.CanInheritValue));

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static RadProperty StringAlignmentProperty = RadProperty.Register(
            "StringAlignment", typeof(StringAlignment), typeof(RadItem), new RadElementPropertyMetadata(
                StringAlignment.Near, ElementPropertyOptions.AffectsDisplay | ElementPropertyOptions.CanInheritValue));

        // Indicates whether the item is the "AddNewItem" used in design-time for interactive adding of
        // items to controls. For mor information see RadNewItemAttribute.
        public static RadProperty IsAddNewItemProperty = RadProperty.RegisterAttached(
            "IsAddNewItem", typeof(bool), typeof(RadItem), new RadElementPropertyMetadata(
                BooleanBoxes.FalseBox, ElementPropertyOptions.None));

        /// <summary>
        ///		Specifies the orientation of the text associated with this item. Whether it should appear horizontal or vertical.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        [Description("Specifies the orientation of the text associated with this item. Whether it should appear horizontal or vertical.")]
        [RadPropertyDefaultValue("TextOrientation", typeof(RadItem))]
        public Orientation TextOrientation
        {
            get
            {
                return (Orientation)GetValue(TextOrientationProperty);
            }
            set
            {
                SetValue(TextOrientationProperty, value);
            }
        }

        /// <summary>
        ///		Specifies the text associated with this item will be flipped.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Specifies the text associated with this item will be flipped.")]
        [RadPropertyDefaultValue("FlipText", typeof(RadItem)), Localizable(true)]
        public bool FlipText
        {
            get
            {
                return (bool)GetValue(FlipTextProperty);
            }
            set
            {
                SetValue(FlipTextProperty, value);
            }
        }

        [Browsable(false)]
        [Obsolete("Property is irrelevant. Use TextPrimitive.TextAlignment or LightVisualElement.TextAlignment instead.")]
        public StringAlignment StringAlignment
        {
            get
            {
                return StringAlignment.Near;
            }
            set
            {
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Capture
        {
            get
            {
                if (this.ElementTree != null)
                {
                    bool controlCaptured = this.ElementTree.Control.Capture;
                    bool itemCaptured = Object.ReferenceEquals(
                        this.ElementTree.ComponentTreeHandler.Behavior.ItemCapture, this);
                    return controlCaptured && itemCaptured;
                }
                return false;
            }

            set
            {
                if (this.ElementTree != null)
                {
                    if (value)
                    {
                        this.ElementTree.ComponentTreeHandler.Behavior.ItemCapture = this;
                        this.ElementTree.ComponentTreeHandler.OnCaptureChangeRequested(this, true);
                    }
                    else
                    {
                        this.ElementTree.ComponentTreeHandler.OnCaptureChangeRequested(this, false);
                        this.ElementTree.ComponentTreeHandler.Behavior.ItemCapture = null;
                    }
                }
            }
        }

        protected internal override bool CanHaveOwnStyle
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///		Specifies when the Click event should fire.
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Specifies when the Click event should fire.")]
        [Bindable(true)]
        [SettingsBindable(true)]
        [RadPropertyDefaultValue("ClickMode", typeof(RadItem))]
        public ClickMode ClickMode
        {
            get
            {
                return (ClickMode)base.GetValue(RadItem.ClickModeProperty);
            }
            set
            {
                base.SetValue(RadItem.ClickModeProperty, value);
            }
        }

        /// <summary>
        ///		Gets or sets the text associated with this item. 
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets the text associated with this item.")]
        [Bindable(true)]
        [SettingsBindable(true)]
        [Editor("Telerik.WinControls.UI.TextOrHtmlSelector, Telerik.WinControls.RadMarkupEditor, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e", typeof(UITypeEditor))]
        [RadPropertyDefaultValue("", typeof(RadItem))]
        [Localizable(true)]
        public virtual string Text
        {
            get
            {
                return (string)this.GetValue(RadItem.TextProperty);
            }
            set
            {
                base.SetValue(RadItem.TextProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the item can be selected.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool Selectable
        {
            get
            {
                return true;
            }
        }

        protected internal virtual bool DesignTimeAllowDrop
        {
            get
            {
                return this.GetBitState(DesignTimeAllowDropStateKey);
            }
            set
            {
                this.SetBitState(DesignTimeAllowDropStateKey, value);
            }
        }

        protected internal virtual bool DesignTimeAllowDrag
        {
            get
            {
                return this.GetBitState(DesignTimeAllowDragStateKey);
            }
            set
            {
                this.SetBitState(DesignTimeAllowDragStateKey, value);
            }
        }

        #endregion

        #region ToolTip Management

        [Category(RadDesignCategory.BehaviorCategory),
        Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
        Localizable(true), Description("Gets or sets the tooltip text associated with this item.")]
        [DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public virtual string ToolTipText
        {
            get
            {
                if (!this.AutoToolTip || !string.IsNullOrEmpty(this.toolTipText))
                {
                    return this.toolTipText;
                }
                string text1 = string.Empty;
                //if (!this.DesignMode) 
                {
                    text1 = this.Text;
                    if (WindowsFormsUtils.ContainsMnemonic(text1))
                    {
                        char[] chArray1 = new char[] { '&' };
                        text1 = string.Join("", text1.Split(chArray1));
                    }
                }
                return text1;
            }
            set
            {
                if (value != this.toolTipText)
                {
                    this.toolTipText = value;
                    this.OnNotifyPropertyChanged("ToolTipText");
                }
            }
        }

        [DefaultValue(false),
        Category(RadDesignCategory.BehaviorCategory),
        Description("ToolStripItemAutoToolTipDescr")]
        public virtual bool AutoToolTip
        {
            get
            {
                return this.GetBitState(AutoToolTipStateKey);
            }
            set
            {
                if (this.GetBitState(AutoToolTipStateKey) != value)
                {
                    this.BitState[AutoToolTipStateKey] = value;
                    this.OnNotifyPropertyChanged("AutoToolTip");
                }
            }
        }

        protected virtual bool DefaultAutoToolTip
        {
            get
            {
                return false;
            }
        }

        [DefaultValue(true)]
        public override bool ShouldHandleMouseInput
        {
            get
            {
                return base.ShouldHandleMouseInput;
            }
            set
            {
                base.ShouldHandleMouseInput = value;
            }
        }

        [DefaultValue(null),
        Category(RadDesignCategory.BehaviorCategory),
        Editor(typeof(ScreenTipEditor), typeof(UITypeEditor))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [TypeConverter(typeof(RadFilteredPropertiesConverter))]
        public virtual RadScreenTipElement ScreenTip
        {
            get
            {
                return this.screenTip;
            }
            set
            {
                if (this.screenTip != value)
                {
                    this.screenTip = value;
                    this.autoNumberKeyTip = 0;
                }
            }
        }

        private RadScreenTipElement screenTip;
        private string keyTip = String.Empty;
        private int autoNumberKeyTip = 0;

        internal protected virtual int AutoNumberKeyTip
        {
            get
            {
                return this.autoNumberKeyTip;
            }
            set
            {
                this.autoNumberKeyTip = value;
            }
        }

        [Category(RadDesignCategory.BehaviorCategory),
        Localizable(true), DefaultValue("")]
        public virtual string KeyTip
        {
            get
            {
                return this.keyTip;
            }
            set
            {
                this.keyTip = value.ToUpper();
            }
        }

        protected internal RadItem ParentItem
        {
            get
            {
                RadElement tempElement;
                tempElement = this;
                while (tempElement != null || tempElement.Parent != null || tempElement is RadItem)
                {
                    tempElement = tempElement.Parent;
                }
                if (tempElement != this)
                {
                    return tempElement as RadItem;
                }
                return null;
            }
        }
        #endregion

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual InputBinding CommandBinding
        {
            get
            {
                if (this.ShortcutsHandler != null)
                {
                    InputBindingsCollection collection =
                        this.ShortcutsHandler.Behavior.Shortcuts.InputBindings.GetBindingByComponent(this);
                    //if (collection == null ||
                    //    (collection != null && collection.Count == 0))
                    //{
                    //    //return new InputBinding();
                    //}
                    if (collection != null && collection.Count > 0)
                    {
                        return collection[0];
                    }
                }
                return null;
            }
            set
            {
                if (this.ShortcutsHandler != null)
                {
                    if (value == null || (value != null && value.IsEmpty))
                    {
                        this.ShortcutsHandler.Behavior.Shortcuts.InputBindings.RemoveBindingByComponent(this);
                    }
                    else
                    {
                        if (!this.ShortcutsHandler.Behavior.Shortcuts.InputBindings.Contains(value))
                        {
                            this.ShortcutsHandler.Behavior.Shortcuts.InputBindings.Add(value);
                        }
                    }
                }
            }
        }

        protected virtual IComponentTreeHandler ShortcutsHandler
        {
            get
            {
                if (this.ElementTree == null)
                {
                    return null;
                }
                return this.ElementTree.ComponentTreeHandler;
            }
        }

        protected internal virtual void SetOwnerCollection(RadItemOwnerCollection ownerCollection)
        {
            this.ownerCollection = ownerCollection;
        }

        protected override void DisposeManagedResources()
        {
            //logic to remove the item from its owner collection upon disposing
            if (this.ownerCollection != null)
            {
                RadElement owner = this.ownerCollection.Owner;
                if (owner != null && owner.IsInValidState(false))
                {
                    int index = this.ownerCollection.IndexOf(this);
                    if (index >= 0)
                    {
                        this.ownerCollection.RemoveAt(index);
                    }
                }

                this.ownerCollection = null;
            }

            if (this.shortcutFilterAdded)
            {
                RadShortcutManager.Instance.RemoveShortcutProvider(this);
            }

            if (stateManagerAttachmentData != null)
            {
                stateManagerAttachmentData.Dispose();
            }

            base.DisposeManagedResources();
        }

        public void PerformClick()
        {
            this.DoClick(EventArgs.Empty);
        }

        public bool Select()
        {
            if (this.Selectable)
            {
                this.OnSelect();
                return true;
            }
            return false;
        }

        protected virtual void OnSelect()
        {
            if (!this.IsInValidState(true))
            {
                return;
            }

            IItemsControl itemsControl = this.ElementTree.Control as IItemsControl;

            if (itemsControl != null)
            {
                itemsControl.SelectItem(this);
            }
        }

        public void Deselect()
        {
            if (this.Selectable)
            {
                this.OnDeselect();
            }
        }

        protected virtual void OnDeselect()
        {
            if (!this.IsInValidState(true))
            {
                return;
            }

            IItemsControl itemsControl = this.ElementTree.Control as IItemsControl;

            if (itemsControl != null && object.ReferenceEquals(itemsControl.GetSelectedItem(), this))
            {
                itemsControl.SelectItem(null);
            }
        }

        protected virtual void DoKeyDown(KeyEventArgs e)
        {
            this.OnKeyDown(e);
            if (this.ElementTree != null)
            {
                this.ElementTree.ComponentTreeHandler.Behavior.UpdateScreenTip(null);
            }
        }

        protected virtual void DoKeyPress(KeyPressEventArgs e)
        {
            this.OnKeyPress(e);
        }

        protected virtual void DoKeyUp(KeyEventArgs e)
        {
            this.OnKeyUp(e);
        }

        protected override void DoClick(EventArgs e)
        {
            this.OnClick(e);

            if (this.ElementState == ElementState.Loaded)
            {
                //this.ElementTree.ComponentTreeHandler.Behavior.HideScreenTip();
                //this.ElementTree.ComponentTreeHandler.Behavior.UpdateScreenTip(this);
                this.ElementTree.Control.Invalidate();
            }
        }

        protected override void DoDoubleClick(EventArgs e)
        {
            this.OnDoubleClick(e);
        }

        protected virtual void DoMouseWheel(MouseEventArgs e)
        {
            this.OnMouseWheel(e);
        }

        internal void CallDoMouseWheel(MouseEventArgs e)
        {
            DoMouseWheel(e);
        }

        protected override void DoMouseHover(EventArgs e)
        {
            base.DoMouseHover(e);

            if (this.ElementTree == null)
            {
                return;
            }

            ComponentInputBehavior inputBehavior = this.ElementTree.ComponentTreeHandler.Behavior;
            ScreenTipNeededEventArgs screenTipArgs = inputBehavior.CallOnScreenTipNeeded(this);

            if (this.ScreenTip != null)
            {
                inputBehavior.UpdateScreenTip(screenTipArgs);
                return;
            }

            ToolTipTextNeededEventArgs toolTipTextArgs = inputBehavior.OnToolTipTextNeeded(this);
            string toolTip = toolTipTextArgs.ToolTipText;

            if (this.ToolTipText != toolTip)
            {
                this.ToolTipText = toolTip;
            }

            if (!String.IsNullOrEmpty(this.ToolTipText))
            {
                inputBehavior.UpdateToolTip(this, toolTipTextArgs.Offset);
            }
        }

        protected override void DoMouseLeave(EventArgs e)
        {
            base.DoMouseLeave(e);

            if (this.ElementTree != null)
            {
                this.ElementTree.ComponentTreeHandler.Behavior.UpdateToolTip(null);
                this.ElementTree.ComponentTreeHandler.Behavior.UpdateScreenTip(null);
            }
        }

        /// <summary>
        /// Determines whether mouse will be captured upon MouseDown event.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CaptureOnMouseDown
        {
            get
            {
                return this.captureOnMouseDown;
            }
            set
            {
                this.captureOnMouseDown = true;
            }
        }

        #region Events
        /// <summary>
        ///		Occurs when the element is clicked. 
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.ActionCategory),
        Description("Occurs when the element is clicked.")]
        public event EventHandler Click
        {
            add
            {
                this.Events.AddHandler(RadItem.MouseClickEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadItem.MouseClickEventKey, value);
            }
        }

        /// <summary>
        /// Occurs when the element is double-clicked.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.ActionCategory),
        Description("Occurs when the element is double-clicked.")]
        public event EventHandler DoubleClick
        {
            add
            {
                this.Events.AddHandler(RadItem.MouseDoubleClickEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadItem.MouseDoubleClickEventKey, value);
            }
        }

        /// <summary>
        /// Occurs when the RadItem has focus and the user scrolls up or down the mouse wheel
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.ActionCategory),
        Description("Occurs when the RadItem has focus and the user pressees a key down")]
        public event MouseEventHandler MouseWheel
        {
            add
            {
                this.Events.AddHandler(RadItem.EventMouseWheel, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadItem.EventMouseWheel, value);
            }
        }

        /// <summary>
        /// Raises the MouseWheel event.
        /// </summary>
        /// <param name="e"></param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseWheel(MouseEventArgs e)
        {
            MouseEventHandler handler1 = (MouseEventHandler)base.Events[RadItem.EventMouseWheel];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        public event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp
        {
            add
            {
                this.Events.AddHandler(RadItem.EventQueryAccessibilityHelp, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadItem.EventQueryAccessibilityHelp, value);
            }
        }

        /// <summary>
        /// Occurs when the RadItem has focus and the user pressees a key down
        /// </summary>
        [Category("Key"), Description("Occurs when the RadItem has focus and the user pressees a key down")]
        public event KeyEventHandler KeyDown
        {
            add
            {
                base.Events.AddHandler(RadItem.EventKeyDown, value);
            }
            remove
            {
                base.Events.RemoveHandler(RadItem.EventKeyDown, value);
            }
        }

        /// <summary>
        /// Occurs when the RadItem has focus and the user pressees a key
        /// </summary>
        [Category("Key"), Description("Occurs when the RadItem has focus and the user pressees a key")]
        public event KeyPressEventHandler KeyPress
        {
            add
            {
                base.Events.AddHandler(RadItem.EventKeyPress, value);
            }
            remove
            {
                base.Events.RemoveHandler(RadItem.EventKeyPress, value);
            }
        }

        /// <summary>
        /// Occurs when the RadItem has focus and the user releases the pressed key up
        /// </summary>
        [Category("Key"), Description("Occurs when the RadItem has focus and the user releases the pressed key up")]
        public event KeyEventHandler KeyUp
        {
            add
            {
                base.Events.AddHandler(RadItem.EventKeyUp, value);
            }
            remove
            {
                base.Events.RemoveHandler(RadItem.EventKeyUp, value);
            }
        }

        /// <summary>
        /// Raises the KeyDown event.
        /// </summary>
        /// <param name="e"></param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            KeyEventHandler handler1 = (KeyEventHandler)base.Events[RadItem.EventKeyDown];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Raises the KeyPress event.
        /// </summary>
        /// <param name="e"></param>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnKeyPress(KeyPressEventArgs e)
        {
            KeyPressEventHandler handler1 = (KeyPressEventHandler)base.Events[RadItem.EventKeyPress];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Raises the KeyUp event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            KeyEventHandler handler1 = (KeyEventHandler)base.Events[RadItem.EventKeyUp];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        ///		Raises the Click event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnClick(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)base.Events[RadItem.MouseClickEventKey];

            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        ///		Raises the DoubleClick event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDoubleClick(EventArgs e)
        {
            EventHandler handler1 = (EventHandler)base.Events[RadItem.MouseDoubleClickEventKey];

            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (this.captureOnMouseDown)
            {
                this.Capture = false;
            }

            base.OnMouseUp(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (this.captureOnMouseDown)
            {
                this.Capture = true;
            }

            this.SetFocus();
            base.OnMouseDown(e);
            this.BitState[DoubleClickInitiatedStateKey] = e.Clicks > 1;
        }

        protected internal virtual void SetFocus()
        {
            if (this.GetBitState(IsFocusableStateKey))
            {
                this.Focus();
            }

        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if ((e.Button != MouseButtons.None) &&
                (this.ClickMode == ClickMode.Hover))
            {
                if (e.Clicks > 1)
                {
                    this.OnDoubleClick(e);
                }
                else
                {
                    this.OnClick(e);
                }
            }
        }

        public event MouseEventHandler LostMouseCapture
        {
            add
            {
                this.Events.AddHandler(RadItem.LostMouseCaptureKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadItem.LostMouseCaptureKey, value);
            }
        }

        protected virtual void OnLostMouseCapture(MouseEventArgs e)
        {
            MouseEventHandler handler1 = (MouseEventHandler)base.Events[RadItem.LostMouseCaptureKey];

            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        internal void CallOnLostMouseCapture(MouseEventArgs e)
        {
            this.OnLostMouseCapture(e);
        }

        protected override void OnBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            base.OnBubbleEvent(sender, args);
        }

        protected virtual bool IsItemHovered
        {
            get
            {
                return this.IsMouseOver;
            }
        }

        public override void RaiseBubbleEvent(RadElement sender, RoutedEventArgs args)
        {
            //Store ItemPressed because it would change after routed event...
            bool itemPressed = this.IsItemHovered;

            base.RaiseBubbleEvent(sender, args);

            if (!args.Canceled && sender == this)
            {
                // We rise the Click and DoubleClick routed events
                if (((args.RoutedEvent == RadElement.MouseDownEvent && this.ClickMode == ClickMode.Press) ||
                     (args.RoutedEvent == RadElement.MouseUpEvent &&
                      this.ClickMode == ClickMode.Release &&
                      itemPressed)))
                {
                    MouseEventArgs e = (MouseEventArgs)args.OriginalEventArgs;
                    if ((e.Button != MouseButtons.None))
                    {
                        RoutedEventArgs args1;
                        if (this.GetBitState(DoubleClickInitiatedStateKey))
                        {
                            args1 = new RoutedEventArgs(e, RadElement.MouseDoubleClickedEvent);
                        }
                        else
                        {
                            args1 = new RoutedEventArgs(e, RadElement.MouseClickedEvent);
                        }

                        this.RaiseRoutedEvent(this, args1);
                    }
                }

                if (args.RoutedEvent == RadElement.MouseDoubleClickedEvent)
                {
                    this.DoDoubleClick(args.OriginalEventArgs);
                }

                else if (args.RoutedEvent == RadElement.MouseClickedEvent)
                {
                    this.DoClick(args.OriginalEventArgs);
                }

                else if (args.RoutedEvent == RadItem.KeyDownEvent)
                {
                    KeyEventArgs e = (KeyEventArgs)args.OriginalEventArgs;
                    this.DoKeyDown(e);
                }

                else if (args.RoutedEvent == RadItem.KeyPressEvent)
                {
                    KeyPressEventArgs e = (KeyPressEventArgs)args.OriginalEventArgs;
                    this.DoKeyPress(e);
                }

                else if (args.RoutedEvent == RadItem.KeyUpEvent)
                {
                    KeyEventArgs e = (KeyEventArgs)args.OriginalEventArgs;
                    this.DoKeyUp(e);
                }

                else if (args.RoutedEvent == RadItem.MouseWheelEvent)
                {
                    MouseEventArgs e = (MouseEventArgs)args.OriginalEventArgs;
                    this.DoMouseWheel(e);
                }
            }
        }

        protected override void OnPropertyChanging(RadPropertyChangingEventArgs args)
        {
            base.OnPropertyChanging(args);
            if (args.Property == TextProperty)
            {
                TextChangingEventArgs textArgs = new TextChangingEventArgs((string)args.OldValue, (string)args.NewValue, false);
                this.OnTextChanging(textArgs);

                if (textArgs.Cancel)
                    args.Cancel = true;
            }
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == TextProperty)
            {
                this.InitializeMnemonic(this.Text);
                this.OnTextChanged(EventArgs.Empty);
            }
            else if (e.Property == TextOrientationProperty)
            {
                this.OnTextOrientationChanged(EventArgs.Empty);
            }
            else if (e.Property == FlipTextProperty)
            {
                this.OnFlipTextChanged(EventArgs.Empty);
            }
            else if (e.Property == RadElement.ClassProperty)
            {
                //functinality introduced for backward compatibility with "old" theming system:
                //The default themeRole equal to the firt class name assigned to this item
                if (this.themeRole == null && !string.IsNullOrEmpty(e.NewValue as string))
                {
                    this.ThemeRole = (string)e.NewValue;
                }
            }
        }

        private void InitializeMnemonic(string text)
        {
            this.BitState[ContainsMnemonicStateKey] = TelerikHelper.ContainsMnemonic(text);
        }

        /// <summary>
        ///		Raises the <see cref="TextChanging"/> event.
        /// </summary>
        /// <param name="e">A <see cref="TextChangingEventArgs"/> that contains the event data.</param>
        protected virtual void OnTextChanging(TextChangingEventArgs e)
        {
            if (TextChanging != null)
                TextChanging(this, e);
        }

        /// <summary>
        ///		Raises the <see cref="TextChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnTextChanged(EventArgs e)
        {
            if (TextChanged != null)
                TextChanged(this, e);
        }

        /// <summary>
        ///		Raises the <see cref="TextOrientationChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnTextOrientationChanged(EventArgs e)
        {
            if (TextOrientationChanged != null)
                TextOrientationChanged(this, e);
        }

        /// <summary>
        ///		Raises the <see cref="FlipTextChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected virtual void OnFlipTextChanged(EventArgs e)
        {
            if (FlipTextChanged != null)
                FlipTextChanged(this, e);
        }

        protected override void PaintOverride(IGraphics screenRadGraphics, Rectangle clipRectangle, float angle, SizeF scale, bool useRelativeTransformation)
        {
            //override base to automate disabled item paint

            scale.Width = scale.Width * this.ScaleTransform.Width;
            scale.Height = scale.Height * this.ScaleTransform.Height;


            if (this.Visibility != ElementVisibility.Visible || this.Size.Width <= 0 || this.Size.Height <= 0)
            {
                return;
            }

            if (this.Enabled || !this.IsThisTheTopDisabledItem || !this.UseDefaultDisabledPaint)
            {
                this.Paint(screenRadGraphics, clipRectangle, angle + this.AngleTransform, scale, useRelativeTransformation);
                return;
            }

            using (Brush backgroundBrush = new SolidBrush(Color.Transparent))
            {
                Bitmap controlBitmap = GetAsTransformedBitmap(clipRectangle, backgroundBrush, angle + this.AngleTransform, scale);
                if (controlBitmap != null)
                {
                    Graphics screenGraphics = (Graphics)screenRadGraphics.UnderlayGraphics;

                    Matrix oldPaintMatrix = screenGraphics.Transform;
                    screenGraphics.ResetTransform();

                    Rectangle rect = this.ControlBoundingRectangle;
                    rect.Intersect(clipRectangle);
                    if (rect.Width > 0 && rect.Height > 0)
                    {
                        screenRadGraphics.DrawImage(rect.Location, controlBitmap, false);
                    }

                    controlBitmap.Dispose();
                    screenGraphics.Transform = oldPaintMatrix;
                }
            }
        }

        #endregion

        /// <summary>
        ///		Determines if the item displays any text.
        /// </summary>
        /// <returns></returns>
        protected internal virtual bool ContainsText()
        {
            return true;
        }

        protected internal override bool? ShouldSerializeProperty(PropertyDescriptor property)
        {
            if (property.Name == "ToolTipText")
            {
                string value = (string)property.GetValue(this);
                return !string.IsNullOrEmpty(value);
            }
            else if (property.Name == "AutoToolTip")
            {
                bool value = (bool)property.GetValue(this);
                return value;
            }   

            return null;//Default should serialize
        }

        protected internal virtual bool IsInputKey(Keys keyData)
        {
            return false;
        }

        //[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows), UIPermission(SecurityAction.InheritanceDemand, Window = UIPermissionWindow.AllWindows)]
        protected internal virtual bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData != Keys.Return) && (keyData != Keys.Space))
            {
                return false;
            }
            this.DoClick(EventArgs.Empty);
            /*
            if ((this.ParentInternal != null) && !this.ParentInternal.IsDropDown)
            {
                this.ParentInternal.RestoreFocusInternal();
            }
             */
            return true;
        }

        //[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode), SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected internal virtual bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            return false;
        }

        //[UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows), UIPermission(SecurityAction.InheritanceDemand, Window = UIPermissionWindow.AllWindows)]
        protected internal virtual bool ProcessMnemonic(char charCode)
        {
            this.DoClick(EventArgs.Empty);
            return true;
        }

        protected virtual string MnemonicText
        {
            get
            {
                char ch1 = WindowsFormsUtils.GetMnemonic(this.Text, false);
                if (ch1 != '\0')
                {
                    return ("Alt+" + ch1);
                }
                return null;
            }
        }

        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        EditorBrowsable(EditorBrowsableState.Never)]
        public override bool SerializeProperties
        {
            get
            {
                return base.SerializeProperties;
            }
            set
            {
                base.SerializeProperties = value;
            }
        }

        protected virtual void RaiseClick(EventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(e, RadElement.MouseClickedEvent);
            this.RaiseRoutedEvent(this, args);
        }

        protected virtual void RaiseDoubleClick(EventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(e, RadElement.MouseDoubleClickedEvent);
            this.RaiseRoutedEvent(this, args);
        }

        internal void CallRaiseKeyDown(KeyEventArgs e)
        {
            this.RaiseKeyDown(e);
        }

        protected virtual void RaiseKeyDown(KeyEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(e, RadItem.KeyDownEvent);
            this.RaiseRoutedEvent(this, args);
        }

        internal void CallRaiseKeyPress(KeyPressEventArgs e)
        {
            this.RaiseKeyPress(e);
        }

        protected virtual void RaiseKeyPress(KeyPressEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(e, RadItem.KeyPressEvent);
            this.RaiseRoutedEvent(this, args);
        }

        internal void CallRaiseKeyUp(KeyEventArgs e)
        {
            this.RaiseKeyUp(e);
        }

        protected virtual void RaiseKeyUp(KeyEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(e, RadItem.KeyUpEvent);
            this.RaiseRoutedEvent(this, args);
        }

        internal void CallRaiseMouseWheel(MouseEventArgs e)
        {
            this.RaiseMouseWheel(e);
        }

        protected virtual void RaiseMouseWheel(MouseEventArgs e)
        {
            RoutedEventArgs args = new RoutedEventArgs(e, RadItem.MouseWheelEvent);
            this.RaiseRoutedEvent(this, args);
        }

        #region Theming

        private string themeRole;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ThemeRole
        {
            get
            {
                if (string.IsNullOrEmpty(themeRole))
                {
                    return this.GetThemeEffectiveType().Name;
                }

                return this.themeRole;
            }
            set
            {
                if (this.themeRole != value)
                {
                    this.themeRole = value;

                    //TODO: ThemeRole can be implemented as rad property stateManager can subscribe for changes as attached
                    if (this.stateManager != null)
                    {
                        this.stateManager.ItemStateChanged(this, null);
                    }
                }
            }
        }

        public static RadProperty VisualStateProperty = RadProperty.Register(
            "VisualState",
            typeof(string),
            typeof(RadItem),
            new RadElementPropertyMetadata(string.Empty, ElementPropertyOptions.Cancelable));

        /// <summary>
        /// Gets or sets string representing the current visual state of the Item which is used by themes to determine the appearance of the item and its child elements
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public string VisualState
        {
            get { return this.GetValue(VisualStateProperty) as string; }
            set { this.SetValue(VisualStateProperty, value); }
        }

        private ItemStateManagerBase stateManager;
        private StateManagerAttachmentData stateManagerAttachmentData;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual ItemStateManagerBase StateManager
        {
            get
            {
                InitializeStateManager();
                return this.stateManager;
            }
            set
            {
                if (this.stateManager != null)
                {
                    this.stateManager.Detach(stateManagerAttachmentData);
                }

                this.stateManager = value;

                if (this.stateManager != null)
                {
                    AttachStateManager();
                }
            }
        }

        private void AttachStateManager()
        {
            stateManagerAttachmentData = this.stateManager.AttachToItem(this);
        }

        private void InitializeStateManager()
        {
            if (this.stateManager == null)
            {
                ItemStateManagerFactoryBase factory = ItemStateManagerFactoryRegistry.GetStateManagerFactory(this.ThemeEffectiveType, true);
                string defaultState;
                if (factory != null)
                {
                    //TODO: get the base state string from somewhere
                    this.stateManager = factory.StateManagerInstance;
                    this.stateManagerAttachmentData = stateManager.AttachToItem(this);
                    defaultState = this.stateManager.GetInitialState(this);
                }
                else
                {
                    defaultState = this.ThemeRole;
                }
                this.VisualState = defaultState;
            }
        }

        /// <summary>
        /// Add the VisualState property if we are in the context of RadControlSpy.
        /// </summary>
        /// <param name="props"></param>
        /// <returns></returns>
        internal override PropertyDescriptorCollection ReplaceDefaultDescriptors(PropertyDescriptorCollection props)
        {
            PropertyDescriptorCollection baseProps = base.ReplaceDefaultDescriptors(props);

            if ((bool)this.GetValue(IsEditedInSpyProperty))
            {
                PropertyDescriptor visualStateProperty = TypeDescriptor.CreateProperty(this.GetType(),
                    "VisualState",
                    typeof(string),
                    new Attribute[] { new BrowsableAttribute(true) });
                baseProps.Add(visualStateProperty);
            }

            return baseProps;
        }

        #endregion

        #region ISupportDrag Members

        bool ISupportDrag.CanDrag(Point dragStartPoint)
        {
            if (!this.AllowDrag)
            {
                return false;
            }

            return this.CanDragCore(dragStartPoint);
        }

        /// <summary>
        /// Determines whether the element may be dragged.
        /// </summary>
        /// <param name="dragStartPoint"></param>
        /// <returns></returns>
        protected virtual bool CanDragCore(Point dragStartPoint)
        {
            return this.ElementState == ElementState.Loaded && this.ControlBoundingRectangle.Contains(dragStartPoint);
        }

        object ISupportDrag.GetDataContext()
        {
            return this.GetDragContextCore();
        }

        /// <summary>
        /// Gets the context, associated with a drag operation.
        /// </summary>
        /// <returns></returns>
        protected virtual object GetDragContextCore()
        {
            return null;
        }

        Image ISupportDrag.GetDragHint()
        {
            return this.GetDragHintCore();
        }

        /// <summary>
        /// Gets the image to be used as a hint when this element is being dragged.
        /// </summary>
        /// <returns></returns>
        protected virtual Image GetDragHintCore()
        {
            return this.GetAsBitmapEx(Color.Transparent, 0, new SizeF(1, 1));
        }

        /// <summary>
        /// Determines whether the element may be dragged by a RadDragDropService instance.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowDrag
        {
            get
            {
                return this.BitState[AllowDragStateKey];
            }
            set
            {
                this.SetBitState(AllowDragStateKey, value);
            }
        }

        #endregion

        #region ISupportDrop Members

        void ISupportDrop.DragDrop(Point dropLocation, ISupportDrag dragObject)
        {
            this.ProcessDragDrop(dropLocation, dragObject);
        }

        /// <summary>
        /// Core logic when a drag-drop is performed over this element.
        /// Allows inheritors to provide their own implementations.
        /// </summary>
        /// <param name="dropLocation"></param>
        /// <param name="dragObject"></param>
        protected virtual void ProcessDragDrop(Point dropLocation, ISupportDrag dragObject)
        {
        }

        bool ISupportDrop.DragOver(Point mousePosition, ISupportDrag dragObject)
        {
            return this.ProcessDragOver(mousePosition, dragObject);
        }

        /// <summary>
        /// Determines whether the element may be treated as a drop target during drag-and-drop operation.
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="dragObject"></param>
        /// <returns></returns>
        protected virtual bool ProcessDragOver(Point mousePosition, ISupportDrag dragObject)
        {
            return false;
        }

        void ISupportDrop.DragEnter(Point mousePosition, ISupportDrag dragObject)
        {
            this.ProcessDragEnter(mousePosition, dragObject);
        }

        /// <summary>
        /// Allows the element to perform additional action upon mouse entering its bounds upon a drag-and-drop operation.
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="dragObject"></param>
        protected virtual void ProcessDragEnter(Point mousePosition, ISupportDrag dragObject)
        {
        }

        void ISupportDrop.DragLeave(Point oldMousePosition, ISupportDrag dragObject)
        {
            this.ProcessDragLeave(oldMousePosition, dragObject);
        }

        /// <summary>
        /// Allows the element to perform additional action upon mouse leaving its bounds upon a drag-and-drop operation.
        /// </summary>
        /// <param name="oldMousePosition"></param>
        /// <param name="dragObject"></param>
        protected virtual void ProcessDragLeave(Point oldMousePosition, ISupportDrag dragObject)
        {
        }

        /// <summary>
        /// Determines whether the element may accept a drop operation.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AllowDrop
        {
            get
            {
                return this.BitState[AllowDropStateKey];
            }
            set
            {
                this.SetBitState(AllowDropStateKey, value);
            }
        }

        #endregion

        #region IShortcutProvider Members

        /// <summary>
        /// Gets the collection of all <see cref="RadShortcut">RadShortcut</see> instances registered with this item.
        /// </summary>
        [Browsable(false)]
        public RadShortcutCollection Shortcuts
        {
            get
            {
                return this.shortcuts;
            }
        }

        void IShortcutProvider.OnShortcut(ShortcutEventArgs e)
        {
            if (this.CanHandleShortcut(e))
            {
                this.PerformClick();
                e.Handled = true;
            }
        }

        void IShortcutProvider.OnPartialShortcut(PartialShortcutEventArgs e)
        {
            if (this.CanHandleShortcut(e))
            {
                e.Handled = true;
            }
        }

        void IShortcutProvider.OnShortcutsChanged()
        {
            this.UpdateOnShortcutsChanged();
        }

        protected virtual void UpdateOnShortcutsChanged()
        {
            if (this.ElementState == ElementState.Disposed)
            {
                throw new InvalidOperationException("Changing shortcuts of an already disposed item");
            }

            if (this.shortcuts.Count > 0)
            {
                if (!this.shortcutFilterAdded)
                {
                    RadShortcutManager.Instance.AddShortcutProvider(this);
                    this.shortcutFilterAdded = true;
                }
            }
            else
            {
                if (this.shortcutFilterAdded)
                {
                    RadShortcutManager.Instance.RemoveShortcutProvider(this);
                    this.shortcutFilterAdded = false;
                }
            }
        }

        protected virtual bool CanHandleShortcut(ShortcutEventArgs e)
        {
            return this.IsOnActiveForm(e.FocusedControl, true);
        }

        protected virtual bool IsOnActiveForm(Control focusedControl, bool checkLoaded)
        {
            if (checkLoaded && this.ElementState != ElementState.Loaded)
            {
                return false;
            }

            Form activeForm = focusedControl != null ? focusedControl.FindForm() : Form.ActiveForm;
            return this.ElementTree.Control.FindForm() == activeForm;
        }

        #endregion

        #region Accessibility Properties

        private string accessibleDescription = string.Empty;
        private string accessibleName = string.Empty;
        private AccessibleRole accessibleRole = AccessibleRole.StaticText;

        /// <summary>
        /// Gets or sets the description that will be reported to accessibility client applications.
        /// </summary>
        [Description("Gets or sets the description that will be reported to accessibility client applications."),
            Localizable(true),
            DefaultValue(""),
            Category("Accessibility")]
        public string AccessibleDescription
        {
            get
            {
                return string.IsNullOrEmpty(this.accessibleDescription) ? this.Text : this.accessibleDescription;
            }
            set
            {
                this.accessibleDescription = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the control for use by accessibility client applications.
        /// </summary>
        [DefaultValue(""),
            Category("Accessibility"), Localizable(true),
            Description("Gets or sets the name of the control for use by accessibility client applications.")]
        public string AccessibleName
        {
            get
            {
                return string.IsNullOrEmpty(this.accessibleName) ? this.Text : this.accessibleName;
            }
            set
            {
                this.accessibleName = value;
            }
        }

        /// <summary>
        /// Gets or sets the accessible role of the item, which specifies the type of user interface element 
        /// of the item.
        /// </summary>
        [Description("Gets or sets the accessible role of the item, which specifies the type of user interface element of the item."),
            DefaultValue(AccessibleRole.StaticText),
            Category("Accessibility")]
        public AccessibleRole AccessibleRole
        {
            get
            {
                if (this.accessibleRole != AccessibleRole.Default)
                {
                    return this.accessibleRole;
                }
                return AccessibleRole.StaticText;
            }
            set
            {
                this.accessibleRole = value;
            }
        }

        #endregion
    }
}
