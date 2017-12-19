using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.AutoLST.Args
{
	abstract class ArgumentMarshaler<T> : IArgumentMarshaler
	{
		public static T GetValue(IArgumentMarshaler argumentMarshaler)
		{
			ArgumentMarshaler<T> am = argumentMarshaler as ArgumentMarshaler<T>;
			return am != null ? am.Value : default(T);
		}

		protected abstract T Value { get; set; }
		public abstract void Set(IEnumerator<string> currentArgument);
	}
}
