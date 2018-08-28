using System;
using System.Runtime.Serialization;

namespace SharpVectors.Dom.Svg
{
	public enum SvgExceptionType
	{
		SvgWrongTypeErr, 
		SvgInvalidValueErr, 
		SvgMatrixNotInvertable
	}

	[Serializable]
	public class SvgException : DomException
	{
		private SvgExceptionType _code;

		public SvgException(SvgExceptionType errorCode)
            : this(errorCode, string.Empty, null)
		{			
		}

		public SvgException(SvgExceptionType errorCode, string message)
            : this(errorCode, message, null)
		{
		}

		public SvgException(SvgExceptionType errorCode, string message, Exception innerException)
            : base(message, innerException)
		{
			_code = errorCode;
		}

		protected SvgException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
		{
		}

		public new SvgExceptionType Code
		{
			get
			{
				return _code;
			}
		}
	}
}
