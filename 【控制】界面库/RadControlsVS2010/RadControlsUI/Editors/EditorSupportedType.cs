using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Returns the type supported by the class implementing the ICellEditor interface. 
    /// The supported type is the data type that can be handled and edited by the editor. 
    /// </summary>
    [Flags]
    public enum EditorSupportedType
    {
            Object = 0,
            DBNull = 1,
            Null = 2,
            Bool = 4,
            Char = 8,
            Sbyte = 16,
            Byte = 32,
            Short = 64,
            UShort = 128,
            Int = 256,
            UInt = 512,
            Long = (UInt << 1),
            Ulong = (UInt << 2),
            Float = (UInt << 3),
            Double = (UInt << 4),
            Decimal = (UInt << 5),
            DateTime = (UInt << 6),
            TimeSpan = (UInt << 7),
            String = (UInt << 8),
            Guid = (UInt << 9),
            Type = (UInt << 10),
            Uri = (UInt << 11),
            SqlBinary = (UInt << 12),
            SqlBoolean = (UInt << 13),
            SqlByte = (UInt << 14),
            SqlBytes = (UInt << 15),
            SqlChars = (UInt << 16),
            SqlDateTime = (UInt << 17),
            SqlDecimal = (UInt << 18),
            SqlDouble = (UInt << 19),
            SqlGuid = (UInt << 20),
            SqlInt16 = (UInt << 21),
            SqlInt32 = (UInt << 22),
            SqlInt64 = (UInt << 23),
            SqlMoney = (UInt << 24),
            SqlSingle = (UInt << 25),
            SqlString = (UInt << 26),
            Numeric = (SqlDecimal | SqlDouble | SqlGuid | SqlInt16 | SqlInt32 | SqlInt64 | SqlMoney | SqlSingle | SqlByte |
                            Sbyte | Byte | Short | UShort | Int | UInt | Long | Ulong | Float | Double | Decimal),
            Alpha = (Char | String | SqlString | Uri | SqlChars),
            AlphaNumeric = (Alpha | Numeric)
    }
}
