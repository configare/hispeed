using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.Smart.AutoLST.Args
{
	public class ArgsException : Exception
	{
		#region " Fields "

		private ErrorCodes errorCode = ErrorCodes.OK;
		private String errorParameter;
		private String errorArgumentId;

		#endregion

		#region " Properties "
		
		public ErrorCodes ErrorCode
		{
			get { return errorCode; }
			set { errorCode = value; }
		}
		public String ErrorParameter
		{
			get { return errorParameter; }
			set { errorParameter = value; }
		}
		public String ErrorArgumentId
		{
			get { return errorArgumentId; }
			set { errorArgumentId = value; }
		}

		#endregion

		#region " Enums "

		public enum ErrorCodes
		{
			OK,
			INVALID_ARGUMENT_FORMAT,
			UNEXPECTED_ARGUMENT,
			INVALID_ARGUMENT_NAME,
      NO_ARGUMENT_SPECIFIED,
			MISSING_STRING,
			MISSING_INTEGER,
			INVALID_INTEGER,
			MISSING_DOUBLE,
			INVALID_DOUBLE
		}

		#endregion

		#region " Constructors "

		public ArgsException() { }
		public ArgsException(String message) : base(message) { }
		public ArgsException(ErrorCodes errorCode) { this.errorCode = errorCode; }

		public ArgsException(ErrorCodes errorCode, String errorParameter)
		{
			this.errorCode = errorCode;
			this.errorParameter = errorParameter;
		}

		public ArgsException(ErrorCodes errorCode, String errorArgumentId, String errorParameter)
		{
			this.errorCode = errorCode;
			this.errorParameter = errorParameter;
			this.errorArgumentId = errorArgumentId;
		}

		#endregion

		#region " Methods "

		public String ErrorMessage()
		{
			string result = string.Empty;

			switch (errorCode)
			{
				case ErrorCodes.OK:
					result = "Should not get here.";
					break;
				case ErrorCodes.UNEXPECTED_ARGUMENT:
					result = string.Format("Argument -{0} unexpected.", errorArgumentId);
					break;
				case ErrorCodes.MISSING_STRING:
					result = string.Format("Could not find string parameter for -{0}.", errorArgumentId);
					break;
				case ErrorCodes.INVALID_INTEGER:
					result = string.Format("Argument -{0} expects an integer but was '{1}'.", 
						errorArgumentId, errorParameter);
					break;
				case ErrorCodes.MISSING_INTEGER:
					result = string.Format("Could not find integer parameter for -{0}.", errorArgumentId);
					break;
				case ErrorCodes.INVALID_DOUBLE:
					result = string.Format("Argument -{0} expects a double but was '{1}'.",
						errorArgumentId, errorParameter);
					break;
				case ErrorCodes.MISSING_DOUBLE:
					result = string.Format("Could not find double parameter for -{0}.", errorArgumentId);
					break;
				case ErrorCodes.INVALID_ARGUMENT_NAME:
					result = string.Format("'{0}' is not a valid argument name.", errorArgumentId);
					break;
				case ErrorCodes.INVALID_ARGUMENT_FORMAT:
					result = string.Format("{0} is not a valid argument format.", errorParameter);
					break;
        case ErrorCodes.NO_ARGUMENT_SPECIFIED:
          result = "An argument was not specified.";
          break;
			}
			return result;
		}

		#endregion
	}
}
