using System;
using System.Runtime.Serialization;

namespace SharpVectors.Dom.Svg
{
	[Serializable]
	public class SvgExternalResourcesRequiredException : DomException
	{
		public SvgExternalResourcesRequiredException()
            : base("", null)
        {
		}

		protected SvgExternalResourcesRequiredException(
            SerializationInfo info, StreamingContext context ) 
            : base(info, context)
		{
		}
	}
}
