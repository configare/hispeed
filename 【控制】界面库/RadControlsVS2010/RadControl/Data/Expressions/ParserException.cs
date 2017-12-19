namespace Telerik.Data.Expressions
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    class ParserException : InvalidExpressionException
    {
        public static Exception MissingOperator(string token)
        {
            return CreateException("Missing operator before {0} operand.", token);
        }

        public static Exception MissingOperandBefore(string op)
        {
            return CreateException("Syntax error: Missing operand before '{0}' operator.", op);
        }
        
        public static Exception UnsupportedOperator(string op)
        {
            return CreateException("The expression contains unsupported operator '{0}'.", op);
        }

        public static Exception MissingRightParen()
        {
            return CreateException("The expression is missing the closing parenthesis.");
        }

        public static Exception MissingOperand(string op)
        {
            return CreateException("Syntax error: Missing operand after '{0}' operator.", op);
        }

        public static Exception SystaxError()
        {
            return CreateException("Syntax error in the expression.");
        }

        public static Exception UnknownToken(string token, int position)
        {
            return CreateException("Cannot interpret token '{0}' at position {1}.", token, position);
        }

        public static Exception TooManyRightParentheses()
        {
            return CreateException("The expression has too many closing parentheses.");
        }

        public static Exception AggregateArgument()
        {
            return CreateException("Syntax error in aggregate argument: Expecting a single column argument.");
        }

        ParserException(string message) : base(message)
        {
        }

        static Exception CreateException(string format, params object[] args)
        {
            return new ParserException(string.Format(format, args));
        }
    }
}