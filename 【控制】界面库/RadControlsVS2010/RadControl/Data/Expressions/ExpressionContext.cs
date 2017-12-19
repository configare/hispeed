namespace Telerik.Data.Expressions
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public class ExpressionContext : DictionaryObject, IDisposable
    {
        #region Static members

        private static ExpressionContext context;

        /// <summary>
        /// Set or get default expression context class, which will be used for determinating the expression functions.
        /// </summary>
        public static ExpressionContext Context
        {
            get
            {
                if (ExpressionContext.context == null)
                {
                    ExpressionContext.context = new ExpressionContext();
                }
                return ExpressionContext.context;
            }

            set
            {
                ExpressionContext.context = value;
            }
        }

        #endregion

        #region Fields        

        DateTime execTime = DateTime.Now;
        IFormatProvider formatProvider = CultureInfo.CurrentCulture;

        #endregion

        #region Constructors

        public ExpressionContext() {}

        #endregion

        #region Methods

        public virtual void Dispose()
        {
            this.formatProvider = null;
        }

        #endregion

        #region DateTime Functions

        [Description("Returns the current date and time on this computer, expressed as the local time.")]
        public virtual DateTime Now()
        {
            return this.execTime;
        }

        [Description("Returns the current date. Regardless of the actual time, this function returns midnight of the current date.")]
        public virtual DateTime Today()
        {
            return this.execTime.Date;
        }

        [Description("Returns a date-time value that is the specified number of days away from the specified DateTime.")]
        public virtual DateTime AddDays(object value, object daysToAdd)
        {
            DateTime convertedValue = Convert.ToDateTime(value);
            double convertedDaysToAdd = Convert.ToDouble(daysToAdd);

            return convertedValue.AddDays(convertedDaysToAdd);
        }

        [Description("Returns a date-time value that is the specified number of hours away from the specified DateTime.")]
        public virtual DateTime AddHours(object value, object hoursToAdd)
        {
            DateTime convertedValue = Convert.ToDateTime(value);
            double convertedHoursToAdd = Convert.ToDouble(hoursToAdd);

            return convertedValue.AddHours(convertedHoursToAdd);
        }

        [Description("Returns a date-time value that is the specified number of milliseconds away from the specified DateTime.")]
        public virtual DateTime AddMilliseconds(object value, object millisecondsToAdd)
        {
            DateTime convertedValue = Convert.ToDateTime(value);
            double convertedMillisecondsToAdd = Convert.ToDouble(millisecondsToAdd);

            return convertedValue.AddMilliseconds(convertedMillisecondsToAdd);
        }

        [Description("Returns a date-time value that is the specified number of minutes away from the specified DateTime.")]
        public virtual DateTime AddMinutes(object value, object minutesToAdd)
        {
            DateTime convertedValue = Convert.ToDateTime(value);
            double convertedMinutesToAdd = Convert.ToDouble(minutesToAdd);

            return convertedValue.AddMinutes(convertedMinutesToAdd);
        }

        [Description("Returns a date-time value that is the specified number of months away from the specified DateTime.")]
        public virtual DateTime AddMonths(object value, object monthsToAdd)
        {
            DateTime convertedValue = Convert.ToDateTime(value);
            int convertedMonthsToAdd = Convert.ToInt32(monthsToAdd);

            return convertedValue.AddMonths(convertedMonthsToAdd);
        }

        [Description("Returns a date-time value that is the specified number of seconds away from the specified DateTime.")]
        public virtual DateTime AddSeconds(object value, object secondsToAdd)
        {
            DateTime convertedValue = Convert.ToDateTime(value);
            double convertedSecondsToAdd = Convert.ToDouble(secondsToAdd);

            return convertedValue.AddSeconds(convertedSecondsToAdd);
        }

        [Description("Returns a date-time value that is the specified number of ticks away from the specified DateTime.")]
        public virtual DateTime AddTicks(object value, object ticksToAdd)
        {
            DateTime convertedValue = Convert.ToDateTime(value);
            long convertedTicksToAdd = Convert.ToInt64(ticksToAdd);

            return convertedValue.AddTicks(convertedTicksToAdd);
        }

        [Description("Returns a date-time value that is away from the specified DateTime for the given TimeSpan.")]
        public virtual DateTime AddTimeSpan(object value, object timeSpan)
        {
            DateTime convertedValue = Convert.ToDateTime(value);
            TimeSpan cTimeSpan = (TimeSpan)timeSpan;

            return convertedValue.Add(cTimeSpan);
        }

        [Description("Returns a date-time value that is the specified number of years away from the specieid DateTime.")]
        public virtual DateTime AddYears(object value, object yearsToAdd)
        {
            DateTime convertedValue = Convert.ToDateTime(value);
            int convertedYearsToAdd = Convert.ToInt32(yearsToAdd);

            return convertedValue.AddYears(convertedYearsToAdd);
        }

        [Description("Returns the number of day boundaries between two non-nullable dates.")]
        public virtual double DateDiffDay(object startDate, object endDate)
        {
            DateTime cStartDate = Convert.ToDateTime(startDate);
            DateTime cEndDate = Convert.ToDateTime(endDate);

            return (cEndDate - cStartDate).TotalDays;
        }

        [Description("Returns the number of hour boundaries between two non-nullable dates.")]
        public virtual double DateDiffHour(object startDate, object endDate)
        {
            DateTime cStartDate = Convert.ToDateTime(startDate);
            DateTime cEndDate = Convert.ToDateTime(endDate);

            return (cEndDate - cStartDate).TotalHours;
        }

        [Description("Returns the number of millisecond boundaries between two non-nullable dates.")]
        public virtual double DateDiffMilliSecond(object startDate, object endDate)
        {
            DateTime cStartDate = Convert.ToDateTime(startDate);
            DateTime cEndDate = Convert.ToDateTime(endDate);

            return (cEndDate - cStartDate).TotalMilliseconds;
        }

        [Description("Returns the number of minute boundaries between two non-nullable dates.")]
        public virtual double DateDiffMinute(object startDate, object endDate)
        {
            DateTime cStartDate = Convert.ToDateTime(startDate);
            DateTime cEndDate = Convert.ToDateTime(endDate);

            return (cEndDate - cStartDate).TotalMinutes;
        }

        [Description("Returns the number of second boundaries between two non-nullable dates.")]
        public virtual double DateDiffSecond(object startDate, object endDate)
        {
            DateTime cStartDate = Convert.ToDateTime(startDate);
            DateTime cEndDate = Convert.ToDateTime(endDate);

            return (cEndDate - cStartDate).TotalSeconds;
        }

        [Description("Returns the number of tick boundaries between two non-nullable dates.")]
        public virtual long DateDiffTick(object startDate, object endDate)
        {
            DateTime cStartDate = Convert.ToDateTime(startDate);
            DateTime cEndDate = Convert.ToDateTime(endDate);

            return (cEndDate - cStartDate).Ticks;
        }

        [Description("Extracts a date from the defined DateTime.")]
        public virtual DateTime GetDate(object value)
        {
            DateTime convertedValue = Convert.ToDateTime(value);

            return convertedValue.Date;
        }

        [Description("Extracts a day from the defined DateTime.")]
        public virtual int GetDay(object value)
        {
            DateTime convertedValue = Convert.ToDateTime(value);

            return convertedValue.Day;
        }

        [Description("Extracts a day of the week from the defined DateTime.")]
        public virtual DayOfWeek GetDayOfWeek(object value)
        {
            DateTime convertedValue = Convert.ToDateTime(value);

            return convertedValue.DayOfWeek;
        }

        [Description("Extracts a day of the year from the defined DateTime.")]
        public virtual int GetDayOfYear(object value)
        {
            DateTime convertedValue = Convert.ToDateTime(value);

            return convertedValue.DayOfYear;
        }

        [Description("Extracts an hour from the defined DateTime.")]
        public virtual int GetHour(object value)
        {
            DateTime convertedValue = Convert.ToDateTime(value);

            return convertedValue.Hour;
        }

        [Description("Extracts milliseconds from the defined DateTime.")]
        public virtual int GetMilliSecond(object value)
        {
            DateTime convertedValue = Convert.ToDateTime(value);

            return convertedValue.Millisecond;
        }

        [Description("Extracts minutes from the defined DateTime.")]
        public virtual int GetMinute(object value)
        {
            DateTime convertedValue = Convert.ToDateTime(value);

            return convertedValue.Minute;
        }

        [Description("Extracts a month from the defined DateTime.")]
        public virtual int GetMonth(object value)
        {
            DateTime convertedValue = Convert.ToDateTime(value);

            return convertedValue.Month;
        }

        [Description("Extracts seconds from the defined DateTime.")]
        public virtual int GetSecond(object value)
        {
            DateTime convertedValue = Convert.ToDateTime(value);

            return convertedValue.Second;
        }

        [Description("Extracts the time of the day from the defined DateTime, in ticks.")]
        public virtual long GetTimeOfDay(object value)
        {
            DateTime convertedValue = Convert.ToDateTime(value);

            return convertedValue.TimeOfDay.Ticks;
        }

        [Description("Extracts a year from the defined DateTime.")]
        public virtual int GetYear(object value)
        {
            DateTime convertedValue = Convert.ToDateTime(value);

            return convertedValue.Year;
        }

        [Description("Returns the current system date and time, expressed as Coordinated Universal Time (UTC).")]
        public virtual DateTime UtcNow()
        {
            return this.execTime.ToUniversalTime();
        }
        

        #endregion

        #region Logical Functions

        [Description("Returns one of two objects, depending on the evaluation of an expression.")]
        public virtual object IIf(object expr, object truePart, object falsePart)
        {
            if (DataStorageHelper.ToBoolean(expr))
                return truePart;

            return falsePart;
        }

        [Description("Replaces the NULL with the specified replacement value.")]
        public virtual object IsNull(object value, object defaultValue)
        {
            if (null == value || DBNull.Value == value)
            {
                return defaultValue;
            }
            return value;
        }

        #endregion

        #region String Functions

        [Description("Returns a string representation of an object.")]
        public virtual string ToStr(object value)
        {
            return value.ToString();
        }

        [Description("Retrieves a substring from a string. The substring starts at a specified character position and has a specified length. ")]
        public virtual string Substr(string text, int startIndex, int length)
        {
            if (null == text)
            {
                text = string.Empty;
            }

            if (length < 0)
            {
                length = 0;
            }

            if (startIndex < 0)
            {
                startIndex = Math.Max(0, text.Length + startIndex);
            }

            // start index is beyond text length
            if (startIndex > (text.Length - 1))
            {
                return string.Empty;
            }

            length = Math.Min(startIndex + length, text.Length) - startIndex;
            return text.Substring(startIndex, length);
        }

        [Description("Replaces the format item in a specified System.String with the text equivalent of the value of a specified System.Object instance. ")]
        public virtual string Format(string format, object value)
        {
            if (DBNull.Value == value)
            {
                value = null;
            }
            return string.Format(format, value);
        }

        [Description("Removes all occurrences of white space characters from the beginning and end of this instance.")]
        public virtual string Trim(string text)
        {
            return text.Trim();
        }

        [Description("Gets the number of characters in a string.")]
        public virtual int Len(string text)
        {
            return text.Length;
        }

        [Description("Inserts String2 into String1 at the position specified by StartPositon.")]
        public virtual string Insert(string str1, int startPosition, string str2)
        {
            return str1.Insert(startPosition, str2);            
        }

        [Description("Returns the String in lowercase.")]
        public virtual string Lower(string str)
        {
            return str.ToLower();
        }

        [Description("Returns String in uppercase.")]
        public virtual string Upper(string str)
        {
            return str.ToUpper();            
        }

        [Description("Left-aligns characters in the defined string, padding its left side with white space characters up to a specified total length.")]
        public virtual string PadLeft(string str, int lenght)
        {
            return str.PadLeft(lenght);
        }

        [Description("Right-aligns characters in the defined string, padding its left side with white space characters up to a specified total length.")]
        public virtual string PadRight(string str, int lenght)
        {
            return str.PadRight(lenght);
        }

        [Description("Deletes a specified number of characters from this instance, beginning at a specified position.")]
        public virtual string Remove(string str1, int startPosition, int lenght)
        {
            return str1.Remove(startPosition, lenght);
        }

        [Description("Returns a copy of String1, in which SubString2 has been replaced with String3.")]
        public virtual string Replace(string str1, string subStr2, string str3)
        {
            return str1.Replace(subStr2, str3);            
        }

        #endregion

        #region Math Functions

        [Description("Returns the absolute value of a specified number.")]
        public virtual object Abs(object value)
        {
            if (null == value || DBNull.Value == value)
            {
                return null;
            }

            StorageType storageType = DataStorageHelper.GetStorageType(value.GetType());
            if (!DataStorageHelper.IsInteger(storageType))
            {
                if (!DataStorageHelper.IsNumeric(storageType))
                {
                    throw InvalidExpressionException.ArgumentTypeInteger("Abs", 1);
                }

                try
                {
                    return Math.Abs((decimal)value);
                }
                catch (Exception /*e*/)
                {
                    return Math.Abs((double)value);
                }
            }

            try
            {
                return Math.Abs((int)value);
            }
            catch (Exception /*e*/)
            {
                return Math.Abs((long)value);
            }
        }

        [Description("Returns the ceiling value of a specified number.")]
        public virtual object Ceiling(object value)
        {
            if (null == value || DBNull.Value == value)
            {
                return null;
            }

            StorageType storageType = DataStorageHelper.GetStorageType(value.GetType());
            if (!DataStorageHelper.IsNumeric(storageType))
            {
                return 0;
            }

            try
            {
                decimal input;
                if (Decimal.TryParse(value.ToString(), out input))
                {
                    return Math.Ceiling(input);
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        [Description("Returns the floor value of a specified number.")]
        public virtual object Floor(object value)
        {
            if (null == value || DBNull.Value == value)
            {
                return null;
            }

            StorageType storageType = DataStorageHelper.GetStorageType(value.GetType());
            if (!DataStorageHelper.IsNumeric(storageType))
            {
                return 0;
            }

            try
            {
                decimal input;
                if (Decimal.TryParse(value.ToString(), out input))
                {
                    return Math.Floor(input);
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        [Description("Returns the arccosine of a number (the angle, in radians, whose cosine is the given float expression).")]
        public virtual double Acos(object value)
        {
            double convertedValue = Convert.ToDouble(value);

            return Math.Acos(convertedValue);
        }

        [Description("Returns the arcsine of a number (the angle, in radians, whose sine is the given float expression).")]
        public virtual double Asin(object value)
        {
            double convertedValue = Convert.ToDouble(value);

            return Math.Asin(convertedValue);
        }

        [Description("Returns the arctangent of a number (the angle, in radians, whose tangent is the given float expression).")]
        public virtual double Atan(object value)
        {
            double convertedValue = Convert.ToDouble(value);

            return Math.Atan(convertedValue);
        }

        [Description("Returns an Int64 containing the full product of two specified 32-bit numbers.")]
        public virtual long BigMul(object value1, object value2)
        {
            int cValue1 = Convert.ToInt32(value1);
            int cValue2 = Convert.ToInt32(value2);

            return Math.BigMul(cValue1, cValue2);
        }

        [Description("Returns the cosine of the angle defined in radians.")]
        public virtual double Cos(object value)
        {
            double convertedValue = Convert.ToDouble(value);

            return Math.Cos(convertedValue);
        }

        [Description("Returns the hyperbolic cosine of the angle defined in radians.")]
        public virtual double Cosh(object value)
        {
            double convertedValue = Convert.ToDouble(value);

            return Math.Cosh(convertedValue);
        }

        [Description("Returns the exponential value of the given float expression.")]
        public virtual double Exp(object value)
        {
            double convertedValue = Convert.ToDouble(value);

            return Math.Exp(convertedValue);
        }

        [Description("Returns the natural logarithm of a specified number.")]
        public virtual double Log(object value)
        {
            double convertedValue = Convert.ToDouble(value);

            return Math.Log(convertedValue);
        }

        [Description("Returns the base 10 logarithm of a specified number.")]
        public virtual double Log10(object value)
        {
            double convertedValue = Convert.ToDouble(value);

            return Math.Log10(convertedValue);
        }

        [Description("Returns the maximum value from the specified values.")]
        public virtual object Max(object value1, object value2)
        {
            StorageType storageType1 = DataStorageHelper.GetStorageType(value1.GetType());
            StorageType storageType2 = DataStorageHelper.GetStorageType(value2.GetType());

            if (!(DataStorageHelper.IsInteger(storageType1) && DataStorageHelper.IsInteger(storageType2)))
            {
                if (!(DataStorageHelper.IsNumeric(storageType1) && DataStorageHelper.IsNumeric(storageType2)))
                {
                    throw InvalidExpressionException.ArgumentTypeInteger("Max", 1);
                }

                try
                {
                    return Math.Max((decimal)value1, (decimal)value2);
                }
                catch (Exception)
                {
                    return Math.Max((double)value1, (double)value2);
                }
            }

            try
            {
                return Math.Max((int)value1, (int)value2);
            }
            catch (Exception)
            {
                return Math.Max((long)value1, (long)value2);
            }            
        }

        [Description("Returns the minimum value from the specified values.")]
        public virtual object Min(object value1, object value2)
        {
            StorageType storageType1 = DataStorageHelper.GetStorageType(value1.GetType());
            StorageType storageType2 = DataStorageHelper.GetStorageType(value2.GetType());

            if (!(DataStorageHelper.IsInteger(storageType1) && DataStorageHelper.IsInteger(storageType2)))
            {
                if (!(DataStorageHelper.IsNumeric(storageType1) && DataStorageHelper.IsNumeric(storageType2)))
                {
                    throw InvalidExpressionException.ArgumentTypeInteger("Min", 1);
                }

                try
                {
                    return Math.Min((decimal)value1, (decimal)value2);
                }
                catch (Exception)
                {
                    return Math.Min((double)value1, (double)value2);
                }
            }

            try
            {
                return Math.Min((int)value1, (int)value2);
            }
            catch (Exception)
            {
                return Math.Min((long)value1, (long)value2);
            }
        }

        [Description("Returns a specified number raised to a specified power.")]
        public virtual double Power(object value, object power)
        {
            double convertedValue = Convert.ToDouble(value);
            double convertedPower = Convert.ToDouble(power);

            return Math.Pow(convertedValue, convertedPower);
        }

        [Description("Returns a random number that is less than 1, but greater than or equal to zero.")]
        public virtual double Rnd()
        {
            Random rdm = new Random();
            return rdm.NextDouble();
        }

        [Description("Rounds the given value to the nearest integer.")]
        public virtual decimal Round(object value)
        {
            decimal convertedValue = Convert.ToDecimal(value);

            return Math.Round(convertedValue);
        }

        [Description("Returns the positive (+1), zero (0), or negative (-1) sign of the given expression.")]
        public virtual int Sign(object value)
        {
            StorageType storageType = DataStorageHelper.GetStorageType(value.GetType());

            if (!DataStorageHelper.IsInteger(storageType))
            {
                if (!DataStorageHelper.IsNumeric(storageType))
                {
                    throw InvalidExpressionException.ArgumentTypeInteger("Sign", 1);
                }

                try
                {
                    return Math.Sign((decimal)value);
                }
                catch (Exception)
                {
                    return Math.Sign((double)value);
                }
            }

            try
            {
                return Math.Sign((int)value);
            }
            catch (Exception)
            {
                return Math.Sign((long)value);
            }
        }

        [Description("Returns the sine of the angle, defined in radians.")]
        public virtual double Sin(object value)
        {
            double convertedValue = Convert.ToDouble(value);
            return Math.Sin(convertedValue);
        }

        [Description("Returns the hyperbolic sine of the angle defined in radians.")]
        public virtual double Sinh(object value)
        {
            double convertedValue = Convert.ToDouble(value);
            return Math.Sinh(convertedValue);
        }

        [Description("Returns the square root of a given number.")]
        public virtual double Sqrt(object value)
        {
            double convertedValue = Convert.ToDouble(value);
            return Math.Sqrt(convertedValue);
        }

        [Description("Returns the tangent of the angle defined in radians.")]
        public virtual double Tan(object value)
        {
            double convertedValue = Convert.ToDouble(value);
            return Math.Tan(convertedValue);
        }

        [Description("Returns the hyperbolic tangent of the angle defined in radians.")]
        public virtual double Tanh(object value)
        {
            double convertedValue = Convert.ToDouble(value);
            return Math.Tanh(convertedValue);
        }

        #endregion

        #region Convert Functions

        [Description("Converts an expression to Integer value.")]
        public virtual object CInt(object value)
        {
            if (null == value || DBNull.Value == value)
            {
                return null;
            }

            return System.Convert.ToInt32(value, this.formatProvider);
        }

        [Description("Converts an expression to Double value.")]
        public virtual object CDbl(object value)
        {
            if (null == value || DBNull.Value == value)
            {
                return null;
            }

            return System.Convert.ToDouble(value, this.formatProvider);
        }

        [Description("Converts an expression to Boolean value.")]
        public virtual object CBool(object value)
        {
            if (DBNull.Value == value)
            {
                value = null;
            }

            try
            {
                return Convert.ToBoolean(value);
            }
            catch (Exception ex)
            {
                throw InvalidExpressionException.DatavalueConvertion(value, typeof(bool), ex);
            }
        }

        [Description("Converts an expression to Date value.")]
        public virtual object CDate(object value)
        {
            if (null == value || DBNull.Value == value)
            {
                return null;
            }

            return System.Convert.ToDateTime(value, this.formatProvider);
        }

        [Description("Converts an expression to string value.")]
        public virtual string CStr(object value)
        {
            if (null == value || DBNull.Value == value)
            {
                return null;
            }

            return System.Convert.ToString(value, this.formatProvider);
        }

        /*
        object Convert(params object[] args)
        {
            object value = args[0];
            Type type = (Type)args[1];

            if (DBNull.Value == value)
            {
                return DBNull.Value;
            }

            StorageType storageType = DataStorageHelper.GetStorageType(type);
            storageType = DataStorageHelper.GetStorageType(value.GetType());
            if (StorageType.Object == storageType)
            {
                return value;
            }
            if ((storageType == StorageType.Guid) && (storageType == StorageType.String))
            {
                return new Guid((string)value);
            }
            if (DataStorageHelper.IsFloatSql(storageType) && DataStorageHelper.IsIntegerSql(storageType))
            {
                if (StorageType.Single == storageType)
                {
                    return SqlConvert.ChangeType2((float)SqlConvert.ChangeType2(value, StorageType.Single, typeof(float), this.formatProvider), storageType, type, this.formatProvider);
                }
                if (StorageType.Double == storageType)
                {
                    return SqlConvert.ChangeType2((double)SqlConvert.ChangeType2(value, StorageType.Double, typeof(double), this.formatProvider), storageType, type, this.formatProvider);
                }
                if (StorageType.Decimal == storageType)
                {
                    return SqlConvert.ChangeType2((decimal)SqlConvert.ChangeType2(value, StorageType.Decimal, typeof(decimal), this.formatProvider), storageType, type, this.formatProvider);
                }
            }

            return SqlConvert.ChangeType2(value, storageType, type, this.formatProvider);
        }
        */

        #endregion
    }
}