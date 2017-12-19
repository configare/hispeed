using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;

namespace Telerik.WinControls.Keyboard
{
    /// <summary>
    /// Represent a chord.
    /// </summary>
    [Editor(typeof(ChordsEditor), typeof(UITypeEditor)), TypeConverter(typeof(ChordConverter))]
    public class Chord : IComparable
    {
		#region Constructors
        /// <summary>
        /// Initializes a new instance of the Chord class.
        /// </summary>
		public Chord()
			: this(new List<Keys>(), new ChordModifier())
		{
		}
        /// <summary>
        /// Initializes a new isntance of the Chord class using a list of keys.
        /// </summary>
        /// <param name="keys"></param>
		public Chord(List<Keys> keys)
			: this(keys, new ChordModifier())
		{
		}
        /// <summary>
        /// Initializes a new instance of the Chord class using a list of keys
        /// and %chord modifier:Telerik.WinControls.Keyboard.ChordModifier%.
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="chordModifier"></param>
		public Chord(List<Keys> keys, ChordModifier chordModifier)
		//	: this(keys)
		{
			this.keys = keys;
			this.chordModifier = chordModifier;
			this.ProccessModifiers();
			//this.ReverseProccessModifiers();
		}
        /// <summary>
        /// Initializes a new instance of the Chord class using a string of keys.
        /// </summary>
        /// <param name="keys"></param>
		public Chord(string keys)
		{
			this.Keys = keys;
			//this.ProccessModifiers();
		} 
		#endregion

		//Fields
		private List<Keys> keys = null;
		private ChordModifier chordModifier;
		private TypeConverter chordModifierConverter;
		private TypeConverter chordConverter;

		#region Properties
        /// <summary>Gets or sets a list of keys in this instance.</summary>
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public List<Keys> KeysInternal
		{
			get
			{
				if (this.keys == null)
				{
					this.keys = new List<Keys>();
				}
				return keys;
			}
			set
			{
				keys = value;
				this.ProccessModifiers();
			}
		}

        /// <summary>Gets or sets the keys in this chord.</summary>
		public string Keys
		{
			get
			{
				if (this.ChordConverter.CanConvertTo(typeof(string)))
				{
					return this.ChordConverter.ConvertToString(this) as string;
				}
				return string.Empty;
			}
			set
			{
				if (string.IsNullOrEmpty(value))
				{
					this.Clear();
				}
				else
				{
					if (this.ChordConverter.CanConvertFrom(typeof(string)))
					{
						Chord temp = (Chord)this.ChordConverter.ConvertFromString(value);
						if (temp != null)
						{
							this.KeysInternal = temp.KeysInternal;
							this.ChordModifier = temp.ChordModifier;
						}
					}
				}
			}
		}

        /// <summary>Gets the modifier strings.</summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ModifierKeys
		{
			get
			{
				if (this.ChordModifierConverter.CanConvertTo(typeof(string)))
				{
					return this.ChordModifierConverter.ConvertToString(this.chordModifier) as string;
				}
				return string.Empty;
			}
		}

        /// <summary>Gets the chord keys.</summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string ChordKeys
		{
			get
			{
				if (this.ChordConverter.CanConvertTo(typeof(string)))
				{
					Chord temp = new Chord(this.KeysInternal);
					return this.ChordConverter.ConvertToString(temp) as string;
				}
				return string.Empty;
			}
		}

        /// <summary>Gets or sets the chord modifier.</summary>
		public ChordModifier ChordModifier
		{
			get
			{
				return chordModifier;
			}
			set
			{
				chordModifier = value;
			}
		}

		private TypeConverter ChordModifierConverter
		{
			get
			{
				if (this.chordModifierConverter == null)
				{
					this.chordModifierConverter = TypeDescriptor.GetConverter(typeof(ChordModifier));
				}
				return this.chordModifierConverter;
			}
		}

		private TypeConverter ChordConverter
		{
			get
			{
				if (this.chordConverter == null)
				{
					this.chordConverter = TypeDescriptor.GetConverter(this);
				}
				return this.chordConverter;
			}
		} 
		#endregion

		#region Methods
        /// <summary>Clears the chord.</summary>
		public void Clear()
		{
			if (this.ChordModifier != null)
			{
				this.ChordModifier.Clear();
			}
			if (this.KeysInternal != null)
			{
				this.KeysInternal.Clear();
			}
		}

        /// <summary>Retrieves the string representation of the instance.</summary>
		public override string ToString()
		{
			if (!string.IsNullOrEmpty(this.Keys))
			{
				return this.Keys;
			}
			return base.ToString();
		}

        /// <summary>Processes the modifiers.</summary>
		public void ProccessModifiers()
		{
			if (this.chordModifier == null)
			{
				this.chordModifier = new ChordModifier();
			}
			for (int i = 0; i < this.keys.Count; i++)
			{
				bool flag1 = false;
				if ((this.keys[i] & System.Windows.Forms.Keys.Shift) == System.Windows.Forms.Keys.Shift)
				{
					this.chordModifier.ShiftModifier = true;
					flag1 = true;
				}
				if ((this.keys[i] & System.Windows.Forms.Keys.Control) == System.Windows.Forms.Keys.Control)
				{
					this.chordModifier.ControlModifier = true;
					flag1 = true;
				}
				if ((this.keys[i] & System.Windows.Forms.Keys.Alt) == System.Windows.Forms.Keys.Alt)
				{
					this.chordModifier.AltModifier = true;
					flag1 = true;
				}
				if (flag1)
				{
					// remove the modifier keys as they are stored inside the ChordModifier instance
					this.keys.Remove(this.keys[i]);
				}
			}
		}
        /// <summary>
        /// 
        /// </summary>
		public void ReverseProccessModifiers()
		{
			if (this.chordModifier != null)
			{
				if (this.chordModifier.ShiftModifier &&
					!this.keys.Contains(System.Windows.Forms.Keys.ShiftKey))
				{
					this.keys.Add(System.Windows.Forms.Keys.ShiftKey);
				}
				if (this.chordModifier.ControlModifier &&
					!this.keys.Contains(System.Windows.Forms.Keys.ControlKey))
				{
					this.keys.Add(System.Windows.Forms.Keys.ControlKey);
				}
				if (this.chordModifier.AltModifier &&
					!this.keys.Contains(System.Windows.Forms.Keys.Alt))
				{
					this.keys.Add(System.Windows.Forms.Keys.Alt);
				}
			}
		} 
		#endregion

        #region IComparable Members
        /// <summary>
        /// Compares two instance for equality. 
        /// <returns>returns 0 if equal, a positive number if the first is greater than the 
        /// second, and a negative number otherwise.</returns>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            Chord tempChord = obj as Chord;
            if (tempChord != null)
            {
                if (this.ChordModifier.CompareTo(tempChord.ChordModifier) == 0)
                {                    
                    if (this.KeysInternal.Count == tempChord.KeysInternal.Count)
                    {
                        this.KeysInternal.Sort();
                        tempChord.KeysInternal.Sort();
                        for (int i = 0; i < this.KeysInternal.Count; i++)
                        {
                            if (!this.KeysInternal[i].Equals(tempChord.KeysInternal[i]))
                            {
                                return -1;
                            }

                         }

                        return 0;
                    }
                    return (this.KeysInternal.Count - tempChord.KeysInternal.Count);
                }
                return this.ChordModifier.CompareTo(tempChord.ChordModifier);
            }
            return 1;
        }

        #endregion
    }
}