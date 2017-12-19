namespace Telerik.Data.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    interface ILexerClient
    {
        OperandType PrevOperand { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    class Lexer
    {
        const char Escape = '\\';
        const char DecimalSeparator = '.';
        const char ExponentL = 'e';
        const char ExponentU = 'E';

        ILexerClient client;
        int startPos;
        Token token = Token.None;
        Operator op = null;
        StringReader reader;

        public string TokenString
        {
            get
            {
                return this.reader.GetString(this.startPos, this.reader.Position - this.startPos);
            }
        }

        public Operator Operator
        {
            get { return this.op; }
        }

        public int StartPos
        {
            get { return this.startPos; }
        }

        public Lexer(ILexerClient client, string text)
        {
            this.reader = new StringReader(text);
            this.client = client;
        }

        public void StartRead()
        {
            this.reader.Restart();
            this.startPos = 0;
        }

        public Token Read(Token token)
        {
            Token token1 = this.Read();
            this.CheckToken(token);
            return token1;
        }

        public Token Read()
        {
            bool toStop = true;

            this.op = null;
            this.token = Token.None;

            do
            {
                toStop = true;
                this.startPos = this.reader.Position;

                char ch = this.reader.Read();
                switch (ch)
                {
                case '\0':
                    this.token = Token.EOF;
                    break;

                case ' ':
                case '\t':
                case '\n':
                case '\r':
                    this.ReadWhiteSpaces();
                    toStop = false;
                    break;

                case '(':
                    this.token = Token.LeftParen;
                    break;

                case ')':
                    this.token = Token.RightParen;
                    break;

                case '+':
                    this.op = Operator.Plus; 
                    this.token = Token.BinaryOp;
                    break;

                case '-':
                    this.op = Operator.Minus; 
                    this.token = Token.BinaryOp;
                    break;

                case '*':
                    this.op = Operator.Multiply; 
                    this.token = Token.BinaryOp;
                    break;

                case '/':
                    this.op = Operator.Divide; 
                    this.token = Token.BinaryOp;
                    break;

                case '%':
                    this.op = Operator.Modulo; 
                    this.token = Token.BinaryOp;
                    break;

                case '&':
                    this.op = Operator.BitwiseAnd; 
                    this.token = Token.BinaryOp;
                    break;

                case '|':
                    this.op = Operator.BitwiseOr; 
                    this.token = Token.BinaryOp;
                    break;

                case '^':
                    this.op = Operator.BitwiseXor; 
                    this.token = Token.BinaryOp;
                    break;

                case '~':
                    this.op = Operator.BitwiseNot; 
                    this.token = Token.BinaryOp;
                    break;

                case '=':
                    this.op = Operator.EqualTo; 
                    this.token = Token.BinaryOp;
                    break;

                case '<':   
                    this.token = Token.BinaryOp;
                    this.ReadWhiteSpaces();
                    if ('=' == this.reader.Current)
                    {
                        this.op = Operator.LessOrEqual;
                        this.reader.Read();
                    }
                    else if ('>' == this.reader.Current)
                    {
                        this.op = Operator.NotEqual;
                        this.reader.Read();
                    }
                    else
                    {
                        this.op = Operator.LessThen; 
                    }
                    break;

                case '>':   
                    this.token = Token.BinaryOp;
                    this.ReadWhiteSpaces();
                    if ('=' == this.reader.Current)
                    {
                        this.op = Operator.GreaterOrEqual;
                        this.reader.Read();
                    }
                    else
                    {
                        this.op = Operator.GreaterThen; 
                    }
                    break;

                case '#':   // date
                    this.ReadDate();
                    this.CheckToken(Token.Date);
                    break;

                case '\'':  // string const
                    this.ReadString('\'');
                    this.CheckToken(Token.StringConst);
                    break;

                case '"':  // string const
                    this.ReadString('"');
                    this.CheckToken(Token.StringConst);
                    break;

                case '`':
                    this.ReadName('`', '`', "`");
                    this.CheckToken(Token.Name);
                    break;

                case '[':
                    this.ReadName(']', Lexer.Escape, @"]\");
                    this.CheckToken(Token.Name);
                    break;

                case '.':
                    if (OperandType.None == this.client.PrevOperand)
                    {
                        this.ReadNumber();
                    }
                    else 
                    {
                        this.token = Token.Dot;
                    }
                    break;

                case ',':
                    this.token = Token.ListSeparator;
                    break;


                case '@':
                    this.token = Token.Parameter;
                    break;

                //case '!':
                //        break;

                default:
                    if ('0' == ch && ('x' == this.reader.Current || 'X' == this.reader.Current))
                    {
                        this.reader.Read();

                        this.ReadHex();
                        this.token = Token.NumericHex;
                    }
                    else if (char.IsDigit(ch))
                    {
                        this.ReadNumber();          
                    }
                    else
                    {
                        this.ReadReserved();
                        if (Token.None == this.token)
                        {
                            if (char.IsLetter(ch) || '_' == ch)
                            {
                                this.ReadName();
                                if (Token.None != this.token)
                                {
                                    this.CheckToken(Token.Name);
                                    break;
                                }
                            }
                            this.token = Token.Unknown;
                            throw LexicalException.UnknownToken(this.TokenString, this.startPos + 1);
                        }
                    }
                    break;
                }
            } while (!toStop);

            return this.token;
        }

        void ReadReserved()
        {
            if (char.IsLetter(this.reader.Current))
            {
                this.ReadName();
                ReservedWord word = ReservedWords.Lookup(this.TokenString);
                if (null != word)
                {
                    this.token = word.Token;
                    this.op = word.Operator;
                }
            }
        }

        void ReadHex()
        {
            while (char.IsLetterOrDigit(this.reader.Current))
            {
                this.reader.Read();
            }

            string s = this.reader.GetString(this.startPos + 2, this.reader.Position); // skip leading 0x
            foreach(char ch in s)
            {
                if (!Utils.IsHexDigit(ch))
                {
                    throw LexicalException.InvalidHex(s);
                }
            }
        }

        void ReadNumber()
        {
            bool isDecimal = false;
            bool isExponential = false;

            this.ReadDigits();

            if (DecimalSeparator == this.reader.Current)
            {
                isDecimal = true;
                this.reader.Read();
            }

            this.ReadDigits();

            if (ExponentL == this.reader.Current || ExponentU == this.reader.Current)
            {
                isExponential = true;
                this.reader.Read();

                if ('-' == this.reader.Current || '+' == this.reader.Current)
                {
                    this.reader.Read();
                }

                this.ReadDigits();
            }

            if (isExponential)
            {
                this.token = Token.Float;
            }
            else if (isDecimal)
            {
                this.token = Token.Decimal;
            }
            else
            {
                this.token = Token.Numeric;
            }
        }

        void ReadDigits()
        {
            while (char.IsDigit(this.reader.Current))
            {
                this.reader.Read();
            }
        }

        void ReadName()
        {
            while (char.IsLetterOrDigit(this.reader.Current) 
                || '_' == this.reader.Current)
            {
                this.reader.Read();
            }
            this.token = Token.Name;
        }

        void ReadName(char endChar, char escape, string charsToEscape)
        {
            do
            {
                if (escape == this.reader.Current
                    && this.reader.CanRead //(this.pos + 1 < this.buffer.Length)
                    && (charsToEscape.IndexOf(this.reader.Peek()) != -1))
                {
                    this.reader.Read();
                }
                this.reader.Read();
            }
            while (!this.reader.End && this.reader.Current != endChar);    //while (this.pos < this.buffer.Length || this.reader.Current != endChar);

            if (this.reader.End)     //if (this.pos >= this.buffer.Length)
            {
                throw LexicalException.InvalidName(this.TokenString);
            }

            this.token = Token.Name;
            this.reader.Read();
        }

        void ReadString(char escape)
        {
            while (!this.reader.End)   //while (this.pos < this.buffer.Length)
            {
                char ch = this.reader.Read();
                if (ch == escape)
                {
                    if (!this.reader.End && escape == this.reader.Current)     //if (this.pos < this.buffer.Length && escape == this.reader.Current)
                    {
                        this.reader.Read();
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (this.reader.End)     //if (this.pos >= this.buffer.Length)
            {
                throw LexicalException.InvalidString(this.TokenString);
            }

            this.token = Token.StringConst;
        }

        void ReadDate()
        {
            do
            {
                this.reader.Read();
            }
            while (!this.reader.End && '#' != this.reader.Current);    //while (this.pos < this.buffer.Length && '#' != this.reader.Current);

            if (this.reader.End || '#' != this.reader.Current)   //if (this.pos >= this.buffer.Length || '#' != this.reader.Current)
            {
                // Date is invalid
                throw LexicalException.InvalidDate(this.TokenString);
            }

            this.token = Token.Date;
            this.reader.Read();
        }

        void ReadWhiteSpaces()
        {
            while (!this.reader.End && Char.IsWhiteSpace(this.reader.Current)) //while (this.pos < this.buffer.Length && Char.IsWhiteSpace(this.reader.Current))
            {
                this.reader.Read();
            }
        }

        void CheckToken(Token token)
        {
            if (token != this.token)
            {
                throw LexicalException.UnexpectedToken(Utils.TokenToString(this.token)
                                                                 , Utils.TokenToString(token)
                                                                 , this.reader.Position);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        class StringReader
        {
            int position = 0;
            char[] buffer;

            public char Current
            {
                get { return this.buffer[this.position]; }    
            }

            public int Position
            {
                get { return this.position; }
            }

            public int Length
            {
                get { return this.buffer.Length; }
            }

            public bool CanRead
            {
                get { return ((this.position + 1) < this.Length); }
            }

            public bool End
            {
                get { return (this.position >= this.Length); }
            }

            public StringReader(string text)
            {
                this.LoadString(text);
            }

            public char Read()
            {
                return this.buffer[this.position++];
            }

            public char Peek()
            {
                return this.buffer[this.position + 1];
            }

            public void Restart()
            {
                this.position = 0;
            }

            public string GetString(int start, int length)
            {
                int len = (this.buffer.Length - 1);   // exclude the trailing '\0'
                if (start + length > len)
                {
                    length = len - start;
                }

                return new string(this.buffer, start, length);
            }

            void LoadString(string text)
            {
                int count = 0;
                if (string.IsNullOrEmpty(text))
                {
                    this.buffer = new char[1];
                }
                else
                {
                    count = text.Length;
                    this.buffer = new char[count + 1];
                    text.CopyTo(0, this.buffer, 0, count);
                }

                this.buffer[count] = '\0'; // EOF
            }
        }

        /// <summary>
        /// 
        /// </summary>
        class ReservedWord
        {
            public readonly string Word;
            public readonly Token Token;
            public readonly Operator Operator;

            public ReservedWord(string word, Token token, Operator op)
            {
                this.Word = word;
                this.Token = token;
                this.Operator = op;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static class ReservedWords
        {
            static readonly List<ReservedWord> list;

            static ReservedWords()
            {
                ReservedWords.list = new List<ReservedWord>(new ReservedWord[]
                    { 
                        new ReservedWord("And", Token.BinaryOp, Operator.And)
                        , new ReservedWord("Or", Token.BinaryOp, Operator.Or)
                        , new ReservedWord("Between", Token.TernaryOp, Operator.Between)
                        , new ReservedWord("In", Token.BinaryOp, Operator.In)
                        , new ReservedWord("Is", Token.BinaryOp, Operator.Is)
                        , new ReservedWord("Like", Token.BinaryOp, Operator.Like)
                        , new ReservedWord("Not", Token.UnaryOp, Operator.Not)
                        , new ReservedWord("True", Token.ZeroOp, Operator.True) 
                        , new ReservedWord("False", Token.ZeroOp, Operator.False)
                        , new ReservedWord("Mod", Token.BinaryOp, Operator.Modulo)
                        , new ReservedWord("Null", Token.ZeroOp, Operator.Null)
                    });

                // Reserver word list should be ordered alphabetically -- the search algorithm relies on this order.
                list.Sort(ReservedWords.Compare);
            }

            static int Compare(ReservedWord reserved1, ReservedWord reserved2)
            {
                return string.Compare(reserved1.Word, reserved2.Word, false, CultureInfo.InvariantCulture);
            }

            public static ReservedWord Lookup(string text)
            {
                int firstIndex = 0;
                int lastIndex = ReservedWords.list.Count - 1;

                do
                {
                    int index = (firstIndex + lastIndex) / 2;
                    int res = string.Compare(ReservedWords.list[index].Word
                                                  , text
                                                  , true
                                                  , CultureInfo.InvariantCulture);

                    if (0 == res)
                    {
                        return ReservedWords.list[index];
                    }

                    if (res < 0)
                    {
                        firstIndex = index + 1;
                    }
                    else
                    {
                        lastIndex = index - 1;
                    }
                } while (firstIndex <= lastIndex);
                return null;
            }
        }
    }
}