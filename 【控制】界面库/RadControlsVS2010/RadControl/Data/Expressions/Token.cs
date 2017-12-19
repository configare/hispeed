namespace Telerik.Data.Expressions
{
    /// <summary>
    /// 
    /// </summary>
    enum Token
    {
        None,
        Name,
        Numeric,
        Decimal,
        Float,
        NumericHex,
        StringConst,
        Date,
        ListSeparator,
        LeftParen,
        RightParen,
        ZeroOp,
        UnaryOp,
        BinaryOp,
        TernaryOp,
        Child,
        Parent,
        Dot,
        Parameter,
        Unknown,
        EOF
    }
}