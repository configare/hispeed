using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.AutoLST.Args
{
	class StringArrayArgumentMarshaler : IArgumentMarshaler
	{
		private List<String> stringValues = new List<string>();

		public void Set(IEnumerator<string> currentArgument)
		{
			string lastParameterValue = null;
			string argumentValue = null;

			try
			{
				argumentValue = currentArgument.Current;
				currentArgument.MoveNext();
				stringValues.Add(currentArgument.Current);
				lastParameterValue = currentArgument.Current;

				for (; currentArgument.MoveNext(); )
				{
					if (Utils.StringStartsWith(currentArgument.Current, cc.ArgumentSwitches))
					{
						MoveToPrevious(currentArgument, argumentValue, lastParameterValue);
						break;
					}
					else
					{
						stringValues.Add(currentArgument.Current);
						lastParameterValue = currentArgument.Current;
					}
				}

			}
			catch (Exception ex)
			{
				throw new ArgsException(ArgsException.ErrorCodes.MISSING_STRING);
			}
		}

		public static String[] GetValue(IArgumentMarshaler am)
		{
			if (am != null && am is StringArrayArgumentMarshaler)
			{
				return ((StringArrayArgumentMarshaler)am).stringValues.ToArray();
			}
			else
			{
				return new String[] { };
			}
		}

		private void MoveToPrevious(IEnumerator<String> currentArgument, string argumentValue, string lastStringValue)
		{
			currentArgument.Reset();
			while (currentArgument.MoveNext())
			{
				if (currentArgument.Current == argumentValue)
				{
					while(currentArgument.MoveNext())
						if (currentArgument.Current == lastStringValue)
							break;
					break;
				} 
			}
		}

	}
}
