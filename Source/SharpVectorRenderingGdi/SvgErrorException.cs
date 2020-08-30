using System;
using System.Runtime.Serialization;

namespace SharpVectors.Renderers
{
	[Serializable]
	public class SvgErrorException : Exception
	{
		private SvgErrorException()
		{
		}

		public SvgErrorException(string msg)
			: base(msg)
		{
		}

		public SvgErrorException(string msg, Exception innerException)
			: base(msg, innerException)
		{
		}

		public SvgErrorException(SvgErrorArgs errorArgs)
			: base(errorArgs.Message, errorArgs.Exception)
		{
		}

		protected SvgErrorException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
