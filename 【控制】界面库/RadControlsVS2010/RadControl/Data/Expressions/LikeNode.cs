namespace Telerik.Data.Expressions
{
    using System;
    using System.Data;
    using System.Data.SqlTypes;
    using System.Globalization;

    /// <summary>
    /// 
    /// </summary>
    class LikeNode : BinaryOpNode
    {
		private CompareOptions compareFlags;

		public LikeNode(Operator op, ExpressionNode left, ExpressionNode right, CompareOptions compareFlags) 
            : base(op, left, right)
        {
			this.compareFlags = compareFlags;
        }

        public override object Eval(object row, object context)
        {
            object lValue = base.Left.Eval(row, context);

            if (!DataStorageHelper.IsObjectNull(lValue))
            {
                if (!(lValue is string || lValue is SqlString))
                {
                    throw InvalidExpressionException.TypeMismatchInBinop(Operator.Like, lValue.GetType(), typeof(string));
                }

                object rValue = base.Right.Eval(row, context);
                if (DataStorageHelper.IsObjectNull(rValue))
                {
                    return DBNull.Value;
                }

                if (!(rValue is string || rValue is SqlString))
                {
                    throw InvalidExpressionException.TypeMismatchInBinop(Operator.Like, typeof(string), rValue.GetType());
                }

                string text = lValue.ToString();
                string pattern = this.AnalizePattern(rValue.ToString());

                char[] trimChars = new char[] { ' ', '\u3000' };
                text = text.TrimEnd(trimChars);

                CompareInfo compareInfo = base.Culture.CompareInfo;
                switch (this.kind)
                {
                    case match_left:
                        return (0 == compareInfo.IndexOf(text, pattern, this.compareFlags));

                    case match_right:
                        {
                            string text4 = pattern.TrimEnd(trimChars);
                            return compareInfo.IsSuffix(text, text4, this.compareFlags);
                        }
                    case match_middle:
                        return (0 <= compareInfo.IndexOf(text, pattern, this.compareFlags));

                    case match_exact:
                        return (0 == compareInfo.Compare(text, pattern, this.compareFlags));

                    case match_all:
                        return true;
                }
            }

            return DBNull.Value;
        }

        int kind;

        const int match_left = 1;
        const int match_right = 2;
        const int match_middle = 3;
        const int match_exact = 4;
        const int match_all = 5;
        
        internal string AnalizePattern(string pat)
        {
            int count = pat.Length;
            char[] destination = new char[count + 1];
            pat.CopyTo(0, destination, 0, count);
            destination[count] = '\0';
            string text = null;
            char[] chArray2 = new char[count + 1];
            int length = 0;
            int wildCharOccurences = 0;
            int index = 0;
            
            while (index < count)
            {
                if ((destination[index] == '*') || (destination[index] == '%'))
                {
                    while (((destination[index] == '*') || (destination[index] == '%')) && (index < count))
                    {
                        // eat wild chars
                        index++;
                    }
                    if (((index < count) && (length > 0)) || (wildCharOccurences >= 2))
                    {
                        // max 2 wild chars are allowed
                        throw InvalidExpressionException.InvalidPattern(pat);
                    }
                    wildCharOccurences++;
                }
                else if (destination[index] == '[')
                {
                    index++;
                    if (index >= count)
                    {
                        throw InvalidExpressionException.InvalidPattern(pat);
                    }
                    chArray2[length++] = destination[index++];
                    if (index >= count)
                    {
                        throw InvalidExpressionException.InvalidPattern(pat);
                    }
                    if (destination[index] != ']')
                    {
                        throw InvalidExpressionException.InvalidPattern(pat);
                    }
                    index++;
                }
                else
                {
                    chArray2[length++] = destination[index];
                    index++;
                }
            }
            text = new string(chArray2, 0, length);
            if (wildCharOccurences == 0)
            {
                this.kind = match_exact;
                return text;
            }
            if (length > 0)
            {
                if ((destination[0] == '*') || (destination[0] == '%'))
                {
                    if ((destination[count - 1] == '*') || (destination[count - 1] == '%'))
                    {
                        this.kind = match_middle;
                        return text;
                    }
                    this.kind = match_right;
                    return text;
                }
                this.kind = match_left;
                return text;
            }
            this.kind = match_all;
            return text;
        }
    }
}