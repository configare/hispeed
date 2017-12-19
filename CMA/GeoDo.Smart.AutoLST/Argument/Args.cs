using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.AutoLST.Args
{
	public class Args
	{
		private System.Collections.Generic.ISet<string> argsFound;
		private System.Collections.Generic.IEnumerator<string> currentArgument;
		private ArgsSchema argsSchema;

		public Args(string schema, string[] args)
		{
			argsFound = new HashSet<string>();

			argsSchema = new ArgsSchema(schema);
			ParseArgumentStrings(new List<string>(args));
		}

		private void ParseArgumentStrings(IEnumerable<string> argsList)
		{
			for (currentArgument = argsList.GetEnumerator(); currentArgument.MoveNext(); )
			{
				string argString = currentArgument.Current;

				if (Utils.StringStartsWith(argString, cc.ArgumentSwitches))
				{
					ParseArgumentCharacters(argString.Substring(1));
				}
				else
				{
					break;
				}
			}
		}

		private void ParseArgumentCharacters(string argChar)
		{
			try
			{
				IArgumentMarshaler m = argsSchema.Marshalers[argChar];

				if (m == null)
				{
					throw new ArgsException(ArgsException.ErrorCodes.UNEXPECTED_ARGUMENT, argChar, null);
				}
				else
				{
					argsFound.Add(argChar);
					try
					{
						m.Set(currentArgument);
					}
					catch (ArgsException ex)
					{
						ex.ErrorArgumentId = argChar;
						throw ex;
					}
				}
			}
			catch (KeyNotFoundException ex)
			{
				throw new ArgsException(ArgsException.ErrorCodes.UNEXPECTED_ARGUMENT, argChar, null);
			}
		}

		public bool Has(string arg)
		{
			return argsFound.Contains(arg);
		}

		public int Cardinality()
		{
			return argsFound.Count();
		}

		public bool GetBoolean(string arg)
		{
			return BooleanArgumentMarshaler.GetValue(argsSchema.Marshalers[arg]);
		}

		public string GetString(string arg)
		{
			return StringArgumentMarshaler.GetValue(argsSchema.Marshalers[arg]);
		}

		public int GetInt(string arg)
		{
			return IntegerArgumentMarshaler.GetValue(argsSchema.Marshalers[arg]);
		}

		public double GetDouble(string arg)
		{
			return DoubleArgumentMarshaler.GetValue(argsSchema.Marshalers[arg]);
		}

		public string[] GetStringArray(string arg)
		{
			return StringArrayArgumentMarshaler.GetValue(argsSchema.Marshalers[arg]);
		}

	}
}
