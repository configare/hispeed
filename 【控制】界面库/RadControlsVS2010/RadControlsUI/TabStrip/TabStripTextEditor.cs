using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Represents Tab strip text editor
	/// </summary>
	public class TabStripTextEditor : RadTextBoxItem, IDisposable, IValueEditor
	{
		private static readonly object ValidatingEventKey = new object();
		private static readonly object ValueChangedEventKey = new object();
		private static readonly object ValueChangingEventKey = new object();
		private static readonly object ValidationErrorEventKey = new object();
		private object oldValue = string.Empty;


		/// <summary>
		/// Represents the TabStripTextEditor constructor
		/// </summary>
		public TabStripTextEditor()
			: base()
		{
			this.WireEvents();
		}

		//fields
		private string defaultValue = string.Empty;

		internal void WireEvents()
		{
			this.KeyDown += new KeyEventHandler(this.editor_KeyDown);
		}

		internal void UnWireEvents()
		{
			this.KeyDown -= new KeyEventHandler(this.editor_KeyDown);
		}


		#region IValueEditor Members

		public void Initialize()
		{
			this.Value = string.Empty;
		}

		public void Initialize(object value)
		{
			this.oldValue = value;
			this.Value = value;
		}

		/// <summary>
		/// Initializes editor's owner and value 
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="value"></param>
		public void Initialize(object owner, object value)
		{
        	TabItem ownerNode = owner as TabItem;
			if (ownerNode == null)
				throw new ArgumentException("unable to cast owner to TabItem");
			this.oldValue = value;
			this.Value = value;

		}

        ///// <summary>
        ///// Saves editor's value
        ///// </summary>
        //public void SaveValue()
        //{

        //}

		/// <summary>
		/// Begins editor's edit
		/// </summary>
		public void BeginEdit()
		{
        	this.Focus();
			this.HostedControl.Focus();
		}

		/// <summary>
		/// Ends editor's edit
		/// </summary>
		/// <returns></returns>
		public bool EndEdit()
		{
			this.OnValueChanged(EventArgs.Empty);
			return true;
		}

		/// <summary>
		/// Ends editor's edit
		/// </summary>
		/// <returns></returns>
		public bool EndEdit(bool cancelEdit)
		{
			if (!cancelEdit)
			{
				return this.EndEdit();
			}
			return false;
		}

		/// <summary>
		/// validates the value
		/// </summary>
		/// <returns></returns>
        public bool Validate()
        {
            this.ValidateCore();
            return true;
        }

        protected virtual void ValidateCore()
        {
        }

		/// <summary>
		/// Gets or sets editor's value
		/// </summary>
		public object Value
		{
			get
			{
				return this.Text;
			}
			set
			{
				this.Text = value.ToString();
			}
		}

		/// <summary>
		/// Gets or sets the default editing value
		/// </summary>
		public object DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
			set
			{
				this.defaultValue = value.ToString();
			}
		}

		/// <summary>
		/// Gets or sets the minimum value
		/// </summary>
		public object MinValue
		{
			get
			{
				return string.Empty;
			}
			set
			{

			}
		}

		/// <summary>
		/// Gets or sets the maximum value
		/// </summary>
		public object MaxValue
		{
			get
			{
				return string.Empty;
			}
			set
			{

			}
		}

		#endregion


		#region Events

		/// <summary>
		/// Occurs when the editor value is being changed. Cancelable event.
		/// </summary>
		[Category(RadDesignCategory.ActionCategory),
		 Description("Occurs when the editor is changing the value during the editing proccess.")]
		public event ValueChangingEventHandler ValueChanging
		{
			add
			{
				base.Events.AddHandler(ValueChangingEventKey, value);
			}
			remove
			{
				base.Events.RemoveHandler(ValueChangingEventKey, value);
			}
		}

		/// <summary>
		/// Occurs when the editor finished the value editing.
		/// </summary>
		[Browsable(true),
		Category(RadDesignCategory.ActionCategory),
		Description("Occurs when the editor finished the value editing.")]
		public event EventHandler ValueChanged
		{
			add
			{
				base.Events.AddHandler(ValueChangedEventKey, value);
			}
			remove
			{
				base.Events.RemoveHandler(ValueChangedEventKey, value);
			}
		}

		///// <summary>
		///// Occurs when internally the editor detects an error or when the Validating event fails.
		///// </summary>
		public event ValidationErrorEventHandler ValidationError
		{
			add
			{
				base.Events.AddHandler(ValidationErrorEventKey, value);
			}
			remove
			{
				base.Events.RemoveHandler(ValidationErrorEventKey, value);
			}
		}

		/// <summary>
		/// Handles the Validating event
		/// </summary>
		/// <param name="args"></param>
		protected virtual void OnValidating(EventArgs args)
		{
			EventHandler handler1 =
				(EventHandler)base.Events[ValidatingEventKey];
			if (handler1 != null)
			{
				handler1(this, args);
			}
		}

		/// <summary>
		/// Handles the ValueChanged event
		/// </summary>
		/// <param name="args"></param>
		protected virtual void OnValueChanged(EventArgs args)
		{
			EventHandler handler1 =
				(EventHandler)base.Events[ValueChangedEventKey];
			if (handler1 != null)
			{
				handler1(this, args);
			}
		}

		/// <summary>
		/// Handles the ValueChanging event
		/// </summary>
		/// <param name="args"></param>
		protected virtual void OnValueChanging(ValueChangingEventArgs args)
		{
			ValueChangingEventHandler handler1 =
				(ValueChangingEventHandler)base.Events[ValueChangingEventKey];
			if (handler1 != null)
			{
				handler1(this, args);
			}
		}


		private void editor_KeyDown(object sender, KeyEventArgs e)
		{
			string oldEditorValue = this.Text;
			//CurrentEvent.Event = e;

			Keys pressedKey = e.KeyCode;
			if (pressedKey == Keys.Return)
			{
				if (e.Modifiers == Keys.None)
				{
					e.SuppressKeyPress = true;
					e.Handled = true;
					this.EndEdit();
					return;
				}
			}
			else if (pressedKey == Keys.Escape)
			{
				e.Handled = true;
				this.Value = this.oldValue;
				this.EndEdit(true);
				return;
			}

			ValueChangingEventArgs args;
			char nextChar = Convert.ToChar(e.KeyValue);
			if (!e.Shift)
				nextChar = char.ToLower(nextChar);
			if (this.ValidateKey(e.KeyData))
			{
				args = new ValueChangingEventArgs(nextChar);
				this.OnValueChanging(args);
				if (args.Cancel)
				{
					this.Value = oldEditorValue;
				}
			}
		}

		private bool ValidateKey(Keys keyData)
		{
			Keys[] beginEditKeysArray1 = new Keys[] { 
                Keys.A, Keys.B, Keys.C, Keys.D, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, 
                Keys.E, Keys.F, Keys.G, Keys.H, Keys.I, Keys.Insert, Keys.J, Keys.K, Keys.L, Keys.M, Keys.N, 
				Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, 
                Keys.O, Keys.OemBackslash, Keys.OemCloseBrackets, Keys.Oemcomma, Keys.OemMinus, Keys.OemOpenBrackets, Keys.OemPeriod, Keys.Oemplus, Keys.OemQuestion, Keys.OemQuotes, Keys.OemSemicolon, Keys.Oemtilde, Keys.P, Keys.Pause, 
                Keys.Q, Keys.R, Keys.Right, Keys.S, Keys.Space, Keys.T, Keys.Tab, Keys.U, Keys.Up, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z};

			Keys k = keyData & ~Keys.Shift;

			for (int num1 = 0; num1 < beginEditKeysArray1.Length; num1++)
			{
				if (k == beginEditKeysArray1[num1])
				{
					return true;
				}
			}
			return false;
		}


		/// <summary>
		/// Handles ErrorOccured event
		/// </summary>
		/// <param name="message"></param>
		protected virtual void OnErrorOccured(string message)
		{
			Exception ex = new Exception(message);
		}
		#endregion

		/// <summary>
		/// Gets editor's visual mode
		/// </summary>
		public EditorVisualMode VisualMode
		{
			get
			{
				return EditorVisualMode.Inline;
			}
		}

		/// <summary>
		/// Gets editor's supported type 
		/// </summary>
		public EditorSupportedType SupportedType
		{
			get
			{
				return EditorSupportedType.Object;
			}
		}

		/// <summary>
		/// Disposes the editor
		/// </summary>
		public new void Dispose()
		{
			this.UnWireEvents();
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
	}

}
