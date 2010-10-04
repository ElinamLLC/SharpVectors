using System;

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
		public SvgException(SvgExceptionType errorCode):this(errorCode, String.Empty, null)
		{
			
		}

		public SvgException(SvgExceptionType errorCode, string message):this(errorCode, message, null)
		{
		}

		public SvgException(SvgExceptionType errorCode, string message, Exception innerException):base(message, innerException)
		{
			code = errorCode;
		}

		protected SvgException ( System.Runtime.Serialization.SerializationInfo info , System.Runtime.Serialization.StreamingContext context ) : base(info, context)
		{
		}

		private SvgExceptionType code;
		public new SvgExceptionType Code
		{
			get
			{
				return code;
			}
		}
	}
}
