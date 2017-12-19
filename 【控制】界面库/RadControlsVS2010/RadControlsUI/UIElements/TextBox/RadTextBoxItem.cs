using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.Design;
using Telerik.WinControls.Primitives;
using System.Drawing.Design;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{
	///<exclude/>
	[ToolboxItem(false), ComVisible(false)]
	public class RadTextBoxItem : RadHostItem
    {
        #region BitState Keys

        internal const ulong ShouldTextChangedFireStateKey = RadHostItemLastStateKey << 1;
        internal const ulong TextLockStateKey = ShouldTextChangedFireStateKey << 1;// true when text is set from item to hosted control
        internal const ulong ControlLockStateKey = TextLockStateKey << 1;// true when text is set from hosted control to item
        // true when text is set from item to hosted control in TextChanged event handler 
        // fired from previous text changing action
        internal const ulong TextLock2StateKey = ControlLockStateKey << 1;
        internal const ulong RadTextBoxItemLastStateKey = TextLock2StateKey;

        #endregion

        public static readonly RoutedEvent MultilineEvent = RadElement.RegisterRoutedEvent("MultilineEvent", typeof(RadTextBoxItem));

		public static RadProperty NullTextProperty = RadProperty.Register("NullText", typeof(string), typeof(RadTextBoxItem),
			new RadElementPropertyMetadata(string.Empty, ElementPropertyOptions.None));

        public static RadProperty NullTextColorProperty = RadProperty.Register("NullTextColor", typeof(Color), typeof(RadTextBoxItem),
            new RadElementPropertyMetadata(SystemColors.GrayText, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty IsNullTextProperty = RadProperty.Register("IsNullText", typeof(bool), typeof(RadTextBoxItem),
            new RadElementPropertyMetadata(true, ElementPropertyOptions.None));
		
		private static readonly object TabStopChangedEventKey;
		private static readonly object AcceptsTabChangedEventKey;
		private static readonly object HideSelectionChangedEventKey;
		private static readonly object ModifiedChangedEventKey;
		private static readonly object MultilineChangedEventKey;
		private static readonly object PreviewKeyDownEventKey;
		private static readonly object ReadOnlyChangedEventKey;
		private static readonly object TextAlignChangedEventKey;

		#region Properties

        /// <summary>
		/// Gets or sets whether the control can receives the focus when tab is pressed 
		/// </summary>
		public bool TabStop
		{
			get { return this.TextBoxControl.TabStop; }
			set { this.TextBoxControl.TabStop = value; }
		}

		/// <summary>
		/// Gets or sets whether the text box accepts the return key
		/// </summary>
		[RadDefaultValue("AcceptsReturn", typeof(TextBox))]
		public bool AcceptsReturn
		{
			get { return this.TextBoxControl.AcceptsReturn; }
			set { this.TextBoxControl.AcceptsReturn = value; }
		}

		/// <summary>
		///  Gets or sets whether the text box accepts the tab key
		/// </summary>
		[RadDefaultValue("AcceptsTab", typeof(TextBox))]
		public bool AcceptsTab
		{
			get { return this.TextBoxControl.AcceptsTab; }
			set { this.TextBoxControl.AcceptsTab = value; }
		}


		/// <summary>
		/// Gets if the text box could undo its value
		/// </summary>
		[Browsable(false)]
		public bool CanUndo
		{
			get { return this.TextBoxControl.CanUndo; }
		}

		/// <summary>
		/// Indicates if all charactes should be left alone or converted
		/// to upper or lower case
		/// </summary>
		public CharacterCasing CharacterCasing
		{
			get { return this.TextBoxControl.CharacterCasing; }
			set { this.TextBoxControl.CharacterCasing = value; }
		}

		/// <summary>
		/// Gets or sets the selection in the text box
		/// </summary>
		[RadDefaultValue("HideSelection", typeof(TextBox))]
		public bool HideSelection
		{
			get { return this.TextBoxControl.HideSelection; }
			set { this.TextBoxControl.HideSelection = value; }
		}

		/// <summary>
		/// The lines of the text in a multi-line edit, as an array of string values
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string[] Lines
		{
			get { return this.TextBoxControl.Lines; }
			set { this.TextBoxControl.Lines = value; }
		}

		/// <summary>
		/// Specifies the maximum length of characters which could be entered
		/// </summary>
		[RadDefaultValue("MaxLength", typeof(TextBox))]
		public int MaxLength
		{
			get { return this.TextBoxControl.MaxLength; }
			set { this.TextBoxControl.MaxLength = value; }
		}

		/// <summary>
		/// Indicates the visibility level of the object
		/// </summary>
		public bool Modified
		{
			get { return this.TextBoxControl.Modified; }
			set { this.TextBoxControl.Modified = value; }
		}

		/// <summary>
		/// The text could span more than a line when the value is true
		/// </summary>
		[RadDefaultValue("Multiline", typeof(HostedTextBoxBase))]
		public bool Multiline
		{
			get { return this.TextBoxControl.Multiline; }
			set
			{
				this.TextBoxControl.Multiline = value;
                if (!this.UseNewLayoutSystem)
				    ResetSizeMode(value);
			}
		}

		/// <summary>
		/// Gets or sets the char used for entering passwords
		/// </summary>
		[RadDefaultValue("PasswordChar", typeof(TextBox))]
		public char PasswordChar
		{
			get { return this.TextBoxControl.PasswordChar; }
			set { this.TextBoxControl.PasswordChar = value; }
		}

		/// <summary>
		/// Gets the preferred height
		/// </summary>
		[Browsable(false)]
		public int PreferedHeght
		{
			get { return this.TextBoxControl.PreferredHeight; }
		}

		/// <summary>
		/// Indicates whether the text could be changed or not
		/// </summary>
		[RadDefaultValue("ReadOnly", typeof(TextBox))]
		public bool ReadOnly
		{
			get { return this.TextBoxControl.ReadOnly; }
			set { this.TextBoxControl.ReadOnly = value; }
		}

		/// <summary>
		/// The scrollbars which will appear if the editing control is in multiline mode
		/// </summary>
		public ScrollBars ScrollBars
		{
			get { return this.TextBoxControl.ScrollBars; }
			set { this.TextBoxControl.ScrollBars = value; }
		}

		/// <summary>
		/// the text which is in selection
		/// </summary>
		public string SelectedText
		{
			get { return this.TextBoxControl.SelectedText; }
			set { this.TextBoxControl.SelectedText = value; }
		}

		/// <summary>
		/// the length of the selection
		/// </summary>
		public virtual int SelectionLength
		{
			get { return this.TextBoxControl.SelectionLength; }
			set { this.TextBoxControl.SelectionLength = value; }
		}

		/// <summary>
		/// Gets or sets the start selection position
		/// </summary>
		public virtual int SelectionStart
		{
			get 
            { 
                return this.TextBoxControl.SelectionStart; 
            }
			set 
            { 
                this.TextBoxControl.SelectionStart = value; 
            }
		}

		/// <summary>
		/// Indicates whether the shortcuts are enabled
		/// </summary>
		[RadDefaultValue("ShortcutsEnabled", typeof(TextBox))]
		public bool ShortcutsEnabled
		{
			get { return this.TextBoxControl.ShortcutsEnabled; }
			set { this.TextBoxControl.ShortcutsEnabled = value; }
		}

		/// <summary>
		/// Gets or sets the alignment of the text in the editing control
		/// </summary>
		public HorizontalAlignment TextAlign
		{
			get { return this.TextBoxControl.TextAlign; }
			set { this.TextBoxControl.TextAlign = value; }
		}

		/// <summary>
		/// Indicates the text length
		/// </summary>
		[Browsable(false)]
		public int TextLength
		{
			get { return this.TextBoxControl.TextLength; }
		}

		/// <summary>
		/// Indicates if lines are automatically word-wrapped for
		/// multiline editing controls
		/// </summary>
		[RadDefaultValue("WordWrap", typeof(TextBox))]
		public bool WordWrap
		{
			get { return this.TextBoxControl.WordWrap; }
			set { this.TextBoxControl.WordWrap = value; }
		}

		/// <summary>
		/// Gets or sets the prompt text that is displayed when the TextBox  contains no text
		/// </summary>
		[RadPropertyDefaultValue("NullText", typeof(RadTextBoxItem))]
		public string NullText
		{
			//get { return this.nullText; }
			get { return (string)this.GetValue(NullTextProperty); }
			set
			{
				if (value == null)
					value = string.Empty;
				HostedTextBoxBase hostedTextBox = this.TextBoxControl as HostedTextBoxBase;
				if (hostedTextBox != null)
					hostedTextBox.NullText = value;
				this.SetValue(NullTextProperty, value);
			}
		}

        /// <summary>
        /// Gets or sets the color of prompt text that is displayed when the TextBox  contains no text
        /// </summary>
        [RadPropertyDefaultValue("NullTextColor", typeof(RadTextBoxItem))]
        public Color NullTextColor
        {
            get { return (Color)this.GetValue(NullTextColorProperty); }
            set
            {
                HostedTextBoxBase hostedTextBox = this.TextBoxControl as HostedTextBoxBase;
                
                if (hostedTextBox != null)
                    hostedTextBox.NullTextColor = value;

                this.SetValue(NullTextColorProperty, value);
            }
        }

		#endregion

        internal override bool VsbVisible
        {
            get
            {
                return false;
            }
        }

		/// <summary>
		/// Indicates the text rendering hint
		/// </summary>
		[Browsable(false)]
		public override System.Drawing.Text.TextRenderingHint TextRenderingHint
		{
			get
			{
				return base.TextRenderingHint;
			}
			set
			{
				base.TextRenderingHint = value;
			}
		}

        internal HostedTextBoxBase TextBoxControl
		{
			get
			{
                return (HostedTextBoxBase)this.HostedControl;
			}
		}

		/// <summary>
		/// Gets or sets the vertical stretch value
		/// </summary>
		[DefaultValue(false)]
		public override bool StretchVertically
		{
			get { return base.StretchVertically; }
			set { base.StretchVertically = value; }
		}

		static RadTextBoxItem()
		{
			TabStopChangedEventKey = new object();
			AcceptsTabChangedEventKey = new object();
			HideSelectionChangedEventKey = new object();
			ModifiedChangedEventKey = new object();
			MultilineChangedEventKey = new object();
			PreviewKeyDownEventKey = new object();
			ReadOnlyChangedEventKey = new object();
			TextAlignChangedEventKey = new object();
		}

		/// <summary>
		/// Gets or sets a value indicating whether to show the bottom part of characters, clipped 
		/// due to font name or size particularities
		/// </summary>
		[RadDescription("UseGenericBorderPaint", typeof(HostedTextBoxBase))]
        [RadDefaultValue("UseGenericBorderPaint", typeof(HostedTextBoxBase))]
		public bool UseGenericBorderPaint
		{
			get
			{
				return this.TextBoxControl.UseGenericBorderPaint;
			}
			set
			{
				this.TextBoxControl.UseGenericBorderPaint = value;
				if (value)
					this.TextBoxControl.BorderStyle = BorderStyle.Fixed3D;
				else
					this.TextBoxControl.BorderStyle = BorderStyle.None;
			}
		}

		/// <summary>
		/// Initializes a new instance of 
		/// </summary>
		/// <param name="hostedControl"></param>
		public RadTextBoxItem(Control hostedControl)
			: base(hostedControl)
		{
            HostedTextBoxBase textBox = hostedControl as HostedTextBoxBase;
            Debug.Assert(textBox != null, "Invalid hosted control for RadTextBoxItem.");

            if (this.UseGenericBorderPaint)
				textBox.BorderStyle = BorderStyle.Fixed3D;
			else
                textBox.BorderStyle = BorderStyle.None;

            textBox.Enter += new EventHandler(TextBoxControl_Enter);
            textBox.Leave += new EventHandler(TextBoxControl_Leave);
            textBox.TabStopChanged += new EventHandler(TextBoxControl_TabStopChanged);

            textBox.AcceptsTabChanged += new EventHandler(TextBoxControl_AcceptsTabChanged);
            textBox.HideSelectionChanged += new EventHandler(TextBoxControl_HideSelectionChanged);
            textBox.ModifiedChanged += new EventHandler(TextBoxControl_ModifiedChanged);
            textBox.MultilineChanged += new EventHandler(TextBoxControl_MultilineChanged);
            textBox.ReadOnlyChanged += new EventHandler(TextBoxControl_ReadOnlyChanged);
            textBox.TextAlignChanged += new EventHandler(TextBoxControl_TextAlignChanged);
            textBox.TextChanged += new EventHandler(TextBoxControl_TextChanged);
            textBox.KeyDown += new KeyEventHandler(TextBoxControl_KeyDown);
            textBox.KeyPress += new KeyPressEventHandler(TextBoxControl_KeyPress);
            textBox.KeyUp += new KeyEventHandler(TextBoxControl_KeyUp);
            textBox.PreviewKeyDown += new PreviewKeyDownEventHandler(TextBoxControl_PreviewKeyDown);
		}
		/// <summary>
		/// Initializes a new instance of 
		/// </summary>
		public RadTextBoxItem()
			: this(new HostedTextBoxBase())//TextBox
		{
		}

        protected override void InitializeFields()
        {
            base.InitializeFields();

            // By default Multiline is false and so is StretchVertically
            this.StretchVertically = false;
            this.BitState[ShouldTextChangedFireStateKey] = true;
        }

		internal void SetTextBoxTextSilently(string text)
		{
			this.BitState[ShouldTextChangedFireStateKey] = false;
			this.TextBoxControl.Text = text;
            this.BitState[ShouldTextChangedFireStateKey] = true;
		}

		private void ResetSizeMode(bool multiline)
		{
			if (this.ElementTree != null)
			{
				// Note that our own stretch mode will be set by catching a tunnel event from the root element
				this.ElementTree.RootElement.StretchVertically = multiline;
			}
		}

		#region TextBox methods
		/// <summary>
		/// Appends the given text
		/// </summary>
		/// <param name="text"></param>
		public void AppendText(string text)
		{
			this.TextBoxControl.AppendText(text);
		}


		/// <summary>
		/// Clears the editing control's text
		/// </summary>
		public void Clear()
		{
			this.TextBoxControl.Clear();
		}

		/// <summary>
		/// Clears and undoes the text
		/// </summary>
		public void ClearUndo()
		{
			this.TextBoxControl.ClearUndo();
		}

		/// <summary>
		/// Copies the selected text
		/// </summary>
		public void Copy()
		{
			this.TextBoxControl.Copy();
		}

		/// <summary>
		/// Cuts the selected text
		/// </summary>
		public void Cut()
		{
			this.TextBoxControl.Cut();
		}

		/// <summary>
		/// clears the selection
		/// </summary>
		public void DeselectAll()
		{
			this.TextBoxControl.DeselectAll();
		}

		/// <summary>
		/// Gets a character from a given point
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public char GetCharFromPosition(Point point)
		{
			return this.TextBoxControl.GetCharFromPosition(point);
		}

		/// <summary>
		/// Gets the index of a character at a given point
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		public int GetCharIndexFromPosition(Point point)
		{
			return this.TextBoxControl.GetCharIndexFromPosition(point);
		}

		/// <summary>
		/// gets the index of the first char in a given line
		/// </summary>
		/// <param name="lineNumber"></param>
		/// <returns></returns>
		public int GetFirstCharIndexFromLine(int lineNumber)
		{
			return this.TextBoxControl.GetFirstCharIndexFromLine(lineNumber);
		}

		/// <summary>
		/// gets the first char index at the current line
		/// </summary>
		/// <returns></returns>
		public int GetFirstCharIndexOfCurrentLine()
		{
			return this.TextBoxControl.GetFirstCharIndexOfCurrentLine();
		}

		/// <summary>
		/// Gets a line number from a char index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public int GetLineFromCharIndex(int index)
		{
			return this.TextBoxControl.GetLineFromCharIndex(index);
		}

		/// <summary>
		/// Gets the position from a char index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Point GetPositionFromCharIndex(int index)
		{
			return this.TextBoxControl.GetPositionFromCharIndex(index);
		}

		/// <summary>
		/// pastes the text in the clipboard
		/// </summary>
		public void Paste()
		{
			this.TextBoxControl.Paste();
		}

		/// <summary>
		/// Pasted a given text
		/// </summary>
		/// <param name="text"></param>
		public void Paste(string text)
		{
			this.TextBoxControl.Paste(text);
		}

		/// <summary>
		/// scrolls the textbox to the caret position
		/// </summary>
		public void ScrollToCaret()
		{
			this.TextBoxControl.ScrollToCaret();
		}

		/// <summary>
		/// Makes a selection in a given range specified by a start position and selection length
		/// </summary>
		/// <param name="start"></param>
		/// <param name="length"></param>
		public void Select(int start, int length)
		{
			this.TextBoxControl.Select(start, length);
		}

		/// <summary>
		/// selects the whole text
		/// </summary>
		public void SelectAll()
		{
			this.TextBoxControl.SelectAll();
		}
		#endregion

		#region TextBox events
		protected void TextBoxControl_AcceptsTabChanged(object sender, EventArgs e)
		{
			this.OnAcceptsTabChanged(e);
		}
		protected void TextBoxControl_HideSelectionChanged(object sender, EventArgs e)
		{
			this.OnHideSelectionChanged(e);
		}
		protected void TextBoxControl_ModifiedChanged(object sender, EventArgs e)
		{
			this.OnModifiedChanged(e);
		}
		protected void TextBoxControl_MultilineChanged(object sender, EventArgs e)
		{
            if (this.UseNewLayoutSystem)
            {
                this.StretchVertically = this.TextBoxControl.Multiline;
            }

            this.RaiseBubbleEvent(this, new RoutedEventArgs(e, MultilineEvent));

			this.OnMultilineChanged(e);
		}
		protected void TextBoxControl_ReadOnlyChanged(object sender, EventArgs e)
		{
			this.OnReadOnlyChanged(e);
		}
		protected void TextBoxControl_TextAlignChanged(object sender, EventArgs e)
		{
			this.OnTextAlignChanged(e);
		}

		protected void TextBoxControl_TextChanged(object sender, EventArgs e)
		{
			if (!this.GetBitState(ShouldTextChangedFireStateKey))
				return;

			if (!(this.GetBitState(ControlLockStateKey) || this.GetBitState(TextLockStateKey)))
			{
				this.BitState[ControlLockStateKey] = true;
				base.SetValue(RadItem.TextProperty, this.TextBoxControl.Text);
                this.BitState[ControlLockStateKey] = false;
			}
		}

		private void TextBoxControl_Enter(object sender, EventArgs e)
		{
			this.Focus();
		}

		private void TextBoxControl_Leave(object sender, EventArgs e)
		{
			this.KillFocus();
		}

		private void TextBoxControl_TabStopChanged(object sender, EventArgs e)
		{
			OnTabStopChanged(e);
		}

		private void TextBoxControl_KeyDown(object sender, KeyEventArgs e)
		{
			this.OnKeyDown(e);
		}

		private void TextBoxControl_KeyPress(object sender, KeyPressEventArgs e)
		{
			this.OnKeyPress(e);
            if (e.KeyChar == (char)Keys.Return && !this.AcceptsReturn)
			{
				e.Handled = true;
			}
		}

		private void TextBoxControl_KeyUp(object sender, KeyEventArgs e)
		{
			this.OnKeyUp(e);
		}

		private void TextBoxControl_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
		{
			this.OnPreviewKeyDown(e);
		}
		#endregion

		#region Events

		/// <summary>
		/// Occurs when the TabStop property has changed.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the TabStop property has changed.")]
		public event EventHandler TabStopChanged
		{
			add
			{
				this.Events.AddHandler(RadTextBoxItem.TabStopChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadTextBoxItem.TabStopChangedEventKey, value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnTabStopChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadTextBoxItem.TabStopChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Occurs when the AcceptsTab property has changed.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the AcceptsTab property has changed.")]
		public event EventHandler AcceptsTabChanged
		{
			add
			{
				this.Events.AddHandler(RadTextBoxItem.AcceptsTabChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadTextBoxItem.AcceptsTabChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the AcceptsTabChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnAcceptsTabChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadTextBoxItem.AcceptsTabChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Occurs when the HideSelection property has changed.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the HideSelection property has changed.")]
		public event EventHandler HideSelectionChanged
		{
			add
			{
				this.Events.AddHandler(RadTextBoxItem.HideSelectionChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadTextBoxItem.HideSelectionChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the HideSelectionChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnHideSelectionChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadTextBoxItem.HideSelectionChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Occurs when the Modified property has changed.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the Modified property has changed.")]
		public event EventHandler ModifiedChanged
		{
			add
			{
				this.Events.AddHandler(RadTextBoxItem.ModifiedChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadTextBoxItem.ModifiedChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the ModifiedChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnModifiedChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadTextBoxItem.ModifiedChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Occurs when the Multiline property has changed.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the Multiline property has changed.")]
		public event EventHandler MultilineChanged
		{
			add
			{
				this.Events.AddHandler(RadTextBoxItem.MultilineChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadTextBoxItem.MultilineChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the MultilineChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnMultilineChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadTextBoxItem.MultilineChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Occurs when a key is pressed while focus is on text box.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when a key is pressed while focus is on text box.")]
		public event PreviewKeyDownEventHandler PreviewKeyDown
		{
			add
			{
				this.Events.AddHandler(RadTextBoxItem.PreviewKeyDownEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadTextBoxItem.PreviewKeyDownEventKey, value);
			}
		}

		/// <summary>
		/// Raises the PreviewKeyDown event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
		{
			PreviewKeyDownEventHandler handler1 = (PreviewKeyDownEventHandler)this.Events[RadTextBoxItem.PreviewKeyDownEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Occurs when the ReadOnly property has changed.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the ReadOnly property has changed.")]
		public event EventHandler ReadOnlyChanged
		{
			add
			{
				this.Events.AddHandler(RadTextBoxItem.ReadOnlyChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadTextBoxItem.ReadOnlyChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the ReadOnlyChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnReadOnlyChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadTextBoxItem.ReadOnlyChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// Occurs when the TextAlign property has changed.
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the TextAlign property has changed.")]
		public event EventHandler TextAlignChanged
		{
			add
			{
				this.Events.AddHandler(RadTextBoxItem.TextAlignChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(RadTextBoxItem.TextAlignChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the TextAlignChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnTextAlignChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[RadTextBoxItem.TextAlignChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}
		#endregion

		#region Overrides

		/// <summary>
		///	Gets or sets the text associated with this item. 
		/// </summary>
		public override string Text
		{
			get
			{
				return this.TextBoxControl.Text;
			}
			set
			{				
				this.BitState[TextLock2StateKey] = true;
				base.SetValue(RadItem.TextProperty, value);
                this.BitState[TextLock2StateKey] = false;
			}
		}

		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{	
			if (e.Property == RadItem.TextProperty)
			{
				string value = (string)e.NewValue;
				string currValue = (string)e.OldValue;
                if (value == null)
                {
                    value = string.Empty;
                }

				if (currValue == value)
				{					
					return;
				}

                if (value == string.Empty)
                {
                    this.SetValue(RadTextBoxItem.IsNullTextProperty, true);
                }
                else
                {
                    this.SetValue(RadTextBoxItem.IsNullTextProperty, false);
                }

                if (!(this.GetBitState(ControlLockStateKey) || this.GetBitState(TextLockStateKey)))
				{
					this.BitState[TextLockStateKey] = true;
					this.TextBoxControl.Text = value;
                    this.BitState[TextLockStateKey] = false;
				}
				else if (this.GetBitState(ControlLockStateKey) && this.GetBitState(TextLock2StateKey))
				{
					this.TextBoxControl.Text = value;
				}
			}

			base.OnPropertyChanged(e);

            if (e.Property == RadElement.RightToLeftProperty)
            {       
				//this.RightToLeft = ((bool)e.NewValue);
                this.TextBoxControl.RightToLeft =  this.RightToLeft ? System.Windows.Forms.RightToLeft.Yes : System.Windows.Forms.RightToLeft.No;
                this.NullText = this.NullText;
            }
		}

		/// <summary>
		/// Handles the tunnel event logic of RadTextBoxItem
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args"></param>

        protected override void OnTunnelEvent(RadElement sender, RoutedEventArgs args)
		{
			base.OnTunnelEvent(sender, args);

			if (this.ElementTree == null || typeof(RadTextBox).IsAssignableFrom(this.ElementTree.Control.GetType()))
			{
				if (args.RoutedEvent == RootRadElement.AutoSizeChangedEvent)
				{
					AutoSizeEventArgs eventArgs = (AutoSizeEventArgs)args.OriginalEventArgs;
					if (eventArgs.AutoSize)
						this.AutoSizeMode = RadAutoSizeMode.WrapAroundChildren;
					else
						this.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
				}
				else if (args.RoutedEvent == RootRadElement.StretchChangedEvent)
				{
                    if (!this.UseNewLayoutSystem)
                    {
                        StretchEventArgs eventArgs = (StretchEventArgs)args.OriginalEventArgs;
                        if (eventArgs.IsStretchHorizontal)
                            this.StretchHorizontally = eventArgs.StretchValue;
                        else
                            this.StretchVertically = eventArgs.StretchValue;
                    }
				}
			}
		}

		protected override void OnTextChanging(TextChangingEventArgs e)
		{
			if (!this.GetBitState(ShouldTextChangedFireStateKey))
				return;

			base.OnTextChanging(e);

			if (e.Cancel)
			{
				this.SetTextBoxTextSilently(e.OldValue);
			}
		}
        
		#endregion
	}
}
