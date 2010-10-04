using System;

namespace SharpVectors.Dom
{
	[Serializable]
	public class DomException : Exception
	{
		/*public DomException() : this(DomExceptionType.SVGSHARP_UNDEFINED_ERROR, "Unknown error")
		{
		}*/

		protected DomException(string msg, Exception innerException) : base(msg, innerException)
		{
		}
		
		public DomException(DomExceptionType code) : this(code, String.Empty)
		{
		}
		
		public DomException(DomExceptionType code, string msg) : this(code, msg, null)
		{
		}

		public DomException(DomExceptionType code, string msg, Exception innerException) : base(msg, innerException)
		{
			this.code = code;
		}

		protected DomException ( System.Runtime.Serialization.SerializationInfo info , System.Runtime.Serialization.StreamingContext context ) : base(info, context)
		{
		}

		private DomExceptionType code;
		public DomExceptionType Code
		{
			get{return code;}
		}
	}
}
