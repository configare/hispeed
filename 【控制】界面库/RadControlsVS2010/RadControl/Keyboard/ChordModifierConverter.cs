using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using System.Collections;

namespace Telerik.WinControls.Keyboard
{
	class ChordModifierConverter : TypeConverter
	{
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
            string chordModifier = ((string)value).Trim();
            if (!string.Equals(chordModifier, string.Empty)) //&& (context != null)
            {
                char[] separator = ChordConverter.separator.ToCharArray();
                ChordModifier returnModifier = new ChordModifier();
                //Keys keysSeparator = Keys.None;
                string[] keys = chordModifier.Split(separator);
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i] = keys[i].Trim();
                    keys[i] = keys[i].ToUpper();
                }
                for (int i = 0; i < keys.Length; i++)
                {
                    object obj1 = ChordConverter.KeyMaps[keys[i]];
                    if (obj1 == null)
                    {
                        try
                        {
                            obj1 = Keys.Parse(typeof(Keys), keys[i]);
                        }
                        catch (ArgumentException)
                        {
                        }
                    }
                    if (obj1 != null)
                    {
                        Keys tempKey = (Keys)obj1;
                        if (tempKey == Keys.Menu)
                        {
                            returnModifier.AltModifier = true;
                        }
                        if (tempKey == Keys.ControlKey)
                        {
                            returnModifier.ControlModifier = true;
                        }
                        if (tempKey == Keys.ShiftKey)
                        {
                            returnModifier.ShiftModifier = true;
                        }
                    }
                }
                if (!returnModifier.IsEmpty)
                {
                    return returnModifier;
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
			if ((value is ChordModifier) || (value is Keys))
			{
				ChordModifier valueToConvert = null;
				if (value is Keys)
				{
					valueToConvert = ChordModifier.GetModifiers((Keys)value);
				}
				else
				{
					valueToConvert = value as ChordModifier;
				}
				if (valueToConvert.IsEmpty)
				{
					return null;
				}
				//string[] keys = null;
				List<string> keysList = new List<string>(1);
				foreach (string key in ChordConverter.KeyMaps.Keys)
				{
                    if ((valueToConvert.AltModifier &&
                        Keys.Menu == (Keys)ChordConverter.KeyMaps[key]) ||

						(valueToConvert.ControlModifier &&
                        Keys.ControlKey == (Keys)ChordConverter.KeyMaps[key]) ||

						(valueToConvert.ShiftModifier &&
						Keys.ShiftKey == (Keys)ChordConverter.KeyMaps[key]))
                    {
						keysList.Add(key.ToString());
                    }
				}
				if (keysList.Count>0)
				{
					return string.Join(ChordConverter.separator, keysList.ToArray());
				}
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
	}
}
