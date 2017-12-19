namespace Telerik.Data.Expressions
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    class InvalidExpressionException : Exception
    {
        public static Exception AmbiguousBinop(Operator op, Type type1, Type type2)
        {
            return CreateException("Operator '{0}' is ambiguous on operands of type '{1}' and '{2}'. Cannot mix signed and unsigned types. Please use explicit Convert() function.", op, type1, type2);
        }

        public static Exception TypeMismatch(string exp)
        {
            return CreateException("Type mismatch in expression '{0}'.", exp);
        }

        public static Exception TypeMismatchInBinop(Operator op, Type type1, Type type2)
        {
            return CreateException("Cannot perform '{0}' operation on {1} and {2}.", op, type1, type2);
        }

        public static Exception DatavalueConvertion(object value, Type type, Exception innerException)
        {
            return CreateException(innerException, "Cannot convert value '{0}' to Type: {1}.");
        }

        public static Exception SqlConvertFailed(Type type1, Type type2)
        {
            return CreateException("Cannot convert from Type: '{0}' to Type: {1}.", type1, type2);
        }
        
        public static Exception InWithoutParentheses()
        {
            return CreateException("Syntax error: The items following the IN keyword must be separated by commas and be enclosed in parentheses.");
        }

        public static Exception UnsupportedOperator(Operator op)
        {
            return CreateException("The expression contains unsupported operator '{0}'.", op);
        }

        public static Exception UndefinedFunction(string name)
        {
            return CreateException("The expression contains undefined function call {0}().", name);
        }

        public static Exception UndefinedObject(string name)
        {
            return CreateException("The expression contains object '{0}' that is not defined in the current context.", name);
        }


        public static Exception ErrorInFunc(string name, Exception innerException)
        {
            return CreateException(innerException, "An error has occured while executing function {0}(). Check InnerException for further information.", name);
        }

        public static Exception ArgumentTypeInteger(string name, int index)
        {
            return CreateException("Type mismatch in function argument: {0}(), argument {1}, expected one of the Integer types.", name, index);
        }

        public static Exception InvalidPattern(string pattern)
        {
            return CreateException("Invalid LIKE pattern: {0}", pattern);
        }

        public InvalidExpressionException(string message) : base(message)
        {
        }

        public InvalidExpressionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        static Exception CreateException(string format, params object[] args)
        {
            return new InvalidExpressionException(string.Format(format, args));
        }

        static Exception CreateException(Exception innerException, string format, params object[] args)
        {
            return new InvalidExpressionException(string.Format(format, args), innerException);
        }
    }
}