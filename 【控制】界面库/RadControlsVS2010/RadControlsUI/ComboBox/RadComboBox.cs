using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms.VisualStyles;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.Themes.Design;
using System.Drawing.Design;
using Telerik.WinControls.Design;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.UI.Design;
using System.Runtime.InteropServices;
using System.Collections;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents a combo box class. The RadComboBox class is essentially a simple
    ///     wrapper for the <see cref="RadComboBoxElement">RadComboBoxElement</see>. The latter
    ///     may be included in other telerik controls. All UI and logic functionality is
    ///     implemented by the <see cref="RadComboBoxElement">RadComboBoxElement</see> class.
    ///     RadComboBox act to transfer event to and from its
    ///     <see cref="RadComboBoxElement">RadComboBoxElement</see> instance.
    /// </summary>
	//[ComplexBindingProperties("DataSource", "ValueMember")]
	[LookupBindingProperties("DataSource", "DisplayMember", "ValueMember", "SelectedValue")]
	[Description("Displays an editable box with a drop down list of permitted values")]
	[DefaultBindingProperty("Text"), DefaultEvent("SelectedIndexChanged"), DefaultProperty("Items")]
    [ToolboxItem(false)]
    [Obsolete("This control is obsolete. Use RadDropDownList control instead")]
	public class RadComboBox : RadControl//RadItemsControl
    {
        #region Constructors
        static RadComboBox()
        {
			CaseSensitiveChangedEventKey = new object();
			DropDownOpenedEventKey = new object();
            DropDownClosedEventKey = new object();
            DropDownOpeningEventKey = new object();
            DropDownClosingEventKey = new object();
            DropDownStyleChangedEventKey = new object();
            SelectedIndexChangedEventKey = new object();
			SelectedValueChangedEventKey = new object();
			SortedChangedEventKey = new object();            
        }

        ///<summary>
        ///Represents a combo box. The RadComboBox class is a simple wrapper for the
        ///RadComboBoxElement class. The RadComboBox acts to transfer events to and from 
        ///its corresponding RadComboBoxElement instance. The RadComboBoxElement which is 
        ///essentially the RadComboBox control may be nested in other telerik controls.
        ///</summary>
        public RadComboBox()
        {
            this.AutoSize = true;
            this.TabStop = false;
            this.SetStyle(ControlStyles.Selectable, true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.UnwireEvents();
            }
            base.Dispose(disposing);
        }

        #endregion
       
        private RadComboBoxElement comboBoxElement = null;
		private static readonly object CaseSensitiveChangedEventKey;
		private static readonly object DropDownOpenedEventKey;
		private static readonly object DropDownClosedEventKey;
        private static readonly object DropDownOpeningEventKey;
        private static readonly object DropDownClosingEventKey;
		private static readonly object DropDownStyleChangedEventKey;
		private static readonly object SelectedIndexChangedEventKey;
		private static readonly object SelectedValueChangedEventKey;
		private static readonly object SortedChangedEventKey;

        /// <summary>
        /// Enables or disables the ReadOnly mode of RadComboBox. The default value is false.
        /// </summary>
        [Description("Enables or disables the ReadOnly mode of RadComboBox. The default value is false.")]
        [Category("Behavior")]
        [DefaultValue(false)]
        [Obsolete("The DropDownStyle property must be set to DropDownList for RadComboBox to be read only.")] // Marked obsolete for the Q3 2009 release.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool ReadOnly
        {
            get
            {
                return this.comboBoxElement.ReadOnly;
            }
            set
            {
                this.comboBoxElement.ReadOnly = value;
            }
        }                
            
        #region Properties
        
        [DefaultValue(true)]
        public override bool AutoSize
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

        /// <commentsfrom cref="RadComboBoxElement.AutoCompleteMode" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory),
        EditorBrowsable(EditorBrowsableState.Always),
		RadDescription("AutoCompleteMode", typeof(RadComboBoxElement)),
		RadDefaultValue("AutoCompleteMode", typeof(RadComboBoxElement))]
        public AutoCompleteMode AutoCompleteMode
        {
            get 
            {
				return this.comboBoxElement.AutoCompleteMode; 
            }
            set 
            {
				this.comboBoxElement.AutoCompleteMode = value; 
            }
        }

		/// <commentsfrom cref="RadComboBoxElement.CaseSensitive" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("CaseSensitive", typeof(RadComboBoxElement))]
		[RadDefaultValue("CaseSensitive", typeof(RadComboBoxElement))]
        public bool CaseSensitive
        {
            get 
            {
				return this.comboBoxElement.CaseSensitive; 
            }
            set 
            {
				this.comboBoxElement.CaseSensitive = value; 
            }
        }

		/// <commentsfrom cref="RadComboBoxElement.DblClickRotate" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
		[RadDescription("DblClickRotate", typeof(RadComboBoxElement))]
		[RadDefaultValue("DblClickRotate", typeof(RadComboBoxElement))]
        public bool DblClickRotate
        {
            get 
			{
				return this.comboBoxElement.DblClickRotate; 
			}
            set 
			{
				this.comboBoxElement.DblClickRotate = value; 
			}
        }

		/// <commentsfrom cref="RadComboBoxElement.DropDownHeight" filter=""/>
        [Browsable(true), Category(RadDesignCategory.BehaviorCategory),
		RadDescription("DropDownHeight", typeof(RadComboBoxElement)),
		RadDefaultValue("DropDownHeight", typeof(RadComboBoxElement)),
        EditorBrowsable(EditorBrowsableState.Always),
	    RefreshProperties(RefreshProperties.Repaint)]
        public int DropDownHeight
        {
            get 
			{
				return this.comboBoxElement.DropDownHeight;
			}
            set 
			{
				this.comboBoxElement.DropDownHeight = value;
			}
        }

		/// <commentsfrom cref="RadComboBoxElement.DropDownStyle" filter=""/>
		[Category(RadDesignCategory.AppearanceCategory),
		RadDefaultValue("DropDownStyle", typeof(RadComboBoxElement)),
	    RadDescription("DropDownStyle", typeof(RadComboBoxElement)),
        RefreshProperties(RefreshProperties.Repaint)]
        public RadDropDownStyle DropDownStyle
        {
            get 
			{
				return this.comboBoxElement.DropDownStyle;
			}
            set
			{
				this.comboBoxElement.DropDownStyle = value;
			}
        }

		/// <commentsfrom cref="RadComboBoxElement.DropDownWidth" filter=""/>
		[Browsable(true), Category(RadDesignCategory.BehaviorCategory),
		RadDescription("DropDownWidth", typeof(RadComboBoxElement)),
		RadDefaultValue("DropDownWidth", typeof(RadComboBoxElement)),
		EditorBrowsable(EditorBrowsableState.Always)]
        public int DropDownWidth
        {
            get 
			{
				return this.comboBoxElement.DropDownWidth;
			}
            set 
			{
				this.comboBoxElement.DropDownWidth = value;
			}
        }

		/// <commentsfrom cref="RadComboBoxElement.IntegralHeight" filter=""/>
		[Category(RadDesignCategory.BehaviorCategory),
		RadDescription("IntegralHeight", typeof(RadComboBoxElement)),
		RadDefaultValue("IntegralHeight", typeof(RadComboBoxElement))]
		public bool IntegralHeight
		{
			get 
			{
				return this.comboBoxElement.IntegralHeight; 
			}
			set 
			{
				this.comboBoxElement.IntegralHeight = value; 
			}
		}

		/// <commentsfrom cref="RadComboBoxElement.Items" filter=""/>
		[RadEditItemsAction]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(DesignerConsts.RadItemCollectionEditorString, typeof(UITypeEditor)),
		Category(RadDesignCategory.DataCategory)]
		[RadDescription("Items", typeof(RadComboBoxElement))]
		public RadItemCollection Items
		{
			get
			{
				return this.ComboBoxElement.Items;
			}
		}

		/// <commentsfrom cref="RadComboBoxElement.IsDroppedDown" filter=""/>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		RadDescription("IsDroppedDown", typeof(RadComboBoxElement))]
		public bool IsDroppedDown
		{
			get 
            {
                return this.comboBoxElement.IsDroppedDown; 
            }
		}

		/// <commentsfrom cref="RadComboBoxElement.MaxDropDownItems" filter=""/>
		[Category(RadDesignCategory.BehaviorCategory), 
		RadDescription("MaxDropDownItems", typeof(RadComboBoxElement)),
	    RadDefaultValue("MaxDropDownItems", typeof(RadComboBoxElement))]
		public int MaxDropDownItems
		{
			get 
			{
				return this.comboBoxElement.MaxDropDownItems; 
			}
			set 
			{
				this.comboBoxElement.MaxDropDownItems = value; 
			}
		}

        /// <summary>
        /// Gets or sets the maximum number of characters the user can type or paste into the text box control.
        /// </summary>
        [Description("Gets or sets the maximum number of characters the user can type or paste into the text box control."),
        Category(RadDesignCategory.BehaviorCategory),
        Localizable(true),
		RadDefaultValue("MaxLength", typeof(RadComboBoxElement))]
        public int MaxLength
        {
            get 
			{
				return this.comboBoxElement.MaxLength;
			}
            set 
			{
				this.comboBoxElement.MaxLength = value;
			}
        }

        /// <summary>
		/// Gets or sets the text that is displayed when the Text property contains empty string or is null.
        /// </summary>
		[Browsable(true),
		Category(RadDesignCategory.BehaviorCategory),
	    Description("Gets or sets the text that is displayed when the Text property contains empty string or is null."),		
		Bindable(true), 
		RadDefaultValue("NullText", typeof(RadComboBoxElement))]
		[Localizable(true)]
        public string NullText
        {
            get 
			{
				return this.comboBoxElement.NullText;
			}
            set 
			{
				this.comboBoxElement.NullText = value;
			}
        }

		/// <commentsfrom cref="RadComboBoxElement.SelectedItem" filter=""/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        RadDescription("SelectedItem", typeof(RadComboBoxElement)),
        Browsable(false),
        Bindable(true)]
        public Object SelectedItem
        {
            get
            {
                return this.comboBoxElement.SelectedItem;
            }
            set
            {
                this.comboBoxElement.SelectedItem = value;
            }
        }

		/// <commentsfrom cref="RadComboBoxElement.SelectedIndex" filter=""/>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false),
        RadDescription("SelectedIndex", typeof(RadComboBoxElement))]
        public int SelectedIndex
        {
            get
            {
                return this.comboBoxElement.SelectedIndex;
            }
            set
            {
                if (this.SelectedIndex != value)
                {
                    this.comboBoxElement.SelectedIndex = value;                    
                }
            }
        }

        /// <summary>
        /// Gets or sets the text that is selected in the editable portion of a ComboBox.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets or sets the text that is selected in the editable portion of a ComboBox."),
        Browsable(false)]
        public string SelectedText
        {
            get 
			{
				return this.comboBoxElement.SelectedText;
			}
            set 
			{
				this.comboBoxElement.SelectedText = value;
			}
        }
		
        /// <summary>
        /// Gets or sets the number of characters selected in the editable portion of the combo box.
        /// </summary>
        [Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Description("Gets or sets the number of characters selected in the editable portion of the combo box.")]
        public int SelectionLength
        {
            get 
			{
				return this.comboBoxElement.SelectionLength;
			}
            set 
			{
				this.comboBoxElement.SelectionLength = value;
			}
        }

        /// <summary>
        /// Gets or sets the starting index of text selected in the combo box.
        /// </summary>
        [Description("Gets or sets the starting index of text selected in the combo box."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false)]
		public int SelectionStart
		{
			get 
			{
				return this.comboBoxElement.SelectionStart;
			}
			set 
			{
				this.comboBoxElement.SelectionStart = value;
			}
		}

		/// <commentsfrom cref="RadComboBoxElement.Sorted" filter=""/>
        [Category(RadDesignCategory.BehaviorCategory),
        RadDefaultValue("Sorted", typeof(RadComboBoxElement)),
	   RadDescription("Sorted", typeof(RadComboBoxElement))]
		public SortStyle Sorted
		{
			get 
			{
				return this.comboBoxElement.Sorted;
			}
			set 
			{
				this.comboBoxElement.Sorted = value;
			}
		}
		
		/// <summary>Gets or sets displayed text.</summary>
        [Editor(DesignerConsts.MultilineStringEditorString, typeof(UITypeEditor))]
        [DefaultValue("")]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				if (this.comboBoxElement.Text != value)
                {
                    this.comboBoxElement.Text = value;
                }
				base.Text = this.comboBoxElement.Text;
			}
		}

		private bool ShouldSerializeText()
		{
			if (!string.IsNullOrEmpty(this.DisplayMember) && this.DataSource != null)
			{
				if (this.Text == this.DisplayMember)
				{
					return false;
				}
			}
			return true;
		}

		/// <commentsfrom cref="RadComboBoxElement.DropDownAnimationEnabled" filter=""/>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[RadDefaultValue("DropDownAnimationEnabled", typeof(RadComboBoxElement))]
		public bool DropDownAnimationEnabled
		{
			get
			{
				return this.comboBoxElement.DropDownAnimationEnabled;
			}
			set
			{
				this.comboBoxElement.DropDownAnimationEnabled =  value;
			}
		}

		/// <commentsfrom cref="RadComboBoxElement.DropDownAnimationEasing" filter=""/>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[RadDefaultValue("DropDownAnimationEasing", typeof(RadComboBoxElement))]
		public RadEasingType DropDownAnimationEasing
		{
			get
			{
				return this.comboBoxElement.DropDownAnimationEasing;
			}
			set
			{
				this.comboBoxElement.DropDownAnimationEasing = value;
			}
		}

		/// <commentsfrom cref="RadComboBoxElement.DropDownAnimationFrames" filter=""/>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[RadDefaultValue("DropDownAnimationFrames", typeof(RadComboBoxElement))]
		public int DropDownAnimationFrames
		{
			get
			{
				return this.comboBoxElement.DropDownAnimationFrames;
			}
			set
			{
				this.comboBoxElement.DropDownAnimationFrames = value;
			}
		}

		/// <commentsfrom cref="RadComboBoxElement.DropDownSizingMode" filter=""/>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[RadDefaultValue("DropDownSizingMode", typeof(RadComboBoxElement))]
		[Description("Gets or sets the drop down sizing mode. The mode can be: horizontal, veritcal or a combination of them.")]
		public SizingMode DropDownSizingMode
		{
			get 
			{
				return this.comboBoxElement.DropDownSizingMode; 
			}
			set 
			{
                this.comboBoxElement.DropDownSizingMode = value; 
			}
		}


		/// <commentsfrom cref="RadComboBoxElement.DropDownMinSize" filter=""/>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[RadDefaultValue("DropDownMinSize", typeof(RadComboBoxElement))]
		[Description("Gets or sets the drop down minimal size.")]
		public Size DropDownMinSize
		{
			get 
			{
				return this.comboBoxElement.DropDownMinSize; 
			}
			set 
			{
				this.comboBoxElement.DropDownMinSize = value; 
			}
		}

		/// <commentsfrom cref="RadComboBoxElement.DropDownMaxSize" filter=""/>
		[Browsable(true), Category(RadDesignCategory.AppearanceCategory)]
		[RadDefaultValue("DropDownMaxSize", typeof(RadComboBoxElement))]
		[Description("Gets or sets the drop down maximal size.")]
		public Size DropDownMaxSize
		{
			get 
			{
				return this.comboBoxElement.DropDownMaxSize; 
			}
			set 
			{
				this.comboBoxElement.DropDownMaxSize = value; 
			}
		}

        #region DataBind
		/// <commentsfrom cref="RadComboBoxElement.DisplayMember" filter=""/>
		[RadDefaultValue("DisplayMember", typeof(RadComboBoxElement)),
		TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"),
	    RadDescription("DisplayMember", typeof(RadComboBoxElement)),
		Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)),
		Category(RadDesignCategory.DataCategory)]
		public string DisplayMember
		{
			get
			{
				return this.comboBoxElement.DisplayMember;
			}
			set
			{
				this.comboBoxElement.DisplayMember = value;
			}
		}

		/// <commentsfrom cref="RadComboBoxElement.DataSource" filter=""/>
        [RadDefaultValue("DataSource", typeof(RadComboBoxElement)),
        AttributeProvider(typeof(IListSource)),
        RadDescription("DataSource", typeof(RadComboBoxElement)),
        Category(RadDesignCategory.DataCategory),
        RefreshProperties(RefreshProperties.Repaint)]
        public object DataSource
        {
            get
            {
				return this.comboBoxElement.DataSource;
            }
            set
            {
				this.comboBoxElement.DataSource = value;
            }
        }

		public event ItemDataBoundEventHandler ItemDataBound;

		protected virtual void OnItemDataBound(ItemDataBoundEventArgs e)
		{
			if (this.ItemDataBound != null)
			{
				this.ItemDataBound(this, e);
			}
		}

		/// <commentsfrom cref="RadComboBoxElement.FormatInfo" filter=""/>
        [Browsable(false),
        RadDescription("FormatInfo", typeof(RadComboBoxElement)),
        EditorBrowsable(EditorBrowsableState.Advanced),
        RadDefaultValue("FormatInfo", typeof(RadComboBoxElement))]
        public IFormatProvider FormatInfo
        {
            get
            {
				return this.comboBoxElement.FormatInfo;
            }
            set
            {
				this.comboBoxElement.FormatInfo = value;
            }
        }

		/// <commentsfrom cref="RadComboBoxElement.FormatString" filter=""/>
        [RadDefaultValue("FormatString", typeof(RadComboBoxElement)),
	    Editor(typeof(FormatStringEditor), typeof(UITypeEditor)),
        MergableProperty(false),
        RadDescription("FormatString", typeof(RadComboBoxElement))]
        public string FormatString
        {
            get
            {
				return this.comboBoxElement.FormatString;
            }
            set
            {
				this.comboBoxElement.FormatString = value;
            }
        }

		/// <commentsfrom cref="RadComboBoxElement.FormattingEnabled" filter=""/>
		[RadDescription("FormattingEnabled", typeof(RadComboBoxElement)),
        RadDefaultValue("FormattingEnabled", typeof(RadComboBoxElement))]
        public bool FormattingEnabled
        {
            get
            {
				return this.comboBoxElement.FormattingEnabled;
            }
            set
            {
				this.comboBoxElement.FormattingEnabled = value;
            }
        }

		/// <commentsfrom cref="RadComboBoxElement.SelectedValue" filter=""/>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
	    RadDescription("SelectedValue", typeof(RadComboBoxElement)),
		Browsable(false),
		Bindable(true)]
		public Object SelectedValue
		{
			get
			{
				return this.comboBoxElement.SelectedValue;
			}
			set
			{
				this.comboBoxElement.SelectedValue = value;
			}
		}

		/// <commentsfrom cref="RadComboBoxElement.ValueMember" filter=""/>
        [RadDescription("ValueMember", typeof(RadComboBoxElement)),
        Category(RadDesignCategory.DataCategory),
        RadDefaultValue("ValueMember", typeof(RadComboBoxElement)),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public string ValueMember
        {
            get
            {
				return this.comboBoxElement.ValueMember;
            }
            set
            {
				this.comboBoxElement.ValueMember = value;
            }
        }

		/// <commentsfrom cref="RadComboBoxElement.Virtualized" filter=""/>
		[Browsable(true)]
        [RadDefaultValue("Virtualized", typeof(RadComboBoxElement))]
		[Category(RadDesignCategory.BehaviorCategory)]
		[Description("Gets or sets a value indicating whether RadScrollViewer uses UI virtualization.")]
		public bool Virtualized
		{
			get
			{
                return this.comboBoxElement.Virtualized;
			}
			set
			{
                this.comboBoxElement.Virtualized = value;
			}
		}

        /// <summary>Gets the text of the given item.</summary>
        [Obsolete("This method is obsolete and will be removed in the next release.")] // Skarlatov 25/09/2009
        public virtual string GetItemText(object item)
        {
            return this.comboBoxElement.GetItemText(item);
        }

        #endregion

		#endregion

		protected override Size DefaultSize
		{
			get
			{
				return new Size(106, 20);
			}
		}

        /// <summary>
        /// Gets the instance of RadComboBoxElement wrapped by this control. RadComboBoxElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadComboBox.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadComboBoxElement ComboBoxElement
        {
            get
			{
				return this.comboBoxElement;
			}
        }


		private void ListBoxElement_ItemDataBound(object sender, ItemDataBoundEventArgs e)
		{
			this.OnItemDataBound(e);
		}

        #region Public methods
        /// <commentsfrom cref="RadComboBoxElement.BeginUpdate" filter=""/>
        public virtual void BeginUpdate()
        {
            if (this.comboBoxElement != null)
                this.comboBoxElement.BeginUpdate();
        }

        /// <commentsfrom cref="RadComboBoxElement.EndUpdate" filter=""/>
        public virtual void EndUpdate()
        {
            if (this.comboBoxElement != null)
                this.comboBoxElement.EndUpdate();
        }

		/// <commentsfrom cref="RadComboBoxElement.GetItemHeight" filter=""/>
        public int GetItemHeight(int index)
        {
            return this.comboBoxElement.GetItemHeight(index);
        }

		/// <commentsfrom cref="RadComboBoxElement.GetItemText(int)" filter=""/>
        public string GetItemText(int index)
        {
            return this.comboBoxElement.GetItemText(index);
        }

		/// <commentsfrom cref="RadComboBoxElement.Select" filter=""/>
        public void Select(int start, int length)
        {
            this.comboBoxElement.Select(start, length);
        }


		/// <commentsfrom cref="RadComboBoxElement.SelectAll" filter=""/>
        public void SelectAll()
        {
            this.comboBoxElement.SelectAll();
        }

		/// <commentsfrom cref="RadComboBoxElement.FindItem" filter=""/>
		public RadComboBoxItem FindItem(string startsWith)
		{
            return this.comboBoxElement.FindItem(startsWith);
		}

		/// <commentsfrom cref="RadComboBoxElement.FindItemExact" filter=""/>
		public RadComboBoxItem FindItemExact(string text)
		{
            return this.comboBoxElement.FindItemExact(text);
		}

		/// <commentsfrom cref="RadComboBoxElement.ShowDropDown" filter=""/>
		public void ShowDropDown()
		{
            this.comboBoxElement.ShowPopup();
		}

		/// <commentsfrom cref="RadComboBoxElement.CloseDropDown" filter=""/>
		public void CloseDropDown()
		{
            this.comboBoxElement.ClosePopup();
		}

        #endregion

        #region ComboBoxElement events		
		private void comboBoxElement_CaseSensitiveChanged(object sender, EventArgs e)
		{
			this.OnCaseSensitiveChanged(e);
		}

		private void comboBoxElement_DropDownStyleChanged(object sender, EventArgs e)
		{
			this.OnDropDownStyleChanged(e);
		}

        private void comboBoxElement_KeyDown(object sender, KeyEventArgs e)
        {
            base.CallBaseOnKeyDown(e);
            this.Behavior.OnKeyDown(this.Behavior.FocusedElement, e);
        }

        private void comboBoxElement_KeyPress(object sender, KeyPressEventArgs e)
        {
            base.CallBaseOnKeyPress(e);
            this.Behavior.OnKeyPress(this.Behavior.FocusedElement, e);
        }

        private void comboBoxElement_KeyUp(object sender, KeyEventArgs e)
        {
            base.CallBaseOnKeyUp(e);
            this.Behavior.OnKeyUp(this.Behavior.FocusedElement, e);
        }

        private void comboBoxElement_PopupOpened(object sender, EventArgs e)
        {
            this.OnDropDownOpened(e);
        }

        private void comboBoxElement_PopupClosing(object sender, RadPopupClosingEventArgs args)
        {
            this.OnDropDownClosing(args);
        }

        private void comboBoxElement_PopupOpening(object sender, CancelEventArgs e)
        {
            this.OnDropDownOpening(e);
        }

        private void comboBoxElement_PopupClosed(object sender, RadPopupClosedEventArgs e)
        {
            this.OnDropDownClosed(e);
        }
		
		private void comboBoxElement_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.OnSelectedIndexChanged(e);
		}

		private void comboBoxElement_SelectedValueChanged(object sender, EventArgs e)
		{
			this.OnSelectedValueChanged(e);
		}

		private void comboBoxElement_SortedChanged(object sender, EventArgs e)
		{
			this.OnSortedChanged(e);
		}
		
        private void comboBoxElement_TextChanged(object sender, EventArgs e)
        {
            this.Text = this.comboBoxElement.Text;
        }

        #endregion

        #region Events

        #region Focus management
        private bool entering = false;
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            if (!entering)
            {
                entering = true;
                this.comboBoxElement.TextBoxControl.Focus();
                this.OnGotFocus(e);
                entering = false;
            }
        }


        protected override void OnLeave(EventArgs e)
        {
            //GEO - this is temporary fix intended for the autocomplete
            this.comboBoxElement.NotifyOnLeave();
            base.OnLeave(e);
            this.OnLostFocus(e);
        }

        protected override void OnLostFocus(EventArgs e) // should be called from control wrapper
        {
            if (!entering)
            {
                base.OnLostFocus(e);
            }

            if (this.comboBoxElement.IsPopupOpen &&
                !this.comboBoxElement.EditorContainsFocus)
            {
                this.comboBoxElement.ClosePopup(RadPopupCloseReason.CloseCalled);
            }
        }

        #endregion


        protected override void OnValidating(CancelEventArgs e)
        {
            if (this.ComboBoxElement.AutoCompleteEnabled)
            {
                this.comboBoxElement.NotifyAutoComplete();
            }
            base.OnValidating(e);
        }

      

        protected override void OnVisibleChanged(EventArgs e) // should be called from control wrapper
        {
            base.OnVisibleChanged(e);

            if (!this.Disposing)
            {
                this.ClosePopup();
            }
        }

		protected override void OnParentVisibleChanged(EventArgs e) // should be called from control wrapper
		{
			this.ClosePopup();
		    base.OnParentVisibleChanged(e);
		}

		protected override void OnParentChanged(EventArgs e)
		{
			this.ClosePopup();
			base.OnParentChanged(e);
		}

		private void ClosePopup()
		{
            if (this.comboBoxElement == null)
            {
                return;
            }

            if (this.Disposing || this.IsDisposed)
            {
                return;
            }

			if (this.comboBoxElement.IsPopupOpen)
			{
				this.comboBoxElement.ClosePopup(RadPopupCloseReason.CloseCalled);
			}
		}

		/// <summary>
		/// Occurs when the CaseSensitive property has changed.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
	    Description("Occurs when the CaseSensitive property has changed.")]
		public event EventHandler CaseSensitiveChanged
		{
			add
			{
				this.Events.AddHandler(RadComboBox.CaseSensitiveChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadComboBox.CaseSensitiveChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the CaseSensitiveChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnCaseSensitiveChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadComboBox.CaseSensitiveChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

        /// <summary>
        /// Occurs when the RadComboBox' popup is about to be opened.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.BehaviorCategory),
        Description("Occurs when the RadComboBox' popup is about to be opened.")]
        public event RadPopupOpeningEventHandler DropDownOpening
        {
            add
            {
                this.Events.AddHandler(RadComboBox.DropDownOpeningEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadComboBox.DropDownOpeningEventKey, value);
            }
        }

        /// <summary>
        /// Raises the DropDownOpening event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDropDownOpening(CancelEventArgs e)
        {
            RadPopupOpeningEventHandler handler1 = (RadPopupOpeningEventHandler)this.Events[RadComboBox.DropDownOpeningEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Occurs when the RadComboBox' popup is opened.
        /// </summary>
		[Browsable(true),
		Category(RadDesignCategory.BehaviorCategory),
        Description("Occurs when the RadComboBox' popup is opened.")]
		public event EventHandler DropDownOpened
		{
			add
			{
				this.Events.AddHandler(RadComboBox.DropDownOpenedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadComboBox.DropDownOpenedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the DropDownOpened event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDropDownOpened(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadComboBox.DropDownOpenedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

        /// <summary>
        /// Occurs when the RadComboBox' popup is about to be closed.
        /// </summary>
        [Browsable(true),
        Category(RadDesignCategory.BehaviorCategory),
        Description("Occurs when the RadComboBox' popup is about to be closed.")]
        public event RadPopupClosingEventHandler DropDownClosing
        {
            add
            {
                this.Events.AddHandler(RadComboBox.DropDownClosingEventKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(RadComboBox.DropDownClosingEventKey, value);
            }
        }

        /// <summary>
        /// Raises the DropDownClosing event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDropDownClosing(RadPopupClosingEventArgs e)
        {
            RadPopupClosingEventHandler handler1 = (RadPopupClosingEventHandler)this.Events[RadComboBox.DropDownClosingEventKey];
            if (handler1 != null)
            {
                handler1(this, e);
            }
        }

        /// <summary>
        /// Occurs when the drop-down window has closed.
        /// </summary>
		[Browsable(true),
		Category(RadDesignCategory.BehaviorCategory),
		Description("Occurs when the drop-down window has closed.")]
		public event RadPopupClosedEventHandler DropDownClosed
		{
			add
			{
				this.Events.AddHandler(RadComboBox.DropDownClosedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadComboBox.DropDownClosedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the DropDownClosed event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDropDownClosed(RadPopupClosedEventArgs e)
		{
			RadPopupClosedEventHandler handler1 = (RadPopupClosedEventHandler)this.Events[RadComboBox.DropDownClosedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

        /// <summary>
        /// Occurs when the DropDownStyle property has changed.
        /// </summary>
		[Browsable(true),
		Category(RadDesignCategory.PropertyChangedCategory),
		Description("Occurs when the DropDownStyle property has changed.")]
		public event EventHandler DropDownStyleChanged
		{
			add
			{
				this.Events.AddHandler(RadComboBox.DropDownStyleChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadComboBox.DropDownStyleChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the DropDownStyleChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnDropDownStyleChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadComboBox.DropDownStyleChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

        /// <summary>
        /// Occurs when the SelectedIndex property has changed.
        /// </summary>
		[Browsable(true),
		Category(RadDesignCategory.BehaviorCategory),
		Description("Occurs when the SelectedIndex property has changed.")]
		public event EventHandler SelectedIndexChanged
		{
			add
			{
				this.Events.AddHandler(RadComboBox.SelectedIndexChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadComboBox.SelectedIndexChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the SelectedIndexChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnSelectedIndexChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadComboBox.SelectedIndexChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

        /// <summary>Fires when the selected value is changed.</summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the SelectedValue property has changed.")]
		public event EventHandler SelectedValueChanged
		{
			add
			{
				this.Events.AddHandler(RadComboBox.SelectedValueChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadComboBox.SelectedValueChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the SelectedValueChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnSelectedValueChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadComboBox.SelectedValueChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Occurs when the Sorted property has changed.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the Sorted property has changed.")]
		public event EventHandler SortedChanged
		{
			add
			{
				this.Events.AddHandler(RadComboBox.SortedChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadComboBox.SortedChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the SortedChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnSortedChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadComboBox.SortedChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}
    
        #endregion

        /// <summary>
        /// Ends the initialization of a RadComboBox control that is used on a form or used by another component. 
        /// The initialization occurs at run time. 
        /// </summary>
        public override void EndInit()
        {
            base.EndInit();

            this.comboBoxElement.EndInit();
            this.RootElement.PerformLayout(this.RootElement);
        }

        protected override void InitializeRootElement(RootRadElement rootElement)
        {
            base.InitializeRootElement(rootElement);
            rootElement.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            rootElement.StretchVertically = false;
        }

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            Type elementType = element.GetType();

            if (elementType == typeof(RadComboBoxElement))
                return true;
            if (elementType == typeof(RadTextBoxElement))
                return true;

            return false;
        }

        protected override void CreateChildItems(RadElement parent)
        {
            this.comboBoxElement = new RadComboBoxElement();
            this.comboBoxElement.ArrowButton.Arrow.AutoSize = true;
            this.comboBoxElement.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
            this.comboBoxElement.BindProperty(RadElement.BindingContextProperty, this.RootElement, RadElement.BindingContextProperty, PropertyBindingOptions.OneWay);

            this.RootElement.Children.Add(this.comboBoxElement);

            base.CreateChildItems(parent);
            //p.p. 28.07.09 moved after CreateChildItems()
            this.WireEvents();
        }

        protected virtual void WireEvents()
        {
            if (this.comboBoxElement != null)
            {
                this.comboBoxElement.CaseSensitiveChanged += new EventHandler(comboBoxElement_CaseSensitiveChanged);
                this.comboBoxElement.DropDownStyleChanged += new EventHandler(comboBoxElement_DropDownStyleChanged);
                this.comboBoxElement.KeyDown += new KeyEventHandler(comboBoxElement_KeyDown);
                this.comboBoxElement.KeyPress += new KeyPressEventHandler(comboBoxElement_KeyPress);
                this.comboBoxElement.KeyUp += new KeyEventHandler(comboBoxElement_KeyUp);
                this.comboBoxElement.PopupOpened += new EventHandler(comboBoxElement_PopupOpened);
                this.comboBoxElement.PopupOpening += new CancelEventHandler(comboBoxElement_PopupOpening);
                this.comboBoxElement.PopupClosing += new RadPopupClosingEventHandler(comboBoxElement_PopupClosing);
                this.comboBoxElement.PopupClosed += new RadPopupClosedEventHandler(comboBoxElement_PopupClosed);
                this.comboBoxElement.SelectedIndexChanged += new EventHandler(comboBoxElement_SelectedIndexChanged);
                this.comboBoxElement.SelectedValueChanged += new EventHandler(comboBoxElement_SelectedValueChanged);
                this.comboBoxElement.SortedChanged += new EventHandler(comboBoxElement_SortedChanged);
                this.comboBoxElement.TextChanged += new EventHandler(comboBoxElement_TextChanged);
                this.comboBoxElement.ListBoxElement.ItemDataBound += new ItemDataBoundEventHandler(ListBoxElement_ItemDataBound);
            }
        }

        

        protected virtual void UnwireEvents()
        {
            if (this.comboBoxElement != null)
            {
                this.comboBoxElement.CaseSensitiveChanged -= new EventHandler(comboBoxElement_CaseSensitiveChanged);
                this.comboBoxElement.DropDownStyleChanged -= new EventHandler(comboBoxElement_DropDownStyleChanged);
                this.comboBoxElement.KeyDown -= new KeyEventHandler(comboBoxElement_KeyDown);
                this.comboBoxElement.KeyPress -= new KeyPressEventHandler(comboBoxElement_KeyPress);
                this.comboBoxElement.KeyUp -= new KeyEventHandler(comboBoxElement_KeyUp);
                this.comboBoxElement.PopupOpened -= new EventHandler(comboBoxElement_PopupOpened);
                this.comboBoxElement.PopupClosed -= new RadPopupClosedEventHandler(comboBoxElement_PopupClosed);
                this.comboBoxElement.SelectedIndexChanged -= new EventHandler(comboBoxElement_SelectedIndexChanged);
                this.comboBoxElement.SelectedValueChanged -= new EventHandler(comboBoxElement_SelectedValueChanged);
                this.comboBoxElement.SortedChanged -= new EventHandler(comboBoxElement_SortedChanged);
                this.comboBoxElement.TextChanged -= new EventHandler(comboBoxElement_TextChanged);
                this.comboBoxElement.ListBoxElement.ItemDataBound -= new ItemDataBoundEventHandler(ListBoxElement_ItemDataBound);
            }
        }
    }
}
