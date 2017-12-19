using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using Telerik.WinControls.Design;
using System.Reflection;
using System.Drawing.Design;
using System.Collections;

namespace Telerik.WinControls.UI
{
    [DefaultBindingProperty("Text")]
    public abstract class RadTextBoxBase : RadControl
	{
        // Fields
		private static readonly object AcceptsTabChangedEventKey;
		private static readonly object HideSelectionChangedEventKey;
		private static readonly object ModifiedChangedEventKey;
		private static readonly object MultilineChangedEventKey;
		private static readonly object ReadOnlyChangedEventKey;
		private static readonly object TextAlignChangedEventKey;
		private static readonly object TextChangingEventKey;
        private static readonly object PropertyChangedEventKey;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the RadTextBoxBase class.
        /// </summary>
        static RadTextBoxBase()
        {
            AcceptsTabChangedEventKey = new object();
            HideSelectionChangedEventKey = new object();
            ModifiedChangedEventKey = new object();
            MultilineChangedEventKey = new object();
            ReadOnlyChangedEventKey = new object();
            TextAlignChangedEventKey = new object();
            TextChangingEventKey = new object();
            PropertyChangedEventKey = new object();
        }

        /// <summary>
        /// Represents RadTextBoxBase constructor
        /// </summary>
        public RadTextBoxBase()
        {
            this.AutoSize = true;
            this.TabStop = false;
            this.SetStyle(ControlStyles.Selectable, true);
        }


        protected override void Dispose(bool disposing)
        {
            this.UnwireHostEvents();
            base.Dispose(disposing);
        }
        #endregion

		#region Properties

        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", 
            typeof(UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), 
        Localizable(true),
        Description("Gets or sets a custom StringCollection to use when the AutoCompleteSource property is set to CustomSource."),
        Browsable(true),
        RadDefaultValue("AutoCompleteCustomSource", typeof(HostedTextBoxBase)),
        EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get
            {
               return this.TextBoxItem.TextBoxControl.AutoCompleteCustomSource;           
            }
            set
            {
                this.TextBoxItem.TextBoxControl.AutoCompleteCustomSource = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Always), 
        Browsable(true),
        RadDefaultValue("AutoCompleteMode", typeof(HostedTextBoxBase)),
        Description("Gets or sets an option that controls how automatic completion works for the TextBox.")]
        public AutoCompleteMode AutoCompleteMode
        {
            get
            {
                return this.TextBoxItem.TextBoxControl.AutoCompleteMode;
            }
            set
            {
                this.TextBoxItem.TextBoxControl.AutoCompleteMode = value;
            }
        }

        [Browsable(true), TypeConverter(typeof(TextBoxAutoCompleteSourceConverter)),
        EditorBrowsable(EditorBrowsableState.Always),
        Description("Gets or sets a value specifying the source of complete strings used for automatic completion."),
        RadDefaultValue("AutoCompleteSource", typeof(HostedTextBoxBase))]
        public AutoCompleteSource AutoCompleteSource
        {
            get
            {
                return this.TextBoxItem.TextBoxControl.AutoCompleteSource;
            }
            set
            {
                this.TextBoxItem.TextBoxControl.AutoCompleteSource = value;
            }
        }
     
        internal abstract RadTextBoxItem TextBoxItem {get;}

        /// <summary>
		/// Gets or sets a value indicating whether pressing ENTER in a multiline RadTextBox
		/// control creates a new line of text in the control or activates the default button for
		/// the form.
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("AcceptsReturn", typeof(RadTextBoxItem))]
		public bool AcceptsReturn
		{
			get 
            {
                return this.TextBoxItem.AcceptsReturn; 
            }
			set 
            {
                this.TextBoxItem.AcceptsReturn = value; 
            }
		}

		/// <summary>
		/// Gets or sets a value indicating whether pressing the TAB key in a multiline text
		/// box control types a TAB character in the control instead of moving the focus to the
		/// next control in the tab order.
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("AcceptsTab", typeof(RadTextBoxItem))]
		public bool AcceptsTab
		{
			get 
            {
                return this.TextBoxItem.AcceptsTab; 
            }
			set 
            {
                this.TextBoxItem.AcceptsTab = value; 
            }
		}

		/// <summary>
		/// Gets or sets whether the edit control is auto-sized
		/// </summary>
		[DefaultValue(true)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
		[Category(RadDesignCategory.LayoutCategory)]
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

		protected override Size DefaultSize
		{
			get
			{
				return new Size(100, 20);
			}
		}

		/// <summary>Gets value indicating whether undo is allowed.</summary>
		[Category(RadDesignCategory.BehaviorCategory)]
		public bool CanUndo
		{
			get 
            {
                return this.TextBoxItem.CanUndo; 
            }
		}

		/// <summary>
		/// 	<para>Gets or sets a value indicating whether the RadTextBox control modifies the
		///     case of characters as they are typed.</para>
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(System.Windows.Forms.CharacterCasing.Normal)]
		public CharacterCasing CharacterCasing
		{
			get 
            {
                return this.TextBoxItem.CharacterCasing; 
            }
			set 
            {
                this.TextBoxItem.CharacterCasing = value; 
            }
		}

		/// <summary>
		/// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl03_LabelAbstract">Gets or sets
		/// the font of the text displayed by the control.</span>
		/// </summary>
		[Category(RadDesignCategory.AppearanceCategory)]
		public override System.Drawing.Font Font
		{
			get
			{
				return base.Font;
			}
			set
			{
                this.TextBoxItem.Font = value;
				base.Font = value;
			}
		}

		/// <summary>
		/// 	<para>Gets or sets a value indicating whether the selected text remains highlighted
		///     even when the RadTextBox has lost the focus.</para>
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(true)]
		public bool HideSelection
		{
			get 
            {
                return this.TextBoxItem.HideSelection; 
            }
			set 
            {
                this.TextBoxItem.HideSelection = value; 
            }
		}

		/// <summary>
		/// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
		/// the lines of text in multiline configurations.</span>
		/// </summary>
		[Category(RadDesignCategory.AppearanceCategory)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string[] Lines
		{
			get 
            {
                return this.TextBoxItem.Lines; 
            }
			set 
            {
                this.TextBoxItem.Lines = value; 
            }
		}

		/// <summary>
		/// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
		/// the maximum number of characters allowed in the text box.</span>
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(32767)]
		public int MaxLength
		{
			get 
            {
                return this.TextBoxItem.MaxLength; 
            }
			set 
            {
                this.TextBoxItem.MaxLength = value; 
            }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the RadTextBox control has been modified
		/// by the user since the control was created or since its contents were last set.
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(false)]
		public bool Modified
		{
			get 
            {
                return this.TextBoxItem.Modified; 
            }
			set 
            {
                this.TextBoxItem.Modified = value; 
            }
		}

		/// <summary>
		/// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
		/// a value indicating whether this is a multiline TextBox control.</span>
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("Multiline", typeof(RadTextBoxItem))]
		public bool Multiline
		{
			get
			{
                return this.TextBoxItem.Multiline;
			}
			set
			{
                // Allow save of the size when switching Multiline
                bool oldAutoSize = this.AutoSize;
                if (oldAutoSize)
                {
                    this.AutoSize = false;
                }
                this.TextBoxItem.Multiline = value;
                if (oldAutoSize)
                {
                    this.AutoSize = true;
                }
			}
		}

		/// <summary>
		/// Gets or sets the text that is displayed when the ComboBox contains a null 
		/// reference. 
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
		[Localizable(true)]
        [RadDefaultValue("NullText", typeof(RadTextBoxItem))]
		public string NullText
		{
			get 
            {
                return this.TextBoxItem.NullText; 
            }
			set 
            {
                this.TextBoxItem.NullText = value; 
            }
		}

		/// <summary>
		/// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
		/// the character used to mask characters of a password in a single-line TextBox
		/// control.</span>
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
        [RadDefaultValue("PasswordChar", typeof(RadTextBoxItem))]
		public char PasswordChar
		{
			get 
            {
                return this.TextBoxItem.PasswordChar; 
            }
			set 
            {
                this.TextBoxItem.PasswordChar = value; 
            }
		}

		/// <summary>
		/// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
		/// a value indicating whether the contents of the TextBox control can be
		/// changed.</span>
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(false)]
		public bool ReadOnly
		{
			get 
            {
                return this.TextBoxItem.ReadOnly; 
            }
			set 
            {
                this.TextBoxItem.ReadOnly = value; 
            }
		}

		/// <summary>
		/// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
		/// which scroll bars should appear in a multiline TextBox control.</span>
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(System.Windows.Forms.ScrollBars.None)]
		public ScrollBars ScrollBars
		{
			get 
            {
                return this.TextBoxItem.ScrollBars; 
            }
			set 
            {
                this.TextBoxItem.ScrollBars = value; 
            }
		}

		/// <summary>
		/// Gets or sets a value indicating the currently selected text in the
		/// control.
		/// </summary>
		[Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue("")]
		public string SelectedText
		{
			get 
            {
                return this.TextBoxItem.SelectedText; 
            }
			set 
            {
                this.TextBoxItem.SelectedText = value; 
            }
		}

		/// <summary>
		/// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
		/// the number of characters selected in the text box.</span>
		/// </summary>
        [DefaultValue(0)]
        public int SelectionLength
		{
			get 
            {
                return this.TextBoxItem.SelectionLength; 
            }
			set 
            {
                this.TextBoxItem.SelectionLength = value; 
            }
		}

		/// <summary>
		/// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
		/// the starting point of text selected in the text box.</span>
		/// </summary>
        [DefaultValue(0)]
        public int SelectionStart
		{
			get 
            {
                return this.TextBoxItem.SelectionStart; 
            }
			set 
            {
                this.TextBoxItem.SelectionStart = value; 
            }
		}

		/// <summary>
		/// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Gets or sets
		/// a value indicating whether the defined shortcuts are enabled.</span>
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(true)]
		public bool ShortcutsEnabled
		{
			get 
            {
                return this.TextBoxItem.ShortcutsEnabled; 
            }
			set 
            {
                this.TextBoxItem.ShortcutsEnabled = value; 
            }
		}

		/// <summary>Gets or sets the displayed text.</summary>
		[Category(RadDesignCategory.AppearanceCategory)]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
		public override string Text
		{
			get
			{
                return this.TextBoxItem.Text;
			}
			set
			{
                if (this.TextBoxItem.Text != value)
                {
                    this.TextBoxItem.Text = value;
                }
			}
		}

		/// <summary>Gets or sets how text is aligned in a TextBox control.</summary>
		[Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(System.Windows.Forms.HorizontalAlignment.Left)]
		public virtual HorizontalAlignment TextAlign
		{
			get 
            {
                return this.TextBoxItem.TextAlign; 
            }
			set 
            {
                this.TextBoxItem.TextAlign = value; 
            }
		}

		/// <summary>Gets the length of the text in the control.</summary>
		[Category(RadDesignCategory.AppearanceCategory)]
		public int TextLength
		{
			get 
            {
                return this.TextBoxItem.TextLength; 
            }
		}

		/// <summary>
		/// 	<para>Gets or sets a value indicating whether a multiline text box control
		///     automatically wraps words to the beginning of the next line when necessary.</para>
		/// </summary>
		[Category(RadDesignCategory.BehaviorCategory)]
        [DefaultValue(true)]
		public bool WordWrap
		{
			get 
            {
                return this.TextBoxItem.WordWrap; 
            }
			set 
            {
                this.TextBoxItem.WordWrap = value; 
            }
		}

        #endregion

        /// <summary>
		/// Initializes textbox's children
		/// </summary>
        protected abstract internal void InitializeTextElement();

        protected virtual internal void WireHostEvents()
        {
            this.TextBoxItem.AcceptsTabChanged += new EventHandler(textBoxItem_AcceptsTabChanged);
            this.TextBoxItem.HideSelectionChanged += new EventHandler(textBoxItem_HideSelectionChanged);
            this.TextBoxItem.ModifiedChanged += new EventHandler(textBoxItem_ModifiedChanged);
            this.TextBoxItem.MultilineChanged += new EventHandler(textBoxItem_MultilineChanged);
            this.TextBoxItem.ReadOnlyChanged += new EventHandler(textBoxItem_ReadOnlyChanged);
            this.TextBoxItem.TextAlignChanged += new EventHandler(textBoxItem_TextAlignChanged);
            this.TextBoxItem.TextChanged += new EventHandler(textBoxItem_TextChanged);
            this.TextBoxItem.TextChanging += new TextChangingEventHandler(textBoxItem_TextChanging);
        }
        
        protected virtual internal void UnwireHostEvents()
        {
            this.TextBoxItem.AcceptsTabChanged -= new EventHandler(textBoxItem_AcceptsTabChanged);
            this.TextBoxItem.HideSelectionChanged -= new EventHandler(textBoxItem_HideSelectionChanged);
            this.TextBoxItem.ModifiedChanged -= new EventHandler(textBoxItem_ModifiedChanged);
            this.TextBoxItem.MultilineChanged -= new EventHandler(textBoxItem_MultilineChanged);
            this.TextBoxItem.ReadOnlyChanged -= new EventHandler(textBoxItem_ReadOnlyChanged);
            this.TextBoxItem.TextAlignChanged -= new EventHandler(textBoxItem_TextAlignChanged);
            this.TextBoxItem.TextChanged -= new EventHandler(textBoxItem_TextChanged);
            this.TextBoxItem.TextChanging -= new TextChangingEventHandler(textBoxItem_TextChanging);
        }

		#region TextBox methods
		/// <summary>
        /// Appends text to the current text.
        /// </summary>
		public void AppendText(string text)
		{
            this.TextBoxItem.AppendText(text);
		}

		/// <summary>
        /// Empties the TextBox.
        /// </summary>
		public void Clear()
		{
            this.TextBoxItem.Clear();
		}

		/// <summary>Undo to the previous text value before clear invocation.</summary>
		public void ClearUndo()
		{
            this.TextBoxItem.ClearUndo();
		}

		/// <summary>Copies the text value to the clipboard.</summary>
		public void Copy()
		{
            this.TextBoxItem.Copy();
		}

		/// <summary>Cuts the text value to the clipboard.</summary>
		public void Cut()
		{
            this.TextBoxItem.Cut();
		}

		/// <summary><para>Deselects the text in the cotrol.</para></summary>
		public void DeselectAll()
		{
            this.TextBoxItem.DeselectAll();
		}

		/// <summary>
		/// Retrieves the character that is closest to the specified location within the
		/// control.
		/// </summary>
		public char GetCharFromPosition(Point point)
		{
            return this.TextBoxItem.GetCharFromPosition(point);
		}

		/// <summary>
        /// Retrieves the index of the character nearest to the specified location.
        /// </summary>
		public int GetCharIndexFromPosition(Point point)
		{
            return this.TextBoxItem.GetCharIndexFromPosition(point);
		}

		/// <summary>
        /// Retrieves the index of the first character of a given line.
        /// </summary>
		public int GetFirstCharIndexFromLine(int lineNumber)
		{
            return this.TextBoxItem.GetFirstCharIndexFromLine(lineNumber);
		}

		/// <summary>
		/// 	<para>Retrieves the index of the first character of the current line. This method
		///     is not supported by MaskedTextBox.</para>
		/// </summary>
		public int GetFirstCharIndexOfCurrentLine()
		{
            return this.TextBoxItem.GetFirstCharIndexOfCurrentLine();
		}

		/// <summary>
		/// 	<para>Retrieves the line number from the specified character position within the
		///     text of the control.</para>
		/// </summary>
		public int GetLineFromCharIndex(int index)
		{
            return this.TextBoxItem.GetLineFromCharIndex(index);
		}

		/// <summary>
		/// 	<para>Retrieves the location within the control at the specified character
		///     index.</para>
		/// </summary>
		public Point GetPositionFromCharIndex(int index)
		{
            return this.TextBoxItem.GetPositionFromCharIndex(index);
		}

		/// <summary>
        /// Pastes the text value to the clipboard.
        /// </summary>
		public void Paste()
		{
            this.TextBoxItem.Paste();
		}

		/// <summary>
        /// Pastes the string parameter to the clipboard.
        /// </summary>
		public void Paste(string text)
		{
            this.TextBoxItem.Paste(text);
		}

		/// <summary>
        /// Scrolls the contents of the control to the current caret position.
        /// </summary>
		public void ScrollToCaret()
		{
            this.TextBoxItem.ScrollToCaret();
		}

		/// <summary>
		/// Selects the text in the TextBox from the start position inclusive to the end
		/// position exclusive.
		/// </summary>
		public void Select(int start, int length)
		{
            this.TextBoxItem.Select(start, length);
		}

		/// <summary>
        /// Selects the text in the TextBox.
        /// </summary>
		public void SelectAll()
		{
            this.TextBoxItem.SelectAll();
		}

		#endregion

		#region TextBoxItem events

        private void textBoxItem_AcceptsTabChanged(object sender, EventArgs e)
		{
			this.OnAcceptsTabChanged(e);
		}

		private void textBoxItem_HideSelectionChanged(object sender, EventArgs e)
		{
			this.OnHideSelectionChanged(e);
		}

		private void textBoxItem_ModifiedChanged(object sender, EventArgs e)
		{
			this.OnModifiedChanged(e);
		}

		private void textBoxItem_MultilineChanged(object sender, EventArgs e)
		{
			this.OnMultilineChanged(e);
		}

		private void textBoxItem_ReadOnlyChanged(object sender, EventArgs e)
		{
			this.OnReadOnlyChanged(e);
		}

		private void textBoxItem_TextAlignChanged(object sender, EventArgs e)
		{
			this.OnTextAlignChanged(e);
		}

		private void textBoxItem_TextChanging(object sender, TextChangingEventArgs e)
		{
			this.OnTextChanging(e);
		}

		private void textBoxItem_TextChanged(object sender, EventArgs e)
		{
			this.OnTextChanged(e);
		}
		#endregion

		#region Events
		/// <summary>
		/// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Occurs when
		/// the value of the AcceptsTab property has changed.</span>
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the AcceptsTab property has changed.")]
		public event EventHandler AcceptsTabChanged
		{
			add
			{
				this.Events.AddHandler(AcceptsTabChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(AcceptsTabChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the AcceptsTabChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnAcceptsTabChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[AcceptsTabChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Occurs when
		/// the value of the HideSelection property changes.</span>
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the HideSelection property has changed.")]
		public event EventHandler HideSelectionChanged
		{
			add
			{
				this.Events.AddHandler(HideSelectionChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(HideSelectionChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the HideSelectionChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnHideSelectionChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[HideSelectionChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Occurs when
		/// the value of the Modified property has changed.</span>
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the Modified property has changed.")]
		public event EventHandler ModifiedChanged
		{
			add
			{
				this.Events.AddHandler(ModifiedChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(ModifiedChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the ModifiedChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnModifiedChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[ModifiedChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl02_LabelAbstract">Occurs when
		/// the value of the Multiline property has changed.</span>
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the Multiline property has changed.")]
		public event EventHandler MultilineChanged
		{
			add
			{
				this.Events.AddHandler(MultilineChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(MultilineChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the MultilineChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnMultilineChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[MultilineChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl01_LabelAbstract">Occurs when
		/// the ReadOnly property changes.</span>
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the ReadOnly property has changed.")]
		public event EventHandler ReadOnlyChanged
		{
			add
			{
				this.Events.AddHandler(ReadOnlyChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(ReadOnlyChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the ReadOnlyChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnReadOnlyChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[ReadOnlyChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// 	<span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl03_LabelAbstract">Occurs when
		/// the value of the TextAlign property has changed.</span>
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs when the TextAlign property has changed.")]
		public event EventHandler TextAlignChanged
		{
			add
			{
				this.Events.AddHandler(TextAlignChangedEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(TextAlignChangedEventKey, value);
			}
		}

		/// <summary>
		/// Raises the TextAlignChanged event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnTextAlignChanged(EventArgs e)
		{
			EventHandler handler1 = (EventHandler)this.Events[TextAlignChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}

		/// <summary>
		/// 	<para><span id="ctl00_ContentPlaceHolder1_src1_resRC_ctl03_LabelAbstract">Occurs
		///     when text is being changed.</span></para>
		/// </summary>
		[Browsable(true),
		Category("Property Changed"),
		Description("Occurs before the Text property changes.")]
		public event TextChangingEventHandler TextChanging
		{
			add
			{
				this.Events.AddHandler(TextChangingEventKey, value);
			}
			remove
			{
				this.Events.RemoveHandler(TextChangingEventKey, value);
			}
		}

		/// <summary>
		/// Raises the TextChanging event.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected virtual void OnTextChanging(TextChangingEventArgs e)
		{
			TextChangingEventHandler handler1 = (TextChangingEventHandler)this.Events[TextChangingEventKey];
			if (handler1 != null)
			{
				handler1(this, e);
			}
		}
        
        #region Focus management

        private bool entering = false;

        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            if (!entering)
            {
                entering = true;
                this.TextBoxItem.TextBoxControl.Focus();
                this.OnGotFocus(e);
                entering = false;
            }
        }

        protected override void OnLeave(EventArgs e)
        {
            base.OnLeave(e);
            this.OnLostFocus(e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            if (!entering)
            {
                base.OnLostFocus(e);
            }
        }

        #endregion

		#endregion

        /// <summary>
        /// Initializes the root element
        /// </summary>
        /// <param name="rootElement"></param>
        protected override void InitializeRootElement(RootRadElement rootElement)
        {
            base.InitializeRootElement(rootElement);
            rootElement.StretchHorizontally = true;
            rootElement.StretchVertically = false;
        }
	}

    internal class TextBoxAutoCompleteSourceConverter : EnumConverter
    {
        // Methods
        public TextBoxAutoCompleteSourceConverter(Type type)
            : base(type)
        {
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            TypeConverter.StandardValuesCollection standardValues = base.GetStandardValues(context);
            ArrayList values = new ArrayList();
            int count = standardValues.Count;
            for (int i = 0; i < count; i++)
            {
                if (!standardValues[i].ToString().Equals("ListItems"))
                {
                    values.Add(standardValues[i]);
                }
            }
            return new TypeConverter.StandardValuesCollection(values);
        }
    }

 


 

}
