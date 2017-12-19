using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.AutoLST.Args
{
	class DoubleArgumentMarshaler : IArgumentMarshaler
	{
		private Double doubleValue = 0D;

		public void Set(IEnumerator<string> currentArgument)
		{
			String parameter = null;
			try
			{
				currentArgument.MoveNext();
				parameter = currentArgument.Current;
				doubleValue = double.Parse(parameter);
			}
			catch (FormatException ex)
			{
				throw new ArgsException(ArgsException.ErrorCodes.INVALID_DOUBLE, parameter);
			}
			catch (Exception ex)
			{
				throw new ArgsException(ArgsException.ErrorCodes.MISSING_DOUBLE);
			}
		}

		public static double GetValue(IArgumentMarshaler am)
		{
			if (am != null && am is DoubleArgumentMarshaler)
			{
				return ((DoubleArgumentMarshaler)am).doubleValue;
			}
			else
			{
				return 0D;
			}
		}
	}
}
