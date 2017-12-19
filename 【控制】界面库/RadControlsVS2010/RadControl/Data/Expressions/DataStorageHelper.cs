namespace Telerik.Data.Expressions
{
    using System;
    using System.Data.SqlTypes;
    using System.Security.Permissions;
    using System.Collections.Generic;
    using Telerik.WinControls.Data;
    using System.ComponentModel;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    static class DataStorageHelper
    {

        static readonly Type[] types;

        static DataStorageHelper()
        {
            Type[] typeArray = new Type[0x27];
            typeArray[1] = typeof(object);
            typeArray[2] = typeof(DBNull);
            typeArray[3] = typeof(bool);
            typeArray[4] = typeof(char);
            typeArray[5] = typeof(sbyte);
            typeArray[6] = typeof(byte);
            typeArray[7] = typeof(short);
            typeArray[8] = typeof(ushort);
            typeArray[9] = typeof(int);
            typeArray[10] = typeof(uint);
            typeArray[11] = typeof(long);
            typeArray[12] = typeof(ulong);
            typeArray[13] = typeof(float);
            typeArray[14] = typeof(double);
            typeArray[15] = typeof(decimal);
            typeArray[0x10] = typeof(DateTime);
            typeArray[0x11] = typeof(TimeSpan);
            typeArray[0x12] = typeof(string);
            typeArray[0x13] = typeof(Guid);
            typeArray[20] = typeof(byte[]);
            typeArray[0x15] = typeof(char[]);
            typeArray[0x16] = typeof(Type);
            typeArray[0x17] = typeof(Uri);
            typeArray[0x18] = typeof(SqlBinary);
            typeArray[0x19] = typeof(SqlBoolean);
            typeArray[0x1a] = typeof(SqlByte);
            typeArray[0x1b] = typeof(SqlBytes);
            typeArray[0x1c] = typeof(SqlChars);
            typeArray[0x1d] = typeof(SqlDateTime);
            typeArray[30] = typeof(SqlDecimal);
            typeArray[0x1f] = typeof(SqlDouble);
            typeArray[0x20] = typeof(SqlGuid);
            typeArray[0x21] = typeof(SqlInt16);
            typeArray[0x22] = typeof(SqlInt32);
            typeArray[0x23] = typeof(SqlInt64);
            typeArray[0x24] = typeof(SqlMoney);
            typeArray[0x25] = typeof(SqlSingle);
            typeArray[0x26] = typeof(SqlString);

            DataStorageHelper.types = typeArray;
        }

        public static List<SortDescriptor> ParseSortString(string sortString)
        {
            List<SortDescriptor> list = new List<SortDescriptor>();
            char[] chArray = new char[] { ',' };
            string[] textArray = sortString.Split(chArray);
            for (int i = 0; i < textArray.Length; i++)
            {
                string sortText = textArray[i].Trim();
                int sortLength = sortText.Length;
                bool ascending = true;

                if ((sortLength >= 5) && (string.Compare(sortText, sortLength - 4, " ASC", 0, 4, true, System.Globalization.CultureInfo.InvariantCulture) == 0))
                {
                    sortText = sortText.Substring(0, sortLength - 4).Trim();
                }
                else if ((sortLength >= 6) && (string.Compare(sortText, sortLength - 5, " DESC", 0, 5, true, System.Globalization.CultureInfo.InvariantCulture) == 0))
                {
                    ascending = false;
                    sortText = sortText.Substring(0, sortLength - 5).Trim();
                }

                if (sortText.StartsWith("["))
                {
                    if (!sortText.EndsWith("]"))
                    {
                        throw new ArgumentException("Invalid sort expression");
                    }

                    sortText = sortText.Substring(1, sortText.Length - 2);
                }

                list.Add(new SortDescriptor(sortText, ascending ? ListSortDirection.Ascending : ListSortDirection.Descending));
            }

            return list;
        }

        public static List<GroupDescriptor> ParseGroupString(string groupString)
        {
            List<GroupDescriptor> list = new List<GroupDescriptor>();
            char[] chArray = new char[] { ';' };
            string[] textArray = groupString.Split(chArray);
            for (int i = 0; i < textArray.Length; i++)
            {

                string sortText = textArray[i].Trim();
                GroupDescriptor groupDescription = new GroupDescriptor(sortText);
                list.Add(groupDescription);
            }

            return list;
        }


        public static object SecureCreateInstance(Type type, object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (!type.IsPublic && !type.IsNestedPublic)
            {
                new ReflectionPermission(PermissionState.Unrestricted).Demand();
            }

            return Activator.CreateInstance(type, args);
        }

        /// <summary>
        /// 
        /// </summary>
        public static StorageType ResultSqlType(StorageType left
            , StorageType right
            , bool lc
            , bool rc
            , Operator op)
        {
            int leftOpPrecedence = (int)DataStorageHelper.GetPrecedence(left);
            if (leftOpPrecedence == 0)
            {
                return StorageType.Empty;
            }
            int rightOpPrecedence = (int)DataStorageHelper.GetPrecedence(right);
            if (rightOpPrecedence == 0)
            {
                return StorageType.Empty;
            }
            if (Operator.IsLogical(op))
            {
                if (((left != StorageType.Boolean) && (left != StorageType.SqlBoolean))
                    || ((right != StorageType.Boolean) && (right != StorageType.SqlBoolean)))
                {
                    return StorageType.Empty;
                }
                if ((left == StorageType.Boolean) && (right == StorageType.Boolean))
                {
                    return StorageType.Boolean;
                }
                return StorageType.SqlBoolean;
            }
            if (op == Operator.Plus)
            {
                if ((left == StorageType.SqlString) || (right == StorageType.SqlString))
                {
                    return StorageType.SqlString;
                }
                if ((left == StorageType.String) || (right == StorageType.String))
                {
                    return StorageType.String;
                }
            }
            if (((left == StorageType.SqlBinary) && (right != StorageType.SqlBinary)) || ((left != StorageType.SqlBinary) && (right == StorageType.SqlBinary)))
            {
                return StorageType.Empty;
            }
            if (((left == StorageType.SqlGuid) && (right != StorageType.SqlGuid)) || ((left != StorageType.SqlGuid) && (right == StorageType.SqlGuid)))
            {
                return StorageType.Empty;
            }
            if ((leftOpPrecedence > 0x13) && (rightOpPrecedence < 20))
            {
                return StorageType.Empty;
            }
            if ((leftOpPrecedence < 20) && (rightOpPrecedence > 0x13))
            {
                return StorageType.Empty;
            }
            if (leftOpPrecedence > 0x13)
            {
                if ((op == Operator.Plus) || (op == Operator.Minus))
                {
                    if (left == StorageType.TimeSpan)
                    {
                        return right;
                    }
                    if (right == StorageType.TimeSpan)
                    {
                        return left;
                    }
                    return StorageType.Empty;
                }
                if (!Operator.IsRelational(op))
                {
                    return StorageType.Empty;
                }
                return left;
            }
            DataTypePrecedence code = (DataTypePrecedence)Math.Max(leftOpPrecedence, rightOpPrecedence);
            StorageType precedenceType = DataStorageHelper.GetPrecedenceType(code);
            precedenceType = DataStorageHelper.GetPrecedenceType((DataTypePrecedence)DataStorageHelper.SqlResultType((int)code));
            if ((Operator.IsArithmetical(op) && (precedenceType != StorageType.String)) && ((precedenceType != StorageType.Char) && (precedenceType != StorageType.SqlString)))
            {
                if (!DataStorageHelper.IsNumericSql(left))
                {
                    return StorageType.Empty;
                }
                if (!DataStorageHelper.IsNumericSql(right))
                {
                    return StorageType.Empty;
                }
            }
            if ((op == Operator.Multiply) && DataStorageHelper.IsIntegerSql(precedenceType))
            {
                return StorageType.SqlDouble;
            }
            if (((precedenceType == StorageType.SqlMoney) && (left != StorageType.SqlMoney)) && (right != StorageType.SqlMoney))
            {
                precedenceType = StorageType.SqlDecimal;
            }
            if (!DataStorageHelper.IsMixedSql(left, right) || !DataStorageHelper.IsUnsignedSql(precedenceType))
            {
                return precedenceType;
            }
            if (code >= DataTypePrecedence.UInt64)
            {
                throw InvalidExpressionException.AmbiguousBinop(op
                    , DataStorageHelper.GetTypeStorage(left)
                    , DataStorageHelper.GetTypeStorage(right));
            }
            return DataStorageHelper.GetPrecedenceType(code + 1);
        }

        /// <summary>
        /// 
        /// </summary>
        public static StorageType ResultType(StorageType left
            , StorageType right
            , bool lc
            , bool rc
            , Operator op)
        {
            if (((left == StorageType.Guid) && (right == StorageType.Guid)) && Operator.IsRelational(op))
            {
                return left;
            }
            if (((left == StorageType.String) && (right == StorageType.Guid)) && Operator.IsRelational(op))
            {
                return left;
            }
            if (((left == StorageType.Guid) && (right == StorageType.String)) && Operator.IsRelational(op))
            {
                return right;
            }
            int num2 = (int)DataStorageHelper.GetPrecedence(left);
            if (num2 == 0)
            {
                return StorageType.Empty;
            }
            int num = (int)DataStorageHelper.GetPrecedence(right);
            if (num == 0)
            {
                return StorageType.Empty;
            }
            if (Operator.IsLogical(op))
            {
                if ((left == StorageType.Boolean) && (right == StorageType.Boolean))
                {
                    return StorageType.Boolean;
                }
                return StorageType.Empty;
            }
            if ((op == Operator.Plus) && ((left == StorageType.String) || (right == StorageType.String)))
            {
                return StorageType.String;
            }
            DataTypePrecedence code = (DataTypePrecedence)Math.Max(num2, num);
            StorageType precedenceType = DataStorageHelper.GetPrecedenceType(code);
            if ((Operator.IsArithmetical(op) && (precedenceType != StorageType.String)) && (precedenceType != StorageType.Char))
            {
                if (!DataStorageHelper.IsNumeric(left))
                {
                    return StorageType.Empty;
                }
                if (!DataStorageHelper.IsNumeric(right))
                {
                    return StorageType.Empty;
                }
            }
            if ((op == Operator.Multiply) && DataStorageHelper.IsInteger(precedenceType))
            {
                return StorageType.Double;
            }
            if (!DataStorageHelper.IsMixed(left, right))
            {
                return precedenceType;
            }
            if (lc && !rc)
            {
                return right;
            }
            if (!lc && rc)
            {
                return left;
            }
            if (!DataStorageHelper.IsUnsigned(precedenceType))
            {
                return precedenceType;
            }
            if (code >= DataTypePrecedence.UInt64)
            {
                throw InvalidExpressionException.AmbiguousBinop(op
                    , DataStorageHelper.GetTypeStorage(left)
                    , DataStorageHelper.GetTypeStorage(right));
            }
            return DataStorageHelper.GetPrecedenceType(code + 1);
        }

        public static bool IsFloat(StorageType type)
        {
            if ((type != StorageType.Single) && (type != StorageType.Double))
            {
                return (type == StorageType.Decimal);
            }
            return true;
        }

        public static bool IsFloatSql(StorageType type)
        {
            if ((((type != StorageType.Single) && (type != StorageType.Double)) && ((type != StorageType.Decimal) && (type != StorageType.SqlDouble))) && ((type != StorageType.SqlDecimal) && (type != StorageType.SqlMoney)))
            {
                return (type == StorageType.SqlSingle);
            }
            return true;
        }

        public static bool IsInteger(StorageType type)
        {
            if ((((type != StorageType.Int16) && (type != StorageType.Int32)) && ((type != StorageType.Int64) && (type != StorageType.UInt16))) && (((type != StorageType.UInt32) && (type != StorageType.UInt64)) && (type != StorageType.SByte)))
            {
                return (type == StorageType.Byte);
            }
            return true;
        }

        public static bool IsIntegerSql(StorageType type)
        {
            if (((((type != StorageType.Int16) && (type != StorageType.Int32)) && ((type != StorageType.Int64) && (type != StorageType.UInt16))) && (((type != StorageType.UInt32) && (type != StorageType.UInt64)) && ((type != StorageType.SByte) && (type != StorageType.Byte)))) && (((type != StorageType.SqlInt64) && (type != StorageType.SqlInt32)) && (type != StorageType.SqlInt16)))
            {
                return (type == StorageType.SqlByte);
            }
            return true;
        }

        public static bool IsNumeric(StorageType type)
        {
            if (!IsFloat(type))
            {
                return IsInteger(type);
            }
            return true;
        }

        public static bool IsNumericSql(StorageType type)
        {
            if (!IsFloatSql(type))
            {
                return IsIntegerSql(type);
            }
            return true;
        }

        public static bool IsUnknown(object value)
        {
            return DataStorageHelper.IsObjectNull(value);
        }

        public static bool IsSigned(StorageType type)
        {
            if (((type != StorageType.Int16) && (type != StorageType.Int32)) && ((type != StorageType.Int64) && (type != StorageType.SByte)))
            {
                return IsFloat(type);
            }
            return true;
        }

        public static bool IsSignedSql(StorageType type)
        {
            if ((((type != StorageType.Int16) && (type != StorageType.Int32)) && ((type != StorageType.Int64) && (type != StorageType.SByte))) && (((type != StorageType.SqlInt64) && (type != StorageType.SqlInt32)) && (type != StorageType.SqlInt16)))
            {
                return IsFloatSql(type);
            }
            return true;
        }

        public static bool IsUnsigned(StorageType type)
        {
            if (((type != StorageType.UInt16) && (type != StorageType.UInt32)) && (type != StorageType.UInt64))
            {
                return (type == StorageType.Byte);
            }
            return true;
        }

        public static bool IsUnsignedSql(StorageType type)
        {
            if (((type != StorageType.UInt16) && (type != StorageType.UInt32)) && ((type != StorageType.UInt64) && (type != StorageType.SqlByte)))
            {
                return (type == StorageType.Byte);
            }
            return true;
        }

        public static bool ToBoolean(object value)
        {
            if (IsUnknown(value))
            {
                return false;
            }
            if (value is bool)
            {
                return (bool)value;
            }
            if (value is SqlBoolean)
            {
                SqlBoolean flag2 = (SqlBoolean)value;
                return flag2.IsTrue;
            }
            if (value is string)
            {
                try
                {
                    return bool.Parse((string)value);
                }
                catch (Exception exception)
                {
                    throw InvalidExpressionException.DatavalueConvertion(value, typeof(bool), exception);
                }
            }
            throw InvalidExpressionException.DatavalueConvertion(value, typeof(bool), null);
        }

        public static int SqlResultType(int typeCode)
        {
            switch (typeCode)
            {
                case -8:
                    return -7;

                case -7:
                case -6:
                case -4:
                case -3:
                case -1:
                case 0:
                case 2:
                case 5:
                case 8:
                case 11:
                case 13:
                case 15:
                case 0x11:
                case 0x13:
                    return typeCode;

                case -5:
                    return -4;

                case -2:
                    return -1;

                case 1:
                    return 2;

                case 3:
                case 4:
                    return 5;

                case 6:
                case 7:
                    return 8;

                case 9:
                case 10:
                    return 11;

                case 12:
                    return 13;

                case 14:
                    return 15;

                case 0x10:
                    return 0x11;

                case 0x12:
                    return 0x13;

                case 20:
                    return 0x15;

                case 0x17:
                    return 0x18;
            }
            return typeCode;
        }

        public static bool IsMixed(StorageType left, StorageType right)
        {
            if (DataStorageHelper.IsSigned(left) && DataStorageHelper.IsUnsigned(right))
            {
                return true;
            }
            if (DataStorageHelper.IsUnsigned(left))
            {
                return DataStorageHelper.IsSigned(right);
            }
            return false;
        }

        public static bool IsMixedSql(StorageType left, StorageType right)
        {
            if (DataStorageHelper.IsSignedSql(left) && DataStorageHelper.IsUnsignedSql(right))
            {
                return true;
            }
            if (DataStorageHelper.IsUnsignedSql(left))
            {
                return DataStorageHelper.IsSignedSql(right);
            }
            return false;
        }

        public static Type GetTypeStorage(StorageType storageType)
        {
            return DataStorageHelper.types[(int)storageType];
        }

        public static StorageType GetStorageType(Type dataType)
        {
            for (int i = 0; i < DataStorageHelper.types.Length; i++)
            {
                if (dataType == DataStorageHelper.types[i])
                {
                    return (StorageType)i;
                }
            }
            TypeCode typeCode = Type.GetTypeCode(dataType);
            if (TypeCode.Object != typeCode)
            {
                return (StorageType)typeCode;
            }
            return StorageType.Empty;
        }

        public static bool IsSqlType(StorageType storageType)
        {
            return (StorageType.SqlBinary <= storageType);
        }

        public static bool IsSqlType(object obj)
        {
            if (null == obj)
                return false;

            return DataStorageHelper.IsSqlType(obj.GetType());
        }

        public static bool IsSqlType(Type dataType)
        {
            for (int i = 0x18; i < DataStorageHelper.types.Length; i++)
            {
                if (dataType == DataStorageHelper.types[i])
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsObjectNull(object value)
        {
            if ((value != null) && (DBNull.Value != value))
            {
                return IsObjectSqlNull(value);
            }
            return true;
        }

        public static bool IsObjectSqlNull(object value)
        {
            INullable nullable = value as INullable;
            if (nullable != null)
            {
                return nullable.IsNull;
            }
            return false;
        }

        public static DataTypePrecedence GetPrecedence(StorageType storageType)
        {
            switch (storageType)
            {
                case StorageType.Boolean:
                    return DataTypePrecedence.Boolean;

                case StorageType.Char:
                    return DataTypePrecedence.Char;

                case StorageType.SByte:
                    return DataTypePrecedence.SByte;

                case StorageType.Byte:
                    return DataTypePrecedence.Byte;

                case StorageType.Int16:
                    return DataTypePrecedence.Int16;

                case StorageType.UInt16:
                    return DataTypePrecedence.UInt16;

                case StorageType.Int32:
                    return DataTypePrecedence.Int32;

                case StorageType.UInt32:
                    return DataTypePrecedence.UInt32;

                case StorageType.Int64:
                    return DataTypePrecedence.Int64;

                case StorageType.UInt64:
                    return DataTypePrecedence.UInt64;

                case StorageType.Single:
                    return DataTypePrecedence.Single;

                case StorageType.Double:
                    return DataTypePrecedence.Double;

                case StorageType.Decimal:
                    return DataTypePrecedence.Decimal;

                case StorageType.DateTime:
                    return DataTypePrecedence.DateTime;

                case StorageType.TimeSpan:
                    return DataTypePrecedence.TimeSpan;

                case StorageType.String:
                    return DataTypePrecedence.String;

                case StorageType.SqlBinary:
                    return DataTypePrecedence.SqlBinary;

                case StorageType.SqlBoolean:
                    return DataTypePrecedence.SqlBoolean;

                case StorageType.SqlByte:
                    return DataTypePrecedence.SqlByte;

                case StorageType.SqlBytes:
                    return DataTypePrecedence.SqlBytes;

                case StorageType.SqlChars:
                    return DataTypePrecedence.SqlChars;

                case StorageType.SqlDateTime:
                    return DataTypePrecedence.SqlDateTime;

                case StorageType.SqlDecimal:
                    return DataTypePrecedence.SqlDecimal;

                case StorageType.SqlDouble:
                    return DataTypePrecedence.SqlDouble;

                case StorageType.SqlGuid:
                    return DataTypePrecedence.SqlGuid;

                case StorageType.SqlInt16:
                    return DataTypePrecedence.SqlInt16;

                case StorageType.SqlInt32:
                    return DataTypePrecedence.SqlInt32;

                case StorageType.SqlInt64:
                    return DataTypePrecedence.SqlInt64;

                case StorageType.SqlMoney:
                    return DataTypePrecedence.SqlMoney;

                case StorageType.SqlSingle:
                    return DataTypePrecedence.SqlSingle;

                case StorageType.SqlString:
                    return DataTypePrecedence.SqlString;
            }
            return DataTypePrecedence.Error;
        }

        public static StorageType GetPrecedenceType(DataTypePrecedence code)
        {
            switch (code)
            {
                case DataTypePrecedence.SqlBinary:
                    return StorageType.SqlBinary;

                case DataTypePrecedence.Char:
                    return StorageType.Char;

                case DataTypePrecedence.String:
                    return StorageType.String;

                case DataTypePrecedence.SqlString:
                    return StorageType.SqlString;

                case DataTypePrecedence.SqlGuid:
                    return StorageType.SqlGuid;

                case DataTypePrecedence.Boolean:
                    return StorageType.Boolean;

                case DataTypePrecedence.SqlBoolean:
                    return StorageType.SqlBoolean;

                case DataTypePrecedence.SByte:
                    return StorageType.SByte;

                case DataTypePrecedence.SqlByte:
                    return StorageType.SqlByte;

                case DataTypePrecedence.Byte:
                    return StorageType.Byte;

                case DataTypePrecedence.Int16:
                    return StorageType.Int16;

                case DataTypePrecedence.SqlInt16:
                    return StorageType.SqlInt16;

                case DataTypePrecedence.UInt16:
                    return StorageType.UInt16;

                case DataTypePrecedence.Int32:
                    return StorageType.Int32;

                case DataTypePrecedence.SqlInt32:
                    return StorageType.SqlInt32;

                case DataTypePrecedence.UInt32:
                    return StorageType.UInt32;

                case DataTypePrecedence.Int64:
                    return StorageType.Int64;

                case DataTypePrecedence.SqlInt64:
                    return StorageType.SqlInt64;

                case DataTypePrecedence.UInt64:
                    return StorageType.UInt64;

                case DataTypePrecedence.SqlMoney:
                    return StorageType.SqlMoney;

                case DataTypePrecedence.Decimal:
                    return StorageType.Decimal;

                case DataTypePrecedence.SqlDecimal:
                    return StorageType.SqlDecimal;

                case DataTypePrecedence.Single:
                    return StorageType.Single;

                case DataTypePrecedence.SqlSingle:
                    return StorageType.SqlSingle;

                case DataTypePrecedence.Double:
                    return StorageType.Double;

                case DataTypePrecedence.SqlDouble:
                    return StorageType.SqlDouble;

                case DataTypePrecedence.TimeSpan:
                    return StorageType.TimeSpan;

                case DataTypePrecedence.DateTime:
                    return StorageType.DateTime;

                case DataTypePrecedence.SqlDateTime:
                    return StorageType.SqlDateTime;
            }
            return StorageType.Empty;
        }

        public static int CompareNulls(object val1, object val2)
        {
            if (val1 == val2)
            {
                return 0;
            }

            if (val1 == DBNull.Value)
            {
                if (val2 == null)
                {
                    return 1;
                }

                return -1;
            }

            if (val2 == DBNull.Value)
            {
                if (val1 == null)
                {
                    return -1;
                }

                return 1;
            }

            if (val1 == null)
            {
                return -1;
            }

            if (val2 == null)
            {
                return 1;
            }

            return 0;
        }

        public static Int32 BinarySearch<T>(IList<T> list, T value)
        {
            return BinarySearch(list, value, Comparer<T>.Default);
        }

        public static Int32 BinarySearch<T>(IList<T> list, T value, IComparer<T> comparer)
        {
            #region Parameter Validation

            if (Object.ReferenceEquals(null, list))
                throw new ArgumentNullException("list");
            if (Object.ReferenceEquals(null, comparer))
                throw new ArgumentNullException("comparer");

            #endregion

            Int32 lower = 0;
            Int32 upper = list.Count - 1;

            while (lower <= upper)
            {
                Int32 middle = (lower + upper) / 2;
                Int32 comparisonResult = comparer.Compare(value, list[middle]);
                if (comparisonResult == 0)
                    return middle;
                else if (comparisonResult < 0)
                    upper = middle - 1;
                else
                    lower = middle + 1;
            }

            return -1;
        }


        /// <summary>
        /// Escapes the name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string EscapeName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return name;
            }

            StringBuilder sb = new StringBuilder();

            bool enclose = false;
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];

                if (i == 0 && char.IsDigit(name[i]))
                {
                    enclose = true;
                }

                if (c == '\\' || c == ']')
                {
                    enclose = true;
                    sb.Append("\\");
                }

                if (!enclose)
                {
                    enclose = (c == '~' || c == '(' || c == ')' || c == '#' || c == '/' || c == '=' ||
                    c == '>' || c == '<' || c == '+' || c == '-' || c == '*' || c == '%' || c == '&' ||
                    c == '|' || c == '^' || c == '\'' || c == '"' || c == '[' || c == ' ' || c == '$' || c == '.');
                }

                sb.Append(c);
            }

            if (enclose)
            {
                sb.Insert(0, '[');
                sb.Append(']');
            }

            return sb.ToString();
        }

        /// <summary>
        /// Escapes the LIKE value.
        /// </summary>
        /// <param name="valueWithoutWildcards">The value without wildcards.</param>
        /// <returns></returns>
        public static string EscapeLikeValue(string valueWithoutWildcards)
        {
            StringBuilder sb = new StringBuilder();

            string escapedValue = EscapeValue(valueWithoutWildcards);
            for (int i = 0; i < escapedValue.Length; i++)
            {
                char c = escapedValue[i];
                if (c == '*' || c == '%' || c == '[' || c == ']')
                {
                    sb.Append("[").Append(c).Append("]");
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Escapes the filtering value.
        /// </summary>
        /// <param name="valueWithoutWildcards">The value without wildcards.</param>
        /// <returns></returns>
        public static string EscapeValue(string valueWithoutWildcards)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < valueWithoutWildcards.Length; i++)
            {
                char c = valueWithoutWildcards[i];
                if (c == '\'')
                {
                    sb.Append("''");
                }
                else
                {
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
