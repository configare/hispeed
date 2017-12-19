using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.AutoLST.Args
{
	class ArgsSchema
	{
		#region " Fields "

		private IDictionary<string, IArgumentMarshaler> marshalers;

		#endregion

		#region " Properties "
		
		internal IDictionary<string, IArgumentMarshaler> Marshalers
		{
			get { return marshalers; }
		}
			
		#endregion

		#region " Constructors "

		public ArgsSchema(string schema)
		{
			marshalers = new Dictionary<string, IArgumentMarshaler>();
			ParseSchema(schema);
		}

		#endregion

		#region " Methods "

		private void ParseSchema(string schema)
		{
			foreach (string element in schema.Split(','))
			{
				if (element.Length > 0)
					ParseSchemaElement(element.Trim());
			}
		}

		private void ParseSchemaElement(string element)
		{
			string elementId = GetElementId(element);

			ValidateSchemaElementId(elementId);

			string elementTail = element.Substring(elementId.Length);

			if (elementTail.Length == 0)
				marshalers.Add(elementId, new BooleanArgumentMarshaler());
			else if (elementTail.Equals("*"))
				marshalers.Add(elementId, new StringArgumentMarshaler());
			else if (elementTail.Equals("#"))
				marshalers.Add(elementId, new IntegerArgumentMarshaler());
			else if (elementTail.Equals("##"))
				marshalers.Add(elementId, new DoubleArgumentMarshaler());
			else if (elementTail.Equals("[*]"))
				marshalers.Add(elementId, new StringArrayArgumentMarshaler());
			else
				throw new ArgsException(ArgsException.ErrorCodes.INVALID_ARGUMENT_FORMAT, elementId, elementTail);
		}

		private string GetElementId(string element)
		{
			string elementId = null;
			char[] elements = element.ToCharArray();

			for (int i = 0; i < elements.Length; i++)
				if (char.IsLetter(elements[i])) elementId += elements[i];

			return (elementId != null ? elementId : element);
		}

		private void ValidateSchemaElementId(string elementId)
		{
			if (!char.IsLetter(elementId[0]))
				throw new ArgsException(ArgsException.ErrorCodes.INVALID_ARGUMENT_NAME, elementId, null);
		}

		#endregion

	}
}
