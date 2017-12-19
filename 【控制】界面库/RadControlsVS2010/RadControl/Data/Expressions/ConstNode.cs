namespace Telerik.Data.Expressions
{
    using System;
    using System.Data;
    using System.Globalization;

    /// <summary>
    /// 
    /// </summary>
    class ConstNode : ExpressionNode
    {
        object value;

        public override bool IsConst
        {
            get { return true; }
        }

        public ConstNode(ValueType type, string text)
        {
            this.Init(type, text);
        }

        public override object Eval(object row, object context)
        {
            return this.value;
        }

        void Init(ValueType type, string text)
        {
            switch (type)
            {
                case ValueType.Null:
                    this.value = DBNull.Value;
                    break;

                case ValueType.Bool:
                    this.value = Convert.ToBoolean(text);
                    break;

                case ValueType.Numeric:
                    try
                    {
                        int num = Convert.ToInt32(text);
                        this.value = num;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            int num = int.Parse(text, NumberStyles.HexNumber);
                            this.value = num;
                        }
                        catch (Exception)
                        {
                            try
                            {
                                long num = Convert.ToInt64(text);
                                this.value = num;
                            }
                            catch (Exception)
                            {
                                try
                                {
                                    double num = Convert.ToDouble(text, NumberFormatInfo.InvariantInfo);
                                    this.value = num;
                                }
                                catch (Exception)
                                {
                                    this.value = text;
                                }
                            }
                        }
                    }
                    break;

                case ValueType.String:
                    // Unescape the escaped chars -- same as the leading/trailing symbol
                    char escape = text[0];
                    char[] buffer = text.ToCharArray(1, text.Length - 2); // Discard the leading/trailing symbol
                    int index = 0;

                    for (int i = 0; i < buffer.Length; i++)
                    {
                        if (buffer[i] == escape)
                        {
                            i++;
                        }
                        buffer[index] = buffer[i];
                        index++;
                    }
                    text = new string(buffer, 0, index);
                    this.value = text;
                    break;

                case ValueType.Float:
                    this.value = Convert.ToDouble(text, NumberFormatInfo.InvariantInfo);
                    break;

                case ValueType.Decimal:
                    try
                    {
                        decimal num = Convert.ToDecimal(text, NumberFormatInfo.InvariantInfo);
                        this.value = num;
                    }
                    catch (Exception)
                    {
                        try
                        {
                            double num = Convert.ToDouble(text, NumberFormatInfo.InvariantInfo);
                            this.value = num;
                        }
                        catch (Exception)
                        {
                            this.value = text;
                        }
                    }
                    break;

                case ValueType.Date:
                    this.value = DateTime.Parse(text, CultureInfo.InvariantCulture);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(type.ToString(), "type");
            }
        }

        public object Value
        {
            get 
            {
                return this.value;
            }
        }


        public override string ToString()
        {
            return ("Const(" + this.value + ")");
        }
    }
}