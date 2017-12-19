using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.AutoLST.Args
{
	class StringArgumentMarshaler : IArgumentMarshaler
	{
		private String stringValue = String.Empty;

		public void Set(IEnumerator<string> currentArgument)
		{
			try
			{
				currentArgument.MoveNext();
				stringValue = currentArgument.Current;
			}
			catch (Exception ex)
			{
				throw new ArgsException(ArgsException.ErrorCodes.MISSING_STRING);
			}
		}
		public static string GetValue(IArgumentMarshaler am)
		{
			if (am != null && am is StringArgumentMarshaler)
			{
				return ((StringArgumentMarshaler)am).stringValue;
			}
			else
			{
				return string.Empty;
			}
		}
	}
}
