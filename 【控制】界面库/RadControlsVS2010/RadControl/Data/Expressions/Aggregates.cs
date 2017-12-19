namespace Telerik.Data.Expressions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.SqlTypes;

    /// <summary>
    /// 
    /// </summary>
    static class Aggregates
    {
        static IEnumerable<object> NotNullValue(IEnumerable data
                                                , ExpressionNode node
                                                , object context)
        {
            foreach (object row in data)
            {
                object value = node.Eval(row, context);
                if (null != value && DBNull.Value != value)
                {
                    yield return value;
                }
            }
        }

        [Description("Returns a sum of the values of the specified expression.")]
        public static object Sum(FunctionContext context, IEnumerable data, ExpressionNode node)
        {
            int count;
            StorageType type;

            return Aggregates.Sum(data
                , node
                , context.GlobalContext
                , "Sum"
                , out count
                , out type);
        }

        [Description("Returns the average of all non-null values from the specified expression.")]
        public static object Avg(FunctionContext context, IEnumerable data, ExpressionNode node)
        {
            int count;
            StorageType type;

            return Aggregates.Mean(data
                , node
                , context.GlobalContext
                , "Avg"
                , out count
                , out type);
        }

        [Description("Returns the first value from the specified expression.")]
        public static object First(FunctionContext context, IEnumerable data, ExpressionNode node)
        {
            foreach (object value in Aggregates.NotNullValue(data, node, context.GlobalContext))
            {
                return value;
            }

            return null;
        }

        [Description("Returns the last value from the specified expression.")]
        public static object Last(FunctionContext context, IEnumerable data, ExpressionNode node)
        {
            object value = null;
            foreach (object value1 in Aggregates.NotNullValue(data, node, context.GlobalContext))
            {
                value = value1;
            }

            return value;
        }

        [Description("Returns the minimum value from all non-null values of the specified expression.")]
        public static object Min(FunctionContext context, IEnumerable data, ExpressionNode node)
        {
            object value = null;
            foreach (object value2 in Aggregates.NotNullValue(data, node, context.GlobalContext))
            {
                if (null == value)
                {
                    value = value2;
                }
                else if (0 < Aggregates.Compare(value, value2, context, "Min"))
                {
                    value = value2;
                }
            }

            return value;
        }

        [Description("Returns the maximum value from all non-null values of the specified expression.")]
        public static object Max(FunctionContext context, IEnumerable data, ExpressionNode node)
        {
            object value = null;
            foreach (object value2 in Aggregates.NotNullValue(data, node, context.GlobalContext))
            {
                if (null == value)
                {
                    value = value2;
                }
                else if (0 > Aggregates.Compare(value, value2, context, "Max"))
                {
                    value = value2;
                }
            }

            return value;
        }

        [Description("Returns a count of the values from the specified expression.")]
        public static object Count(FunctionContext context, IEnumerable data, ExpressionNode node)
        {
            int count = 0;
            foreach (object row in data)
            {
                count++;
            }
            return count;
        }

        static object StDev(IEnumerable data, string columnName, FunctionContext context)
        {
            throw new NotImplementedException();

            /*int count;
            StorageType type;

            object mean = Mean(rows, columnName, "StDev", out count, out type);
            double varp = 0;
            for (int i = 1; i <= count; i++)
            {
                //varp += (i - mean)*(i - mean);
            }

            // http://en.wikibooks.org/wiki/Statistics:Summary/Variance

            return Math.Sqrt(varp);*/
        }

        static object Var(IEnumerable data, string columnName, FunctionContext context)
        {
            throw new NotImplementedException();

            /*int count;
            StorageType type;

            object mean = Mean(rows, columnName, "StDev", out count, out type);
            double varp = 0;
            for (int i = 1; i <= count; i++)
            {
                //varp += (i - mean)*(i - mean);
            }
            // http://en.wikibooks.org/wiki/Statistics:Summary/Variance
            return varp;*/
        }

        /// <returns>
        /// -1: value1 &lt; value2
        ///  0: value1 = value2
        ///  1: value1 &gt; value2
        /// </returns>
        static int Compare(object value1
                           , object value2
                           , FunctionContext context
                           , string exp)
        {
            Type lType = value1.GetType();
            Type rType = value2.GetType();

            StorageType lStorageType = DataStorageHelper.GetStorageType(lType);
            StorageType rStorageType = DataStorageHelper.GetStorageType(rType);

            bool isSqlTypeL = DataStorageHelper.IsSqlType(lStorageType);
            bool isSqlTypeR = DataStorageHelper.IsSqlType(rStorageType);

            if ((isSqlTypeL && DataStorageHelper.IsObjectSqlNull(value1)) || value1 == DBNull.Value)
            {
                return -1;
            }

            if ((isSqlTypeR && DataStorageHelper.IsObjectSqlNull(value2)) || value2 == DBNull.Value)
            {
                return 1;
            }

            StorageType resultType = StorageType.Empty;
            if (isSqlTypeL || isSqlTypeR)
            {
                resultType = DataStorageHelper.ResultSqlType(lStorageType
                                                             , rStorageType
                                                             , false
                                                             , false
                                                             , Operator.LessThen); // op doesn't matter here
            }
            else
            {
                resultType = DataStorageHelper.ResultType(lStorageType
                                                          , rStorageType
                                                          , false
                                                          , false
                                                          , Operator.LessThen); // op doesn't matter here
            }

            if (resultType == StorageType.Empty)
            {
                throw InvalidExpressionException.TypeMismatch(exp);
            }

            return Operator.BinaryCompare(value1
                                          , value2
                                          , resultType
                                          , Operator.LessOrEqual  // op doesn't matter here
                                          , context.FormatProvider);
        }

        static object Sum(IEnumerable data
                          , ExpressionNode node
                          , object context
                          , string exp
                          , out int count
                          , out StorageType type)
        {
            count = 0;
            type = StorageType.Empty;

            //if (0 == view.Data.Count)
            //    return 0;

            object value = 0;
            foreach (object value2 in Aggregates.NotNullValue(data, node, context))
            {
                count++;

                if (type == StorageType.Empty)
                {
                    type = DataStorageHelper.GetStorageType(value2.GetType());
                    if (count == 1 && type == StorageType.String)
                    {
                        value = 0m;
                    }

                    if (!(type == StorageType.SqlDecimal
                          || type == StorageType.SqlMoney
                          || type == StorageType.Decimal
                          || type == StorageType.Single))
                    {
                        if (ExpressionNode.IsUnsignedSql(type))
                        {
                            type = StorageType.UInt64;
                        }
                        if (ExpressionNode.IsInteger(type))
                        {
                            type = StorageType.Int64;
                        }
                        else if (ExpressionNode.IsIntegerSql(type))
                        {
                            type = StorageType.SqlInt64;
                        }
                        else if (ExpressionNode.IsFloat(type))
                        {
                            type = StorageType.Double;
                        }
                        else if (ExpressionNode.IsFloatSql(type))
                        {
                            type = StorageType.SqlDouble;
                        }
                    }
                }

                value = Add(value, value2, type);
            }
            return value;
        }

        static object Add(object value1, object value2, StorageType type)
        {
            switch (type)
            {
                case StorageType.Char:
                    return Convert.ToChar(value1) + Convert.ToChar(value2);
                case StorageType.SByte:
                    return Convert.ToSByte(value1) + Convert.ToSByte(value2);
                case StorageType.Byte:
                    return Convert.ToByte(value1) + Convert.ToByte(value2);
                case StorageType.Int16:
                    return Convert.ToInt16(value1) + Convert.ToInt16(value2);
                case StorageType.UInt16:
                    return Convert.ToUInt16(value1) + Convert.ToUInt16(value2);
                case StorageType.Int32:
                    return Convert.ToInt32(value1) + Convert.ToInt32(value2);
                case StorageType.UInt32:
                    return Convert.ToUInt32(value1) + Convert.ToUInt32(value2);
                case StorageType.Int64:
                    return Convert.ToInt64(value1) + Convert.ToInt64(value2);
                case StorageType.UInt64:
                    return Convert.ToUInt64(value1) + Convert.ToUInt64(value2);
                case StorageType.Single:
                    return Convert.ToSingle(value1) + Convert.ToSingle(value2);
                case StorageType.Double:
                    return Convert.ToDouble(value1) + Convert.ToDouble(value2);
                case StorageType.Decimal:
                    return Convert.ToDecimal(value1) + Convert.ToDecimal(value2);
                case StorageType.String:
                    {
                        Decimal decimalValue1;
                        Decimal decimalValue2;
                        Decimal.TryParse(Convert.ToString(value1), out decimalValue1);
                        Decimal.TryParse(Convert.ToString(value2), out decimalValue2);
                        return decimalValue1 + decimalValue2;
                    }
                case StorageType.TimeSpan:
                    return (TimeSpan)value1 + (TimeSpan)value2;
                case StorageType.SqlByte:
                    return (SqlByte)value1 + (SqlByte)value2;
                case StorageType.SqlDecimal:
                    return (SqlDecimal)value1 + (SqlDecimal)value2;
                case StorageType.SqlDouble:
                    return (SqlDouble)value1 + (SqlDouble)value2;
                case StorageType.SqlInt16:
                    return (SqlInt16)value1 + (SqlInt16)value2;
                case StorageType.SqlInt32:
                    return (SqlInt32)value1 + (SqlInt32)value2;
                case StorageType.SqlInt64:
                    return (SqlInt64)value1 + (SqlInt64)value2;
                case StorageType.SqlMoney:
                    return (SqlMoney)value1 + (SqlMoney)value2;
                case StorageType.SqlSingle:
                    return (SqlSingle)value1 + (SqlSingle)value2;
                case StorageType.SqlString:
                    return (SqlString)value1 + (SqlString)value2;
                default:
                    throw InvalidExpressionException.TypeMismatch("Add");
            }
        }

        static object Subtract(object value1, object value2, StorageType type)
        {
            switch (type)
            {
                case StorageType.Char:
                    return Convert.ToChar(value1) - Convert.ToChar(value2);
                case StorageType.SByte:
                    return Convert.ToSByte(value1) - Convert.ToSByte(value2);
                case StorageType.Byte:
                    return Convert.ToByte(value1) - Convert.ToByte(value2);
                case StorageType.Int16:
                    return Convert.ToInt16(value1) - Convert.ToInt16(value2);
                case StorageType.UInt16:
                    return Convert.ToUInt16(value1) - Convert.ToUInt16(value2);
                case StorageType.Int32:
                    return Convert.ToInt32(value1) - Convert.ToInt32(value2);
                case StorageType.UInt32:
                    return Convert.ToUInt32(value1) - Convert.ToUInt32(value2);
                case StorageType.Int64:
                    return Convert.ToInt64(value1) - Convert.ToInt64(value2);
                case StorageType.UInt64:
                    return Convert.ToUInt64(value1) - Convert.ToUInt64(value2);
                case StorageType.Single:
                    return Convert.ToSingle(value1) - Convert.ToSingle(value2);
                case StorageType.Double:
                    return Convert.ToDouble(value1) - Convert.ToDouble(value2);
                case StorageType.Decimal:
                    return Convert.ToDecimal(value1) - Convert.ToDecimal(value2);
                case StorageType.String:
                    {
                        Decimal decimalValue1;
                        Decimal decimalValue2;
                        Decimal.TryParse(Convert.ToString(value1), out decimalValue1);
                        Decimal.TryParse(Convert.ToString(value2), out decimalValue2);
                        return decimalValue1 - decimalValue2;
                    }
                case StorageType.TimeSpan:
                    return (TimeSpan)value1 - (TimeSpan)value2;
                case StorageType.SqlByte:
                    return (SqlByte)value1 + (SqlByte)value2;
                case StorageType.SqlDecimal:
                    return (SqlDecimal)value1 - (SqlDecimal)value2;
                case StorageType.SqlDouble:
                    return (SqlDouble)value1 - (SqlDouble)value2;
                case StorageType.SqlInt16:
                    return (SqlInt16)value1 - (SqlInt16)value2;
                case StorageType.SqlInt32:
                    return (SqlInt32)value1 - (SqlInt32)value2;
                case StorageType.SqlInt64:
                    return (SqlInt64)value1 - (SqlInt64)value2;
                case StorageType.SqlMoney:
                    return (SqlMoney)value1 - (SqlMoney)value2;
                case StorageType.SqlSingle:
                    return (SqlSingle)value1 - (SqlSingle)value2;
                default:
                    throw InvalidExpressionException.TypeMismatch("Subtract");
            }
        }

        static object Multiply(object value1, object value2, StorageType type)
        {
            switch (type)
            {
                case StorageType.Char:
                    return Convert.ToChar(value1) * Convert.ToChar(value2);
                case StorageType.SByte:
                    return Convert.ToSByte(value1) * Convert.ToSByte(value2);
                case StorageType.Byte:
                    return Convert.ToByte(value1) * Convert.ToByte(value2);
                case StorageType.Int16:
                    return Convert.ToInt16(value1) * Convert.ToInt16(value2);
                case StorageType.UInt16:
                    return Convert.ToUInt16(value1) * Convert.ToUInt16(value2);
                case StorageType.Int32:
                    return Convert.ToInt32(value1) * Convert.ToInt32(value2);
                case StorageType.UInt32:
                    return Convert.ToUInt32(value1) * Convert.ToUInt32(value2);
                case StorageType.Int64:
                    return Convert.ToInt64(value1) * Convert.ToInt64(value2);
                case StorageType.UInt64:
                    return Convert.ToUInt64(value1) * Convert.ToUInt64(value2);
                case StorageType.Single:
                    return Convert.ToSingle(value1) * Convert.ToSingle(value2);
                case StorageType.Double:
                    return Convert.ToDouble(value1) * Convert.ToDouble(value2);
                case StorageType.Decimal:
                    return Convert.ToDecimal(value1) * Convert.ToDecimal(value2);
                case StorageType.String:
                    {
                        Decimal decimalValue1;
                        Decimal decimalValue2;
                        Decimal.TryParse(Convert.ToString(value1), out decimalValue1);
                        Decimal.TryParse(Convert.ToString(value2), out decimalValue2);
                        return decimalValue1 * decimalValue2;
                    }
                case StorageType.SqlByte:
                    return (SqlByte)value1 * (SqlByte)value2;
                case StorageType.SqlDecimal:
                    return (SqlDecimal)value1 * (SqlDecimal)value2;
                case StorageType.SqlDouble:
                    return (SqlDouble)value1 * (SqlDouble)value2;
                case StorageType.SqlInt16:
                    return (SqlInt16)value1 * (SqlInt16)value2;
                case StorageType.SqlInt32:
                    return (SqlInt32)value1 * (SqlInt32)value2;
                case StorageType.SqlInt64:
                    return (SqlInt64)value1 * (SqlInt64)value2;
                case StorageType.SqlMoney:
                    return (SqlMoney)value1 * (SqlMoney)value2;
                case StorageType.SqlSingle:
                    return (SqlSingle)value1 * (SqlSingle)value2;
                default:
                    throw InvalidExpressionException.TypeMismatch("Multiply");
            }
        }

        static object Mean(IEnumerable data
                           , ExpressionNode node
                           , object context
                           , string exp
                           , out int count
                           , out StorageType type)
        {
            count = 0;
            type = StorageType.Empty;

            //if (0 == view.Data.Count)
            //    return 0;

            object sum = Sum(data, node, context, exp, out count, out type);

            if (0 == count)
               return 0;

            switch (type)
            {
                case StorageType.Char:
                    return (char)sum / count;

                case StorageType.SByte:
                    return (SByte)sum / count;

                case StorageType.Byte:
                    return (Byte)sum / count;

                case StorageType.Int16:
                    return (Int16)sum / count;

                case StorageType.UInt16:
                    return (UInt16)sum / count;

                case StorageType.Int32:
                    return (Int32)sum / count;

                case StorageType.UInt32:
                    return (UInt32)sum / count;

                case StorageType.Int64:
                    return (Int64)sum / count;

                case StorageType.UInt64:
                    return (UInt64)sum / (UInt64)count;

                case StorageType.Single:
                    return (Single)sum / count;

                case StorageType.Double:
                    return (Double)sum / count;

                case StorageType.Decimal:
                    return (Decimal)sum / count;

                case StorageType.String:
                    return (Decimal)sum / count;

                case StorageType.SqlByte:
                    return (SqlByte)sum / (byte)count;

                case StorageType.SqlDecimal:
                    return (SqlDecimal)sum / count;

                case StorageType.SqlDouble:
                    return (SqlDouble)sum / count;

                case StorageType.SqlInt16:
                    return (SqlInt16)sum / (short)count;

                case StorageType.SqlInt32:
                    return (SqlInt32)sum / count;

                case StorageType.SqlInt64:
                    return (SqlInt64)sum / count;

                case StorageType.SqlMoney:
                    return (SqlMoney)sum / count;

                case StorageType.SqlSingle:
                    return (SqlSingle)sum / count;

                default:
                    throw InvalidExpressionException.TypeMismatch(exp);
            }
        }
    }
}