namespace Telerik.Data.Expressions
{
    using System;
    using System.ComponentModel;
    using System.Data.SqlTypes;
    using System.Globalization;
    using System.Text.RegularExpressions;
   // using Processing.Expressions;

    static class ExpressionUtils
    {
        // All LOWER case!!!
        static readonly string[] aggregates = {
                                                  "sum",   "avg",   "count",
                                                  "first", "last",  "min",
                                                  "max",   "stdev", "var",
                                              };

        // Poor man's expression parser.
        // TODO: Add support for embedded expressions.
        // TODO: Use the "real" expression parser/expression tree to find if the expression contains an aggregate function.

        public static bool IsExpression(string value)
        {
            return ("" + value).StartsWith("=");
        }

        public static bool IsAggregateExpression(string value)
        {
            value = value.ToLower();
            foreach (string aggr in aggregates)
            {
                string exp = string.Format("={0}(", aggr);
                if (value.StartsWith(exp))
                    return true;
            }

            return false;
        }

        public static string GetFieldReference(string value)
        {
            // =Fields.XXX
            Match m = Regex.Match(value, @"^=Fields\.(\w+)$");
            if (!m.Success)
            {
                // =XXX
                m = Regex.Match(value, @"^=(\w+)$");
            }

            if (m.Success)
            {
                return m.Groups[1].Value;
            }

            return string.Empty;
        }

        public static bool IsNumericType(System.Type type)
        {
            return (
                       //type == typeof(object)
                   //|| type == typeof(DBNull)
                   //|| type == typeof(bool)
                   //|| type == typeof(char)|| 
                   type == typeof(sbyte)
                   || type == typeof(byte)
                   || type == typeof(short)
                   || type == typeof(ushort)
                   || type == typeof(int)
                   || type == typeof(uint)
                   || type == typeof(long)
                   || type == typeof(ulong)
                   || type == typeof(float)
                   || type == typeof(double)
                   || type == typeof(decimal)
                   //|| type == typeof(DateTime)
                   //|| type == typeof(TimeSpan)
                   //|| type == typeof(string)
                   //|| type == typeof(Guid)
                   //|| type == typeof(byte[])
                   //|| type == typeof(char[])
                   //|| type == typeof(Type)
                   //|| type == typeof(DateTimeOffset)
                   //|| type == typeof(Uri)
                   //|| type == typeof(SqlBinary)
                   //|| type == typeof(SqlBoolean)
                   || type == typeof(SqlByte)
                   //|| type == typeof(SqlBytes)
                   //|| type == typeof(SqlChars)
                   //|| type == typeof(SqlDateTime)
                   || type == typeof(SqlDecimal)
                   || type == typeof(SqlDouble)
                   //|| type == typeof(SqlGuid)
                   || type == typeof(SqlInt16)
                   || type == typeof(SqlInt32)
                   || type == typeof(SqlInt64)
                   || type == typeof(SqlMoney)
                   || type == typeof(SqlSingle)
                   //|| type == typeof(SqlString)
                   );
        }

        public static string SplitName(string name)
        {
            return Regex.Replace(name, @"(\p{Ll})(\p{Lu})|_+", "$1 $2");
        }
    }
}