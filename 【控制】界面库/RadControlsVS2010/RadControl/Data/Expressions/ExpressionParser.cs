namespace Telerik.Data.Expressions
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Diagnostics;
	using System.Globalization;

	/**********************************************

	EXPRESSION SYNTAX
		When you create an expression, enclose strings with single quotation marks: 
			"LastName = 'Jones'" 
		
		The following characters are special characters and must be escaped, as explained here, if they are used in a data field name: 
		\n (newline) 
		\t (tab) 
		\r (carriage return) 
		~ 
		(
		) 
		# 
		\ 
		/ 
		= 
		> 
		< 
		+ 
		- 
		* 
		% 
		& 
		| 
		^ 
		' 
		" 
		[ 
		] 
		.
		
		If a data field name contains one of the previous characters, the name must be wrapped in brackets. 
		For example to use a data field named "Field#" in an expression, you would write "[Field#]": 
			Total * [Field#] 
		
		Because brackets are special characters, you must use a slash ("\") to escape the bracket, 
		if it is part of a data field name. For example, a data field named "Field[]" would be written: 
			Total * [Field[\]] (Only the second bracket must be escaped.) 
		
	USER-DEFINED VALUES 
		User-defined values may be used within expressions to be compared with data field values. 
		String values should be enclosed within single quotation marks. 
		Date values should be enclosed within pound signs (#). 
		Decimals and scientific notation are permissible for numeric values. 
		For example: 
			"FirstName = 'John'" 
			"Price <= 50.00" 
			"Birthdate < #1/31/82#" 
		
		For data fields that contain enumeration values, cast the value to an integer data type. For example: 
			"EnumField = 5" 
		
	OPERATORS 
		Concatenation is allowed using Boolean AND, OR, and NOT operators. 
		You can use parentheses to group clauses and force precedence. 
		The AND operator has precedence over other operators. 
		For example: (LastName = 'Smith' OR LastName = 'Jones') AND FirstName = 'John' 
		
		When you create comparison expressions, the following operators are allowed: 
			< 
			> 
			<= 
			>= 
			<> 
			= 
			IN 
			LIKE 
		
		The following arithmetic operators are also supported in expressions: 
			+ (addition) 
			- (subtraction) 
			* (multiplication) 
			/ (division) 
			% (modulus) 
		
	STRING OPERATORS 
		To concatenate a string, use the + character
		
	AGGREGATES 
		The following aggregate types are supported: 
			Sum (Sum) 
			Avg (Average) 
			Min (Minimum) 
			Max (Maximum) 
			Count (Count)
			StDev (Statistical standard deviation) 
			Var (Statistical variance).
			First (First)
			Last (Last)
		
	FUNCTIONS 
		The following functions are also supported: 
		
			NOW() -- Returns the current date/time
			
			IIF(expr, truepart, falsepart) -- Gets one of two values depending on the result of a logical expression.
				Arguments 
					expr -- The expression to evaluate.
					truepart -- The value to return if the expression is true.
					falsepart -- The value to return if the expression is false. 
		 
			ISNULL(expression, replacementValue)  -- Checks an expression and either returns the checked expression or a replacement value. 
				Arguments:
					expression -- The expression to check.
					replacementValue -- If expression is a null reference (Nothing in Visual Basic), replacementvalue is returned. 
		 
			CONVERT(expression, type) -- Converts particular expression to a specified .NET Framework Type
				Arguments: 
					expression -- The expression to convert.
					type -- The .NET Framework type to which the value will be converted. 
					
					All conversions are valid with the following exceptions: Boolean can be coerced to and from Byte, SByte, Int16, Int32, Int64, UInt16, UInt32, UInt64, String and itself only. Char can be coerced to and from Int32, UInt32, String, and itself only. DateTime can be coerced to and from String and itself only. TimeSpan can be coerced to and from String and itself only.
		
			CINT(expression) -- Converts particular expression to System.Int32
			
			CDBL(expression) -- Converts particular expression to System.Double
			
			CBOOL(expression) -- Converts particular expression to System.Boolean
			
			CDATE(expression) -- Converts particular expression to System.DateTime
			
			CSTR(expression) -- Converts particular expression to System.String

	BUILT-IN OBJECTS
		PageNumber -- the number of the current page (System.Int32). Can be used only in the Page Header/Footer sections.
		PageCount -- number of the pages in the document (System.Int32). Can be used only in the Page Header/Footer sections.
			
	MEMBER ACCESS
		To access a member of a object, use the . character:
			Now().ToShortDateString() -- returns a string that containst the current date in short format.
			'abc'.ToUpper() -- returns "ABC" string
			'abc'.Length -- returns the length of the current string - 3
		
		You are allowed to call all public members of one object.
	 
	***********************************************/

	class ExpressionParser : ILexerClient, IDisposable
	{
		static readonly Dictionary<int, ExpressionNode> expressionHive = new Dictionary<int, ExpressionNode>();

		Lexer lexer = null;
		OperandType prevOperand = OperandType.None;
		Stack<ExpressionNode> nodeStack = new Stack<ExpressionNode>(100);
		Stack<OperatorInfo> opStack = new Stack<OperatorInfo>(100);

		public static bool TryParse(string expression, bool caseSensitiveLike, out ExpressionNode expressionNode)
		{
			expressionNode = null;
			string s = ("" + expression).Trim();
			if (s.StartsWith("="))
			{
				s = s.Substring(1);
			}

			int expressionHash = s.GetHashCode() ^ caseSensitiveLike.GetHashCode();
			if (expressionHive.ContainsKey(expressionHash))
			{
				expressionNode = expressionHive[expressionHash];
				return true;
			}

			if (String.IsNullOrEmpty(s))
			{
				return false;
			}

			using (ExpressionParser parser = new ExpressionParser(s, caseSensitiveLike))
			{
				if (!parser.TryParse(false, out expressionNode))
				{
					return false;
				}
			}
			expressionHive[expressionHash] = expressionNode;

			return true;
		}

		public static ExpressionNode Parse(string expression, bool caseSensitiveLike)
		{
			string s = ("" + expression).Trim();
			if (s.StartsWith("="))
			{
				s = s.Substring(1);
			}

			int expressionHash = s.GetHashCode() ^ caseSensitiveLike.GetHashCode();
			if (expressionHive.ContainsKey(expressionHash))
			{
				return expressionHive[expressionHash];
			}

			if (string.Empty != s)
			{
				ExpressionNode node = null;
				using (ExpressionParser parser = new ExpressionParser(s, caseSensitiveLike))
				{
					node = parser.Parse();
				}
				expressionHive[expressionHash] = node;
				return node;
			}

			return null;
		}

		OperandType ILexerClient.PrevOperand
		{
			get { return this.prevOperand; }
		}

		private CompareOptions compareFlags;

		ExpressionParser(string text, bool caseSensitiveLike)
		{
			if (caseSensitiveLike)
			{
				this.compareFlags = CompareOptions.None;
			}
			else
			{
				this.compareFlags = CompareOptions.IgnoreWidth | CompareOptions.IgnoreKanaType | CompareOptions.IgnoreCase;
			}

			this.lexer = new Lexer(this, text);
		}

		public void Dispose()
		{
			this.opStack.Clear();
			this.nodeStack.Clear();

			this.lexer = null;
			this.opStack = null;
			this.nodeStack = null;
		}

		void StartParse()
		{
			this.opStack.Push(new OperatorInfo(NodeType.Noop, Operator.Noop));

			this.lexer.StartRead();
		}

		private bool TryParse(bool enableExceptions, out ExpressionNode expressionNode)
		{
			expressionNode = null;
			Token token;
			int parenCount = 0;

			this.StartParse();
			do
			{
				try
				{
					token = this.lexer.Read();
				}
				catch (LexicalException e)
				{
					if (enableExceptions) throw e;
					return false;
				}
				Debug.WriteLine(Utils.TokenToString(token) + " : " + this.lexer.TokenString);

				switch (token)
				{
					case Token.Name:
					case Token.Numeric:
					case Token.Decimal:
					case Token.Float:
					case Token.NumericHex:
					case Token.StringConst:
					case Token.Date:
						if (OperandType.None != this.prevOperand)
						{
							if (enableExceptions) throw ParserException.MissingOperator(this.lexer.TokenString);
							return false;
						}

						/*if (this.opStack.Count > 0)
						{
							OperatorInfo info = this.opStack.Peek();
							if (info.Operator == Operator.In)
							{
								throw ParserException.InWithoutParentheses();
							}
						}*/

						this.prevOperand = OperandType.Scalar;
						this.nodeStack.Push(this.CreateScalarNode(token, this.lexer.TokenString));
						break;

					case Token.ListSeparator:
						{
							if (this.prevOperand == OperandType.None)
							{
								if (enableExceptions) throw ParserException.MissingOperandBefore(",");
								return false;
							}
							this.BuildExpression(3);
							OperatorInfo info = this.opStack.Peek();
							if (info.Type != NodeType.Call)
							{
								if (enableExceptions) throw ParserException.SystaxError();
								return false;
							}
							ExpressionNode arg = this.nodeStack.Pop();
							FunctionNode func = (FunctionNode)this.nodeStack.Pop();
							func.AddArgument(arg);
							this.nodeStack.Push(func);
							this.prevOperand = OperandType.None;
						}
						break;

					case Token.LeftParen:
						parenCount++;
						if (OperandType.None != this.prevOperand)
						{
							this.BuildExpression(22);
							this.prevOperand = OperandType.None;

							ExpressionNode node = this.nodeStack.Peek();
							if (!(node is NameNode))
							{
								if (enableExceptions) throw ParserException.SystaxError();
								return false;
							}

							NameNode node1 = (NameNode)this.nodeStack.Pop();

							ExpressionNode func;
							if (AggregateNode.IsAggregare(node1.Name))
							{
								func = new AggregateNode(node1.Name);
							}
							else
							{
								func = new FunctionNode(node1.Parent, node1.Name);
							}

							this.nodeStack.Push(func);
							this.opStack.Push(new OperatorInfo(NodeType.Call, Operator.Proc));
						}
						else
						{
							OperatorInfo info = this.opStack.Peek();
							if (info.Type != NodeType.Binop || info.Operator != Operator.In)
							{
								this.opStack.Push(new OperatorInfo(NodeType.Paren, Operator.Proc));
							}
							else
							{
								this.nodeStack.Push(new FunctionNode(null, "In"));
								this.opStack.Push(new OperatorInfo(NodeType.Call, Operator.Proc));
							}
						}
						break;

					case Token.RightParen:
						{
							if (this.prevOperand != OperandType.None)
							{
								this.BuildExpression(3);
							}

							if (this.opStack.Count <= 1)
							{
								if (enableExceptions) throw ParserException.TooManyRightParentheses();
								return false;
							}

							OperatorInfo info = this.opStack.Pop();
							if (this.prevOperand == OperandType.None && info.Type != NodeType.Call)
							{
								if (enableExceptions) throw ParserException.MissingOperand(info.Operator.ToString());
								return false;
							}

							if (info.Type == NodeType.Call)
							{
								if (this.prevOperand != OperandType.None)
								{
									ExpressionNode argument = this.nodeStack.Pop();
									FunctionNode func = (FunctionNode)this.nodeStack.Pop();
									func.AddArgument(argument);
									func.Check();

									this.nodeStack.Push(func);
								}
							}
							else
							{
								ExpressionNode node = this.nodeStack.Pop();
								node = new UnaryOpNode(Operator.Noop, node);    // noop -- return value
								this.nodeStack.Push(node);
							}
							this.prevOperand = OperandType.Expr;
							parenCount--;
						}
						break;

					case Token.ZeroOp:
						if (OperandType.None != this.prevOperand)
						{
							if (enableExceptions) throw ParserException.MissingOperator(this.lexer.TokenString);
							return false;
						}
						this.opStack.Push(new OperatorInfo(NodeType.Zop, this.lexer.Operator));
						this.prevOperand = OperandType.Expr;
						break;

					case Token.UnaryOp:
						this.opStack.Push(new OperatorInfo(NodeType.Unop, this.lexer.Operator));
						break;

					case Token.BinaryOp:
						if (OperandType.None != this.prevOperand)
						{
							this.prevOperand = OperandType.None;

							Operator op = this.lexer.Operator;
							NodeType nodeType = NodeType.Binop;

							if (op == Operator.And)
							{
								this.BuildExpression(Operator.BetweenAnd.Priority);   // eat all ops higher than BETWEEN AND: the unary +/-; comparison ops, etc.

								OperatorInfo lastOp = this.opStack.Peek();

								if (op == Operator.And
									&& lastOp.Operator == Operator.Between)
								{
									op = Operator.BetweenAnd;
									nodeType = NodeType.TernaryOp2;
								}
							}

							this.BuildExpression(op.Priority);
							this.opStack.Push(new OperatorInfo(nodeType, op));
						}
						else
						{
							if (Operator.Plus == this.lexer.Operator)
							{
								this.opStack.Push(new OperatorInfo(NodeType.Unop, Operator.UnaryPlus));
							}
							else if (Operator.Minus == this.lexer.Operator)
							{
								this.opStack.Push(new OperatorInfo(NodeType.Unop, Operator.Negative));
							}
							else
							{
								if (enableExceptions) throw ParserException.MissingOperandBefore(this.lexer.Operator.ToString());
								return false;
							}
						}
						break;

					case Token.TernaryOp:
						if (OperandType.None != this.prevOperand)
						{
							Operator op = this.lexer.Operator;

							this.prevOperand = OperandType.None;
							this.BuildExpression(op.Priority);
							this.opStack.Push(new OperatorInfo(NodeType.TernaryOp, op));
						}
						break;

					case Token.Dot:
						{
							ExpressionNode node = this.nodeStack.Peek();
							if (!(node is NameNode)
								&& !(node is FunctionNode)
								&& !(node is ConstNode))
							{
								if (enableExceptions) throw ParserException.UnknownToken(this.lexer.TokenString, this.lexer.StartPos);
								return false;
							}

							try
							{
								token = this.lexer.Read();
							}
							catch (LexicalException e)
							{
								if (enableExceptions) throw e;
								return false;
							}

							if (Token.Name != token)
							{
								if (enableExceptions) throw ParserException.UnknownToken(this.lexer.TokenString, this.lexer.StartPos);
								return false;
							}

							ExpressionNode node1 = this.nodeStack.Pop(); //NameNode node1 = (NameNode)this.nodeStack.Pop();
							//string name = node1.Name + "." + this.lexer.TokenString;
							this.nodeStack.Push(new NameNode(node1, this.lexer.TokenString));
						}
						break;

					case Token.EOF:
						if (OperandType.None != this.prevOperand)
						{
							this.BuildExpression(3);
							if (this.opStack.Count != 1)
							{
								if (enableExceptions) throw ParserException.MissingRightParen();
								return false;
							}
						}
						else if (this.nodeStack.Count > 0)
						{
							if (enableExceptions) throw ParserException.MissingOperand(this.opStack.Peek().Operator.ToString());
							return false;
						}
						break;

					case Token.Parameter:

						try
						{
							token = this.lexer.Read();
						}
						catch (LexicalException e)
						{
							if (enableExceptions) throw e;
							return false;
						}

						if (Token.Name != token)
						{
							if (enableExceptions) throw ParserException.UnknownToken(this.lexer.TokenString, this.lexer.StartPos);
							return false;
						}

						NameNode parametersNode = new NameNode(null, "Parameters");
						this.nodeStack.Push(new NameNode(parametersNode, this.lexer.TokenString));
						this.prevOperand = OperandType.Scalar;
						break;

					default:
						if (enableExceptions) throw ParserException.UnknownToken(this.lexer.TokenString, this.lexer.StartPos);
						return false;
				}
			} while (Token.EOF != token);


			//Debug.Assert(1 == this.nodeStack.Count);

			if (this.nodeStack.Count != 1)
			{
				return false;
			}
			
			expressionNode = this.nodeStack.Peek();

			if (expressionNode == null)
			{
				return false;
			}

			return true;
		}

		private ExpressionNode Parse()
		{
			ExpressionNode node = null;
			TryParse(true, out node);
			return node;
		}

		/*ExpressionNode ParseAggregateArgument(string aggregate)
		{
			Token token = this.lexer.Read();
			if (Token.Name != token 
				&& "*" != this.lexer.TokenString)   // Allow Count(*) where the * is BinaryOp
			{
				throw ParserException.AggregateArgument();
			}

			string columnName = this.lexer.TokenString;
			this.lexer.Read(Token.RightParen);
			return new AggregateNode(aggregate, columnName);
		}*/

		void BuildExpression(int priority)
		{
			ExpressionNode left = null;
			ExpressionNode right = null;

			while (true)
			{
				OperatorInfo info = this.opStack.Peek();
				if (info.Operator.Priority < priority)
					return;    

				this.opStack.Pop();
				switch (info.Type)
				{
				case NodeType.Unop:
					left = null;
					right = this.nodeStack.Pop();
					if (Operator.IsUnary(info.Operator))
					{
						this.nodeStack.Push(new UnaryOpNode(info.Operator, right));
					}
					else
					{
						throw ParserException.UnsupportedOperator(info.Operator.ToString());
					}
					break;

				case NodeType.Binop:
					right = this.nodeStack.Pop();
					left = this.nodeStack.Pop();

					if (Operator.IsBinary(info.Operator))
					{
						this.nodeStack.Push(new BinaryOpNode(info.Operator, left, right));
					}
					else if (Operator.Like == info.Operator)
					{
						this.nodeStack.Push(new LikeNode(info.Operator, left, right, this.compareFlags));
					}
					else
					{
						throw ParserException.UnsupportedOperator(info.Operator.ToString());
					}
					break;

				case NodeType.TernaryOp2:
					OperatorInfo opInfo1 = this.opStack.Pop();
					if (opInfo1.Type != NodeType.TernaryOp)
					{
						throw ParserException.MissingOperator("Ternary operator requires 2 operators");
					}

					if (this.nodeStack.Count < 3)
					{
						throw ParserException.MissingOperand("Ternary operator requires 3 operands");
					}

					ExpressionNode node3 = this.nodeStack.Pop();
					ExpressionNode node2 = this.nodeStack.Pop();
					ExpressionNode node1 = this.nodeStack.Pop();

					this.nodeStack.Push(new TernaryOpNode(opInfo1.Operator
							, node1
							, node2
							, node3));
					break;
				
				case NodeType.Zop:
					this.nodeStack.Push(new ZeroOpNode(info.Operator));
					break;

				case NodeType.UnopSpec:
				case NodeType.BinopSpec:
					return;

				default:
					Debug.WriteLine("BuildExpression skipped: " + info.Operator);
					break;
				}
			}
		}

		ExpressionNode CreateScalarNode(Token token, string str)
		{
			switch (token)
			{
				case Token.Name:
					return new NameNode(null, str);

				case Token.Numeric:
					return new ConstNode(ValueType.Numeric, str);

				case Token.NumericHex:
					return new ConstNode(ValueType.Numeric, str.Substring(2));

				case Token.Decimal:
					return new ConstNode(ValueType.Decimal, str);

				case Token.Float:
					return new ConstNode(ValueType.Float, str);

				case Token.StringConst:
					 // string is passed with the leading and trailing ' / "
					 return new ConstNode(ValueType.String, str);

				case Token.Date:
					str = str.Substring(1, str.Length - 2); // Discard the leading/trailing #
					return new ConstNode(ValueType.Date, str);   

				default:
					throw new ArgumentException("Unexpected token: " + Utils.TokenToString(token));
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		enum NodeType
		{
			Noop,
			Unop,
			UnopSpec,
			Binop,
			BinopSpec,
			Zop,
			Call,
			Const,
			Name,
			Paren,
			Conv,
			TernaryOp,
			TernaryOp2 // ternary operator: operant1 op operant2 op2 operand3
		}

		/// <summary>
		/// 
		/// </summary>
		[DebuggerDisplay("{Operator.Name} ({Type})")]
		sealed class OperatorInfo
		{
			public readonly NodeType Type;
			public readonly Operator Operator;

			public OperatorInfo(NodeType type, Operator op)
			{
				this.Type = type;
				this.Operator = op;
			}
		}
	}
}