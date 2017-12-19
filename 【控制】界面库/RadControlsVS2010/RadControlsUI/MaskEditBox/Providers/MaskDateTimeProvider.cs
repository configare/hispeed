using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{ 
    public class MaskDateTimeProvider : IMaskProvider
    {
        string mask;
        CultureInfo culture;
        RadTextBoxItem textBoxItem;
        string maskFromFormat;
        List<MaskPart> list;
        int selectedItemIndex = -1;
        DateTime value = DateTime.Now;
        bool includePrompt = true;
        bool includeLiterals = true;
        DateTimeFormatInfo dateTimeFormatInfo;
        char promptChar = ' ';
        RadMaskedEditBoxElement owner;
        int yearResetValue = 1;
 
        public int YearResetValue
        {
            get
            {
                return yearResetValue;
            }
            set
            {
                yearResetValue = value;
            }
        }

        public int SelectedItemIndex
        {
            get
            {
                return selectedItemIndex;
            }
            set
            {
                selectedItemIndex = value;
            }
        }

        public object Value
        {
            get
            {
                return this.value;//this.ToString(this.includePrompt, this.includeLiterals);
            }
            set
            {
                CancelEventArgs eventArgs = new CancelEventArgs();
                this.owner.CallValueChanging(eventArgs);
                if (eventArgs.Cancel)
                {
                    return;
                }

                if (value == "")
                {
                    this.value = DateTime.Now;
                }
                else
                {
                    this.value = (DateTime)value;
                }

                textBoxItem.Text = this.value.ToString(this.mask, this.culture);
                this.FillCollectionWithValues(this.list, this.value, this.mask);
                this.SelectCurrentItemWithSelectedItem();
                this.owner.CallValueChanged(EventArgs.Empty);
            }
        }

        public MaskDateTimeProvider(string mask, CultureInfo culture, RadMaskedEditBoxElement owner)
        {
            this.owner = owner;
            this.textBoxItem = owner.TextBoxItem;
            this.mask = mask;
            this.culture = culture;
            this.textBoxItem = owner.TextBoxItem;
            this.dateTimeFormatInfo = culture.DateTimeFormat;
            this.maskFromFormat = MaskDateTimeProvider.GetSpecificFormat(mask, culture.DateTimeFormat);
            this.list = MaskDateTimeProvider.FillCollection(this.maskFromFormat, culture.DateTimeFormat);
            this.FillCollectionWithValues(list, this.value, this.mask);
            this.selectedItemIndex = 0;
            this.SelectFirstEditableItem();
        }

        public void KeyDown(object sender, KeyEventArgs e)
        { 
            KeyEventArgsWithMinMax keyEventArgsWithMinMax = e as KeyEventArgsWithMinMax;
            DateTime minDate= DateTime.MinValue;
            DateTime maxDate = DateTime.MaxValue;

            if (keyEventArgsWithMinMax!=null)
            {
                minDate = keyEventArgsWithMinMax.MinDate;
                maxDate = keyEventArgsWithMinMax.MaxDate;
            }

            if (e.KeyCode == Keys.Right)
            { 
                this.SelectNextEditableItem(); 
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Left)
            { 
                this.SelectPrevEditableItem(); 
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up)
            {                
                this.HandleSpinUp(minDate, maxDate);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
            {

                this.HandleSpinDown(minDate, maxDate);
                e.Handled = true;
            }
            else if (e.KeyValue == 46)
            {
                this.ResetCurrentPartValue(sender, e);
                e.Handled = true;
            } 
            else if (e.KeyCode == Keys.Home && (int)(e.KeyData & Keys.Shift) == 0)
            {
                this.SelectFirstEditableItem();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.End && (int)(e.KeyData & Keys.Shift) == 0)
            {
                this.SelectLastEditableItem();
                e.Handled = true;
            }

        }

        public void KeyPress(object sender, KeyPressEventArgs e)
        {
            this.SelectCurrentItemFromCurrentCaret();
            MaskPart part = this.list[this.selectedItemIndex];

            if (part.readOnly)
            {
                e.Handled = true;
                return;
            }

            this.ResetOnDelPartValue(part, e.KeyChar);
            switch (part.type)
            {
                case PartTypes.ReadOnly:
                    break;
                case PartTypes.Day:
                    this.HandleKeyPressDay(part, e);
                    this.SetDayMaxValue(this.list);
                    part.Validate();   
                    this.Value = new DateTime(this.value.Year, this.value.Month, part.value, this.value.Hour , this.value.Minute, this.value.Second);
                    break;                    
                case PartTypes.MiliSeconds:
                    break;
                case PartTypes.h12:                    
                case PartTypes.h24:
                    this.HandleKeyPressHour(part, e);
                    part.Validate();   
                    this.Value = new DateTime(this.value.Year, this.value.Month, this.value.Day, part.value, this.value.Minute, this.value.Second);
                    break;                    
                case PartTypes.Minutes:
                    this.HandleKeyPress(part, e);
                    part.Validate();   
                    this.Value = new DateTime(this.value.Year, this.value.Month, this.value.Day, this.value.Hour, part.value, this.value.Second);
                    break;
                case PartTypes.Month:
                    if (char.IsDigit(e.KeyChar))
                    {
                        this.HandleKeyPressMonth(part, e);
                    }
                    else
                    {
                        string[] monthNames = null;
                        if (part.maskPart.Length == 3)
                        {
                            monthNames = dateTimeFormatInfo.AbbreviatedMonthNames;
                            this.HandleKeyPressWithCharacters(part, e, monthNames);
                        }
                        else if (part.maskPart.Length > 3)
                        {
                            monthNames = dateTimeFormatInfo.MonthNames;
                            this.HandleKeyPressWithCharacters(part, e, monthNames);
                        }
                        else
                        {
                            this.HandleKeyPressMonth(part, e);
                        }
                    }
                    
                    part.Validate();   
                    MaskPart dayPart = this.SetDayMaxValue(this.list);
                    int dayPartValue = this.value.Day;
                    if (dayPart != null)
                    {
                        dayPartValue = dayPart.value;
                    }

                    this.Value = new DateTime(this.value.Year, part.value, dayPartValue, this.value.Hour, this.value.Minute, this.value.Second);
                    
                    break;

                case PartTypes.Seconds:
                    this.HandleKeyPress(part, e);
                    part.Validate();   
                    this.Value = new DateTime(this.value.Year, this.value.Month, this.value.Day, this.value.Hour, this.value.Minute, part.value);
                    break;
                case PartTypes.AmPm:                    
                    char am = culture.DateTimeFormat.AMDesignator.ToLower()[0];
                    char pm = culture.DateTimeFormat.PMDesignator.ToLower()[0];
                    if (am == char.ToLower(e.KeyChar))
                    { 	
                        if (value.Hour > 12)
                        {
                            this.Value = this.value.AddHours(-12);
                        }
                    }
                    else if (pm == char.ToLower(e.KeyChar))
                    {
                        if (value.Hour < 12)
                        {
                            this.Value = this.value.AddHours(12);
                        }
                    }

                    break;
                case PartTypes.Year:                    
                    this.HandleKeyPress(part, e);
                    part.Validate(); 
                    MaskPart dayPart1 = this.SetDayMaxValue(this.list);
                    int dayPartValue1 = this.value.Day;
                    if (dayPart1 != null)
                    {
                        dayPartValue1 = dayPart1.value;
                    }
                    this.Value = new DateTime(part.value, this.value.Month, dayPartValue1, this.value.Hour, this.value.Minute, this.value.Second);
                    break;
                case PartTypes.Character:
                    break;
                default:
                    break;
            }
            e.Handled = true;
            
            this.FillCollectionWithValues(this.list, this.value, this.mask); 
            this.RestoreSelectedItem();       
        }

        public void ResetCurrentPartValue(object sender, KeyEventArgs e)
        {
            this.SelectCurrentItemFromCurrentCaret();
            MaskPart part = this.list[this.selectedItemIndex];

            if (part.readOnly)
            {
                e.Handled = true;
                return;
            }

            this.ResetOnDelPartValue(part, e.KeyValue);
            switch (part.type)
            {
                case PartTypes.ReadOnly:
                    break;
                case PartTypes.Day:                
                    this.SetDayMaxValue(this.list);
                    part.Validate();
                    this.Value = new DateTime(this.value.Year, this.value.Month, part.value, this.value.Hour, this.value.Minute, this.value.Second);
                    break;
                case PartTypes.MiliSeconds:
                    break;
                case PartTypes.h12:
                case PartTypes.h24:                  
                    part.Validate();
                    this.Value = new DateTime(this.value.Year, this.value.Month, this.value.Day, part.value, this.value.Minute, this.value.Second);
                    break;
                case PartTypes.Minutes:                
                    part.Validate();
                    this.Value = new DateTime(this.value.Year, this.value.Month, this.value.Day, this.value.Hour, part.value, this.value.Second);
                    break;
                case PartTypes.Month:
                    part.Validate();
                    MaskPart dayPart = this.SetDayMaxValue(this.list);
                    int dayPartValue = this.value.Day;
                    if (dayPart != null)
                    {
                        dayPartValue = dayPart.value;
                    }

                    this.Value = new DateTime(this.value.Year, part.value, dayPartValue, this.value.Hour, this.value.Minute, this.value.Second);
                    break;
                case PartTypes.Seconds:                  
                    part.Validate();
                    this.Value = new DateTime(this.value.Year, this.value.Month, this.value.Day, this.value.Hour, this.value.Minute, part.value);
                    break;
                case PartTypes.AmPm:
                    break;
                case PartTypes.Year:                   
                    part.Validate();
                    MaskPart dayPart1 = this.SetDayMaxValue(this.list);
                    int dayPartValue1 = this.value.Day;
                    if (dayPart1 != null)
                    {
                        dayPartValue1 = dayPart1.value;
                    }
                    this.Value = new DateTime(part.value, this.value.Month, dayPartValue1, this.value.Hour, this.value.Minute, this.value.Second);
                    break;
                case PartTypes.Character:
                    break;
                default:
                    break;
            }

            e.Handled = true;

            this.FillCollectionWithValues(this.list, this.value, this.mask);
            this.RestoreSelectedItem();
        }

        private void HandleKeyPressWithCharacters(MaskPart part, KeyPressEventArgs e, string[] names)
        {
            string lowerInput = e.KeyChar.ToString().ToLower();
            for (int i = part.value; i < names.Length; ++i)
            {
                if (names[i].ToLower().StartsWith(lowerInput))
                {
                    part.value = i + 1;
                    return;
                }
            }

            for (int i = 0; i < part.value; ++i)
            {
                if (names[i].ToLower().StartsWith(lowerInput))
                {
                    part.value = i + 1;
                    return;
                }
            }
        }

        private void HandleKeyPressMonth(MaskPart part, KeyPressEventArgs e)
        {
            this.HandleKeyPressDay(part, e);            
        }

        private void HandleKeyPressDay(MaskPart part, KeyPressEventArgs e) 
        {
            int value = 0;
            if (!int.TryParse(e.KeyChar.ToString(), out value))
            {
                return;
            }

            string stringValue = part.value.ToString();
            if (stringValue.Length >= part.max.ToString().Length)
            {
                part.value = 0;// part.min;
                part.hasZero = true;
            }
            
            int tempValue = 0;
            if (part.hasZero)
            {
                if (tempValue < part.min || tempValue > part.max)
                {
                    tempValue = value;
                }
            }
            else
            {
                if (stringValue.Length > 1)
                {
                    stringValue = stringValue.Substring(1);
                }

                tempValue = int.Parse(stringValue + e.KeyChar);
            }

            if (tempValue < part.min || tempValue > part.max)
            {
                part.value = value;
            }
            else
            {
                part.value = tempValue;
            }

            part.hasZero = value == 0;
        }

        private void HandleKeyPress(MaskPart part, KeyPressEventArgs e)
        {
            int value = 0;
            if (!int.TryParse(e.KeyChar.ToString(), out value))
            {
                return;
            }

            string stringValue = part.value.ToString();
            if (stringValue.Length >= part.max.ToString().Length)
            {
                part.value = 0;// part.min;
                part.hasZero = true;
                stringValue = "";
            }
             
            if (stringValue.Length == part.len && stringValue.Length >= part.maskPart.Length)
            {
                stringValue = stringValue.Substring(1);
            }

            part.value = int.Parse(stringValue + e.KeyChar);            
            if (part.value > part.max || part.value < part.min)
            {
                part.value = value;
            }
        }

        private void HandleKeyPressHour(MaskPart part, KeyPressEventArgs e)
        {
            int value = 0;
            if (!int.TryParse(e.KeyChar.ToString(), out value))
            {
                return;
            }

            string stringValue = part.value.ToString();
            if (stringValue.Length == 2)
            {
                stringValue = stringValue.Substring(1);
            }

            part.value = int.Parse(stringValue + e.KeyChar);
            if (part.value > part.max || part.value < part.min)
            {
                part.value = value;
            }
        }

        private bool ResetOnDelPartValue(MaskPart part, int keyChar)
        {
            if (keyChar == 8)
            {

                if (part.type == PartTypes.Year)
                {
                    part.value = this.YearResetValue;
                }
                else
                {
                    part.value = part.min;
                }

                this.SelectPrevItem();
                return true;
            }

            if (keyChar == 46)
            {
                if (part.type == PartTypes.Year)
                {
                    part.value = this.YearResetValue;
                }
                else
                {
                    part.value = part.min;
                }

                return true;
            }

            return false;
        }

        public void HandleSpinUp(DateTime minDate, DateTime maxDate)
        {
            this.SelectCurrentItemFromCurrentCaret();
            MaskPart part = this.list[this.selectedItemIndex];
            this.Up(part,minDate ,maxDate);
        }

        public void HandleSpinDown(DateTime minDate, DateTime maxDate)
        {
            this.SelectCurrentItemFromCurrentCaret();
            MaskPart part = this.list[this.selectedItemIndex];
            this.Down(part, minDate, maxDate);
        }

        private void RestoreSelectedItem()
        {
            textBoxItem.Text = this.value.ToString(this.mask, this.culture);
            int oldSelectedItem = this.selectedItemIndex;
            this.FillCollectionWithValues(this.list, this.value, this.mask);
            this.selectedItemIndex = oldSelectedItem;
            this.SelectCurrentItemWithSelectedItem();
        }

        public void Up(MaskPart part, DateTime minDate, DateTime maxDate)
        {
            if (part.readOnly)
            {
                return;
            }

            try
            {
                if (value < minDate)
                {
                    value = minDate;
                }

                DateTime resultValue = DateTime.MaxValue;
                switch (part.type)
                {
                    case PartTypes.Year:
                        resultValue = this.value.AddYears(1);
                        break;
                    case PartTypes.Month:
                        resultValue = this.value.AddMonths(1);
                        break;
                    case PartTypes.Day:
                        resultValue = this.value.AddDays(1);
                        break;
                    case PartTypes.h12:
                    case PartTypes.h24:
                        resultValue = this.value.AddHours(1);
                        break;
                    case PartTypes.Minutes:
                        resultValue = this.value.AddMinutes(1);
                        break;
                    case PartTypes.Seconds:
                        resultValue = this.value.AddSeconds(1);
                        break;
                    case PartTypes.MiliSeconds:
                        resultValue = this.value.AddMilliseconds(1);
                        break;
                    case PartTypes.AmPm:
                        if (value.Hour >= 12)
                        {
                            resultValue = this.value.AddHours(-12);
                        }
                        else if (value.Hour < 12)
                        {
                            resultValue = this.value.AddHours(12);
                        }

                        break;
                }

                if (resultValue <= maxDate)
                {
                    this.Value = resultValue;
                }
            }
            catch (ArgumentOutOfRangeException) 
            {
            }
            this.FillCollectionWithValues(this.list, this.value, this.mask);
            this.RestoreSelectedItem();         
        }

        public void Down(MaskPart part, DateTime minDate, DateTime maxDate)
        {
            if (part.readOnly)
            {
                return;
            }
            try
            {
                DateTime resultValue = DateTime.MinValue;

                if (value > maxDate)
                {
                    value = maxDate;
                }

                switch (part.type)
                {
                    case PartTypes.Year:
                        resultValue = this.value.AddYears(-1);
                        break;
                    case PartTypes.Month:
                        resultValue = this.value.AddMonths(-1);
                        break;
                    case PartTypes.Day:
                        resultValue = this.value.AddDays(-1);
                        break;
                    case PartTypes.h12:
                    case PartTypes.h24:
                        resultValue = this.value.AddHours(-1);
                        break;
                    case PartTypes.Minutes:
                        resultValue = this.value.AddMinutes(-1);
                        break;
                    case PartTypes.Seconds:
                        resultValue = this.value.AddSeconds(-1);
                        break;
                    case PartTypes.MiliSeconds:
                        resultValue = this.value.AddMilliseconds(-1);
                        break;
                    case PartTypes.AmPm:                        
                        if (value.Hour > 12)
                        {
                            resultValue = this.value.AddHours(-12);
                        }
                        else if (value.Hour <= 12)
                        {
                            resultValue = this.value.AddHours(12);
                        }

                        break;
                }

                if (resultValue >= minDate)
                {
                    this.Value = resultValue;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
            }

            this.FillCollectionWithValues(this.list, this.value, this.mask);
            this.RestoreSelectedItem();
        }

        public void SelectCurrentItemWithSelectedItem()
        {
            for (int i = this.selectedItemIndex; i < this.list.Count; ++i)
            {
                MaskPart part = this.list[i];
                if (part.type != PartTypes.Character && !part.readOnly)
                {
                    this.textBoxItem.SelectionStart = part.offset;
                    this.textBoxItem.SelectionLength = part.len;
                    this.selectedItemIndex = i;
                    break;
                }
            }
        }

        public bool SelectCurrentItemFromCurrentCaret()
        {
            int currentSelection = this.textBoxItem.SelectionStart;
            int currentPos = 0;
            bool selected = false;
            for (int i = 0; i < this.list.Count; ++i)
            {
                MaskPart part = this.list[i];               
                if (currentSelection >= part.offset && currentSelection <= part.offset + part.len && !part.readOnly && part.type != PartTypes.Character)
                {
                    this.textBoxItem.SelectionStart = this.list[i].offset;
                    this.textBoxItem.SelectionLength = this.list[i].len;
                    this.selectedItemIndex = i;
                    selected = true;
                    break;
                }

                currentPos += part.len;
            }

            return selected;
        }

        public bool SelectNextEditableItemFromCurrentCaret()
        {
            int currentSelection = this.textBoxItem.SelectionStart;
            int currentPos = 0;
            bool selected = false;
            for (int i = 0; i < this.list.Count; ++i)
            {
                MaskPart part = this.list[i];
                if (currentSelection >= part.offset && currentSelection <= part.offset + part.len)
                {                   
                    this.selectedItemIndex = i;
                    selected = true;
                    this.SelectNextEditableItem();
                    break;
                }

                currentPos += part.len;
            }

            return selected;
        }

        public void SelectPrevItemFromCurrentCaret()
        {
            int currentSelection = this.textBoxItem.SelectionStart + this.textBoxItem.SelectionLength;
            int currentPos = 0;
            for (int i = 1; i < this.list.Count; ++i)
            {
                MaskPart part = this.list[i];
                currentPos += part.len;
                if (currentSelection >= part.offset && currentSelection <= part.offset + part.len)
                {
                    this.textBoxItem.SelectionStart = this.list[i - 1].offset;
                    this.textBoxItem.SelectionLength = this.list[i - 1].len;
                    this.selectedItemIndex = i - 1;
                    break;
                }
            }
        }

        public void SelectNextItemFromCurrentCaret()
        {
            int currentSelection = this.textBoxItem.SelectionStart + this.textBoxItem.SelectionLength;
            int currentPos = 0;
            for (int i = 0; i < this.list.Count - 1; ++i)
            {
                MaskPart part = this.list[i];
                currentPos += part.len;
                if (currentSelection >= part.offset && currentSelection <= part.offset + part.len)
                { 
                    this.textBoxItem.SelectionStart = this.list[i + 1].offset;
                    this.textBoxItem.SelectionLength = this.list[i + 1].len;
                    this.selectedItemIndex = i + 1;
                    break; 
                }
            }
        }

        public void SelectPrevItem()
        { 
            for (int i = this.selectedItemIndex - 1; i >= 0; --i)
            {
                MaskPart part = this.list[i];                
                if (part.type != PartTypes.Character && !part.readOnly)
                {
                    this.textBoxItem.SelectionStart = part.offset;
                    this.textBoxItem.SelectionLength = part.len;
                    this.selectedItemIndex = i;
                    break;
                }
            }
        }

        public void SelectNextItem()
        {
            for (int i = this.selectedItemIndex + 1; i < this.list.Count; ++i)
            {
                MaskPart part = this.list[i];
                if (part.type != PartTypes.Character && !part.readOnly)
                {
                    this.textBoxItem.SelectionStart = part.offset;
                    this.textBoxItem.SelectionLength = part.len;
                    this.selectedItemIndex = i;
                    break;
                }
            }
        }

        public virtual void SelectLastItem()
        {
            this.selectedItemIndex = this.list.Count - 1;
            if (this.list[selectedItemIndex].type != PartTypes.Character && !this.list[selectedItemIndex].readOnly)
            {
                this.textBoxItem.SelectionStart = this.list[selectedItemIndex].offset;
                this.textBoxItem.SelectionLength = this.list[selectedItemIndex].len;
            }
        }

        public virtual void SelectFirstItem()
        { 
            this.selectedItemIndex = 0;
            if (this.list[selectedItemIndex].type != PartTypes.Character && !this.list[selectedItemIndex].readOnly)
            {
                this.textBoxItem.SelectionStart = this.list[selectedItemIndex].offset;
                this.textBoxItem.SelectionLength = this.list[selectedItemIndex].len;
            }
        }

        public virtual void SelectFirstEditableItem()
        {
            int oldSelectedIndex = -1;
            this.SelectFirstItem();
            while (this.list[selectedItemIndex].readOnly && selectedItemIndex != oldSelectedIndex)
            {
                this.SelectNextItem();
                oldSelectedIndex = selectedItemIndex;
            }
        }

        public virtual void SelectLastEditableItem()
        {
            int oldSelectedIndex = -1;
            this.SelectLastItem();
            while (this.list[selectedItemIndex].readOnly && selectedItemIndex != oldSelectedIndex)
            {
                this.SelectPrevEditableItem();
                oldSelectedIndex = selectedItemIndex;
            }
        }



        public virtual void SelectPrevEditableItem()
        {
            int oldSelectedIndex = -1;
            this.SelectPrevItem();
            while (this.list[selectedItemIndex].readOnly && selectedItemIndex != oldSelectedIndex)
            {
                this.SelectPrevItem();
                oldSelectedIndex = selectedItemIndex;
            }
        }

        public virtual void SelectNextEditableItem()
        {
            int oldSelectedIndex = -1;
            this.SelectNextItem();
            while (this.list[selectedItemIndex].readOnly && selectedItemIndex != oldSelectedIndex)
            {
                this.SelectNextItem();
                oldSelectedIndex = selectedItemIndex;
            }
        }

        public bool Click()
        {
            bool selected = this.SelectCurrentItemFromCurrentCaret();
            if (!selected)
            {
                selected = this.SelectNextEditableItemFromCurrentCaret();
            }
            DateTime dateTime;
            if (DateTime.TryParse(this.textBoxItem.Text, culture, DateTimeStyles.None, out dateTime))
            { 
                return true;
            }
            
            return false;
        }

        public bool Validate(string stringValue)
        {
            CancelEventArgs eventArgs = new CancelEventArgs();
            this.owner.CallValueChanging(eventArgs);
            if (eventArgs.Cancel)
            {
                return false;
            }

            if (DateTime.TryParse(stringValue, culture, DateTimeStyles.None, out this.value))
            {
                textBoxItem.Text = this.value.ToString(this.mask, this.culture);
                this.FillCollectionWithValues(this.list, this.value, this.mask);
                this.owner.CallValueChanged(EventArgs.Empty);
                return true;
            }
             
            textBoxItem.Text = DateTime.Now.ToString(this.mask, this.culture);
            this.owner.CallValueChanged(EventArgs.Empty);
            return false;            
        }

        public RadTextBoxItem TextBoxItem
        {
            get
            {
                return this.textBoxItem;
            }
        }

        public List<MaskPart> List
        {
            get
            {
                return list;
            }
            set
            {
                list = value;
            }
        }

        public string ToString(bool includePromt, bool includeLiterals)
        {
            return this.value.ToString(this.mask, culture);
        }

        public IMaskProvider Clone()
        {
            return new MaskDateTimeProvider(this.mask, this.culture, this.owner);
        }

        public CultureInfo Culture
        {
            get
            {
                return this.culture;
            }
        }

        public string Mask
        {
            get
            {
                return this.mask;
            }
        }

        public bool IncludePrompt
        {
            get
            {
                return this.includePrompt;
            }
            set
            {
                this.includePrompt = value;
            }
        }

        public char PromptChar
        {
            get
            {
                return this.promptChar;
            }
            set
            {
                this.promptChar = value;
            }
        }

        static string GetSpecificFormat(string format, DateTimeFormatInfo info)
        {
            if (format == null || format.Length == 0)
            {
                format = "G";
            }

            if (format.Length == 1)
            {
                switch (format[0])
                {
                    case 'd':
                        return info.ShortDatePattern;
                    case 'D':
                        return info.LongDatePattern;
                    case 't':
                        return info.ShortTimePattern;
                    case 'T':
                        return info.LongTimePattern;
                    case 'f':
                        return info.LongDatePattern + ' ' + info.ShortTimePattern;
                    case 'F':
                        return info.FullDateTimePattern;
                    case 'g':
                        return info.ShortDatePattern + ' ' + info.ShortTimePattern;
                    case 'G':
                        return info.ShortDatePattern + ' ' + info.LongTimePattern;
                    case 'm':
                    case 'M':
                        return info.MonthDayPattern;
                    case 'r':
                    case 'R':
                        return info.RFC1123Pattern;
                    case 's':
                        return info.SortableDateTimePattern;
                    case 'u':
                        return info.UniversalSortableDateTimePattern;
                    case 'y':
                    case 'Y':
                        return info.YearMonthPattern;
                }
            }

            if (format.Length == 2 && format[0] == '%')
            {
                format = format.Substring(1);
            }

            return format;
        }

        static int GetGroupLengthByMask(string mask)
        {
            for (int i = 1; i < mask.Length; ++i)
            {
                if (mask[i] != mask[0])
                {
                    return i;
                }
            }

            return mask.Length;
        }

        protected virtual void FillCollectionWithValues(List<MaskPart> collection, DateTime dateTime, string mask)
        { 
            for (int i = 0; i < collection.Count;++i)
            {
                MaskPart part = collection[i];
                switch (part.type)
                {
                    case PartTypes.ReadOnly:
                        part.len = part.maskPart.Length;
                        if (part.len < part.value.ToString().Length)
                        {
                            part.len = part.value.ToString().Length;
                        }
                        break;
                    case PartTypes.Day:

                        part.value = dateTime.Day;
                        part.len = part.maskPart.Length;
                        if (part.len < part.value.ToString().Length)
                        {
                            part.len = part.value.ToString().Length;
                        }

                        if (part.maskPart.Length == 3)
                        {
                            part.len = dateTimeFormatInfo.AbbreviatedDayNames[(int)dateTime.DayOfWeek].Length;
                        }
                        else if (part.maskPart.Length > 3)
                        {
                            part.len = dateTimeFormatInfo.DayNames[(int)dateTime.DayOfWeek].Length;
                        }

                        break;
                    case PartTypes.MiliSeconds:
                        part.value = dateTime.Millisecond;
                        part.len = part.value.ToString().Length;
                        part.max = 59;
                        break;
                    case PartTypes.h12:
                        int initialValue = dateTime.Hour % 12;
                        if (initialValue == 0)
                        {
                            initialValue = 12;
                        }
                        part.value = initialValue;
                        part.len = part.maskPart.Length;
                        if (part.len < part.value.ToString().Length)
                        {
                            part.len = part.value.ToString().Length;
                        }
                        part.max = 12;
                        break;
                    case PartTypes.h24:
                        part.value = dateTime.Hour;
                        part.len = part.maskPart.Length;
                        if (part.len < part.value.ToString().Length)
                        {
                            part.len = part.value.ToString().Length;
                        }
                        part.max = 23;
                        break;
                    case PartTypes.Minutes:
                        part.value = dateTime.Minute;
                        part.len = part.maskPart.Length;
                        if (part.len < part.value.ToString().Length)
                        {
                            part.len = part.value.ToString().Length;
                        }
                        part.max = 59;
                        break;
                    case PartTypes.Month:
                        part.value = dateTime.Month;
                        part.len = part.maskPart.Length;

                        if (part.len < part.value.ToString().Length)
                        {
                            part.len = part.value.ToString().Length;
                        }

                        if (part.maskPart.Length == 3)
                        {
                            part.len = dateTimeFormatInfo.AbbreviatedMonthNames[part.value - 1].Length;
                        }
                        else if (part.maskPart.Length > 3)
                        {
                            part.len = dateTimeFormatInfo.MonthNames[part.value - 1].Length;
                        }
                        part.min = 1;
                        part.max = 12;
                        break;
                    case PartTypes.Seconds:
                        part.value = dateTime.Second;
                        part.len = part.maskPart.Length;
                        if (part.len < part.value.ToString().Length)
                        {
                            part.len = part.value.ToString().Length;
                        }
                        part.max = 59;
                        break;
                    case PartTypes.AmPm:
                        part.value = dateTime.Hour / 12;
                        part.len = part.maskPart.Length;
                        if (part.len < part.value.ToString().Length)
                        {
                            part.len = part.value.ToString().Length;
                        }
                        break;
                    case PartTypes.Year:
                       
                        part.value = dateTime.Year;
                        part.len = part.maskPart.Length;
                        if (part.maskPart == "yyy" || part.maskPart == "yy" || part.maskPart == "y")
                        {
                            part.len = 4;
                        }

                        if (part.len < part.value.ToString().Length)
                        {
                            part.len = part.value.ToString().Length;
                        }
                        part.min = DateTime.MinValue.Year;
                        part.max = DateTime.MaxValue.Year;                       

                        break;
                    case PartTypes.Character:                              
                        break;
                    default:
                        break;
                }
            }

            this.AdjustItemsPossitionOffset(collection);
            this.SetDayMaxValue(collection);            
        }

        private void AdjustItemsPossitionOffset(List<MaskPart> collection)
        {
            int offset = 0;
            for (int i = 0; i < collection.Count; ++i)
            { 
                collection[i].offset = offset;          
                offset += collection[i].len;
            }
        }

        protected virtual MaskPart SetDayMaxValue(List<MaskPart> collection)
        {
            int year = DateTime.Now.Year;
            int month = DateTime.Now.Month;
            MaskPart dayPart = null;
            for (int i = 0;i < collection.Count;++i)
            {
                if (collection[i].type == PartTypes.Day)
                {
                    dayPart = collection[i];
                }

                if (collection[i].type == PartTypes.Year)
                {
                    year = collection[i].value;
                }

                if (collection[i].type == PartTypes.Month)
                {
                    month = collection[i].value;
                }
            }

            if (dayPart != null)
            { 
                if (month == 0)
                {
                    month = 1;
                }

                dayPart.max = DateTime.DaysInMonth(year, month);
            }

            if (dayPart != null)
            {
                dayPart.Validate();
            }

            return dayPart;
        }
 
        static List<MaskPart> FillCollection(string mask, DateTimeFormatInfo dateTimeFormatInfo)
        {
            List<MaskPart> result = new List<MaskPart>();          
            string currentMask = mask;
            int len = 0;         
            while (currentMask.Length > 0)
            {
                int elementLength = GetGroupLengthByMask(currentMask);
                MaskPart element = new MaskPart();
                switch (currentMask[0])
                {
                    case 'd':
                    case 'D':
                        if (elementLength > 2)
                        {
                            element.readOnly = true;
                            element.type = PartTypes.Day;
                            element.maskPart = currentMask.Substring(0, elementLength);
                        }
                        else
                        {
                            element.month = true;
                            element.year = true;
                            element.day = true;
                            element.type = PartTypes.Day;
                            element.min = 1;                            
                            element.maskPart = currentMask.Substring(0, elementLength);                            
                        }
                        break;
                    case 'f':
                    case 'F':
                        if (elementLength > 7)
                        {
                            elementLength = 7;
                        }

                        if (elementLength > 3)
                        {
                            element.readOnly = true;
                            element.maskPart = currentMask.Substring(0, elementLength);
                        }
                        else
                        {
                            element.type = PartTypes.MiliSeconds;
                            element.maskPart = currentMask.Substring(0, elementLength);
                        }
                        break;
                    case 'g':
                        element.readOnly = true;
                        element.maskPart = currentMask.Substring(0, elementLength);
                        break;
                    case 'h':
                        element.type = PartTypes.h12;
                        element.maskPart = currentMask.Substring(0, elementLength);
                        break;
                    case 'H':
                        element.type = PartTypes.h24;
                        element.maskPart = currentMask.Substring(0, elementLength);                        
                        break;
                    case 'm':
                        element.type = PartTypes.Minutes;
                        element.maskPart = currentMask.Substring(0, elementLength);
                        break;
                    case 'M':
                        if (elementLength > 4)
                        {
                            elementLength = 4;
                        }

                        element.type = PartTypes.Month;
                        element.maskPart = currentMask.Substring(0, elementLength);
                        element.month = true;
                        break;
                    case 's':
                    case 'S':
                        element.type = PartTypes.Seconds;
                        element.maskPart = currentMask.Substring(0, elementLength);
                        break;
                    case 't':
                    case 'T':
                        element.type = PartTypes.AmPm;
                        element.maskPart = currentMask.Substring(0, elementLength);
                        break;
                    case 'y':
                    case 'Y':
                        element.type = PartTypes.Year;
                        element.maskPart = currentMask.Substring(0, elementLength);
                        element.year = true;
                        break;
                    case 'z':
                        element.readOnly = true;
                        element.maskPart = currentMask.Substring(0, elementLength);
                        break;
                    case ':':
                    case '/':
                        elementLength = 1;
                        element.readOnly = true;
                        element.maskPart = currentMask.Substring(0, 1);
                        break;
                    case '"':
                    case '\'':
                        int closingQuotePosition = currentMask.IndexOf(currentMask[0], 1);
                        element.type = PartTypes.Character;
                        element.maskPart = currentMask.Substring(1, Math.Max(1, closingQuotePosition - 1));
                        elementLength = Math.Max(1, closingQuotePosition + 1);
                        break;
                    case '\\':
                        if (currentMask.Length >= 2)
                        {
                            element.type = PartTypes.Character;
                            element.maskPart = currentMask.Substring(1, 1);
                            elementLength = 1;
                        }

                        break;
                    default:
                        elementLength = 1;
                        element.type = PartTypes.Character;
                        element.maskPart = currentMask.Substring(0, 1);                        
                        break;
                }
                element.offset = len;
                len += element.maskPart.Length;
                result.Add(element);
                currentMask = currentMask.Substring(elementLength);
                element.len = element.maskPart.Length;
            }

            return result;
        }

        public bool Delete()
        {
            this.ResetCurrentPartValue(this, new KeyEventArgs(Keys.Delete));
            return true;
        }
    }
}