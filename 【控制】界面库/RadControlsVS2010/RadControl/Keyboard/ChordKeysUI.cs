using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Telerik.WinControls.Keyboard
{
	[ToolboxItem(false), ComVisible(false)]
    public partial class ChordKeysUI : UserControl
    {
		#region Constructors
		static ChordKeysUI()
		{
			//Keys[] keysArray1 = new Keys[] { 
			//    Keys.A, Keys.B, Keys.C, Keys.D, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.Delete, Keys.Down, 
			//    Keys.E, Keys.End, Keys.F, Keys.F1, Keys.F10, Keys.F11, Keys.F12, Keys.F13, Keys.F14, Keys.F15, Keys.F16, Keys.F17, Keys.F18, Keys.F19, Keys.F2, Keys.F20, 
			//    Keys.F21, Keys.F22, Keys.F23, Keys.F24, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8, Keys.F9, Keys.G, Keys.H, Keys.I, Keys.Insert, Keys.J, 
			//    Keys.K, Keys.L, Keys.Left, Keys.M, Keys.N, Keys.NumLock, Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, 
			//    Keys.O, Keys.OemBackslash, Keys.OemClear, Keys.OemCloseBrackets, Keys.Oemcomma, Keys.OemMinus, Keys.OemOpenBrackets, Keys.OemPeriod, Keys.OemPipe, Keys.Oemplus, Keys.OemQuestion, Keys.OemQuotes, Keys.OemSemicolon, Keys.Oemtilde, Keys.P, Keys.Pause, 
			//    Keys.Q, Keys.R, Keys.Right, Keys.S, Keys.Space, Keys.T, Keys.Tab, Keys.U, Keys.Up, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z
			// };
			//validKeys = keysArray1;
			validKeys = Telerik.WinControls.Keyboard.ChordConverter.ValidKeys;
		}

		public ChordKeysUI()
			: this(null)
		{
		}

		public ChordKeysUI(ChordsEditor editor)
		{
			this.editor = editor;
			this.chordConverter = null;
			this.currentValue = null;
			this.End();
			this.InitializeComponent();

			this.keyNames = new Hashtable(0x22);
			this.AddKeyMap("Enter", Keys.Return);
			this.AddKeyMap("Ctrl", Keys.Control);
			this.AddKeyMap("PgDn", Keys.Next);
			this.AddKeyMap("Ins", Keys.Insert);
			this.AddKeyMap("Del", Keys.Delete);
			this.AddKeyMap("PgUp", Keys.Prior);
			this.AddKeyMap("0", Keys.D0);
			this.AddKeyMap("1", Keys.D1);
			this.AddKeyMap("2", Keys.D2);
			this.AddKeyMap("3", Keys.D3);
			this.AddKeyMap("4", Keys.D4);
			this.AddKeyMap("5", Keys.D5);
			this.AddKeyMap("6", Keys.D6);
			this.AddKeyMap("7", Keys.D7);
			this.AddKeyMap("8", Keys.D8);
			this.AddKeyMap("9", Keys.D9);
		} 
		#endregion

		#region Initialization
		public void Start(IWindowsFormsEditorService edSvc, object value)
		{
			this.edSvc = edSvc;
			this.currentValue = new Chord();
			this.updateCurrentValue = false;
			if (value is Chord)
			{
				this.chkAlt.Checked = this.currentValue.ChordModifier.AltModifier = (value as Chord).ChordModifier.AltModifier;
				this.chkCtrl.Checked = this.currentValue.ChordModifier.ControlModifier = (value as Chord).ChordModifier.ControlModifier;
				this.chkShift.Checked = this.currentValue.ChordModifier.ShiftModifier = (value as Chord).ChordModifier.ShiftModifier;
			}
			this.originalValue = value;
			this.updateCurrentValue = true;
			if (value is Chord)
			{
				this.chordBox.Text = (value as Chord).Keys;
			}
		}

		public void End()
		{
			this.edSvc = null;
			this.Reset();
			this.originalValue = null;
			this.currentValue = null;
		}

		public void Reset() 
		{
			this.updateCurrentValue = false;
			if (this.chkCtrl != null)
			{
				this.chkCtrl.Checked = false;
			}
			if (this.chkShift != null)
			{
				this.chkShift.Checked = false;
			}
			if (this.chkAlt != null)
			{
				this.chkAlt.Checked = false;
			}
			if (this.chordBox != null)
			{
				this.chordBox.ResetText();
			}
			this.updateCurrentValue = true;
			this.keys.Clear();
		}

		#endregion

        private void ModifierChanged(object sender, EventArgs e)
        {
            this.UpdateCurrentValue();
        }

		private void ResetClick(object sender, EventArgs e)
		{
			this.Reset();
			if (this.currentValue != null)
			{
				this.currentValue.Clear();
			}
			this.UpdateCurrentValue();
		}

		private void AssignClick(object sender, EventArgs e)
		{
			this.UpdateCurrentValue();
			if (this.EditorService != null)
			{
				this.EditorService.CloseDropDown();
			}
		}

		private void ChordBoxTextChanged(object sender, EventArgs e)
		{
			this.UpdateCurrentValue();
		}

		private void UpdateCurrentValue()
		{
			if (this.updateCurrentValue && 
				this.currentValue != null)
			{
				this.currentValue.ChordModifier.AltModifier = this.chkAlt.Checked;
				this.currentValue.ChordModifier.ControlModifier = this.chkCtrl.Checked;
				this.currentValue.ChordModifier.ShiftModifier = this.chkShift.Checked;

				if (this.updateTextValue)
				{
					this.currentValue.Keys = this.chordBox.Text;
					this.updateTextValue = false;
					//this.chordBox.Text = String.Empty;
					this.chordBox.Clear();
					if (!string.IsNullOrEmpty(this.currentValue.Keys))
					{
						this.chordBox.AppendText(this.currentValue.Keys);
					}
					this.updateTextValue = true;
				}
			}
		}

		private static bool IsValidKey(Keys keyCode)
        {
            //Keys[] keysArray1 = validKeys;
			for (int i = 0; i < validKeys.Length; i++)
            {
				Keys keys1 = validKeys[i];
                if (keys1 == keyCode)
                {
                    return true;
                }
            }
            return false;
        }

		#region Control Overrides
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			this.chkCtrl.Focus();
			//this.chordBox.Focus();
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			Keys keys1 = keyData & Keys.KeyCode;
			Keys keys2 = keyData & Keys.Modifiers;// ((Keys)(-65536));
			Keys keys3 = keys1;
			if (keys3 != Keys.Tab)
			{
				if (keys3 == Keys.Escape)
				{
					if ((!this.chordBox.Focused || ((keys2 & (Keys.Alt | Keys.Control)) != Keys.None)))
					{
						this.currentValue = this.originalValue as Chord;
					}
				}
				else
				{
					if ((!this.chordBox.Focused || ((keys2 & (Keys.Alt | Keys.Control)) != Keys.None)))
					{
						return base.ProcessDialogKey(keyData);
					}
				}
			}
			else if ((keys2 == Keys.Shift) && this.chordBox.Focused)
			{
				this.btnReset.Focus();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}

		#endregion

		#region Typing Methods

		private void ChordBoxKeyPress(object sender, KeyPressEventArgs e)
		{
		}

		private void ChordBoxKeyDown(object sender, KeyEventArgs e)
		{
			// handling shortcuts inside the textbox
			Keys keyCode = (e.KeyCode & Keys.KeyCode);
			if (e.Modifiers != Keys.None /*&& (e.KeyCode != Keys.None)*/)
			{
				// updating the modifiers if they are pressed inside the textbox
				if (((keyCode | Keys.Menu) == Keys.Menu) ||
					((keyCode | Keys.ControlKey) == Keys.ControlKey) ||
					((keyCode | Keys.ShiftKey) == Keys.ShiftKey))
				{
					if (e.Alt)
					{
						this.chkAlt.Checked = true;
					}
					if (e.Control)
					{
						this.chkCtrl.Checked = true;
					}
					if (e.Shift)
					{
						this.chkShift.Checked = true;
					}
					this.updateCurrentValue = false;
					this.UpdateCurrentValue();
					this.updateCurrentValue = true;
				}
				return;
			}
			
			// searching for a human readable alias and string presentation
			string alias = String.Empty;
			foreach (string key in this.keyNames.Keys)
			{
				Keys keyAlias = (Keys)this.keyNames[key];
				if ((keyCode & keyAlias) == keyAlias)
				{
					alias = key;
					break;
				}
			}
			if (string.IsNullOrEmpty(alias))
			{
				alias = keyCode.ToString();
			}
			//Debug.WriteLine(alias);
			// detecting whether the key is autorepeat
			if (this.autorepeatKey)
			{
				int index = (this.keys.Count == 0) ? 0 : (this.keys.Count - 1);
				Keys tempCompareKey = Keys.None;
				try
				{
					object obj1 = Enum.Parse(typeof(Keys), this.keys[index]);
					if (obj1 != null)
					{
						tempCompareKey = (Keys)obj1;
					}
				}
				catch (ArgumentException)
				{
				}
				if (tempCompareKey != keyCode) 
				{
					this.autorepeatKey = false;
				}
			}
			if (!this.autorepeatKey)
			{
				this.chordBox.Select(this.chordBox.Text.Length, this.chordBox.Text.Length);
				this.keys.Add(alias);
				this.autorepeatKey = true;
			}
			else
			{
				e.SuppressKeyPress = true;
			}
		}

		private void ChordBoxKeyUp(object sender, KeyEventArgs e)
		{
			this.autorepeatKey = false;
			//Debug.WriteLine("KEYUP!!!!!!");
		} 

		#endregion

		private void AddKeyMap(string key, Keys value)
		{
			this.keyNames[key] = value;
		}

		#region Properties
		public IWindowsFormsEditorService EditorService
		{
			get
			{
				return this.edSvc;
			}
		}

		private TypeConverter ChordConverter
		{
			get
			{
				if (this.chordConverter == null)
				{
					this.chordConverter = TypeDescriptor.GetConverter(typeof(Chord));
				}
				return this.chordConverter;
			}
		}

		public object Value
		{
			get
			{
				//TODO: Use the type converter to get the correct value
				if (this.chordBox.Text.Length > 0)
				{
					return this.currentValue; // new Chord(this.chordBox.Text);
				}
				return null;
			}
		}

		internal System.Windows.Forms.Button AssignButton
		{
			get 
			{ 
				return this.btnAssign; 
			}
		}

		internal System.Windows.Forms.TextBox ChordBox
		{
			get
			{
				return this.chordBox;
			}
		}
		#endregion

        // Fields
		private static Keys[] validKeys;

		private bool autorepeatKey = false;
		private object originalValue;
        private Chord currentValue;            
        private bool updateCurrentValue;
		private bool updateTextValue = true;

        private ChordsEditor editor;
        private IWindowsFormsEditorService edSvc;
        private TypeConverter chordConverter;

        private List<string> keys = new List<string>();
        private IDictionary keyNames;
    }
}
