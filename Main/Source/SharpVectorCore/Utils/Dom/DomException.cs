using System;
using System.Runtime.Serialization;

namespace SharpVectors.Dom
{
	[Serializable]
	public class DomException : Exception
	{
		private DomExceptionType _exceptionCode;

		private DomException()
		{
		}

		protected DomException(string msg, Exception innerException) 
            : base(msg, innerException)
		{
		}
		
		public DomException(DomExceptionType code) 
            : this(code, string.Empty)
		{
		}
		
		public DomException(DomExceptionType code, string msg) 
            : this(code, msg, null)
		{
		}

		public DomException(DomExceptionType code, string msg, Exception innerException) 
            : base(msg, innerException)
		{
			_exceptionCode = code;
		}

		protected DomException (SerializationInfo info, StreamingContext context ) : base(info, context)
		{
		}

		public DomExceptionType Code
		{
			get {
                return _exceptionCode;
            }
            protected set {
                _exceptionCode = value;
            }
		}
	}
}
