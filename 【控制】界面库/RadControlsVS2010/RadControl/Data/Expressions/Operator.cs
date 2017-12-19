namespace Telerik.Data.Expressions
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Data.SqlTypes;
    using System.Diagnostics;

    /// <summary>
    /// 
    /// </summary>
    sealed class Operator
    {                                                                                               // OpCode  Pri  Word
        public static Operator Noop = new Operator("", 0, new UnaryFunc(Operator.RValueFunc));                     //    0     0   ""
        public static Operator Negative = new Operator("-", 20, new UnaryFunc(Operator.NegativeFunc));             //    1     20  "-" 
        public static Operator UnaryPlus = new Operator("+", 20, new UnaryFunc(Operator.PositiveFunc));            //    2     20  "+" 
        public static Operator Not = new Operator("Not", 9, new UnaryFunc(Operator.NotFunc));                      //    3     9   "Not"
        public static Operator BetweenAnd = new Operator("BetweenAnd", 12, new BinaryFunc(Operator.NotImpl));       //    4     12  "BetweenAnd"
        public static Operator In = new Operator("In", 11, new BinaryFunc(Operator.InFunc));                        //    5     11  "In" 
        public static Operator Between = new Operator("Between", 11, new TernaryFunc(Operator.BetweenFunc));             //    6     11  "Between"
        public static Operator EqualTo = new Operator("=", 13, new BinaryFunc(Operator.EqualToFunc));               //    7     13  "="
        public static Operator GreaterThen = new Operator(">", 13, new BinaryFunc(Operator.GreaterThenFunc));       //    8     13  ">"
        public static Operator LessThen = new Operator("<", 13, new BinaryFunc(Operator.LessThenFunc));             //    9     13  "<"
        public static Operator GreaterOrEqual = new Operator(">=", 13,new BinaryFunc(Operator.GreaterOrEqualFunc));//    10    13  ">=" 
        public static Operator LessOrEqual = new Operator("<=", 13, new BinaryFunc(Operator.LessOrEqualFunc));      //    11    13  "<="
        public static Operator NotEqual = new Operator("<>", 13, new BinaryFunc(Operator.NotEqualFunc));            //    12    13  "<>"
        public static Operator Is = new Operator("Is", 10, new BinaryFunc(Operator.IsFunc));                        //    13    10  "Is" 
        public static Operator Like = new Operator("Like", 11, new BinaryFunc(Operator.NotImpl));                   //    14    11  "Like"
        
        public static Operator Plus = new Operator("+", 16, new BinaryFunc(Operator.AddFunc));                      //    15    16  "+"
        public static Operator Minus = new Operator("-", 16, new BinaryFunc(Operator.SubtractFunc));                //    16    16  "-" 
        public static Operator Multiply = new Operator("*", 19, new BinaryFunc(Operator.MultiplyFunc));             //    17    19  "*" 
        public static Operator Divide = new Operator("/", 19, new BinaryFunc(Operator.DivideFunc));                 //    18    19  "/" 
                                                                                                    //    19    18  @"\"
        public static Operator Modulo = new Operator("Mod", 17, new BinaryFunc(Operator.ModuloFunc));               //    20    17  "Mod"
                                                                                                    //    21    21  "**"
        public static Operator BitwiseAnd = new Operator("&", 8, new BinaryFunc(Operator.NotImpl));                 //    22    8   "&"
        public static Operator BitwiseOr = new Operator("|", 7, new BinaryFunc(Operator.NotImpl));                  //    23    7   "|" 
        public static Operator BitwiseNot = new Operator("^", 6, new BinaryFunc(Operator.NotImpl));                 //    25    6   "^" 
        public static Operator BitwiseXor = new Operator("~", 9, new BinaryFunc(Operator.NotImpl));                 //    24    9   "~" 
        public static Operator And = new Operator("And", 8, new BinaryFunc(Operator.AndFunc));                      //    26    8   "And" 
        public static Operator Or = new Operator("Or", 7, new BinaryFunc(Operator.OrFunc));                         //    27    7   "Or", 
        public static Operator Proc = new Operator("Proc", 2, new BinaryFunc(Operator.NotImpl));                    //    28    2   "Proc", 
        public static Operator Iff = new Operator("Iff", 22, new BinaryFunc(Operator.NotImpl));                     //    29    22  "Iff", 
        public static Operator Qual = new Operator(".", 23, new BinaryFunc(Operator.NotImpl));                      //    30    23  ".", 
        public static Operator Dot = new Operator(".", 23, new BinaryFunc(Operator.NotImpl));                       //    31    23  ".", 
        public static Operator Null = new Operator("Null", 24, new ZeroFunc(delegate { return null; }));  //    32    24  "Null", 
        public static Operator True = new Operator("True", 24, new ZeroFunc(delegate { return true; }));          //    33    24  "True", 
        public static Operator False = new Operator("False", 24, new ZeroFunc(delegate { return false; }));       //    34    24  "False", 
        public static Operator IsNot = new Operator("Is Not", 24, new UnaryFunc(Operator.IsNotFunc));              //    39    24  "Is Not"

        static readonly Operator[] unaryOps = new Operator[]
            {
                Operator.UnaryPlus
                , Operator.Negative
                , Operator.Not
            };


        static readonly Operator[] binaryOps = new Operator[]
            {
                Operator.In,
                Operator.EqualTo,
                Operator.GreaterThen,
                Operator.LessThen,
                Operator.GreaterOrEqual,
                Operator.LessOrEqual,
                Operator.NotEqual,
                Operator.Is,
                Operator.Plus,
                Operator.Minus,
                Operator.Multiply,
                Operator.Divide,
                Operator.Modulo,
                Operator.And,
                Operator.Or,
                Operator.Between,
                Operator.BetweenAnd
            };

        public static bool IsUnary(Operator op)
        {
            return (Array.IndexOf(Operator.unaryOps, op) >= 0);
        }

        public static bool IsTernary(Operator op)
        {
            return op == Operator.BetweenAnd;
        }

        public static bool IsBinary(Operator op)
        {
            return (Array.IndexOf(Operator.binaryOps, op) >= 0);
        }

        internal static bool IsArithmetical(Operator op)
        {
            return (op == Operator.Plus 
                || op == Operator.Minus 
                || op == Operator.Multiply 
                || op == Operator.Divide
                || op == Operator.Modulo);
        }

        internal static bool IsLogical(Operator op)
        {
            return (Operator.And == op
                || Operator.Or == op
                || Operator.Not == op
                || Operator.Is == op);
        }

        internal static bool IsRelational(Operator op)
        {
            return (Operator.EqualTo == op
                || Operator.GreaterThen == op
                || Operator.LessThen == op
                || Operator.GreaterOrEqual == op
                || Operator.LessOrEqual == op
                || Operator.NotEqual == op);
        }

        public delegate object ZeroFunc();
        public delegate object UnaryFunc(Operand operand, OperatorContext context);
        public delegate object BinaryFunc(Operand lhs, Operand rhs, OperatorContext context);
        public delegate object TernaryFunc(Operand op1, Operand op2, Operand op3, OperatorContext context);

        public readonly int Priority;
        public readonly string Name;
        public readonly Delegate Func;

        Operator(string name, int priority, Delegate func)
        {
            this.Priority = priority;
            this.Name = name;
            this.Func = func;
        }

        public override string ToString()
        {
            return this.Name;
        }

        static object NotImpl(Operand lhs, Operand rhs, OperatorContext context)
        {
            throw new NotImplementedException("The requested op is not implemented.");
        }

        static object NullFunc(Operand lhs, Operand rhs, OperatorContext context)
        {
            return null;
        }

        static object RValueFunc(Operand rhs, OperatorContext context)
        {
            return rhs.Value;
        }

        static object AddFunc(Operand lhs, Operand rhs, OperatorContext context)
        {
            StorageType resultType;
            object retValue;

            if (!Operator.GetResultType(Operator.Plus, lhs, rhs, out resultType, out retValue))
            {
                return retValue;
            }
            
            switch (resultType)
            {
                case StorageType.Char:
                case StorageType.String:
                    return Convert.ToString(lhs.Value, context.FormatProvider) + Convert.ToString(rhs.Value, context.FormatProvider);

                case StorageType.SByte:
                    return Convert.ToSByte(Convert.ToSByte(lhs.Value, context.FormatProvider) + Convert.ToSByte(rhs.Value, context.FormatProvider), context.FormatProvider);

                case StorageType.Int16:
                    return Convert.ToInt16(Convert.ToInt16(lhs.Value, context.FormatProvider) + Convert.ToInt16(rhs.Value, context.FormatProvider), context.FormatProvider);
                    
                case StorageType.UInt16:
                    return Convert.ToUInt16(Convert.ToUInt16(lhs.Value, context.FormatProvider) + Convert.ToUInt16(rhs.Value, context.FormatProvider), context.FormatProvider);
                    
                case StorageType.Int32:
                    return Convert.ToInt32(lhs.Value, context.FormatProvider) + Convert.ToInt32(rhs.Value, context.FormatProvider);
                    
                case StorageType.UInt32:
                    return Convert.ToUInt32(lhs.Value, context.FormatProvider) + Convert.ToUInt32(rhs.Value, context.FormatProvider);
                    
                case StorageType.Int64:
                    return Convert.ToInt64(lhs.Value, context.FormatProvider) + Convert.ToInt64(rhs.Value, context.FormatProvider);
                    
                case StorageType.UInt64:
                    return Convert.ToUInt64(lhs.Value, context.FormatProvider) + Convert.ToUInt64(rhs.Value, context.FormatProvider);
                    
                case StorageType.Single:
                    return Convert.ToSingle(lhs.Value, context.FormatProvider) + Convert.ToSingle(rhs.Value, context.FormatProvider);
                    
                case StorageType.Double:
                    return Convert.ToDouble(lhs.Value, context.FormatProvider) + Convert.ToDouble(rhs.Value, context.FormatProvider);
                    
                case StorageType.Decimal:
                    return Convert.ToDecimal(lhs.Value, context.FormatProvider) + Convert.ToDecimal(rhs.Value, context.FormatProvider);
                    
                case StorageType.DateTime:
                    if ((lhs.Value is TimeSpan) && (rhs.Value is DateTime))
                    {
                        return ((DateTime)rhs.Value) + ((TimeSpan)lhs.Value);
                    }
                    else if ((lhs.Value is DateTime) && (rhs.Value is TimeSpan))
                    {
                        return ((DateTime)lhs.Value) + ((TimeSpan)rhs.Value);
                    }
                    break;

                case StorageType.TimeSpan:
                    return ((TimeSpan)lhs.Value) + ((TimeSpan)rhs.Value);
                    
/*                case StorageType.Guid:
                case StorageType.ByteArray:
                case StorageType.CharArray:
                case StorageType.Type:
                case StorageType.Uri:
                case StorageType.SqlBinary:
                case StorageType.SqlBoolean:
                case StorageType.SqlBytes:
                case StorageType.SqlChars:
                case StorageType.SqlGuid:
                    //typeMismatch = true;
                    break;*/

                case StorageType.SqlByte:
                    return SqlConvert.ConvertToSqlByte(lhs.Value) + SqlConvert.ConvertToSqlByte(rhs.Value);
                    
                case StorageType.SqlDateTime:
                    if ((lhs.Value is TimeSpan) && (rhs.Value is SqlDateTime))
                    {
                        return SqlConvert.ConvertToSqlDateTime(SqlConvert.ConvertToSqlDateTime(rhs.Value).Value + ((TimeSpan)lhs.Value));
                    }
                    else if ((lhs.Value is SqlDateTime) && (rhs.Value is TimeSpan))
                    {
                        return SqlConvert.ConvertToSqlDateTime(SqlConvert.ConvertToSqlDateTime(lhs.Value).Value + ((TimeSpan)rhs.Value));
                    }
                    break;

                case StorageType.SqlDecimal:
                    return SqlConvert.ConvertToSqlDecimal(lhs.Value) + SqlConvert.ConvertToSqlDecimal(rhs.Value);
                    
                case StorageType.SqlDouble:
                    return SqlConvert.ConvertToSqlDouble(lhs.Value) + SqlConvert.ConvertToSqlDouble(rhs.Value);
                    
                case StorageType.SqlInt16:
                    return SqlConvert.ConvertToSqlInt16(lhs.Value) + SqlConvert.ConvertToSqlInt16(rhs.Value);
                    
                case StorageType.SqlInt32:
                    return SqlConvert.ConvertToSqlInt32(lhs.Value) + SqlConvert.ConvertToSqlInt32(rhs.Value);
                    
                case StorageType.SqlInt64:
                    return SqlConvert.ConvertToSqlInt64(lhs.Value) + SqlConvert.ConvertToSqlInt64(rhs.Value);
                    
                case StorageType.SqlMoney:
                    return SqlConvert.ConvertToSqlMoney(lhs.Value) + SqlConvert.ConvertToSqlMoney(rhs.Value);
                    
                case StorageType.SqlSingle:
                    return SqlConvert.ConvertToSqlSingle(lhs.Value) + SqlConvert.ConvertToSqlSingle(rhs.Value);
                    
                case StorageType.SqlString:
                    return SqlConvert.ConvertToSqlString(lhs.Value) + SqlConvert.ConvertToSqlString(rhs.Value);
            }

            throw InvalidExpressionException.TypeMismatch(Operator.Plus.ToString());
        }

        static object SubtractFunc(Operand lhs, Operand rhs, OperatorContext context)
        {
            StorageType resultType;
            object retValue;

            if (!Operator.GetResultType(Operator.Minus, lhs, rhs, out resultType, out retValue))
            {
                return retValue;
            }

            switch (resultType)
            {
                case StorageType.SByte:
                    return Convert.ToSByte(Convert.ToSByte(lhs.Value, context.FormatProvider) - Convert.ToSByte(rhs.Value, context.FormatProvider), context.FormatProvider);
                    
                case StorageType.Byte:
                    return Convert.ToByte(Convert.ToByte(lhs.Value, context.FormatProvider) - Convert.ToByte(rhs.Value, context.FormatProvider), context.FormatProvider);
                    
                case StorageType.Int16:
                    return Convert.ToInt16(Convert.ToInt16(lhs.Value, context.FormatProvider) - Convert.ToInt16(rhs.Value, context.FormatProvider), context.FormatProvider);
                    
                case StorageType.UInt16:
                    return Convert.ToUInt16(Convert.ToUInt16(lhs.Value, context.FormatProvider) - Convert.ToUInt16(rhs.Value, context.FormatProvider), context.FormatProvider);
                    
                case StorageType.Int32:
                    return Convert.ToInt32(lhs.Value, context.FormatProvider) - Convert.ToInt32(rhs.Value, context.FormatProvider);
                    
                case StorageType.UInt32:
                    return Convert.ToUInt32(lhs.Value, context.FormatProvider) - Convert.ToUInt32(rhs.Value, context.FormatProvider);
                    
                case StorageType.Int64:
                    return Convert.ToInt64(lhs.Value, context.FormatProvider) - Convert.ToInt64(rhs.Value, context.FormatProvider);
                    
                case StorageType.UInt64:
                    return Convert.ToUInt64(lhs.Value, context.FormatProvider) - Convert.ToUInt64(rhs.Value, context.FormatProvider);
                    
                case StorageType.Single:
                    return Convert.ToSingle(lhs.Value, context.FormatProvider) - Convert.ToSingle(rhs.Value, context.FormatProvider);
                    
                case StorageType.Double:
                    return Convert.ToDouble(lhs.Value, context.FormatProvider) - Convert.ToDouble(rhs.Value, context.FormatProvider);
                    
                case StorageType.Decimal:
                    return Convert.ToDecimal(lhs.Value, context.FormatProvider) - Convert.ToDecimal(rhs.Value, context.FormatProvider);
                    
                case StorageType.DateTime:
                    return ((DateTime)lhs.Value) - ((TimeSpan)rhs.Value);
                    
                case StorageType.TimeSpan:
                    if (lhs.Value is DateTime)
                    {
                        return (TimeSpan)(((DateTime)lhs.Value) - ((DateTime)rhs.Value));
                    }
                    else
                    {
                        return ((TimeSpan)lhs.Value) - ((TimeSpan)rhs.Value);
                    }
                    
                /*case StorageType.String:
                case StorageType.Guid:
                case StorageType.ByteArray:
                case StorageType.CharArray:
                case StorageType.Type:
                case StorageType.Uri:
                case StorageType.SqlBinary:
                case StorageType.SqlBoolean:
                case StorageType.SqlBytes:
                case StorageType.SqlChars:
                case StorageType.SqlGuid:
                    break;*/

                case StorageType.SqlByte:
                    return SqlConvert.ConvertToSqlByte(lhs.Value) - SqlConvert.ConvertToSqlByte(rhs.Value);
                    
                case StorageType.SqlDateTime:
                    if ((lhs.Value is TimeSpan) && (rhs.Value is SqlDateTime))
                    {
                        return SqlConvert.ConvertToSqlDateTime(SqlConvert.ConvertToSqlDateTime(rhs.Value).Value - ((TimeSpan)lhs.Value));
                    }
                    else if ((lhs.Value is SqlDateTime) && (rhs.Value is TimeSpan))
                    {
                        return SqlConvert.ConvertToSqlDateTime(SqlConvert.ConvertToSqlDateTime(lhs.Value).Value - ((TimeSpan)rhs.Value));
                    }
                    /*else
                    {
                        typeMismatch = true;
                    }*/
                    break;

                case StorageType.SqlDecimal:
                    return SqlConvert.ConvertToSqlDecimal(lhs.Value) - SqlConvert.ConvertToSqlDecimal(rhs.Value);
                    
                case StorageType.SqlDouble:
                    return SqlConvert.ConvertToSqlDouble(lhs.Value) - SqlConvert.ConvertToSqlDouble(rhs.Value);
                    
                case StorageType.SqlInt16:
                    return SqlConvert.ConvertToSqlInt16(lhs.Value) - SqlConvert.ConvertToSqlInt16(rhs.Value);
                    
                case StorageType.SqlInt32:
                    return SqlConvert.ConvertToSqlInt32(lhs.Value) - SqlConvert.ConvertToSqlInt32(rhs.Value);
                    
                case StorageType.SqlInt64:
                    return SqlConvert.ConvertToSqlInt64(lhs.Value) - SqlConvert.ConvertToSqlInt64(rhs.Value);
                    
                case StorageType.SqlMoney:
                    return SqlConvert.ConvertToSqlMoney(lhs.Value) - SqlConvert.ConvertToSqlMoney(rhs.Value);
                    
                case StorageType.SqlSingle:
                    return SqlConvert.ConvertToSqlSingle(lhs.Value) - SqlConvert.ConvertToSqlSingle(rhs.Value);
            }

            throw InvalidExpressionException.TypeMismatch(Operator.Minus.ToString());
        }

        static object MultiplyFunc(Operand lhs, Operand rhs, OperatorContext context)
        {
            StorageType resultType;
            object retValue;

            if (!Operator.GetResultType(Operator.Multiply, lhs, rhs, out resultType, out retValue))
            {
                return retValue;
            }

            switch (resultType)
            {
                case StorageType.SByte:
                    return Convert.ToSByte(Convert.ToSByte(lhs.Value, context.FormatProvider) * Convert.ToSByte(rhs.Value, context.FormatProvider), context.FormatProvider);

                case StorageType.Byte:
                    return Convert.ToByte(Convert.ToByte(lhs.Value, context.FormatProvider) * Convert.ToByte(rhs.Value, context.FormatProvider), context.FormatProvider);

                case StorageType.Int16:
                    return Convert.ToInt16(Convert.ToInt16(lhs.Value, context.FormatProvider) * Convert.ToInt16(rhs.Value, context.FormatProvider), context.FormatProvider);

                case StorageType.UInt16:
                    return Convert.ToUInt16(Convert.ToUInt16(lhs.Value, context.FormatProvider) * Convert.ToUInt16(rhs.Value, context.FormatProvider), context.FormatProvider);

                case StorageType.Int32:
                    return Convert.ToInt32(lhs.Value, context.FormatProvider) * Convert.ToInt32(rhs.Value, context.FormatProvider);

                case StorageType.UInt32:
                    return Convert.ToUInt32(lhs.Value, context.FormatProvider) * Convert.ToUInt32(rhs.Value, context.FormatProvider);

                case StorageType.Int64:
                    return Convert.ToInt64(lhs.Value, context.FormatProvider) * Convert.ToInt64(rhs.Value, context.FormatProvider);

                case StorageType.UInt64:
                    return Convert.ToUInt64(lhs.Value, context.FormatProvider) * Convert.ToUInt64(rhs.Value, context.FormatProvider);

                case StorageType.Single:
                    return Convert.ToSingle(lhs.Value, context.FormatProvider) * Convert.ToSingle(rhs.Value, context.FormatProvider);

                case StorageType.Double:
                    return Convert.ToDouble(lhs.Value, context.FormatProvider) * Convert.ToDouble(rhs.Value, context.FormatProvider);

                case StorageType.Decimal:
                    return Convert.ToDecimal(lhs.Value, context.FormatProvider) * Convert.ToDecimal(rhs.Value, context.FormatProvider);

/*                case StorageType.DateTime:
                case StorageType.TimeSpan:
                case StorageType.String:
                case StorageType.Guid:
                case StorageType.ByteArray:
                case StorageType.CharArray:
                case StorageType.Type:
                case StorageType.Uri:
                case StorageType.SqlBinary:
                case StorageType.SqlBoolean:
                case StorageType.SqlBytes:
                case StorageType.SqlChars:
                case StorageType.SqlDateTime:
                case StorageType.SqlGuid:
                    break;*/

                case StorageType.SqlByte:
                    return SqlConvert.ConvertToSqlByte(lhs.Value) * SqlConvert.ConvertToSqlByte(rhs.Value);

                case StorageType.SqlDecimal:
                    return SqlConvert.ConvertToSqlDecimal(lhs.Value) * SqlConvert.ConvertToSqlDecimal(rhs.Value);

                case StorageType.SqlDouble:
                    return SqlConvert.ConvertToSqlDouble(lhs.Value) * SqlConvert.ConvertToSqlDouble(rhs.Value);

                case StorageType.SqlInt16:
                    return SqlConvert.ConvertToSqlInt16(lhs.Value) * SqlConvert.ConvertToSqlInt16(rhs.Value);

                case StorageType.SqlInt32:
                    return SqlConvert.ConvertToSqlInt32(lhs.Value) * SqlConvert.ConvertToSqlInt32(rhs.Value);

                case StorageType.SqlInt64:
                    return SqlConvert.ConvertToSqlInt64(lhs.Value) * SqlConvert.ConvertToSqlInt64(rhs.Value);

                case StorageType.SqlMoney:
                    return SqlConvert.ConvertToSqlMoney(lhs.Value) * SqlConvert.ConvertToSqlMoney(rhs.Value);

                case StorageType.SqlSingle:
                    return SqlConvert.ConvertToSqlSingle(lhs.Value) * SqlConvert.ConvertToSqlSingle(rhs.Value);
            }
            throw InvalidExpressionException.TypeMismatch(Operator.Multiply.ToString());
        }

        static object DivideFunc(Operand lhs, Operand rhs, OperatorContext context)
        {
            StorageType resultType;
            object retValue;

            if (!Operator.GetResultType(Operator.Divide, lhs, rhs, out resultType, out retValue))
            {
                return retValue;
            }

            switch (resultType)
            {
                case StorageType.SByte:
                    return Convert.ToSByte(Convert.ToSByte(lhs.Value, context.FormatProvider) / Convert.ToSByte(rhs.Value, context.FormatProvider), context.FormatProvider);

                case StorageType.Byte:
                    return Convert.ToByte(Convert.ToByte(lhs.Value, context.FormatProvider) / Convert.ToByte(rhs.Value, context.FormatProvider), context.FormatProvider);

                case StorageType.Int16:
                    return Convert.ToInt16(Convert.ToInt16(lhs.Value, context.FormatProvider) / Convert.ToInt16(rhs.Value, context.FormatProvider), context.FormatProvider);

                case StorageType.UInt16:
                    return Convert.ToUInt16(Convert.ToUInt16(lhs.Value, context.FormatProvider) / Convert.ToUInt16(rhs.Value, context.FormatProvider), context.FormatProvider);

                case StorageType.Int32:
                    return Convert.ToInt32(lhs.Value, context.FormatProvider) / Convert.ToInt32(rhs.Value, context.FormatProvider);

                case StorageType.UInt32:
                    return Convert.ToUInt32(lhs.Value, context.FormatProvider) / Convert.ToUInt32(rhs.Value, context.FormatProvider);

                case StorageType.Int64:
                    return Convert.ToInt64(lhs.Value, context.FormatProvider) / Convert.ToInt64(rhs.Value, context.FormatProvider);

                case StorageType.UInt64:
                    return Convert.ToUInt64(lhs.Value, context.FormatProvider) / Convert.ToUInt64(rhs.Value, context.FormatProvider);

                case StorageType.Single:
                    return Convert.ToSingle(lhs.Value, context.FormatProvider) / Convert.ToSingle(rhs.Value, context.FormatProvider);

                case StorageType.Double:
                    double num4 = Convert.ToDouble(rhs.Value, context.FormatProvider);
                    return Convert.ToDouble(lhs.Value, context.FormatProvider) / num4;

                case StorageType.Decimal:
                    return Convert.ToDecimal(lhs.Value, context.FormatProvider) / Convert.ToDecimal(rhs.Value, context.FormatProvider);

/*                case StorageType.DateTime:
                case StorageType.TimeSpan:
                case StorageType.String:
                case StorageType.Guid:
                case StorageType.ByteArray:
                case StorageType.CharArray:
                case StorageType.Type:
                case StorageType.Uri:
                case StorageType.SqlBinary:
                case StorageType.SqlBoolean:
                case StorageType.SqlBytes:
                case StorageType.SqlChars:
                case StorageType.SqlDateTime:
                case StorageType.SqlGuid:
                    break;*/

                case StorageType.SqlByte:
                    return SqlConvert.ConvertToSqlByte(lhs.Value) / SqlConvert.ConvertToSqlByte(rhs.Value);

                case StorageType.SqlDecimal:
                    return SqlConvert.ConvertToSqlDecimal(lhs.Value) / SqlConvert.ConvertToSqlDecimal(rhs.Value);

                case StorageType.SqlDouble:
                    return SqlConvert.ConvertToSqlDouble(lhs.Value) / SqlConvert.ConvertToSqlDouble(rhs.Value);

                case StorageType.SqlInt16:
                    return SqlConvert.ConvertToSqlInt16(lhs.Value) / SqlConvert.ConvertToSqlInt16(rhs.Value);

                case StorageType.SqlInt32:
                    return SqlConvert.ConvertToSqlInt32(lhs.Value) / SqlConvert.ConvertToSqlInt32(rhs.Value);

                case StorageType.SqlInt64:
                    return SqlConvert.ConvertToSqlInt64(lhs.Value) / SqlConvert.ConvertToSqlInt64(rhs.Value);

                case StorageType.SqlMoney:
                    return SqlConvert.ConvertToSqlMoney(lhs.Value) / SqlConvert.ConvertToSqlMoney(rhs.Value);

                case StorageType.SqlSingle:
                    return SqlConvert.ConvertToSqlSingle(lhs.Value) / SqlConvert.ConvertToSqlSingle(rhs.Value);
            }

            throw InvalidExpressionException.TypeMismatch(Operator.Divide.ToString());
        }

        static object ModuloFunc(Operand lhs, Operand rhs, OperatorContext context)
        {
            StorageType resultType;
            object retValue;

            if (!Operator.GetResultType(Operator.Modulo, lhs, rhs, out resultType, out retValue))
            {
                return retValue;
            }

            if (!DataStorageHelper.IsIntegerSql(resultType))
            {
                throw InvalidExpressionException.TypeMismatch(Operator.Plus.ToString());
            }
            
            if (resultType != StorageType.UInt64)
            {
                if (DataStorageHelper.IsSqlType(resultType))
                {
                    SqlInt64 num3 = SqlConvert.ConvertToSqlInt64(lhs.Value) % SqlConvert.ConvertToSqlInt64(rhs.Value);
                    switch (resultType)
                    {
                        case StorageType.SqlInt32:
                            return num3.ToSqlInt32();

                        case StorageType.SqlInt16:
                            return num3.ToSqlInt16();

                        case StorageType.SqlByte:
                            return num3.ToSqlByte();
                    }
                    return num3;
                }
                else
                {
                    retValue = Convert.ToInt64(lhs.Value, context.FormatProvider) % Convert.ToInt64(rhs.Value, context.FormatProvider);
                    retValue = Convert.ChangeType(retValue, DataStorageHelper.GetTypeStorage(resultType), context.FormatProvider);
                    return retValue;
                }
            }

            return Convert.ToUInt64(lhs.Value, context.FormatProvider) % Convert.ToUInt64(rhs.Value, context.FormatProvider);
        }

        static object AndFunc(Operand lhs, Operand rhs, OperatorContext context)
        {
            if (!DataStorageHelper.IsObjectNull(lhs.Value)
                && (!DataStorageHelper.IsSqlType(lhs.Value) || !DataStorageHelper.IsObjectSqlNull(lhs.Value)))
            {
                if (!(lhs.Value is bool) && !(lhs.Value is SqlBoolean))
                {
                    throw InvalidExpressionException.TypeMismatch(Operator.And.ToString());
                }

                if (lhs.Value is bool)
                {
                    if (DataStorageHelper.IsObjectNull(rhs.Value)
                            || (DataStorageHelper.IsSqlType(rhs.Value) && DataStorageHelper.IsObjectSqlNull(rhs.Value)))
                    {
                        return DBNull.Value;
                    }

                    if ((bool)lhs.Value)
                    {
                        /*if ((rhs.Value == DBNull.Value)
                            || (DataStorageHelper.IsSqlType(rhs.Value) && DataStorageHelper.IsObjectSqlNull(rhs.Value)))
                        {
                            return DBNull.Value;
                        }*/
                        
                        if (!(rhs.Value is bool) && !(rhs.Value is SqlBoolean))
                        {
                            throw InvalidExpressionException.TypeMismatch(Operator.And.ToString());
                        }
                        else if (rhs.Value is bool)
                        {
                            return (bool)rhs.Value;
                        }
                        else
                        {
                            SqlBoolean flag5 = (SqlBoolean)rhs.Value;
                            return flag5.IsTrue;
                        }
                    }
                    return false;
                }
                SqlBoolean flag6 = (SqlBoolean)lhs.Value;
                if (flag6.IsFalse)
                {
                    return false;
                }
                return true;
            }
            return DBNull.Value;
        }

        static object OrFunc(Operand lhs, Operand rhs, OperatorContext context)
        {
            if (DataStorageHelper.IsObjectNull(lhs.Value))
            {
                if (DataStorageHelper.IsObjectNull(rhs.Value))
                {
                    return DBNull.Value;
                }

                if (!(rhs.Value is bool) && !(rhs.Value is SqlBoolean))
                {
                    throw InvalidExpressionException.TypeMismatch(Operator.Or.ToString());
                }

                return (rhs.Value is bool) ? ((bool)rhs.Value) : ((SqlBoolean)rhs.Value).IsTrue;
            }

            if (!(lhs.Value is bool) && !(lhs.Value is SqlBoolean))
            {
                throw InvalidExpressionException.TypeMismatch(Operator.Or.ToString());
            }

            if (DataStorageHelper.IsObjectNull(rhs.Value))
            {
                return lhs.Value;
            }

            if (!(rhs.Value is bool) && !(rhs.Value is SqlBoolean))
            {
                throw InvalidExpressionException.TypeMismatch(Operator.Or.ToString());
            }

            if ((bool)lhs.Value)
            {
                return true;
            }

            return (bool)rhs.Value;
        }

        static object NegativeFunc(Operand rhs, OperatorContext context)
        {
            object value = rhs.Value;

            if (DataStorageHelper.IsUnknown(value))
            {
                return DBNull.Value;
            }

            StorageType storageType = DataStorageHelper.GetStorageType(value.GetType());
            if (!DataStorageHelper.IsNumericSql(storageType))
            {
                throw InvalidExpressionException.TypeMismatch(Operator.Not.ToString());
            }
            
            switch (storageType)
            {
                case StorageType.Byte:
                    return -((byte)value);

                case StorageType.Int16:
                    return -((short)value);

                case StorageType.Int32:
                    return -((int)value);

                case StorageType.Int64:
                    return -((long)value);

                case StorageType.Single:
                    return -((float)value);

                case StorageType.Double:
                    return -((double)value);

                case StorageType.Decimal:
                    return -((decimal)value);

                case StorageType.SqlDecimal:
                    return -((SqlDecimal)value);

                case StorageType.SqlDouble:
                    return -((SqlDouble)value);

                case StorageType.SqlInt16:
                    return -((SqlInt16)value);

                case StorageType.SqlInt32:
                    return -((SqlInt32)value);

                case StorageType.SqlInt64:
                    return -((SqlInt64)value);

                case StorageType.SqlMoney:
                    return -((SqlMoney)value);

                case StorageType.SqlSingle:
                    return -((SqlSingle)value);
            }

            return DBNull.Value;
        }

        static object PositiveFunc(Operand rhs, OperatorContext context)
        {
            if (DataStorageHelper.IsUnknown(rhs.Value))
            {
                return DBNull.Value;
            }

            if (!DataStorageHelper.IsNumericSql(DataStorageHelper.GetStorageType(rhs.Value.GetType())))
            {
                throw InvalidExpressionException.TypeMismatch(Operator.UnaryPlus.ToString());
            }

            return rhs.Value;
        }

        static object NotFunc(Operand rhs, OperatorContext context)
        {
            object value = rhs.Value;

            if (DataStorageHelper.IsUnknown(value))
            {
                return DBNull.Value;
            }

            if (!(value is SqlBoolean))
            {
                if (DataStorageHelper.ToBoolean(value))
                {
                    return false;
                }
                return true;
            }
            SqlBoolean flag2 = (SqlBoolean)value;
            if (!flag2.IsFalse)
            {
                SqlBoolean flag = (SqlBoolean)value;
                if (!flag.IsTrue)
                {
                    throw InvalidExpressionException.UnsupportedOperator(Operator.Not);
                }
                return SqlBoolean.False;
            }
            return SqlBoolean.True;
        }

        static object InFunc(Operand lhs, Operand rhs, OperatorContext context)
        {
            /*if ((lhs.Value == DBNull.Value)
                || (DataStorageHelper.IsSqlType(lhs.Value) && DataStorageHelper.IsObjectSqlNull(lhs.Value)))
            {
                return DBNull.Value;
            }*/

            if (DataStorageHelper.IsObjectNull(lhs.Value))
            {
                return false;
            }

            IEnumerable enumerable = null;

            if (rhs.Node is FunctionNode 
                && ((FunctionNode)rhs.Node).IsGlobal
                && ((FunctionNode)rhs.Node).Name == "In")
            {
                FunctionNode func = (FunctionNode)rhs.Node;
                if (func.Arguments != null && func.Arguments.Count > 0)
                {
                    ArrayList args = new ArrayList(func.Arguments.Count);
                    for (int i = 0; i < func.Arguments.Count; i++)
                    {
                        object value = func.Arguments[i].Eval(context.Row, context.ExpressionContext);
                        args.Add(value);
                    }
                    enumerable = args;    
                }
            }
            else if (rhs.Value is IEnumerable)
            {
                enumerable = (IEnumerable)rhs.Value;
            }
            else
            {
                enumerable = new object[] { rhs.Value };
            }

            if (null != enumerable)
            {
                foreach (object value in enumerable)
                {
                    if ((value != DBNull.Value)
                        && (!DataStorageHelper.IsSqlType(value) || !DataStorageHelper.IsObjectSqlNull(value)))
                    {
                        StorageType resultType = DataStorageHelper.GetStorageType(lhs.Value.GetType());
                        if (0 == BinaryCompare(lhs.Value, value, resultType, EqualTo, context.FormatProvider))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        static object EqualToFunc(Operand lhs, Operand rhs, OperatorContext context)
        {
            StorageType resultType;
            object retValue;

            if (!Operator.GetResultType(Operator.EqualTo, lhs, rhs, out resultType, out retValue))
            {
                return retValue;
            }

            if (lhs.Value != DBNull.Value
                && (!DataStorageHelper.IsSqlType(lhs.Value) || !DataStorageHelper.IsObjectSqlNull(lhs.Value)) 
                && (rhs.Value != DBNull.Value
                && (!DataStorageHelper.IsSqlType(rhs.Value) || !DataStorageHelper.IsObjectSqlNull(rhs.Value))))
            {
                return (0 == BinaryCompare(lhs.Value
                    , rhs.Value
                    , resultType
                    , Operator.EqualTo
                    , context.FormatProvider));
            }
            return DBNull.Value;
        }

        static object GreaterThenFunc(Operand lhs, Operand rhs, OperatorContext context)
        {
            StorageType resultType;
            object retValue;

            if (!Operator.GetResultType(Operator.GreaterThen, lhs, rhs, out resultType, out retValue))
            {
                return retValue;
            }

            if (lhs.Value != DBNull.Value
                && (!DataStorageHelper.IsSqlType(lhs.Value) || !DataStorageHelper.IsObjectSqlNull(lhs.Value))
                && (rhs.Value != DBNull.Value
                && (!DataStorageHelper.IsSqlType(rhs.Value) || !DataStorageHelper.IsObjectSqlNull(rhs.Value))))
            {
                return (0 < BinaryCompare(lhs.Value
                    , rhs.Value
                    , resultType
                    , Operator.GreaterThen
                    , context.FormatProvider));
            }
            return DBNull.Value;
        }

        private static object GreaterOrEqualFunc(Operand lhs, Operand rhs, OperatorContext context)
        {
            StorageType resultType;
            object retValue;

            if (!Operator.GetResultType(Operator.GreaterOrEqual, lhs, rhs, out resultType, out retValue))
            {
                return retValue;
            }

            if (lhs.Value != DBNull.Value
                && (!DataStorageHelper.IsSqlType(lhs.Value) || !DataStorageHelper.IsObjectSqlNull(lhs.Value))
                && (rhs.Value != DBNull.Value
                && (!DataStorageHelper.IsSqlType(rhs.Value) || !DataStorageHelper.IsObjectSqlNull(rhs.Value))))
            {
                return (0 <= BinaryCompare(lhs.Value
                    , rhs.Value
                    , resultType
                    , Operator.GreaterOrEqual
                    , context.FormatProvider));
            }
            return DBNull.Value;
        }

        static object LessThenFunc(Operand lhs, Operand rhs, OperatorContext context)
        {
            StorageType resultType;
            object retValue;

            if (!Operator.GetResultType(Operator.LessThen, lhs, rhs, out resultType, out retValue))
            {
                return retValue;
            }

            if (lhs.Value != DBNull.Value
                && (!DataStorageHelper.IsSqlType(lhs.Value) || !DataStorageHelper.IsObjectSqlNull(lhs.Value))
                && (rhs.Value != DBNull.Value
                && (!DataStorageHelper.IsSqlType(rhs.Value) || !DataStorageHelper.IsObjectSqlNull(rhs.Value))))
            {
                return (0 > BinaryCompare(lhs.Value
                    , rhs.Value
                    , resultType
                    , Operator.LessThen
                    , context.FormatProvider));
            }
            return DBNull.Value;
        }

        static object LessOrEqualFunc(Operand lhs, Operand rhs, OperatorContext context)
        {
            StorageType resultType;
            object retValue;

            if (!Operator.GetResultType(Operator.LessOrEqual, lhs, rhs, out resultType, out retValue))
            {
                return retValue;
            }

            if (lhs.Value != DBNull.Value
                && (!DataStorageHelper.IsSqlType(lhs.Value) || !DataStorageHelper.IsObjectSqlNull(lhs.Value))
                && (rhs.Value != DBNull.Value
                && (!DataStorageHelper.IsSqlType(rhs.Value) || !DataStorageHelper.IsObjectSqlNull(rhs.Value))))
            {
                return (0 >= BinaryCompare(lhs.Value
                    , rhs.Value
                    , resultType
                    , Operator.LessOrEqual
                    , context.FormatProvider));
            }
            return DBNull.Value;
        }

        static object NotEqualFunc(Operand lhs, Operand rhs, OperatorContext context)
        {
            StorageType resultType;
            object retValue;

            if (!Operator.GetResultType(Operator.NotEqual, lhs, rhs, out resultType, out retValue))
            {
                return retValue;
            }

            if (lhs.Value != DBNull.Value
                && (!DataStorageHelper.IsSqlType(lhs.Value) || !DataStorageHelper.IsObjectSqlNull(lhs.Value))
                && (rhs.Value != DBNull.Value
                && (!DataStorageHelper.IsSqlType(rhs.Value) || !DataStorageHelper.IsObjectSqlNull(rhs.Value))))
            {
                return (0 != BinaryCompare(lhs.Value
                    , rhs.Value
                    , resultType
                    , Operator.NotEqual
                    , context.FormatProvider));
            }
            return DBNull.Value;
        }

        static object IsFunc(Operand lhs, Operand rhs, OperatorContext context)
        {
            // Impl as IS NULL?!
            if (!DataStorageHelper.IsObjectNull(lhs.Value)
                && (!DataStorageHelper.IsSqlType(lhs.Value) 
                    || !DataStorageHelper.IsObjectSqlNull(lhs.Value)))
            {
                return false;
            }
            return true;
        }

        static object IsNotFunc(Operand lhs, OperatorContext context)
        {
            // Impl as ISNOT NULL?!
            if (DataStorageHelper.IsObjectNull(lhs.Value)
                || (DataStorageHelper.IsSqlType(lhs.Value) 
                    && DataStorageHelper.IsObjectSqlNull(lhs.Value)))
            {
                return false;
            }
            return true;
        }

        static object BetweenFunc(Operand op1, Operand op2, Operand op3, OperatorContext context)
        {
            object value1 = op1.Value;
            object value2 = op2.Value;
            object value3 = op3.Value;

            StorageType resultType = DataStorageHelper.GetStorageType(value1.GetType());

            int result1 = BinaryCompare(value2, value1, resultType, Operator.LessOrEqual, context.FormatProvider);
            int result2 = BinaryCompare(value1, value3, resultType, Operator.LessOrEqual, context.FormatProvider);

            return (result1 <= 0 && result2 <= 0);
        }

        internal static int BinaryCompare(object value1
            , object value2
            , StorageType resultType
            , Operator op
            , IFormatProvider formatProvider)
        {
            //int num2 = 0;
            try
            {
                if (!DataStorageHelper.IsSqlType(resultType))
                {
                    switch (resultType)
                    {
                        case StorageType.Boolean:
                            if ((op != Operator.EqualTo) && (op != Operator.NotEqual))
                            {
                                goto Label_0436;
                            }
                            return (Convert.ToInt32(DataStorageHelper.ToBoolean(value1), formatProvider) - Convert.ToInt32(DataStorageHelper.ToBoolean(value2), formatProvider));

                        case StorageType.Char:
                            return Convert.ToInt32(value1, formatProvider).CompareTo(Convert.ToInt32(value2, formatProvider));

                        case StorageType.SByte:
                        case StorageType.Byte:
                        case StorageType.Int16:
                        case StorageType.UInt16:
                        case StorageType.Int32:
                            return Convert.ToInt32(value1, formatProvider).CompareTo(Convert.ToInt32(value2, formatProvider));

                        case StorageType.UInt32:
                        case StorageType.Int64:
                        case StorageType.UInt64:
                        case StorageType.Decimal:
                            return decimal.Compare(Convert.ToDecimal(value1, formatProvider), Convert.ToDecimal(value2, formatProvider));

                        case StorageType.Single:
                            return Convert.ToSingle(value1, formatProvider).CompareTo(Convert.ToSingle(value2, formatProvider));

                        case StorageType.Double:
                            return Convert.ToDouble(value1, formatProvider).CompareTo(Convert.ToDouble(value2, formatProvider));

                        case StorageType.DateTime:
                            return DateTime.Compare(Convert.ToDateTime(value1, formatProvider), Convert.ToDateTime(value2, formatProvider));

                        case StorageType.TimeSpan:
                            goto Label_0436;

                        case StorageType.String:
                            //return base.table.Compare(Convert.ToString(vLeft, formatProvider), Convert.ToString(vRight, formatProvider));
                            return string.Compare(Convert.ToString(value1, formatProvider), Convert.ToString(value2, formatProvider));

                        case StorageType.Guid:
                            {
                                Guid guid2 = (Guid)value1;
                                return guid2.CompareTo((Guid)value2);
                            }
                    }
                }
                else
                {
                    switch (resultType)
                    {
                        case StorageType.SByte:
                        case StorageType.Byte:
                        case StorageType.Int16:
                        case StorageType.UInt16:
                        case StorageType.Int32:
                        case StorageType.SqlByte:
                        case StorageType.SqlInt16:
                        case StorageType.SqlInt32:
                            return SqlConvert.ConvertToSqlInt32(value1).CompareTo(SqlConvert.ConvertToSqlInt32(value2));

                        case StorageType.UInt32:
                        case StorageType.Int64:
                        case StorageType.SqlInt64:
                            return SqlConvert.ConvertToSqlInt64(value1).CompareTo(SqlConvert.ConvertToSqlInt64(value2));

                        case StorageType.UInt64:
                        case StorageType.SqlDecimal:
                            return SqlConvert.ConvertToSqlDecimal(value1).CompareTo(SqlConvert.ConvertToSqlDecimal(value2));

                        case StorageType.Single:
                        case StorageType.Double:
                        case StorageType.Decimal:
                        case StorageType.DateTime:
                        case StorageType.TimeSpan:
                        case StorageType.String:
                        case StorageType.Guid:
                        case StorageType.ByteArray:
                        case StorageType.CharArray:
                        case StorageType.Type:
                        case StorageType.Uri:
                        case StorageType.SqlBytes:
                        case StorageType.SqlChars:
                            goto Label_0436;

                        case StorageType.SqlBinary:
                            return SqlConvert.ConvertToSqlBinary(value1).CompareTo(SqlConvert.ConvertToSqlBinary(value2));

                        case StorageType.SqlBoolean:
                            goto Label_031C;

                        case StorageType.SqlDateTime:
                            return SqlConvert.ConvertToSqlDateTime(value1).CompareTo(SqlConvert.ConvertToSqlDateTime(value2));

                        case StorageType.SqlDouble:
                            return SqlConvert.ConvertToSqlDouble(value1).CompareTo(SqlConvert.ConvertToSqlDouble(value2));

                        case StorageType.SqlGuid:
                            {
                                SqlGuid guid = (SqlGuid)value1;
                                return guid.CompareTo(value2);
                            }
                        case StorageType.SqlMoney:
                            return SqlConvert.ConvertToSqlMoney(value1).CompareTo(SqlConvert.ConvertToSqlMoney(value2));

                        case StorageType.SqlSingle:
                            return SqlConvert.ConvertToSqlSingle(value1).CompareTo(SqlConvert.ConvertToSqlSingle(value2));

                        case StorageType.SqlString:
                            //return base.table.Compare(vLeft.ToString(), vRight.ToString());
                            return string.Compare(value1.ToString(), value2.ToString());
                    }
                }
                goto Label_0436;
            Label_031C:
                if ((op == Operator.EqualTo) || (op == Operator.NotEqual))
                {
                    //num2 = 1;
                    if (((value1.GetType() == typeof(SqlBoolean)) && ((value2.GetType() == typeof(SqlBoolean)) || (value2.GetType() == typeof(bool)))) || ((value2.GetType() == typeof(SqlBoolean)) && ((value1.GetType() == typeof(SqlBoolean)) || (value1.GetType() == typeof(bool)))))
                    {
                        return SqlConvert.ConvertToSqlBoolean(value1).CompareTo(SqlConvert.ConvertToSqlBoolean(value2));
                    }
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine("" + exception);
            }
        
            Label_0436:
                throw InvalidExpressionException.TypeMismatchInBinop(op, value1.GetType(), value2.GetType());
            
            //return num2;
        }

        /// <summary>
        /// Use for all op except And, Or, In, Is and IsNot
        /// </summary>
        /// <returns>if false to stop processing the op and return the retValue</returns>
        internal static bool GetResultType(Operator op
            , Operand lhs
            , Operand rhs
            , out StorageType resultType
            , out object retValue)
        {
            object lValue = lhs.Value;
            object rValue = rhs.Value;
            
            retValue = null;
            resultType = StorageType.Empty;

            Type lType = null;
            Type rType = null;

            if (lValue != null)
            {
                lType = lValue.GetType();
            }

            if (rValue != null)
            {
                rType = rValue.GetType();
            }

            StorageType lStorageType = DataStorageHelper.GetStorageType(lType);
            StorageType rStorageType = DataStorageHelper.GetStorageType(rType);

            bool isSqlTypeL = DataStorageHelper.IsSqlType(lStorageType);
            bool isSqlTypeR = DataStorageHelper.IsSqlType(rStorageType);

            if (isSqlTypeL && DataStorageHelper.IsObjectSqlNull(lValue))
            {
                retValue = lValue;
                return false;
            }

            if (isSqlTypeR && DataStorageHelper.IsObjectSqlNull(rValue))
            {
                retValue = rValue;
                return false;
            }

            if (DataStorageHelper.IsObjectNull(lValue) || DataStorageHelper.IsObjectNull(rValue))
            {
                retValue = DBNull.Value;
                return false;
            }

            if (isSqlTypeL || isSqlTypeR)
            {
                resultType = DataStorageHelper.ResultSqlType(lStorageType
                                                , rStorageType
                                                , lhs.IsConst
                                                , rhs.IsConst
                                                , op);
            }
            else
            {
                resultType = DataStorageHelper.ResultType(lStorageType
                                             , rStorageType
                                             , lhs.IsConst
                                             , rhs.IsConst
                                             , op);
            }

            if (resultType == StorageType.Empty)
            {
                throw InvalidExpressionException.TypeMismatchInBinop(op, lType, rType);
            }

            return true;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    sealed class OperatorContext
    {
        object row;
        IFormatProvider formatProvider;
        object expressionContext;

        public IFormatProvider FormatProvider
        {
            get { return this.formatProvider; }
        }

        public object Row
        {
            get { return this.row; }
        }

        public object ExpressionContext
        {
            get { return this.expressionContext; }
        }

        public OperatorContext(object row, IFormatProvider formatProvider, object expressionContext)
        {
            this.row = row;
            this.formatProvider = formatProvider;
            this.expressionContext = expressionContext;
        }
    }

    /*
    /// <summary>
    /// 
    /// </summary>
    static class Priority
    {
        internal const int Start = 0;
        internal const int Substr = 1;
        internal const int Paren = 2;
        internal const int Low = 3;
        internal const int Imp = 4;
        internal const int Eqv = 5;
        internal const int Xor = 6;
        internal const int Or = 7;
        internal const int And = 8;
        internal const int Not = 9;
        internal const int Is = 10;
        internal const int BetweenInLike = 11; 
        internal const int BetweenAnd = 12;
        internal const int RelOp = 13;
        internal const int Concat = 14;
        internal const int Contains = 15;
        internal const int PlusMinus = 16;
        internal const int Mod = 17;
        internal const int IDiv = 18;
        internal const int MulDiv = 19;
        internal const int Neg = 20;
        internal const int Exp = 21;
        internal const int Proc = 22;
        internal const int Dot = 23;
        internal const int Max = 24;
    }
     */ 
}