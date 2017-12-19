using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.AutoLST.Args
{
    interface IArgumentMarshaler
    {
			void Set(IEnumerator<string> currentArgument);
		}
}
