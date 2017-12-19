using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.AutoLST.Args
{
	class IntegerArgumentMarshaler : IArgumentMarshaler
	{
		private int intValue = 0;

		public void Set(IEnumerator<string> currentArgument)
		{
			String parameter = null;
			try
			{
				currentArgument.MoveNext();
				parameter = currentArgument.Current;
				intValue = int.Parse(parameter);
			}
			catch (FormatException ex)
			{
				throw new ArgsException(ArgsException.ErrorCodes.INVALID_INTEGER, parameter);
			}
			catch (Exception ex)
			{
				throw new ArgsException(ArgsException.ErrorCodes.MISSING_INTEGER);
			}
		}

		public static int GetValue(IArgumentMarshaler am)
		{
			if (am != null && am is IntegerArgumentMarshaler)
			{
				return ((IntegerArgumentMarshaler)am).intValue;
			}
			else
			{
				return 0;
			}
		}

	}
}
