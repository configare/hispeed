using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using System.Windows.Forms;

namespace Telerik.WinControls.Keyboard
{
    class ChordConverter : TypeConverter, IComparer
    {
		#region Constructors
		static ChordConverter()
		{
			Keys[] keysArray1 = new Keys[] { 
                Keys.A, Keys.B, Keys.C, Keys.D, Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.Delete, Keys.Down, 
                Keys.E, Keys.End, Keys.F, Keys.F1, Keys.F10, Keys.F11, Keys.F12, Keys.F13, Keys.F14, Keys.F15, Keys.F16, Keys.F17, Keys.F18, Keys.F19, Keys.F2, Keys.F20, 
                Keys.F21, Keys.F22, Keys.F23, Keys.F24, Keys.F3, Keys.F4, Keys.F5, Keys.F6, Keys.F7, Keys.F8, Keys.F9, Keys.G, Keys.H, Keys.I, Keys.Insert, Keys.J, 
                Keys.K, Keys.L, Keys.Left, Keys.M, Keys.N, Keys.NumLock, Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9, 
                Keys.O, Keys.OemBackslash, Keys.OemClear, Keys.OemCloseBrackets, Keys.Oemcomma, Keys.OemMinus, Keys.OemOpenBrackets, Keys.OemPeriod, Keys.OemPipe, Keys.Oemplus, Keys.OemQuestion, Keys.OemQuotes, Keys.OemSemicolon, Keys.Oemtilde, Keys.P, Keys.Pause, 
                Keys.Q, Keys.R, Keys.Right, Keys.S, Keys.Space, Keys.T, Keys.Tab, Keys.U, Keys.Up, Keys.V, Keys.W, Keys.X, Keys.Y, Keys.Z
             };
			ValidKeys = keysArray1;
			displayOrder = new List<string>(0x22);

			KeyMaps = new Hashtable(0x22);

			AddKey("Enter", Keys.Return);
			AddKey("Ctrl", Keys.ControlKey);
			AddKey("PgDn", Keys.Next);
			AddKey("Ins", Keys.Insert);
			AddKey("Del", Keys.Delete);
			AddKey("PgUp", Keys.Prior);
			AddKey("Alt", Keys.Menu);
			AddKey("0", Keys.D0);
			AddKey("1", Keys.D1);
			AddKey("2", Keys.D2);
			AddKey("3", Keys.D3);
			AddKey("4", Keys.D4);
			AddKey("5", Keys.D5);
			AddKey("6", Keys.D6);
			AddKey("7", Keys.D7);
			AddKey("8", Keys.D8);
			AddKey("9", Keys.D9);
			AddKey("+", Keys.Oemplus);
			//AddKey("Home", Keys.Home);
			AddKey("Shift", Keys.ShiftKey);
			//AddKey("Back", Keys.Back);
		}

		public ChordConverter()
		{
		} 
		#endregion

		#region Override Methods
		public override bool CanConvertFrom(ITypeDescriptorContext context, System.Type sourceType)
		{
			if (sourceType != typeof(string))
			{
                return base.CanConvertFrom(context, sourceType);
			}
            return true;	
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, System.Type destinationType)
		{
			if (destinationType != typeof(string))
			{
                return base.CanConvertTo(context, destinationType);
			}
            return true;
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
            if (!(value is string))
            {
                return base.ConvertFrom(context, culture, value);
            }
            string chord = ((string)value).Trim();
			Chord returnChord = new Chord();
            if (!string.Equals(chord, string.Empty)) //&& (context != null)
            { 
			    List<Keys> realKeys = new List<Keys>();
                string[] keys = chord.Split(ChordConverter.separator.ToCharArray());
                //clearing and unifying the string representations of Keys
                for (int i = 0; i < keys.Length; i++)
			    {
				    keys[i] = keys[i].Trim();
				    keys[i] = keys[i].ToUpper();
			    }
			    Keys keysSeparator = Keys.None;
                object separatorObj = KeyMaps[ChordConverter.separator.ToCharArray()];
			    if (separatorObj != null)
			    {
				    keysSeparator = (Keys)separatorObj;
			    }
                for (int i = 0; i < keys.Length; i++)
                {
                    Keys tempKey = Keys.None;
                    // searching for key aliases
                    foreach (string key in ChordConverter.KeyMaps.Keys)
                    {
                        if (keys[i] == key.ToUpper())
                        {
                            tempKey = (Keys)ChordConverter.KeyMaps[key];
                            break;
                        }
                    }
                    // trying to parse 
                    if (tempKey == Keys.None)
                    {
                        try
                        {
                            object obj1 = Keys.Parse(typeof(Keys), keys[i]);
                            if (obj1 != null)
                            {
                                tempKey = (Keys)obj1;
                            }
                        }
                        catch (ArgumentException)
                        {
                            //throw new FormatException("Invalid Key Name: " + keys[i]);
                        }
                    }
                    if (tempKey != Keys.None)
                    {
						if (tempKey == Keys.Menu)
						{
							returnChord.ChordModifier.AltModifier = true;
						}
						else if (tempKey == Keys.ControlKey)
						{
							returnChord.ChordModifier.ControlModifier = true;
						}
						else if (tempKey == Keys.ShiftKey)
						{
							returnChord.ChordModifier.ShiftModifier = true;
						}
						else
						{
							realKeys.Add(tempKey);
						}
                    }
                    else
                    {
                        // the keys separator is used in the chord combination
                        if (string.IsNullOrEmpty(keys[i]))
                        {
                            bool separatorFixed = false;
                            int internalCount = i;
                            int pairCount = 0;
                            while (!separatorFixed)
                            {
                                if (string.IsNullOrEmpty(keys[internalCount]))
                                {
                                    pairCount++;
                                }
                                if (pairCount == 2)
                                {
                                    pairCount = 0;
                                    if (keysSeparator != Keys.None)
                                    {
                                        realKeys.Add(keysSeparator);
                                    }
                                    else
                                    {
                                        try
                                        {
                                            object obj1 = Keys.Parse(typeof(Keys), ChordConverter.separator);
                                            if (obj1 != null)
                                            {
                                                realKeys.Add((Keys)obj1);
                                            }
                                        }
                                        catch
                                        {
                                            //throw new FormatException("Invalid Key Name: " + ChordConverter.separator);
                                        }
                                    }
                                }
                                if (!string.IsNullOrEmpty(keys[internalCount]))
                                {
                                    break;
                                }
                                if ((internalCount + 1) < keys.Length)
                                {
                                    internalCount++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        // testing for mallformations like few chars combined without a separator
                        else //if (keys[i].Length > 0)
                        {
                            for (int j = 0; j < keys[i].Length; j++)
                            {
                                try
                                {
                                    object obj1 = Keys.Parse(typeof(Keys), (keys[i])[j].ToString().ToUpper());
                                    if (obj1 != null)
                                    {
                                        realKeys.Add((Keys)obj1);
                                    }
                                }
                                catch (ArgumentException)
                                {
                                    throw new FormatException("Invalid Key Name: " + keys[i][j]);
                                }
                            }
                        }
                    }
                }
				if (!returnChord.ChordModifier.IsEmpty || 
					realKeys.Count>0)
				{
					returnChord.KeysInternal = realKeys;
					return returnChord;
				}
            }
            return null;
		}

		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, System.Type destinationType)
		{
            if (destinationType == null)
            {
                throw new ArgumentNullException("Destination Type is not defined.");
            }
            if (destinationType != typeof(string))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
            if (value == null)
            {
                return null;
            }
			if ((value is Chord) || (value is List<Keys>))
			{
				List<Keys> keysToConvert = value as List<Keys>;
                if (keysToConvert == null)
				{
					keysToConvert = (value as Chord).KeysInternal;
				}
				//if (keysToConvert.Count == 0)
				//{
				//    return null;
				//}
				string[] convertedKeys = new string[keysToConvert.Count];

				for (int i = 0; i < keysToConvert.Count; i++)
				{
					Keys keyValue = keysToConvert[i];
                    foreach (string key in ChordConverter.KeyMaps.Keys)
					{
						if (keyValue == (Keys)ChordConverter.KeyMaps[key])
						{
							convertedKeys[i] = key;
							break;
						}
					}
					if (string.IsNullOrEmpty(convertedKeys[i]))
					{
						convertedKeys[i] = keyValue.ToString();
					}
				}
				//converting the modifier keys and serializing them
				string modifierKeys = string.Empty;
				if (value is Chord)
				{
					Chord temp = value as Chord;
					modifierKeys = temp.ModifierKeys;
				}
				if (!string.IsNullOrEmpty(modifierKeys) ||
					convertedKeys.Length > 0)
				{
					string returnString = string.Empty;
					if (!string.IsNullOrEmpty(modifierKeys))
					{
						 returnString += modifierKeys;
					}
					if (convertedKeys.Length > 0)
					{
						if (!string.IsNullOrEmpty(modifierKeys))
						{
							returnString += ChordConverter.separator;
						}
						returnString += string.Join(ChordConverter.separator, convertedKeys);
					}
					return returnString;
				}
				// Should be removed as no chord is valid if there are no modifier keys
				//return string.Join(ChordConverter.separator, convertedKeys);
				return null;
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
		{
			return false;
		}

		public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
		{
			return true;
		} 
		#endregion

		public virtual bool ValidateChordCombination() 
		{
			return true;
		}

		private static void AddKey(string key, Keys value)
		{
			KeyMaps[key] = value;
			displayOrder.Add((string)key);
		}

		#region IComparer Interface Members
		public int Compare(object a, object b)
		{
			return string.Compare(base.ConvertToString(a), base.ConvertToString(b), false, CultureInfo.InvariantCulture);
		}
		#endregion

        // Fields
		public static Keys[] ValidKeys;
		public static IDictionary KeyMaps;
        private static List<string> displayOrder;
		internal static string separator = "+";

        private const Keys FirstAscii = Keys.A;
        private const Keys FirstDigit = Keys.D0;
        private const Keys FirstNumpadDigit = Keys.NumPad0;

        private const Keys LastAscii = Keys.Z;
        private const Keys LastDigit = Keys.D9;
        private const Keys LastNumpadDigit = Keys.NumPad9;
		//private TypeConverter.StandardValuesCollection values;
    }
}
//public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
//{
//    if (this.values == null)
//    {
//        ArrayList list1 = new ArrayList();
//        foreach (object obj1 in KeyMaps.Values)
//        {
//            list1.Add(obj1);
//        }
//        list1.Sort(this);
//        this.values = new TypeConverter.StandardValuesCollection(list1.ToArray());
//    }
//    return this.values;
//}
//private void Initialize()
//{
//    //this.AddKey("F12", Keys.F12);
//    //this.AddKey("F11", Keys.F11);
//    //this.AddKey("F10", Keys.F10);
//    //this.AddKey(System.Windows.Forms.SR.GetString("toStringEnd"), Keys.End);
//    //this.AddKey("F8", Keys.F8);
//    //this.AddKey("F9", Keys.F9);
//    //this.AddKey(System.Windows.Forms.SR.GetString("toStringAlt"), Keys.Alt);
//    //this.AddKey("F4", Keys.F4);
//    //this.AddKey("F5", Keys.F5);
//    //this.AddKey("F6", Keys.F6);
//    //this.AddKey("F7", Keys.F7);
//    //this.AddKey("F1", Keys.F1);
//    //this.AddKey("F2", Keys.F2);
//    //this.AddKey("F3", Keys.F3);
//    //this.AddKey("toStringHome", Keys.Home);
//    //this.AddKey("toStringShift", Keys.Shift);
//    //this.AddKey("toStringBack", Keys.Back);
//}
//private List<string> DisplayOrder
//{
//    get
//    {
//        //if (this.displayOrder == null)
//        //{
//        //    this.Initialize();
//        //}
//        return displayOrder;
//    }
//}

//private IDictionary KeyMaps
//{
//    get
//    {
//        return KeyMaps;
//    }
//}