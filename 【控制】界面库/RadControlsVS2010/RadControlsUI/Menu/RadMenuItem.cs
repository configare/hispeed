using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Layouts;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Permissions;
using System.ComponentModel.Design;
using Telerik.WinControls.Layout;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents a selectable option displayed on a <see cref="RadMenuElement"/> or
    ///     in a drop down panel.
    /// </summary>
	[ToolboxItem(false), ComVisible(false)]
	[RadNewItem("Add New Item", true)]
	[DefaultEvent("Click")]
	[Designer(DesignerConsts.RadMenuItemDesignerString)]
    public class RadMenuItem : RadMenuItemBase
    {

        // fields
        private Form mdiChildFormToActivate;
		private FillPrimitive fillPrimitive;
		private BorderPrimitive borderPrimitive;
        private RadMenuItemLayout layout;
        private ElementVisibility textSeparatorVisibility = ElementVisibility.Collapsed;
        private int mergeIndex = -1;
		private bool checkOnClick = false;

		private static readonly object ToggleStateChangingEventKey;
		private static readonly object ToggleStateChangedEventKey;

        public readonly static ActivateMenuItemCommand ActivateMenuItemCommand;
             
        static RadMenuItem()
        {			
            ToggleStateChangedEventKey = new object();
            ToggleStateChangingEventKey = new object();

            ActivateMenuItemCommand = new ActivateMenuItemCommand();
            ActivateMenuItemCommand.Name = "ActivateMenuItemCommand";
            ActivateMenuItemCommand.Text = "This command activates the selected menu item.";
            ActivateMenuItemCommand.OwnerType = typeof(RadMenuItem);

        }

        /// <summary>
        /// Initializes a new instance of the RadMenuItem class.
        /// </summary>
		public RadMenuItem(): this("", null)
        {
		}

        /// <summary>
        /// Initializes a new instance of the RadMenuItem class using the displayed
        /// text.
        /// </summary>
        /// <param name="text"></param>
		public RadMenuItem(string text): this(text, null)
		{
		}

        /// <summary>
        /// Initializes a new instance of the RadMenuItem class using the displayed text.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tag"></param>
        public RadMenuItem(string text, object tag)
        {
            this.Text = text;
            this.Tag = tag;
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Class = "RadMenuItem";
        }

        protected override void CreateChildElements()
        {
            this.SetDefaultValueOverride(TextImageRelationProperty,TextImageRelation.ImageBeforeText);
            
            // fill
            this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.Class = "RadMenuItemFillPrimitive";
            this.fillPrimitive.BackColor = Color.Empty;
            this.fillPrimitive.GradientStyle = GradientStyles.Solid;
            this.Children.Add(this.fillPrimitive);

            // border
            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.Class = "RadMenuItemBorderPrimitive";
            this.Children.Add(this.borderPrimitive);

            // layout
            this.layout = new RadMenuItemLayout();
            this.layout.Class = "RadMenuItemLayout";
            this.Children.Add(layout);

            // bindings
            layout.ImagePrimitive.BindProperty(ImagePrimitive.ImageIndexProperty, this, RadButtonItem.ImageIndexProperty, PropertyBindingOptions.TwoWay);
            layout.ImagePrimitive.BindProperty(ImagePrimitive.ImageProperty, this, RadButtonItem.ImageProperty, PropertyBindingOptions.TwoWay);
            layout.ImagePrimitive.BindProperty(ImagePrimitive.ImageKeyProperty, this, RadButtonItem.ImageKeyProperty, PropertyBindingOptions.TwoWay);
            layout.Checkmark.BindProperty(RadCheckmark.CheckStateProperty, this, RadMenuItem.CheckStateProperty, PropertyBindingOptions.OneWay);
            layout.Text.BindProperty(TextPrimitive.TextProperty, this, RadButtonItem.TextProperty, PropertyBindingOptions.OneWay);
            layout.Description.BindProperty(TextPrimitive.FontProperty, this, RadMenuItem.DescriptionFontProperty, PropertyBindingOptions.OneWay);
            layout.Shortcut.BindProperty(TextPrimitive.TextProperty, this, RadMenuItem.HintTextProperty, PropertyBindingOptions.OneWay);
            layout.InternalLayoutPanel.BindProperty(ImageAndTextLayoutPanel.DisplayStyleProperty, this, RadButtonItem.DisplayStyleProperty, PropertyBindingOptions.OneWay);
            layout.InternalLayoutPanel.BindProperty(ImageAndTextLayoutPanel.ImageAlignmentProperty, this, RadButtonItem.ImageAlignmentProperty, PropertyBindingOptions.OneWay);
            layout.InternalLayoutPanel.BindProperty(ImageAndTextLayoutPanel.TextAlignmentProperty, this, RadButtonItem.TextAlignmentProperty, PropertyBindingOptions.OneWay);
            layout.InternalLayoutPanel.BindProperty(ImageAndTextLayoutPanel.TextImageRelationProperty, this, RadButtonItem.TextImageRelationProperty, PropertyBindingOptions.OneWay);
        }

        protected override void OnDropDownCreated()
        {
            base.OnDropDownCreated();

            this.DropDown.Items.ItemsChanged += new ItemChangedDelegate(ItemsChanged);
        }

        protected override void DisposeManagedResources()
        {
            this.mdiChildFormToActivate = null;
            if (this.DropDown != null)
            {
                this.DropDown.Items.ItemsChanged -= new ItemChangedDelegate(ItemsChanged);
            }

            base.DisposeManagedResources();
        }

        #region Events

        /// <summary>
        /// Occurs before the item's toggle state changes.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.ActionCategory),
        Description("Occurs before the elements's state changes."),
        EditorBrowsable(EditorBrowsableState.Advanced)]
        public event StateChangingEventHandler ToggleStateChanging
        {
            add
            {
                this.Events.AddHandler(RadMenuItem.ToggleStateChangingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadMenuItem.ToggleStateChangingEventKey, value);
            }
        }

        /// <summary>
        /// Occurs when the item's toggle state changes.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.ActionCategory),
        Description("Occurs when the elements's state changes."),
        EditorBrowsable(EditorBrowsableState.Advanced)]
        public event StateChangedEventHandler ToggleStateChanged
        {
            add
            {
                this.Events.AddHandler(RadMenuItem.ToggleStateChangedEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadMenuItem.ToggleStateChangedEventKey, value);
            }
        }

        #endregion
        
        #region Properties

        internal Form MdiChildFormToActivate
        {
            get
            {
                return mdiChildFormToActivate;
            }
            set
            {
                mdiChildFormToActivate = value;
            }
        }
         
		public static RadProperty HintTextProperty = RadProperty.Register(
            "HintText", typeof(string), typeof(RadMenuItem), new RadElementPropertyMetadata(
                string.Empty, ElementPropertyOptions.None));

        /// <summary>
        ///		Gets or sets the text that appears as a HintText for a menu item.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the text that appears as a HintText for a menu item.")]
        [DefaultValue("")]
        public string HintText
        {
            get
            {
                return (string)this.GetValue(HintTextProperty);
            }
            set
            {
                this.SetValue(HintTextProperty, value);
            }
        }

        public static RadProperty CheckStateProperty = RadProperty.Register(
            "CheckState", typeof(ToggleState), typeof(RadMenuItem), new RadElementPropertyMetadata(
				ToggleState.Off, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        /// <summary>
        ///     Gets or sets the <see cref="Telerik.WinControls.Enumerations.ToggleState">toggle
        ///     state</see>. Toggle state enumeration defines the following values: Off,
        ///     Indeterminate, and On.
        /// </summary>
        [Browsable(true), Bindable(true),
        Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(ToggleState.Off)]
        public ToggleState ToggleState
        {
            get
            {
                return (ToggleState)base.GetValue(RadMenuItem.CheckStateProperty);
            }
            set
            {
                StateChangingEventArgs eventArgs = new StateChangingEventArgs(this.ToggleState, value, false);
                this.OnToggleStateChanging(eventArgs);
                if (eventArgs.Cancel)
                    return;
                base.SetValue(RadMenuItem.CheckStateProperty, value);
            }
        }
	
        public static RadProperty ShowArrowProperty = RadProperty.Register(
			"ShowArrow", typeof(bool), typeof(RadMenuItem), 
            new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsMeasure));

        /// <summary>
        ///		Gets or sets if the arrow is shown when the menu item contains sub menu.
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets if the arrow is shown when the menu item contains sub menu.")]
        [RadPropertyDefaultValue("ShowArrow", typeof(RadMenuItem))]
        public bool ShowArrow
        {
            get
            {
                return (bool)this.GetValue(RadMenuItem.ShowArrowProperty);
            }
            set
            {
                this.SetValue(RadMenuItem.ShowArrowProperty, value);
            }
        }

		public static RadProperty DescriptionFontProperty = RadProperty.Register(
			"DescriptionFont", typeof(Font), typeof(RadMenuItem), new RadElementPropertyMetadata(
			Control.DefaultFont, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        /// <summary>
        /// Gets or sets the font of the descrition text of the RadMenuItem. 
        /// </summary>
        [Description("DescriptionFont - ex. of the descritpion text of an RadMenuItem. The property is inheritable through the element tree.")]
        [RadPropertyDefaultValue("DescriptionFont", typeof(RadMenuItem)),
        Category(RadDesignCategory.AppearanceCategory)]
        public virtual Font DescriptionFont
        {
            get
            {
                return (Font)this.GetValue(DescriptionFontProperty);
            }
            set
            {
                this.SetValue(DescriptionFontProperty, value);
            }
        }

		public static RadProperty DescriptionTextVisibleProperty = RadProperty.Register(
			"DescriptionTextVisible", typeof(bool), typeof(RadMenuItem), new RadElementPropertyMetadata(
			false, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout));

        /// <summary>
        /// Gets the visibility of description text element
        /// </summary>
        [DefaultValue(false),
        Category(RadDesignCategory.AppearanceCategory),
        Description("Gets the visibility of description text element.")]
        public virtual bool DescriptionTextVisible
        {
            get
            {
                return (bool)GetValue(DescriptionTextVisibleProperty);
            }
        }

        /// <summary>
        ///		Gets or sets the description text associated with this item. 
        /// </summary>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory),
        Description("Gets or sets the description text associated with this item. "),
        Bindable(true),
        SettingsBindable(true),
        Editor(DesignerConsts.MultilineStringEditorString, typeof(UITypeEditor)),
        DefaultValue("")]
        public string DescriptionText
        {
            get
            {
                if ( this.layout != null && this.layout.Description != null)
                {
                    return this.layout.Description.Text;
                }
                return "";
            }
            set
            {
                if (this.Layout != null && this.Layout.Description != null)
                {
                    this.Layout.Description.Text = value;
                    if (value != string.Empty)
                        this.Layout.TextSeparator.Visibility = this.textSeparatorVisibility;
                    else
                        this.Layout.TextSeparator.Visibility = ElementVisibility.Collapsed;
                }
            }
        }

		/// <summary>
		///	Gets or sets a value indicating whether a menu item should toggle its CheckState on mouse click.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[Description("Gets or sets a value indicating whether a menu item should toggle its CheckState on mouse click.")]
		[DefaultValue(false)]
		public bool CheckOnClick
		{
			get
			{
				return this.checkOnClick;
			}
			set
			{
				this.checkOnClick = value;
			}
		}


        /// <summary>
        ///	Gets the FillPrimitive of RadMenuItem responsible for the background appearance.
        /// </summary>
        [Description("Gets the FillPrimitive of RadMenuItem responsible for the background appearance.")]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public FillPrimitive FillPrimitive
        {
            get
            {
                return this.fillPrimitive;
            }
        }

        /// <summary>
        ///	Gets the BorderPrimitive of RadMenuItem responsible for appearance of the border.
        /// </summary>
        [Description("Gets the BorderPrimitive of RadMenuItem responsible for appearance of the border.")]
        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public BorderPrimitive BorderPrimitive
        {
            get
            {
                return this.borderPrimitive;
            }
        }



		/// <summary>Gets or sets a value indicating whether the menu item is checked.</summary>
		[Browsable(true), Bindable(true),
		Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets a value indicating whether a menu item is in the checked, unchecked, or indeterminate state. ")]
		[DefaultValue(false)]
		public virtual bool IsChecked
		{
			get 
			{
				return this.ToggleState != ToggleState.Off;
			}
			set 
			{
				if (value != this.IsChecked)
				{
					this.ToggleState = value ? ToggleState.On : ToggleState.Off;
				}				
			}
		}

		/// <summary>
		///    Gets or sets the index value of the image that is displayed on the item. 
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets the index value of the image that is displayed on the item.")]
		[RelatedImageList("OwnerControl.ImageList")]
		public override int ImageIndex
		{
			get
			{
				return base.ImageIndex;
			}
			set
			{
				base.ImageIndex = value;
			}
		}

		/// <summary>
		///    Gets or sets the key accessor for the image in the ImageList.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[Description("Gets or sets the key accessor for the image in the ImageList.")]
		[RelatedImageList("OwnerControl.ImageList")]
		public override string ImageKey
		{
			get
			{
				return base.ImageKey;
			}
			set
			{
				base.ImageKey = value;
			}
		}
		
		/// <summary>
		///		Gets or sets the position of a merged item within the current menu.
		/// </summary>
		[Browsable(true), Category(RadDesignCategory.LayoutCategory)]
		[Description("Gets or sets the position of a merged item within the current menu.")]
        [DefaultValue(-1)]
        public int MergeIndex
        {
            get
            {
                int num1 = this.mergeIndex;
                if (num1 >=0)
                {
                    return num1;
                }
                return -1;
            }
            set
            {
                this.mergeIndex = value;
            }
        }

		/// <summary>
		/// Gets or sets the visibility of the separator element between the text and the description text
		/// </summary>
		[DefaultValue(ElementVisibility.Collapsed),
		Category(RadDesignCategory.AppearanceCategory),
		Description("Gets or sets the visibility of the separator element between the text and the description text.")]
		public virtual ElementVisibility TextSeparatorVisibility
		{
			get
			{
				return this.textSeparatorVisibility;
			}
			set
			{
				this.textSeparatorVisibility = value;
				if (this.DescriptionText != string.Empty)
				{
					this.Layout.TextSeparator.Visibility = value;
				}
				else
				{
					this.Layout.TextSeparator.Visibility = ElementVisibility.Collapsed;
				}
			}
		}

        private bool IsRootMenuItem
        {
            get
            {
                if (this.ElementTree != null)
                {
                    return this.ElementTree.Control is RadMenu;
                }
                return false;
            }
        }

        public virtual RadElement LeftColumnElement
        {
            get
            {
                if (this.layout == null)
                    return null;

                if (this.Layout.ImagePrimitive.Image != null)
                {
                    return this.Layout.ImagePrimitive;
                }
                return this.Layout.Checkmark;
            }
        }

        public virtual RadElement RightColumnElement
        {
            get
            {
                if (this.layout == null)
                    return null;
                return this.Layout.ArrowPrimitive;
            }
        }

        public virtual RadDropDownMenuLayout MenuLayout
        {
            get
            {
                RadDropDownMenuLayout layout = FindAncestor<RadDropDownMenuLayout>();
                return layout;
            }
        }

        public virtual RadMenuItemLayout Layout
        {
            get
            {
                return this.layout;
            }
        }

		#endregion

        #region Event handlers

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            if (e.Property == RadButtonItem.ImageProperty ||
                e.Property == RadButtonItem.ImageIndexProperty ||
                e.Property == RadButtonItem.ImageKeyProperty)
            {
                if (this.IsRootMenuItem && (this.Image == null && this.ImageIndex == -1 && this.ImageKey == ""))
                {
                    this.Layout.Checkmark.Visibility = ElementVisibility.Collapsed;
                }
                else
                {
                    this.Layout.Checkmark.Visibility = ElementVisibility.Visible;
                }
            }
            else if (e.Property == RadMenuItem.ShowArrowProperty)
            {
                this.UpdateArrow();
            }
            else if (e.Property == RadMenuItem.HintTextProperty)
            {
                if (string.IsNullOrEmpty(e.NewValue as string))
                {
                    this.Layout.Shortcut.Visibility = ElementVisibility.Collapsed;
                }
                else
                {
                    this.Layout.Shortcut.Visibility = ElementVisibility.Visible;
                }
            }

            base.OnPropertyChanged(e);

            if (e.Property == RadMenuItem.CheckStateProperty)
            {
                foreach (RadElement child in this.ChildrenHierarchy)
                {
                    //TODO: This is a hack to support both old and new themes. In the new theming mechanism this is not needed
                    if (child is RadCheckmark)
                    {
                        child.SetValue(RadCheckmark.CheckStateProperty, e.NewValue);
                    }
                    child.SetValue(RadMenuItem.CheckStateProperty, e.NewValue);
                }
                this.OnToggleStateChanged(new StateChangedEventArgs(this.ToggleState));
            }
        }

        private void UpdateArrow()
        {
            if (!this.ShowArrow)
            {
                if (this.IsMainMenuItem)
                {
                    this.Layout.ArrowPrimitive.Visibility = ElementVisibility.Collapsed;
                }
                else
                {
                    this.Layout.ArrowPrimitive.Visibility = ElementVisibility.Hidden;
                }

                return;
            }

            if (this.Items.Count == 0)
            {
                this.Layout.ArrowPrimitive.Visibility = ElementVisibility.Hidden;
                return;
            }

            this.Layout.ArrowPrimitive.Direction = (this.RightToLeft) ? ArrowDirection.Left : ArrowDirection.Right;
            this.Layout.ArrowPrimitive.Visibility = ElementVisibility.Visible;
        }

        protected override void OnBitStateChanged(ulong key, bool oldValue, bool newValue)
        {
            base.OnBitStateChanged(key, oldValue, newValue);

            if (key == IsMainMenuItemStateKey)
            {
                this.SetDefaultValueOverride(ShowArrowProperty, !newValue);
            }
        }

		protected override void OnClick(EventArgs e)
		{
            bool canCheck = this.IsOnDropDown;
            if (canCheck && this.checkOnClick && !this.DesignMode)
            {
                this.IsChecked = !this.IsChecked;
            }
			base.OnClick(e);
        }

        protected virtual void ItemsChanged(RadItemCollection changed, RadItem target, ItemsChangeOperation operation)
        {
            if (operation == ItemsChangeOperation.Inserted || 
                operation == ItemsChangeOperation.Set ||
                operation == ItemsChangeOperation.Removed)
            {
                this.UpdateArrow();
            }
        }

        /// <summary>
        /// Raises the ToggleStateChanging event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnToggleStateChanging(StateChangingEventArgs e)
        {
            StateChangingEventHandler handler1 = (StateChangingEventHandler)base.Events[RadMenuItem.ToggleStateChangingEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Raises the ToggleStateChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnToggleStateChanged(StateChangedEventArgs e)
        {
            StateChangedEventHandler handler1 = (StateChangedEventHandler)base.Events[RadMenuItem.ToggleStateChangedEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        #endregion

        /// <summary>
        /// Determines whether the arrow is currently displayed for this item.
        /// </summary>
        public bool GetArrowVisible()
        {
            if (!this.ShowArrow)
            {
                return false;
            }

            return this.Items.Count > 0;
        }

        protected internal override object CoerceValue(RadPropertyValue propVal, object baseValue)
        {
            if (propVal.Property == DescriptionTextVisibleProperty)
            {
                if (this.Layout != null && this.Layout.Description != null)
                {
                    return this.Layout.Description.Text != string.Empty;
                }
                return false;
            }

            return base.CoerceValue(propVal, baseValue);
        }

        protected virtual ArrowDirection TranslateArrowDirection(RadDirection dropdownDirection) 
        {
            switch (dropdownDirection)
            {
                case RadDirection.Left:
                case RadDirection.Right:
                    return ArrowDirection.Left;

                case RadDirection.Up:
                case RadDirection.Down:
                    return ArrowDirection.Down;

                default:
                    return ArrowDirection.Left;
            }
        }

        //private void RepositionNewMenuItem(RadItemCollection items)
        //{
        //    for (int i = 0; i < items.Count; i++)
        //    {
        //        if (items[i] is RadNewMenuItem)
        //        {
        //            RadNewMenuItem item = (RadNewMenuItem)items[i];
        //            items.RemoveAt(i);
        //            items.Add(item);
        //            break;
        //        }
        //    }
        //}

        internal bool ShowKeyboardCue
        {
            get
            {
                return this.Layout.Text.ShowKeyboardCues;
            }

            set
            {
                this.Layout.Text.ShowKeyboardCues = value;
            }
        }

        #region Shortcuts

        protected override bool CanHandleShortcut(ShortcutEventArgs e)
        {
            if (!this.IsInValidState(true))
            {
                return false;
            }

            Control owner = this.OwnerControl;
            if (owner == null)
            {
                return false;
            }

            Form form = owner.FindForm();
            if (form != null)
            {
                Form activeForm = e.FocusedControl == null ? Form.ActiveForm : e.FocusedControl.FindForm();
                return form == activeForm;
            }

            //no Form, we are on context menu
            return true;
        }

        protected override void UpdateOnShortcutsChanged()
        {
            base.UpdateOnShortcutsChanged();

            string text = string.Empty;
            if (this.Shortcuts.Count > 0)
            {
                this.SetDefaultValueOverride(HintTextProperty, this.Shortcuts.GetDisplayText());
            }
            else
            {
                this.ResetValue(HintTextProperty, ValueResetFlags.DefaultValueOverride);
            }
        }

        #endregion
    }
}

