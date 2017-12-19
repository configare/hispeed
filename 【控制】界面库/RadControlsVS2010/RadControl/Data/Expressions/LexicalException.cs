namespace Telerik.Data.Expressions
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    class LexicalException : InvalidExpressionException
    {
        public static Exception InvalidDate(string date)
        {
            return CreateException("The expression contains invalid date constant '{0}'.", date);
        }

        public static Exception UnexpectedToken(string expectedToken, string currentToken, int pos)
        {
            return CreateException("Expected {0}, but actual token at the position {2} is {1}.", expectedToken, currentToken, pos);
        }

        public static Exception UnknownToken(string token, int position)
        {
            return CreateException("Cannot interpret token '{0}' at position {1}.", token, position);
        }

        public static Exception InvalidString(string str)
        {
            return CreateException("The expression contains an invalid string constant: {0}.", str);
        }

        public static Exception InvalidName(string name)
        {
            return CreateException("The expression contains invalid name: '{0}'.", name);
        }

        public static Exception InvalidHex(string text)
        {
            return CreateException("The expression contains invalid hexadecimal number: '{0}'.", text);
        }

        static Exception CreateException(string format, params object[] args)
        {
            return new LexicalException(string.Format(format, args));
        }

        public LexicalException(string message) : base(message)
        {
        }
    }
}